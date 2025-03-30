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

public interface IDynamicCollectablePart
{
    void Initialize();
}

//[HarmonyPatch(typeof(ShipController), nameof(ShipController.Initialize))]
class ShipControllerInitialize
{
    public static void Prefix(ShipController __instance)
    {
        var parts = __instance.gameObject.GetComponentsInChildren<IDynamicCollectablePart>(includeInactive:true);
        //Debug.LogError("Creating " + parts.Length + " children");

        foreach (var part in parts)
        {

            part.Initialize();
        }

        //__instance.GetComponentInChildren<FixedCellLauncherComponent>().Slurp();
        //__instance.GetComponentInChildren<FixedCellLauncherComponent>().OnAmmoQuantityChanged.Invoke()
    }
}

[HarmonyPatch(typeof(Hull), "GetAllSubPartsInternal")]
class HullGetAllSubPartsInternal
{
    public static void Prefix(Hull __instance)
    {
        Common.LogPatch();
        var parts = __instance.gameObject.GetComponentsInChildren<IDynamicCollectablePart>(includeInactive: true);
        //Debug.LogError("Creating " + parts.Length + " children");

        foreach (var part in parts)
        {

            part.Initialize();
        }

        //__instance.GetComponentInChildren<FixedCellLauncherComponent>().Slurp();
        //__instance.GetComponentInChildren<FixedCellLauncherComponent>().OnAmmoQuantityChanged.Invoke()
    }
}


