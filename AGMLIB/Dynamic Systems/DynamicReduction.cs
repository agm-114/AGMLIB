using Bundles;
using HarmonyLib;
using Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;
using Munitions;
using UnityEngine.UI.Extensions;
using static Utility.GameColors;
using FleetEditor;
using TMPro;

public class DynamicReduction : ActiveSettings
{
    public String ResourceName = "";
    public float Multiplier = 0.9f;
    private ResourceType Res => ResourceDefinitions.Instance.GetResource(ResourceName);
    private ResourceValue[] _requiredResources;

    public List<string> Blacklist = new();
    public List<string> Whitelist = new();
    protected bool Default = true;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (active != lastactive)
        {
        }

    }

    public bool TargetedComponent(HullComponent hullComponent) 
    {
        if(hullComponent is not WeaponComponent weaponComponent) 
            return Default;

        MunitionTags[] _compatibleAmmoTags = Common.GetVal<MunitionTags[]>(weaponComponent, "_compatibleAmmoTags");
        IEnumerable<string> taglist = _compatibleAmmoTags.ToList().ConvertAll(tag => tag.Subclass);
        if(Blacklist.Intersect(taglist).Any())
            return false;
        if(Whitelist.Intersect(taglist).Any() ) 
            return true;
        taglist = _compatibleAmmoTags.ToList().ConvertAll(tag => tag.Class);
        if (Blacklist.Intersect(taglist).Any())
            return false;
        if (Whitelist.Intersect(taglist).Any())
            return true;
        return Default;
    }

    public static ResourceValue Reduce(ResourceValue basevalue, HullComponent hullComponent)
    {
        float multiplier = TotalMultiplier(basevalue.Resource, hullComponent);
        if (multiplier == 1)
            return basevalue;
        return new ResourceValue(basevalue.Resource, (int)(float)(basevalue.AmountRequired * multiplier), basevalue.OnlyWhenOperating);
    }

    public static float TotalMultiplier(ResourceType type, HullComponent hullComponent)
    {
        IEnumerable<DynamicReduction> reductions = hullComponent?.transform?.root?.GetComponentsInChildren<DynamicReduction>().ToList(); ;
        reductions = reductions.Where(reduction => type.Name == reduction.ResourceName);
        reductions = reductions.Where(reduction => reduction.TargetedComponent(hullComponent));
        if (!reductions?.Any() ?? false)
            return 1;
        return reductions.ConvertAll(reduction => reduction.Multiplier).Aggregate(1f, (a, x) => a * x);
    }
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

    public static void Prefix(HullComponent __instance, ResourcePool pool)
    {
        List<DynamicReduction> _reduced = __instance?.transform?.root?.GetComponentsInChildren<DynamicReduction>().ToList();
        if (_reduced == null || _reduced.Count <= 0)
            return;
        OldResourceVals vals = __instance.gameObject.GetOrAddComponent<OldResourceVals>();
        vals.Setup(__instance);
        ResourceValue[] ModifiedRequiredResources = vals.RequiredResources.ConvertAll(element => DynamicReduction.Reduce(element, __instance)).ToArray();
        //reduction.Setup(__instance, ____requiredResources);
        Common.SetVal(__instance, "_requiredResources", ModifiedRequiredResources);
    }

}


[HarmonyPatch(typeof(HullComponent), nameof(HullComponent.GetResourceDemand))]
class HullComponentGetResourceDemand
{
    static void Prefix(HullComponent __instance, ResourcePool pool)
    {
        HullComponentConsumeResources.Prefix(__instance, pool);
    }

}

[HarmonyPatch(typeof(ResourcePool), nameof(ResourcePool.CalculateDemandForEditor))]
class ResourcePoolCalculateDemandForEditor
{
    static void Postfix(ResourcePool __instance)
    {
        ResourcePool pool = __instance;

        List<HullComponent> _providers = Common.GetVal<List<HullComponent>>(pool, "_providers");
	    List<HullComponent> _consumers = Common.GetVal<List<HullComponent>>(pool, "_consumers");
        pool.Reset();
        pool.SortConsumers();
        string text = "";
        foreach (HullComponent provider in _providers)
        {
            if (provider.ResourcesProvided.Any((ResourceModifier x) => x.ResourceName == pool.Resource.Name))
            {
                ResourceModifier resourceModifier = provider.ResourcesProvided.First((ResourceModifier x) => x.ResourceName == pool.Resource.Name);
                int resourceProvideAmount = provider.GetResourceProvideAmount(pool.Resource);
                pool.AddAvailable(resourceProvideAmount);
                pool.AddUnmodifiedAvailable(resourceModifier.Amount);
                text += $"   <color={GameColors.GreenTextColor}>+{resourceProvideAmount}</color> - {provider.ShortUIName}\n";
            }
        }
        string text2 = "";
        foreach (HullComponent consumer in _consumers)
        {
            if (consumer.ResourcesRequired.Any((ResourceModifier x) => x.ResourceName == pool.Resource.Name))
            {
                ResourceModifier resourceModifier2 = consumer.ResourcesRequired.First((ResourceModifier x) => x.ResourceName == pool.Resource.Name);
                int num = Mathf.RoundToInt(consumer.GetResourceDemand(pool));
                pool.ConsumeGreedy(num);
                pool.AddUnmodifiedConsumed(resourceModifier2.Amount);
                text2 += $"   <color={GameColors.RedTextColor}>-{num}</color> - {consumer.ShortUIName}";
                float reduction = DynamicReduction.TotalMultiplier(pool.Resource, consumer);
                if (reduction != 1)
                    text2 += $" [{StatModifier.FormatModifierColored((float)Math.Round(reduction-1, 2), positiveBad: true)}]";
                text2 += '\n';
            }
        }
        string _editorDetails = "Produced:\n" + text.TrimEnd() + "\n\nConsumed:\n" + text2.TrimEnd() + "\n";
        Common.SetVal(pool, "_editorDetails", _editorDetails);
    }

}

[HarmonyPatch(typeof(ResourceItem), nameof(ResourceItem.SetResource))]
class ResourceItemSetResource
{
    static void Postfix(ResourceItem __instance, IReadOnlyResourcePool resource)
    {
        ResourceItem item = __instance;
        TextMeshProUGUI _summaryText = Common.GetVal<TextMeshProUGUI>(item, "_summaryText");

        float num = (float)resource.AmountConsumed / (float)resource.TotalAvailable;
        string text = ((resource.ConsumedModifier == 0f) ? "" : (" (" + StatModifier.FormatModifierColored((float)Math.Round(resource.ConsumedModifier, 2) , positiveBad: true) + ")"));
        string text2 = ((resource.ProducedModifier == 0f) ? "" : (" (" + StatModifier.FormatModifierColored((float)Math.Round(resource.ProducedModifier, 2), positiveBad: false) + ")"));
        _summaryText.text = $"{resource.ResourceName} - {Mathf.Round(num * 100f)}%\n<size=20>{resource.AmountConsumed} {resource.UnitAbbrev}{text} / {resource.TotalAvailable} {resource.UnitAbbrev}{text2}</size>";
    }

}