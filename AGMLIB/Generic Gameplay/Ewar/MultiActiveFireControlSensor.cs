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