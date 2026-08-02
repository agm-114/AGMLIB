using HarmonyLib;
using Ships;
using UnityEngine;

public static partial class NativeInternalsExtensions
{
    public static BaseHullInternals Internals(this BaseHull hull) => new(hull);
}

public readonly struct BaseHullInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<BaseHull, Sprite> NormalSilhouette =
            AccessTools.FieldRefAccess<BaseHull, Sprite>("_normalSilhouette");

        internal static readonly AccessTools.FieldRef<BaseHull, Sprite> HudSilhouette =
            AccessTools.FieldRefAccess<BaseHull, Sprite>("_hudSilhouette");
    }

    private readonly BaseHull _hull;

    internal BaseHullInternals(BaseHull hull)
    {
        _hull = hull ?? throw new ArgumentNullException(nameof(hull));
    }

    public ref Sprite NormalSilhouette => ref Refs.NormalSilhouette(_hull);

    public ref Sprite HudSilhouette => ref Refs.HudSilhouette(_hull);
}

