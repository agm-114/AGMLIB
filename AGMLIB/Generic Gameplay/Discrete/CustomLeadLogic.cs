using Lib.Generic_Gameplay.Discrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DiscreteWeaponComponentLeadLogic : MonoBehaviour
{
    public DiscreteWeaponComponent Weapon;

    public bool LeadMissiles = true;
}
[HarmonyPatch(typeof(DiscreteWeaponComponent), "CalculateLead")]
class DiscreteWeaponComponentCalculateLead
{
    static void Postfix(
        DiscreteWeaponComponent __instance,
        Vector3 target,
        Vector3 velocity,
        Vector3 acceleration,
        float age,
        ref float flightTime,
        ref Vector3 __result)
    {
        if (__instance.TryGetComponent<DiscreteWeaponComponentLeadLogic>(out var leadLogic))
        {
            IMunition SelectedAmmoType = __instance.SelectedAmmoType;
            Vector3 _aimPosition = Common.GetVal<Transform>(__instance, "_aimCheckFrom").position;
            float muzzleVelocity = __instance.MuzzleVelocity;
            if(SelectedAmmoType is Missile missile)
            {
                muzzleVelocity = missile.FlightSpeed;
            }

            Vector3 leadTarget = MathHelpers.EstimateLeadPosition(_aimPosition, __instance.MuzzleVelocity, target, velocity, acceleration, out flightTime, age);
            __result = leadTarget;
        }
    }
}