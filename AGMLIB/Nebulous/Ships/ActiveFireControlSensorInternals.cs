using Game.EWar;
using Game.Sensors;
using HarmonyLib;
using Ships;

public static partial class NativeInternalsExtensions
{
    public static ActiveFireControlSensorInternals Internals(this ActiveFireControlSensor sensor) => new(sensor);
}

public readonly struct ActiveFireControlSensorInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<FireControlSensor, ISensorProvider> Provider =
            AccessTools.FieldRefAccess<FireControlSensor, ISensorProvider>("<_provider>k__BackingField");

        internal static readonly AccessTools.FieldRef<FireControlSensor, ReceivedJamming> JammingSources =
            AccessTools.FieldRefAccess<FireControlSensor, ReceivedJamming>("<_jammingSources>k__BackingField");

        internal static readonly AccessTools.FieldRef<ActiveFireControlSensor, StatValue> RadiatedPower =
            AccessTools.FieldRefAccess<ActiveFireControlSensor, StatValue>("_statRadiatedPower");

        internal static readonly AccessTools.FieldRef<ActiveFireControlSensor, StatValue> Gain =
            AccessTools.FieldRefAccess<ActiveFireControlSensor, StatValue>("_statGain");

        internal static readonly AccessTools.FieldRef<ActiveFireControlSensor, StatValue> Aperture =
            AccessTools.FieldRefAccess<ActiveFireControlSensor, StatValue>("_statAperture");

        internal static readonly AccessTools.FieldRef<FireControlSensor, StatValue> NoiseFiltering =
            AccessTools.FieldRefAccess<FireControlSensor, StatValue>("_statNoiseFiltering");

        internal static readonly AccessTools.FieldRef<ActiveFireControlSensor, float> MaintainLockSnr =
            AccessTools.FieldRefAccess<ActiveFireControlSensor, float>("_maintainLockSNR");
    }

    private readonly ActiveFireControlSensor _sensor;

    internal ActiveFireControlSensorInternals(ActiveFireControlSensor sensor)
    {
        _sensor = sensor ?? throw new ArgumentNullException(nameof(sensor));
    }

    public ISensorProvider? Provider => Refs.Provider(_sensor);

    public ReceivedJamming? JammingSources => Refs.JammingSources(_sensor);

    public float RadiatedPower => Refs.RadiatedPower(_sensor)?.Value ?? 0f;

    public float Gain => Refs.Gain(_sensor)?.Value ?? 0f;

    public float Aperture => Refs.Aperture(_sensor)?.Value ?? 0f;

    public float NoiseFiltering => Refs.NoiseFiltering(_sensor)?.Value ?? 0f;

    public float MaintainLockSnr => Refs.MaintainLockSnr(_sensor);
}
