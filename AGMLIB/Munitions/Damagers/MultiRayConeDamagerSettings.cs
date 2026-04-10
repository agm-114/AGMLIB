using Munitions.InstancedDamagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Munitions.Damagers
{

    [CreateAssetMenu(fileName = "New MultiRayConeDamagerSettings", menuName = "Nebulous/Damagers/Multi Ray Cone Damager Settings")]
    public class MultiRayConeDamagerSettings : SingleRayDamagerSettings
    {
        [Header("Multiple Ray Values")]
        public int RayAngle = 1;
        public int RayCount = 1;
        public override IDamageDealer MakeDamageDealer(BeamWarheadDescriptor warhead, float range = 0)
        {
            IDamageCharacteristic characteristic = new RangeBasedDamageCharacteristic(this, warhead, range);

            return new MultiRayConeDamager(characteristic, warhead.ComponentDamage / (float)RayCount, RayAngle, null, IgnoreDamageReduction);
        }
    }
}
