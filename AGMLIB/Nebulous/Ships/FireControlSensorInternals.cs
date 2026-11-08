using Ships;

public static partial class NativeInternalsExtensions
{
    public static FireControlSensorInternals Internals(this FireControlSensor target) => new(target);
}

public readonly struct FireControlSensorInternals(FireControlSensor target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<FireControlSensor, float> Fov =
            AccessTools.FieldRefAccess<FireControlSensor, float>("_fov");
        internal static readonly AccessTools.FieldRef<FireControlSensor, StatValue> StatMaxRange =
            AccessTools.FieldRefAccess<FireControlSensor, StatValue>("_statMaxRange");
    }

    public ref float Fov => ref Refs.Fov(target);
    public ref StatValue StatMaxRange => ref Refs.StatMaxRange(target);
}