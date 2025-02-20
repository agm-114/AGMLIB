using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Lib.Generic_Gameplay.Ewar
{
    public class MultiSensor : ActiveFireControlSensor
    {



    }

    public class MultiActiveFireControlSensor : ActiveFireControlSensor
    {
        private DeltaSensor<IActiveSignature> _deltaSensor;

        public void Update()
        {
            //this.IsWorking;
        }
    }

}