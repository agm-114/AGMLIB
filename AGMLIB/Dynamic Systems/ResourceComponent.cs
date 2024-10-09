public class ResourceComponent : ActiveSettings
{
    // Start is called before the first frame update

    public enum FillBehavior
    {
        Clamp,
        NoCap,
    }

    //public string resourcename = "";
    //public HullComponent component;
    //public ResourceComponent parent;

    public FillBehavior overflow = FillBehavior.Clamp;
    public FillBehavior underflow = FillBehavior.Clamp;
    [Range(-1, 1)]
    public float capacity = 10;
    public float floor = 0;
    public float currentamount = 0;
    public GameObject ResourceIcon;
    [HideInInspector]
    public float fillpercentage;

    //private ShipController ShipController;
    //private Ship Ship;

    void Start()
    {
        //Destroy(this);
        //ShipController = transform.gameObject.GetComponentInParent<ShipController>();
        //if(parent == null)
        //    parent = ShipController.gameObject.GetComponent<ResourceComponent>();
        //Ship = ShipController.Ship;
        if(ResourceIcon != null)
        {
            ResourceBar newicon = Instantiate(ResourceIcon).GetComponentInChildren<ResourceBar>();
            newicon.ResourceComponent = this;
            newicon.ShipController = ShipController;
        }
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        //d
        //Debug.LogError("RAIDERLIB");
        //Debug.Log("Current ammount: " + currentamount);

        if (currentamount > capacity && overflow == FillBehavior.Clamp)
        {
            //Debug.Log("Venting Resource");
            currentamount = capacity;
        }

        if (currentamount < floor && underflow == FillBehavior.Clamp)
        {

            //Debug.Log("Venting Resource");
            currentamount = floor;
        }
        fillpercentage = currentamount / capacity; ;

}
}
