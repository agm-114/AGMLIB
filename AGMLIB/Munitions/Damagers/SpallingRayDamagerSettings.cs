using Munitions.InstancedDamagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Munitions.Damagers
{

    [CreateAssetMenu(fileName = "New SpallingRayDamagerSettings", menuName = "Nebulous/Damagers/Spalling Ray Damager Settings")]
    public class SpallingRayDamagerSettings : MultiRayConeDamagerSettings
    {
        public override IDamageDealer MakeDamageDealer(BeamWarheadDescriptor warhead, float range = 0)
        {
            IDamageCharacteristic characteristic = new RangeBasedDamageCharacteristic(this, warhead, range);

            return new SpallingRayDamager(characteristic, RayAngle, RayCount, SpreadingMethod, AlwaysSpreadDamage, null, IgnoreDamageReduction);
        }
    }
}
