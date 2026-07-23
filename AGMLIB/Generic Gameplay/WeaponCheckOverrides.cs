[DisallowMultipleComponent]
public class WeaponCheckOverrides : MonoBehaviour
{
    [Header("Checks To Bypass")]
    [SerializeField]
    [Tooltip("Skip the layer-9 obstacle raycast used to keep a weapon from firing through the hull or another obstruction.")]
    private bool _ignoreHullObstructionCheck = true;

    [SerializeField]
    [Tooltip("Skip the friendly-ship raycast before firing.")]
    private bool _allowFriendlyFire = true;

    [SerializeField]
    [Tooltip("Permit firing beyond the weapon's effective range.")]
    private bool _ignoreRangeCheck = true;

    [SerializeField]
    [Tooltip("Permit firing while the weapon's aim direction is outside its normal on-target angle.")]
    private bool _ignoreAimAngleCheck = true;

    [SerializeField]
    [Tooltip("Permit firing without a lock when the weapon normally waits for its integrated fire-control sensor.")]
    private bool _ignoreFireControlLockCheck = true;

    [Header("Friendly Targets")]
    [SerializeField]
    [UnityEngine.Serialization.FormerlySerializedAs("Targetfriendlies")]
    [Tooltip("Allow spawned EWAR effects to select friendly jamming targets.")]
    private bool _targetFriendlies;

    [SerializeField]
    [UnityEngine.Serialization.FormerlySerializedAs("Affectfriendlies")]
    [Tooltip("Allow spawned effects to affect friendly targets.")]
    private bool _affectFriendlies;

    [HideInInspector]
    public WeaponComponent? Source;

    public bool IgnoreHullObstructionCheck
    {
        get => _ignoreHullObstructionCheck;
        set => _ignoreHullObstructionCheck = value;
    }

    public bool AllowFriendlyFire
    {
        get => _allowFriendlyFire;
        set => _allowFriendlyFire = value;
    }

    public bool IgnoreRangeCheck
    {
        get => _ignoreRangeCheck;
        set => _ignoreRangeCheck = value;
    }

    public bool IgnoreAimAngleCheck
    {
        get => _ignoreAimAngleCheck;
        set => _ignoreAimAngleCheck = value;
    }

    public bool IgnoreFireControlLockCheck
    {
        get => _ignoreFireControlLockCheck;
        set => _ignoreFireControlLockCheck = value;
    }

    public bool TargetFriendlies
    {
        get => _targetFriendlies;
        set => _targetFriendlies = value;
    }

    public bool AffectFriendlies
    {
        get => _affectFriendlies;
        set => _affectFriendlies = value;
    }

    internal void ApplyRuntimeSettings(WeaponCheckOverrides settings, WeaponComponent? source)
    {
        IgnoreHullObstructionCheck = settings.IgnoreHullObstructionCheck;
        AllowFriendlyFire = settings.AllowFriendlyFire;
        IgnoreRangeCheck = settings.IgnoreRangeCheck;
        IgnoreAimAngleCheck = settings.IgnoreAimAngleCheck;
        IgnoreFireControlLockCheck = settings.IgnoreFireControlLockCheck;
        TargetFriendlies = settings.TargetFriendlies;
        AffectFriendlies = settings.AffectFriendlies;
        Source = source;
    }

    internal static void RemoveFrom(NetworkPoolable instance)
    {
        if (instance == null)
        {
            return;
        }

        WeaponCheckOverrides overrides = instance.gameObject.GetComponent<WeaponCheckOverrides>();
        if (overrides != null)
        {
            Destroy(overrides);
        }
    }
}

public class FratricidalWeapon : WeaponCheckOverrides
{

}

internal static class WeaponCheckOverrideResolver
{
    internal static bool TryResolve(Component component, out WeaponCheckOverrides overrides)
    {
        WeaponCheckOverrides? resolved = Find<WeaponCheckOverrides>(component);
        if (resolved != null)
        {
            overrides = resolved;
            return true;
        }

        overrides = null!;
        return false;
    }

    private static T? Find<T>(Component component) where T : Component
    {
        if (component == null)
        {
            return null;
        }

        return component.GetComponent<T>()
            ?? component.GetComponentInChildren<T>(includeInactive: true)
            ?? component.GetComponentInParent<T>();
    }
}

[HarmonyPatch(typeof(WeaponComponent), "AimCheck")]
internal static class WeaponComponentAimCheckOverridesPatch
{
    private readonly struct AimCheckState
    {
        internal AimCheckState(
            bool active,
            bool checkFriendlyFire,
            bool checkObstaclesInWay,
            bool waitForFireControlLock,
            float onTargetAngle)
        {
            Active = active;
            CheckFriendlyFire = checkFriendlyFire;
            CheckObstaclesInWay = checkObstaclesInWay;
            WaitForFireControlLock = waitForFireControlLock;
            OnTargetAngle = onTargetAngle;
        }

        internal bool Active { get; }

        internal bool CheckFriendlyFire { get; }

        internal bool CheckObstaclesInWay { get; }

        internal bool WaitForFireControlLock { get; }

        internal float OnTargetAngle { get; }
    }

    private static void Prefix(
        WeaponComponent __instance,
        ref bool ignoreRange,
        ref bool ____checkFriendlyFire,
        ref bool ____checkObstaclesInWay,
        ref bool ____waitForFCLock,
        ref float ____onTargetAngle,
        out AimCheckState __state)
    {
        if (!WeaponCheckOverrideResolver.TryResolve(__instance, out WeaponCheckOverrides settings))
        {
            __state = default;
            return;
        }

        __state = new AimCheckState(
            active: true,
            ____checkFriendlyFire,
            ____checkObstaclesInWay,
            ____waitForFCLock,
            ____onTargetAngle);

        if (settings.AllowFriendlyFire)
        {
            ____checkFriendlyFire = false;
        }

        if (settings.IgnoreHullObstructionCheck)
        {
            ____checkObstaclesInWay = false;
        }

        if (settings.IgnoreFireControlLockCheck)
        {
            ____waitForFCLock = false;
        }

        if (settings.IgnoreAimAngleCheck)
        {
            ____onTargetAngle = 180f;
        }

        if (settings.IgnoreRangeCheck)
        {
            ignoreRange = true;
        }
    }

    private static void Postfix(
        ref bool ____checkFriendlyFire,
        ref bool ____checkObstaclesInWay,
        ref bool ____waitForFCLock,
        ref float ____onTargetAngle,
        AimCheckState __state)
    {
        if (!__state.Active)
        {
            return;
        }

        ____checkFriendlyFire = __state.CheckFriendlyFire;
        ____checkObstaclesInWay = __state.CheckObstaclesInWay;
        ____waitForFCLock = __state.WaitForFireControlLock;
        ____onTargetAngle = __state.OnTargetAngle;
    }
}

[HarmonyPatch(typeof(ActiveJammingEffect), "CheckTargetValidity")]
internal static class ActiveJammingEffectCheckTargetValidityOverridesPatch
{
    private static void Postfix(
        ActiveEWarEffect __instance,
        ref bool __result,
        IEWarTarget target,
        SignatureType ____sigType)
    {
        if (!WeaponCheckOverrideResolver.TryResolve(__instance, out WeaponCheckOverrides settings)
            || !settings.TargetFriendlies)
        {
            return;
        }

        if (target is IJammable jammable
            && jammable.SigType == ____sigType
            && jammable.CanJammerHitAperture(__instance.transform.position.To(jammable.Position)))
        {
            __result = true;
        }
    }
}

[HarmonyPatch(typeof(FollowingInstanceMuzzle), nameof(FollowingInstanceMuzzle.Fire))]
internal static class FollowingInstanceMuzzleFireWeaponCheckOverridesPatch
{
    private static void Prefix(NetworkPoolable ____followingInstance)
    {
        WeaponCheckOverrides.RemoveFrom(____followingInstance);
    }

    private static void Postfix(FollowingInstanceMuzzle __instance, NetworkPoolable ____followingInstance)
    {
        if (____followingInstance == null
            || !WeaponCheckOverrideResolver.TryResolve(__instance, out WeaponCheckOverrides settings))
        {
            return;
        }

        WeaponCheckOverrides runtimeOverrides =
            ____followingInstance.gameObject.GetOrAddComponent<WeaponCheckOverrides>();
        runtimeOverrides.ApplyRuntimeSettings(
            settings,
            __instance.GetComponentInParent<WeaponComponent>());
    }
}

[HarmonyPatch(typeof(FollowingInstanceMuzzle), nameof(FollowingInstanceMuzzle.StopFire))]
internal static class FollowingInstanceMuzzleStopFireWeaponCheckOverridesPatch
{
    private static void Prefix(NetworkPoolable ____followingInstance)
    {
        WeaponCheckOverrides.RemoveFrom(____followingInstance);
    }
}
