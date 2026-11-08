using System.Runtime.CompilerServices;

public class DynamicStun : ActiveSettings
{
    public override void Awake()
    {
        DynamicWorkingHelpers.Register(this);
        base.Awake();
    }

    protected virtual void OnDestroy() => DynamicWorkingHelpers.Unregister(this);

    public enum KnockbackMode
    {
        None,
        COM,
        ComponentPostion,
        TransformPostion
    }
    public Vector3 Localize(Transform basis, Vector3 force) =>
        basis.forward * force.z + basis.right * force.x + basis.up * force.y;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (active != lastactive)
        {
            DynamicWorkingHelpers.RefreshWorkingState(ShipController);

            if (Knockback == KnockbackMode.COM)
                Rigidbody?.AddRelativeForce(Force, Mode);
            else if (Knockback == KnockbackMode.TransformPostion && Postion != null)
                Rigidbody?.AddForceAtPosition(Localize(Rigidbody.transform, Force), Postion.position, Mode);
            else if (Knockback == KnockbackMode.ComponentPostion)
                Rigidbody?.AddForceAtPosition(Localize(Rigidbody.transform, Force), transform.position, Mode);
        }
    }
    public bool DisableDrives = true;
    public bool DisableReactors = true;
    public bool DisableWeapons = true;
    public ForceMode Mode = ForceMode.Impulse;
    public KnockbackMode Knockback = KnockbackMode.ComponentPostion;
    public Transform Postion;
    public Vector3 Force = new(0, 0, -1000);
}
public class DynamicWorkingCache : MonoBehaviour
{
    public List<DriveComponent> _allDrives;
    public List<PowerplantComponent> _allPowerplants;
}
public class TimedDynamicStun : DynamicStun
{
    private ShipController _targetShip;
    private float _endTime;

    public void Apply(ShipController ship, float durationSeconds, bool disableDrives, bool disableReactors, bool disableWeapons)
    {
        _targetShip = ship;
        DisableDrives = disableDrives;
        DisableReactors = disableReactors;
        DisableWeapons = disableWeapons;
        active = true;
        lastactive = false;
        _endTime = Time.time + Mathf.Max(0f, durationSeconds);
        DynamicWorkingHelpers.RefreshWorkingState(ship);
    }

    protected override void FixedUpdate()
    {
        if (_targetShip == null)
        {
            Destroy(this);
            return;
        }

        if (Time.time >= _endTime)
        {
            active = false;
            DynamicWorkingHelpers.RefreshWorkingState(_targetShip);
            Destroy(this);
            return;
        }

        active = true;
    }
}

public static class DynamicWorkingHelpers
{
    private static readonly List<DynamicStun> Stuns = [];
    private static readonly ConditionalWeakTable<ShipController, object> InitializedShips = new();

    internal static void Register(DynamicStun stun)
    {
        if (!Stuns.Contains(stun))
            Stuns.Add(stun);
    }

    internal static void Unregister(DynamicStun stun) => Stuns.Remove(stun);

    private static object Initialize(ShipController ship)
    {
        // Seed once, including components whose Awake has not run yet. New live
        // components register in Awake; ship keys do not survive their owners.
        foreach (DynamicStun stun in ship.GetComponentsInChildren<DynamicStun>(true))
            Register(stun);
        return new object();
    }

    internal static bool HasActive(this ShipController ship, Func<DynamicStun, bool> predicate)
    {
        InitializedShips.GetValue(ship, Initialize);
        // An inactive root is still searched by Unity. Components added there may
        // not have received Awake, so retain discovery for this non-combat case.
        if (!ship.gameObject.activeInHierarchy)
            return ship.GetActive().Any(predicate);

        Transform root = ship.transform;
        for (int index = Stuns.Count - 1; index >= 0; index--)
        {
            DynamicStun stun = Stuns[index];
            if (stun == null)
            {
                Stuns.RemoveAt(index);
                continue;
            }
            // Check live ancestry instead of caching ownership: moving a whole
            // module need not notify each descendant's OnTransformParentChanged.
            if (stun.active && predicate(stun) && stun.gameObject.activeInHierarchy && stun.transform.IsChildOf(root))
                return true;
        }
        return false;
    }

    public static IEnumerable<DynamicStun> GetActive(this ShipController ship) =>
        ship.gameObject.GetComponentsInChildren<DynamicStun>().Where(stun => stun.active);

    public static void ApplyTimed(ShipController ship, float durationSeconds, bool disableDrives, bool disableReactors, bool disableWeapons)
    {
        if (ship == null || durationSeconds <= 0f || (!disableDrives && !disableReactors && !disableWeapons))
        {
            return;
        }

        ship.gameObject.AddComponent<TimedDynamicStun>()
            .Apply(ship, durationSeconds, disableDrives, disableReactors, disableWeapons);
    }

    public static void RefreshWorkingState(ShipController ship)
    {
        if (ship != null)
            ship.Internals().RefreshWorkingState();
    }
}

[HarmonyPatch(typeof(ShipController))]
[HarmonyPatch("WeaponsControl", MethodType.Getter)]
class ShipControllerWeaponsControl
{
    public static void Postfix(ref WeaponsControlStatus __result, ShipController __instance)
    {
        if (!ShipInfoBarMatchAllButtons.MatchingAllButtons && __result != WeaponsControlStatus.Hold
            && __instance.HasActive(static stun => stun.DisableWeapons))
            __result = WeaponsControlStatus.Hold;
    }
}

[HarmonyPatch(typeof(ShipController), "HandleDrivesWorkingChanged")]
class ShipControllerHandleDrivesWorkingChanged
{
    static void Prefix(ShipController __instance)
    {
        Common.LogPatch();
        GameObject ship = __instance.gameObject;
        DynamicWorkingCache cache = ship.GetComponent<DynamicWorkingCache>() ?? ship.AddComponent<DynamicWorkingCache>();
        if (!__instance.HasActive(static stun => stun.DisableDrives))
            return;
        cache._allDrives = __instance.Internals().AllDrives;
        __instance.Internals().AllDrives = [];
    }

    static void Postfix(ShipController __instance)
    {
        Common.LogPatch();
        GameObject ship = __instance.gameObject;
        DynamicWorkingCache cache = ship.GetComponent<DynamicWorkingCache>() ?? ship.AddComponent<DynamicWorkingCache>();
        if (cache._allDrives != null)
            __instance.Internals().AllDrives = cache._allDrives;
        cache._allDrives = null;
    }
}

[HarmonyPatch(typeof(ShipController), "HandlePowerplantsWorkingChanged")]
class ShipControllerHandlePowerplantsWorkingChanged
{
    static void Prefix(ShipController __instance)
    {
        Common.LogPatch();
        GameObject ship = __instance.gameObject;
        DynamicWorkingCache cache = ship.GetComponent<DynamicWorkingCache>() ?? ship.AddComponent<DynamicWorkingCache>();
        if (!__instance.HasActive(static stun => stun.DisableReactors))
            return;
        cache._allPowerplants = __instance.Internals().AllPowerplants;
        __instance.Internals().AllPowerplants = [];
    }

    static void Postfix(ShipController __instance)
    {
        Common.LogPatch();
        GameObject ship = __instance.gameObject;
        DynamicWorkingCache cache = ship.GetComponent<DynamicWorkingCache>() ?? ship.AddComponent<DynamicWorkingCache>();
        if (cache._allPowerplants != null)
            __instance.Internals().AllPowerplants = cache._allPowerplants;
        cache._allPowerplants = null;
    }
}
