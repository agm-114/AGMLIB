using FleetEditor;
using Game;
using HarmonyLib;
using Munitions;
using Ships;
using Ships.Controls;
using Ships.Serialization;
using Steamworks.Ugc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Ships.BulkMagazineComponent;

namespace AGMLIB.Dynamic_Systems.Area
{
    public class ResupplyEffect : FalloffEffect<Ship>
    {

        public override void TargetFixedUpdate(Ship target)
        {
            if (target == AreaEffect.Ship)
                return;

            if (AreaEffect.ShipController?.GetIFF(target?.Controller?.OwnedBy) == IFF.Enemy)
                return;
            //Debug.LogError("Resupply Ship");

            bool dirtymissiles = false;

            foreach (BulkMagazineComponent hullComponent in target.gameObject.transform.root.GetComponentsInChildren<BulkMagazineComponent>())
            {

                BulkMagazineData magdata = hullComponent.gameObject.GetComponent<StartState>().saveData as BulkMagazineData;
                List<IMagazine> mags = hullComponent.Magazines.ToList();

                foreach (IMagazine mag in hullComponent.Magazines)
                {
                    if(mag.QuantityAvailable < mag.PeakQuantity)
                    {
                        Debug.LogError($"{AreaEffect.Ship.gameObject.name} Resupply Ship {target.gameObject.name} {mag.AmmoType.MunitionName} {mag.QuantityAvailable}/{mag.PeakQuantity}");
                        hullComponent.AddToMagazine(mag.AmmoType, 1u);

                        AreaEffect.Hull.MyShip.AmmoFeed.GetAmmoSource(mag.AmmoType).Withdraw(1u);
                       
                    }
                }

                foreach (Magazine.MagSaveData targetdata in magdata.Load)
                {
                    IMunition munition = AreaEffect.Hull.MyShip.Fleet.AvailableMunitions.GetMunition(targetdata.MunitionKey);

                    IMagazine mag = mags.Find(x => x.AmmoType == munition);

                    //if(mag.)
                    //hullComponent.AddToMagazine(mags, 1);
                }
            }
            foreach (CellLauncherComponent hullComponent in target.gameObject.transform.root.GetComponentsInChildren<CellLauncherComponent>())
            {
                IMagazineProvider magazineProvider = hullComponent as IMagazineProvider;

                foreach (IMagazine mag in hullComponent.Missiles)
                {
                    dirtymissiles = true;
                    if (mag.QuantityAvailable < mag.PeakQuantity)
                    {
                        magazineProvider.AddToMagazine(mag.AmmoType, 1);
                        AreaEffect.Hull.MyShip.AmmoFeed.GetAmmoSource(mag.AmmoType).Withdraw(1);
                    }


                }

            }
            //target.BuildMissileGroups();
            if(dirtymissiles)
                target?.BuildMissileMagazineTracker();

        }




    }

    public class StartState : MonoBehaviour
    {
        public ComponentSaveData saveData;
        public HullComponent hullComponent;
    }

    [HarmonyPatch(typeof(HullComponent), nameof(HullComponent.LoadSaveData))]
    class HullComponentLoadSaveData
    {
        static void Postfix(HullComponent __instance, ComponentSaveData data)
        {
            StartState startState = __instance.GetComponent<StartState>();
            if (startState == null)
                startState = __instance.gameObject.AddComponent<StartState>();
            startState.saveData = data;
            startState.hullComponent = __instance;
        }
    }
}
