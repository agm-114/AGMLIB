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

public class Debugger : MonoBehaviour
{
    // Start is called before the first frame update
    public string key = "";
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

//[HarmonyPatch(typeof(HullComponent), nameof(HullComponent.GetFormattedDesc))]
class Described
{
    static void Postfix(HullComponent __instance, ref string __result)
    {

        __result += "<color=" + GameColors.RedTextColor + ">Colored Text</color>\n";
    }
}