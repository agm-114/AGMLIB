using Ships;

public static partial class NativeInternalsExtensions
{
    public static HullSegmentBasicInternals Internals(this HullSegmentBasic target) => new(target);
}

public readonly struct HullSegmentBasicInternals(HullSegmentBasic target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<HullSegmentBasic, Material> OriginalMaterial =
            AccessTools.FieldRefAccess<HullSegmentBasic, Material>("_originalMaterial");
    }

    public ref Material OriginalMaterial => ref Refs.OriginalMaterial(target);
}