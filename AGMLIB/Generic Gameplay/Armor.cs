using Game.Units;
using HarmonyLib;
using Munitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Armor : MonoBehaviour
{
}

[HarmonyPatch(typeof(ShipController), "WouldArmorHitPenetrate")]
class ShipControllerWouldArmorHitPenetrate
{
    static void Prefix(ShipController __instance, MunitionHitInfo hitInfo, IDamageDealer character)
    {

    }
}

//WouldArmorHitPenetrate
