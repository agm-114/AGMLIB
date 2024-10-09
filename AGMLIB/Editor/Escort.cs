public class Escort : MonoBehaviour
{

    public bool disableremoval = false;
    public bool disabepalette = false;
    public List<string> whitelist = new();
    public bool whitelisteverything = false;
    public List<string> blacklist = new();
    public bool blacklisteverything = false;

    /*
    public void Setup()
    {
        if (_defaultComponent.Length > 0)
            whitelist.Add(_defaultComponent);
    }
    */

    // Start is called before the first frame update
    [HideInInspector]
    public Vector3Int Size;
    [HideInInspector]
    public bool Resized = false;
}
//CoroutineSpawnFleets

//[HarmonyPatch(typeof(SerializedFleet), nameof(SerializedFleet.EnforceShipLimit))]
class SerializedFleetEnforceShipLimit
{
    static bool Prefix(SerializedFleet __instance)
    {

        if (__instance.Ships.Count < 10)
            return true;
        if (Ship.CheckUniformFaction(__instance.Ships, __instance.FactionKey))
        {
            return false;
        }

        Debug.LogWarning("Fleet " + __instance.Name + " has hulls from multiple factions.  Removing all ships that are not from the " + __instance.FactionKey + " faction.");
        FactionDescription faction = BundleManager.Instance.GetFaction(__instance.FactionKey);
        if (faction != null)
        {
            __instance.Ships.RemoveAll(delegate (SerializedShip ship)
            {
                BaseHull hull = BundleManager.Instance.GetHull(ship.HullType);
                return hull == null || !hull.UseableByFaction(faction);
            });
        }
        return false;
    }
}

//[HarmonyPatch(typeof(FleetCompositionSubmodeController), nameof(FleetCompositionSubmodeController.CopyShip))]

class FleetEditorFleetCompositionSubmodeController
{
    static bool Prefix(FleetCompositionSubmodeController __instance, Ship ship, FleetEditorController ____controller) => false;
    /*
        FleetEditorController _controller = ____controller;
        if (_controller.Fleet.FleetSize < 10)
            return true;
        Debug.LogError("Adding ship");
        SerializedShip serializable = ship.GetSerializable(includeState: false, includeMissileTemplates: false);
        serializable.Key = System.Guid.Empty;
        serializable.Name = ShipNameGenerator.Instance.GenerateName();
        serializable.Callsign = null;
        serializable.InitialFormation = null;
        serializable.Number = ShipNameGenerator.Instance.GenerateHullNumber();
        serializable.HullConfig = null;
        Ship activeShip = _controller.CreateNewShip(serializable.HullType, serializable);
        __instance.SelectShip(activeShip);
        return false;
        //__instance.SetActiveShip(activeShip);
        */
}

//[HarmonyPatch(typeof(FleetListPane), "HandleShipAdded")]
/*
class FleetListPaneHandleShipAdded : MonoBehaviour
{
    static bool Prefix(FleetListPane __instance, Ship ship, Fleet fleet, GameObject ____shipItemPrefab, RectTransform ____scrollPaneContent, FleetCompositionSubmodeController ____controller)
    {

        GameObject _shipItemPrefab = ____shipItemPrefab;
        RectTransform _scrollPaneContent = ____scrollPaneContent;
        FleetCompositionSubmodeController _controller = ____controller;

        List<ShipListItem> _items = new();
        GameObject itemObj = UnityEngine.Object.Instantiate(_shipItemPrefab, _scrollPaneContent);
        ShipListItem item = itemObj.GetComponent<ShipListItem>();
        item.SetShip(ship);
        item.DeleteButton.onClick.AddListener(delegate
        {
            _controller.DeleteShip(ship);
        });
        item.CopyButton.onClick.AddListener(delegate
        {
            _controller.CopyShip(ship);
        });
        Destroy(item.CopyButton.gameObject);
        item.BlueprintButton.onClick.AddListener(delegate
        {
            _controller.BlueprintShip(ship);
        });
        item.SelectButton.onClick.AddListener(delegate
        {
            _controller.SelectShip(ship);
        });
        _items.Add(item);
        return false;
    }
}
*/
/*
[HarmonyPatch(typeof(SkirmishGameManager), "CoroutineSpawnPlayerFleet")]
class SkirmishGameManagerCoroutineSpawnPlayerFleet
{


    static void Postfix(SkirmishGameManager __instance, SkirmishPlayer player, SerializedFleet fleetData, List<ShipController> ____allShips )
    {
        List<ShipController> _allShips =  ____allShips;
        //Debug.LogError("Ship Editor Pane Postfix");
        foreach (SerializedShip shipData in fleetData.Ships)
        {
            Vector3 shipStartPos = Vector3.zero;
            Quaternion shipStartRot = Quaternion.identity;
            Debug.Log("Spawning ship " + shipData.Name);
            GameObject shipObj = UnityEngine.Object.Instantiate(_shipRootPrefab, shipStartPos, shipStartRot);
            List<NetworkIdentity> shipIdentities = new List<NetworkIdentity>();
            shipIdentities.Add(shipObj.GetComponent<NetworkIdentity>());
            NetworkServer.Spawn(shipObj);
            Ship ship = shipObj.GetComponent<Ship>();
            if (shipData.SaveID.HasValue)
            {
                ship.GetComponent<SaveFileObject>().AssignID(shipData.SaveID);
            }
            ship.SetKey(shipData.Key);

            Ship ship = player.PlayerFleet.GetShip(shipData.Key);
            if (ship == null)
                continue;
            BaseHull hullPrefab = BundleManager.Instance.GetHull(shipData.HullType);
            if (hullPrefab == null)
            {
                Debug.LogError("No hull found with name " + shipData.HullType);
                continue;
            }
            ShipController controller = ship.gameObject.GetComponent<ShipController>();
            controller.SetInitialOwner(player);
            ship.SetHull(hullPrefab.gameObject, shipData.HullConfig);
            ship.LoadFromSave(shipData);
            ship.Hull.PrepareComponentsForNetworkTraffic();
            ship.EditorRecalcCrewAndResources();
            controller.Initialize((I)__instance.);
            if (shipData.SavedState != null)
            {
                yield return null;
                controller.LoadSavedState(shipData);
            }
            _allShips.Add(controller);
            yield return new WaitForSecondsRealtime(0.1f);
        }
        return;
    }



}
*/