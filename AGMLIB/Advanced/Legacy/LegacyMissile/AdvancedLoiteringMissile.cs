using System.Reflection;
using Munitions;
using SmallCraft;

public class AdvancedLoiteringMissile : LoiteringMissile
{
    private static readonly FieldInfo ActivationTriggerField = typeof(LoiteringMissile).GetField(
        "_activationTrigger",
        BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo TriggerAttackMethod = typeof(LoiteringMissile).GetMethod(
        "TriggerAttack",
        BindingFlags.Instance | BindingFlags.NonPublic);

    [Header("Advanced Magnetic Fuse")]
    [Tooltip("Enables the replacement proximity and sensor fuse controller.")]
    public bool MagneticFuse = true;

    [Tooltip("Maximum distance at which a physical target can activate the munition.")]
    [Min(0f)]
    [SerializeField]
    private float _magneticFuseRange = 1000f;

    [Tooltip("Allows hostile ships to activate the magnetic fuse.")]
    public bool ProximityShips = true;

    [Tooltip("Allows hostile fighters and other small craft to activate the magnetic fuse.")]
    public bool ProximityFighters = true;

    [Tooltip("Allows hostile missiles to activate the magnetic fuse.")]
    public bool ProximityMissiles = false;

    [Tooltip("Legacy missile seekers that may activate the munition when they acquire a target.")]
    public List<MissileSeeker> SensorFuses = new();

    [Tooltip("Requires an unobstructed line between the munition and a physical proximity target.")]
    [SerializeField]
    private bool _requireFuseLineOfSight = true;

    [Tooltip("Physics layers that obstruct the magnetic fuse.")]
    [SerializeField]
    private LayerMask _fuseObstructionLayers = 512;

    private readonly HashSet<Collider> _fuseContacts = new();
    private SphereCollider _stockActivationTrigger;

    protected override void Awake()
    {
        EnsureActivationTrigger();
        base.Awake();
    }

    protected override string GetDetailTextInternal()
    {
        _stockActivationTrigger ??= ActivationTriggerField?.GetValue(this) as SphereCollider;
        if (_stockActivationTrigger != null)
        {
            return base.GetDetailTextInternal();
        }

        string rangeDetail = _magneticFuseRange > 0f ? $"\nActivation Range: {_magneticFuseRange * 10f:N0}m" : string.Empty;
        return $"{GetWeaponSummary()}\nAdvanced Loitering Munition{rangeDetail}";
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!MagneticFuse && _stockActivationTrigger != null)
        {
            _stockActivationTrigger.enabled = false;
        }

        if (isServer && MagneticFuse && _stockActivationTrigger != null && _stockActivationTrigger.enabled && TryGetFuseTarget(out Vector3 targetPosition))
        {
            TriggerAttack(targetPosition);
        }
    }

    public new void OnTriggerEnter(Collider other)
    {
        if (isServer && MagneticFuse)
        {
            _fuseContacts.Add(other);
        }
    }

    public new void OnTriggerExit(Collider other)
    {
        _fuseContacts.Remove(other);
    }

    protected override void OnImbued(IImbuedObjectSource platform)
    {
        base.OnImbued(platform);
        foreach (MissileSeeker sensor in SensorFuses)
        {
            if (sensor == null)
            {
                continue;
            }

            sensor.SetOwner(platform.OwnedBy);
            sensor.SetLaunchingPlatform(platform);
        }
    }

    protected override void OnUnpooled()
    {
        base.OnUnpooled();
        foreach (MissileSeeker sensor in SensorFuses)
        {
            sensor?.ResetSeeker();
        }
        _fuseContacts.Clear();
    }

    private bool TryGetFuseTarget(out Vector3 targetPosition)
    {
        if (TryGetSensorFuseTarget(out targetPosition))
        {
            return true;
        }

        if (!ProximityShips && !ProximityFighters && !ProximityMissiles)
        {
            targetPosition = default;
            return false;
        }

        _fuseContacts.RemoveWhere(collider => collider == null);
        foreach (Collider contact in _fuseContacts)
        {
            Transform targetRoot = contact.transform.root;
            if (targetRoot == null)
            {
                continue;
            }

            if (ProximityShips)
            {
                ShipController ship = targetRoot.GetComponent<ShipController>();
                if (ship != null && !ship.IsEliminated && IsHostile(ship.OwnedBy) && HasFuseLineOfSight(ship.Position))
                {
                    targetPosition = ship.Position;
                    return true;
                }
            }

            if (ProximityFighters)
            {
                Spacecraft fighter = targetRoot.GetComponent<Spacecraft>();
                if (fighter != null && IsHostile(fighter.OwnedBy) && HasFuseLineOfSight(fighter.Position))
                {
                    targetPosition = fighter.Position;
                    return true;
                }
            }

            if (ProximityMissiles)
            {
                Missile missile = targetRoot.GetComponent<Missile>();
                if (missile != null && missile != this && IsHostile(missile.OwnedBy) && HasFuseLineOfSight(missile.transform.position))
                {
                    targetPosition = missile.transform.position;
                    return true;
                }
            }
        }

        targetPosition = default;
        return false;
    }

    private bool TryGetSensorFuseTarget(out Vector3 targetPosition)
    {
        foreach (MissileSeeker sensor in SensorFuses)
        {
            if (sensor != null && sensor.SearchForTarget(out targetPosition, out _, out _))
            {
                return true;
            }
        }

        targetPosition = default;
        return false;
    }

    private bool IsHostile(IPlayer targetOwner)
    {
        return targetOwner != null && !IFFExtensions.GetIFF(targetOwner, OwnedBy).IsFriendly();
    }

    private bool HasFuseLineOfSight(Vector3 targetPosition)
    {
        return !_requireFuseLineOfSight || !Physics.Linecast(transform.position, targetPosition, _fuseObstructionLayers);
    }

    private void ConfigureActivationTrigger()
    {
        if (_stockActivationTrigger != null)
        {
            _stockActivationTrigger.isTrigger = true;
            if (_magneticFuseRange > 0f)
            {
                _stockActivationTrigger.radius = _magneticFuseRange;
            }
            _stockActivationTrigger.enabled = false;
        }
    }

    private void EnsureActivationTrigger()
    {
        _stockActivationTrigger ??= ActivationTriggerField?.GetValue(this) as SphereCollider;
        if (_stockActivationTrigger == null)
        {
            _stockActivationTrigger = gameObject.AddComponent<SphereCollider>();
            ActivationTriggerField?.SetValue(this, _stockActivationTrigger);
        }
        ConfigureActivationTrigger();
    }

    private void TriggerAttack(Vector3 targetPosition)
    {
        TriggerAttackMethod?.Invoke(this, new object[] { targetPosition, true });
    }
}
