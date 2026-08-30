using Game.EWar;
using Game.Sensors;
using HarmonyLib;
using SmallCraft;

public static partial class NativeInternalsExtensions
{
    public static CraftActiveSensorInternals Internals(this CraftActiveSensor sensor) => new(sensor);
}

public readonly struct CraftActiveSensorInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<CraftActiveSensor, DeltaSensor<IActiveSignature>> DeltaSensor =
            AccessTools.FieldRefAccess<CraftActiveSensor, DeltaSensor<IActiveSignature>>("_deltaSensor");

        internal static readonly AccessTools.FieldRef<CraftBaseSensor, ReceivedJamming> JammingSources =
            AccessTools.FieldRefAccess<CraftBaseSensor, ReceivedJamming>("_jammingSources");

        internal static readonly AccessTools.FieldRef<CraftBaseSensor, string> SensorName =
            AccessTools.FieldRefAccess<CraftBaseSensor, string>("_sensorName");

        internal static readonly AccessTools.FieldRef<CraftActiveSensor, float> RadiatedPower =
            AccessTools.FieldRefAccess<CraftActiveSensor, float>("_radiatedPower");

        internal static readonly AccessTools.FieldRef<CraftActiveSensor, float> Gain =
            AccessTools.FieldRefAccess<CraftActiveSensor, float>("_gain");

        internal static readonly AccessTools.FieldRef<CraftActiveSensor, float> Sensitivity =
            AccessTools.FieldRefAccess<CraftActiveSensor, float>("_sensitivity");

        internal static readonly AccessTools.FieldRef<CraftActiveSensor, float> Aperture =
            AccessTools.FieldRefAccess<CraftActiveSensor, float>("_apertureSize");
    }

    private readonly CraftActiveSensor _sensor;

    internal CraftActiveSensorInternals(CraftActiveSensor sensor)
    {
        _sensor = sensor ?? throw new ArgumentNullException(nameof(sensor));
    }

    public ISensorProvider? Provider => Refs.DeltaSensor(_sensor)?.Provider;

    public ReceivedJamming? JammingSources => Refs.JammingSources(_sensor);

    public string SensorName => Refs.SensorName(_sensor);

    public float RadiatedPower => Refs.RadiatedPower(_sensor);

    public float Gain => Refs.Gain(_sensor);

    public float Sensitivity => Refs.Sensitivity(_sensor);

    public float Aperture => Refs.Aperture(_sensor);
}
