public class GunLauncher : AmmoCompatiblity
{
    public bool ConsumesResourcesWhileCycling;

}

//[HarmonyPatch(typeof(HullComponent), "GetMissingResources")]
class HullComponentAwake
{
    public static void Prefix(HullComponent __instance)
    {
        CycledComponent weaponComponent = __instance as CycledComponent;


        if (weaponComponent == null)
            return;
        foreach (ResourceModifier resource in __instance.ResourcesRequired.Where(a => a.Amount == 1))
        {
            Debug.LogError(__instance.ComponentName + " is consuming " + weaponComponent.CycleActive + " " + resource.ResourceName + " " + resource.OnlyWhenOperating);
        }
    }
}

//[HarmonyPatch(typeof(HullComponent), nameof(HullComponent.ConsumeResources))]
class HullComponentConsumeResourcesDead
{
    public static bool Prefix(HullComponent __instance, ResourcePool pool, ResourceValue[] ____requiredResources)
    {
        //_operatingConsumingResources = Common.GetVal<bool>(__instance, "____operatingConsumingResources");
        BaseTubeLauncherComponent tube = __instance as BaseTubeLauncherComponent;

        if (!__instance.IsFunctional || ____requiredResources == null || tube == null)
        {
            return true;
        }



        TargetingMode _targetMode = Common.GetVal<TargetingMode>(tube, "_targetMode");
        LauncherProgrammingQueue _pendingLaunches = tube?.Internals().PendingLaunches;
        bool _isReloading = (tube?.Internals().IsReloading ?? default);
        bool _isCyclingNextShot = (tube?.Internals().IsCyclingNextShot ?? default);
        float _nextShotTime = (tube?.Internals().NextShotTime ?? default);
        float _cycleLength = (tube?.Internals().CycleLength ?? default);
        bool _operatingConsumingResources = _nextShotTime > Time.time;
        bool TEST =
            _targetMode != 0 ||
            _isCyclingNextShot ||
            _pendingLaunches == null ||
            tube.CurrentlyFiring ||
            _isReloading ||
            _isCyclingNextShot;

        return !_operatingConsumingResources;
    }
}

