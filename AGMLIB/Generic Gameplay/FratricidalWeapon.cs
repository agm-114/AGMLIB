using FleetEditor;
using Game;
using Game.EWar;
using Game.Sensors;
using Game.Units;
using HarmonyLib;
using Munitions;
using Ships;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Utility;

[HarmonyPatch(typeof(ActiveJammingEffect), "CheckTargetValidity")]
class CheckTargetValidityTweak
{
    static void Postfix(ActiveJammingEffect __instance, ref bool __result, IEWarTarget target, SignatureType ____sigType)
    {
        FratricidalWeapon weapon = __instance.GetComponent<FratricidalWeapon>();
        if (weapon == null || !weapon.affectfriendlies)
        {
            //Debug.LogError("NO  FratricidalWeapon");
            return;
        }
            
        IJammable jammable = target as IJammable;
        //Debug.LogError("Jamming " + target.ToString());
        //__result = true;
        if (jammable != null && jammable.SigType == ____sigType && jammable.CanJammerHitAperture(__instance.transform.position.To(jammable.Position)))
        {
            //Debug.LogError("Valid Jamming " + target.ToString());
            //ActiveJammingEffect
            //ShipController
            __result = true;
        }
    }
}





public class FratricidalWeapon : MonoBehaviour
{
    [SerializeField]
    public bool affectfriendlies = false;
    [SerializeField]
    public bool targetfriendlies = false;

    //ActiveJammingEffect
    //OmnidirectionalEWarComponent
    public static void RemoveFratricidalWeapon(NetworkPoolable ____followingInstance)
    {
        if (____followingInstance == null)
            return;
        FratricidalWeapon followingweapon = ____followingInstance.gameObject.GetComponent<FratricidalWeapon>();
        if (followingweapon != null)
            Destroy(followingweapon);
    }
}

[HarmonyPatch(typeof(RezFollowingMuzzle), nameof(RezFollowingMuzzle.Fire))]
class RezFollowingMuzzleFire : MonoBehaviour
{

    static void Prefix(RezFollowingMuzzle __instance, NetworkPoolable ____followingInstance)
    {
        FratricidalWeapon.RemoveFratricidalWeapon(____followingInstance);
    }

    static void Postfix(RezFollowingMuzzle __instance, NetworkPoolable ____followingInstance)
    {
        if (____followingInstance == null)
            return;
        //TEMP
        //__instance.gameObject.GetOrAddComponent<FratricidalWeapon>().affectfriendlies = true;

        //TEMP
        FratricidalWeapon weapon = __instance.GetComponentInChildren<FratricidalWeapon>();
        if (weapon == null)
            weapon = __instance.GetComponentInParent<FratricidalWeapon>();
        if (weapon == null)
            return;
        FratricidalWeapon followingweapon = ____followingInstance.gameObject.GetOrAddComponent<FratricidalWeapon>();
        followingweapon.affectfriendlies = weapon.affectfriendlies;

    }

}

[HarmonyPatch(typeof(RezFollowingMuzzle), nameof(RezFollowingMuzzle.StopFire))]
class RezFollowingMuzzleStopFire
{
    static void Prefix(RezFollowingMuzzle __instance, NetworkPoolable ____followingInstance)
    {
        FratricidalWeapon.RemoveFratricidalWeapon(____followingInstance);
    }
}
