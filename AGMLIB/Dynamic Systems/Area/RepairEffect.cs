using Game.Units;
using Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AGMLIB.Dynamic_Systems.Area
{

    public class RepairEffect : FalloffEffect<Ship>
    {

        public override void TargetFixedUpdate(Ship target)
        {
           Debug.LogError("Repairing Ship");
            
            foreach (HullPart hullComponent in target.gameObject.transform.root.GetComponentsInChildren<HullPart>()) //.Where(a => a.HealthPercentage < 1)
            {

                if (hullComponent.HealthPercentage >= 1f)
                {
                    Debug.Log("Not Repairing Component " + hullComponent.name + " " + hullComponent.HealthPercentage);
                    continue;


                }
                Debug.LogError("Reparing Component " + hullComponent.name + " " + hullComponent.HealthPercentage);

                List<IRepairJob> repairJobs = new();
                hullComponent.GetAvailableDCJobs(ref repairJobs, ref repairJobs, true, false);
                hullComponent.DoHeal(1);
                foreach (IRepairJob job in repairJobs.Where(a => !a.IsRestoration && a.Name.Contains("Repairing") && a.Name.Contains("in")))
                {
                    job.DoRepairWork(1);

                }

            }
 
        }


    }
}
