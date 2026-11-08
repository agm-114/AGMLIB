using Bundles;
using Game;
using Game.InventoryManagement;

namespace Lib.Generic_Gameplay;

/// <summary>Resolves transferred inventory using existing skirmish assets and fleet designs.</summary>
[DisallowMultipleComponent]
internal class SkirmishInventory : MonoBehaviour, IStorableItemSet
{
    private SkirmishGameManager _manager = null!;
    private readonly Dictionary<string, IStorable> _items = new(StringComparer.Ordinal);

    private void Awake()
    {
        _manager = GetComponent<SkirmishGameManager>();
        foreach (IStorable item in BundleManager.Instance.AllMunitions)
            _items[item.SaveKey] = item;
        _items[FluidUnit.ShipFuel.SaveKey] = FluidUnit.ShipFuel;
        _items[GeneralItem.Restore.SaveKey] = GeneralItem.Restore;
    }

    private IEnumerable<Fleet> Fleets => _manager.Players.OfType<SkirmishPlayer>()
        .Select(player => player.PlayerFleet).Where(fleet => fleet != null);

    public IEnumerable<IStorable> AllItemTypes => _items.Values
        .Concat(Fleets.SelectMany(fleet => fleet.AvailableMunitions.AllMissileTypes.Cast<IStorable>()
            .Concat(fleet.FleetCraftDesigns.AllCraftDesigns)))
        .GroupBy(item => item.SaveKey, StringComparer.Ordinal).Select(group => group.First());

    public IStorable? GetItem(string saveKey)
    {
        if (saveKey == null)
            return null;
        if (_items.TryGetValue(saveKey, out IStorable item))
            return item;
        // Query live sets so late fleets and subsequently added designs are visible.
        foreach (Fleet fleet in Fleets)
        {
            IStorable? design = fleet.AvailableMunitions.GetMunition(saveKey)
                ?? (IStorable?)fleet.FleetCraftDesigns.GetCraftPattern(saveKey);
            if (design != null)
                return design;
        }
        return null;
    }

    internal static SkirmishInventory For(SkirmishGameManager manager) =>
        manager.GetComponent<SkirmishInventory>() ?? manager.gameObject.AddComponent<SkirmishInventory>();
}

[HarmonyPatch(typeof(SkirmishGameManager), nameof(SkirmishGameManager.StorableItems), MethodType.Getter)]
internal static class SkirmishInventoryPatch
{
    private static void Postfix(SkirmishGameManager __instance, ref IStorableItemSet? __result)
    {
        if (__result == null && !__instance.InCampaign)
            __result = SkirmishInventory.For(__instance);
    }
}
