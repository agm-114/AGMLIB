using AGMLIB.Dynamic_Systems.Area;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Dynamic_Systems.Area
{
    public class AmmoConsumingFalloffEffect : FalloffEffect<Ship>
    {
        public AmmoFeeder AmmoFeed => AreaEffect.Hull.MyShip.AmmoFeed;
        protected Dictionary<string, float> _residuals = new();

        public uint DiscreteReload(float truereloadamount, IMagazine source)
        {
            if (_residuals.TryGetValue(source.AmmoType.SaveKey, out float residual))
            {
                truereloadamount += residual;
            }
            _residuals[source.AmmoType.SaveKey] = 0;


            uint reloadamount = (uint)truereloadamount;
            float fractionalpart = truereloadamount - (uint)truereloadamount;
            _residuals[source.AmmoType.SaveKey] = fractionalpart;
            source.Withdraw(reloadamount);
            return reloadamount;
        }

        public IMagazine? GetAmmoSource(List<MunitionTags> AllowedAmmoTags)

        {
            IEnumerable<IMunition> possibleammos = AmmoFeed.AllAmmoTypes;

            if (AllowedAmmoTags.Count <= 0)
            {
                return null;
            }
            possibleammos = possibleammos.Where(ammo => AllowedAmmoTags.Contains(ammo.Tags));
            possibleammos = possibleammos.Where(ammo => AmmoFeed.GetAmmoSource(ammo).QuantityAvailable > 1);
            if (possibleammos.Any())
            {
                return AmmoFeed.GetAmmoSource(possibleammos.First());
                
            }
            return null;

        }
    }
}
