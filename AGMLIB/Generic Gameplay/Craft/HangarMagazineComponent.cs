using System.Reflection;
using System.Xml.Serialization;
using Game.InventoryManagement;
using Ships.SaveGame;
using SmallCraft;

[RequireComponent(typeof(MagazineComponent))]
public class HangarMagazineComponent : ConfigurableBulkCraftHangarComponent, IMagazineProvider, IConfigurableMagazineLoadout
{
    [Serializable]
    public class HangarMagazineData : CraftHangarData, IMagazineClipboardData
    {
        public List<Magazine.MagSaveData> Load = [];
        [XmlIgnore] public float? ClipboardTotalVolume;
        IReadOnlyList<Magazine.MagSaveData> IMagazineLoadComponentData.DefaultLoad => Load;
        float? IMagazineClipboardData.ClipboardTotalVolume => ClipboardTotalVolume;
        public HangarMagazineData() => StoredCraft = [];
    }

    [SerializeField] private MagazineComponent? _magazine;
    public MagazineComponent Magazine => _magazine ??= GetComponent<MagazineComponent>() ?? gameObject.AddComponent<MagazineComponent>();

    [Header("Shared Storage Costs")]
    [Range(0, 100), SerializeField]
    [Tooltip("Percentage of the original magazine volume consumed by each occupied craft storage unit.")]
    private float _magazineReductionPercentPerSlot = 10;
    [Min(0), SerializeField]
    [Tooltip("Fixed volume in cubic metres consumed by each occupied craft storage unit, added to the percentage cost.")]
    private float _magazineVolumePerSlot = 0;

    private SettingsHangarLoadout? _hangarPanel;
    private int FullMagazineCapacity => Magazine.AvailableVolume;
    private int MaximumStorageUnits => Math.Max(0, _spaceUnitsPerTile) * CalcTileMultiplier();
    public override int TotalStorageUnits => CalculateHangarCapacity(FullMagazineCapacity, Magazine.Collection?.UsedCapacity ?? 0,
        MaximumStorageUnits, _magazineReductionPercentPerSlot, _magazineVolumePerSlot);
    public override int AvailableStorageUnits => Math.Max(0, TotalStorageUnits - OccupiedStorageUnits);
    public override int AvailableUnreservedStorageUnits => Math.Max(0, TotalStorageUnits - OccupiedStorageUnitsWithReserved);
    public int Capacity => CalculateMagazineCapacity(FullMagazineCapacity, OccupiedStorageUnitsWithReserved,
        _magazineReductionPercentPerSlot, _magazineVolumePerSlot);
    public int UsedCapacity => Magazine.UsedCapacity;
    public float RemainingCapacity => Magazine.RemainingCapacity;
    public bool NoLoad => Magazine.Collection.NoLoad;
    public string ProviderKey => PartKey;
    public string UnitName => "$UNIT_CUBICMETERS";
    public bool CanFeedExternally => true;
    public IEnumerable<IMagazine> Magazines => Magazine.Collection.Magazines;
    public IMunitionCollection AvailableMunitions => Socket.AvailableMunitions;
    bool IStorageContainer.VolumeBased => true;
    int IStorageContainer.Capacity => Capacity;
    IEnumerable<IStorable> IStorageContainer.StoredItemTypes =>
        Magazines.Select(magazine => (IStorable)magazine.AmmoType).Concat(StoredCrafts.Distinct(Spacecraft.SameSourceTemplateComparer));

    public event MagazineProviderChanged? OnAmmoCapacityChanged;
    public event QuantityChanged OnAmmoQuantityChanged
    {
        add => Magazine.Collection.OnAmmoQuantityChanged += value;
        remove => Magazine.Collection.OnAmmoQuantityChanged -= value;
    }
    public event MagazineChanged OnMagazineAdded
    {
        add => Magazine.Collection.OnMagazineAdded += value;
        remove => Magazine.Collection.OnMagazineAdded -= value;
    }
    public event MagazineChanged OnMagazineRemoved
    {
        add => Magazine.Collection.OnMagazineRemoved += value;
        remove => Magazine.Collection.OnMagazineRemoved -= value;
    }

    protected override void Awake()
    {
        base.Awake();
        Magazine.Initialize(this);
        Magazine.Collection.OnAmmoQuantityChanged += HandleAmmoChanged;
        Magazine.Collection.OnMagazineAdded += HandleMagazineAdded;
        OnStorageChanged += HandleCraftStorageChanged;
    }

    protected override void OnDestroy()
    {
        Magazine.Collection.OnAmmoQuantityChanged -= HandleAmmoChanged;
        Magazine.Collection.OnMagazineAdded -= HandleMagazineAdded;
        OnAmmoCapacityChanged = null;
        _hangarPanel = null;
        base.OnDestroy();
    }

    public bool RestrictionCheck(IMunition ammo) => Magazine.RestrictionCheck(ammo);
    public IMagazine AddToMagazine(IMunition ammo, uint quantity) => Magazine.Collection.AddToMagazine(ammo, quantity);
    public bool RemoveFromMagazine(IMunition ammo, uint quantity) => Magazine.Collection.RemoveFromMagazine(ammo, quantity);
    public bool RemoveAllFromMagazine(IMunition ammo) => Magazine.Collection.RemoveAllFromMagazine(ammo);
    public void CollectDeltas(List<Magazine.MagChange> deltas) => Magazine.Collection.CollectDeltas(deltas);
    public void ResetPeakQuantities() => Magazine.Collection.ResetPeakQuantities();
    public void ApplyDeltas(List<Magazine.MagChange> deltas)
    {
        Magazine.Collection.ApplyDeltas(deltas);
        // Native ApplyDeltas raises per-magazine events before updating the collection's volume.
        HandleAmmoChanged(this);
    }

    public override bool CanFitCraft(Spacecraft craft) => craft != null && craft.StorageUnits > 0 && craft.StorageUnits <= MaximumStorageUnits;
    public override void AddCraftToStorage(Spacecraft craft)
    {
        if (_reservedStorage.Contains(craft) || HasStorageSpaceForCraft(craft))
            base.AddCraftToStorage(craft);
    }
    public override void ReserveLandingSpace(Spacecraft craft)
    {
        base.ReserveLandingSpace(craft);
        HandleCraftStorageChanged(this);
    }
    public override void ReleaseReservedLandingSpace(Spacecraft craft)
    {
        base.ReleaseReservedLandingSpace(craft);
        HandleCraftStorageChanged(this);
    }

    private void HandleCraftStorageChanged(ICraftHangar hangar) => OnAmmoCapacityChanged?.Invoke(this);
    private void HandleMagazineAdded(IMagazineProvider provider, IMagazine magazine) => this.Internals().NewItemTypeAdded?.Invoke(this, magazine.AmmoType);
    private void HandleAmmoChanged(IQuantityHolder quantity)
    {
        this.Internals().QuantityChanged?.Invoke(quantity);
        this.Internals().StorageChanged?.Invoke(this);
        RefreshHangarPanel();
    }

    internal void BindHangarPanel(SettingsHangarLoadout panel)
    {
        _hangarPanel = panel;
        RefreshHangarPanel();
    }
    private void RefreshHangarPanel()
    {
        if (_hangarPanel == null || !ReferenceEquals(_hangarPanel.Internals().Hangar, this))
            return;
        _hangarPanel.Internals().RefreshQuantities();
        if (TotalStorageUnits == 0)
            _hangarPanel.Internals().CapacityGauge.fillAmount = 0;
    }

    internal static int CalculateMagazineCapacity(int fullCapacity, int occupiedSlots, float percentPerSlot, float volumePerSlot) =>
        (int)Math.Max(0, Math.Floor(fullCapacity - Math.Max(0, occupiedSlots) * SlotVolume(fullCapacity, percentPerSlot, volumePerSlot)));

    internal static int CalculateHangarCapacity(int fullCapacity, float usedVolume, int maximumSlots, float percentPerSlot, float volumePerSlot)
    {
        double costPerSlot = SlotVolume(fullCapacity, percentPerSlot, volumePerSlot);
        // Fractional ammunition volume needs the next whole cubic metre of integer magazine capacity.
        double available = Math.Max(0, fullCapacity - Math.Ceiling(usedVolume));
        return (int)Math.Max(0, costPerSlot > 0 ? Math.Min(maximumSlots, Math.Floor(available / costPerSlot)) : maximumSlots);
    }
    private static double SlotVolume(int fullCapacity, float percentPerSlot, float volumePerSlot) =>
        fullCapacity * Math.Min(100d, Math.Max(0d, percentPerSlot)) / 100 + Math.Max(0d, volumePerSlot);

    TProvider IConfigurableMagazineLoadout.GetMagazineProvider<TProvider>() => (this as TProvider)!;
    public override SettingsPanelTypes[] GetConfigPanels() => [SettingsPanelTypes.MagazineLoadout, SettingsPanelTypes.HangarLoadout];
    public override ComponentSaveData GetSaveData() => new HangarMagazineData
    {
        StoredCraft = ((CraftHangarData)base.GetSaveData()).StoredCraft,
        Load = Magazine.Collection.SaveMagazineContents(),
        ClipboardTotalVolume = UsedCapacity
    };
    public override void LoadSaveData(ComponentSaveData data)
    {
        ClearAllStorage();
        Magazine.Collection.ClearContents();
        base.LoadSaveData(data);
        Magazine.LoadSaveData(data);
    }
    public override void WriteSaveState(SavedHullComponentStates state)
    {
        base.WriteSaveState(state);
        Magazine.WriteSaveState(state);
    }
    public override void RestoreFromSaveState(SavedHullComponentStates state)
    {
        Magazine.Collection.ClearContents();
        base.RestoreFromSaveState(state);
        Magazine.RestoreFromSaveState(state);
    }

    public override int GetPointCost(int existingCount, HullComponentCostCalculator? saveData = null) =>
        base.GetPointCost(existingCount, saveData) + Magazine.GetPointCost(saveData);
    public override void GetPointCostBreakdown(int existingCount, HullComponentCostBreakdownCalculator breakdown)
    {
        base.GetPointCostBreakdown(existingCount, breakdown);
        Magazine.GetPointCostBreakdown(breakdown);
    }
    public override void CollectModDependencies(HashSet<ulong> mods)
    {
        base.CollectModDependencies(mods);
        Magazine.CollectModDependencies(mods);
        StoredCrafts.Select(craft => craft.SourceModId).Where(mod => mod.HasValue).ToList().ForEach(mod => mods.Add(mod!.Value));
    }
    public override void GetDesignWarnings(List<DesignWarning> warnings)
    {
        base.GetDesignWarnings(warnings);
        Magazine.GetDesignWarnings(warnings);
    }
    public override void GetFormattedStats(List<(string, string)> rows, bool full, int groupSize = 1)
    {
        base.GetFormattedStats(rows, full, groupSize);
        rows.Add(("$SHIPSTAT_STORAGECAPACITY", $"{Capacity:n0} m³"));
        rows.Add(("$SHIPSTAT_CRAFTCAPACITY", $"{TotalStorageUnits:n0}"));
        rows.Add(("Magazine volume per craft slot", $"-{_magazineReductionPercentPerSlot:0.##}% + {_magazineVolumePerSlot:n0} m³"));
    }
    public override string GetDCTooltipDetails() => base.GetDCTooltipDetails() + "\n" + Magazine.GetDCTooltipDetails();

    bool IStorageContainer.RestrictionCheck(IStorable item) => item is Spacecraft craft ? CanFitCraft(craft) : item is IMunition ammo && RestrictionCheck(ammo);
    int IStorageContainer.GetItemQuantity(IStorable item) => item is IMunition ammo ? Magazine.Collection.GetQuantity(ammo) : StoredCrafts.Count(craft => craft.SaveKey == item.SaveKey);
    int IStorageContainer.GetAccessibleItemQuantity(IStorable item) => CanTransfer ? ((IStorageContainer)this).GetItemQuantity(item) : 0;
    int IStorageContainer.GetItemUpperQuantityBound(IStorable item, bool includePlanned) => item is IMunition ammo
        ? Magazine.Collection.GetUpperQuantityBound(ammo, includePlanned) : item is Spacecraft craft ? UpperStorageBound(craft, includePlanned) : 0;
    bool IStorageContainer.IsItemTypeUsed(IStorable item) => item is Spacecraft || item is IMunition ammo && Socket.Hull.AmmoTypeInUse(ammo, externalFeed: true);
    int ISimpleStorageContainer.WithdrawItems(IStorable item, int quantity) => item is IMunition ammo ? Magazine.Collection.Withdraw(ammo, quantity) : 0;
    int ISimpleStorageContainer.DepositItems(IStorable item, int quantity) => item is IMunition ammo ? Magazine.Collection.Deposit(ammo, quantity) : 0;
    bool IStorageContainer.GetCapacityStats(out float percentage, out float accessible, out float used, out float max, out string unit)
    {
        max = Capacity;
        accessible = CanTransfer ? max : 0;
        used = Magazine.Collection.UsedCapacity;
        percentage = max > 0 ? used / max : 0;
        unit = UnitName;
        return true;
    }
}

[HarmonyPatch(typeof(SettingsHangarLoadout), nameof(SettingsHangarLoadout.Set))]
static class HangarMagazineEditorPatch
{
    static void Postfix(SettingsHangarLoadout __instance, HullComponent component)
    {
        if (component is HangarMagazineComponent hangar)
            hangar.BindHangarPanel(__instance);
    }
}

// The socket serializer must recognize the combined payload nested under ComponentSaveData.
[HarmonyPatch]
static class HangarMagazineSerializationPatch
{
    static ConstructorInfo TargetMethod() => AccessTools.Constructor(
        AccessTools.Inner(typeof(XmlDocSerializerExtensions), "TypeSerializationInfo"), [typeof(Type)]);
    static void Postfix(Type type, ref Type[] ____childTypes)
    {
        if (type == typeof(SerializedHullSocket))
            ____childTypes = [.. ____childTypes, typeof(HangarMagazineComponent.HangarMagazineData)];
    }
}
