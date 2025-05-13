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
        LauncherProgrammingQueue _pendingLaunches = Common.GetVal<LauncherProgrammingQueue>(tube, "_pendingLaunches");
        bool _isReloading = Common.GetVal<bool>(tube, "_isReloading");
        bool _isCyclingNextShot = Common.GetVal<bool>(tube, "_isCyclingNextShot");
        float _nextShotTime = Common.GetVal<float>(tube, "_nextShotTime");
        float _cycleLength = Common.GetVal<float>(tube, "_cycleLength");
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

