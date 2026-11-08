using Ships;

public static partial class NativeInternalsExtensions
{
    public static DCLockerComponentInternals Internals(this DCLockerComponent target) => new(target);
}

public readonly struct DCLockerComponentInternals(DCLockerComponent target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<DCLockerComponent, int> RestoreCount =
            AccessTools.FieldRefAccess<DCLockerComponent, int>("_restoreCount");
    }

    public ref int RestoreCount => ref Refs.RestoreCount(target);
}