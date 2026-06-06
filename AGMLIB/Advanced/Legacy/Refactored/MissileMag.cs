using UnityEngine.Profiling;

public class MagazineCellMissileMagazine : BaseCellMissileMagazine
{
    private List<Magazine> _missiles = new();

    private IReadOnlyList<MissileEjector> _cells;

    public override int Capacity => _launcher.Capacity;

    public override int UsedCapacity => _missiles.Sum((Magazine x) => x.Quantity);

    public override float RemainingCapacity => Capacity - _missiles.Sum((Magazine x) => x.Quantity);

    public override bool NoLoad => _missiles.Count == 0;

    protected override IEnumerable<Magazine> _missileMags => _missiles;

    List<BulkMagazineComponent> BulkMagazineComponents => _launcher.transform.parent.parent.GetComponentsInChildren<BulkMagazineComponent>().ToList();

    public void Slurp()
    {
        foreach (BulkMagazineComponent fullmag in BulkMagazineComponents.Where(a => a?.Magazines != null))
        {
            foreach (IMagazine mag in fullmag.Magazines)
            {
                if (RestrictionCheck(mag.AmmoType))
                {
                    //_missiles.AddToMagazineInternal(submag);
                    //_missiles.AddToMagazine(submag.AmmoType, (uint)submag.Quantity);
                    AddToMagazine(mag.AmmoType, (uint)mag.Quantity);
                    fullmag.RemoveFromMagazine(mag.AmmoType, 1);
                    //Debug.LogError("Slurp " + mag.AmmoType.MunitionName);
                    //mag.RemoveAllFromMagazine(submag.AmmoType);

                }
            }
        }
        UpdateEjectorContents();
    }

    public override int QuantityRemaining(IMunition missileType, bool excludeReserved)
    {
        for (int i = 0; i < _missiles.Count; i++)
        {
            if (_missiles[i].AmmoType == missileType)
            {
                return excludeReserved ? _missiles[i].QuantityNotReserved : _missiles[i].Quantity;
            }
        }

        return 0;
    }

    public override bool ChangeAmmoType(IMunition ammo)
    {
        foreach (Magazine missileMag in _missileMags)
        {
            if (missileMag.AmmoType == ammo)
            {
                _selectedAmmo = missileMag;
                return true;
            }
        }

        _selectedAmmo = null;
        return false;
    }

    public override PreppedCell? PrepFireCell(IMagazine fromMag)
    {
        int num = 0;
        foreach (Magazine missileMag in _missileMags)
        {
            if (missileMag == fromMag)
            {
                num += missileMag.Expended;
                break;
            }

            num += missileMag.PeakQuantity;
        }

        if (fromMag.Withdraw(1u) == 0)
        {
            return null;
        }

        if (num >= _cells.Count)
        {
            Debug.LogWarning("Overran bounds of available cells");
            return null;
        }

        PreppedCell value = default;
        value.Ejector = _cells[num];
        value.EjectorIndex = num;
        value.Missile = fromMag.AmmoType as IMissile;
        return value;
    }

    public override void LoadSaveData(List<Magazine.MagSaveData> mags)
    {
        foreach (Magazine.MagSaveData mag in mags)
        {
            IMunition munition = _launcher.Socket.AvailableMunitions.GetMunition(mag.MunitionKey);
            if (munition != null && munition.Type == MunitionType.Missile)
            {
                AddToMagazineInternal(munition, mag.Quantity, mag.MagazineKey);
            }
        }
        UpdateEjectorContents();
    }

    public override void RestoreSavedState(List<Magazine.MagSaveData> mags, List<Magazine.MagStateData> states)
    {
        ClearContents();
        foreach (Magazine.MagStateData magState in states)
        {
            Magazine.MagSaveData magSave = mags.Find((Magazine.MagSaveData x) => x.MagazineKey == magState.MagazineKey);
            if (magSave.Quantity != 0)
            {
                IMunition missile = _launcher.Socket.AvailableMunitions.GetMunition(magSave.MunitionKey);
                if (missile != null && missile.Type == MunitionType.Missile)
                {
                    Magazine mag = AddToMagazineInternal(missile, magSave.Quantity, magSave.MagazineKey);
                    RemoveFromMagazineInternal(mag, magState.Expended, removeIfEmpty: false, updatePeak: false);
                }
            }
        }
        UpdateEjectorContents();
    }



    protected override Magazine CreateMissingMagazine(Magazine.MagChange delta)
    {

        IMunition addAmmoType = _launcher.Socket.AvailableMunitions.GetMunition(delta.AmmoTypeKey);
        if (addAmmoType != null)
        {
            return CreateMagazineInternal(addAmmoType, delta.MagKey);
        }
        return null;
    }

    private Magazine CreateMagazineInternal(IMunition ammoType, string magKey)
    {
        return AddToMagazineInternal(ammoType, 0, magKey);
    }

    protected override Magazine AddToMagazineInternal(IMunition ammoType, uint quantity, string magKey = null)
    {
        if (!RestrictionCheck(ammoType))
        {
            //Debug.LogError("BadAmmo");
            return null;
        }
        //Debug.LogError("Adding Ammo " + ammoType.MunitionName + " " + quantity);
        Magazine magazine = _missiles.Find((Magazine x) => x.AmmoType == ammoType);
        if (magazine == null)
        {
            magazine = new Magazine(this, ammoType, magKey);
            _missiles.Add(magazine);
            FireMagAddedEvent(magazine);
        }

        int a = Capacity - _missiles.Sum((Magazine x) => x.Quantity);
        magazine.AddQuantity((uint)Mathf.Min(a, (int)quantity));
        FireAmmoQuantityChangedEvent(magazine);
        return magazine;
    }

    protected override bool RemoveFromMagazineInternal(Magazine mag, uint quantity, bool removeIfEmpty, bool updatePeak)
    {
        bool flag = false;
        if (mag != null)
        {
            mag.RemoveQuantity(quantity);
            if (mag.Quantity == 0 && removeIfEmpty)
            {
                _missiles.Remove(mag);
                flag = true;
            }
        }

        FireAmmoQuantityChangedEvent(mag);
        if (flag)
        {
            FireMagRemovedEvent(mag);
        }

        return flag;
    }
    public override void ClearContents()
    {

        _missiles.Clear();
    }

    public override void TrimMagazineTop()
    {
        if (_missiles.Count > 0)
        {
            for (uint num = (uint)_missiles.Sum((Magazine x) => x.Quantity); num > Capacity; num = (uint)_missiles.Sum((Magazine x) => x.Quantity))
            {
                Magazine magazine = _missiles.LastOrDefault();
                if (magazine == null)
                {
                    break;
                }

                RemoveFromMagazineInternal(magazine, num - (uint)Capacity, removeIfEmpty: true, updatePeak: true);
                if (_missiles.Count == 0)
                {
                    break;
                }
            }
        }

        FireAmmoCapacityChangedEvent();
    }

    protected override void UpdateEjectorContents()
    {
        IEnumerator<MissileEjector> enumerator = _cells.GetEnumerator();
        foreach (Magazine missileMag in _missileMags)
        {
            for (int i = 0; i < missileMag.Quantity; i++)
            {
                if (!enumerator.MoveNext())
                {
                    break;
                }

                enumerator.Current.SetContents(missileMag.AmmoType as IMissile);
            }
        }

        while (enumerator.MoveNext())
        {
            enumerator.Current.SetContents(null);
        }
    }

    

    public MagazineCellMissileMagazine(BaseCellLauncherComponent launcher, IReadOnlyList<MissileEjector> cells) : base(launcher) => _cells = cells;
}

public class HybridCellMissileMagazine : StandardCellMissileMagazine
{
    List<BulkMagazineComponent> BulkMagazineComponents => _launcher.transform.parent.parent.GetComponentsInChildren<BulkMagazineComponent>().ToList();

    protected override IEnumerable<Magazine> _missileMags => base._missileMags;

    private List<Magazine> _missiles = new();
    public HybridCellMissileMagazine(BaseCellLauncherComponent launcher, IReadOnlyList<MissileEjector> cells) : base(launcher, cells)
    {



    }
    public void UpdateEjectors() => base.UpdateEjectorContents();
}