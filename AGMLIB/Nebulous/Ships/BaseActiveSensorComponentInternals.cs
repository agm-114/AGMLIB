using Game.EWar;
using Game.Sensors;
using HarmonyLib;
using Ships;

public static partial class NativeInternalsExtensions
{
    public static BaseActiveSensorComponentInternals Internals(this BaseActiveSensorComponent sensor) => new(sensor);
}

public readonly struct BaseActiveSensorComponentInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<BaseActiveSensorComponent, DeltaSensor<IActiveSignature>> DeltaSensor =
            AccessTools.FieldRefAccess<BaseActiveSensorComponent, DeltaSensor<IActiveSignature>>("_deltaSensor");
        internal static readonly AccessTools.FieldRef<BaseActiveSensorComponent, ReceivedJamming> JammingSources =
            AccessTools.FieldRefAccess<BaseActiveSensorComponent, ReceivedJamming>("_jammingSources");
        internal static readonly AccessTools.FieldRef<BaseActiveSensorComponent, StatValue> RadiatedPower =
            AccessTools.FieldRefAccess<BaseActiveSensorComponent, StatValue>("_statRadiatedPower");
        internal static readonly AccessTools.FieldRef<BaseActiveSensorComponent, StatValue> Gain =
            AccessTools.FieldRefAccess<BaseActiveSensorComponent, StatValue>("_statGain");
        internal static readonly AccessTools.FieldRef<BaseActiveSensorComponent, StatValue> Sensitivity =
            AccessTools.FieldRefAccess<BaseActiveSensorComponent, StatValue>("_statSensitivity");
        internal static readonly AccessTools.FieldRef<BaseActiveSensorComponent, StatValue> Aperture =
            AccessTools.FieldRefAccess<BaseActiveSensorComponent, StatValue>("_statAperture");
        internal static readonly AccessTools.FieldRef<BaseActiveSensorComponent, StatValue> NoiseFiltering =
            AccessTools.FieldRefAccess<BaseActiveSensorComponent, StatValue>("_statNoiseFiltering");
        internal static readonly Func<BaseActiveSensorComponent, ISensorProvider> GetProviderValue =
            AccessTools.MethodDelegate<Func<BaseActiveSensorComponent, ISensorProvider>>(
                AccessTools.PropertyGetter(typeof(BaseActiveSensorComponent), "_provider")
                ?? throw new MissingMemberException(typeof(BaseActiveSensorComponent).FullName, "_provider"));
        internal static readonly AccessTools.FieldRef<BaseActiveSensorComponent, bool> RunSweepCycle =
            AccessTools.FieldRefAccess<BaseActiveSensorComponent, bool>("_runSweepCycle");
        internal static readonly AccessTools.FieldRef<BaseActiveSensorComponent, StatValue> StatBurnthroughPowerMultiplier =
            AccessTools.FieldRefAccess<BaseActiveSensorComponent, StatValue>("_statBurnthroughPowerMultiplier");
        internal static readonly AccessTools.FieldRef<BaseActiveSensorComponent, StatValue> StatMaxRange =
            AccessTools.FieldRefAccess<BaseActiveSensorComponent, StatValue>("_statMaxRange");
    }

    private readonly BaseActiveSensorComponent _sensor;
    internal BaseActiveSensorComponentInternals(BaseActiveSensorComponent sensor)
    {
        _sensor = sensor ?? throw new ArgumentNullException(nameof(sensor));
    }

    public ISensorProvider? Provider => Refs.DeltaSensor(_sensor)?.Provider;
    public ReceivedJamming? JammingSources => Refs.JammingSources(_sensor);
    public float RadiatedPower => Refs.RadiatedPower(_sensor)?.Value ?? 0f;
    public float Gain => Refs.Gain(_sensor)?.Value ?? 0f;
    public float Sensitivity => Refs.Sensitivity(_sensor)?.Value ?? float.PositiveInfinity;
    public float Aperture => Refs.Aperture(_sensor)?.Value ?? 0f;
    public float NoiseFiltering => Refs.NoiseFiltering(_sensor)?.Value ?? 0f;
    public ref DeltaSensor<IActiveSignature> DeltaSensor => ref Refs.DeltaSensor(_sensor);
    public ref ReceivedJamming JammingSourcesRef => ref Refs.JammingSources(_sensor);
    public ISensorProvider ProviderValue => Refs.GetProviderValue(_sensor);
    public ref bool RunSweepCycle => ref Refs.RunSweepCycle(_sensor);
    public ref StatValue StatAperture => ref Refs.Aperture(_sensor);
    public ref StatValue StatBurnthroughPowerMultiplier => ref Refs.StatBurnthroughPowerMultiplier(_sensor);
    public ref StatValue StatGain => ref Refs.Gain(_sensor);
    public ref StatValue StatMaxRange => ref Refs.StatMaxRange(_sensor);
    public ref StatValue StatNoiseFiltering => ref Refs.NoiseFiltering(_sensor);
    public ref StatValue StatRadiatedPower => ref Refs.RadiatedPower(_sensor);
    public ref StatValue StatSensitivity => ref Refs.Sensitivity(_sensor);
}