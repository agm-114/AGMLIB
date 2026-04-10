using Munitions.InstancedDamagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Munitions.Damagers
{

    [CreateAssetMenu(fileName = "New SingleSpherecastDamagerSettings", menuName = "Nebulous/Damagers/Single Spherecast Damager Settings")]
    public class SingleSpherecastDamagerSettings : SingleRayDamagerSettings
    {
        [Header("Sphere Values")]
        public float SpherecastRadius = 1;

        public override IDamageDealer MakeDamageDealer(BeamWarheadDescriptor warhead, float range = 0)
        {
            IDamageCharacteristic characteristic = new RangeBasedDamageCharacteristic(this, warhead, range);

            return new SingleSpherecastDamager(characteristic, SpherecastRadius, SpreadingMethod, AlwaysSpreadDamage, null, IgnoreDamageReduction);
        }
    }
}
