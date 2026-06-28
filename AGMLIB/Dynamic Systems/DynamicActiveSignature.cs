using Scripting.Nodes.General;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Experimental.GlobalIllumination;
using static System.Net.WebRequestMethods;
using static Testing.CameraGun;
[RequireComponent(typeof(Signature))]
public class DynamicActiveSignature : ActiveSettings
{
    public Signature Signature;

    public bool DisableSignature = true;

    protected override void FixedUpdate()
    {

        base.FixedUpdate();
        /*
        if (!active)
        {
            if (DisableWeapons)
                Common.SetVal(ShipController, "_weaponsControl", WeaponsControlStatus.Hold);
        }
        */
        /*
        if (active != MonoBehaviour.isActiveAndEnabled)
        {
            MonoBehaviour.enabled = active;
            if (MonoBehaviour is BaseSignature sig)
            {
                Common.RunFunc(sig, "FireSignatureChangedEvent", []);
                //sig.OnSignatureChanged.Invoke();
            }
        }

        */
    }
    //public enum ForceMode { Start, Impulse, Acceleration, Force, VelocityChange };
}



[HarmonyPatch(typeof(Signature), nameof(Signature.CheckOccluded))]
class SignatureCheckOccluded
{
    static void Postfix(
        Signature __instance,
        Vector3 sourcePosition,
        ref bool __result)
    {
        DynamicActiveSignature? siglogic = __instance.gameObject.GetComponentInChildren<DynamicActiveSignature>();
        if (siglogic != null && siglogic.DisableSignature && siglogic.active)
        {
            //Debug.LogError("SignatureCheckOccluded: " + __instance.name + " has no DynamicActiveSignature component.");
            //Debug.LogError("SignatureCheckNotOccluded: " + __instance.name + " is active: " + siglogic.active + ", DisableSignature: " + siglogic.DisableSignature);
            //Debug.LogError("SignatureCheckOccluded: " + __instance.name + " is active: " + siglogic.active + ", DisableSignature: " + siglogic.DisableSignature);
            __result = true;
            return;
        }

    }
}



[HarmonyPatch(typeof(Signature), nameof(Signature.GetReturnPowerDensity))]
class SignatureGetReturnPowerDensity
{
    static void Postfix(
        Signature __instance,
        ref float __result)
    {
        DynamicActiveSignature? siglogic = __instance.gameObject.GetComponentInChildren<DynamicActiveSignature>();
        if (siglogic != null && siglogic.DisableSignature && siglogic.active)
        {
            //Debug.LogError("SignatureCheckOccluded: " + __instance.name + " has no DynamicActiveSignature component.");
            //Debug.LogError("SignatureCheckNotOccluded: " + __instance.name + " is active: " + siglogic.active + ", DisableSignature: " + siglogic.DisableSignature);
            //Debug.LogError("SignatureCheckOccluded: " + __instance.name + " is active: " + siglogic.active + ", DisableSignature: " + siglogic.DisableSignature);
            __result = 0;
            return;
        }
        

    }
}


/*
[HarmonyPatch(typeof(Signature), nameof(Signature.GetCrossSection))]
class SignatureGetCrossSection
{
    static void Postfix(
        Signature __instance,
        Vector3 worldDirection,
        bool ignoreCached,
        ref float __result)
    {
        DynamicActiveSignature? siglogic = __instance.gameObject.GetComponentInChildren<DynamicActiveSignature>();
        if (siglogic != null && siglogic.DisableSignature && siglogic.active)
        {
            //Debug.LogError("SignatureCheckOccluded: " + __instance.name + " has no DynamicActiveSignature component.");
            //Debug.LogError("SignatureCheckNotOccluded: " + __instance.name + " is active: " + siglogic.active + ", DisableSignature: " + siglogic.DisableSignature);
            //Debug.LogError("SignatureCheckOccluded: " + __instance.name + " is active: " + siglogic.active + ", DisableSignature: " + siglogic.DisableSignature);
            __result = 0;
            return;
        }

    }
}
*/  

