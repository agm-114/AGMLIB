#if DEBUG && AGMLIB_LOCAL_TEST_FIXTURES
using Game;
using Game.InventoryManagement;
using Lib.Generic_Gameplay;
using Mirror;
using Munitions.ModularMissiles;
using SmallCraft;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Lib.Testing;

[TestingComponentFactory(Order = 140)]
public class SkirmishInventoryTestingFactory : ITestingComponentFactory
{
    public void CreateTestingComponents(TestingComponentContext context)
    {
        if (Environment.GetEnvironmentVariable("AGMLIB_SKIRMISH_INVENTORY_TEST") != "1")
            return;
        if (Environment.GetEnvironmentVariable("AGMLIB_SKIRMISH_INVENTORY_BASELINE") == "1")
            new Harmony("agmlib-testing-inventory-baseline").Unpatch(
                AccessTools.PropertyGetter(typeof(SkirmishGameManager), nameof(SkirmishGameManager.StorableItems)),
                AccessTools.Method(typeof(SkirmishInventoryPatch), "Postfix"));
        new GameObject("Skirmish Inventory Tests").AddComponent<SkirmishInventoryTests>();
    }
}

internal class SkirmishInventoryTests : MonoBehaviour
{
    private int _checks;

    private IEnumerator Start()
    {
        DontDestroyOnLoad(gameObject);
        float deadline = Time.realtimeSinceStartup + 120f;
        while (!Bundles.BundleManager.IsInitialized)
        {
            Check(Time.realtimeSinceStartup < deadline, "bundle-timeout");
            yield return null;
        }
        yield return new WaitForSecondsRealtime(2f);
        Fleet.FleetSummary fleet = Fleet.GetSummaryForFleet(new Utility.FilePath(
            Environment.GetEnvironmentVariable("AGMLIB_SKIRMISH_INVENTORY_FLEET")));
        Check(fleet != null && !fleet.NeedsUpgrade && !fleet.HasMixedFactionHulls, "invalid-fleet");
        Utility.CrossScenePipe.SetData(new FleetEditor.QuickFleetTestData
        {
            PlayerFleet = fleet, TargetFleet = fleet, BraindeadTarget = true,
            Map = Bundles.BundleManager.Instance.GetMap("Stock-Maps/ShootingGalleryInfo")
        });
        UnityEngine.SceneManagement.SceneManager.LoadScene(6);
        deadline = Time.realtimeSinceStartup + 180f;
        SkirmishGameManager manager;
        while ((manager = FindObjectOfType<SkirmishGameManager>()) == null || !manager.IsGameActive)
        {
            Check(Time.realtimeSinceStartup < deadline, "range-timeout");
            yield return null;
        }
        HumanSkirmishPlayer player = manager.SkirmishLocalPlayer;
        Check(player.isServer && NetworkClient.isConnected && !manager.InCampaign, "local-skirmish-host-required");
        ShipController[] ships = player.PlayerFleet.FleetShipControllers.ToArray();
        ShipController tender = ships.Single(ship => ship.Ship.ShipNameNoPrefix == "Repair Tender");
        ShipController patient = ships.Single(ship => ship.Ship.ShipNameNoPrefix == "Repair Patient");
        ReplenishmentDockingPoint berth = tender.GetComponentsInChildren<ReplenishmentDockingPoint>()
            .First(point => point.CanShipDock(patient));
        berth.QuickDockShip(patient);
        Check(ReferenceEquals(patient.DockedWith, berth), "native-docking-failed");
        int fuelQuantity = Mathf.Min(10, patient.FuelTank.Capacity / 2, tender.FuelTank.Capacity / 2);
        Check(fuelQuantity > 0 && tender.FuelTank.Current >= fuelQuantity, "insufficient-fuel");
        patient.FuelTank.SetCurrent(patient.FuelTank.Capacity - fuelQuantity);
        if (Environment.GetEnvironmentVariable("AGMLIB_SKIRMISH_INVENTORY_BASELINE") == "1")
        {
            Check(manager.StorableItems == null, "baseline-registry-not-null");
            bool rejected = false;
            Application.LogCallback observe = (message, stack, type) =>
            {
                if (message.Contains("Disconnecting connId=") && message.Contains("NullReferenceException"))
                    rejected = true;
            };
            Application.logMessageReceived += observe;
            try
            {
                player.CmdImmediateItemTransfer(tender.FuelTank.GetStorageReference(), patient.FuelTank.GetStorageReference(),
                    FluidUnit.ShipFuel, fuelQuantity);
                deadline = Time.realtimeSinceStartup + 10f;
                while (!rejected && Time.realtimeSinceStartup < deadline)
                    yield return null;
            }
            finally
            {
                Application.logMessageReceived -= observe;
            }
            Check(rejected, "baseline-command-not-rejected");
            Debug.Log("[AGMLIB Test] event=skirmish-inventory-baseline result=disconnect-reproduced");
        }
        else
        {
            int missilesBefore = Resources.FindObjectsOfTypeAll<ModularMissile>().Length;
            int craftBefore = Resources.FindObjectsOfTypeAll<Spacecraft>().Length;
            bool firstUse = manager.GetComponent<SkirmishInventory>() == null;
            Stopwatch timer = Stopwatch.StartNew();
            IStorableItemSet items = manager.StorableItems;
            timer.Stop();
            Debug.Log($"[AGMLIB Test] event=skirmish-inventory-init elapsedMs={timer.Elapsed.TotalMilliseconds:F3} firstUse={firstUse} bundledItems={Bundles.BundleManager.Instance.AllMunitions.Count}");
            Check(ReferenceEquals(items, manager.StorableItems), "registry-not-reused");
            Check(missilesBefore == Resources.FindObjectsOfTypeAll<ModularMissile>().Length
                && craftBefore == Resources.FindObjectsOfTypeAll<Spacecraft>().Length, "registry-cloned-patterns");
            foreach (IStorable item in Bundles.BundleManager.Instance.AllMunitions)
                Check(ReferenceEquals(item, RoundTrip(item)), "bundled-item-roundtrip");
            Check(ReferenceEquals(FluidUnit.ShipFuel, RoundTrip(FluidUnit.ShipFuel)), "fuel-roundtrip");
            Check(ReferenceEquals(GeneralItem.Restore, RoundTrip(GeneralItem.Restore)), "restore-roundtrip");
            Check(RoundTrip(null) == null && items.GetItem("agmlib-test/missing") == null, "absent-item");
            int missiles = 0, craft = 0;
            foreach (SkirmishPlayer owner in manager.Players.OfType<SkirmishPlayer>())
            {
                if (owner.PlayerFleet == null)
                    continue;
                foreach (ModularMissile item in owner.PlayerFleet.AvailableMunitions.AllMissileTypes)
                {
                    Check(RoundTrip(item)?.SaveKey == item.SaveKey, "missile-roundtrip");
                    missiles++;
                }
                foreach (Spacecraft item in owner.PlayerFleet.FleetCraftDesigns.AllCraftDesigns)
                {
                    Check(RoundTrip(item)?.SaveKey == item.SaveKey, "craft-roundtrip");
                    craft++;
                }
            }
            Check(missiles > 0 && craft > 0, "custom-design-coverage-missing");
            IStorable[] all = items.AllItemTypes.ToArray();
            Check(all.Length == all.Select(item => item.SaveKey).Distinct().Count(), "duplicate-item-keys");
            Debug.Log($"[AGMLIB Test] event=skirmish-inventory-designs missiles={missiles} craft={craft} uniqueItems={all.Length}");
            yield return Transfer(player, tender.FuelTank, patient.FuelTank, FluidUnit.ShipFuel, fuelQuantity);
            yield return Transfer(player, patient.FuelTank, tender.FuelTank, FluidUnit.ShipFuel, fuelQuantity);
            IStorable ammunition = patient.StoragePool.StoredItemTypes.First();
            yield return Transfer(player, patient.StoragePool, tender.StoragePool, ammunition, 1);
            yield return Transfer(player, tender.StoragePool, patient.StoragePool, ammunition, 1);
            Check(((ISimpleStorageContainer)tender.RestoreStoragePool).WithdrawItems(GeneralItem.Restore, 1) == 1,
                "restore-capacity-setup");
            yield return Transfer(player, patient.RestoreStoragePool, tender.RestoreStoragePool, GeneralItem.Restore, 1);
            yield return Transfer(player, tender.RestoreStoragePool, patient.RestoreStoragePool, GeneralItem.Restore, 1);
            yield return new WaitForSecondsRealtime(2f);
            Check(NetworkClient.isConnected && NetworkClient.ready && manager.IsGameActive, "network-disconnected");
            Debug.Log($"[AGMLIB Test] event=skirmish-inventory-summary result=success checks={_checks} transfers=6 connected={NetworkClient.isConnected}");
        }
        yield return new WaitForSecondsRealtime(2f);
        Application.Quit();
    }

    private IEnumerator Transfer(HumanSkirmishPlayer player, ISimpleStorageContainer from, ISimpleStorageContainer to,
        IStorable item, int quantity)
    {
        int source = from.GetItemQuantity(item), destination = to.GetItemQuantity(item);
        Check(source >= quantity, "transfer-source-empty");
        player.CmdImmediateItemTransfer(from.GetStorageReference(), to.GetStorageReference(), item, quantity);
        float deadline = Time.realtimeSinceStartup + 10f;
        while (to.GetItemQuantity(item) != destination + quantity)
        {
            Check(NetworkClient.isConnected && Time.realtimeSinceStartup < deadline, "transfer-failed");
            yield return null;
        }
        Check(from.GetItemQuantity(item) == source - quantity, "transfer-not-conserved");
        Debug.Log($"[AGMLIB Test] event=skirmish-inventory-transfer item={item.SaveKey} quantity={quantity} source={source}->{from.GetItemQuantity(item)} destination={destination}->{to.GetItemQuantity(item)}");
    }

    private static IStorable? RoundTrip(IStorable? item)
    {
        NetworkWriter writer = new();
        writer.WriteStorableItem(item!);
        return new NetworkReader(writer.ToArray()).ReadStorableItem();
    }

    private void Check(bool condition, string reason)
    {
        if (!condition)
            throw new InvalidOperationException($"Skirmish inventory test failed: {reason}");
        _checks++;
    }
}
#endif
