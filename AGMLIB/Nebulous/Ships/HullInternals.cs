using HarmonyLib;
using Ships;
using UnityEngine;

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

        internal static readonly AccessTools.FieldRef<Hull, Transform> SocketRoot =
            AccessTools.FieldRefAccess<Hull, Transform>("_socketRoot");
    }

    private readonly Hull _hull;

    internal HullInternals(Hull hull)
    {
        _hull = hull ?? throw new ArgumentNullException(nameof(hull));
    }

    public ref HullSegmentBasic[] PaintableMeshes => ref Refs.PaintableMeshes(_hull);

    public ref Transform SocketRoot => ref Refs.SocketRoot(_hull);
}
