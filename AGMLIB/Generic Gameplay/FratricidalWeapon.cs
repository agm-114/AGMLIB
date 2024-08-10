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
    static void Postfix(ActiveEWarEffect __instance, ref bool __result, IEWarTarget target, SignatureType ____sigType)
    {
        FratricidalWeapon weapon = __instance.GetComponent<FratricidalWeapon>();
        if (weapon == null || !weapon.Targetfriendlies)
        {
            //Debug.LogError("NO  FratricidalWeapon");
            return;
        }

        //Debug.LogError("Jamming " + target.ToString());
        //__result = true;
        if (target is IJammable jammable && jammable.SigType == ____sigType && jammable.CanJammerHitAperture(__instance.transform.position.To(jammable.Position)))
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
    public bool Affectfriendlies = false;
    public bool Targetfriendlies = false;
    [HideInInspector]
    public WeaponComponent Source;

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

    static void Prefix(RezFollowingMuzzle __instance, NetworkPoolable ____followingInstance) => FratricidalWeapon.RemoveFratricidalWeapon(____followingInstance);

    static void Postfix(RezFollowingMuzzle __instance, NetworkPoolable ____followingInstance)
    {
        if (____followingInstance == null)
            return;
        //TEMP
        //__instance.gameObject.GetOrAddComponent<FratricidalWeapon>().affectfriendlies = true;

        //TEMP
        FratricidalWeapon weapon = __instance.GetComponentInChildren<FratricidalWeapon>() 
                                ?? __instance.GetComponentInParent<FratricidalWeapon>();
        if (weapon == null)
            return;
        FratricidalWeapon followingweapon = ____followingInstance.gameObject.GetOrAddComponent<FratricidalWeapon>();
        followingweapon.Source = __instance.GetComponentInParent<WeaponComponent>();
        followingweapon.Affectfriendlies = weapon.Affectfriendlies;
    }
}

[HarmonyPatch(typeof(RezFollowingMuzzle), nameof(RezFollowingMuzzle.StopFire))]
class RezFollowingMuzzleStopFire
{
    static void Prefix(RezFollowingMuzzle __instance, NetworkPoolable ____followingInstance) => FratricidalWeapon.RemoveFratricidalWeapon(____followingInstance);
}
