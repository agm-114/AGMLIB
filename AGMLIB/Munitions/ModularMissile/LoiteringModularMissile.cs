using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Descriptors;
using Ships;
using Munitions.ModularMissiles.Runtime.Seekers;
using SmallCraft;
using Mirror;
using Mirror.RemoteCalls;
using Utility;
using Game.UI;
using Game.Sensors;
using Game.EWar;
using UnityEngine.Serialization;

public class LoiteringModularMissile : ModularMissile
{
    private const int MissileFuzeLayer = SpecialLayers.ProximityDetection;

    private enum LoiteringFlightPhase
    {
        Inactive,
        Flyout,
        Arming,
        Loitering,
        TurningToTarget,
        Attacking
    }

    [Header("Loitering Munition")]
    [Tooltip("Time spent in guided flyout or push-off before entering the arming state.")]
    [Min(0f)]
    [SerializeField]
    private float _flyoutTime = 5f;

    [Tooltip("Uses the installed engine's pre-ignition positioning thrust during flyout instead of normal missile guidance. This is useful for mines deployed as submunitions.")]
    [SerializeField]
    private bool _usePushoffEngines = true;

    [Tooltip("Displays the current modular stage's maneuvering-thruster particles during push-off. This does not affect the thrust force.")]
    [SerializeField]
    private bool _useManeuveringThrustersForPushoff = true;

    [Tooltip("Optional additional push-off effect played alongside the modular stage's maneuvering-thruster particles.")]
    [SerializeField]
    private DynamicVisibleParticles _pushoffEngineParticles;

    [SerializeField]
    private AudioSource _pushoffEngineLoop;

    [SerializeField]
    private Animation _deployedAnimation;

    [Tooltip("Delay between completing flyout and becoming able to detect targets.")]
    [Min(0f)]
    [SerializeField]
    private float _armingDelay = 2f;

    [Tooltip("Maximum time spent waiting for a target. Zero gives unlimited loiter time.")]
    [Min(0f)]
    [SerializeField]
    private float _loiterLifetime = 120f;

    [Tooltip("Radius applied to a linked SphereCollider. Leave at zero to preserve the collider's authored radius.")]
    [Min(0f)]
    [SerializeField]
    private float _activationRange = 1000f;

    [Tooltip("Any trigger collider used by the magnetic fuse. Sphere colliders use Activation Range as their radius; other collider shapes retain their authored dimensions.")]
    [SerializeField]
    private Collider _activationTrigger;

    [Tooltip("Enables proximity activation while the missile is loitering.")]
    public bool MagneticFuse = true;

    [Tooltip("Allows the magnetic fuse to be triggered by hostile ships.")]
    public bool ProximityShips = true;

    [Tooltip("Allows the magnetic fuse to be triggered by hostile fighters and other small craft.")]
    public bool ProximityFighters = true;

    [Tooltip("Allows the magnetic fuse to be triggered by hostile missiles.")]
    public bool ProximityMissiles = false;

    [Tooltip("When enabled, friendly contacts are ignored only when their comms can reach this mine, matching vanilla mine behavior. When disabled, friendly IFF is always respected.")]
    public bool RequireCommsForIFF = true;

    [Tooltip("One entry per runtime seeker. Enabled entries allow that seeker to trigger the magnetic fuse.")]
    public List<bool> SeekerFuses = new();

    [Tooltip("Applies the seeker-fuse filters below. IFF and the ship, fighter, and missile target-type switches are always respected.")]
    public bool FilterSeekerFuses = false;

    [Tooltip("Prevents enabled seeker fuses from triggering on large, non-point-defense targets.")]
    public bool FilterLargeSeekerFuseTargets = false;

    [Tooltip("Prevents enabled seeker fuses from triggering on small point-defense targets.")]
    public bool FilterSmallSeekerFuseTargets = false;

    [Tooltip("Prevents enabled seeker fuses from triggering on fighter targets.")]
    public bool FilterFighterSeekerFuseTargets = false;

    [Tooltip("Prevents enabled seeker fuses from triggering on missile targets.")]
    public bool FilterMissileSeekerFuseTargets = false;

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

    [SerializeField]
    private Communicator _comms;

    private readonly HashSet<Collider> _fuseContacts = new();
    private LoiteringFlightPhase _phase;
    private float _phaseTime;
    private float _defaultDrag;
    private Vector3 _pushoffDirection;
    private Vector3 _attackDirection;
    private bool _usingPushoffEngines;
    private bool _pushoffEngineOn;

    public bool IsLoitering => _phase == LoiteringFlightPhase.Loitering;

    protected override string _contactIntelPrefix => "UI_HUD_STATUS";

    protected override string _contactIntel
    {
        get
        {
            if (_phase == LoiteringFlightPhase.Arming)
            {
                return "Armed in " +
                    TimeSpan.FromSeconds(_armingDelay - _phaseTime).ToString("mm\\:ss");
            }

            if (_phase == LoiteringFlightPhase.Loitering && _loiterLifetime > 0f)
            {
                return "ARMED - BAT: " +
                    TimeSpan.FromSeconds(_loiterLifetime - _phaseTime).ToString("mm\\:ss");
            }

            return null;
        }
    }

    protected override MarkerType _markerType =>
        OwnedBy != null &&
        OwnedBy.IsOnLocalPlayerTeam &&
        _phase == LoiteringFlightPhase.Loitering
            ? MarkerType.MineActive
            : MarkerType.Mine;

    public override bool SupportsPositionTargeting => true;

    public override bool SupportsTrackTargeting => false;

    static LoiteringModularMissile()
    {
        RemoteCallHelper.RegisterRpcDelegate(
            typeof(LoiteringModularMissile),
            nameof(RpcBeginEngagement),
            InvokeUserCode_RpcBeginEngagement);
    }

    protected override void Awake()
    {
        base.Awake();
        _comms ??= GetComponent<Communicator>();
        if (_comms != null)
        {
            _comms.SetOwnership(this, GetComponents<ICommsAntenna>().ToList());
        }

        _defaultDrag = _body.drag;
        ConfigureActivationTrigger();
        ResetLoiteringState();
    }

    protected override void LaunchInternal(ILaunchingPlatform platform, bool forceHotLaunch, bool immediateSearching)
    {
        base.LaunchInternal(platform, forceHotLaunch, immediateSearching);
        ExtendSeekerFuseList();
        _usingPushoffEngines = _usePushoffEngines && HasPushoffThrust();
        EnterPhase(immediateSearching ? LoiteringFlightPhase.Loitering : LoiteringFlightPhase.Flyout);
    }

    protected override void FixedUpdate()
    {
        _phaseTime += Time.fixedDeltaTime;

        switch (_phase)
        {
            case LoiteringFlightPhase.Flyout:
                if (_usingPushoffEngines)
                {
                    FlightInterface.PointTowards(_pushoffDirection, Vector3.up);
                    ApplyPushoffThrust();
                }
                else
                {
                    base.FixedUpdate();
                }

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
                if (isServer && MagneticFuse && TryGetFuseTarget(out Vector3 targetPosition))
                {
                    TriggerAttack(targetPosition);
                }
                else if (_loiterLifetime > 0f && _phaseTime >= _loiterLifetime && isServer)
                {
                    SelfDestruct();
                }
                break;

            case LoiteringFlightPhase.TurningToTarget:
                FlightInterface.PointTowards(_attackDirection, Vector3.up);
                UpdateThrusters(_attackDirection);
                if (Vector3.Angle(transform.forward, _attackDirection) <= 1f)
                {
                    EnterPhase(LoiteringFlightPhase.Attacking);
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

    public new void OnTriggerEnter(Collider other)
    {
        if (isServer && _phase == LoiteringFlightPhase.Loitering)
        {
            _fuseContacts.Add(other);
            return;
        }

        if (_phase == LoiteringFlightPhase.Attacking)
        {
            base.OnTriggerEnter(other);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (isServer)
        {
            _fuseContacts.Remove(other);
        }
    }

    protected override void OnUnpooled()
    {
        base.OnUnpooled();
        ResetLoiteringState();
        if (_deployedAnimation != null)
        {
            _deployedAnimation.Rewind();
        }
    }

    protected override void OnRepooled()
    {
        ResetLoiteringState();
        base.OnRepooled();
    }

    private void EnterPhase(LoiteringFlightPhase phase)
    {
        bool markerChanged =
            (_phase == LoiteringFlightPhase.Loitering) !=
            (phase == LoiteringFlightPhase.Loitering);

        if (_pushoffEngineOn && phase != LoiteringFlightPhase.Flyout)
        {
            _pushoffEngineOn = false;
            SetPushoffEngineEmission(false);
        }

        _phase = phase;
        _phaseTime = 0f;

        if (phase == LoiteringFlightPhase.Flyout && _usingPushoffEngines)
        {
            _pushoffDirection = transform.forward;
            EngineOff();
            _body.drag = _defaultDrag;
            _pushoffEngineOn = true;
            SetPushoffEngineEmission(true);
        }
        else if (phase is LoiteringFlightPhase.Arming or LoiteringFlightPhase.Loitering)
        {
            EngineOff();
            if (_usingPushoffEngines)
            {
                UpdateThrusters(Vector3.zero);
            }

            _body.drag = _loiterDrag;
            if (_activationTrigger != null)
            {
                _activationTrigger.enabled = phase == LoiteringFlightPhase.Loitering && MagneticFuse;
            }

            if (phase == LoiteringFlightPhase.Loitering && _deployedAnimation != null)
            {
                _deployedAnimation.Play();
            }
        }
        else if (phase == LoiteringFlightPhase.TurningToTarget)
        {
            EngineOff();
            _body.drag = _loiterDrag;
            if (_activationTrigger != null)
            {
                _activationTrigger.enabled = false;
            }
            _fuseContacts.Clear();
        }
        else if (phase == LoiteringFlightPhase.Attacking)
        {
            if (_activationTrigger != null)
            {
                _activationTrigger.enabled = false;
            }
            _fuseContacts.Clear();
            UpdateThrusters(Vector3.zero);
            _body.drag = _defaultDrag;
            EngineOn();
            ResetFlightTime();
        }

        if (markerChanged)
        {
            FireTrackingChangedEvent();
        }
    }

    private bool TryGetFuseTarget(out Vector3 targetPosition)
    {
        return TryGetSelectedSeekerFuseTarget(out targetPosition) || TryGetContactFuseTarget(out targetPosition);
    }

    private bool TryGetContactFuseTarget(out Vector3 targetPosition)
    {
        if (!ProximityShips && !ProximityFighters && !ProximityMissiles)
        {
            targetPosition = default;
            return false;
        }

        _fuseContacts.RemoveWhere(collider => collider == null);
        foreach (Collider contact in _fuseContacts)
        {
            Component target = contact.GetComponentInParent<Missile>();
            target ??= contact.GetComponentInParent<Spacecraft>();
            target ??= contact.GetComponentInParent<ShipController>();
            if (target == null || !HasActivationLineOfSight(target.transform.position))
            {
                continue;
            }

            switch (target)
            {
                case ShipController ship
                    when ProximityShips &&
                    !ship.IsEliminated &&
                    IsFuseIFFTarget(ship):
                    targetPosition = ship.Position;
                    return true;

                case Spacecraft fighter
                    when ProximityFighters &&
                    IsFuseIFFTarget(fighter):
                    targetPosition = fighter.Position;
                    return true;

                case Missile missile
                    when ProximityMissiles &&
                    missile != this &&
                    IsFuseIFFTarget(missile):
                    targetPosition = missile.transform.position;
                    return true;
            }
        }

        targetPosition = default;
        return false;
    }

    private bool TryGetSelectedSeekerFuseTarget(out Vector3 targetPosition)
    {
        if (FilterSeekerFuses &&
            FilterLargeSeekerFuseTargets &&
            FilterSmallSeekerFuseTargets)
        {
            targetPosition = default;
            return false;
        }

        RuntimeMissileSeeker[] seekers = GetComponentsInChildren<RuntimeMissileSeeker>();
        IReadOnlyList<RuntimeMissileSeeker> noValidators = Array.Empty<RuntimeMissileSeeker>();

        foreach (RuntimeMissileSeeker seeker in seekers)
        {
            int socketIndex = seeker.Descriptor.Socket.Index;
            if (socketIndex < 0 || socketIndex >= SeekerFuses.Count || !SeekerFuses[socketIndex])
            {
                continue;
            }

            if (FilterSeekerFuses &&
                FilterLargeSeekerFuseTargets &&
                !FilterSmallSeekerFuseTargets)
            {
                seeker.SetForceDetectPDTargets();
            }

            RuntimeMissileSeeker.SeekerSearchResult result = seeker.SearchForTarget(
                noValidators,
                out targetPosition,
                out _,
                out _);
            if (result == RuntimeMissileSeeker.SeekerSearchResult.Found &&
                IsAllowedSeekerFuseTarget(seeker.CurrentTarget))
            {
                return true;
            }
        }

        targetPosition = default;
        return false;
    }

    private bool IsAllowedSeekerFuseTarget(ISensorTrackable target)
    {
        if (target == null)
        {
            return false;
        }

        Component component = target as Component;
        if (!IsFuseIFFTarget(component, target.GetIFF(OwnedBy)))
        {
            return false;
        }

        if (FilterSeekerFuses &&
            ((FilterLargeSeekerFuseTargets && !target.IsPointDefenseTarget) ||
            (FilterSmallSeekerFuseTargets && target.IsPointDefenseTarget)))
        {
            return false;
        }

        if (component == null)
        {
            return false;
        }

        if (ProximityFighters)
        {
            Spacecraft fighter = component.GetComponentInParent<Spacecraft>();
            if (fighter != null)
            {
                return !FilterSeekerFuses || !FilterFighterSeekerFuseTargets;
            }
        }

        if (ProximityMissiles)
        {
            Missile missile = component.GetComponentInParent<Missile>();
            if (missile != null &&
                missile != this)
            {
                return !FilterSeekerFuses || !FilterMissileSeekerFuseTargets;
            }
        }

        if (ProximityShips)
        {
            ShipController ship = component.GetComponentInParent<ShipController>();
            if (ship != null && !ship.IsEliminated)
            {
                return true;
            }
        }

        return false;
    }

    protected override void HandleVisibilityChanged(bool visible)
    {
        base.HandleVisibilityChanged(visible);
        SetPushoffEngineAudio(visible && _pushoffEngineOn);
    }

    private void SetPushoffEngineEmission(bool active)
    {
        if (active)
        {
            _pushoffEngineParticles?.Play();
        }
        else
        {
            _pushoffEngineParticles?.Stop();
        }

        SetPushoffEngineAudio(active && IsVisible);
    }

    private void SetPushoffEngineAudio(bool active)
    {
        if (active)
        {
            _pushoffEngineLoop?.Play();
        }
        else
        {
            _pushoffEngineLoop?.Stop();
        }
    }

    private bool IsFuseIFFTarget(Component? target, IFF? targetIFF = null)
    {
        IFF resolvedIFF =
            targetIFF ??
            (target as IOwned)?.GetIFF(OwnedBy) ??
            IFF.None;
        if (!resolvedIFF.IsFriendly())
        {
            return true;
        }

        if (!RequireCommsForIFF)
        {
            return false;
        }

        Communicator? targetComms = target?.GetComponentInParent<Communicator>();
        if (_comms == null || targetComms == null)
        {
            return true;
        }

        Communicator.CommsPath broadcastPath = targetComms.GetBroadcastPath();
        return !(broadcastPath.ConnectionOpen && broadcastPath.ReceiverCanReceive(_comms));
    }

    private void TriggerAttack(Vector3 targetPosition)
    {
        Vector3 attackDirection = transform.position.To(targetPosition);
        SetProgramPath(this, new List<Vector3>
        {
            transform.position,
            targetPosition
        });
        BeginEngagement(attackDirection);
        RpcBeginEngagement(attackDirection);
    }

    private void BeginEngagement(Vector3 attackDirection)
    {
        _attackDirection = attackDirection;
        EnterPhase(LoiteringFlightPhase.TurningToTarget);
    }

    private void RpcBeginEngagement(Vector3 attackDirection)
    {
        PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
        writer.WriteVector3(attackDirection);
        SendRPCInternal(typeof(LoiteringModularMissile), nameof(RpcBeginEngagement), writer, 0, true);
        NetworkWriterPool.Recycle(writer);
    }

    private static void InvokeUserCode_RpcBeginEngagement(
        NetworkBehaviour behaviour,
        NetworkReader reader,
        NetworkConnectionToClient senderConnection)
    {
        if (NetworkClient.active)
        {
            ((LoiteringModularMissile)behaviour).UserCode_RpcBeginEngagement(reader.ReadVector3());
        }
    }

    private void UserCode_RpcBeginEngagement(Vector3 attackDirection)
    {
        if (_phase != LoiteringFlightPhase.TurningToTarget &&
            _phase != LoiteringFlightPhase.Attacking)
        {
            BeginEngagement(attackDirection);
        }
    }

    private void ExtendSeekerFuseList()
    {
        while (SeekerFuses.Count < Sockets.Count)
        {
            SeekerFuses.Add(false);
        }
    }

    private bool HasPushoffThrust()
    {
        MissileEngineDescriptor? engine = GetCurrentStageEngine();
        return engine != null && engine.StrafeThrust(inBoostPhase: true) > 0f;
    }

    private void ApplyPushoffThrust()
    {
        MissileEngineDescriptor? engine = GetCurrentStageEngine();
        if (engine == null)
        {
            return;
        }

        bool inBoostPhase = InBoostPhase;
        Thrust(
            _pushoffDirection,
            engine.StrafeThrust(inBoostPhase),
            Mathf.Max(engine.StrafeSpeed(inBoostPhase), _launchedFromSpeed * 1.25f),
            null);

        if (_useManeuveringThrustersForPushoff)
        {
            UpdateThrusters(_pushoffDirection);
        }
    }

    private MissileEngineDescriptor? GetCurrentStageEngine()
    {
        return this.Internals().TryGetCurrentStage(out ModularMissile.Stage stage)
            ? stage.EngineComp
            : null;
    }

    private bool HasActivationLineOfSight(Vector3 targetPosition)
    {
        return !_requireLineOfSight || !Physics.Linecast(transform.position, targetPosition, _obstructionLayers);
    }

    private void ConfigureActivationTrigger()
    {
        if (!MagneticFuse)
        {
            return;
        }
        if (_activationTrigger == null)
        {
            GameObject triggerObject = new("Magnetic Fuse Trigger");
            triggerObject.layer = MissileFuzeLayer;
            triggerObject.transform.SetParent(transform, false);
            _activationTrigger = triggerObject.AddComponent<SphereCollider>();
        }
        else
        {
            if (_activationTrigger.gameObject.layer != MissileFuzeLayer)
            {
                Debug.LogWarning($"Possible bug, activation trigger '{_activationTrigger.name}' is not on the correct layer. Changing to layer {MissileFuzeLayer}.");
            }
            _activationTrigger.gameObject.layer = MissileFuzeLayer;
        }

        _activationTrigger.isTrigger = true;
        if (_activationRange > 0f && _activationTrigger is SphereCollider sphereTrigger)
        {
            sphereTrigger.radius = _activationRange;
        }
        _activationTrigger.enabled = false;
    }

    private void ResetLoiteringState()
    {
        _phase = LoiteringFlightPhase.Inactive;
        _phaseTime = 0f;
        _pushoffDirection = Vector3.zero;
        _attackDirection = Vector3.zero;
        _usingPushoffEngines = false;
        _pushoffEngineOn = false;
        SetPushoffEngineEmission(false);
        _fuseContacts.Clear();
        if (_activationTrigger != null)
        {
            _activationTrigger.enabled = false;
        }
        if (_body != null)
        {
            _body.drag = _defaultDrag;
        }
    }
}
