using Lib.Generic_Gameplay.Discrete;
using Mirror.RemoteCalls;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Object = System.Object;


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
        MuzzleEffects.SpawnImpacts(__instance , __result);

    }
}


[HarmonyPatch(typeof(BallisticRaycastMuzzle), "DamageableImpact")]
class BallisticRaycastMuzzleDamageableImpact
{
    static void Postfix(RaycastMuzzle __instance, MunitionHitInfo hitInfo)
    {
        Common.LogPatch();
        MuzzleEffects.SpawnImpacts(__instance, hitInfo);

    }
}

[HarmonyPatch(typeof(BallisticRaycastMuzzle), "GenericImpact")]
class BallisticRaycastMuzzleGenericImpact
{
    static void Postfix(BallisticRaycastMuzzle __instance, MunitionHitInfo hitInfo)
    {
        Common.LogPatch();
        MuzzleEffects.SpawnImpacts(__instance, hitInfo);

    }
}


[HarmonyPatch(typeof(BallisticRaycastMuzzle), "CoroutineUpdateBullets")]
class BallisticRaycastMuzzleFixedUpdate
{

    public class RaycastBulletObject
    {
        public float Lifetime;

        public Vector3 Position;

        public Vector3 Direction;

        public float Velocity;

        public float ExpectedFlightTime;

        public RaycastBulletObject(Object bullet)
        {
            Lifetime = Common.GetVal<float>(bullet, "Lifetime");
            Position = Common.GetVal<Vector3>(bullet, "Position");
        }
    }

    static void Prefix(BallisticRaycastMuzzle __instance)
    {
        Common.LogPatch();

        //Common.Hint("updating bullet queue");


        object _activeBullets = Common.GetVal<object>(__instance, "_activeBullets");
        var enumerable = _activeBullets as System.Collections.IEnumerable;

        foreach (var bulletobj in enumerable)
        {
            RaycastBulletObject bullet = new RaycastBulletObject(bulletobj);
            if((bullet.Lifetime - Time.fixedDeltaTime) <= 0)
            {
                continue;
            }
        
            MunitionHitInfo hitInfo = new MunitionHitInfo();
            hitInfo.Point = bullet.Position;
            MuzzleEffects.SpawnImpacts(__instance, hitInfo);

        }
        //MuzzleEffects.SpawnImpacts(__instance, hitInfo);

    }
}

