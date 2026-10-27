#if DEBUG && AGMLIB_LOCAL_TEST_FIXTURES
using UnityEngine.SceneManagement;

namespace Lib.Testing;

[TestingComponentFactory(Order = 140)]
public class HangarMagazineTestingFactory : ITestingComponentFactory
{
    public const string SaveKey = "agmlib-testing/Hangar Magazine";

    public void CreateTestingComponents(TestingComponentContext context)
    {
        if (Environment.GetEnvironmentVariable("AGMLIB_HANGAR_MAGAZINE_TEST") != "1")
            return;
        HullComponent? component = context.Create("Stock/Bulk Internal Hangar", SaveKey, builder =>
        {
            builder.ReplaceRoot<HangarMagazineTestingComponent>()
                .SetDisplayName("[TEST] Hangar Magazine")
                .SetCategory("Testing")
                .SetDescription("1,000 m³ shared magazine; 8 craft slots. Each occupied slot costs 5% plus 50 m³. Four slots leave 600 m³. Includes four pre/postflight work slots.")
                .SetPointCost(0)
                .UnlockForAllFactions();
            builder.Component.Internals().Size = Vector3Int.one;
            MagazineComponent magazine = builder.Prefab.GetComponent<MagazineComponent>() ?? builder.AddToRoot<MagazineComponent>();
            AccessTools.Field(typeof(MagazineComponent), "_availableVolume").SetValue(magazine, 1000);
            AccessTools.Field(typeof(HangarMagazineComponent), "_spaceUnitsPerTile").SetValue(builder.Component, 8);
            AccessTools.Field(typeof(HangarMagazineComponent), "_magazineReductionPercentPerSlot").SetValue(builder.Component, 5f);
            AccessTools.Field(typeof(HangarMagazineComponent), "_magazineVolumePerSlot").SetValue(builder.Component, 50f);
            LightweightCraftWorkSlotComponent slots = builder.AddToRoot<LightweightCraftWorkSlotComponent>();
            AccessTools.Field(typeof(LightweightCraftWorkSlotComponent), "_fixedWorkSlots").SetValue(slots, 4);
            AccessTools.Field(typeof(HangarMagazineComponent), "_workSlotProviders")
                .SetValue(builder.Component, new List<CraftWorkSlotComponentBase> { slots });
        });
        if (component == null)
            return;
        Debug.Log($"[AGMLIB Test] event=hangar-magazine-ready key={SaveKey}");
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AGMLIB_HANGAR_MAGAZINE_FLEET")))
            new GameObject("Hangar Magazine Playtest Setup").AddComponent<HangarMagazinePlaytestSetup>();
    }
}

// Keep the fixture's advertised capacities constant in differently sized test sockets.
public class HangarMagazineTestingComponent : HangarMagazineComponent
{
    protected override int CalcTileMultiplier() => 1;
}

internal class HangarMagazinePlaytestSetup : MonoBehaviour
{
    private IEnumerator Start()
    {
        DontDestroyOnLoad(gameObject);
        float deadline = Time.realtimeSinceStartup + 120;
        while (!BundleManager.IsInitialized)
        {
            Require(Time.realtimeSinceStartup < deadline, "bundle-loading-timeout");
            yield return null;
        }
        yield return new WaitForSecondsRealtime(2);
        Require(SceneManager.GetActiveScene().buildIndex == 0, "main-menu-required");
        Fleet.FleetSummary fleet = Fleet.GetSummaryForFleet(new FilePath(Environment.GetEnvironmentVariable("AGMLIB_HANGAR_MAGAZINE_FLEET")));
        Require(fleet != null && !fleet.NeedsUpgrade && !fleet.HasMixedFactionHulls, "invalid-test-fleet");
        if (Environment.GetEnvironmentVariable("AGMLIB_HANGAR_MAGAZINE_EDITOR") == "1")
        {
            CrossScenePipe.SetData(new EditFleetData { Fleet = fleet });
            Debug.Log("[AGMLIB Test] event=hangar-magazine-editor-loading");
            SceneManager.LoadScene(1);
            yield return PrepareEditor();
            Destroy(gameObject);
            yield break;
        }
        CrossScenePipe.SetData(new QuickFleetTestData
        {
            PlayerFleet = fleet,
            TargetFleet = fleet,
            BraindeadTarget = true,
            Map = BundleManager.Instance.GetMap("Stock-Maps/ShootingGalleryInfo")
        });
        Debug.Log("[AGMLIB Test] event=hangar-magazine-range-loading");
        SceneManager.LoadScene(6);
        deadline = Time.realtimeSinceStartup + 180;
        SkirmishGameManager? manager;
        while ((manager = FindObjectOfType<SkirmishGameManager>()) == null || !manager.IsGameActive)
        {
            Require(Time.realtimeSinceStartup < deadline, "testing-range-timeout");
            yield return null;
        }
        ShipController carrier = manager.SkirmishLocalPlayer.PlayerFleet.FleetShipControllers
            .Single(ship => ship.Ship.ShipNameNoPrefix == "Hangar Lab");
        HangarMagazineComponent hangar = carrier.Ship.Hull.CollectComponents<HangarMagazineComponent>().Single();
        Require(hangar.StoredCrafts.Count() == 4 && hangar.OccupiedStorageUnits == 4, "craft-load-mismatch");
        Require(hangar.Capacity == 600, "magazine-capacity-mismatch");
        Require(carrier.CraftManager.AllStoredCraft.Count() == 4, "carrier-storage-not-registered");
        manager.SkirmishLocalPlayer.Select(carrier, doubleClick: true);
        Debug.Log($"[AGMLIB Test] event=hangar-magazine-playtest-ready ship=Hangar-Lab crafts={hangar.StoredCrafts.Count()} slots={hangar.OccupiedStorageUnits}/{hangar.TotalStorageUnits} magazine={hangar.UsedCapacity}/{hangar.Capacity} workSlots={((ICraftWorkSlotProvider)hangar).WorkSlots.Length}");
        Destroy(gameObject);
    }

    private static IEnumerator PrepareEditor()
    {
        float deadline = Time.realtimeSinceStartup + 120;
        FleetEditorController? editor;
        EditorShipController? carrier;
        while ((editor = FindObjectOfType<FleetEditorController>()) == null
            || (carrier = editor.EditorControllers
                .FirstOrDefault(ship => ship.Ship.ShipNameNoPrefix == "Hangar Lab")) == null)
        {
            Require(Time.realtimeSinceStartup < deadline, "editor-fleet-loading-timeout");
            yield return null;
        }
        yield return new WaitForSecondsRealtime(1);
        FindObjectOfType<FleetCompositionSubmodeController>().SelectShip(carrier);
        yield return null;
        HangarMagazineComponent hangar = carrier.Ship.Hull.CollectComponents<HangarMagazineComponent>().Single();
        SettingsHangarLoadout panel = FindObjectsOfType<SettingsHangarLoadout>()
            .Single(candidate => candidate.Internals().Hangar is HangarMagazineComponent);
        ComponentSaveData saved = hangar.GetSaveData();
        IMunition ammo = hangar.Magazines.First().AmmoType;
        hangar.StoredCrafts.ToList().ForEach(hangar.RemoveCraftFromStorage);
        hangar.Magazines.ToList().ForEach(magazine => hangar.RemoveAllFromMagazine(magazine.AmmoType));
        hangar.AddToMagazine(ammo, 199800);
        Require(hangar.UsedCapacity == 999 && hangar.TotalStorageUnits == 0, "ammo-full-hangar-capacity");
        Require(PanelText(panel).Contains("0/0"), "ammo-full-hangar-readout");
        Require(panel.Internals().CapacityGauge.fillAmount == 0, "empty-hangar-gauge");
        hangar.RemoveFromMagazine(ammo, 19800);
        Require(hangar.TotalStorageUnits == 1 && PanelText(panel).Contains("0/1"), "ammo-removal-live-refresh");
        hangar.LoadSaveData(saved);
        Require(hangar.TotalStorageUnits == 8 && PanelText(panel).Contains("4/8"), "restored-hangar-readout");
        editor.Fleet.ClearModifiedFlag();
        Debug.Log("[AGMLIB Test] event=hangar-magazine-editor-ready checks=5 fullAmmo=0/0 reducedAmmo=0/1 restored=4/8");
    }

    private static string PanelText(SettingsHangarLoadout panel) =>
        string.Join(" | ", panel.GetComponentsInChildren<TMP_Text>().Select(label => label.text));

    private static void Require(bool condition, string reason)
    {
        if (!condition)
            throw new InvalidOperationException($"Hangar magazine playtest setup failed: {reason}");
    }
}
#endif
