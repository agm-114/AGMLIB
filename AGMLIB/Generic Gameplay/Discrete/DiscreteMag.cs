
using Ships.SaveGame;
using static Ships.BulkMagazineComponent;
/*
public class InternalDiscreteMagazine : MonoBehaviour, IMagazineProvider
{


    public DiscreteWeaponComponent _parent;

    public string ProviderKey => throw new NotImplementedException();

    public bool CanProvide => throw new NotImplementedException();

    public int MaxCapacity => throw new NotImplementedException();

    public int UsedCapacity => throw new NotImplementedException();

    public bool NoLoad => throw new NotImplementedException();

    public float RemainingCapacity => throw new NotImplementedException();

    public string UnitName => throw new NotImplementedException();

    public bool VolumeBased => throw new NotImplementedException();

    public bool CanFeedExternally => throw new NotImplementedException();

    public IEnumerable<IMagazine> Magazines => throw new NotImplementedException();

    public bool IsReinforced => throw new NotImplementedException();

    public event QuantityChanged OnAmmoQuantityChanged;
    public event MagazineProviderChanged OnAmmoCapacityChanged;
    public event MagazineChanged OnMagazineAdded;
    public event MagazineChanged OnMagazineRemoved;

    public IMagazine AddToMagazine(IMunition ammoType, uint quantity) => throw new NotImplementedException();
    public void ApplyDeltas(List<Magazine.MagChange> deltas) => throw new NotImplementedException();
    public void CollectDeltas(List<Magazine.MagChange> deltas) => throw new NotImplementedException();
    public bool RemoveAllFromMagazine(IMunition ammoType) => throw new NotImplementedException();
    public bool RemoveFromMagazine(IMunition ammoType, uint quantity) => throw new NotImplementedException();
    public bool RestrictionCheck(IMunition ammo) => throw new NotImplementedException();

    public void SetParent(DiscreteWeaponComponent parent) => throw new NotImplementedException();
    public SettingsPanelTypes[] GetConfigPanels() => throw new NotImplementedException();
    public void LoadSaveData(ComponentSaveData data) => throw new NotImplementedException();
    public ComponentSaveData GetSaveData() => throw new NotImplementedException();
}
*/
public class DiscreteMagazine : MonoBehaviour
{
    public int MagCapacity = 10;
    public string EditorUnitName = "Slots";
    public bool IsReinforced = false;
}

