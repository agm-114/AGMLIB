using UnityEngine.UI.Extensions;
using ResourceType = Ships.ResourceType;
public class DynamicReductionCache : MonoBehaviour
{
    public List<DynamicReduction> dynamicReductions = new List<DynamicReduction>();
    public Dictionary<ResourcePool, int> AmountExtra = new();

    public static void CacheValues(Ship ship)
    {


        DynamicReductionCache cache = ship.GetComponent<DynamicReductionCache>() ?? ship.gameObject.AddComponent<DynamicReductionCache>();
        cache.AmountExtra = Common.GetVal<Dictionary<string, ResourcePool>>(ship, "_resources").Values.ToDictionary(pool => pool, pool => pool.AmountRemaining);
        cache.dynamicReductions = ship.transform?.root?.GetComponentsInChildren<DynamicReduction>().ToList() ?? new();
        //Common.Trace($"CacheValues {cache.dynamicReductions.Count()}");
    }
}

public class DynamicReduction : ActiveSettings
{
    public String ResourceName = "";
    public float Multiplier = 0.9f;
    private ResourceType Res => ResourceDefinitions.Instance.GetResource(ResourceName);
    private ResourceValue[] _requiredResources;

    public BaseFilter Filter;

    public override void Awake()
    {
        base.Awake();
        //if(Filter == null)
        //    Common.Hint($"reduction {gameObject} has null filter ");

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (active != lastactive)
        {
        }

    }

    public bool TargetedComponent(HullComponent hullComponent)
    {
        //Common.Trace($"filter {Filter?.gameObject.name ?? "null filter"} hullcomp {hullComponent.SaveKey}");
        //if (!Module.isActiveAndEnabled)
        //    return false;
        return Filter?.CheckComponent(hullComponent) ?? true;
    }

    public static ResourceValue Reduce(ResourceValue basevalue, HullComponent hullComponent)
    {
        float multiplier = TotalMultiplier(basevalue.Resource, hullComponent);
        if (multiplier == 1)
            return basevalue;
        return new ResourceValue(basevalue.Resource, (int)(float)(basevalue.AmountRequired * multiplier), basevalue.OnlyWhenOperating);
    }
    public static IEnumerable<DynamicReduction> GetReductions(HullComponent hullComponent)
    {
        return hullComponent?.transform?.root?.GetComponent<DynamicReductionCache>().dynamicReductions.Where(reduction => reduction.TargetedComponent(hullComponent)) ?? new List<DynamicReduction>();

    }

    public static float TotalMultiplier(ResourceType type, HullComponent hullComponent)
    {

        IEnumerable<DynamicReduction> reductions = GetReductions(hullComponent);
        if (!reductions?.Any() ?? false)
            return 1;
        //Common.Trace($"start {type.Name} {hullComponent.UIName} has {reductions.Count()} valid reduction");
        //foreach (DynamicReduction reduction in reductions)
        //    Common.Trace($"{reduction.gameObject}  has {reduction.ResourceName} reduction of {reduction.Multiplier}");
        reductions = reductions.Where(reduction => type.Name == reduction.ResourceName);
        //Common.Trace($"trim {type.Name} {hullComponent.UIName} has {reductions.Count()} valid reduction");

        if (!reductions?.Any() ?? false)
            return 1;

        float reductionf = reductions.ConvertAll(reduction => reduction.Multiplier).Aggregate(1f, (a, x) => a * x);
        //Common.Trace($"reduction {reductionf} {type.Name} {hullComponent.UIName} has {reductions.Count()} valid reduction");

        return reductionf;
    }

    public static void UpdateResources(HullComponent hullComponent)
    {
        IEnumerable<DynamicReduction> _reduced = GetReductions(hullComponent);
        RequiredResources vals = hullComponent.gameObject.GetOrAddComponent<RequiredResources>();
        vals?.Setup(hullComponent);

        if (_reduced == null || _reduced.Count() <= 0)
        {
            return;
        }


        ResourceValue[] ModifiedRequiredResources = vals.Base.ConvertAll(element => DynamicReduction.Reduce(element, hullComponent)).ToArray();
        //reduction.Setup(__instance, ____requiredResources);
        vals.SetRequireResoures(ModifiedRequiredResources);
    }
}
public class RequiredResources : MonoBehaviour
{
    HullComponent _hullComponent;
    public ResourceValue[] Base;

    public ResourceValue[] Current => Common.GetVal<ResourceValue[]>(_hullComponent, "_requiredResources");

    public void SetRequireResoures(ResourceValue[] resourceValues)
    {
        Common.SetVal(_hullComponent, "_requiredResources", resourceValues);

    }
    public void Setup(HullComponent hullComponent)
    {
        _hullComponent = hullComponent;
        if (Base == null)
            Base = Current;
        SetRequireResoures(Base);

    }



    public void Reset()
    {

    }
}



[HarmonyPatch(typeof(Ship), nameof(Ship.SpawnAndAllocateResources))]
class ShipSpawnAndAllocateResources
{
    public static void Prefix(Ship __instance)
    {
        Common.LogPatch();
        DynamicReductionCache.CacheValues(__instance);
    }
}

[HarmonyPatch(typeof(Ship), nameof(Ship.RunResourceTick))]
class ShipRunResourceTick
{
    public static void Prefix(Ship __instance)
    {
        Common.LogPatch();
        DynamicReductionCache.CacheValues(__instance);
    }

}

[HarmonyPatch(typeof(Ship), nameof(Ship.EditorRecalcCrewAndResources))]
class ShipEditorRecalcCrewAndResources
{
    public static void Prefix(Ship __instance)
    {
        Common.LogPatch();
        DynamicReductionCache.CacheValues(__instance);
    }
}

[HarmonyPatch(typeof(HullComponent), nameof(HullComponent.ConsumeResources))]
class HullComponentConsumeResources
{

    public static void Prefix(HullComponent __instance, ResourcePool pool)
    {
        Common.LogPatch();
        //Debug.LogError("ticking consumer");
        DynamicReduction.UpdateResources(__instance);
    }

}


[HarmonyPatch(typeof(HullComponent), nameof(HullComponent.GetResourceDemand))]
class HullComponentGetResourceDemand
{
    static void Prefix(HullComponent __instance, ResourcePool pool)
    {
        Common.LogPatch();
        DynamicReduction.UpdateResources(__instance);
    }

}

[HarmonyPatch(typeof(ResourcePool), nameof(ResourcePool.CalculateDemandForEditor))]
class ResourcePoolCalculateDemandForEditor
{
    static void Postfix(ResourcePool __instance)
    {
        Common.LogPatch();
        //Common.Trace("calc demends");

        ResourcePool pool = __instance;
        ResourceType Resource = pool.Resource;

        List<HullComponent> _providers = Common.GetVal<List<HullComponent>>(pool, "_providers");
        List<HullComponent> _consumers = Common.GetVal<List<HullComponent>>(pool, "_consumers");
        HashSet<HullComponent> components = new HashSet<HullComponent>(_providers);
        components.UnionWith(_consumers);
        List<float> reductions = new();
        foreach (HullComponent comp in components)
        {

            float reduction = DynamicReduction.TotalMultiplier(pool.Resource, comp);
            if (reduction != 1)
                reductions.Add(reduction);

        }
        if (reductions.Count <= 0)
            return;

        //IEnumerable<DynamicReduction> reductions = _consumers?.First()?.transform?.root?.GetComponentsInChildren<DynamicReduction>().ToList();
        //foreach(DynamicReduction reduction in reductions)
        //{
        //{
        //Common.Trace(reduction.ToString());
        //}

        pool.Reset();
        pool.SortConsumers();
        EditorResourceSummary _summary = default(EditorResourceSummary);
        _summary.Details = ["Produced:"];
        foreach (HullComponent provider in _providers)
        {
            if (provider.ResourcesProvided.Any((ResourceModifier x) => x.ResourceName == pool.Resource.Name))
            {

                int baseAmount;
                int resourceProvideAmount = provider.GetResourceProvideAmount(Resource, out baseAmount);
                _summary.TotalProvided += resourceProvideAmount;
                _summary.TotalProvidedUnmodified += baseAmount;
                pool.AddAvailable(resourceProvideAmount);
                pool.AddUnmodifiedAvailable(baseAmount);
                _summary.Details.Add($"   <color={GameColors.GreenTextColor}>+{resourceProvideAmount}</color> - {provider.ShortUIName}");
            }
        }
        _summary.Details.Add("Consumed:");


        foreach (HullComponent consumer in _consumers)
        {
            if (consumer.ResourcesRequired.Any((ResourceModifier x) => x.ResourceName == Resource.Name))
            {
                ResourceModifier resourceModifier = consumer.ResourcesRequired.First((ResourceModifier x) => x.ResourceName == Resource.Name);
                bool onlyWhenOperating;
                float reduction = DynamicReduction.TotalMultiplier(pool.Resource, consumer);

                int num = Mathf.RoundToInt(consumer.GetResourceDemand(pool, out onlyWhenOperating) * reduction);

                _summary.TotalConsumed += num;
                _summary.TotalConsumedUnmodified += resourceModifier.Amount;
                if (onlyWhenOperating)
                {
                    _summary.OperatingConsumed += num;
                }
                else
                {
                    _summary.AlwaysConsumed += num;
                }

                pool.ConsumeGreedy(num);
                pool.AddUnmodifiedConsumed(resourceModifier.Amount);
                _summary.Details.Add($"   <color={(onlyWhenOperating ? GameColors.YellowTextColor : GameColors.RedTextColor)}>-{num}</color> - {consumer.ShortUIName}");
                if (reduction != 1)
                    _summary.Details[_summary.Details.Count - 1] += $" [{StatModifier.FormatModifierColored((float)Math.Round(reduction - 1, 2), positiveBad: true)}]";

            }
        }


        Common.SetVal(pool, "_summary", _summary);
    }

}

[HarmonyPatch(typeof(ResourceItem), nameof(ResourceItem.SetResource))]
class ResourceItemSetResource
{
    static void Postfix(ResourceItem __instance, IReadOnlyResourcePool resource)
    {
        Common.LogPatch();
        ResourceItem item = __instance;
        TextMeshProUGUI _summaryText = Common.GetVal<TextMeshProUGUI>(item, "_summaryText");

        float num = (float)resource.AmountConsumed / (float)resource.TotalAvailable;
        string text = ((resource.ConsumedModifier == 0f) ? "" : (" (" + StatModifier.FormatModifierColored((float)Math.Round(resource.ConsumedModifier, 2), positiveBad: true) + ")"));
        string text2 = ((resource.ProducedModifier == 0f) ? "" : (" (" + StatModifier.FormatModifierColored((float)Math.Round(resource.ProducedModifier, 2), positiveBad: false) + ")"));
        _summaryText.text = $"{resource.ResourceName} - {Mathf.Round(num * 100f)}%\n<size=20>{resource.AmountConsumed} {resource.UnitAbbrev}{text} / {resource.TotalAvailable} {resource.UnitAbbrev}{text2}</size>";
    }

}