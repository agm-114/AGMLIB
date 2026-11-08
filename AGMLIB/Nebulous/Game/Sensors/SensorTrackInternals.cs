using Game.Sensors;

public static partial class NativeInternalsExtensions
{
    public static SensorTrackInternals Internals(this SensorTrack target) => new(target);
}

public readonly struct SensorTrackInternals(SensorTrack target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<SensorTrack, List<ISensor>> PassiveSensors =
            AccessTools.FieldRefAccess<SensorTrack, List<ISensor>>("_passiveSensors");
        internal static readonly AccessTools.FieldRef<SensorTrack, List<ISensor>> PingSensors =
            AccessTools.FieldRefAccess<SensorTrack, List<ISensor>>("_pingSensors");
    }

    public ref List<ISensor> PassiveSensors => ref Refs.PassiveSensors(target);
    public ref List<ISensor> PingSensors => ref Refs.PingSensors(target);
}