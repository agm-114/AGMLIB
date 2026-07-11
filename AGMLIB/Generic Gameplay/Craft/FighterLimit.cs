using SmallCraft;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public sealed class CraftLaunchLimit : MonoBehaviour
{
    public static bool AutoAttachToAllShips = false;
    public static int DefaultMaxActiveCraft = 2;

    public int MaxActiveCraft = DefaultMaxActiveCraft;

    public static CraftLaunchLimit EnsureAttachedTo(ShipController ship)
    {
        if (ship == null)
        {
            return null;
        }

        CraftLaunchLimit limit = ship.GetComponent<CraftLaunchLimit>();
        if (limit == null && AutoAttachToAllShips)
        {
            limit = ship.gameObject.AddComponent<CraftLaunchLimit>();
        }

        return limit;
    }
}

public static class CraftLaunchLimitExtensions
{
    public static CraftLaunchLimit GetCraftLaunchLimit(this CraftCarrierController carrier)
    {
        ShipController ship = carrier != null
            ? carrier.GetComponentInParent<ShipController>()
            : null;
        return CraftLaunchLimit.EnsureAttachedTo(ship);
    }

    public static bool CanLaunchOrders(this CraftCarrierController carrier, int launchOrderCount)
    {
        CraftLaunchLimit limit = carrier.GetCraftLaunchLimit();
        return limit == null || carrier.GetUsedCraftSlots() + launchOrderCount <= limit.MaxActiveCraft;
    }

    public static int GetAvailableSelectionSlots(this CraftCarrierController carrier)
    {
        CraftLaunchLimit limit = carrier.GetCraftLaunchLimit();
        return limit == null
            ? int.MaxValue
            : Mathf.Max(0, limit.MaxActiveCraft - carrier.GetUsedCraftSlots());
    }

    public static int GetUsedCraftSlots(this CraftCarrierController carrier)
    {
        return carrier.GetActiveCraft() + carrier.GetQueuedLaunches();
    }

    public static int GetQueuedLaunches(this CraftCarrierController carrier)
    {
        if (carrier == null)
        {
            Debug.LogError("AGMLIB craft limit could not count queued launches because the carrier was null.");
            return 0;
        }

        IEnumerable queue = Common.GetVal<IEnumerable>(carrier, "_trafficQueue");
        if (queue == null)
        {
            Debug.LogError("AGMLIB craft limit could not count queued launches because the carrier traffic queue was unavailable.");
            return 0;
        }

        int count = 0;
        foreach (object order in queue)
        {
            if (Common.GetVal<object>(order, "Type")?.ToString() == "Launch")
            {
                count++;
            }
        }

        return count;
    }

    public static int GetActiveCraft(this CraftCarrierController carrier)
    {
        if (carrier == null)
        {
            Debug.LogError("AGMLIB craft limit could not count active craft because the carrier was null.");
            return 0;
        }

        ShipController ship = carrier.GetComponentInParent<ShipController>();
        if (ship?.OwnedBy == null)
        {
            Debug.LogError("AGMLIB craft limit could not count active craft because the carrier owner was unavailable.");
            return 0;
        }

        SpacecraftGroupPooler pooler = SpacecraftGroupPooler.Instance;
        if (pooler == null)
        {
            Debug.LogError("AGMLIB craft limit could not count active craft because the spacecraft group pooler was unavailable.");
            return 0;
        }

        int count = 0;
        foreach (SpacecraftGroup group in pooler.AllActiveGroupsForPlayer(ship.OwnedBy))
        {
            if (group == null)
            {
                continue;
            }

            count += group.Members.Count(craft =>
                craft != null
                && !craft.IsDead
                && craft.IsFlying
                && craft.LaunchedFromShip != null
                && craft.LaunchedFromShip.OwnedBy == ship.OwnedBy);
        }

        return count;
    }

    public static bool IsLaunchCandidate(this Spacecraft craft)
    {
        return craft != null
            && craft.HangarWorkStatus != Spacecraft.HangarStatus.PostFlight
            && craft.HangarWorkStatus != Spacecraft.HangarStatus.PreFlight;
    }
}

[HarmonyPatch(typeof(ShipController), nameof(ShipController.Initialize))]
internal static class ShipControllerInitializeCraftLaunchLimitPatch
{
    private static void Postfix(ShipController __instance)
    {
        CraftLaunchLimit.EnsureAttachedTo(__instance);
    }
}

[HarmonyPatch(typeof(CraftCarrierController), nameof(CraftCarrierController.LaunchCraftFlight))]
internal static class CraftCarrierControllerLaunchCraftLimitPatch
{
    private static bool Prefix(
        CraftCarrierController __instance,
        IEnumerable<CraftCarrierController.LaunchOrder> orders,
        ref SpacecraftGroup __result)
    {
        List<CraftCarrierController.LaunchOrder> orderList = orders?.ToList();
        if (orderList == null || orderList.Count == 0 || __instance.CanLaunchOrders(orderList.Count))
        {
            return true;
        }

        __result = null;
        return false;
    }
}

[HarmonyPatch(typeof(ShipFlightControlMenu), nameof(ShipFlightControlMenu.AddCraftToSelection))]
internal static class ShipFlightControlMenuCraftLaunchSelectionLimitPatch
{
    private static void Postfix(ShipFlightControlMenu __instance)
    {
        List<StoredCraftItem> selectedSet = Common.GetVal<List<StoredCraftItem>>(__instance, "_selectedSet");
        if (selectedSet == null || selectedSet.Count == 0)
        {
            return;
        }

        CraftCarrierController carrier = Common.GetVal<CraftCarrierController>(__instance, "_carrier");
        if (carrier.GetCraftLaunchLimit() == null)
        {
            return;
        }

        int availableSelectionSlots = carrier.GetAvailableSelectionSlots();
        int selectedLaunchCount = selectedSet.Count(selected => selected.Craft.IsLaunchCandidate());
        while (selectedLaunchCount > availableSelectionSlots)
        {
            StoredCraftItem trim = selectedSet.FirstOrDefault(selected => selected.Craft.IsLaunchCandidate());
            if (trim == null)
            {
                break;
            }

            __instance.RemoveCraftFromSelection(trim);
            selectedLaunchCount--;
        }
    }
}
