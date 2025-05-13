public class MissileSpawner : MonoBehaviour
{
    public GameObject missile;
    private ShipController _myShip;

    [HideInInspector]
    public ShipPDTarget target;

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
            target = missileObj.GetComponent<ShipPDTarget>();
            target.ShipController = _myShip;
            //imissile.Imbue(_myShip.NetID);
        }
        //else
        //    Debug.LogError("Missile failed to spawn");
        //Debug.LogError("Grabbing Ridgidbody");
        missileObj.GetComponent<Rigidbody>().isKinematic = false;
        //if (body == null)
        //    Debug.LogError("No rigidbody on missile, cannot launch");

        //Debug.LogError("Launching Missile");
        //callback?.Invoke(basicMissile);
        basicMissile?.Launch(_myShip, 0, false, false, true);
    }
}