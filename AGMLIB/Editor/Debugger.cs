using Ships;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FleetEditor;
using Munitions;
using System.Linq;
using System.Reflection;
using Utility;
using HarmonyLib;
using System.Diagnostics;

using UI;
using Debug = UnityEngine.Debug;
using static UnityEngine.ParticleSystem;
using Bundles;
using static UnityEngine.EventSystems.EventTrigger;
using System.Runtime.CompilerServices;
using Game;
using Ships.Serialization;
using Game.Units;
using Mirror;
using System.Xml.Linq;
using Factions;
using Procedural.Naming;
using Steamworks;
using System;
using UnityEngine.UI;
using Game.UI;

public class KeyDebugger : ShipState
{
    // Start is called before the first frame update
    public string key = "";
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GameObject dcboardgo = Common.GetVal<GameObject>(Ship.Hull, "_damageControlBoardPrefab");
        if (dcboardgo == null)
        {
            Debug.LogError("No DC Board Detected");

        }
        List<ShipStatusDisplayPart> display = dcboardgo?.GetComponentsInChildren<ShipStatusDisplayPart>().ToList();
        if(display == null || display.Count == 0)
        {
            Debug.LogError("No ShipStatusDisplayPart Detected");

        }
        List<String> keys = display.ConvertAll(displaypart => displaypart.PartKeys.ToList()).SelectMany(a => a).ToList();
        foreach (string key in Ship.Hull.AllParts.ConvertAll(part => part.Key))
        {
            if (keys.Contains(key))
            {

                continue; 
            }
            else
            {
                Debug.LogError("Missing part key on dc board " + key);
            }
        }
        foreach (string key in Ship.Hull.AllSockets.ConvertAll(part => part.Key))
        {
            if (keys.Contains(key))
            {

                continue;
            }
            else
            {
                Debug.LogError("Missing socket key on dc board " + key);
            }
        }
        
    }
}

//[HarmonyPatch(typeof(HullComponent), nameof(HullComponent.GetFormattedDesc))]
class Described
{
    static void Postfix(HullComponent __instance, ref string __result) => __result += "<color=" + GameColors.RedTextColor + ">Colored Text</color>\n";
}