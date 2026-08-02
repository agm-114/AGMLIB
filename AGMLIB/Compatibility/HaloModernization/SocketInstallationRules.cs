using System;
using System.Collections.Generic;
using Bundles;
using FleetEditor;
using HarmonyLib;
using Ships;
using UI;
using UnityEngine;

namespace AGMLIB.Compatibility.HaloModernization;

public class ComponentRequiresSocketWhitelist : MonoBehaviour
{
}

public class SocketComponentWhitelist : MonoBehaviour
{
    [SerializeField]
    protected string[] _allowedComponents = [];

    public bool Allows(HullComponent component)
    {
        if (component == null)
            return true;
        return _allowedComponents == null
            || _allowedComponents.Length == 0
            || (
                Array.IndexOf(_allowedComponents, component.ComponentName) >= 0
                || Array.IndexOf(_allowedComponents, component.SaveKey) >= 0
            );
    }
}

public class SocketComponentBlacklist : MonoBehaviour
{
    [SerializeField]
    protected string[] _forbiddenComponents = [];

    public bool Allows(HullComponent component)
    {
        if (component == null)
            return true;
        return _forbiddenComponents == null
            || (
                Array.IndexOf(_forbiddenComponents, component.ComponentName) < 0
                && Array.IndexOf(_forbiddenComponents, component.SaveKey) < 0
            );
    }
}

internal static class SocketInstallationRuleEvaluator
{
    public static bool Allows(HullSocket socket, HullComponent component)
    {
        if (socket == null || component == null)
            return true;

        SocketComponentWhitelist whitelist = socket.GetComponent<SocketComponentWhitelist>();
        bool requiresWhitelist =
            component.GetComponent<ComponentRequiresSocketWhitelist>() != null;
        if (requiresWhitelist)
        {
            // Unity restores the legacy allow-list data into the exact facade type, not its
            // AGMLIB base. The presence of the whitelist still survives and is sufficient
            // to retain the important "special component needs a special socket" rule.
            // Exact per-component filtering can be restored when the old serialized arrays
            // are migrated out of the bundles.
            if (whitelist == null)
                return false;
        }
        else if (whitelist != null && !whitelist.Allows(component))
        {
            return false;
        }

        SocketComponentBlacklist blacklist = socket.GetComponent<SocketComponentBlacklist>();
        return blacklist == null || blacklist.Allows(component);
    }
}

[HarmonyPatch(typeof(HullSocket), nameof(HullSocket.SetComponent))]
internal static class HullSocketInstallationRulePatch
{
    private static bool Prefix(
        HullSocket __instance,
        HullComponent componentPrefab,
        ref HullComponent __result
    )
    {
        if (SocketInstallationRuleEvaluator.Allows(__instance, componentPrefab))
            return true;

        Debug.LogWarning(
            $"Component '{componentPrefab?.SaveKey}' is not allowed in socket '{__instance?.Key}'."
        );
        __result = null;
        return false;
    }
}

[HarmonyPatch(typeof(ComponentPalette), nameof(ComponentPalette.GetItemsForSocket))]
internal static class ComponentPaletteInstallationRulePatch
{
    private static void Postfix(
        ComponentPalette __instance,
        HullSocket socket,
        ref List<SelectableListItem> __result
    )
    {
        if (socket == null || __result == null)
            return;

        __result.RemoveAll(
            item =>
                item is PaletteItem paletteItem
                && paletteItem.Component != null
                && !SocketInstallationRuleEvaluator.Allows(socket, paletteItem.Component)
        );

        // Some legacy Halo components carrying a whitelist marker are registered after the
        // palette's grouped list has been populated. Recover their existing PaletteItem so
        // torpedo launchers, plasma weapons, and other special equipment remain selectable.
        foreach (HullComponent component in BundleManager.Instance.AllComponents)
        {
            if (
                component == null
                || component.Type != socket.Type
                || component.GetComponent<ComponentRequiresSocketWhitelist>() == null
                || (
                    !socket.Hull.CanMountEquipment(component)
                    && !SharesHaloEquipmentBranch(
                        (socket.Hull as BaseHull)?.SaveKey,
                        component.SaveKey
                    )
                )
                || !SocketInstallationRuleEvaluator.Allows(socket, component)
            )
            {
                continue;
            }

            PaletteItem paletteItem =
                __instance.GetItemForComponent(component) as PaletteItem;
            if (paletteItem == null)
                paletteItem = __instance.Internals().CreateItem(component);
            if (paletteItem == null || __result.Contains(paletteItem))
                continue;

            paletteItem.SetCurrentSocketDims(socket.Size);
            socket.Hull.CountComponentsOfType(
                component,
                out int ofType,
                out int ofCompoundingClass
            );
            paletteItem.SetCurrentCount(ofType, ofCompoundingClass);
            __result.Add(paletteItem);
        }
    }

    private static bool SharesHaloEquipmentBranch(string hullSaveKey, string componentSaveKey)
    {
        return SharesPrefix(hullSaveKey, componentSaveKey, "Halo/Covenant/")
            || SharesPrefix(hullSaveKey, componentSaveKey, "Halo/UNSC/")
            || SharesPrefix(
                hullSaveKey,
                componentSaveKey,
                "Halo/United Rebel Front/"
            );
    }

    private static bool SharesPrefix(
        string hullSaveKey,
        string componentSaveKey,
        string prefix
    )
    {
        return hullSaveKey?.StartsWith(prefix, StringComparison.Ordinal) == true
            && componentSaveKey?.StartsWith(prefix, StringComparison.Ordinal) == true;
    }
}
