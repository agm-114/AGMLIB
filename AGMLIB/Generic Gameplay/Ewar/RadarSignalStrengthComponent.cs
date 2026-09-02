using System.Globalization;
using System.Text;
using Game.EWar;
using Game.Sensors;
using SmallCraft;
using Ships;
using UnityEngine;

/// <summary>
/// Opt-in client-side presentation for the hostile active radars contributing to this ship's
/// native tracked status. Attach it anywhere below a ship controller in an authored prefab.
/// </summary>
[DisallowMultipleComponent]
public sealed class RadarSignalStrengthComponent : MonoBehaviour
{
    private sealed class SensorState
    {
        public string Name = string.Empty;
        public AcquisitionType AcquisitionType;
        public bool Locked;
        public bool HasTrackQuality;
        public int TrackQuality;
        public bool HasSignalMargin;
        public float SignalRatio;
        public float SignalMarginDb;
    }

    private static readonly Dictionary<SensorTrackableObject, RadarSignalStrengthComponent> ActiveComponents = new();

    private readonly Dictionary<ISensor, SensorState> _sensors = new();
    private ShipController? _shipController;
    private SensorTrackableObject? _registeredTrackable;
    private long _revision;

    internal long Revision => _revision;

    private void Awake()
    {
        ResolveShip();
        RegisterTrackable();
    }

    private void OnEnable()
    {
        ResolveShip();
        RegisterTrackable();
    }

    private void Start() => RegisterTrackable();

    private void Update()
    {
        if (_registeredTrackable == null)
        {
            RegisterTrackable();
        }
    }

    private void OnDisable()
    {
        UnregisterTrackable();
        _sensors.Clear();
    }

    private void OnDestroy()
    {
        UnregisterTrackable();
        _sensors.Clear();
    }

    internal static bool TryGet(ISensorTrackable trackable, out RadarSignalStrengthComponent? component)
    {
        component = null;
        if (trackable is not SensorTrackableObject nativeTrackable
            || !ActiveComponents.TryGetValue(nativeTrackable, out RadarSignalStrengthComponent found)
            || found == null
            || !found.isActiveAndEnabled)
        {
            return false;
        }

        component = found;
        return true;
    }

    internal void RecordAcquisition(ISensor sensor, AcquisitionType acquisitionType)
    {
        if (!CanReport(sensor) || (acquisitionType != AcquisitionType.Active && acquisitionType != AcquisitionType.Ping))
        {
            return;
        }

        SensorState state = GetOrCreateState(sensor);
        if (state.AcquisitionType != acquisitionType)
        {
            state.AcquisitionType = acquisitionType;
            MarkChanged();
        }
    }

    internal void RemoveAcquisition(ISensor sensor, AcquisitionType acquisitionType)
    {
        if (_sensors.TryGetValue(sensor, out SensorState state) && state.AcquisitionType == acquisitionType)
        {
            _sensors.Remove(sensor);
            MarkChanged();
        }
    }

    internal void SetLocked(ISensor sensor, bool locked)
    {
        if (!CanReport(sensor))
        {
            return;
        }

        SensorState state = GetOrCreateState(sensor);
        if (state.Locked != locked)
        {
            state.Locked = locked;
            MarkChanged();
        }
    }

    internal void RecordTrackSample(
        ISensor sensor,
        ISensorTrackable trackable,
        Vector3 measuredPosition,
        CachedCrossSectionData? cachedCrossSection)
    {
        if (!CanReport(sensor))
        {
            return;
        }

        SensorState state = GetOrCreateState(sensor);
        state.TrackQuality = SensorMath.CalculateTrackQuality(Vector3.Distance(trackable.Position, measuredPosition));
        state.HasTrackQuality = true;

        RefreshSignalFromSample(sensor, state, trackable, cachedCrossSection);
        MarkChanged();
    }

    public string BuildTooltipText()
    {
        RemoveInvalidSensors();

        StringBuilder builder = new();
        builder.Append("Radar Detection");
        if (_sensors.Count == 0)
        {
            builder.Append("\nNo hostile radar contributors");
            return builder.ToString();
        }

        foreach (KeyValuePair<ISensor, SensorState> entry in _sensors)
        {
            entry.Value.Name = GetSensorName(entry.Key);
            if (_registeredTrackable != null)
            {
                RefreshLiveSignal(entry.Key, entry.Value, _registeredTrackable);
            }
        }

        List<SensorState> states = _sensors.Values
            .OrderByDescending(state => state.Locked)
            .ThenByDescending(state => state.HasSignalMargin ? state.SignalRatio : float.NegativeInfinity)
            .ThenBy(state => state.Name, StringComparer.CurrentCulture)
            .ToList();

        foreach (SensorState state in states)
        {
            builder.Append("\n\n");
            builder.Append(state.Name);
            builder.Append(" — ");
            builder.Append(state.Locked ? "LOCK" : state.AcquisitionType == AcquisitionType.Ping ? "PING" : "TRACK");
            builder.Append("\nSignal: ");
            AppendSignal(builder, state);
            builder.Append("   TQ: ");
            builder.Append(state.HasTrackQuality ? state.TrackQuality.ToString(CultureInfo.CurrentCulture) : "—");
            builder.Append("/15");
        }

        return builder.ToString();
    }

    private void ResolveShip()
    {
        _shipController ??= GetComponentInParent<ShipController>() ?? GetComponentInChildren<ShipController>();
    }

    private void RegisterTrackable()
    {
        ResolveShip();
        SensorTrackableObject? trackable = _shipController?.Trackable as SensorTrackableObject;
        if (trackable == null || _registeredTrackable == trackable)
        {
            return;
        }

        UnregisterTrackable();
        ActiveComponents[trackable] = this;
        _registeredTrackable = trackable;
    }

    private void UnregisterTrackable()
    {
        if (_registeredTrackable != null
            && ActiveComponents.TryGetValue(_registeredTrackable, out RadarSignalStrengthComponent registered)
            && registered == this)
        {
            ActiveComponents.Remove(_registeredTrackable);
        }

        _registeredTrackable = null;
    }

    private bool CanReport(ISensor sensor)
    {
        if (sensor == null || sensor.SigType != SignatureType.Radar)
        {
            return false;
        }

        if (sensor is UnityEngine.Object sensorObject && sensorObject == null)
        {
            return false;
        }

        IPlayer? owner = _shipController?.OwnedBy;
        return owner != null
            && owner.IsOnLocalPlayerTeam
            && sensor.TeamID != TeamIdentifier.None
            && sensor.TeamID != owner.TeamId
            && RadarSignalStrengthMath.Supports(sensor);
    }

    private SensorState GetOrCreateState(ISensor sensor)
    {
        if (_sensors.TryGetValue(sensor, out SensorState state))
        {
            state.Name = GetSensorName(sensor);
            return state;
        }

        state = new SensorState
        {
            Name = GetSensorName(sensor),
            AcquisitionType = AcquisitionType.Active,
        };
        _sensors.Add(sensor, state);
        MarkChanged();
        return state;
    }

    private void RemoveInvalidSensors()
    {
        List<ISensor>? invalid = null;
        foreach (KeyValuePair<ISensor, SensorState> entry in _sensors)
        {
            ISensor sensor = entry.Key;
            if (CanReport(sensor))
            {
                continue;
            }

            invalid ??= new List<ISensor>();
            invalid.Add(sensor);
        }

        if (invalid == null)
        {
            return;
        }

        foreach (ISensor sensor in invalid)
        {
            _sensors.Remove(sensor);
        }

        MarkChanged();
    }

    private static void RefreshSignalFromSample(
        ISensor sensor,
        SensorState state,
        ISensorTrackable trackable,
        CachedCrossSectionData? cachedCrossSection)
    {
        if (RadarSignalStrengthMath.TryCalculate(
                sensor,
                trackable,
                cachedCrossSection,
                out float ratio,
                out float marginDb))
        {
            SetSignal(state, ratio, marginDb);
        }
        else
        {
            state.HasSignalMargin = false;
        }
    }

    private static void RefreshLiveSignal(ISensor sensor, SensorState state, ISensorTrackable trackable)
    {
        if (RadarSignalStrengthMath.TryCalculateLive(sensor, trackable, out float ratio, out float marginDb))
        {
            SetSignal(state, ratio, marginDb);
        }
        else
        {
            state.HasSignalMargin = false;
        }
    }

    private static void SetSignal(SensorState state, float ratio, float marginDb)
    {
        state.HasSignalMargin = true;
        state.SignalRatio = ratio;
        state.SignalMarginDb = marginDb;
    }

    private void MarkChanged() => _revision = unchecked(_revision + 1);

    private string GetSensorName(ISensor sensor)
    {
        string sensorName;
        if (sensor is HullComponent hullComponent)
        {
            sensorName = hullComponent.ShortUIName;
        }
        else if (sensor is ActiveFireControlSensor fireControl)
        {
            HullComponent parentComponent = fireControl.GetComponentInParent<HullComponent>();
            sensorName = parentComponent != null ? parentComponent.ShortUIName : fireControl.gameObject.name;
        }
        else if (sensor is CraftActiveSensor craftSensor)
        {
            string configuredName = craftSensor.Internals().SensorName;
            sensorName = !string.IsNullOrWhiteSpace(configuredName) ? configuredName : craftSensor.gameObject.name;
        }
        else
        {
            sensorName = sensor is Component component ? component.gameObject.name : sensor.GetType().Name;
        }

        if (sensor is Component sensorComponent)
        {
            ShipController emitter = sensorComponent.GetComponentInParent<ShipController>();
            SkirmishPlayer? localOwner = _shipController?.OwnedBy;
            if (emitter?.Trackable is SensorTrackableObject emitterTrackable
                && localOwner != null
                && emitterTrackable.IsAcquiredByTeam(localOwner.TeamId))
            {
                TrackIdentifier identifier = new(emitterTrackable.GetTrackId(localOwner.TeamId));
                return $"{identifier}: {sensorName}";
            }

            return $"Untracked: {sensorName}";
        }

        return sensorName;
    }

    private static void AppendSignal(StringBuilder builder, SensorState state)
    {
        if (!state.HasSignalMargin)
        {
            builder.Append('—');
            return;
        }

        if (float.IsPositiveInfinity(state.SignalRatio))
        {
            builder.Append("unopposed");
            return;
        }

        builder.Append((state.SignalRatio * 100f).ToString("0", CultureInfo.CurrentCulture));
        builder.Append("% (");
        if (state.SignalMarginDb >= 0f)
        {
            builder.Append('+');
        }

        builder.Append(state.SignalMarginDb.ToString("0.0", CultureInfo.CurrentCulture));
        builder.Append(" dB)");
    }
}

internal static class RadarSignalStrengthMath
{
    internal static bool Supports(ISensor sensor) => sensor is BaseActiveSensorComponent
        or ActiveFireControlSensor
        or CraftActiveSensor;

    internal static bool TryCalculate(
        ISensor sensor,
        ISensorTrackable trackable,
        CachedCrossSectionData? cachedCrossSection,
        out float ratio,
        out float marginDb)
    {
        return TryCalculate(
            sensor,
            trackable,
            cachedCrossSection,
            forceFreshCrossSection: false,
            out ratio,
            out marginDb);
    }

    internal static bool TryCalculateLive(
        ISensor sensor,
        ISensorTrackable trackable,
        out float ratio,
        out float marginDb)
    {
        return TryCalculate(
            sensor,
            trackable,
            cachedCrossSection: null,
            forceFreshCrossSection: true,
            out ratio,
            out marginDb);
    }

    private static bool TryCalculate(
        ISensor sensor,
        ISensorTrackable trackable,
        CachedCrossSectionData? cachedCrossSection,
        bool forceFreshCrossSection,
        out float ratio,
        out float marginDb)
    {
        ratio = 0f;
        marginDb = 0f;
        if (trackable[sensor.SigType] is not IActiveSignature signature)
        {
            return false;
        }

        return sensor switch
        {
            BaseActiveSensorComponent activeSensor => TryCalculate(activeSensor, signature, cachedCrossSection, forceFreshCrossSection, out ratio, out marginDb),
            ActiveFireControlSensor fireControl => TryCalculate(fireControl, signature, cachedCrossSection, forceFreshCrossSection, out ratio, out marginDb),
            CraftActiveSensor craftSensor => TryCalculate(craftSensor, signature, cachedCrossSection, forceFreshCrossSection, out ratio, out marginDb),
            _ => false,
        };
    }

    private static bool TryCalculate(
        BaseActiveSensorComponent sensor,
        IActiveSignature signature,
        CachedCrossSectionData? cachedCrossSection,
        bool forceFreshCrossSection,
        out float ratio,
        out float marginDb)
    {
        BaseActiveSensorComponentInternals internals = sensor.Internals();
        return TryCalculateSensitivityThreshold(
            sensor.transform,
            signature,
            cachedCrossSection,
            forceFreshCrossSection,
            internals.Provider,
            internals.JammingSources,
            internals.RadiatedPower,
            internals.Gain,
            internals.Aperture,
            internals.Sensitivity,
            internals.NoiseFiltering,
            out ratio,
            out marginDb);
    }

    private static bool TryCalculate(
        CraftActiveSensor sensor,
        IActiveSignature signature,
        CachedCrossSectionData? cachedCrossSection,
        bool forceFreshCrossSection,
        out float ratio,
        out float marginDb)
    {
        CraftActiveSensorInternals internals = sensor.Internals();
        return TryCalculateSensitivityThreshold(
            sensor.transform,
            signature,
            cachedCrossSection,
            forceFreshCrossSection,
            internals.Provider,
            internals.JammingSources,
            internals.RadiatedPower,
            internals.Gain,
            internals.Aperture,
            internals.Sensitivity,
            0f,
            out ratio,
            out marginDb);
    }

    private static bool TryCalculate(
        ActiveFireControlSensor sensor,
        IActiveSignature signature,
        CachedCrossSectionData? cachedCrossSection,
        bool forceFreshCrossSection,
        out float ratio,
        out float marginDb)
    {
        ActiveFireControlSensorInternals internals = sensor.Internals();
        if (!TryCalculateReceivedPower(
                sensor.transform,
                signature,
                cachedCrossSection,
                forceFreshCrossSection,
                internals.RadiatedPower,
                internals.Gain,
                internals.Aperture,
                out float receivedPower)
            || internals.Provider == null
            || internals.JammingSources == null)
        {
            ratio = 0f;
            marginDb = 0f;
            return false;
        }

        float jamming = internals.JammingSources.AnyJamming
            ? internals.JammingSources.GetTotalJammingPower(signature.Position)
            : 0f;
        float noise = SensorMath.CalculateNoiseLevel(
            internals.Provider.AmbientNoiseLevel,
            jamming,
            internals.Gain,
            internals.Aperture,
            internals.NoiseFiltering);
        float requiredPower = noise * Mathf.Pow(10f, internals.MaintainLockSnr / 10f);
        return CalculateMargin(receivedPower, requiredPower, out ratio, out marginDb);
    }

    private static bool TryCalculateSensitivityThreshold(
        Transform sensorTransform,
        IActiveSignature signature,
        CachedCrossSectionData? cachedCrossSection,
        bool forceFreshCrossSection,
        ISensorProvider? provider,
        ReceivedJamming? jammingSources,
        float radiatedPower,
        float gain,
        float aperture,
        float sensitivityDbm,
        float noiseFiltering,
        out float ratio,
        out float marginDb)
    {
        if (provider == null
            || jammingSources == null
            || !TryCalculateReceivedPower(
                sensorTransform,
                signature,
                cachedCrossSection,
                forceFreshCrossSection,
                radiatedPower,
                gain,
                aperture,
                out float receivedPower))
        {
            ratio = 0f;
            marginDb = 0f;
            return false;
        }

        float jamming = jammingSources.AnyJamming
            ? jammingSources.GetTotalJammingPower(signature.Position)
            : 0f;
        float ambient = provider.AmbientNoiseLevel;
        float filteredNoise = SensorMath.CalculateNoiseLevel(ambient, jamming, gain, aperture, noiseFiltering);
        float sensitivityPower = 0.001f * Mathf.Pow(10f, sensitivityDbm / 10f);
        float requiredPower = Mathf.Max(sensitivityPower, ambient, filteredNoise);
        return CalculateMargin(receivedPower, requiredPower, out ratio, out marginDb);
    }

    private static bool TryCalculateReceivedPower(
        Transform sensorTransform,
        IActiveSignature signature,
        CachedCrossSectionData? cachedCrossSection,
        bool forceFreshCrossSection,
        float radiatedPower,
        float gain,
        float aperture,
        out float receivedPower)
    {
        Vector3 direction = sensorTransform.position.To(signature.Position);
        if (direction.sqrMagnitude <= Mathf.Epsilon)
        {
            receivedPower = 0f;
            return false;
        }

        Vector3 normalizedDirection = direction.normalized;
        float crossSection = forceFreshCrossSection
            ? signature.GetCrossSection(normalizedDirection, ignoreCached: true)
            : cachedCrossSection?.CSSize ?? signature.GetCrossSection(normalizedDirection, ignoreCached: false);
        receivedPower = signature.GetReturnPowerDensity(
            radiatedPower,
            direction.magnitude,
            gain,
            crossSection,
            normalizedDirection) * aperture;
        return !float.IsNaN(receivedPower) && receivedPower >= 0f;
    }

    private static bool CalculateMargin(float receivedPower, float requiredPower, out float ratio, out float marginDb)
    {
        if (requiredPower <= 0f)
        {
            ratio = float.PositiveInfinity;
            marginDb = float.PositiveInfinity;
            return true;
        }

        ratio = receivedPower / requiredPower;
        if (float.IsNaN(ratio) || ratio < 0f)
        {
            marginDb = 0f;
            return false;
        }

        marginDb = ratio > 0f ? 10f * Mathf.Log10(ratio) : float.NegativeInfinity;
        return true;
    }
}

[HarmonyPatch(typeof(SensorTrack), nameof(SensorTrack.AddSensor))]
internal static class RadarSignalStrengthSensorAddedPatch
{
    private static void Postfix(SensorTrack __instance, ISensor sensor, AcquisitionType acqType)
    {
        if (RadarSignalStrengthComponent.TryGet(__instance.Trackable, out RadarSignalStrengthComponent? component))
        {
            component.RecordAcquisition(sensor, acqType);
        }
    }
}

[HarmonyPatch(typeof(SensorTrack), nameof(SensorTrack.RemoveSensor))]
internal static class RadarSignalStrengthSensorRemovedPatch
{
    private static void Postfix(SensorTrack __instance, ISensor sensor, AcquisitionType acqType, bool __result)
    {
        if (__result && RadarSignalStrengthComponent.TryGet(__instance.Trackable, out RadarSignalStrengthComponent? component))
        {
            component.RemoveAcquisition(sensor, acqType);
        }
    }
}

[HarmonyPatch(typeof(SensorTrack), nameof(SensorTrack.AcquireLock))]
internal static class RadarSignalStrengthLockAcquiredPatch
{
    private static void Postfix(SensorTrack __instance, ISensor sensor)
    {
        if (RadarSignalStrengthComponent.TryGet(__instance.Trackable, out RadarSignalStrengthComponent? component))
        {
            component.SetLocked(sensor, locked: true);
        }
    }
}

[HarmonyPatch(typeof(SensorTrack), nameof(SensorTrack.ReleaseLock))]
internal static class RadarSignalStrengthLockReleasedPatch
{
    private static void Postfix(SensorTrack __instance, ISensor sensor)
    {
        if (RadarSignalStrengthComponent.TryGet(__instance.Trackable, out RadarSignalStrengthComponent? component))
        {
            component.SetLocked(sensor, locked: false);
        }
    }
}

[HarmonyPatch(typeof(BaseActiveSensorComponent), nameof(BaseActiveSensorComponent.UpdateTrack))]
internal static class RadarSignalStrengthShipRadarTrackSamplePatch
{
    private static void Postfix(
        BaseActiveSensorComponent __instance,
        ISensorTrackable trackable,
        Vector3 position,
        CachedCrossSectionData? cachedCS,
        bool __result)
    {
        RadarSignalStrengthTrackSampleRecorder.Record(__instance, trackable, position, cachedCS, __result);
    }
}

[HarmonyPatch(typeof(ActiveFireControlSensor), nameof(ActiveFireControlSensor.UpdateTrack))]
internal static class RadarSignalStrengthFireControlTrackSamplePatch
{
    private static void Postfix(
        ActiveFireControlSensor __instance,
        ISensorTrackable trackable,
        Vector3 position,
        CachedCrossSectionData? cachedCS,
        bool __result)
    {
        RadarSignalStrengthTrackSampleRecorder.Record(__instance, trackable, position, cachedCS, __result);
    }
}

[HarmonyPatch(typeof(CraftActiveSensor), nameof(CraftActiveSensor.UpdateTrack))]
internal static class RadarSignalStrengthCraftRadarTrackSamplePatch
{
    private static void Postfix(
        CraftActiveSensor __instance,
        ISensorTrackable trackable,
        Vector3 position,
        CachedCrossSectionData? cachedCS,
        bool __result)
    {
        RadarSignalStrengthTrackSampleRecorder.Record(__instance, trackable, position, cachedCS, __result);
    }
}

internal static class RadarSignalStrengthTrackSampleRecorder
{
    internal static void Record(
        ISensor sensor,
        ISensorTrackable trackable,
        Vector3 position,
        CachedCrossSectionData? cachedCrossSection,
        bool succeeded)
    {
        if (succeeded && RadarSignalStrengthComponent.TryGet(trackable, out RadarSignalStrengthComponent? component))
        {
            component.RecordTrackSample(sensor, trackable, position, cachedCrossSection);
        }
    }
}
