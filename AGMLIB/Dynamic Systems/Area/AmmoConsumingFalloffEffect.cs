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
            //Common.Hint($"reloading {truereloadamount} of {source.AmmoType.MunitionName}");
            if (_residuals.TryGetValue(source.AmmoType.SaveKey, out float residual))
            {
                truereloadamount += residual;
                //Common.Hint($"including residual {residual} new total {truereloadamount}");
            }
            _residuals.Remove(source.AmmoType.SaveKey);
            


            uint reloadamount = (uint)truereloadamount;
            //Common.Hint($"discrete reload amount {reloadamount}");
            float fractionalpart = truereloadamount - (uint)truereloadamount;
            //Common.Hint($"fractional part {fractionalpart}");
            _residuals[source.AmmoType.SaveKey] = fractionalpart;
            source.Withdraw(reloadamount);
            return reloadamount;
        }

        public IMagazine? GetAmmoSource(BaseFilter SourceAmmoFilter)

        {
            IEnumerable<IMunition> possibleammos = AmmoFeed.AllAmmoTypes;

            if (SourceAmmoFilter == null)
            {
                
                return null;
            }
            //Common.Hint(this, $"total possible ammo {possibleammos.Count()}");
            possibleammos = possibleammos.Where(ammo => SourceAmmoFilter.IsAmmoCompatible(ammo));
            //Common.Hint(this, $"total compatible ammo {possibleammos.Count()}");
            possibleammos = possibleammos.Where(ammo => AmmoFeed.GetAmmoSource(ammo).QuantityAvailable > 1);
            //Common.Hint(this, $"total real ammo {possibleammos.Count()}");
            if (possibleammos.Any())
            {
                return AmmoFeed.GetAmmoSource(possibleammos.First());
                
            }
            return null;

        }
    }
}
