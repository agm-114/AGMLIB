using Munitions;
using Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AGMLIB.Dynamic_Systems.Area
{

    public class ResupplyEffect : FalloffEffect<Ship>
    {

        public override void TargetFixedUpdate(Ship target)
        {
            Debug.LogError("Repairing Ship");

            foreach (BulkMagazineComponent hullComponent in target.gameObject.transform.root.GetComponentsInChildren<BulkMagazineComponent>()) //.Where(a => a.HealthPercentage < 1)
            {

                foreach(IMagazine mag in hullComponent.Magazines)
                {
                    hullComponent.AddToMagazine(mag.AmmoType, 1);
                }
            }

        }


    }
}
