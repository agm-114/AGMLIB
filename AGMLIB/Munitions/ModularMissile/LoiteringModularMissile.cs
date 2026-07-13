using Munitions.ModularMissiles;
using Ships;
using Munitions.ModularMissiles.Runtime.Seekers;
using SmallCraft;

public class LoiteringModularMissile : ModularMissile
{
    private enum LoiteringFlightPhase
    {
        Inactive,
        Flyout,
        Arming,
        Loitering,
        Attacking
    }

    [Header("Loitering Munition")]
    [Tooltip("Time spent following the programmed missile course before entering the loiter state.")]
    [Min(0f)]
    [SerializeField]
    private float _flyoutTime = 5f;

    [Tooltip("Delay between completing flyout and becoming able to detect targets.")]
    [Min(0f)]
    [SerializeField]
    private float _armingDelay = 2f;

    [Tooltip("Maximum time spent waiting for a target. Zero gives unlimited loiter time.")]
    [Min(0f)]
    [SerializeField]
    private float _loiterLifetime = 120f;

    [Tooltip("Range within which a hostile ship wakes the missile.")]
    [Min(0f)]
    [SerializeField]
    private float _activationRange = 1000f;

    [Tooltip("Enables proximity activation while the missile is loitering.")]
    public bool MagneticFuse = true;

    [Tooltip("Allows the magnetic fuse to be triggered by hostile ships.")]
    public bool ProximityShips = true;

    [Tooltip("Allows the magnetic fuse to be triggered by hostile fighters and other small craft.")]
    public bool ProximityFighters = true;

    [Tooltip("Allows the magnetic fuse to be triggered by hostile missiles.")]
    public bool ProximityMissiles = false;

    [Tooltip("One entry per runtime seeker. Enabled entries allow that seeker to trigger the magnetic fuse.")]
    public List<bool> SeekerFuses = new();

    [Tooltip("Physics layers searched for ships. Leave as Everything unless the missile prefab uses a narrower mask.")]
    [SerializeField]
    private LayerMask _activationLayers = ~0;

    [Tooltip("Do not wake for targets hidden behind map geometry.")]
    [SerializeField]
    private bool _requireLineOfSight = true;

    [Tooltip("Layers that obstruct activation line of sight.")]
    [SerializeField]
    private LayerMask _obstructionLayers = 512;

    [Tooltip("Rigidbody drag used while the missile is arming or loitering.")]
    [Min(0f)]
    [SerializeField]
    private float _loiterDrag = 1f;

    private readonly Collider[] _activationResults = new Collider[64];
    private LoiteringFlightPhase _phase;
    private float _phaseTime;
    private float _defaultDrag;

    public bool IsLoitering => _phase == LoiteringFlightPhase.Loitering;

    public override bool SupportsPositionTargeting => true;

    public override bool SupportsTrackTargeting => false;

    protected override void Awake()
    {
        base.Awake();
        _defaultDrag = _body.drag;
        ResetLoiteringState();
    }

    protected override void LaunchInternal(ILaunchingPlatform platform, bool forceHotLaunch, bool immediateSearching)
    {
        base.LaunchInternal(platform, forceHotLaunch, immediateSearching);
        ExtendSeekerFuseList();
        EnterPhase(immediateSearching ? LoiteringFlightPhase.Attacking : LoiteringFlightPhase.Flyout);
    }

    protected override void FixedUpdate()
    {
        _phaseTime += Time.fixedDeltaTime;

        switch (_phase)
        {
            case LoiteringFlightPhase.Flyout:
                base.FixedUpdate();
                if (_phaseTime >= _flyoutTime)
                {
                    EnterPhase(LoiteringFlightPhase.Arming);
                }
                break;

            case LoiteringFlightPhase.Arming:
                if (_phaseTime >= _armingDelay)
                {
                    EnterPhase(LoiteringFlightPhase.Loitering);
                }
                break;

            case LoiteringFlightPhase.Loitering:
                if (MagneticFuse && TryTriggerMagneticFuse())
                {
                    EnterPhase(LoiteringFlightPhase.Attacking);
                }
                else if (_loiterLifetime > 0f && _phaseTime >= _loiterLifetime && isServer)
                {
                    SelfDestruct();
                }
                break;

            case LoiteringFlightPhase.Attacking:
            case LoiteringFlightPhase.Inactive:
            default:
                base.FixedUpdate();
                break;
        }
    }

    protected override bool FuzeActive()
    {
        return _phase == LoiteringFlightPhase.Attacking && base.FuzeActive();
    }

    protected override void OnUnpooled()
    {
        base.OnUnpooled();
        ResetLoiteringState();
    }

    protected override void OnRepooled()
    {
        ResetLoiteringState();
        base.OnRepooled();
    }

    private void EnterPhase(LoiteringFlightPhase phase)
    {
        _phase = phase;
        _phaseTime = 0f;

        if (phase is LoiteringFlightPhase.Arming or LoiteringFlightPhase.Loitering)
        {
            EngineOff();
            _body.drag = _loiterDrag;
        }
        else if (phase == LoiteringFlightPhase.Attacking)
        {
            _body.drag = _defaultDrag;
            EngineOn();
            ResetFlightTime();
        }
    }

    private bool TryTriggerMagneticFuse()
    {
        if (TryTriggerSelectedSeekerFuse())
        {
            return true;
        }

        if (!ProximityShips && !ProximityFighters && !ProximityMissiles)
        {
            return false;
        }

        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            _activationRange,
            _activationResults,
            _activationLayers,
            QueryTriggerInteraction.Collide);

        for (int i = 0; i < count; i++)
        {
            Transform targetRoot = _activationResults[i]?.transform.root;
            if (targetRoot == null)
            {
                continue;
            }

            if (ProximityShips)
            {
                ShipController ship = targetRoot.GetComponent<ShipController>();
                if (ship != null && !ship.IsEliminated && !ship.GetIFF(OwnedBy).IsFriendly() && HasActivationLineOfSight(ship.Position))
                {
                    return true;
                }
            }

            if (ProximityFighters)
            {
                Spacecraft fighter = targetRoot.GetComponent<Spacecraft>();
                if (fighter != null && !fighter.GetIFF(OwnedBy).IsFriendly() && HasActivationLineOfSight(fighter.Position))
                {
                    return true;
                }
            }

            if (ProximityMissiles)
            {
                Missile missile = targetRoot.GetComponent<Missile>();
                if (missile != null && missile != this && !IFFExtensions.GetIFF(missile.OwnedBy, OwnedBy).IsFriendly() && HasActivationLineOfSight(missile.transform.position))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool TryTriggerSelectedSeekerFuse()
    {
        RuntimeMissileSeeker[] seekers = GetComponentsInChildren<RuntimeMissileSeeker>();
        IReadOnlyList<RuntimeMissileSeeker> noValidators = Array.Empty<RuntimeMissileSeeker>();

        foreach (RuntimeMissileSeeker seeker in seekers)
        {
            int socketIndex = seeker.Descriptor.Socket.Index;
            if (socketIndex < 0 || socketIndex >= SeekerFuses.Count || !SeekerFuses[socketIndex])
            {
                continue;
            }

            RuntimeMissileSeeker.SeekerSearchResult result = seeker.SearchForTarget(
                noValidators,
                out _,
                out _,
                out _);
            if (result == RuntimeMissileSeeker.SeekerSearchResult.Found)
            {
                return true;
            }
        }

        return false;
    }

    private void ExtendSeekerFuseList()
    {
        while (SeekerFuses.Count < Sockets.Count)
        {
            SeekerFuses.Add(false);
        }
    }

    private bool HasActivationLineOfSight(Vector3 targetPosition)
    {
        return !_requireLineOfSight || !Physics.Linecast(transform.position, targetPosition, _obstructionLayers);
    }

    private void ResetLoiteringState()
    {
        _phase = LoiteringFlightPhase.Inactive;
        _phaseTime = 0f;
        if (_body != null)
        {
            _body.drag = _defaultDrag;
        }
    }
}
