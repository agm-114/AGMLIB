using Game.Units;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

//[HarmonyPatch(typeof(DynamicVisibleParticles), "UpdatePlaying")]
class DynamicVisibleParticlesUpdatePlaying
{
    public static void Prefix(ref DynamicVisibleParticles __instance)
    {
        //Debug.LogError(__instance.gameObject.name);
        //Debug.LogError(__instance.gameObject.transform.parent.gameObject.name);

        //__instance.GetComponentInChildren<FixedCellLauncherComponent>().Slurp();
        //__instance.GetComponentInChildren<FixedCellLauncherComponent>().OnAmmoQuantityChanged.Invoke()
    }
}


//[HarmonyPatch(typeof(ShipController), nameof(ShipController.Initialize))]
class ShipControllerInitialize
{
    public static void Prefix(ShipController __instance)
    {
        //__instance.GetComponentInChildren<FixedCellLauncherComponent>().Slurp();
        //__instance.GetComponentInChildren<FixedCellLauncherComponent>().OnAmmoQuantityChanged.Invoke()
    }
}
