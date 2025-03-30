using Lib.Generic_Gameplay.Discrete;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




//[HarmonyPatch(typeof(Muzzle), nameof(Muzzle.FireEffect))]
class MuzzleFireEffect
{
    static void Postfix(Muzzle __instance)
    {
        Common.LogPatch();
        MuzzleEffects.FireEffects(__instance);
    }
}
[HarmonyPatch(typeof(RaycastMuzzle), nameof(RaycastMuzzle.Fire))]
class RaycastMuzzleFire
{
    static void Postfix(RaycastMuzzle __instance)
    {
                Common.LogPatch();
        if (__instance is SinglePulseRaycastMuzzle singlePulseRaycastMuzzle)
        {
            MuzzleEffects.FireEffects(__instance);
        }
    }
}
[HarmonyPatch(typeof(RaycastMuzzle), nameof(RaycastMuzzle.FireEffect))]
class RaycastMuzzleFireEffect
{
    static void Postfix(RaycastMuzzle __instance) {
        Common.LogPatch();
        //Common.Trace("RaycastMuzzleFireEffect");
        MuzzleEffects.FireEffects(__instance);
            
    }
}
[HarmonyPatch(typeof(RezzingMuzzle), nameof(RezzingMuzzle.FireEffect))]
class RezzingMuzzleFireEffect
{
    static void Postfix(RezzingMuzzle __instance) {
        Common.LogPatch();
        //Common.Trace("RezzingMuzzleFireEffect");
        MuzzleEffects.FireEffects(__instance);
    } 
}
[HarmonyPatch(typeof(SinglePulseRaycastMuzzle), nameof(SinglePulseRaycastMuzzle.FireEffect))]
class SinglePulseRaycastMuzzleFireEffect
{
    static void Postfix(SinglePulseRaycastMuzzle __instance)
    {
        Common.LogPatch();
        //Common.Trace("RezzingMuzzleFireEffect");
        MuzzleEffects.FireEffects(__instance);
    }
}





[HarmonyPatch(typeof(RaycastMuzzle), "DoRaycast")]
class RaycastMuzzleDoRaycast
{
    static void Postfix(RaycastMuzzle __instance, MunitionHitInfo __result)
    {
        Common.LogPatch();
        if (__result == null)
            return;
        //Common.Trace("RaycastMuzzleDoRaycast");

        MuzzleEffects.SpawnImpacts(__instance , __result);

    }
}


