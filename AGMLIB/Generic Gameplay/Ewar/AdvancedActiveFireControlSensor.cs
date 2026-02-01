using Lib.Generic_Gameplay.Ewar;
using Munitions.ModularMissiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Lib.Generic_Gameplay.Ewar
{
    public class ActiveFireControlSensorOptions : MonoBehaviour
    {

        public bool AllowModifedMaxRange = true;

        public static bool CanSeeSignature(ActiveFireControlSensor sensor, ISignature sig)
        {
            //Common.Hint("using modified max range");
            float _maxRange = Common.GetVal<StatValue>(sensor, "_statMaxRange").Value;
            float _fov = Common.GetVal<float>(sensor, "_fov");

            Vector3 to = sig.Position - sensor.transform.position;

            if (to.magnitude - sig.SigRadius > _maxRange || Vector3.Angle(sensor.transform.forward, to) > _fov)
            {
                return false;
            }

            if (Physics.Linecast(sensor.transform.position, sig.Position, out var _, 512, QueryTriggerInteraction.Ignore))
            {
                return false;
            }

            return true;
        }
    }
}

[HarmonyPatch(typeof(ActiveFireControlSensor), nameof(ActiveFireControlSensor.CanSeeSignature))]
class ActiveFireControlSensorCanSeeSignature
{
    public static void Postfix(ISignature sig, ref bool __result, ActiveFireControlSensor __instance)
    {
        ActiveFireControlSensorOptions options = __instance.gameObject.GetComponent<ActiveFireControlSensorOptions>();
        if (options != null)
        {
            if (options.AllowModifedMaxRange) 
                __result = ActiveFireControlSensorOptions.CanSeeSignature(__instance, sig);
        }
        //__result = true;
    }
}