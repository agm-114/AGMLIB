// Need to patch InitializeStatValues
using Game.InventoryManagement;
using Ships.SaveGame;
using SmallCraft;
using static Ships.BulkMagazineComponent;
using Utility.Localization;

public class FixedDiscreteMagWeaponComponent : FixedDiscreteWeaponComponent, IMagazineProvider, ISimpleStorageContainer, IStorageContainer, IConfigurableMagazineLoadout
{

    [SerializeField]
    private float _traverseRate = 1;//=> Common.GetVal<float>(this, "_traverseRate");
    [SerializeField]
    private float _elevationRate = 1;//=> Common.GetVal<float>(this, "_elevationRate");

    [SerializeField]
    private int _availableVolume = 100;

    [SerializeField]
    private MunitionType[] _storageRestrictions;

    [SerializeField]
    private string _containerGroup = "Bulk Mags";

    private MagazineCollection _magCollection;

    private StorageChanged _onNewItemTypeAdded;

    IStorageContainerProvider IStorageContainer.Parent => base.Socket.Hull.Controller;

    string IStorageContainer.StorageName => base.Socket.ShortName + " - " + ShortUIName;

    ushort IStorageContainer.StorageID => base.RpcKey;

    public string ProviderKey => PartKey;

    public bool CanTransfer => (base.IsFunctional && !base.IsDestroyed) || base.Socket.Hull.Controller.ForceAllowTransfers;

    public int Capacity => _availableVolume * CalcTileMultiplier();

    public int UsedCapacity => Mathf.RoundToInt(_magCollection.UsedCapacity);

    public float RemainingCapacity => (float)Capacity - _magCollection.UsedCapacity;

    public bool NoLoad => _magCollection.NoLoad;

    public string UnitName => "$UNIT_CUBICMETERS";

    public bool VolumeBased => true;

    public bool CanFeedExternally => true;

    string IStorageContainer.ContainerGroupName => (!string.IsNullOrEmpty(_containerGroup)) ? _containerGroup : null;

    public IEnumerable<IMagazine> Magazines => _magCollection.Magazines;

    public IMunitionCollection AvailableMunitions => base.Socket.AvailableMunitions;

    IEnumerable<IStorable> IStorageContainer.StoredItemTypes => _magCollection.Magazines.Select((IMagazine x) => x.AmmoType);

    public event MagazineProviderChanged OnAmmoCapacityChanged;

    public event QuantityChanged OnAmmoQuantityChanged
    {
        add
        {
            _magCollection.OnAmmoQuantityChanged += value;
        }
        remove
        {
            _magCollection.OnAmmoQuantityChanged -= value;
        }
    }

    public event MagazineChanged OnMagazineAdded
    {
        add
        {
            _magCollection.OnMagazineAdded += value;
        }
        remove
        {
            _magCollection.OnMagazineAdded -= value;
        }
    }

    public event MagazineChanged OnMagazineRemoved
    {
        add
        {
            _magCollection.OnMagazineRemoved += value;
        }
        remove
        {
            _magCollection.OnMagazineRemoved -= value;
        }
    }

    event QuantityChanged IStorageContainer.OnStorageQuantityChanged
    {
        add
        {
            _magCollection.OnAmmoQuantityChanged += value;
        }
        remove
        {
            _magCollection.OnAmmoQuantityChanged -= value;
        }
    }

    event StorageChanged IStorageContainer.OnNewItemTypeAdded
    {
        add
        {
            _onNewItemTypeAdded = (StorageChanged)Delegate.Combine(_onNewItemTypeAdded, value);
        }
        remove
        {
            _onNewItemTypeAdded = (StorageChanged)Delegate.Remove(_onNewItemTypeAdded, value);
        }
    }

    TMagProvider IConfigurableMagazineLoadout.GetMagazineProvider<TMagProvider>()
    {
        return this as TMagProvider;
    }

    protected override void Awake()
    {
        base.Awake();
        _magCollection = new MagazineCollection(this);
        _magCollection.OnMagazineAdded += delegate (IMagazineProvider p, IMagazine mag)
        {
            _onNewItemTypeAdded?.Invoke(this, mag.AmmoType);
        };
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        this.OnAmmoCapacityChanged = null;
    }

    public bool RestrictionCheck(IMunition ammoType)
    {
        if (_storageRestrictions.Length != 0 && !_storageRestrictions.Contains(ammoType.Type))
        {
            return false;
        }
        return true;
    }

    public int CountAmmoType(IMunition ammoType, bool availableOnly = false)
    {
        IMagazine mag = _magCollection.Magazines.FirstOrDefaultNoAlloc((IMagazine x) => x.AmmoType == ammoType);
        if (mag != null)
        {
            return availableOnly ? mag.QuantityAvailable : mag.Quantity;
        }
        return 0;
    }

    public IMagazine AddToMagazine(IMunition ammoType, uint quantity)
    {
        return _magCollection.AddToMagazine(ammoType, quantity);
    }

    public bool RemoveFromMagazine(IMunition ammoType, uint quantity)
    {
        return _magCollection.RemoveFromMagazine(ammoType, quantity);
    }

    public bool RemoveAllFromMagazine(IMunition ammoType)
    {
        return _magCollection.RemoveAllFromMagazine(ammoType);
    }

    protected override void PartDestroyedInternal()
    {
        base.PartDestroyedInternal();
        foreach (Magazine mag in _magCollection.Magazines)
        {
            mag.ProviderCanProvideChanged();
        }
    }

    protected override void PartRestoredInternal()
    {
        base.PartRestoredInternal();
        foreach (Magazine mag in _magCollection.Magazines)
        {
            mag.ProviderCanProvideChanged();
        }
    }

    protected override void PartFunctionalChangedInternal(bool newFunctional)
    {
        base.PartFunctionalChangedInternal(newFunctional);
        foreach (Magazine mag in _magCollection.Magazines)
        {
            mag.ProviderCanProvideChanged();
        }
    }

    public bool ContainsAnyAmmo()
    {
        return _magCollection.ContainsAnyAmmo();
    }

    public void CollectDeltas(List<Magazine.MagChange> deltas)
    {
        _magCollection.CollectDeltas(deltas);
    }

    public void ApplyDeltas(List<Magazine.MagChange> deltas)
    {
        _magCollection.ApplyDeltas(deltas);
    }

    public override SettingsPanelTypes[] GetConfigPanels()
    {
        return new SettingsPanelTypes[1] { SettingsPanelTypes.MagazineLoadout };
    }

    public override void GetFormattedStats(List<(string, string)> rows, bool full, int groupSize = 1)
    {
        base.GetFormattedStats(rows, full, groupSize);
        rows.Add(("$SHIPSTAT_STORAGECAPACITY", $"{Capacity:n0}" + " m³"));
        if (_storageRestrictions != null && _storageRestrictions.Length != 0)
        {
            rows.Add(("$SHIPSTAT_STORAGERESTRICTION", string.Join(", ", _storageRestrictions)));
        }
        else
        {
            rows.Add(("$SHIPSTAT_STORAGEUNRESTRICTED", ""));
        }
    }

    public override string GetDCTooltipDetails()
    {
        string ammoDetails = "$UI_SKIRMISH_DC_PART_STOREDAMMO:";
        if (_magCollection.Magazines.All((IMagazine x) => x.Quantity == 0))
        {
            ammoDetails += "\n   $UI_SKIRMISH_DC_PART_EMPTY";
        }
        else
        {
            foreach (Magazine mag in _magCollection.Magazines)
            {
                if (mag.Quantity != 0)
                {
                    ammoDetails += $"\n   {mag.Quantity}x {mag.AmmoType.MunitionDisplayName}";
                }
            }
        }
        return ammoDetails;
    }

    public override void GetDesignWarnings(List<DesignWarning> warnings)
    {
        base.GetDesignWarnings(warnings);
        if (!_magCollection.CheckAmmoDesignsValid())
        {
            warnings.Add(ShipDesignWarning.Warning("$UI_FLTED_WARNINGS_INVALIDMISSILEAMMO_TITLE", "$UI_FLTED_WARNINGS_INVALIDMISSILEAMMO_BODY".Localize(("compName", base.ComponentName), ("socketName", base.Socket.Name)), null));
        }
        List<string> unusedTypes = null;
        foreach (Magazine mag in _magCollection.Magazines)
        {
            if (!base.Socket.Hull.AmmoTypeInUse(mag.AmmoType, externalFeed: true))
            {
                if (unusedTypes == null)
                {
                    unusedTypes = new List<string>();
                }
                unusedTypes.Add(mag.AmmoType.MunitionDisplayName);
            }
        }
        if (unusedTypes != null)
        {
            warnings.Add(ShipDesignWarning.Risk("$UI_FLTED_WARNINGS_UNUSEDAMMO_TITLE", "Magazine in " + base.Socket.Name + " contains ammunition not used by any weapons: " + string.Join(", ", unusedTypes), null));
        }
    }

    public override ComponentSaveData GetSaveData()
    {
        return new BulkMagazineData
        {
            ClipboardTotalVolume = UsedCapacity,
            Load = _magCollection.SaveMagazineContents()
        };
    }

    public override void LoadSaveData(ComponentSaveData data)
    {
        base.LoadSaveData(data);
        if (data is BulkMagazineData magData)
        {
            float? proportionalScaling = null;
            if (magData.ClipboardTotalVolume.HasValue && magData.ClipboardTotalVolume.Value > (float)Capacity)
            {
                proportionalScaling = (float)Capacity / magData.ClipboardTotalVolume.Value;
            }
            _magCollection.ClearContents();
            _magCollection.LoadMagazineContents(magData.Load, base.Socket.AvailableMunitions, proportionalScaling);
        }
    }

    public override void WriteSaveState(SavedHullComponentStates state)
    {
        base.WriteSaveState(state);
        state.Write(this, new SavedComponentMagazineState
        {
            Mags = _magCollection.SaveMagazineContents(),
            States = _magCollection.SaveMagazineState()
        });
    }

    public override void RestoreFromSaveState(SavedHullComponentStates state)
    {
        base.RestoreFromSaveState(state);
        SavedComponentMagazineState magState = state.Read<SavedComponentMagazineState>(this);
        if (magState != null)
        {
            _magCollection.RestoreSavedState(magState.Mags, magState.States);
        }
    }

    public override void CollectModDependencies(HashSet<ulong> mods)
    {
        base.CollectModDependencies(mods);
        foreach (Magazine mag in _magCollection.Magazines)
        {
            if (mag.AmmoType.SourceModId.HasValue)
            {
                mods.Add(mag.AmmoType.SourceModId.Value);
            }
        }
    }

    public override int GetPointCost(int existingCount, HullComponentCostCalculator saveData = null)
    {
        int baseCost = base.GetPointCost(existingCount, saveData);
        int ammoCost = 0;
        checked
        {
            if (saveData == null)
            {
                foreach (Magazine mag in _magCollection.Magazines)
                {
                    ammoCost += mag.AmmoType.GetPointCost(mag.Quantity);
                }
            }
            else if (saveData.ComponentData is BulkMagazineData magData)
            {
                SavedComponentMagazineState magState = saveData.SavedState?.Components?.Read<SavedComponentMagazineState>(saveData.SocketKey);
                foreach (Magazine.MagSaveData mag2 in magState?.Mags ?? magData.Load)
                {
                    IPointCostItem munition = saveData.GetScopedCostItem(mag2.MunitionKey) ?? BundleManager.Instance.GetMunition(mag2.MunitionKey);
                    if (munition == null)
                    {
                        continue;
                    }
                    uint actualQuantity = mag2.Quantity;
                    if (magState != null)
                    {
                        actualQuantity = unchecked(actualQuantity - magState.States.Find((Magazine.MagStateData x) => x.MagazineKey == mag2.MagazineKey).Expended);
                    }
                    ammoCost += munition.GetPointCost((int)actualQuantity);
                }
            }
            return baseCost + ammoCost;
        }
    }

    public override void GetPointCostBreakdown(int existingCount, HullComponentCostBreakdownCalculator breakdown)
    {
        base.GetPointCostBreakdown(existingCount, breakdown);
        foreach (Magazine mag in _magCollection.Magazines)
        {
            int cost = mag.AmmoType.GetPointCost(mag.Quantity);
            if (mag.AmmoType.Type == MunitionType.Missile)
            {
                breakdown.AddCost(mag.AmmoType.Role.IsDefensive() ? ComponentCostClass.Defenses : ComponentCostClass.Missiles, cost);
            }
            else
            {
                breakdown.AddCost(ComponentCostClass.Ammunition, cost);
            }
        }
    }

    StorageContainerReference IStorageContainer.GetStorageReference()
    {
        return new PlatformStorageContainerReference(base.Socket.Hull.Controller, this);
    }

    bool IStorageContainer.RestrictionCheck(IStorable item)
    {
        return item is IMunition munition && RestrictionCheck(munition);
    }

    int ISimpleStorageContainer.WithdrawItems(IStorable itemType, int quantity)
    {
        if (itemType is IMunition ammoType)
        {
            return _magCollection.Withdraw(ammoType, quantity);
        }
        return 0;
    }

    int ISimpleStorageContainer.DepositItems(IStorable itemType, int quantity)
    {
        if (itemType is IMunition ammoType)
        {
            return _magCollection.Deposit(ammoType, quantity);
        }
        return 0;
    }

    int IStorageContainer.GetItemQuantity(IStorable itemType)
    {
        if (itemType is IMunition ammoType)
        {
            return _magCollection.GetQuantity(ammoType);
        }
        return 0;
    }

    int IStorageContainer.GetAccessibleItemQuantity(IStorable itemType)
    {
        if (CanTransfer && itemType is IMunition ammoType)
        {
            return _magCollection.GetQuantity(ammoType);
        }
        return 0;
    }

    int IStorageContainer.GetItemUpperQuantityBound(IStorable itemType, bool includePlanned)
    {
        if (itemType is IMunition ammoType)
        {
            return _magCollection.GetUpperQuantityBound(ammoType, includePlanned);
        }
        return 0;
    }

    bool IStorageContainer.GetCapacityStats(out float percentage, out float accessible, out float current, out float max, out string unit)
    {
        accessible = (CanTransfer ? Capacity : 0);
        max = Capacity;
        current = UsedCapacity;
        unit = UnitName;
        percentage = current / max;
        return true;
    }

    bool IStorageContainer.IsItemTypeUsed(IStorable itemType)
    {
        if (!(itemType is IMunition ammoType))
        {
            return false;
        }
        foreach (IWeapon weapon in base.Socket.Hull.CollectComponents<IWeapon>())
        {
            if (weapon.NeedsExternalAmmoFeed && weapon.IsAmmoCompatible(ammoType))
            {
                return true;
            }
        }
        IEnumerable<string> craftKeys = (from x in base.Socket.Hull.CollectComponents<ICraftHangar>().SelectMany((ICraftHangar x) => x.StoredCrafts)
                                         select x.SaveKey).Distinct();
        foreach (string craftKey in craftKeys)
        {
            Spacecraft template = base.Socket.AvailableCraft.GetCraftPattern(craftKey);
            if (template != null && template.IsAmmoTypeCompatible(ammoType))
            {
                return true;
            }
        }
        return false;
    }

    public virtual void ResetPeakQuantities()
    {
        _magCollection.ResetPeakQuantities();
    }

}

