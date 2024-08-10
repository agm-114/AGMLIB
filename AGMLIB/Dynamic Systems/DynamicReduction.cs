using Bundles;
using Game.Sensors;
using Game.Units;
using HarmonyLib;
using Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

public class DynamicReduction : ActiveSettings
{
    public String ResourceName = "";
    public float Multiplier = 0.9f;
    private ResourceType res => ResourceDefinitions.Instance.GetResource(ResourceName);
    private ResourceValue[] _requiredResources;



    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (active != lastactive)
        {


        }
        //Debug.LogError("Last Fired " + lastfired + " Current Time " + Time.fixedTime + " Activate Time " +  (lastfired + fireactivetime) + " Status " + OnFireState);
        //Rigidbody?.AddForceAtPosition(transform.TransformDirection(Force/10), transform.position, Mode);

    }



    //public enum ForceMode { Start, Impulse, Acceleration, Force, VelocityChange };
}
public class OldResourceVals : MonoBehaviour
{
    public ResourceValue[] RequiredResources;
    public void Setup(HullComponent hullComponent)
    {
        if (RequiredResources == null)
            RequiredResources = Common.GetVal<ResourceValue[]>(hullComponent, "_requiredResources");
    }
}

[HarmonyPatch(typeof(HullComponent), nameof(HullComponent.ConsumeResources))]
class HullComponentConsumeResources
{
    public static ResourceValue Reduce(ResourceValue basevalue, IEnumerable<DynamicReduction> reductions)
    {
        reductions = reductions.Where(reduction  => basevalue.Resource.Name == reduction.ResourceName);
        if(!reductions?.Any() ?? false)
            return basevalue;
        return new ResourceValue(basevalue.Resource, (int)(float)(basevalue.AmountRequired * reductions.ConvertAll(reduction => reduction.Multiplier).Aggregate(1f, (a, x) => a * x)), basevalue.OnlyWhenOperating);
    }

    static void Prefix(HullComponent __instance, ResourcePool pool)
    {
        List<DynamicReduction> _reduced = __instance?.transform?.parent?.GetComponentsInChildren<DynamicReduction>().ToList();
        if (_reduced == null || _reduced.Count <= 0)
            return;
        OldResourceVals vals = __instance.GetComponent<OldResourceVals>();
        if(vals == null)
            __instance.gameObject.AddComponent<OldResourceVals>();
        vals.Setup(__instance);
        ResourceValue[] ModifiedRequiredResources = vals.RequiredResources.ConvertAll(element => Reduce(element, _reduced)).ToArray();
        //reduction.Setup(__instance, ____requiredResources);
        Common.SetVal(__instance, "_requiredResources", ModifiedRequiredResources);
    }

}

