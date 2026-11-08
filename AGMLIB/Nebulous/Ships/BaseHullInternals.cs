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
        internal static readonly AccessTools.FieldRef<BaseHull, GameObject> DamageControlBoardPrefab =
            AccessTools.FieldRefAccess<BaseHull, GameObject>("_damageControlBoardPrefab");
        internal static readonly AccessTools.FieldRef<BaseHull, IArmorSection[]> DynamicArmor =
            AccessTools.FieldRefAccess<BaseHull, IArmorSection[]>("_dynamicArmor");
        internal static readonly AccessTools.FieldRef<BaseHull, Dictionary<string, HullSocket>> SocketLookup =
            AccessTools.FieldRefAccess<BaseHull, Dictionary<string, HullSocket>>("_socketLookup");
    }

    private readonly BaseHull _hull;
    internal BaseHullInternals(BaseHull hull)
    {
        _hull = hull ?? throw new ArgumentNullException(nameof(hull));
    }

    public ref Sprite NormalSilhouette => ref Refs.NormalSilhouette(_hull);
    public ref Sprite HudSilhouette => ref Refs.HudSilhouette(_hull);
    public ref GameObject DamageControlBoardPrefab => ref Refs.DamageControlBoardPrefab(_hull);
    public ref IArmorSection[] DynamicArmor => ref Refs.DynamicArmor(_hull);
    public ref Dictionary<string, HullSocket> SocketLookup => ref Refs.SocketLookup(_hull);
}