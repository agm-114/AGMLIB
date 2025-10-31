using Lib.Dynamic_Systems.Area;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace AGMLIB.Dynamic_Systems.Area
{
    public class ResupplyEffect : AmmoConsumingFalloffEffect
    {
        public float PointsPerSecond = 1;

        private float ReloadPoints = 0;
        private float _accumulator = 0;

        public bool BulkMagazines = true;
        public bool CellLaunchers = true;
        public BaseFilter? GenericAmmoFilter = null;

        public float SpecficAmmoMultiplier = 1;
        public float GenericAmmoMultiplier = 1;
        public bool UseSpecificAmmoFirst = true;

        public BaseFilter?  TargetAmmoFilter = null;



        void CalcCommonStats(IMagazine sink, out uint sinkquantitylimit, out float componentratelimit, out List<float> limits)
        {
            
            sinkquantitylimit = (uint)(sink.PeakQuantity - sink.QuantityAvailable);
            componentratelimit = ReloadPoints / (float)sink.AmmoType.PointCost;
            limits = new() { sinkquantitylimit, componentratelimit };
        }

        float ReplaceSpecfic(IMagazine sink, IMagazine source, HullComponent hullComponent, out bool dirtymissiles)
        {
            CalcCommonStats(sink, out uint maxsinkshells, out float reloadratelimit, out List<float> limits);
          
            float truereloadamount = limits.Append(source.QuantityAvailable).Min() * SpecficAmmoMultiplier;
            uint reloadamount = DiscreteReload(truereloadamount, source);

            //BulkMagazineData magdata = hullComponent.gameObject.GetComponent<StartState>().saveData as BulkMagazineData;
            //List<IMagazine> mags = hullComponent.Magazines.ToList();
            dirtymissiles = false;

            //Debug.LogError($"{AreaEffect.Ship.gameObject.name} Resupply Ship {target.gameObject.name} {sink.AmmoType.MunitionName} {sink.QuantityAvailable}/{sink.PeakQuantity}");
            if (hullComponent is BulkMagazineComponent magcomp)
                magcomp.AddToMagazine(sink.AmmoType, reloadamount);
            else if (hullComponent is CellLauncherComponent cellcomp)
            {
                IMagazineProvider magazineProvider = cellcomp as IMagazineProvider;
                //Debug.LogError("movin missiles");
                dirtymissiles = true;

                magazineProvider.AddToMagazine(sink.AmmoType, reloadamount);
            }

            
            return reloadamount * sink.AmmoType.PointCost;
        }

        float ReplaceGeneric(IMagazine sink, IMagazine source, HullComponent hullComponent, out bool dirtymissiles)
        {
            //Common.Hint($"{sink.PeakQuantity}-{sink.QuantityAvailable} of {sink.AmmoType.MunitionName} needs resupply");
            int avaliablepoints = source.QuantityAvailable * source.AmmoType.PointCost;
            int avaliablepointsratelimit = avaliablepoints / sink.AmmoType.PointCost;
            CalcCommonStats(sink, out uint sinkquantitylimit, out float componentratelimit, out List<float> limits);
            
            
            
            uint replacereloadamount = (uint)limits.Append((uint)avaliablepointsratelimit).Min();

            float removereloadamount = ((float)(replacereloadamount * source.AmmoType.PointCost) / ((float)source.AmmoType.PointCost)) * GenericAmmoMultiplier;
            //Common.Hint($"Generic reload calc srl {sinkquantitylimit} crl {componentratelimit} ap {avaliablepoints} rpra {replacereloadamount} rvra {removereloadamount}");

            DiscreteReload(removereloadamount, source);
            //BulkMagazineData magdata = hullComponent.gameObject.GetComponent<StartState>().saveData as BulkMagazineData;
            //List<IMagazine> mags = hullComponent.Magazines.ToList();
            dirtymissiles = false;
            //Debug.LogError($"{AreaEffect.Ship.gameObject.name} Resupply Ship {target.gameObject.name} {sink.AmmoType.MunitionName} {sink.QuantityAvailable}/{sink.PeakQuantity}");
            if (hullComponent is BulkMagazineComponent magcomp)
                magcomp.AddToMagazine(sink.AmmoType, replacereloadamount);
            else if (hullComponent is CellLauncherComponent cellcomp)
            {
                IMagazineProvider magazineProvider = cellcomp as IMagazineProvider;
                //Debug.LogError("movin missiles");
                dirtymissiles = true;

                magazineProvider.AddToMagazine(sink.AmmoType, replacereloadamount);
            }

            return replacereloadamount * sink.AmmoType.PointCost;
        }
        private float _reloadtick = 4f;

        public override void FixedUpdate()
        {
            _accumulator += Time.fixedDeltaTime;
            base.FixedUpdate();
            if(_accumulator >= _reloadtick)
            {
                _accumulator = 0;
            }

            
        }

        public override void TargetFixedUpdate(Ship target)
        {
            if (_accumulator < _reloadtick)
            {
                return;
            }

            ReloadPoints += PointsPerSecond * _accumulator;
            if (target == AreaEffect.Ship || target == null)
                return;

            if (AreaEffect.ShipController?.GetIFF(target.Controller?.OwnedBy) == IFF.Enemy)
                return;

            Transform targettransform = target.gameObject.transform.root;
            //Debug.LogError("Resupply Ship");
            //Common.Hint("Resupply Ship");

            //Common.Hint($"{AreaEffect.Ship.ShipName} Resupplying {target.ShipName}");

            bool dirtymissiles = false;

            List<IEnumerable<KeyValuePair<IMagazine, HullComponent>>> test1 = new();
            if (BulkMagazines)
                test1.AddRange(targettransform.GetComponentsInChildren<BulkMagazineComponent>().ConvertAll(comp => comp.Magazines.Select(mag => new KeyValuePair<IMagazine, HullComponent>(mag, comp))));
            if (CellLaunchers)
                test1.AddRange(targettransform.GetComponentsInChildren<CellLauncherComponent>().ConvertAll(comp => comp.Missiles.Select(mag => new KeyValuePair<IMagazine, HullComponent>(mag, comp))));
            IEnumerable<KeyValuePair<IMagazine, HullComponent>> test2 = test1.SelectMany(list => list).Where(pair => pair.Key.QuantityAvailable < pair.Key.PeakQuantity);
            if (TargetAmmoFilter != null)
                test2 = test2.Where(pair => TargetAmmoFilter.IsAmmoCompatible(pair.Key.AmmoType));
            IOrderedEnumerable<KeyValuePair<IMagazine, HullComponent>> test3 = test2.OrderBy(kvp => kvp.Key.PercentageAvailable);


            IEnumerable<IMunition> genericammo = Array.Empty<IMunition>();




            foreach (var kvp in test3)
            {
                IMagazine sink = kvp.Key;
                IMagazine? source = AmmoFeed.GetAmmoSource(sink.AmmoType);
                HullComponent hullComponent = kvp.Value;

                //if (source == null)
                //    Common.Hint(this, $"No specific ammo for {sink.AmmoType.MunitionName}");

                if (source == null && GenericAmmoFilter != null)
                {
                    //Common.Hint(this, "Looking for ammo");
                    source = GetAmmoSource(GenericAmmoFilter);
                    if( source != null)
                    {
                        //Common.Hint(this, $"Found generic ammo {source.AmmoType.MunitionName}" );
                    }
                }
                

                
                if (sink.AmmoType.PointCost > ReloadPoints || sink == null || source == null)
                {
                    //Common.Hint(this, $"Stopping resupply of {target.gameObject.name} with {source?.AmmoType.MunitionName ?? "no ammo"} for {sink.AmmoType.MunitionName}, not enough points {ReloadPoints} or no source");
                    return;
                }
                    
                else
                {
                    //Common.Hint(this, $"Resupplying {target.gameObject.name} with {source.AmmoType.MunitionName} for {sink.AmmoType.MunitionName}");
                }

                if(source.AmmoType == sink.AmmoType)
                {
                    ReloadPoints -= ReplaceSpecfic(sink, source, hullComponent, out bool missilestouched);
                    dirtymissiles = dirtymissiles || missilestouched;
                }
                else
                {
                    ReloadPoints -= ReplaceGeneric(sink, source, hullComponent, out bool missilestouched);
                    dirtymissiles = dirtymissiles || missilestouched;

                }
                
            }


            //target.BuildMissileGroups();
            if (dirtymissiles)
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
            Common.LogPatch();
            StartState startState = __instance.GetComponent<StartState>();
            if (startState == null)
                startState = __instance.gameObject.AddComponent<StartState>();
            startState.saveData = data;
            startState.hullComponent = __instance;
        }
    }
}
