using Game;
using Game.UI;

namespace Lib.Generic_Gameplay;

/// <summary>Prioritizes native docking commands when targeting this tender in skirmish.</summary>
[DisallowMultipleComponent]
public class SkirmishTenderDocking : MonoBehaviour
{
    internal static void ValidateBerths(BaseHull hull)
    {
        ShipController ship = hull.GetComponentInParent<ShipController>();
        if (ship == null || !ship.GetComponentsInChildren<SkirmishTenderDocking>(true)
            .Any(marker => marker.GetComponentInParent<ShipController>() == ship)
            || hull.CollectAllParts<ReplenishmentDockingPoint>().Count != 0)
            return;
        Debug.LogError($"Could not find {nameof(ReplenishmentDockingPoint)} on {hull.ClassName}", hull);
    }

    internal bool IsAvailable(ShipController ship)
    {
        HullComponent? component = GetComponentInParent<HullComponent>();
        return isActiveAndEnabled && GetComponentInParent<ShipController>() == ship
            && (component == null || component.IsFunctional && !component.IsDestroyed);
    }

    internal static void ShowDockingCommand(ActionMenu menu)
    {
        if (SkirmishGameManager.Instance.InCampaign)
            return;
        var native = menu.Internals();
        if (!native.MovementButton.interactable || !native.DockButton.gameObject.activeSelf
            || !native.DockButton.interactable || native.DockingTarget is not ReplenishmentDockingPoint point)
            return;
        ShipController tender = point.GetComponentInParent<ShipController>();
        if (tender != null && tender.GetComponentsInChildren<SkirmishTenderDocking>()
            .Any(marker => marker.IsAvailable(tender)))
            native.MovementButton.isOn = true;
    }
}

[HarmonyPatch(typeof(BaseHull), nameof(BaseHull.PrepareComponentsForNetworkTraffic))]
internal static class SkirmishTenderDockingValidationPatch
{
    private static void Prefix(BaseHull __instance) => SkirmishTenderDocking.ValidateBerths(__instance);
}

[HarmonyPatch(typeof(ActionMenu), "OpenMenuInternal")]
internal static class SkirmishTenderDockingMenuPatch
{
    private static void Postfix(ActionMenu __instance) => SkirmishTenderDocking.ShowDockingCommand(__instance);
}
