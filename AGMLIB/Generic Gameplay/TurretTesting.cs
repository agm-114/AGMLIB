using Game.EWar;
using Game.Sensors;
using HarmonyLib;
using Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI;
using UnityEngine;
using UnityEngine.Profiling;
using Utility;


public class CustomTartTraversalLimits : CustomTraversalLimits
{
    public override TraversalLimits PublicForwardLimits
    {
        get
        {
            TraversalLimits forwardLimits = new();
            forwardLimits.LimitFiringOnly = ForwardLimits.LimitFiringOnly;
            forwardLimits.LeftAngle = ForwardLimits.LeftAngle;
            forwardLimits.RightAngle = ForwardLimits.RightAngle;
            forwardLimits.UseElevationLimit = ForwardLimits.UseElevationLimit;
            forwardLimits.ElevationAngle = 180 - ForwardLimits.ElevationAngle;
            return forwardLimits;
        }
        set => ForwardLimits = value;
    }

}

public class CustomTraversalLimits : MonoBehaviour
{
    public TraversalLimits ForwardLimits = new();
    public HullSocket socket;
    public virtual TraversalLimits PublicForwardLimits { get => ForwardLimits; set => ForwardLimits = value; }

    public bool Blocked
    {
        get
        {
            if (socket == null)
            {
                //Debug.LogError("No locked socket");
                return false; 
            }
            if(socket.Component == null)
            {
                //Debug.LogError("No Blocking component");
                return false;
            }
            return true;
        }
    }
}

//[HarmonyPatch(typeof(TurretController), nameof(TurretController.TargetWithinLimits))]
class TurretControllerTargetWithinLimits
{

    /*
     * 
        Vector3 to = base.transform.InverseTransformPoint(worldPosition);
        Vector3 normalized = new Vector3(to.x, 0f, to.z).normalized;
        float num = Vector3.Angle(normalized, to);
        if ((to.y >= 0f && num > _maxElevation) || (to.y < 0f && num > Mathf.Abs(_minElevation)))
        {
            return false;
        }

        if (_limits.HasValue)
        {
            if (_limits.Value.LimitFiringOnly && _limits.Value.UseElevationLimit && to.y >= 0f && num >= _limits.Value.ElevationAngle)
            {
                return true;
            }

            float num2 = Vector3.SignedAngle(Vector3.forward, normalized, Vector3.up);
            return (num2 >= 0f || Mathf.Abs(num2) <= _limits.Value.LeftAngle) && (num2 < 0f || num2 <= _limits.Value.RightAngle);
        }

        return true;
     */

    static void Postfix(TurretController __instance)//, Vector3 worldPosition, ref bool __result
    {
        return;
        /*
        if (__result == false)
            return;



        Vector3 to = __instance.transform.InverseTransformPoint(worldPosition);
        Vector3 normalized = new Vector3(to.x, 0f, to.z).normalized;
        float num = Vector3.Angle(normalized, to);
        float _minElevation = Common.GetVal<float>(__instance, "_minElevation");
        float _maxElevation = Common.GetVal<float>(__instance, "_maxElevation");

        if ((to.y >= 0f && num > _maxElevation) || (to.y < 0f && num > Mathf.Abs(_minElevation)))
        {
            return;
        }


        Vector3 local = __instance.transform.InverseTransformPoint(worldPosition);
        Vector3 onHorizontal = new Vector3(local.x, 0f, local.z).normalized;
        float elevation = Vector3.Angle(onHorizontal, local);
        TraversalLimits? _limits = Common.GetVal<TraversalLimits?>(__instance, "_forwardLimits"); 

        if (_limits.HasValue)
        {
            return;
            if (_limits.Value.LimitFiringOnly && _limits.Value.UseElevationLimit && local.y >= 0f && elevation >= _limits.Value.ElevationAngle)
            {
                return;
            }

            float traverse = Vector3.SignedAngle(Vector3.forward, onHorizontal, Vector3.up);
            Debug.LogError("Limits test" + traverse + " " + _limits.Value.LeftAngle + " " + _limits.Value.RightAngle);
            __result =(traverse >= 0f || Mathf.Abs(traverse) <= _limits.Value.LeftAngle) && (traverse < 0f || traverse <= _limits.Value.RightAngle);
        }
        */
    }
}

[HarmonyPatch(typeof(TurretController), nameof(TurretController.FaceTarget))]
class TurretControllerFaceTarget
{

    static void Postfix(TurretController __instance)
    {
         
        if (__instance.gameObject.transform.GetComponentInParent<CustomTraversalLimits>() is not CustomTraversalLimits customlimits)
        {
            return;
        }
        if(!customlimits.Blocked)
        {
            Common.SetVal<bool>(__instance, "_insideForwardLimits", false);
            return;

        }
        TraversalLimits _forwardLimits = customlimits.ForwardLimits;
        Common.SetVal<bool>(__instance, "_insideForwardLimits", true);
        Transform _body = Common.GetVal<Transform>(__instance, "_body"); ;
        Transform _barrel = Common.GetVal<Transform>(__instance, "_barrel"); ;
        
        float traverse = MathHelpers.ConvertAngle360to180(_body.localRotation.eulerAngles.y);
        float elevation = MathHelpers.ConvertAngle360to180(_barrel.localRotation.eulerAngles.x) * -1;
        //Debug.LogError("Limits test "  + _forwardLimits.LeftAngle * -1 + " " + _forwardLimits.RightAngle + " " +  (int)traverse);
        //Debug.LogError("Limits test "  + (traverse > (_forwardLimits.LeftAngle * -1)) + " " + (traverse < _forwardLimits.RightAngle) + " " +  (int)traverse);
        //Debug.LogError("Elevation test " + (int)elevation);
        if (traverse > (_forwardLimits.LeftAngle * -1) && traverse < _forwardLimits.RightAngle)
        {
            if (!_forwardLimits.UseElevationLimit || elevation < _forwardLimits.ElevationAngle)
                Common.SetVal<bool>(__instance, "_insideForwardLimits", false);
            //Debug.LogError("Outside Limits");
        }
    }
}
/* This is the version that messed with vanilla 
   
  static void Prefix(TurretController __instance)
    {
        _forwardLimits = Common.GetVal<TraversalLimits?>(__instance, "_forwardLimits");
        Common.SetVal<TraversalLimits?>(__instance, "_forwardLimits", (TraversalLimits?)null);
        Common.SetVal<bool>(__instance, "_insideForwardLimits", true);

    }
     
    static void Postfix(TurretController __instance)
    {


        Common.SetVal<bool>(__instance, "_insideForwardLimits", true);

        if (_forwardLimits.HasValue)
        {
            Transform _body = Common.GetVal<Transform>(__instance, "_body"); ;
            Transform _barrel = Common.GetVal<Transform>(__instance, "_barrel"); ;

            float traverse = MathHelpers.ConvertAngle360to180(_body.localRotation.eulerAngles.y);
            float elevation = MathHelpers.ConvertAngle360to180(_barrel.localRotation.eulerAngles.x) * -1;
            //Debug.LogError("Limits test " + (int)traverse + " " + _forwardLimits.Value.LeftAngle * -1 + " " + _forwardLimits.Value.RightAngle);
            //Debug.LogError("Limits test " + (int)traverse + " " + (traverse > (_forwardLimits.Value.LeftAngle * -1)) + " " + (traverse < _forwardLimits.Value.RightAngle));
            //Debug.LogError("Elevation test " + (int)elevation);

            if (traverse > (_forwardLimits.Value.LeftAngle * -1)  && traverse < _forwardLimits.Value.RightAngle)
            {
                if(!_forwardLimits.Value.UseElevationLimit || elevation <  _forwardLimits.Value.ElevationAngle)
                    Common.SetVal<bool>(__instance, "_insideForwardLimits", false);
                //Debug.LogError("Outside Limits");
            }




        }
        else
        {
            //Debug.LogError("No forward limits");
        }


        
        Common.SetVal<TraversalLimits?>(__instance, "_forwardLimits", _forwardLimits);


        _forwardLimits = null;
    }
}
*/