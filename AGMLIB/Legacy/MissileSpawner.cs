using Game.Intel;
using Game;
using Munitions;
using UnityEngine;
using Game.Sensors;
using Game.Reports;
using Game.Units;
using Mirror;
using Mirror.RemoteCalls;
using Utility;
using System.Reflection;
using System;
using System.Linq;
using Game.AI;
using Game.UI;
using Game.Orders;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using Ships;

using Modding;

using Debug = UnityEngine.Debug;
using HarmonyLib;
using FleetEditor;



/*
[HarmonyPatch(typeof(MissileSpawner), "IDamageable.DoDamage")]
class PDPatch
{
    static void Prefix(IDamageable __instance, MunitionHitInfo hitInfo, IDamageDealer character, out float damageDone, out bool destroyed)
    {
        damageDone = 0;
        destroyed = false;

        MissileSpawner spawner = __instance.GameObj.GetComponentInChildren<MissileSpawner>();
        Debug.LogError("shiphit");
        //if (spawner != null)
        //    spawner.target.DoDamage(hitInfo, character, out damageDone, out destroyed);

    }

}
*/


public class MissileSpawner : MonoBehaviour
{
    public GameObject missile;
    private ShipController _myShip;

    [HideInInspector]
    public PDTarget target;

    //ShipController;

    //protected override bool _wasSoftkilled => false;  
    void Start()
    {
        _myShip = gameObject.GetComponentInParent<ShipController>();

        if (_myShip == null || gameObject.GetComponentInParent<EditorShipController>() != null)
            return;
        base.StartCoroutine(LaunchMissile());
    }


    private IEnumerator LaunchMissile()
    {
        yield return new WaitForSeconds(5);
        //Debug.LogError("Launching Missile");
        IMissile imissile = missile.GetComponent<IMissile>();
        NetworkPoolable missileObj = imissile.InstantiateSelf(base.transform.position, base.transform.rotation, Vector3.zero);
        //Debug.LogError("Getting Ins Missile");
        IMissile basicMissile = missileObj.GetComponent<IMissile>();

        
        if (basicMissile != null && _myShip != null)
        {
            //Debug.LogError("Imbuing");
            //Debug.LogError("Imbuing Ship");
            imissile.Imbue(_myShip);
            //Debug.LogError("Imbuing Ship2");
            target = missileObj.GetComponent<PDTarget>();
            target.ShipController = _myShip;
            //imissile.Imbue(_myShip.NetID);
        }
        //else
        //    Debug.LogError("Missile failed to spawn");
        //Debug.LogError("Grabbing Ridgidbody");
        missileObj.GetComponent<Rigidbody>().isKinematic = false ;
        //if (body == null)
        //    Debug.LogError("No rigidbody on missile, cannot launch");

        //Debug.LogError("Launching Missile");
        //callback?.Invoke(basicMissile);
        basicMissile?.Launch(_myShip.NetID, 0, false);
    }

}