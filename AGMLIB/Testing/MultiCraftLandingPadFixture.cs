#if DEBUG && AGMLIB_LOCAL_TEST_FIXTURES
using SmallCraft;
using UnityEngine.SceneManagement;

namespace Lib.Testing;

[TestingComponentFactory(Order = 150)]
public class MultiCraftPadTestFactory : ITestingComponentFactory
{
    internal static string? FleetPath => Environment.GetEnvironmentVariable("AGMLIB_MULTI_CRAFT_PAD_TEST_FLEET");
    public void CreateTestingComponents(TestingComponentContext context)
    {
        if (!string.IsNullOrEmpty(FleetPath))
            new GameObject("Multi-craft pad range test").AddComponent<MultiCraftPadRangeTest>();
    }
}

[HarmonyPatch(typeof(CraftCarrierController), nameof(CraftCarrierController.Initialize))]
internal static class MultiCraftPadTestLayout
{
    [HarmonyPriority(HarmonyLib.Priority.First)]
    private static void Prefix(ref List<ICraftEntryExitPoint> standalonePads)
    {
        if (string.IsNullOrEmpty(MultiCraftPadTestFactory.FleetPath))
            return;
        var original = standalonePads.OfType<ElevatorCraftLandingPad>().ToList();
        if (original.Count < 3 || !original[0].transform.root.GetComponentsInChildren<Hull>(true)
            .Any(hull => hull.SaveKey == "Stock/Levy Escort Carrier"))
            return;
        var positions = original.OrderByDescending(pad => pad.PadSize).Take(1)
            .Concat(original.OrderBy(pad => pad.PadSize).Take(2)).Select(MakePosition).ToArray();
        var root = positions[0];
        for (int i = 0; i < positions.Length; i++)
        {
            var position = positions[i];
            if (i != 0)
            {
                position.transform.SetParent(root.transform, false);
                Vector3 offset = Vector3.right * (i == 1 ? -2 : 2);
                position.transform.localPosition = offset;
                position.transform.localRotation = Quaternion.identity;
                position.Internals().Animations = null!;
                var anchor = new GameObject("Fighter launch anchor").transform;
                anchor.SetParent(root.PadAnchor, false);
                anchor.position = root.PadAnchor.position + root.transform.TransformVector(offset);
                AccessTools.Field(typeof(ElevatorCraftLandingPad), "_launchAnchor").SetValue(position, anchor);
                // Stagger takeoff so the first release must leave the elevator open for the other fighter.
                AccessTools.Field(typeof(CraftLandingPad), "_launchLandingDelay").SetValue(position, i * 2f);
            }
            // Use the stock hull's authored flight paths, shifted sideways with each fighter spot.
            var part = root.Internals().Part;
            var offsetWorld = position.transform.position - root.transform.position;
            part.GetWorldCraftTakeoffReleasePoint(out var takeoff, out var departure);
            part.GetWorldCraftLandingReleasePoint(out var landing, out var approach);
            AccessTools.Field(typeof(CraftLandingPad), "_takeoffReleasePoint").SetValue(position, position.transform.InverseTransformPoint(takeoff + offsetWorld));
            AccessTools.Field(typeof(CraftLandingPad), "_takeoffDirection").SetValue(position, position.transform.InverseTransformDirection(departure));
            AccessTools.Field(typeof(CraftLandingPad), "_landingReleasePoint").SetValue(position, position.transform.InverseTransformPoint(landing + offsetWorld));
            AccessTools.Field(typeof(CraftLandingPad), "_landingApproachDirection").SetValue(position, position.transform.InverseTransformDirection(approach));
            position.Internals().Part = root.Internals().Part;
            position.gameObject.SetActive(true);
        }
        standalonePads = [root];
        Debug.Log("[MultiCraftPadTest] layout installed");
    }

    internal static MultiCraftLandingPad MakePosition(ElevatorCraftLandingPad source)
    {
        GameObject obj = new("Test shared pad position");
        obj.SetActive(false);
        obj.transform.SetParent(source.transform, false);
        var pad = obj.AddComponent<MultiCraftLandingPad>();
        foreach (Type type in new[] { typeof(CraftLandingPad), typeof(ElevatorCraftLandingPad) })
            foreach (var field in AccessTools.GetDeclaredFields(type).Where(field => Attribute.IsDefined(field, typeof(SerializeField))))
                field.SetValue(pad, field.GetValue(source));
        AccessTools.Field(typeof(CraftLandingPad), "_useOwnPatternPoints").SetValue(pad, true);
        return pad;
    }
}

internal class MultiCraftPadRangeTest : MonoBehaviour
{
    private IEnumerator Start()
    {
        DontDestroyOnLoad(gameObject);
        yield return Await(() => BundleManager.IsInitialized, "bundles");
        yield return new WaitForSecondsRealtime(2);
        var fleet = Fleet.GetSummaryForFleet(new FilePath(MultiCraftPadTestFactory.FleetPath));
        CrossScenePipe.SetData(new QuickFleetTestData { PlayerFleet = fleet, TargetFleet = fleet, BraindeadTarget = true,
            Map = BundleManager.Instance.GetMap("Stock-Maps/ShootingGalleryInfo") });
        SceneManager.LoadScene(6);
        yield return Await(() => SkirmishGameManager.Instance != null && SkirmishGameManager.Instance.IsGameActive, "range");
        var player = SkirmishGameManager.Instance.SkirmishLocalPlayer;
        var ship = player.PlayerFleet.FleetShipControllers.First(s => s.CraftManager.AllStoredCraft.Any() &&
            s.CraftManager.Internals().SharedPads.OfType<MultiCraftLandingPad>().Any());
        var carrier = ship.CraftManager;
        var carrierBody = ship.GetComponent<Rigidbody>();
        carrierBody.velocity = carrierBody.angularVelocity = Vector3.zero;
        carrierBody.constraints = RigidbodyConstraints.FreezeAll;
        // Keep real pad animation and traffic ticks, but avoid spending the test on refueling.
        AccessTools.Field(typeof(CraftCarrierController), "_loadoutTransferRate").SetValue(carrier, 1000f);
        yield return Await(() => carrier.AllStoredCraft.Count(c => c.HangarWorkStatus == Spacecraft.HangarStatus.Stored) >= 2, "preflight");
        var craft = carrier.AllStoredCraft.Where(c => c.HangarWorkStatus == Spacecraft.HangarStatus.Stored).Take(2).ToArray();
        var pads = carrier.Internals().SharedPads.OfType<MultiCraftLandingPad>().ToArray();
        Require(pads.Length == 3 && pads.All(p => carrier.GetLandingPad(p.RpcKey) == p && carrier.GetLandingPadBySaveID(p.SaveID) == p), "registered IDs");
        var root = pads[0];
        CheckLargeLayout(root, craft[0], listed: false);
        CheckLargeLayout(root, craft[0], listed: true);
        Require(pads.Skip(1).All(p => p.Internals().Animations == root.Internals().Animations && p.PadAnchor.IsChildOf(root.PadAnchor))
            && pads[1].PadAnchor.position != pads[2].PadAnchor.position, "shared animator and separate moving anchors");
        root.enabled = false;
        Require(root.AvailableForUse(true) && !pads[1].AvailableForImmediateUse(false), "disabled structural/immediate availability");
        root.enabled = true;
        var size = AccessTools.Field(typeof(Spacecraft), "_requiredPadSize");
        var originalSize = craft[0].RequiredPadDimensions;
        size.SetValue(craft[0], root.PadDimensions);
        var fixture = new GameObject("Inactive hangar admission test");
        fixture.SetActive(false);
        var hangar = fixture.AddComponent<SingleCraftHangarComponent>();
        AccessTools.Field(typeof(SingleCraftHangarComponent), "_integratedLandingPad").SetValue(hangar, pads[1]);
        var units = AccessTools.Field(typeof(SingleCraftHangarComponent), "_craftUnitSize");
        units.SetValue(hangar, craft[0].StorageUnits);
        Require(!pads[1].CanFitCraft(craft[0]) && hangar.CanFitCraft(craft[0]), "larger sibling admission");
        units.SetValue(hangar, 0);
        Require(!hangar.CanFitCraft(craft[0]), "storage limit");
        Destroy(fixture);
        size.SetValue(craft[0], originalSize);
        Require(carrier.BaseSingleCycleLaunch(craft[0], false) == 2, "two positions");
        var group = carrier.LaunchCraftFlight(craft.Select(c => new CraftCarrierController.LaunchOrder { CraftId = c.netId, LoadoutIndex = 0 }));
        Require(group != null, "launch order accepted");
        yield return Await(() => pads.Any(p => p.OperationInProgress), "launch");
        Require(pads[1].OperationInProgress && pads[2].OperationInProgress && root.HasConflict, "same-frame fighter launches");
        yield return WatchLaunch(pads, craft, 2);
        foreach (var flight in craft.Select(c => c.Group).Distinct())
            player.OrderLandOnShip(flight, ship);
        float recoveryStart = Time.realtimeSinceStartup;
        while (craft.Any(c => c.StoredInHangar == null))
        {
            string states = string.Join(", ", craft.Select(c => $"{c.FlyingState} clearance={c.LandingClearance?.Pad?.RpcKey} pos={c.Position} anchor={c.LandingClearance?.Pad?.PadAnchor.position}"));
            Debug.Log($"[MultiCraftPadTest] recovery {states} holders={pads.Count(p => p.HoldingElevatorOpen)}");
            Require(Time.realtimeSinceStartup - recoveryStart < 160, "recovery");
            yield return new WaitForSecondsRealtime(6);
        }
        Debug.Log("[MultiCraftPadTest] completed recovery");
        yield return Await(() => pads.All(p => !p.OperationInProgress && !((ICraftEntryExitPoint)p).IsReserved), "deck clear");
        var solo = carrier.AllStoredCraft.First(c => c.HangarWorkStatus == Spacecraft.HangarStatus.Stored);
        carrier.LaunchCraftFlight([new CraftCarrierController.LaunchOrder { CraftId = solo.netId, LoadoutIndex = 0 }]);
        yield return Await(() => pads.Skip(1).Any(p => p.OperationInProgress), "solo fighter launch");
        yield return WatchLaunch(pads, [solo], 1);
        yield return Await(() => pads.All(p => !p.OperationInProgress && !((ICraftEntryExitPoint)p).IsReserved), "solo deck clear");
        var heavy = carrier.AllStoredCraft.First(c => c.HangarWorkStatus == Spacecraft.HangarStatus.Stored);
        size.SetValue(heavy, root.PadDimensions);
        Require(carrier.BaseSingleCycleLaunch(heavy, false) == 1, "one large position");
        carrier.LaunchCraftFlight([new CraftCarrierController.LaunchOrder { CraftId = heavy.netId, LoadoutIndex = 0 }]);
        yield return Await(() => root.OperationInProgress, "large launch");
        Require(pads.Skip(1).All(p => p.HasConflict && !p.AvailableForImmediateUse(false)), "large craft blocks both fighters");
        yield return WatchLaunch(pads, [heavy], 1);
        Debug.Log("[MultiCraftPadTest] PASS shared elevator, paired launch and recovery, solo fighter, exclusive large launch");
        Application.Quit();
    }

    private static void CheckLargeLayout(MultiCraftLandingPad source, Spacecraft craft, bool listed)
    {
        var root = MultiCraftPadTestLayout.MakePosition(source);
        root.transform.SetParent(null, true);
        var children = Enumerable.Range(0, 40).Select(_ => MultiCraftPadTestLayout.MakePosition(source)).ToList();
        if (listed)
            AccessTools.Field(typeof(MultiCraftLandingPad), "_positions").SetValue(root, children);
        else
            children.ForEach(child => child.transform.SetParent(root.transform, false));
        var expanded = MultiCraftLandingPad.Expand([root]);
        Require(expanded.Length == 41 && children.All(p => p.Deck == root)
            && root.CompatibleCapacity(root.Positions) == 40, "40 independent child positions");
        children[39].SetReservation(craft);
        Require(root.HasConflict && children.All(p => !p.HasConflict), "last child blocks only the center");
        children[39].SetReservation(null);
        root.SetReservation(craft);
        Require(children.All(p => p.HasConflict), "center blocks all 40 children");
        children.ForEach(p => Destroy(p.gameObject));
        Destroy(root.gameObject);
        Debug.Log($"[MultiCraftPadTest] 40 child positions passed (explicit list={listed})");
    }

    private static IEnumerator WatchLaunch(MultiCraftLandingPad[] pads, Spacecraft[] craft, int expectedHolders)
    {
        var root = pads[0];
        var native = root.Internals();
        var start = root.transform.InverseTransformPoint(root.PadAnchor.position);
        bool previous = false, moved = false, sawPartialRelease = false;
        int opens = 0, closes = 0, peak = 0;
        float deadline = Time.realtimeSinceStartup + 150;
        float nextLog = 0;
        do
        {
            int holders = pads.Count(p => p.HoldingElevatorOpen);
            bool open = native.Animations.GetBool(native.OpenControl);
            if (Time.realtimeSinceStartup >= nextLog || open != previous)
            {
                Debug.Log($"[MultiCraftPadTest] lift holders={holders} control={open} opened={root.IsOpen} closed={root.IsClosed} flying={craft.Count(c => c.IsFlying)}");
                nextLog = Time.realtimeSinceStartup + 15;
            }
            Require(open == (holders > 0), "elevator follows outstanding requests");
            if (open && !previous) opens++;
            if (!open && previous) closes++;
            peak = Math.Max(peak, holders);
            sawPartialRelease |= peak == 2 && holders == 1;
            moved |= (root.transform.InverseTransformPoint(root.PadAnchor.position) - start).sqrMagnitude > 0.0001f;
            previous = open;
            Require(Time.realtimeSinceStartup < deadline, "shared launch cycle");
            yield return null;
        } while (closes == 0 || !root.IsClosed || craft.Any(c => !c.IsFlying));
        Require(opens == 1 && closes == 1 && peak == expectedHolders && moved && (peak == 1 || sawPartialRelease),
            $"launch cycle opens={opens} closes={closes} peak={peak} moved={moved} partialRelease={sawPartialRelease}");
        Debug.Log($"[MultiCraftPadTest] one elevator cycle, {peak} position(s), moving launch anchors");
    }

    private static IEnumerator Await(Func<bool> ready, string stage)
    {
        float deadline = Time.realtimeSinceStartup + 150;
        while (!ready())
        {
            Require(Time.realtimeSinceStartup < deadline, stage);
            yield return null;
        }
        Debug.Log($"[MultiCraftPadTest] completed {stage}");
    }

    private static void Require(bool ok, string stage)
    {
        if (!ok)
            throw new InvalidOperationException("[MultiCraftPadTest] Failed " + stage);
    }
}
#endif
