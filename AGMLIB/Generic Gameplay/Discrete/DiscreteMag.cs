using FleetEditor;
using Munitions;
using Ships.Controls;
using Ships.Serialization;
using Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ships.BulkMagazineComponent;
using UnityEngine;
using Utility;

public class DiscreteMagazine : IMagazineProvider
{
    public SettingsPanelTypes[] GetConfigPanels()
    {
        //Debug.LogError("Config Panel Check");
        if (MaxCapacity > 0)
            return new SettingsPanelTypes[1] { SettingsPanelTypes.MagazineLoadout };
        if (_parent.AllowGrouping && !_parent.Role.IsDefensiveOnly())
        {
            return new SettingsPanelTypes[1];
        }
        return null;
    }
    public DiscreteMagazine(DiscreteWeaponComponent parent) => _parent = parent;
    protected List<Magazine> Mags { get; } = new List<Magazine>();
    public DiscreteWeaponComponent _parent;
    IMunition SelectedAmmo => _parent.SelectedAmmoType;
    public IMagazineProvider _parentprovider => _parent as IMagazineProvider;
    public int UsedCapacity
    {
        get
        {
            if (VolumeBased)
                return (int)Mags.Sum((Magazine x) => x.StoredVolume);
            else
                return Mags.Sum((Magazine x) => x.Quantity);
        }
    }
    public float RemainingCapacity => MaxCapacity - UsedCapacity;
    public string ProviderKey => _parent.SaveKey;
    public bool CanProvide => _parent.IsFunctional;
    public int MaxCapacity => _parentprovider.MaxCapacity;
    public bool NoLoad => _parentprovider.NoLoad;
    public string UnitName => _parentprovider.UnitName;
    public bool VolumeBased => _parentprovider.VolumeBased;
    public bool CanFeedExternally => _parentprovider.CanFeedExternally;
    public IEnumerable<IMagazine> Magazines => Mags;
    public event QuantityChanged OnAmmoQuantityChanged;
    protected void FireAmmoQuantityChangedEvent(IQuantityHolder holder) => OnAmmoQuantityChanged?.Invoke(holder);
    public event MagazineProviderChanged OnAmmoCapacityChanged;
    public event MagazineChanged OnMagazineAdded;
    protected void FireMagAddedEvent(IMagazine mag) => OnMagazineAdded?.Invoke(this, mag);
    public event MagazineChanged OnMagazineRemoved;
    protected void FireMagRemovedEvent(IMagazine mag) => OnMagazineRemoved?.Invoke(this, mag);
    public int QuantityRemaining(IMunition ammoType) => FindMag(ammoType)?.QuantityAvailable ?? 0;
    protected Magazine FindMag(IMunition ammoType) => Mags.Find((Magazine x) => x.AmmoType == ammoType);
    protected Magazine FindMag(string magkey) => Mags.Find((Magazine x) => x.Key == magkey);
    public IMagazine AddToMagazine(IMunition ammoType, uint quantity) => AddToMagazineInternal(ammoType, quantity);
    public uint Capacity(IMunition ammoType)
    {
        if (VolumeBased)
            return (uint)(RemainingCapacity / ammoType.StorageVolume);
        else
            return (uint)RemainingCapacity;

    }
    public IMagazine AddToMagazineInternal(IMunition ammoType, uint quantity, string magKey = null)
    {
        if (RestrictionCheck(ammoType) == false)
            return null;
        Magazine mag = FindMag(ammoType);
        if (mag == null)
        {
            mag = new Magazine(this, ammoType, magKey);
            Mags.Add(mag);
            FireMagAddedEvent(mag);
        }
        mag.AddQuantity((uint)Mathf.Min(Capacity(ammoType), quantity));
        FireAmmoQuantityChangedEvent(mag);
        return mag;
    }
    public bool RemoveAllFromMagazine(IMunition ammoType) => RemoveFromMagazine(ammoType, uint.MaxValue);
    public bool RemoveFromMagazine(IMunition ammoType, uint quantity) => RemoveFromMagazineInternal(ammoType, quantity, removeIfEmpty: true);
    protected bool RemoveFromMagazineInternal(string magkey, uint quantity, bool removeIfEmpty = true) => RemoveFromMagazineInternal(FindMag(magkey), quantity, removeIfEmpty: true);
    protected bool RemoveFromMagazineInternal(IMunition ammoType, uint quantity, bool removeIfEmpty = true) => RemoveFromMagazineInternal(FindMag(ammoType), quantity, removeIfEmpty: true);
    protected bool RemoveFromMagazineInternal(Magazine mag, uint quantity, bool removeIfEmpty = true)
    {
        if (mag != null)
        {
            mag.RemoveQuantity(quantity);
            if (mag.Quantity == 0 && removeIfEmpty)
            {
                Mags.Remove(mag);
                FireAmmoQuantityChangedEvent(mag);
                FireMagRemovedEvent(mag);
                return true;
            }
            FireAmmoQuantityChangedEvent(mag);
        }
        return false;
    }
    public bool RestrictionCheck(IMunition ammo) => ammo != null && _parent.IsAmmoCompatible(ammo);
    IMunition GetMunition(Magazine.MagSaveData magdata) => _parent.Socket.MyHull.MyShip.Fleet.AvailableMunitions.GetMunition(magdata.MunitionKey);
    public ComponentSaveData GetSaveData()
    {
        BulkMagazineData data = new();
        foreach (Magazine mag in Mags)
        {
            data.Load.Add(new Magazine.MagSaveData
            {
                MagazineKey = mag.Key,
                MunitionKey = mag.AmmoType.SaveKey,
                Quantity = (uint)mag.PeakQuantity
            });
        }
        return data;
    }
    public void LoadSaveData(ComponentSaveData data)
    {
        foreach (Magazine.MagSaveData magazine in ((BulkMagazineData)data)?.Load)
            AddToMagazineInternal(GetMunition(magazine), magazine.Quantity, magazine.MagazineKey);
    }
    public void FillSaveState(PersistentComponentState state)
    {
        foreach (Magazine mag in Mags)
        {
            ((BulkMagazineState)state)?.Mags?.Add(new Magazine.MagStateData
            {
                MagazineKey = mag.Key,
                Expended = (uint)(mag.PeakQuantity - mag.Quantity)
            });
        }
    }
    public void RestoreSavedState(PersistentComponentState state)
    {
        foreach (Magazine.MagStateData magazine in ((BulkMagazineState)state)?.Mags)
            RemoveFromMagazineInternal(magazine.MagazineKey, magazine.Expended, removeIfEmpty: false);
    }
    public List<Magazine.MagStateData> GetSaveState()
    {
        List<Magazine.MagStateData> mags = new List<Magazine.MagStateData>();
        foreach (Magazine mag in Mags)
        {
            mags.Add(new Magazine.MagStateData
            {
                MagazineKey = mag.Key,
                Expended = (uint)(mag.PeakQuantity - mag.Quantity)
            });
        }
        return mags;
    }
    public void ApplyDeltas(List<Magazine.MagChange> deltas)
    {
        foreach (Magazine.MagChange change in deltas.Where(change => change.ProviderKey == ProviderKey))
            FindMag(change.MagKey)?.ApplyDelta(change);
    }
    public void CollectDeltas(List<Magazine.MagChange> deltas)
    {
        foreach (Magazine mag in Mags)
            mag.CollectDeltas(deltas);
    }
    public bool CanFireMissile(bool playerOrder)
    {

        if (_parent.IsDoingWork && !_parent.IsAmmoCompatible(SelectedAmmo) && QuantityRemaining(SelectedAmmo) > 0)  //there is not missile
        {
            return true;
        }
        return true; //if launch area is clear
    }
}

