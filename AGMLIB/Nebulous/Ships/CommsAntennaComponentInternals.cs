using Ships;

public static partial class NativeInternalsExtensions
{
    public static CommsAntennaComponentInternals Internals(this CommsAntennaComponent target) => new(target);
}

public readonly struct CommsAntennaComponentInternals(CommsAntennaComponent target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<CommsAntennaComponent, StatValue> StatTransmitPower =
            AccessTools.FieldRefAccess<CommsAntennaComponent, StatValue>("_statTransmitPower");
    }

    public ref StatValue StatTransmitPower => ref Refs.StatTransmitPower(target);
}