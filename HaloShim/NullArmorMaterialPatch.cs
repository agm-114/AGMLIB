extern alias agmlib;

using HarmonyLib;
using Ships;
using UnityEngine;
using Utility;

[HarmonyPatch(typeof(HullComponent), "ApplyArmorUVsForMaterial")]
internal static class NullArmorMaterialPatch
{
    [HarmonyPrefix]
    private static bool Prefix(Material mat)
    {
        return mat != null;
    }
}

[HarmonyPatch(
    typeof(ComponentHullPaintLODShared),
    nameof(ComponentHullPaintLODShared.SetColors)
)]
internal static class NullHullPaintMaterialPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ComponentHullPaintLODShared __instance)
    {
        LODGroupSharedMaterial sharedMaterial =
            __instance.GetComponent<LODGroupSharedMaterial>();
        if (sharedMaterial == null)
            return true;

        if (
            agmlib::NativeInternalsExtensions
                .Internals(__instance)
                .MultipleMaterials
        )
            return true;

        if (sharedMaterial.Material != null)
            return true;

        Debug.LogWarning(
            $"[HaloShim] Skipping hull paint on {__instance.transform.root.name}: "
                + "legacy LOD shared material has no surviving source material."
        );
        return false;
    }
}
