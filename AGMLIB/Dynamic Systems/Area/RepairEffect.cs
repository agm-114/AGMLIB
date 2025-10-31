using Lib.Dynamic_Systems.Area;

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

    public class RepairEffect : AmmoConsumingFalloffEffect
    {
        public int MaxRepairsPerHull = 3;
        public float RepairPerSecond = 1;
        public float RestoresPerSecond = 0.2f;
        protected float _accum = 0;

        public float RestoreAmmoMultiplier = 1;
        public float RepairAmmoMultiplier = 1;
        public BaseFilter? RestoreAmmoFilter = null;
        public BaseFilter? RepairAmmoFilter = null;
        public AmmoFeeder AmmoFeed => AreaEffect.Hull.MyShip.AmmoFeed;

        public override void FixedUpdate()
        {
            //
            _accum += Time.fixedDeltaTime;
            if (_accum < 1)
                return;
            _accum -= 1;
            if (Active)
            {
                //Debug.LogError("Generic repair Fixed Update");
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
                    if (parts.Any())
                        ships = ships.Append(parts);
                    else
                    {
                        IEnumerable<RestoreStatus> hulllockers = root.GetComponentsInChildren<DCLockerComponent>().ToList().OrderByDescending(h => h.DCPriority).ConvertAll(locker => RestoreStatus.GetRestoreStatus(locker)).Where(status => status.NeedsRestores).Take(1);
                        if (hulllockers.Any())
                            lockers = lockers.Append(hulllockers.First());
                    }

                }
                float repairsperhull = RepairPerSecond / (ships.Count());
                foreach (IEnumerable<HullPart> repairparts in ships)
                {
                    float repairsperpart = repairsperhull / repairparts.Count();

                    //continue;


                    foreach (HullPart hullPart in repairparts)
                    {
                        float actualrepair = Math.Min(hullPart.MaxHealth - hullPart.CurrentHealth, repairsperpart); 
                        if (RestoreAmmoFilter != null)
                        {
                            IMagazine? repairmag = GetAmmoSource(RestoreAmmoFilter);
                            if (repairmag == null || repairmag.QuantityAvailable < 1)
                                continue;
                            DiscreteReload(actualrepair * RepairAmmoMultiplier, repairmag);

                        }
                        hullPart.DoHeal(actualrepair);
                    }
                        
                }
                float restoresperlocker = RestoresPerSecond / (lockers.Count());

                foreach (RestoreStatus status in lockers)
                {
                    
                    if (RepairAmmoFilter != null)
                    {
                        IMagazine? restoremagazine = GetAmmoSource(RepairAmmoFilter);
                        if(restoremagazine == null || restoremagazine.QuantityAvailable < 1)
                            continue;
                        DiscreteReload(restoresperlocker * RestoreAmmoMultiplier, restoremagazine);
                    }
                    status.DoPartialRestore(restoresperlocker);
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
