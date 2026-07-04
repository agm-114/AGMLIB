using System;
using System.Collections.Generic;
using System.Reflection;
using Game;
using Game.EWar;
using Game.Sensors;
using Mirror;
using Ships;
using Ships.Controls;
using UnityEngine;
using Utility;

public class PassiveCommsSensorComponent : HullComponent, ISensorComponent, IDeltaSensor, ISensor, IJammable, IEWarTarget, IOwned, IHullComponent
{
    [Header("Passive Comms Sensor")]
    [SerializeField]
    private Sector _coverage = (Sector)255;

    [SerializeField]
    private bool _limitByCoverage = false;

    [SerializeField]
    private float _accuracy = 1f;

    [SerializeField]
    private string _stackingSensorID;

    [SerializeField]
    private bool _collectsIntel = false;

    [SerializeField]
    private bool _requireTransmitEnabled = true;

    [SerializeField]
    private bool _jammableByCommsJammers = false;

    [SerializeField]
    private bool _debugLogging = false;

    private SensorContext _context;
    private bool _sensorEnabled = true;
    private ISensorProvider _provider;
    private ReceivedJamming _jammingSources;
    private bool _registeredAsJammableTarget;
    private int _debugAcquireCycles;
    private int _rejectNotComms;
    private int _rejectNotCommunicator;
    private int _rejectTransmitDisabled;
    private int _rejectNotFunctional;
    private int _rejectOwnTrackable;
    private int _rejectNoTrackable;
    private int _rejectCoverage;
    private int _rejectLinecast;
    private int _rejectPassiveCommsSensor;

    private HashSet<SensorTrack> _tracks = new HashSet<SensorTrack>();
    private List<IEWarTarget> _sweptCommsTargets = new List<IEWarTarget>();
    private HashSet<SensorTrackableObject> _sweptTrackables = new HashSet<SensorTrackableObject>();
    private List<NetworkIdentity> _lostTracks = new List<NetworkIdentity>();
    private List<GainedTrack> _gainedTracks = new List<GainedTrack>();

    public IPlayer OwnedBy { get; private set; }

    SensorContext ISensor.Context => _context;

    public bool Enabled => _sensorEnabled;

    protected override bool _operatingConsumingResources => _sensorEnabled;

    public bool IgnoreEMCON => false;

    public bool CanOrderLock => false;

    public bool CanOrderBurnthrough => false;

    public bool CueingOnly => true;

    public bool CollectsIntel => _collectsIntel;

    Transform ISensorComponent.ShipTransform => _provider != null ? _provider.Trans : transform;

    bool IDeltaSensor.IsWorking => base.IsDoingWork;

    public string SensorKey => base.Socket.Key;

    public string StackingSensorID => _stackingSensorID;

    public TeamIdentifier TeamID => OwnedBy?.TeamId ?? TeamIdentifier.None;

    public float MaxRange => 0f;

    public SignatureType SigType => SignatureType.Comms;

    public Vector3 Position => _provider != null ? _provider.ProviderPosition : transform.position;

    SensorType IDeltaSensor.SenseType => SensorType.Passive;

    public bool ShowJammingLOB => false;

    public float JammingLOBAccuracy => 0f;

    bool ISensor.OpticalProvidesVision => false;

    bool ISensor.RawVisual => false;

    SensorCategory ISensorComponent.Category => SensorCategory.Passive;

    public bool AnyJamming => _jammableByCommsJammers && JammingSources.AnyJamming;

    private ReceivedJamming JammingSources => _jammingSources ??= new ReceivedJamming(this);

    public event Action<ISensorComponent> OnSensorActiveChanged;

    public event Action<bool> OnJammedStatusChanged
    {
        add => JammingSources.OnJammedStatusChanged += value;
        remove => JammingSources.OnJammedStatusChanged -= value;
    }

    protected override void Awake()
    {
        base.Awake();
        _jammingSources = new ReceivedJamming(this);
        DebugLog($"Awake coverage={_coverage} limitByCoverage={_limitByCoverage} socket={base.Socket?.Key ?? "<no socket>"} active={isActiveAndEnabled}");
    }

    protected void OnEnable()
    {
        DebugLog($"OnEnable jammable={_jammableByCommsJammers} providerSet={_provider != null}");
        RegisterJammableTarget();
    }

    protected void OnDisable()
    {
        DebugLog("OnDisable");
        UnregisterJammableTarget();
    }

    private void RegisterJammableTarget()
    {
        if (_jammableByCommsJammers && !_registeredAsJammableTarget)
        {
            SignatureRegistry.Register((IEWarTarget)this);
            _registeredAsJammableTarget = true;
            DebugLog("Registered as comms-jammable IEWarTarget");
        }
    }

    private void UnregisterJammableTarget()
    {
        if (_registeredAsJammableTarget)
        {
            SignatureRegistry.Unregister((IEWarTarget)this);
            _registeredAsJammableTarget = false;
            DebugLog("Unregistered as comms-jammable IEWarTarget");
        }
    }

    protected override void OnDestroy()
    {
        UnregisterJammableTarget();
        base.OnDestroy();
        OwnedBy = null;
        _context = null;
        _provider = null;
        _tracks = null;
        DebugLog("OnDestroy");
    }

    public void SetSensorEnabled(bool active)
    {
        DebugLog($"SetSensorEnabled {active}");
        _sensorEnabled = active;
        FireActivityChangedEvent();
        OnSensorActiveChanged?.Invoke(this);
    }

    public void SetOwner(IPlayer owner)
    {
        OwnedBy = owner;
        DebugLog($"SetOwner team={TeamID} ownerNull={owner == null}");
    }

    public void SetProvider(ISensorProvider provider)
    {
        DebugLog($"SetProvider providerNull={provider == null}");
        SensorContext oldContext = _provider?.Context;
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

    private void HandleContextChanged(SensorContext oldContext, SensorContext newContext)
    {
        if (_context != null)
        {
            foreach (SensorTrack track in _tracks)
            {
                _context.RemoveTrack(track);
            }

            _tracks.Clear();
        }

        _context = newContext;
        DebugLog($"ContextChanged oldNull={oldContext == null} newNull={newContext == null} tracksCleared={_tracks?.Count ?? -1}");
    }

    public IFF GetIFF(IPlayer selectingPlayer)
    {
        return IFFExtensions.GetIFF(OwnedBy, selectingPlayer);
    }

    public void MarkNextBurnthroughCycle()
    {
    }

    public SensorDelta? AcquireContacts(Transform shipTransform)
    {
        _sweptCommsTargets.Clear();
        _sweptTrackables.Clear();
        _lostTracks.Clear();
        _gainedTracks.Clear();

        ResetDebugCounters();

        if (_provider == null)
        {
            DebugLog("Acquire skipped: provider is null");
        }
        else if (!base.IsDoingWork || !_sensorEnabled || AnyJamming)
        {
            DebugLog($"Acquire skipped: doingWork={base.IsDoingWork} enabled={_sensorEnabled} anyJamming={AnyJamming} functional={base.IsFunctional}");
        }
        else
        {
            SignatureRegistry.SweepEWarTargets(SignatureType.Comms, IsCommsTarget, CanSeeCommsTarget, _sweptCommsTargets);

            foreach (IEWarTarget target in _sweptCommsTargets)
            {
                if (TryGetTrackable(target, out SensorTrackableObject trackable))
                {
                    _sweptTrackables.Add(trackable);
                }
                else
                {
                    _rejectNoTrackable++;
                }
            }
        }

        foreach (SensorTrack track in _tracks)
        {
            if (track.Trackable is SensorTrackableObject trackedObject && _sweptTrackables.Contains(trackedObject))
            {
                _sweptTrackables.Remove(trackedObject);
            }
            else
            {
                _lostTracks.Add(track.Trackable.NetID);
            }
        }

        foreach (SensorTrackableObject trackable in _sweptTrackables)
        {
            _gainedTracks.Add(new GainedTrack
            {
                TrackID = trackable.GetTrackId(this),
                NetID = trackable.NetID,
            });
        }

        DebugAcquireSummary();

        if (_lostTracks.Count > 0 || _gainedTracks.Count > 0)
        {
            return new SensorDelta
            {
                SensorKey = SensorKey,
                Gains = _gainedTracks.Count > 0 ? _gainedTracks : null,
                Losses = _lostTracks.Count > 0 ? _lostTracks : null,
                LockTarget = null,
            };
        }

        return null;
    }

    public void AcquireWithDelta(SensorDelta delta, IReadOnlyDictionary<NetworkIdentity, SensorTrackableObject> allTrackable)
    {
        if (delta.Gains != null)
        {
            foreach (GainedTrack gain in delta.Gains)
            {
                if (allTrackable.TryGetValue(gain.NetID, out SensorTrackableObject trackable))
                {
                    SensorTrack track = trackable.Acquire(this, gain.TrackID, AcquisitionType.Passive);
                    _tracks.Add(track);
                    DebugLog($"Gain applied trackable={trackable.name} trackId={gain.TrackID}");
                }
            }
        }

        if (delta.Losses == null)
        {
            return;
        }

        foreach (NetworkIdentity netId in delta.Losses)
        {
            if (netId != null && allTrackable.TryGetValue(netId, out SensorTrackableObject trackable))
            {
                trackable.Release(this, AcquisitionType.Passive);
                _tracks.RemoveWhere(x => x.Trackable == trackable);
                DebugLog($"Loss applied trackable={trackable.name}");
            }
        }
    }

    private bool IsCommsTarget(IEWarTarget target)
    {
        if (target.SigType != SignatureType.Comms)
        {
            _rejectNotComms++;
            return false;
        }

        if (target is not Communicator comms)
        {
            _rejectNotCommunicator++;
            return false;
        }

        if (_requireTransmitEnabled)
        {
            if (!comms.TransmitEnabled)
            {
                _rejectTransmitDisabled++;
                return false;
            }

            if (!comms.EnabledAndFunctional)
            {
                _rejectNotFunctional++;
                return false;
            }
        }
        else if (!comms.HasWorkingAntenna)
        {
            _rejectNotFunctional++;
            return false;
        }

        if (TryGetTrackable(target, out SensorTrackableObject trackable) && _provider != null && trackable == _provider.Trackable)
        {
            _rejectOwnTrackable++;
            return false;
        }

        return true;
    }

    private bool CanSeeCommsTarget(IEWarTarget target)
    {
        Vector3 toTarget = _provider.ProviderPosition.To(target.Position);
        Vector3 localDirection = transform.InverseTransformDirection(toTarget).normalized;
        Sector targetSector = localDirection.ClassifySector();
        if (!IsSectorCovered(targetSector))
        {
            _rejectCoverage++;
            DebugLog("Coverage rejected target=" + GetTargetDebugName(target) + " sector=" + targetSector + " coverage=" + _coverage + " limitByCoverage=" + _limitByCoverage);
            return false;
        }

        if (Physics.Linecast(_provider.ProviderPosition, target.Position, out _, 512, QueryTriggerInteraction.Ignore))
        {
            _rejectLinecast++;
            return false;
        }

        return true;
    }

    private static bool TryGetTrackable(IEWarTarget target, out SensorTrackableObject trackable)
    {
        trackable = null;
        if (target is Component component)
        {
            trackable = component.GetComponent<SensorTrackableObject>() ?? component.GetComponentInParent<SensorTrackableObject>() ?? component.transform.root.GetComponent<SensorTrackableObject>() ?? component.transform.root.GetComponentInChildren<SensorTrackableObject>();
        }

        return trackable != null;
    }

    public Collider[] SweepColliders(Vector3 shipPosition)
    {
        return new Collider[0];
    }

    public bool UpdateTrack(ISensorTrackable trackable, out Vector3 position, out Vector3 velocity, CachedCrossSectionData? cachedCS = null)
    {
        Vector3 direction = _provider.ProviderPosition.To(trackable.Position).normalized;
        position = _provider.ProviderPosition + MathHelpers.RandomRayInCone(direction, _accuracy).normalized * 10000f;
        velocity = Vector3.zero;
        return true;
    }

    public override void GetFormattedStats(List<(string, string)> rows, bool full, int groupSize = 1)
    {
        base.GetFormattedStats(rows, full, groupSize);
        rows.Add(("Role", "SIGINT / Comms LOB"));
        rows.Add(("$SHIPSTAT_SIGNATURETYPE", SignatureType.Comms.GetAbbrevWithColor()));
        rows.Add(("Acquisition", "Passive bearing-only track"));
        rows.Add(("Accuracy", $"{_accuracy:n2} $UNIT_DEGREES"));
        rows.Add(("Detection", _requireTransmitEnabled ? "Transmitting communicators" : "Working comms antennas"));
        rows.Add(("Coverage", _limitByCoverage ? _coverage.ToString() : "Omnidirectional"));
        rows.Add(("Comms Jamming", _jammableByCommsJammers ? "Susceptible" : "Immune"));
    }
    protected override ComponentActivity GetFunctionalActivityStatus()
    {
        if (base.IsFunctional && _sensorEnabled)
        {
            return ComponentActivity.Active;
        }

        return base.GetFunctionalActivityStatus();
    }

    public virtual void GetSpatialOverlapSphere(out Vector3 center, out float radius)
    {
        throw new NotImplementedException("Passive comms sensors do not support spatial sensor queries");
    }

    public bool CanTrainOnTarget(Vector3 targetPos)
    {
        Vector3 toTarget = _provider.ProviderPosition.To(targetPos);
        Vector3 localDirection = transform.InverseTransformDirection(toTarget).normalized;
        return IsSectorCovered(localDirection.ClassifySector());
    }

    public void SetLockedTarget(TrackIdentifier? trackId)
    {
    }

    public void AddJammingSource(IJammingSource source)
    {
        if (_jammableByCommsJammers && source is ActiveCommsJammer)
        {
            JammingSources.AddSource(source);
            _context?.AddJammingVolume(this, source);
            DebugLog($"AddJammingSource {source.GetType().Name} any={AnyJamming}");
        }
    }

    public void RemoveJammingSource(IJammingSource source)
    {
        if (_jammableByCommsJammers && source is ActiveCommsJammer)
        {
            _context?.RemoveJammingVolume(this, source);
            JammingSources.RemoveSource(source);
            DebugLog($"RemoveJammingSource {source.GetType().Name} any={AnyJamming}");
        }
    }

    public bool CanJammerHitAperture(Vector3 jammingDirection)
    {
        if (!_jammableByCommsJammers)
        {
            return false;
        }

        Vector3 localDirection = transform.root.InverseTransformDirection(-jammingDirection);
        return IsSectorCovered(localDirection.ClassifySector());
    }

    private void ResetDebugCounters()
    {
        _rejectNotComms = 0;
        _rejectNotCommunicator = 0;
        _rejectTransmitDisabled = 0;
        _rejectNotFunctional = 0;
        _rejectOwnTrackable = 0;
        _rejectNoTrackable = 0;
        _rejectCoverage = 0;
        _rejectLinecast = 0;
        _rejectPassiveCommsSensor = 0;
    }

    private void DebugAcquireSummary()
    {
        if (!_debugLogging)
        {
            return;
        }

        _debugAcquireCycles++;
        if (_debugAcquireCycles % 10 != 1 && _gainedTracks.Count == 0 && _lostTracks.Count == 0)
        {
            return;
        }

        DebugLog($"Acquire summary sweptTargets={_sweptCommsTargets.Count} sweptTrackables={_sweptTrackables.Count} existingTracks={_tracks.Count} gains={_gainedTracks.Count} losses={_lostTracks.Count} rejects=[notComms={_rejectNotComms}, passiveCommsSensor={_rejectPassiveCommsSensor}, notCommunicator={_rejectNotCommunicator}, txDisabled={_rejectTransmitDisabled}, notFunctional={_rejectNotFunctional}, own={_rejectOwnTrackable}, noTrackable={_rejectNoTrackable}, coverage={_rejectCoverage}, linecast={_rejectLinecast}] coverage={_coverage} limitByCoverage={_limitByCoverage} team={TeamID}");
    }

    private bool IsSectorCovered(Sector sector)
    {
        return !_limitByCoverage || (_coverage & sector) != 0;
    }

    private static string GetTargetDebugName(IEWarTarget target)
    {
        return target is Component component ? component.GetType().Name + ":" + component.name : target.GetType().Name;
    }

    private void DebugLog(string message)
    {
        if (_debugLogging)
        {
            Debug.LogWarning($"[PassiveCommsSensor:{name}] {message}");
        }
    }

    public Vector3 GetPredominantJammingDirection()
    {
        return _jammableByCommsJammers ? JammingSources.GetPredominantJammingDirection() : Vector3.zero;
    }
}

static class PassiveCommsSensorTrackHelpers
{
    public static bool HasPassiveCommsSensor(SensorTrack track)
    {
        List<ISensor> passiveSensors = Common.GetVal<List<ISensor>>(track, "_passiveSensors");
        return passiveSensors != null && passiveSensors.Any(sensor => sensor is PassiveCommsSensorComponent);
    }

    public static bool HasPassiveCommsSensor(IBoardPiece piece)
    {
        if (piece is SensorTrack track)
        {
            return HasPassiveCommsSensor(track);
        }

        IBoardPiece primaryPiece = Common.GetVal<IBoardPiece>(piece, "_primaryPiece");
        if (primaryPiece != null && HasPassiveCommsSensor(primaryPiece))
        {
            return true;
        }

        IEnumerable<IBoardPiece> pieces = Common.GetVal<IEnumerable<IBoardPiece>>(piece, "_pieces");
        return pieces != null && pieces.Any(HasPassiveCommsSensor);
    }

    public static bool HasPassiveCommsSensor(object handle)
    {
        NetworkedSensorTrack parent = Common.GetVal<NetworkedSensorTrack>(handle, "_parent");
        List<ITrack> contributors = Common.GetVal<List<ITrack>>(parent, "_contributors");
        return contributors != null && contributors.Any(HasPassiveCommsContributor);
    }

    private static bool HasPassiveCommsContributor(ITrack track)
    {
        if (track is SensorTrack sensorTrack)
        {
            return HasPassiveCommsSensor(sensorTrack);
        }

        return track is IBoardPiece piece && HasPassiveCommsSensor(piece);
    }
}

[HarmonyPatch(typeof(SensorTrack), "Game.UI.IBoardPiece.get_ContactIntelPrefix")]
class PassiveCommsSensorContactIntelPrefix
{
    static void Postfix(SensorTrack __instance, ref string __result)
    {
        if (PassiveCommsSensorTrackHelpers.HasPassiveCommsSensor(__instance))
        {
            __result = "UI_HUD_SIGINT";
        }
    }
}

[HarmonyPatch]
class PassiveCommsSensorNetworkedContactIntelPrefix
{
    static MethodBase TargetMethod() => AccessTools.Method(AccessTools.Inner(typeof(NetworkedSensorTrack), "Handle"), "Game.UI.IBoardPiece.get_ContactIntelPrefix");

    static void Postfix(object __instance, ref string __result)
    {
        if (PassiveCommsSensorTrackHelpers.HasPassiveCommsSensor(__instance))
        {
            __result = "UI_HUD_SIGINT";
        }
    }
}
[HarmonyPatch(typeof(Utility.Localization.LocalizationCore), nameof(Utility.Localization.LocalizationCore.LocalizeToken))]
class PassiveCommsSensorSigintToken
{
    static void Postfix(string token, ref string __result)
    {
        if (token == "UI_HUD_SIGINT")
        {
            __result = "SIGINT";
        }
    }
}


[HarmonyPatch(typeof(SensorTrack), "Game.UI.IBoardPiece.get_ContactIntel")]
class PassiveCommsSensorLobContactIntel
{
    static void Postfix(SensorTrack __instance, ref string __result)
    {
        if (PassiveCommsSensorTrackHelpers.HasPassiveCommsSensor(__instance) && string.IsNullOrEmpty(__result) && (__instance.Mode == TrackingMode.BearingOnly || __instance.Mode == TrackingMode.Ping))
        {
            __result = "DETECTED";
        }
    }
}
[HarmonyPatch]
class PassiveCommsSensorNetworkedContactSymbol
{
    static MethodBase TargetMethod() => AccessTools.Method(AccessTools.Inner(typeof(NetworkedSensorTrack), "Handle"), "Game.UI.IBoardPiece.get_ContactSymbol");

    static void Postfix(object __instance, ref string __result)
    {
        if (PassiveCommsSensorTrackHelpers.HasPassiveCommsSensor(__instance))
        {
            __result = SignatureType.Comms.GetLOBSymbol();
        }
    }
}

[HarmonyPatch(typeof(SensorTrack), "Game.UI.IBoardPiece.get_OverrideColor")]
class PassiveCommsSensorOverrideColor
{
    static void Postfix(SensorTrack __instance, ref Color? __result)
    {
        if (PassiveCommsSensorTrackHelpers.HasPassiveCommsSensor(__instance))
        {
            __result = GameColors.Yellow;
        }
    }
}

[HarmonyPatch]
class PassiveCommsSensorNetworkedOverrideColor
{
    static MethodBase TargetMethod() => AccessTools.Method(AccessTools.Inner(typeof(NetworkedSensorTrack), "Handle"), "Game.UI.IBoardPiece.get_OverrideColor");

    static void Postfix(object __instance, ref Color? __result)
    {
        if (PassiveCommsSensorTrackHelpers.HasPassiveCommsSensor(__instance))
        {
            __result = GameColors.Yellow;
        }
    }
}



