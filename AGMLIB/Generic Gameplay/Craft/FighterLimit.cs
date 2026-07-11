using SmallCraft;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public sealed class CraftLaunchLimit : MonoBehaviour
{
    private static readonly HashSet<Spacecraft> PendingLaunches = new();

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

    public static void RegisterPendingLaunch(Spacecraft craft)
    {
        if (craft != null)
        {
            PendingLaunches.Add(craft);
        }
    }

    public static void UnregisterPendingLaunch(Spacecraft craft)
    {
        if (craft != null)
        {
            PendingLaunches.Remove(craft);
        }
    }

    public static int GetPendingLaunchCount(ShipController ship)
    {
        PendingLaunches.RemoveWhere(craft =>
            craft == null
            || craft.IsDead
            || craft.IsFlying);
        if (ship?.OwnedBy == null)
        {
            return 0;
        }

        return PendingLaunches.Count(craft =>
            craft.LaunchedFromShip != null
            && craft.LaunchedFromShip.OwnedBy == ship.OwnedBy);
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

    public static bool HasLaunchCapacity(this CraftCarrierController carrier)
    {
        CraftLaunchLimit limit = carrier.GetCraftLaunchLimit();
        return limit == null || carrier.GetUsedCraftSlots() < limit.MaxActiveCraft;
    }

    public static int GetUsedCraftSlots(this CraftCarrierController carrier)
    {
        ShipController ship = carrier != null
            ? carrier.GetComponentInParent<ShipController>()
            : null;
        return carrier.GetActiveCraft() + CraftLaunchLimit.GetPendingLaunchCount(ship);
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

[HarmonyPatch(typeof(CraftCarrierController), nameof(CraftCarrierController.Tick))]
internal static class CraftCarrierControllerTickCraftLaunchLimitPatch
{
    private sealed class SuspendedLaunchOrder
    {
        public object Order;
        public object OriginalType;
    }

    private static void Prefix(CraftCarrierController __instance, ref List<SuspendedLaunchOrder> __state)
    {
        if (__instance.HasLaunchCapacity())
        {
            return;
        }

        IEnumerable queue = Common.GetVal<IEnumerable>(__instance, "_trafficQueue");
        if (queue == null)
        {
            Debug.LogError("AGMLIB craft limit could not suspend queued launches because the carrier traffic queue was unavailable.");
            return;
        }

        __state = new List<SuspendedLaunchOrder>();
        foreach (object order in queue)
        {
            object orderType = Common.GetVal<object>(order, "Type");
            if (orderType?.ToString() != "Launch")
            {
                continue;
            }

            __state.Add(new SuspendedLaunchOrder
            {
                Order = order,
                OriginalType = orderType,
            });
            Common.SetVal(order, "Type", System.Enum.ToObject(orderType.GetType(), -1));
        }
    }

    private static void Postfix(List<SuspendedLaunchOrder> __state)
    {
        RestoreLaunchOrders(__state);
    }

    private static System.Exception Finalizer(System.Exception __exception, List<SuspendedLaunchOrder> __state)
    {
        RestoreLaunchOrders(__state);
        return __exception;
    }

    private static void RestoreLaunchOrders(List<SuspendedLaunchOrder> suspendedOrders)
    {
        if (suspendedOrders == null)
        {
            return;
        }

        foreach (SuspendedLaunchOrder suspended in suspendedOrders)
        {
            Common.SetVal(suspended.Order, "Type", suspended.OriginalType);
        }

        suspendedOrders.Clear();
    }
}

[HarmonyPatch(typeof(CraftLandingPad), nameof(CraftLandingPad.LaunchCraftFromPad))]
internal static class CraftLandingPadLaunchCraftLimitPatch
{
    private static void Postfix(Spacecraft craftInstance, bool __result)
    {
        if (__result)
        {
            CraftLaunchLimit.RegisterPendingLaunch(craftInstance);
        }
    }
}

[HarmonyPatch(typeof(Spacecraft), nameof(Spacecraft.SetInStorage))]
internal static class SpacecraftSetInStorageCraftLaunchLimitPatch
{
    private static void Postfix(Spacecraft __instance, Ships.Controls.ICraftHangar hangar)
    {
        if (hangar != null)
        {
            CraftLaunchLimit.UnregisterPendingLaunch(__instance);
        }
    }
}
