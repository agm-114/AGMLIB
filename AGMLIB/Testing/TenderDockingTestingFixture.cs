#if DEBUG && AGMLIB_LOCAL_TEST_FIXTURES
using Game;
using Game.UI;
using Lib.Generic_Gameplay;

namespace Lib.Testing;

[TestingComponentFactory(Order = 121)]
public class TenderDockingTestingFactory : ITestingComponentFactory
{
    public void CreateTestingComponents(TestingComponentContext context)
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AGMLIB_TENDER_DOCKING_FLEET")))
            return;
        new GameObject("Tender Docking Playtest").AddComponent<TenderDockingPlaytest>();
    }
}

internal class TenderDockingPlaytest : MonoBehaviour
{
    private IEnumerator Start()
    {
        DontDestroyOnLoad(gameObject);
        float deadline = Time.realtimeSinceStartup + 120f;
        while (!Bundles.BundleManager.IsInitialized)
        {
            Require(Time.realtimeSinceStartup < deadline, "bundle-timeout");
            yield return null;
        }
        yield return new WaitForSecondsRealtime(2f);
        Fleet.FleetSummary fleet = Fleet.GetSummaryForFleet(new Utility.FilePath(
            Environment.GetEnvironmentVariable("AGMLIB_TENDER_DOCKING_FLEET")));
        Require(fleet != null && !fleet.NeedsUpgrade && !fleet.HasMixedFactionHulls, "invalid-fleet");
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
            Require(Time.realtimeSinceStartup < deadline, "range-timeout");
            yield return null;
        }
        ShipController[] ships = manager.SkirmishLocalPlayer.PlayerFleet.FleetShipControllers.ToArray();
        ShipController patient = ships.Single(ship => ship.Ship.ShipNameNoPrefix ==
            Environment.GetEnvironmentVariable("AGMLIB_TENDER_DOCKING_PATIENT"));
        Require(patient.isServer, "host-required");
        foreach (string name in Environment.GetEnvironmentVariable("AGMLIB_TENDER_DOCKING_TARGETS").Split('|'))
        {
            ShipController tender = ships.Single(ship => ship.Ship.ShipNameNoPrefix == name);
            manager.SkirmishLocalPlayer.Select(patient, doubleClick: true);
            ActionMenu orders = manager.UI.OrdersMenu;
            orders.OpenMenu(manager.SkirmishLocalPlayer, patient, new Vector2(Screen.width / 2f, Screen.height / 2f),
                tender, manager.SkirmishLocalPlayer.ActionLimiter);
            var native = orders.Internals();
            Debug.Log($"[AGMLIB Test] event=tender-dock-menu hull={tender.Ship.Hull.SaveKey} berths={tender.Ship.Hull.CollectAllParts<ReplenishmentDockingPoint>().Count} target={native.DockingTarget != null} visible={native.DockButton.gameObject.activeInHierarchy} enabled={native.DockButton.interactable}");
            if (Environment.GetEnvironmentVariable("AGMLIB_TENDER_DOCKING_EXPECT_MISSING") == "1")
            {
                Require(native.DockingTarget == null && !native.DockButton.gameObject.activeSelf, "baseline-dock-unexpectedly-available");
                Debug.Log($"[AGMLIB Test] event=tender-docking-diagnosis hull={tender.Ship.Hull.SaveKey} result=missing-dock-confirmed");
                yield return new WaitForSecondsRealtime(5f);
                continue;
            }
            Require(native.DockButton.gameObject.activeInHierarchy && native.DockButton.interactable, "missing-dock-command");
            ReplenishmentDockingPoint berth = (ReplenishmentDockingPoint)native.DockingTarget;
            Require(tender.GetDockingPoint(berth.RpcKey) == berth, "unregistered-berth");
            List<ShipRepairFacility> facilities = tender.GetComponentsInChildren<ShipRepairFacility>()
                .Where(facility => facility.enabled).ToList();
            facilities.ForEach(facility => facility.enabled = false);
            Require(berth.CanShipDock(patient), "docking-requires-repairs");
            facilities.ForEach(facility => facility.enabled = true);
            if (Environment.GetEnvironmentVariable("AGMLIB_TENDER_DOCKING_MANUAL") != "1")
                orders.OrderDocking();
            Debug.Log("[AGMLIB Test] event=tender-docking-awaiting-order");
            deadline = Time.realtimeSinceStartup + 300f;
            while (patient.DockedWith != berth)
            {
                Require(Time.realtimeSinceStartup < deadline, "normal-docking-timeout");
                yield return null;
            }
            Require(berth.DockedShip == patient, "berth-occupant-mismatch");
            Debug.Log($"[AGMLIB Test] event=tender-docking-completed hull={tender.Ship.Hull.SaveKey} rpcKey={berth.RpcKey} result=success");
            yield return new WaitForSecondsRealtime(5f);
            berth.Undock(patient);
            deadline = Time.realtimeSinceStartup + 30f;
            while (patient.DockedWith != null)
            {
                Require(Time.realtimeSinceStartup < deadline, "undocking-timeout");
                yield return null;
            }
            yield return new WaitForSecondsRealtime(2f);
            Require(patient.DockedWith == null, "undocking-failed");
        }
        Debug.Log("[AGMLIB Test] event=tender-docking-summary result=success");
        Destroy(gameObject);
    }

    private static void Require(bool condition, string reason)
    {
        if (!condition)
            throw new InvalidOperationException($"Tender docking playtest failed: {reason}");
    }
}
#endif
