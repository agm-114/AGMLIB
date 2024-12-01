using static Ships.BulkMagazineComponent;
using static UnityEngine.UI.CanvasScaler;

namespace AGMLIB.Dynamic_Systems.Area
{
    public class ResupplyEffect : FalloffEffect<Ship>
    {
        public float PointsPerSecond = 1;

        private float ReloadPoints = 0;

        public bool BulkMagazines = true;
        public bool CellLaunchers = true;

        [SerializeField] protected ISimpleFilter _simplefilter;


        public override void TargetFixedUpdate(Ship target)
        {
            ReloadPoints += PointsPerSecond * Time.fixedDeltaTime;
            if (target == AreaEffect.Ship)
                return;

            if (AreaEffect.ShipController?.GetIFF(target?.Controller?.OwnedBy) == IFF.Enemy)
                return;

            Transform targettransform = target.gameObject.transform.root;
            //Debug.LogError("Resupply Ship");

            bool dirtymissiles = false;
            
            List<IEnumerable<KeyValuePair<IMagazine, HullComponent>>> test1 = new();
            if(BulkMagazines)
                test1.AddRange(targettransform.GetComponentsInChildren<BulkMagazineComponent>().ConvertAll(comp => comp.Magazines.Select(mag => new KeyValuePair<IMagazine, HullComponent>(mag, comp))));
            if(CellLaunchers)
                test1.AddRange(targettransform.GetComponentsInChildren<CellLauncherComponent>().ConvertAll(comp => comp.Missiles .Select(mag => new KeyValuePair<IMagazine, HullComponent>(mag, comp))));
            IEnumerable<KeyValuePair<IMagazine, HullComponent>>  test2 = test1.SelectMany(list => list).Where(pair => pair.Key.QuantityAvailable < pair.Key.PeakQuantity);
            if (_simplefilter != null)
                test2 = test2.Where(pair => _simplefilter.IsAmmoCompatible(pair.Key.AmmoType));
            IOrderedEnumerable<KeyValuePair<IMagazine, HullComponent>>  test3 = test2.OrderBy(kvp => kvp.Key.PercentageAvailable);




            foreach (var kvp in test3)
            {
                IMagazine sink = kvp.Key;
                IMagazine source = AreaEffect.Hull.MyShip.AmmoFeed.GetAmmoSource(sink.AmmoType);
                HullComponent hullComponent = kvp.Value;

                if (sink.AmmoType.PointCost > ReloadPoints)
                    return;

                uint reloadamount = (uint)Math.Min((uint)Math.Min(source.QuantityAvailable, sink.PeakQuantity - sink.QuantityAvailable), ReloadPoints / (float)sink.AmmoType.PointCost);
                ReloadPoints -= reloadamount * sink.AmmoType.PointCost;
                //BulkMagazineData magdata = hullComponent.gameObject.GetComponent<StartState>().saveData as BulkMagazineData;
                //List<IMagazine> mags = hullComponent.Magazines.ToList();


                //Debug.LogError($"{AreaEffect.Ship.gameObject.name} Resupply Ship {target.gameObject.name} {sink.AmmoType.MunitionName} {sink.QuantityAvailable}/{sink.PeakQuantity}");
                if(hullComponent is BulkMagazineComponent magcomp)
                    magcomp.AddToMagazine(sink.AmmoType, reloadamount);
                else if( hullComponent is CellLauncherComponent cellcomp)
                {
                    IMagazineProvider magazineProvider = cellcomp as IMagazineProvider;
                    //Debug.LogError("movin missiles");
                    dirtymissiles = true;

                    magazineProvider.AddToMagazine(sink.AmmoType, reloadamount);
                }


                source.Withdraw(reloadamount);

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
