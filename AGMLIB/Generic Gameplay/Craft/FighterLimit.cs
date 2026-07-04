using SmallCraft;
using System.Collections.Generic;
using System.Linq;
/*
public static class CraftLaunchLimitExtensions
{
    public const int MaxActiveCraft = 2;

    public static bool CanLaunchOrders(this CraftCarrierController carrier, int launchOrderCount)
    {
        return carrier.GetUsedCraftSlots() + launchOrderCount <= MaxActiveCraft;
    }

    public static int GetAvailableSelectionSlots(this CraftCarrierController carrier)
    {
        return MaxActiveCraft - carrier.GetUsedCraftSlots();
    }

    public static int GetUsedCraftSlots(this CraftCarrierController carrier)
    {
        return carrier.GetActiveCraft() + carrier.GetQueuedLaunches();
    }

    public static int GetQueuedLaunches(this CraftCarrierController carrier)
    {
        if (carrier == null)
            return 0;

        var queue = Common.GetVal<IEnumerable>(carrier, "_trafficQueue");
        if (queue == null)
            return 0;

        int count = 0;
        foreach (object order in queue)
        {
            if (Common.GetVal<object>(order, "Type")?.ToString() == "Launch")
                count++;
        }

        return count;
    }

    public static int GetActiveCraft(this CraftCarrierController carrier)
    {
        if (carrier == null)
            return 0;

        var owner = carrier.GetComponentInParent<ShipController>()?.OwnedBy;
        var pooler = SpacecraftGroupPooler.Instance;
        if (owner == null || pooler == null)
            return 0;

        int count = 0;
        foreach (SpacecraftGroup group in pooler.AllActiveGroupsForPlayer(owner))
        {
            if (group == null)
                continue;

            count += group.Members.Count(craft =>
                craft != null &&
                !craft.IsDead &&
                craft.IsFlying &&
                craft.LaunchedFromShip != null &&
                craft.LaunchedFromShip.OwnedBy == owner);
        }

        return count;
    }

    public static bool IsLaunchCandidate(this Spacecraft craft)
    {
        return craft != null &&
               craft.HangarWorkStatus != Spacecraft.HangarStatus.PostFlight &&
               craft.HangarWorkStatus != Spacecraft.HangarStatus.PreFlight;
    }
}



[HarmonyPatch(typeof(CraftCarrierController), nameof(CraftCarrierController.LaunchCraftFlight))]
public static class CraftLaunchLimitPatch
{
    public static bool Prefix(
        CraftCarrierController __instance,
        IEnumerable<CraftCarrierController.LaunchOrder> orders,
        ref SpacecraftGroup __result)
    {
        var orderList = orders?.ToList();
        if (orderList == null || orderList.Count == 0)
            return true;

        if (__instance.CanLaunchOrders(orderList.Count))
            return true;

        __result = null;
        return false;
    }
}

[HarmonyPatch(typeof(ShipFlightControlMenu), nameof(ShipFlightControlMenu.AddCraftToSelection))]
public static class CraftLaunchSelectionLimitPatch
{
    public static void Postfix(ShipFlightControlMenu __instance, StoredCraftItem item)
    {
        var selectedSet = Common.GetVal<List<StoredCraftItem>>(__instance, "_selectedSet");
        if (selectedSet == null || selectedSet.Count == 0)
            return;

        var carrier = Common.GetVal<CraftCarrierController>(__instance, "_carrier");
        int availableSelectionSlots = carrier.GetAvailableSelectionSlots();
        int selectedLaunchCount = selectedSet.Count(selected => selected.Craft.IsLaunchCandidate());
        while (selectedLaunchCount > availableSelectionSlots)
        {
            StoredCraftItem trim = selectedSet.FirstOrDefault(selected => selected.Craft.IsLaunchCandidate());
            if (trim == null)
                break;

            __instance.RemoveCraftFromSelection(trim);
            selectedLaunchCount--;
        }
    }
}
*/
/*
[HarmonyPatch(typeof(ShipFlightControlMenu), nameof(ShipFlightControlMenu.SetShip))]
public static class CraftLaunchUsageSetShipPatch
{
    public static void Postfix(ShipFlightControlMenu __instance)
    {
        CraftLimitUsageDisplay.Ensure(__instance);
    }
}

public class CraftLimitUsageDisplay : MonoBehaviour
{
    private ShipFlightControlMenu _menu;
    private TextMeshProUGUI _text;
    private UI.TooltipTrigger _tooltip;
    private CraftCarrierController _carrier;

    public static CraftLimitUsageDisplay Ensure(ShipFlightControlMenu menu)
    {
        CraftLimitUsageDisplay display = menu.GetComponent<CraftLimitUsageDisplay>() ?? menu.gameObject.AddComponent<CraftLimitUsageDisplay>();
        display._menu = menu;
        display.EnsureText(menu);
        return display;
    }

    private void EnsureText(ShipFlightControlMenu menu)
    {
        if (_text != null)
            return;

        TextMeshProUGUI template = Common.GetVal<TextMeshProUGUI>(menu, "_landCountText") ?? Common.GetVal<TextMeshProUGUI>(menu, "_launchCountText");
        if (template == null)
        {
            Debug.LogError("AGMLIB craft limit could not create usage display because no flight count text template was found.");
            return;
        }

        GameObject displayObject = Instantiate(template.gameObject, template.transform.parent);
        displayObject.name = "AGMLIB Craft Limit Usage";
        displayObject.transform.SetSiblingIndex(template.transform.GetSiblingIndex() + 1);
        _text = displayObject.GetComponent<TextMeshProUGUI>();
        _text.text = string.Empty;
        _tooltip = displayObject.GetComponent<UI.TooltipTrigger>() ?? displayObject.GetComponentInChildren<UI.TooltipTrigger>();

        RectTransform rect = displayObject.transform as RectTransform;
        RectTransform templateRect = template.transform as RectTransform;
        if (rect != null && templateRect != null && template.transform.parent.GetComponent<HorizontalOrVerticalLayoutGroup>() == null)
        {
            rect.anchoredPosition = templateRect.anchoredPosition + new Vector2(templateRect.rect.width + 8f, 0f);
        }
    }

    private void Update()
    {
        if (!isActiveAndEnabled || _text == null || !_text.gameObject.activeInHierarchy)
            return;

        _carrier = Common.GetVal<CraftCarrierController>(_menu, "_carrier");
        Refresh();
    }

    public void Refresh()
    {
        if (_text == null)
            return;

        int active = _carrier.GetActiveCraft();
        int queued = _carrier.GetQueuedLaunches();
        int used = active + queued;
        _text.text = $"{used} / {CraftLaunchLimitExtensions.MaxActiveCraft}\n<size=16>DEPLOYED</size>";

        if (_tooltip != null)
            _tooltip.Text = $"Deployed Craft: {active}\nQueued Launches: {queued}\nLimit: {CraftLaunchLimitExtensions.MaxActiveCraft}";
    }
}
*/