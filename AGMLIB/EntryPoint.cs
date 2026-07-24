using AGMLIB.Dynamic_Systems.Area;
using Modding;
using System.Reflection;

public class EntryPoint : IModEntryPoint
{
    private const string HarmonyId = "neb.lib.harmony.product";

    public void PreLoad()
    {
        if (Common.InventoryDebug)
        {
            FleetTools.Inventory = FleetTools.DefaultInventory();
            FleetTools.SaveInventory();
        }

        Debug.Log($"AGMLIB: {Assembly.GetExecutingAssembly().GetName().Version} Preload");

        if (Harmony.HasAnyPatches(HarmonyId))
            return;

        new Harmony(HarmonyId).PatchAll();
    }

    public void PostLoad()
    {
#if DEBUG
        Lib.Testing.TestingPrefabYamlDumper.ScheduleDumpAfterAllModsLoaded();
#endif

        Lib.Testing.TestingComponentBootstrap.DiscoverAndCreate(Assembly.GetExecutingAssembly());

        foreach (FactionDescription faction in BundleManager.Instance.AllFactions)
        {
            if (faction is not IModular modularFaction)
                continue;

            foreach (var module in modularFaction.Modules)
            {
                if (module is not InventoryRules inventory)
                    continue;

                Common.Hint($"Found Inventory Rules for {faction.FactionName}");
                FleetTools.Inventory = FleetTools.DefaultInventory(inventory.DefaultAmount);
            }
        }
    }
}
