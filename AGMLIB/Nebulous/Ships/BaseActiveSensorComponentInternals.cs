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
}
