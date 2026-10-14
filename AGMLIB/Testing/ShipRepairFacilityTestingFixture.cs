#if DEBUG && AGMLIB_LOCAL_TEST_FIXTURES
using Lib.Generic_Gameplay;
using System;

namespace Lib.Testing;

[TestingComponentFactory(Order = 120)]
public class ShipRepairFacilityTestingFactory : ITestingComponentFactory
{
    public void CreateTestingComponents(TestingComponentContext context)
    {
        if (Environment.GetEnvironmentVariable("AGMLIB_SHIP_REPAIR_TEST") == "1")
            new GameObject("Ship Repair Adapter Tests").AddComponent<ShipRepairFacilityAdapterTests>();
        if (Environment.GetEnvironmentVariable("AGMLIB_SHIP_REPAIR_PLAYTEST") != "1")
            return;
        HullComponent? component = context.Create("Stock/Reinforced Thruster Nozzles", "agmlib-testing/Ship Repair Facility", builder =>
        {
            builder.SetDisplayName("[TEST] Ship Repair Facility")
                .SetCategory("Testing")
                .SetDescription("Station-style repairs at all replenishment berths: 50% repair allowance and +75% repair speed. Remove this module to compare native tender behavior.")
                .SetPointCost(0)
                .UnlockForAllFactions();
            builder.Component.Internals().Size = Vector3Int.one;
            builder.Component.Internals().BindToTag = "";
            builder.Component.Modifiers = [];
            builder.AddToRoot<ShipRepairFacility>();
            if (Environment.GetEnvironmentVariable("AGMLIB_SHIP_REPAIR_SKIRMISH_DOCKING") == "1")
                builder.AddToRoot<SkirmishTenderDocking>();
        });
        if (component != null)
            Debug.Log("[AGMLIB Test] event=ship-repair-playtest-ready saveKey=agmlib-testing/Ship Repair Facility size=1x1x1");
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AGMLIB_SHIP_REPAIR_AUTOSTART_FLEET")))
            new GameObject("Ship Repair Playtest Setup").AddComponent<ShipRepairFacilityPlaytestSetup>();
    }
}

internal class ShipRepairFacilityPlaytestSetup : MonoBehaviour
{
    private IEnumerator Start()
    {
        DontDestroyOnLoad(gameObject);
        float deadline = Time.realtimeSinceStartup + 120f;
        while (!Bundles.BundleManager.IsInitialized)
        {
            Require(Time.realtimeSinceStartup < deadline, "bundle-loading-timeout");
            yield return null;
        }
        yield return new WaitForSecondsRealtime(2f);
        Require(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0, "main-menu-required");
        Fleet.FleetSummary fleet = Fleet.GetSummaryForFleet(new Utility.FilePath(
            Environment.GetEnvironmentVariable("AGMLIB_SHIP_REPAIR_AUTOSTART_FLEET")));
        Require(fleet != null && !fleet.NeedsUpgrade && !fleet.HasMixedFactionHulls, "invalid-test-fleet");
        Utility.CrossScenePipe.SetData(new FleetEditor.QuickFleetTestData
        {
            PlayerFleet = fleet,
            TargetFleet = fleet,
            BraindeadTarget = true,
            Map = Bundles.BundleManager.Instance.GetMap("Stock-Maps/ShootingGalleryInfo")
        });
        Debug.Log("[AGMLIB Test] event=ship-repair-range-loading");
        UnityEngine.SceneManagement.SceneManager.LoadScene(6);
        deadline = Time.realtimeSinceStartup + 180f;
        Game.SkirmishGameManager? manager;
        while ((manager = FindObjectOfType<Game.SkirmishGameManager>()) == null || !manager.IsGameActive)
        {
            Require(Time.realtimeSinceStartup < deadline, "testing-range-timeout");
            yield return null;
        }
        ShipController[] ships = manager.SkirmishLocalPlayer.PlayerFleet.FleetShipControllers.ToArray();
        ShipController tender = ships.Single(ship => ship.Ship.ShipNameNoPrefix == "Repair Tender");
        ShipController patient = ships.Single(ship => ship.Ship.ShipNameNoPrefix == "Repair Patient");
        Require(tender.GetComponentInChildren<ShipRepairFacility>() != null, "facility-not-installed");
        ReplenishmentDockingPoint berth = tender.GetComponentsInChildren<ReplenishmentDockingPoint>()
            .First(point => point.CanShipDock(patient));
        if (Environment.GetEnvironmentVariable("AGMLIB_SHIP_REPAIR_NATIVE_DOCK") == "1")
        {
            manager.SkirmishLocalPlayer.Select(patient, doubleClick: true);
            Game.UI.ActionMenu orders = manager.UI.OrdersMenu;
            orders.OpenMenu(manager.SkirmishLocalPlayer, patient, new Vector2(Screen.width / 2f, Screen.height / 2f),
                tender, manager.SkirmishLocalPlayer.ActionLimiter);
            var native = orders.Internals();
            Debug.Log($"[AGMLIB Test] event=tender-native-dock-menu campaign={manager.InCampaign} target={native.DockingTarget != null} active={native.DockButton.gameObject.activeInHierarchy} interactable={native.DockButton.interactable}");
            Require(native.DockButton.gameObject.activeInHierarchy && native.DockButton.interactable, "dock-command-unavailable");
            SkirmishTenderDocking? marker = tender.GetComponentInChildren<SkirmishTenderDocking>();
            if (marker != null)
            {
                native.MovementButton.isOn = false;
                SkirmishTenderDocking.ShowDockingCommand(orders);
                Require(native.MovementButton.isOn, "marked-tender-command-not-prioritized");
                marker.enabled = false;
                native.MovementButton.isOn = false;
                SkirmishTenderDocking.ShowDockingCommand(orders);
                Require(!native.MovementButton.isOn, "disabled-marker-command-prioritized");
                marker.enabled = true;
                ShipRepairFacility facility = tender.GetComponentInChildren<ShipRepairFacility>();
                facility.enabled = false;
                SkirmishTenderDocking.ShowDockingCommand(orders);
                Require(native.MovementButton.isOn, "docking-marker-requires-repair-facility");
                facility.enabled = true;
                Debug.Log("[AGMLIB Test] event=skirmish-tender-docking-marker result=success checks=3");
            }
            berth = (ReplenishmentDockingPoint)native.DockingTarget;
            orders.OrderDocking();
            deadline = Time.realtimeSinceStartup + 120f;
            while (patient.DockedWith != berth)
            {
                Require(Time.realtimeSinceStartup < deadline, "native-docking-order-timeout");
                yield return null;
            }
        }
        else
            berth.QuickDockShip(patient);
        yield return null;
        Require(patient.DockedWith == berth && berth.DockedShip == patient, "native-docking-failed");
        Require(patient.Ship.Hull.ComponentMaxRepair.Value == 0.5f
            && berth.ComponentRepairRateBonus?.Modifier == 0.75f, "repair-stats-not-applied");
        manager.SkirmishLocalPlayer.Select(patient, doubleClick: true);
        yield return null;
        Game.UI.DockedShipMenu menu = manager.UI.ShipBar.GetComponentInChildren<Game.UI.DockedShipMenu>();
        Game.UI.DockedRepairPane pane = menu.GetComponentInChildren<Game.UI.DockedRepairPane>(true);
        UI.TabbedControl tabs = menu.GetComponentInChildren<UI.TabbedControl>(true);
        for (int index = 0; index < 4 && !pane.gameObject.activeInHierarchy; index++)
            tabs.OpenTab(index);
        Require(pane.gameObject.activeInHierarchy, "repair-pane-not-visible");
        string text = string.Join(" | ", pane.GetComponentsInChildren<TMPro.TMP_Text>().Select(label => label.text));
        Require(text.Contains("50%"), "repair-pane-allowance-missing");
        Debug.Log($"[AGMLIB Test] event=ship-repair-playtest-docked allowance=0.5 speedBonus=0.75 panel={text}");
        Destroy(gameObject);
    }

    private static void Require(bool condition, string reason)
    {
        if (!condition)
            throw new InvalidOperationException($"Ship repair playtest setup failed: {reason}");
    }
}

// These adapter objects exercise real Unity hierarchy searches and native getter patches,
// while suppressing unrelated live-ship navigation, physics, and networking initialization.
internal class RepairAdapterShip : ShipController
{
    private void Awake() { }
    private void Update() { }
    private void FixedUpdate() { }
    private void OnDestroy() { }
}

internal class RepairAdapterShipModel : Ship
{
    private void OnDestroy() { }
}

internal class RepairAdapterModule : HullComponent
{
    protected override void Awake() => ReinitHP();
    protected override void Start() { }
    protected override void Update() { }
    internal void SetFunctional(bool functional)
    {
        _maxHealth = functional ? 100f : 0f;
        ReinitHP();
    }
}

internal class RepairAdapterBerth : ReplenishmentDockingPoint
{
    protected override void OnDisable() { }
}

internal class ShipRepairFacilityAdapterTests : MonoBehaviour
{
    private readonly List<GameObject> _roots = [];
    private int _checks;

    private IEnumerator Start()
    {
        // Nested/sibling placement, module damage, disabled components, ship isolation,
        // strongest-value selection, and discovery after runtime hierarchy changes.
        GameObject root = NewRoot("Repair Adapter Ship");
        ShipController ship = root.AddComponent<RepairAdapterShip>();
        ReplenishmentDockingPoint berth = Child(root, "Berth").AddComponent<RepairAdapterBerth>();
        ReplenishmentDockingPoint secondBerth = Child(root, "Second Berth").AddComponent<RepairAdapterBerth>();
        ShipRepairFacility builtIn = Child(Child(root, "Hull Visuals"), "Built In").AddComponent<ShipRepairFacility>();
        builtIn.ComponentMaxRepair = 0.4f;
        builtIn.ComponentRepairRateBonus = 0.8f;
        RepairAdapterModule module = Child(root, "Upgrade Module").AddComponent<RepairAdapterModule>();
        ShipRepairFacility upgrade = Child(module.gameObject, "Upgrade Child").AddComponent<ShipRepairFacility>();
        upgrade.ComponentMaxRepair = 0.7f;
        upgrade.ComponentRepairRateBonus = 0.5f;
        Check(berth.MaxComponentRepair?.Value == 0.7f, "sibling-upgrade-discovery");
        Check(secondBerth.MaxComponentRepair?.Value == 0.7f, "all-berths");
        Check(berth.ComponentRepairRateBonus?.Modifier == 0.8f, "independent-best-speed");
        Check(upgrade.IsAvailable(ship), "functional-module");
        SkirmishTenderDocking dockingMarker = Child(module.gameObject, "Docking Marker").AddComponent<SkirmishTenderDocking>();
        Check(dockingMarker.IsAvailable(ship) && !dockingMarker.IsAvailable(null!), "docking-marker-owner");
        module.SetFunctional(false);
        Check(!upgrade.IsAvailable(ship) && berth.MaxComponentRepair?.Value == 0.4f, "damaged-module-fallback");
        Check(!dockingMarker.IsAvailable(ship), "damaged-docking-module");
        builtIn.enabled = false;
        Check(berth.MaxComponentRepair == null, "all-facilities-unavailable");
        module.SetFunctional(true);
        Check(berth.MaxComponentRepair?.Value == 0.7f, "module-repaired");
        Check(dockingMarker.IsAvailable(ship), "docking-module-repaired");
        dockingMarker.enabled = false;
        Check(!dockingMarker.IsAvailable(ship), "disabled-docking-marker");
        upgrade.gameObject.SetActive(false);
        Check(berth.MaxComponentRepair == null, "inactive-child");
        upgrade.gameObject.SetActive(true);
        Check(berth.MaxComponentRepair?.Value == 0.7f, "child-reactivated");

        GameObject otherRoot = NewRoot("Unmodified Adapter Ship");
        ShipController otherShip = otherRoot.AddComponent<RepairAdapterShip>();
        ReplenishmentDockingPoint otherBerth = Child(otherRoot, "Other Berth").AddComponent<RepairAdapterBerth>();
        Check(otherBerth.MaxComponentRepair == null && !upgrade.IsAvailable(otherShip), "ship-isolation");
        ShipRepairService service = ShipRepairService.For(berth)!;
        Check(ReferenceEquals(service.EmptyRestoreStorage.Parent, ship)
            && ((Game.InventoryManagement.IStorageContainer)service.EmptyRestoreStorage).Capacity == 0,
            "repair-only-empty-storage");

        upgrade.enabled = false;
        ShipRepairFacility onBerth = berth.gameObject.AddComponent<ShipRepairFacility>();
        onBerth.ComponentMaxRepair = 0.9f;
        yield return new WaitForSecondsRealtime(0.3f);
        Check(secondBerth.MaxComponentRepair?.Value == 0.9f, "runtime-berth-facility-discovery");
        onBerth.transform.SetParent(otherRoot.transform, false);
        yield return new WaitForSecondsRealtime(0.3f);
        Check(secondBerth.MaxComponentRepair == null && otherBerth.MaxComponentRepair?.Value == 0.9f, "reparented-facility");
        GameObject template = NewRoot("Inactive Clone Template", active: false);
        template.AddComponent<RepairAdapterShip>();
        template.AddComponent<ShipRepairService>();
        Child(template, "Template Berth").AddComponent<RepairAdapterBerth>();
        Child(template, "Template Facility").AddComponent<ShipRepairFacility>();
        GameObject clone = Instantiate(template);
        _roots.Add(clone);
        clone.SetActive(true);
        Check(clone.GetComponentInChildren<ReplenishmentDockingPoint>().MaxComponentRepair?.Value == 0.5f, "cloned-ship-owner");
        Debug.Log($"[AGMLIB Test] event=ship-repair-adapters result=success checks={_checks}");
        Destroy(gameObject);
    }

    private GameObject NewRoot(string name, bool active = true)
    {
        GameObject root = new(name);
        root.SetActive(false);
        root.AddComponent<RepairAdapterShipModel>();
        root.SetActive(active);
        _roots.Add(root);
        return root;
    }

    private static GameObject Child(GameObject parent, string name)
    {
        GameObject child = new(name);
        child.transform.SetParent(parent.transform, false);
        return child;
    }

    private void Check(bool condition, string name)
    {
        if (!condition)
            throw new InvalidOperationException($"Ship repair adapter failed: {name}");
        _checks++;
    }

    private void OnDestroy() => _roots.ForEach(root => Destroy(root));
}
#endif
