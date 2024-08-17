using FleetEditor;
using Game;
using Game.Units;
using HarmonyLib;
using Munitions;
using Ships;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using Utility;

[HarmonyPatch(typeof(ShipController), nameof(ShipController.HasOffensiveAbility))]
class HasOffensiveAbilityTweak
{
    static void Update(ShipController __instance, ref bool __result)
    {
        TacticalModule tacticalModule = __instance.GetComponent<TacticalModule>();
        if (tacticalModule != null && tacticalModule.ForceOffensiveStatus)
        {
            __result = tacticalModule.ForcedOffensiveStatus;
        }
        else
        {
            //__instance.gameObject.AddComponent<HardcodedOffensiveAblity>(); 
        }
        __result = false;
    }
}

[HarmonyPatch(typeof(ObjectivePoint), "OnTriggerEnter")]
class ContestTweak
{
    static bool Prefix(ObjectivePoint __instance, Collider other)
    {
        
        if (!other.isTrigger)
        {
            TacticalModule component = other.gameObject.transform.root.GetComponent<TacticalModule>() ?? other.gameObject.transform.root.GetComponentInChildren<TacticalModule>();
            return component == null || component.CanContestPoints;
        }
        else
            return true;
    }
}

public class TacticalModule : ShipState
{
    [SerializeField]
    public bool ForceOffensiveStatus = false;
    [SerializeField]
    public bool ForcedOffensiveStatus = false;
    [SerializeField]
    public bool CanContestPoints = true;
}