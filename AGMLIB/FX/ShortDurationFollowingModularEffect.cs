using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.FX
{
    public class ShortDurationFollowingModularEffect : FollowingModularEffect
    {
        [SerializeField]
        private float _repoolDelay = 1f;

        public override void OnUnpooled()
        {
            base.OnUnpooled();
            RepoolSelfAfterDelay(_repoolDelay, disableImmediately: false);
        }
    }
}
