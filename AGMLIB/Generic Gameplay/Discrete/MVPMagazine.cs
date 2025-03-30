using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Generic_Gameplay.Discrete
{
    public class MVPMagazine : MonoBehaviour, IMagazineProvider
    {
        public int MagCapacity = 10;
        public string EditorUnitName = "Slots";
        public bool IsReinforced = false;

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
        /*
        public DiscreteMagazine()
        { 

            Debug.LogError("DM constructor");

        }
        */
        public void SetParent(DiscreteWeaponComponent parent)
        {

            _parent = parent;
        }

        protected List<Magazine> Mags { get; } = new List<Magazine>();
        public DiscreteWeaponComponent _parent;
        IMunition SelectedAmmo => _parent.SelectedAmmoType;
        public IMagazineProvider _parentprovider => _parent as IMagazineProvider;

        bool IMagazineProvider.IsReinforced => IsReinforced;


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
    }

}
