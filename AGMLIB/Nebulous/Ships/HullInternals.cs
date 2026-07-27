using HarmonyLib;
using Ships;

public static partial class NativeInternalsExtensions
{
    public static HullInternals Internals(this Hull hull) => new(hull);
}

public readonly struct HullInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<Hull, HullSegmentBasic[]> PaintableMeshes =
            AccessTools.FieldRefAccess<Hull, HullSegmentBasic[]>("_paintableMeshes");
    }

    private readonly Hull _hull;

    internal HullInternals(Hull hull)
    {
        _hull = hull ?? throw new ArgumentNullException(nameof(hull));
    }

    public ref HullSegmentBasic[] PaintableMeshes => ref Refs.PaintableMeshes(_hull);
}
