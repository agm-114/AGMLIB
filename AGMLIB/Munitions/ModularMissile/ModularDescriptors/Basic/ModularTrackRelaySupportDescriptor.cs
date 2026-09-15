using Game;
using Game.EWar;
using Game.Sensors;
using Mirror;
using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Descriptors;
using Munitions.ModularMissiles.Descriptors.Support;
using Munitions.ModularMissiles.Runtime;
using Munitions.ModularMissiles.Runtime.Seekers;
using Ships;
using System.Xml;

#pragma warning disable CA1050 // Unity-authored component types remain global for asset compatibility.

[CreateAssetMenu(
    fileName = "New Modular Track Relay Support",
    menuName = "Nebulous/Missiles/Support/Modular Track Relay")]
public sealed class ModularTrackRelaySupportDescriptor : BaseSupportDescriptor, IModular
{
    [Header("Modular Components")]
    [SerializeField]
    private List<ScriptableObject> _modules = new();

    [Header("Track Relay")]
    [Tooltip("How missile-local seeker contacts contribute to the missile's sensor context.")]
    [SerializeField]
    private AcquisitionType _acquisitionType = AcquisitionType.Active;

    [Tooltip("Signature category displayed for tracks contributed by this relay.")]
    [SerializeField]
    private SignatureType _reportedSignatureType = SignatureType.Radar;

    [Tooltip("Allows this relay's tracks to contribute identification work as well as position data.")]
    [SerializeField]
    private bool _collectsIntel = false;

    List<ScriptableObject> IModular.Modules => _modules;

    internal AcquisitionType AcquisitionType => _acquisitionType;

    internal SignatureType ReportedSignatureType => _reportedSignatureType;

    internal bool CollectsIntel => _collectsIntel;

    public override void FinalSetup(ModularMissile missile)
    {
        base.FinalSetup(missile);

        GameObject missileObject = missile.GameObj;
        _ = missileObject.GetComponent<Communicator>() ?? missileObject.AddComponent<Communicator>();
        if (missileObject.GetComponents<ICommsAntenna>().Length == 0)
        {
            missileObject.AddComponent<SimpleCommsAntenna>();
        }

        _ = missileObject.GetComponent<SensorHost>() ?? missileObject.AddComponent<SensorHost>();
        _ = missileObject.GetComponent<MissileTrackRelayHost>() ?? missileObject.AddComponent<MissileTrackRelayHost>();

        missile.AddRuntimeBehaviour<RuntimeMissileTrackRelay>(this);
    }

    public override string GetSummarySegment() => "<color=#7FD7FF>RLY</color>";

    public override string GetDetailSummarySegment() => "MISSILE TRACK RELAY";

    public override string GetFunctionalDescriptionSegment() =>
        "shares tracks acquired by the missile's onboard seekers with the friendly sensor network";
}

[DisallowMultipleComponent]
public sealed class MissileTrackRelayHost : MonoBehaviour
{
    private ModularMissile? _missile;
    private SensorHost? _sensorHost;
    private Communicator? _communicator;
    private Coroutine? _createContextCoroutine;
    private int _activationVersion;

    private void Awake()
    {
        _missile = GetComponent<ModularMissile>();
        _sensorHost = GetComponent<SensorHost>();
        _communicator = GetComponent<Communicator>();
    }

    public bool Register(RuntimeMissileTrackRelay relay)
    {
        if (!TryResolveComponents())
        {
            Debug.LogWarning($"[AGMLIB Track Relay] Unable to register relay on '{name}' because its missile sensor host is incomplete.");
            return false;
        }

        if (!_communicator!.HasWorkingAntenna)
        {
            _communicator.SetOwnership(_missile!, GetComponents<ICommsAntenna>().ToList());
        }

        _sensorHost!.SetOwner(_missile!, _missile!.MunitionDisplayName);
        _sensorHost.AddSensor(relay);
        EnsureContextAfterLaunch();
        return true;
    }

    public void Unregister(RuntimeMissileTrackRelay relay)
    {
        _activationVersion++;
        if (_createContextCoroutine != null)
        {
            StopCoroutine(_createContextCoroutine);
            _createContextCoroutine = null;
        }

        if (_sensorHost == null)
        {
            return;
        }

        _sensorHost.RemoveSensor(relay);
        if (_sensorHost.SensorCount > 0)
        {
            EnsureContextAfterLaunch();
        }
        else if (NetworkServer.active &&
                 _sensorHost.Context != null &&
                 SensorContextManager.Instance != null)
        {
            SensorContextManager.Instance.DestroyContext(_sensorHost);
        }
    }

    private bool TryResolveComponents()
    {
        _missile ??= GetComponent<ModularMissile>();
        _sensorHost ??= GetComponent<SensorHost>();
        _communicator ??= GetComponent<Communicator>();
        return _missile != null && _sensorHost != null && _communicator != null;
    }

    private void EnsureContextAfterLaunch()
    {
        if (!NetworkServer.active ||
            _sensorHost == null ||
            _sensorHost.Context != null ||
            _createContextCoroutine != null)
        {
            return;
        }

        int activationVersion = ++_activationVersion;
        _createContextCoroutine = StartCoroutine(CreateContextAfterLaunch(activationVersion));
    }

    private IEnumerator CreateContextAfterLaunch(int activationVersion)
    {
        // LaunchInternal sends the missile launch RPC after runtime behaviours return.
        // Waiting one frame ensures clients register their relay before the context RPC arrives.
        yield return null;
        _createContextCoroutine = null;

        if (activationVersion != _activationVersion ||
            !NetworkServer.active ||
            _sensorHost == null ||
            _sensorHost.SensorCount == 0 ||
            _sensorHost.Context != null ||
            SensorContextManager.Instance == null)
        {
            yield break;
        }

        SensorContextManager.Instance.MakeNewContext(_sensorHost);
    }
}

public sealed class RuntimeMissileTrackRelay : RuntimeMissileBehaviour, IDeltaSensor
{
    private const string SensorKeyPrefix = "AGMLIB/MissileTrackRelay";

    [SerializeField]
    private AcquisitionType _acquisitionType = AcquisitionType.Active;

    [SerializeField]
    private SignatureType _reportedSignatureType = SignatureType.Radar;

    [SerializeField]
    private bool _collectsIntel;

    [SerializeField]
    private string _socketId = "Support-0";

    private readonly HashSet<NetworkIdentity> _reportedTargets = new();
    private readonly Dictionary<NetworkIdentity, SensorTrackableObject> _acquiredTargets = new();
    private readonly Dictionary<NetworkIdentity, SensorTrack> _relayTracks = new();

    private RuntimeMissileSeeker[] _seekers = Array.Empty<RuntimeMissileSeeker>();
    private MissileTrackRelayHost? _relayHost;
    private ISensorProvider? _provider;
    private SensorContext? _context;
    private IPlayer? _ownedBy;
    private string? _sensorKey;
    private bool _enabled;
    private bool _registered;

    public string SensorKey => _sensorKey ?? SensorKeyPrefix + "/unregistered";

    public string StackingSensorID => SensorKey;

    public TeamIdentifier TeamID => _ownedBy?.TeamId ?? _provider?.TeamID ?? TeamIdentifier.None;

    public float MaxRange => float.MaxValue;

    public SignatureType SigType => _reportedSignatureType;

    public SensorContext? Context => _context;

    public bool CueingOnly => false;

    public bool CollectsIntel => _collectsIntel;

    public bool ShowJammingLOB => false;

    public float JammingLOBAccuracy => 0f;

    public bool OpticalProvidesVision => false;

    public bool RawVisual => false;

    public bool Enabled => _enabled;

    public SensorType SenseType => _acquisitionType == AcquisitionType.Passive
        ? SensorType.Passive
        : SensorType.Active;

    public bool IsWorking => _enabled && _registered && _context != null;

    public IPlayer? OwnedBy => _ownedBy ?? Missile?.OwnedBy;

    public bool AnyJamming => false;

    SignatureType IEWarTarget.SigType => _reportedSignatureType;

    Vector3 IEWarTarget.Position => transform.position;

    public event Action<bool> OnJammedStatusChanged
    {
        add { }
        remove { }
    }

    public override void OnAdded(ModularMissile missile, MissileComponentDescriptor descriptor)
    {
        base.OnAdded(missile, descriptor);

        if (descriptor is ModularTrackRelaySupportDescriptor relayDescriptor)
        {
            _acquisitionType = relayDescriptor.AcquisitionType;
            _reportedSignatureType = relayDescriptor.ReportedSignatureType;
            _collectsIntel = relayDescriptor.CollectsIntel;
        }

        _socketId = descriptor.Socket?.SocketID ?? _socketId;
        RefreshRuntimeReferences();
    }

    public override void OnCloned()
    {
        base.OnCloned();
        ResetRuntimeState();
        RefreshRuntimeReferences();
    }

    public override void OnUnpooled()
    {
        base.OnUnpooled();
        ResetRuntimeState();
        RefreshRuntimeReferences();
    }

    public override void OnLaunched(ILaunchingPlatform platform, bool forceHotLaunch, bool immediateSearching)
    {
        base.OnLaunched(platform, forceHotLaunch, immediateSearching);
        RefreshRuntimeReferences();

        uint networkId = netId;
        if (networkId == 0 || _relayHost == null)
        {
            Debug.LogWarning($"[AGMLIB Track Relay] Relay on '{name}' launched without a network identity or relay host.");
            return;
        }

        _sensorKey = $"{SensorKeyPrefix}/{networkId}/{_socketId}";
        _ownedBy = Missile?.OwnedBy;
        _enabled = true;
        _registered = _relayHost.Register(this);
    }

    public override void OnDead()
    {
        Deactivate();
        base.OnDead();
    }

    public override void OnRepooled()
    {
        Deactivate();
        base.OnRepooled();
    }

    protected override void OnDestroy()
    {
        Deactivate();
        base.OnDestroy();
    }

    public SensorDelta? AcquireContacts(Transform shipTransform)
    {
        HashSet<NetworkIdentity> detectedTargets = new();
        if (IsWorking)
        {
            foreach (RuntimeMissileSeeker seeker in _seekers)
            {
                if (TryGetMissileLocalTrack(seeker, out SensorTrack sourceTrack) &&
                    sourceTrack.Trackable.NetID != null)
                {
                    detectedTargets.Add(sourceTrack.Trackable.NetID);
                }
            }
        }

        List<GainedTrack>? gains = null;
        foreach (NetworkIdentity detectedTarget in detectedTargets)
        {
            if (_reportedTargets.Contains(detectedTarget))
            {
                continue;
            }

            ISensorTrackable trackable = detectedTarget.GetComponent<ISensorTrackable>();
            if (trackable != null)
            {
                gains ??= new List<GainedTrack>();
                gains.Add(new GainedTrack
                {
                    NetID = detectedTarget,
                    TrackID = trackable.GetTrackId(TeamID)
                });
            }
        }

        List<NetworkIdentity>? losses = null;
        foreach (NetworkIdentity reportedTarget in _reportedTargets)
        {
            if (!detectedTargets.Contains(reportedTarget))
            {
                losses ??= new List<NetworkIdentity>();
                losses.Add(reportedTarget);
            }
        }

        _reportedTargets.Clear();
        _reportedTargets.UnionWith(detectedTargets);

        bool pingCycle = _acquisitionType == AcquisitionType.Ping;
        if (gains == null && losses == null && !(pingCycle && detectedTargets.Count > 0))
        {
            return null;
        }

        return new SensorDelta
        {
            SensorKey = SensorKey,
            Gains = gains,
            Losses = losses,
            LockTarget = null,
            PingCycle = pingCycle,
            DropAllPingResults = false
        };
    }

    public void AcquireWithDelta(
        SensorDelta delta,
        IReadOnlyDictionary<NetworkIdentity, SensorTrackableObject> allTrackable)
    {
        if (delta.Gains != null)
        {
            foreach (GainedTrack gain in delta.Gains)
            {
                if (gain.NetID == null ||
                    !allTrackable.TryGetValue(gain.NetID, out SensorTrackableObject trackable))
                {
                    continue;
                }

                SensorTrack track = trackable.Acquire(this, gain.TrackID, _acquisitionType);
                _acquiredTargets[gain.NetID] = trackable;
                _relayTracks[gain.NetID] = track;
            }
        }

        if (delta.Losses != null)
        {
            foreach (NetworkIdentity loss in delta.Losses)
            {
                ReleaseTarget(loss);
            }
        }

        if (delta.PingCycle)
        {
            foreach (SensorTrack track in _relayTracks.Values)
            {
                track.UpdatePing();
            }
        }
    }

    public bool UpdateTrack(
        ISensorTrackable trackable,
        out Vector3 position,
        out Vector3 velocity,
        CachedCrossSectionData? cachedCS = null)
    {
        SensorTrack? sourceTrack = FindBestMissileLocalTrack(trackable);
        if (sourceTrack == null)
        {
            position = Vector3.zero;
            velocity = Vector3.zero;
            return false;
        }

        position = sourceTrack.KnownPosition;
        velocity = sourceTrack.AbsoluteVelocity;
        return true;
    }

    public void GetSpatialOverlapSphere(out Vector3 center, out float radius)
    {
        center = transform.position;
        radius = 0f;
    }

    public void SetProvider(ISensorProvider provider)
    {
        SensorContext? oldContext = _context;
        if (_provider != null)
        {
            _provider.OnSensorContextChanged -= HandleContextChanged;
        }

        _provider = provider;
        if (_provider != null)
        {
            _provider.OnSensorContextChanged += HandleContextChanged;
        }

        HandleContextChanged(oldContext, _provider?.Context);
    }

    public void SetOwner(IPlayer owner)
    {
        _ownedBy = owner;
    }

    public void SetSensorEnabled(bool enabled)
    {
        _enabled = enabled;
    }

    public IFF GetIFF(IPlayer toPlayer) => IFFExtensions.GetIFF(OwnedBy, toPlayer);

    public void AddJammingSource(IJammingSource source)
    {
    }

    public void RemoveJammingSource(IJammingSource source)
    {
    }

    public bool CanJammerHitAperture(Vector3 jammingDirection) => false;

    public Vector3 GetPredominantJammingDirection() => Vector3.zero;

    protected override void WriteSaveStateInternal(XmlElement self)
    {
    }

    protected override void LoadSaveStateInternal(XmlElement self)
    {
    }

    private void RefreshRuntimeReferences()
    {
        _relayHost = GetComponent<MissileTrackRelayHost>();
        _seekers = GetComponentsInChildren<RuntimeMissileSeeker>();
    }

    private void ResetRuntimeState()
    {
        Deactivate();
        _reportedTargets.Clear();
        _enabled = false;
        _registered = false;
        _sensorKey = null;
        _ownedBy = null;
    }

    private void Deactivate()
    {
        _enabled = false;
        if (_registered && _relayHost != null)
        {
            _relayHost.Unregister(this);
        }
        else
        {
            ReleaseAllAcquisitions();
        }

        _registered = false;
        _reportedTargets.Clear();
        _sensorKey = null;
        _ownedBy = null;
    }

    private void HandleContextChanged(SensorContext? oldContext, SensorContext? newContext)
    {
        if (_context != null)
        {
            ReleaseAllAcquisitions();
        }

        _context = newContext;
    }

    private void ReleaseTarget(NetworkIdentity targetId)
    {
        if (ReferenceEquals(targetId, null) ||
            !_acquiredTargets.TryGetValue(targetId, out SensorTrackableObject trackable))
        {
            return;
        }

        if (trackable != null)
        {
            trackable.Release(this, _acquisitionType);
        }

        _acquiredTargets.Remove(targetId);
        _relayTracks.Remove(targetId);
    }

    private void ReleaseAllAcquisitions()
    {
        foreach (SensorTrackableObject trackable in _acquiredTargets.Values.ToArray())
        {
            if (trackable != null)
            {
                trackable.Release(this, _acquisitionType);
            }
        }

        _acquiredTargets.Clear();
        _relayTracks.Clear();
    }

    private SensorTrack? FindBestMissileLocalTrack(ISensorTrackable trackable)
    {
        SensorTrack? bestTrack = null;
        foreach (RuntimeMissileSeeker seeker in _seekers)
        {
            if (!TryGetMissileLocalTrack(seeker, out SensorTrack sourceTrack) ||
                sourceTrack.Trackable != trackable)
            {
                continue;
            }

            if (bestTrack == null || sourceTrack.Mode > bestTrack.Mode)
            {
                bestTrack = sourceTrack;
            }
        }

        return bestTrack;
    }

    private static bool TryGetMissileLocalTrack(
        RuntimeMissileSeeker seeker,
        out SensorTrack sourceTrack)
    {
        if (seeker.CurrentTargetTrack is SensorTrack candidate &&
            candidate.Context == null &&
            candidate.IsValid &&
            candidate.Trackable != null &&
            !candidate.Trackable.IsDestroyed)
        {
            sourceTrack = candidate;
            return true;
        }

        sourceTrack = null!;
        return false;
    }
}

#pragma warning restore CA1050
