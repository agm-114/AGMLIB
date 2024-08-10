using HarmonyLib;
using Munitions.ModularMissiles;
using Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ModularMissileOptions : MonoBehaviour
{
    public bool SupportsVisualTargeting = true;
}
[HarmonyPatch(typeof(ModularMissile))]//, nameof(BaseCellLauncherComponent.)
[HarmonyPatch("SupportsVisualTargeting", MethodType.Getter)]
class ModularMissileSupportsVisualTargeting
{
    public static void Postfix(ref bool __result, ModularMissile __instance)
    {
        ModularMissileOptions options = __instance.gameObject.GetComponent<ModularMissileOptions>();
        if (options != null)
        {
            __result = options.SupportsVisualTargeting;
        }
        //__result = true;
    }
}   