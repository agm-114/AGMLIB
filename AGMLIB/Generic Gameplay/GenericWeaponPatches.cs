using Missions.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[HarmonyPatch(typeof(DiscreteWeaponComponent), "OnTarget")]
class DiscreteWeaponComponentOnTarget
{
    static bool Prefix(DiscreteWeaponComponent __instance, Vector3 aimPoint)
    {
        Common.LogPatch();
        //if (__instance?.gameObject?.GetComponent<MultiTarget>()?.UpdateTargets(true) ?? Common.RunFunction == Common.SkipFunction )
        //    return __instance?.GetComponentInChildren<DiscreteWeaponEjectors>()?.OnTarget(aimPoint) ?? Common.SkipFunction;

        return __instance?.GetComponentInChildren<DiscreteWeaponEjectors>()?.OnTarget(aimPoint) ?? Common.RunFunction;
        //Common.SetVal(__instance, "Mode", TrackingMode.Ping);
    }
}
[HarmonyPatch(typeof(WeaponComponent), "GetNextMuzzle")]
class WeaponComponentGetNextMuzzle
{
    static void Prefix(WeaponComponent __instance)
    {
        Common.LogPatch();
        __instance?.gameObject?.GetComponent<MultiTarget>()?.UpdateTargets(true);
        //Common.SetVal(__instance, "Mode", TrackingMode.Ping);
    }
}
[HarmonyPatch(typeof(WeaponComponent), "FixedUpdate")]
class WeaponComponentFixedUpdate
{
    static void Postfix(WeaponComponent __instance)
    {
        //Common.LogPatch();
        //Common.Trace(__instance, "fixedupate");
        if (__instance?.gameObject?.GetComponent<MultiTarget>() is MultiTarget multiTarget)
        {
            //Common.Trace(__instance, "target found");

            multiTarget.UpdateTargets(false);

        }
    }
}
