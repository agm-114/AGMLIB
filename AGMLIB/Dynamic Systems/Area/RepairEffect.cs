namespace AGMLIB.Dynamic_Systems.Area
{

    public class RestoreStatus : MonoBehaviour
    {
        public DCLockerComponent LockerComponent;
        public int RestoreCount = 0;
        int _restoresRemaining => Common.GetVal<int>(LockerComponent, "_restoresRemaining");
        public bool NeedsRestores => RestoreCount > _restoresRemaining;
        public float RestoreStock = 0;

        public static RestoreStatus GetRestoreStatus(DCLockerComponent lockerComponent)
        {
            RestoreStatus restoreStatus = lockerComponent.GetComponent<RestoreStatus>();
            if (restoreStatus == null)
            {
                restoreStatus = lockerComponent.gameObject.AddComponent<RestoreStatus>();
                restoreStatus.LockerComponent = lockerComponent;
                restoreStatus.RestoreCount = Common.GetVal<int>(lockerComponent, "_restoreCount");

            }
            return restoreStatus;
        }

        public void DoPartialRestore(float partialrestore)
        {
            RestoreStock += partialrestore;

        }

        public void FixedUpdate()
        {
            if (RestoreStock > 1)
            {
                Common.SetVal(LockerComponent, "_restoreCount", _restoresRemaining + 1);
                RestoreStock -= 1;
            }
        }
    }

        public class RepairEffect : FalloffEffect<Ship>
    {
        public int MaxRepairsPerHull = 3;
        public float RepairPerSecond = 1;
        public float RestoresPerSecond = 0.2f;
        protected float _accum = 0;
        public override void FixedUpdate()
        {
            //Debug.LogError("Generic Fixed Update");
            _accum += Time.fixedDeltaTime;
            if(_accum < 1)
                return;
            _accum -= 1;
            if (Active)
            {
                IEnumerable<IEnumerable<HullPart>> ships = new List<IEnumerable<HullPart>>();
                IEnumerable<RestoreStatus> lockers = new List<RestoreStatus>();
                foreach (Ship target in Targets.Where(target => target != null))
                {
                    Transform root = target.gameObject.transform.root;
                    IEnumerable<HullPart> parts = root.GetComponentsInChildren<HullPart>().Where(a => a.HealthPercentage < 1);
                    if (parts.Count() > MaxRepairsPerHull)
                    {
                        parts = parts.OrderBy(h => h.HealthPercentage);

                        parts = parts.OrderByDescending(h => h.DCPriority);
                        parts = parts.Take(MaxRepairsPerHull);
                    }
                    if(parts.Count() > 0)
                        ships.Append(parts);
                    else
                    {
                        IEnumerable<RestoreStatus> shiplockers = root.GetComponentsInChildren<DCLockerComponent>().ToList().OrderByDescending(h => h.DCPriority).ConvertAll(locker => RestoreStatus.GetRestoreStatus(locker)).Where(status => status.NeedsRestores).Take(1);
                        if(shiplockers.Count() > 0)
                        {
                            lockers.First();
                        }
                    }

                }
                float repairsperhull =  RepairPerSecond / (ships.Count() + lockers.Count());
                foreach(IEnumerable<HullPart> repairparts in ships)
                {
                    float repairsperpart = repairsperhull / repairparts.Count();

                    foreach(HullPart hullPart in repairparts)
                        hullPart.DoHeal(repairsperpart);
                }
                float restoresperhull = RestoresPerSecond / (ships.Count() + lockers.Count());

                foreach (RestoreStatus status in lockers)
                {
                    status.DoPartialRestore(restoresperhull);
                }
            }

        }

        /*
        public override void TargetFixedUpdate(Ship target)
        {
           Debug.LogError("Repairing Ship");

             = ;


            foreach (HullPart hullComponent in hullParts) //
            {

                if (hullComponent.HealthPercentage >= 1f)
                {
                    Debug.Log("Not Repairing Component " + hullComponent.name + " " + hullComponent.HealthPercentage);
                    continue;


                }
                Debug.LogError("Reparing Component " + hullComponent.name + " " + hullComponent.HealthPercentage);

                List<IRepairJob> repairJobs = new();
                hullComponent.GetAvailableDCJobs(ref repairJobs, ref repairJobs, true, false);
                hullComponent.DoHeal(RepairPerSecond);
                foreach (IRepairJob job in repairJobs.Where(a => !a.IsRestoration && a.Name.Contains("Repairing") && a.Name.Contains("in")))
                {
                    //job.DoRepairWork(1);

                }

            }
 
        }
        */

    }
}
