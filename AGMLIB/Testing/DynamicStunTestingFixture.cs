#if DEBUG && AGMLIB_LOCAL_TEST_FIXTURES
using System.Diagnostics;

namespace Lib.Testing;

[TestingComponentFactory(Order = 146)]
public class DynamicStunTestingFactory : ITestingComponentFactory
{
    public void CreateTestingComponents(TestingComponentContext context)
    {
        if (Environment.GetEnvironmentVariable("AGMLIB_DYNAMIC_STUN_TEST") == "1")
            new GameObject("Dynamic Stun Tests").AddComponent<DynamicStunTests>();
    }
}

internal class DynamicStunTests : MonoBehaviour
{
    private readonly List<GameObject> _roots = [];
    private int _checks;

    private IEnumerator Start()
    {
        float deadline = Time.realtimeSinceStartup + 120;
        while (!BundleManager.IsInitialized)
        {
            if (Time.realtimeSinceStartup >= deadline)
                throw new InvalidOperationException("Dynamic stun adapter bundle loading timed out.");
            yield return null;
        }

        ShipController ship = NewShip("Stun Adapter Ship");
        ShipController other = NewShip("Other Stun Adapter Ship");
        CheckState(ship, "empty-ship");

        DynamicStun stun = AddStun(Child(ship.gameObject, "Stun Module"));
        CheckState(ship, "runtime-addition");
        CheckState(other, "ship-isolation");
        stun.active = false;
        CheckState(ship, "inactive-effect");
        stun.active = true;
        stun.DisableWeapons = false;
        CheckState(ship, "drives-and-reactors-only");
        stun.DisableWeapons = true;
        stun.DisableDrives = stun.DisableReactors = false;
        CheckState(ship, "weapons-only");
        Check(!stun.enabled && ship.WeaponsControl == WeaponsControlStatus.Hold, "disabled-component-still-counted");

        ShipInfoBarMatchAllButtons.MatchingAllButtons = true;
        try { Check(ship.WeaponsControl == WeaponsControlStatus.Free, "button-refresh-bypass"); }
        finally { ShipInfoBarMatchAllButtons.MatchingAllButtons = false; }
        foreach (WeaponsControlStatus setting in new[] { WeaponsControlStatus.Free, WeaponsControlStatus.Tight, WeaponsControlStatus.Hold })
        {
            WeaponsControlStatus result = setting;
            ShipControllerWeaponsControl.Postfix(ref result, ship);
            Check(result == WeaponsControlStatus.Hold, $"override-{setting}");
            stun.active = false;
            result = setting;
            ShipControllerWeaponsControl.Postfix(ref result, ship);
            Check(result == setting, $"preserve-{setting}");
            stun.active = true;
        }

        stun.gameObject.SetActive(false);
        CheckState(ship, "inactive-child-object");
        stun.gameObject.SetActive(true);
        CheckState(ship, "reactivated-child-object");
        GameObject module = Child(ship.gameObject, "Movable Module");
        stun.transform.SetParent(module.transform, false);
        module.transform.SetParent(other.transform, false);
        CheckState(ship, "moved-ancestor-old-ship");
        CheckState(other, "moved-ancestor-new-ship");
        module.SetActive(false);
        module.transform.SetParent(ship.transform, false);
        CheckState(ship, "moved-inactive-ancestor");
        module.SetActive(true);
        CheckState(ship, "moved-ancestor-reactivated");

        ShipController dormant = NewShip("Dormant Stun Adapter Ship");
        dormant.gameObject.SetActive(false);
        DynamicStun dormantStun = AddStun(Child(dormant.gameObject, "Dormant Module"));
        CheckState(dormant, "never-awakened-child");
        DynamicStun rootStun = AddStun(dormant.gameObject);
        CheckState(dormant, "late-added-inactive-root");
        dormant.gameObject.SetActive(true);
        CheckState(dormant, "previously-dormant-ship");
        DestroyImmediate(dormantStun);
        DestroyImmediate(rootStun);
        CheckState(dormant, "destroyed-components");

        DynamicStun second = AddStun(Child(ship.gameObject, "Overlapping Stun"));
        DestroyImmediate(stun);
        CheckState(ship, "overlapping-stun-survives");
        DestroyImmediate(second);
        CheckState(ship, "last-stun-removed");

        ShipController template = NewShip("Stun Clone Template", active: false);
        AddStun(Child(template.gameObject, "Clone Stun"));
        GameObject clone = Instantiate(template.gameObject);
        _roots.Add(clone);
        clone.SetActive(true);
        ShipController clonedShip = clone.GetComponent<ShipController>();
        clonedShip.Internals().AllDrives = [];
        clonedShip.Internals().AllPowerplants = [];
        CheckState(clonedShip, "cloned-ship");
        DestroyImmediate(clone);
        CheckState(ship, "destroyed-clone-isolation");

        DynamicWorkingHelpers.ApplyTimed(ship, 0.1f, true, true, true);
        CheckState(ship, "timed-stun-applied");
        DynamicWorkingHelpers.ApplyTimed(ship, 2f, false, false, true);
        deadline = Time.realtimeSinceStartup + 5;
        while (ship.GetComponents<TimedDynamicStun>().Length > 1 && Time.realtimeSinceStartup < deadline)
            yield return null;
        Check(ship.WeaponsControl == WeaponsControlStatus.Hold, "timed-stun-overlap");
        CheckState(ship, "first-timed-stun-expired");
        while (ship.GetComponents<TimedDynamicStun>().Length > 0 && Time.realtimeSinceStartup < deadline)
            yield return null;
        CheckState(ship, "all-timed-stuns-expired");
        Check(ship.GetComponents<TimedDynamicStun>().Length == 0, "expired-components-destroyed");

        Benchmark(ship);
        Debug.Log($"[AGMLIB Test] event=dynamic-stun-adapters result=success checks={_checks}");
        _roots.ForEach(root => Destroy(root));
        yield return null;
        Application.Quit();
    }

    private void CheckState(ShipController ship, string name)
    {
        Check(ship.HasActive(static stun => stun.DisableWeapons) == ship.GetActive().Any(stun => stun.DisableWeapons)
            && ship.HasActive(static stun => stun.DisableDrives) == ship.GetActive().Any(stun => stun.DisableDrives)
            && ship.HasActive(static stun => stun.DisableReactors) == ship.GetActive().Any(stun => stun.DisableReactors), name);
        Check(ship.WeaponsControl == (ship.GetActive().Any(stun => stun.DisableWeapons)
            ? WeaponsControlStatus.Hold : WeaponsControlStatus.Free), $"{name}-native-getter");
        List<DriveComponent> drives = ship.Internals().AllDrives;
        List<PowerplantComponent> powerplants = ship.Internals().AllPowerplants;
        DynamicWorkingHelpers.RefreshWorkingState(ship);
        Check(ReferenceEquals(drives, ship.Internals().AllDrives)
            && ReferenceEquals(powerplants, ship.Internals().AllPowerplants), $"{name}-working-lists-restored");
    }

    private void Benchmark(ShipController ship)
    {
        for (int index = 0; index < 2048; index++)
            Child(ship.gameObject, $"Hierarchy Cost Control {index}");
        const int iterations = 2000;
        for (int index = 0; index < 100; index++)
            _ = ship.WeaponsControl;
        Stopwatch timer = Stopwatch.StartNew();
        for (int index = 0; index < iterations; index++)
        {
            _ = ship.GetActive().Any();
            _ = ship.GetActive().Count(stun => stun.DisableWeapons);
        }
        double originalUs = timer.Elapsed.TotalMilliseconds * 1000 / iterations;
        timer.Restart();
        for (int index = 0; index < iterations; index++)
            _ = ship.WeaponsControl;
        double optimizedUs = timer.Elapsed.TotalMilliseconds * 1000 / iterations;
        Debug.Log($"[AGMLIB Test] event=dynamic-stun-cost children=2048 iterations={iterations} originalUs={originalUs:F3} optimizedUs={optimizedUs:F3}");
    }

    private ShipController NewShip(string name, bool active = true)
    {
        GameObject root = new(name);
        root.SetActive(false);
        root.AddComponent<Mirror.NetworkIdentity>();
        root.AddComponent<RepairAdapterShipModel>();
        ShipController ship = root.AddComponent<RepairAdapterShip>();
        ship.Internals().AllDrives = [];
        ship.Internals().AllPowerplants = [];
        root.SetActive(active);
        _roots.Add(root);
        return ship;
    }

    private static DynamicStun AddStun(GameObject target)
    {
        DynamicStun stun = target.AddComponent<DynamicStun>();
        stun.enabled = false;
        stun.active = true;
        return stun;
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
            throw new InvalidOperationException($"Dynamic stun adapter failed: {name}");
        _checks++;
    }
}
#endif
