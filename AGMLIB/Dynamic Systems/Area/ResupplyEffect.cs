using Lib.Dynamic_Systems.Area;
using Munitions;
using Ships;
using System.Security.Cryptography;

namespace AGMLIB.Dynamic_Systems.Area
{
    public class ResupplyEffect : AmmoConsumingFalloffEffect
    {
        public float PointsPerSecond = 1;

        private float ReloadPoints = 0;

        public bool BulkMagazines = true;
        public bool CellLaunchers = true;
        public List<MunitionTags> GenericAmmoTypes = new();

        public float SpecficAmmoMultiplier = 1;
        public float GenericAmmoMultiplier = 1;
        public bool UseSpecificAmmoFirst = true;

        [SerializeField] protected BaseFilter AmmoFilter;



        void CalcCommonStats(IMagazine sink, out uint maxsinkshells, out float reloadratelimit, out List<float> limits)
        {
            
            maxsinkshells = (uint)(sink.PeakQuantity - sink.QuantityAvailable);
            reloadratelimit = ReloadPoints / (float)sink.AmmoType.PointCost;
            limits = new() { maxsinkshells, reloadratelimit };
        }

        float ReplaceSpecfic(IMagazine sink, IMagazine source, HullComponent hullComponent, out bool dirtymissiles)
        {
            CalcCommonStats(sink, out uint maxsinkshells, out float reloadratelimit, out List<float> limits);
          
            float truereloadamount = limits.Append(source.QuantityAvailable).Min() * SpecficAmmoMultiplier;
            uint reloadamount = DiscreteReload(truereloadamount, source);

            //BulkMagazineData magdata = hullComponent.gameObject.GetComponent<StartState>().saveData as BulkMagazineData;
            //List<IMagazine> mags = hullComponent.Magazines.ToList();
            dirtymissiles = true;

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
            CalcCommonStats(sink, out uint maxsinkshells, out float reloadratelimit, out List<float> limits);
            
            uint addreloadamount = (uint)limits.Append(maxsinkshells / (source.QuantityAvailable * source.AmmoType.PointCost)).Min();
            float trueremovereloadamount = ((addreloadamount * source.AmmoType.PointCost) / (source.QuantityAvailable * source.AmmoType.PointCost))* GenericAmmoMultiplier;
            uint removereloadamount = DiscreteReload(trueremovereloadamount, source);
            //BulkMagazineData magdata = hullComponent.gameObject.GetComponent<StartState>().saveData as BulkMagazineData;
            //List<IMagazine> mags = hullComponent.Magazines.ToList();
            dirtymissiles = true;
            //Debug.LogError($"{AreaEffect.Ship.gameObject.name} Resupply Ship {target.gameObject.name} {sink.AmmoType.MunitionName} {sink.QuantityAvailable}/{sink.PeakQuantity}");
            if (hullComponent is BulkMagazineComponent magcomp)
                magcomp.AddToMagazine(sink.AmmoType, addreloadamount);
            else if (hullComponent is CellLauncherComponent cellcomp)
            {
                IMagazineProvider magazineProvider = cellcomp as IMagazineProvider;
                //Debug.LogError("movin missiles");
                dirtymissiles = true;

                magazineProvider.AddToMagazine(sink.AmmoType, addreloadamount);
            }

            return addreloadamount * sink.AmmoType.PointCost;
        }

        public override void TargetFixedUpdate(Ship target)
        {
            ReloadPoints += PointsPerSecond * Time.fixedDeltaTime;
            if (target == AreaEffect.Ship || target == null)
                return;

            if (AreaEffect.ShipController?.GetIFF(target.Controller?.OwnedBy) == IFF.Enemy)
                return;

            Transform targettransform = target.gameObject.transform.root;
            //Debug.LogError("Resupply Ship");

            bool dirtymissiles = false;

            List<IEnumerable<KeyValuePair<IMagazine, HullComponent>>> test1 = new();
            if (BulkMagazines)
                test1.AddRange(targettransform.GetComponentsInChildren<BulkMagazineComponent>().ConvertAll(comp => comp.Magazines.Select(mag => new KeyValuePair<IMagazine, HullComponent>(mag, comp))));
            if (CellLaunchers)
                test1.AddRange(targettransform.GetComponentsInChildren<CellLauncherComponent>().ConvertAll(comp => comp.Missiles.Select(mag => new KeyValuePair<IMagazine, HullComponent>(mag, comp))));
            IEnumerable<KeyValuePair<IMagazine, HullComponent>> test2 = test1.SelectMany(list => list).Where(pair => pair.Key.QuantityAvailable < pair.Key.PeakQuantity);
            if (AmmoFilter != null)
                test2 = test2.Where(pair => AmmoFilter.IsAmmoCompatible(pair.Key.AmmoType));
            IOrderedEnumerable<KeyValuePair<IMagazine, HullComponent>> test3 = test2.OrderBy(kvp => kvp.Key.PercentageAvailable);


            IEnumerable<IMunition> genericammo = Array.Empty<IMunition>();




            foreach (var kvp in test3)
            {
                IMagazine sink = kvp.Key;
                IMagazine? source = AmmoFeed.GetAmmoSource(sink.AmmoType);
                HullComponent hullComponent = kvp.Value;

                if (source == null)
                {
                    source = GetAmmoSource(GenericAmmoTypes);
                }


                if (sink.AmmoType.PointCost > ReloadPoints || sink == null || source == null)
                    return;

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
