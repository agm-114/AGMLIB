// Need to patch InitializeStatValues
public class FixedDiscreteMagWeaponComponent : FixedDiscreteWeaponComponent, IMagazineProvider, IConfigurableMagazineLoadout 
{



    [SerializeField]
    private float _traverseRate = 1;//=> Common.GetVal<float>(this, "_traverseRate");
    [SerializeField]
    private float _elevationRate = 1;//=> Common.GetVal<float>(this, "_elevationRate");
                                     //BaseCellLauncherComponent BaseCellLauncherComponent { get; set; }

    public int MagSize = 10;
    public string MagUnitName = "Slots";


    //_requireExternalAmmoFeed = false;
    private DiscreteMagazine Mag;


    protected override void SocketSet()
    {
        base.SocketSet();
        Mag = new(this);
    }
    public string ProviderKey => PartKey;
    public bool CanProvide => base.IsFunctional;

    public int MaxCapacity => MagSize;
    public int UsedCapacity => Mag.UsedCapacity;
    public bool NoLoad => false;
    public float RemainingCapacity => Mag.RemainingCapacity;
    public string UnitName => MagUnitName;
    public bool VolumeBased => false;
    public bool CanFeedExternally => true;
    public IEnumerable<IMagazine> Magazines => Mag.Magazines;
    public event QuantityChanged OnAmmoQuantityChanged
    {
        add => Mag.OnAmmoQuantityChanged += value;
        remove => Mag.OnAmmoQuantityChanged -= value;
    }
    public event MagazineProviderChanged OnAmmoCapacityChanged
    {
        add => Mag.OnAmmoCapacityChanged += value;
        remove => Mag.OnAmmoCapacityChanged -= value;
    }
    public event MagazineChanged OnMagazineAdded
    {
        add => Mag.OnMagazineAdded += value;
        remove => Mag.OnMagazineAdded -= value;
    }
    public event MagazineChanged OnMagazineRemoved
    {
        add => Mag.OnMagazineRemoved += value;
        remove => Mag.OnMagazineRemoved -= value;
    }
    public override SettingsPanelTypes[] GetConfigPanels() => Mag.GetConfigPanels();
    public bool RestrictionCheck(IMunition ammo) => Mag.RestrictionCheck(ammo);
    public IMagazine AddToMagazine(IMunition ammoType, uint quantity) => Mag.AddToMagazine(ammoType, quantity);
    public bool RemoveFromMagazine(IMunition ammoType, uint quantity) => Mag.RemoveFromMagazine(ammoType, quantity);
    public bool RemoveAllFromMagazine(IMunition ammoType) => Mag.RemoveAllFromMagazine(ammoType);
    public void CollectDeltas(List<Magazine.MagChange> deltas) => Mag.CollectDeltas(deltas);
    public void ApplyDeltas(List<Magazine.MagChange> deltas) => Mag.ApplyDeltas(deltas);
    TMagProvider IConfigurableMagazineLoadout.GetMagazineProvider<TMagProvider>() => Mag as TMagProvider;
    public override ComponentSaveData GetSaveData() => Mag.GetSaveData();
    public override void LoadSaveData(ComponentSaveData data) => Mag.LoadSaveData(data);
    //protected override PersistentComponentState NewSaveStateInstance() => new BulkMagazineState();
    protected override void FillSaveState(PersistentComponentState state)
    {
        base.FillSaveState(state);
        //Mag.FillSaveState(state);
    }
    public override void RestoreSavedState(PersistentComponentState state)
    {
        base.RestoreSavedState(state);
        //Mag.RestoreSavedState(state);
    }
    public String x = "The below are legacy fields, please talk to AGM";
    public List<MissileEjector> Cells = new();
    [SerializeField]
    protected float _baseAccuracy = 0.01f;

    [ShipStat("muzzle-accuracy", "Spread", "MoA", InitializeFrom = "_baseAccuracy", TextValueMultiplier = 3437.75f, PositiveBad = true)]
    protected StatValue _statAccuracy;
    public float TimeBetweenCells = 0.2f;

    public String y = "The above are legacy fields, please talk to AGM";
    protected override void Start()
    {
        if(Cells.Count > 0)
        {

            DiscreteWeaponEjectors ejectors = this.gameObject.AddComponent<DiscreteWeaponEjectors>();
            ejectors.Cells = Cells;
            ejectors.BaseAccuracy = _baseAccuracy;
            ejectors.TimeBetweenCells = TimeBetweenCells;

        }
        base.Start();
    }

}

