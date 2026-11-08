using HarmonyLib;
using Munitions;
using Ships;

public static partial class NativeInternalsExtensions
{
    public static HullComponentInternals Internals(this HullComponent component) => new(component);
}

public readonly struct HullComponentInternals
{
    private static class Refs
    {
        internal static readonly Func<object, ComponentDebuff> InstanceDebuff =
            NativePrivateFields.Getter<ComponentDebuff>(typeof(HullComponent), "DebuffInstance", "Debuff");
        internal static readonly Func<object, int> DebuffInstanceId =
            NativePrivateFields.Getter<int>(typeof(HullComponent), "DebuffInstance", "InstanceID");
        internal static readonly Func<HullComponent, int> CalcTileMultiplier =
            AccessTools.MethodDelegate<Func<HullComponent, int>>(AccessTools.Method(typeof(HullComponent), "CalcTileMultiplier"));
        internal static readonly AccessTools.FieldRef<HullComponent, Vector3Int> Size =
            AccessTools.FieldRefAccess<HullComponent, Vector3Int>("_size");
        internal static readonly AccessTools.FieldRef<HullComponent, string> BindToTag =
            AccessTools.FieldRefAccess<HullComponent, string>("_bindToTag");
        internal static readonly Action<HullComponent, ComponentDebuff, MunitionHitInfo, bool> AddDebuffToComponent =
            AccessTools.MethodDelegate<Action<HullComponent, ComponentDebuff, MunitionHitInfo, bool>>(
                AccessTools.Method(typeof(HullComponent), "AddDebuffToComponent", [typeof(ComponentDebuff), typeof(MunitionHitInfo), typeof(bool)])
                ?? throw new MissingMethodException(typeof(HullComponent).FullName, "AddDebuffToComponent"));
        internal static readonly AccessTools.FieldRef<HullComponent, List<ComponentDebuff>> AllDebuffs =
            AccessTools.FieldRefAccess<HullComponent, List<ComponentDebuff>>("_allDebuffs");
        internal static readonly AccessTools.FieldRef<HullComponent, List<StatValue>> AllStatValues =
            AccessTools.FieldRefAccess<HullComponent, List<StatValue>>("_allStatValues");
        internal static readonly AccessTools.FieldRef<HullComponent, List<HullPart>> BoundParts =
            AccessTools.FieldRefAccess<HullComponent, List<HullPart>>("_boundParts");
        internal static readonly AccessTools.FieldRef<HullComponent, string> CachedSocketKey =
            AccessTools.FieldRefAccess<HullComponent, string>("_cachedSocketKey");
        internal static readonly AccessTools.FieldRef<HullComponent, bool> CanTile =
            AccessTools.FieldRefAccess<HullComponent, bool>("_canTile");
        internal static readonly AccessTools.FieldRef<HullComponent, string> Category =
            AccessTools.FieldRefAccess<HullComponent, string>("_category");
        internal static readonly AccessTools.FieldRef<HullComponent, HullComponent.IHullComponentRPC> ComponentRpcProvider =
            AccessTools.FieldRefAccess<HullComponent, HullComponent.IHullComponentRPC>("_componentRpcProvider");
        internal static readonly AccessTools.FieldRef<HullComponent, bool> CompoundingCost =
            AccessTools.FieldRefAccess<HullComponent, bool>("_compoundingCost");
        internal static readonly AccessTools.FieldRef<HullComponent, string> CompoundingCostClass =
            AccessTools.FieldRefAccess<HullComponent, string>("_compoundingCostClass");
        internal static readonly AccessTools.FieldRef<HullComponent, float> CompoundingMultiplier =
            AccessTools.FieldRefAccess<HullComponent, float>("_compoundingMultiplier");
        internal static readonly AccessTools.FieldRef<HullComponent, bool> ConsistentCostForTiled =
            AccessTools.FieldRefAccess<HullComponent, bool>("_consistentCostForTiled");
        internal static readonly AccessTools.FieldRef<HullComponent, ComponentCostClass> CostBreakdownClass =
            AccessTools.FieldRefAccess<HullComponent, ComponentCostClass>("_costBreakdownClass");
        internal static readonly AccessTools.FieldRef<HullComponent, bool> DebuffAddedThisFrame =
            AccessTools.FieldRefAccess<HullComponent, bool>("_debuffAddedThisFrame");
        internal static readonly AccessTools.FieldRef<HullComponent, AnimationCurve> DebuffProbability =
            AccessTools.FieldRefAccess<HullComponent, AnimationCurve>("_debuffProbability");
        internal static readonly AccessTools.FieldRef<HullComponent, string> FactionKey =
            AccessTools.FieldRefAccess<HullComponent, string>("_factionKey");
        internal static readonly AccessTools.FieldRef<HullComponent, bool> FirstInstanceFree =
            AccessTools.FieldRefAccess<HullComponent, bool>("_firstInstanceFree");
        internal static readonly AccessTools.FieldRef<HullComponent, string> FlavorText =
            AccessTools.FieldRefAccess<HullComponent, string>("_flavorText");
        internal static readonly AccessTools.FieldRef<HullComponent, Material[]> HullDamageMaterials =
            AccessTools.FieldRefAccess<HullComponent, Material[]>("_hullDamageMaterials");
        internal static readonly AccessTools.FieldRef<HullComponent, Sides> InteriorOverhang =
            AccessTools.FieldRefAccess<HullComponent, Sides>("_interiorOverhang");
        internal static readonly AccessTools.FieldRef<HullComponent, bool> IsTiled =
            AccessTools.FieldRefAccess<HullComponent, bool>("_isTiled");
        internal static readonly AccessTools.FieldRef<HullComponent, Sprite> LargeImage =
            AccessTools.FieldRefAccess<HullComponent, Sprite>("_largeImage");
        internal static readonly AccessTools.FieldRef<HullComponent, string> LongDescription =
            AccessTools.FieldRefAccess<HullComponent, string>("_longDescription");
        internal static readonly AccessTools.FieldRef<HullComponent, float> Mass =
            AccessTools.FieldRefAccess<HullComponent, float>("_mass");
        internal static readonly AccessTools.FieldRef<HullComponent, ulong> ModId =
            AccessTools.FieldRefAccess<HullComponent, ulong>("_modId");
        internal static readonly AccessTools.FieldRef<HullComponent, List<ComponentDebuff>> NormalDebuffTable =
            AccessTools.FieldRefAccess<HullComponent, List<ComponentDebuff>>("_normalDebuffTable");
        internal static readonly AccessTools.FieldRef<HullComponent, int> PointCost =
            AccessTools.FieldRefAccess<HullComponent, int>("_pointCost");
        internal static readonly AccessTools.FieldRef<HullComponent, float> RareDebuffChance =
            AccessTools.FieldRefAccess<HullComponent, float>("_rareDebuffChance");
        internal static readonly AccessTools.FieldRef<HullComponent, string> RareDebuffSubtype =
            AccessTools.FieldRefAccess<HullComponent, string>("_rareDebuffSubtype");
        internal static readonly AccessTools.FieldRef<HullComponent, List<ComponentDebuff>> RareDebuffTable =
            AccessTools.FieldRefAccess<HullComponent, List<ComponentDebuff>>("_rareDebuffTable");
        internal static readonly AccessTools.FieldRef<HullComponent, bool> RemoveSocketCap =
            AccessTools.FieldRefAccess<HullComponent, bool>("_removeSocketCap");
        internal static readonly AccessTools.FieldRef<HullComponent, HullComponent.RotateAxis> RotateToFit =
            AccessTools.FieldRefAccess<HullComponent, HullComponent.RotateAxis>("_rotateToFit");
        internal static readonly AccessTools.FieldRef<HullComponent, string> SaveKey =
            AccessTools.FieldRefAccess<HullComponent, string>("_saveKey");
        internal static readonly AccessTools.FieldRef<HullComponent, string> ShortDescription =
            AccessTools.FieldRefAccess<HullComponent, string>("_shortDescription");
        internal static readonly AccessTools.FieldRef<HullComponent, string> ShortUIName =
            AccessTools.FieldRefAccess<HullComponent, string>("_shortUIName");
        internal static readonly AccessTools.FieldRef<HullComponent, Sprite> SmallImage =
            AccessTools.FieldRefAccess<HullComponent, Sprite>("_smallImage");
        internal static readonly AccessTools.FieldRef<HullComponent, IHullSocket> Socket =
            AccessTools.FieldRefAccess<HullComponent, IHullSocket>("_socket");
        internal static readonly AccessTools.FieldRef<HullComponent, StatValue> StatCrewWorkSpeed =
            AccessTools.FieldRefAccess<HullComponent, StatValue>("_statCrewWorkSpeed");
        internal static readonly AccessTools.FieldRef<HullComponent, StatValue> StatDamageThreshold =
            AccessTools.FieldRefAccess<HullComponent, StatValue>("_statDamageThreshold");
        internal static readonly AccessTools.FieldRef<HullComponent, StatValue> StatMaxHP =
            AccessTools.FieldRefAccess<HullComponent, StatValue>("_statMaxHP");
        internal static readonly AccessTools.FieldRef<HullComponent, StatValue> StatRareDebuffChance =
            AccessTools.FieldRefAccess<HullComponent, StatValue>("_statRareDebuffChance");
        internal static readonly AccessTools.FieldRef<HullComponent, bool> StatsInitialized =
            AccessTools.FieldRefAccess<HullComponent, bool>("_statsInitialized");
        internal static readonly AccessTools.FieldRef<HullComponent, int> TiledSizeMultiplier =
            AccessTools.FieldRefAccess<HullComponent, int>("_tiledSizeMultiplier");
        internal static readonly AccessTools.FieldRef<HullComponent, HullSocketType> Type =
            AccessTools.FieldRefAccess<HullComponent, HullSocketType>("_type");
        internal static readonly AccessTools.FieldRef<HullComponent, ComponentDebuff[]> UniqueDebuffs =
            AccessTools.FieldRefAccess<HullComponent, ComponentDebuff[]>("_uniqueDebuffs");
        internal static readonly AccessTools.FieldRef<HullComponent, IEnumerable> ActiveDebuffs =
            AccessTools.FieldRefAccess<HullComponent, IEnumerable>("_activeDebuffs");
    }

    private readonly HullComponent _component;
    internal HullComponentInternals(HullComponent component)
    {
        _component = component ?? throw new ArgumentNullException(nameof(component));
    }

    public ref Vector3Int Size => ref Refs.Size(_component);
    public int TileMultiplier => Refs.CalcTileMultiplier(_component);
    public ref string BindToTag => ref Refs.BindToTag(_component);

    public void AddDebuff(ComponentDebuff debuff, MunitionHitInfo hitInfo, bool checkValid = false)
    {
        Refs.AddDebuffToComponent(_component, debuff, hitInfo, checkValid);
    }

    public ref List<ComponentDebuff> AllDebuffs => ref Refs.AllDebuffs(_component);
    public ref List<StatValue> AllStatValues => ref Refs.AllStatValues(_component);
    public ref List<HullPart> BoundParts => ref Refs.BoundParts(_component);
    public ref string CachedSocketKey => ref Refs.CachedSocketKey(_component);
    public ref bool CanTile => ref Refs.CanTile(_component);
    public ref string Category => ref Refs.Category(_component);
    public ref HullComponent.IHullComponentRPC ComponentRpcProvider => ref Refs.ComponentRpcProvider(_component);
    public ref bool CompoundingCost => ref Refs.CompoundingCost(_component);
    public ref string CompoundingCostClass => ref Refs.CompoundingCostClass(_component);
    public ref float CompoundingMultiplier => ref Refs.CompoundingMultiplier(_component);
    public ref bool ConsistentCostForTiled => ref Refs.ConsistentCostForTiled(_component);
    public ref ComponentCostClass CostBreakdownClass => ref Refs.CostBreakdownClass(_component);
    public ref bool DebuffAddedThisFrame => ref Refs.DebuffAddedThisFrame(_component);
    public ref AnimationCurve DebuffProbability => ref Refs.DebuffProbability(_component);
    public ref string FactionKey => ref Refs.FactionKey(_component);
    public ref bool FirstInstanceFree => ref Refs.FirstInstanceFree(_component);
    public ref string FlavorText => ref Refs.FlavorText(_component);
    public ref Material[] HullDamageMaterials => ref Refs.HullDamageMaterials(_component);
    public ref Sides InteriorOverhang => ref Refs.InteriorOverhang(_component);
    public ref bool IsTiled => ref Refs.IsTiled(_component);
    public ref Sprite LargeImage => ref Refs.LargeImage(_component);
    public ref string LongDescription => ref Refs.LongDescription(_component);
    public ref float Mass => ref Refs.Mass(_component);
    public ref ulong ModId => ref Refs.ModId(_component);
    public ref List<ComponentDebuff> NormalDebuffTable => ref Refs.NormalDebuffTable(_component);
    public ref int PointCost => ref Refs.PointCost(_component);
    public ref float RareDebuffChance => ref Refs.RareDebuffChance(_component);
    public ref string RareDebuffSubtype => ref Refs.RareDebuffSubtype(_component);
    public ref List<ComponentDebuff> RareDebuffTable => ref Refs.RareDebuffTable(_component);
    public ref bool RemoveSocketCap => ref Refs.RemoveSocketCap(_component);
    public ref HullComponent.RotateAxis RotateToFit => ref Refs.RotateToFit(_component);
    public ref string SaveKey => ref Refs.SaveKey(_component);
    public ref string ShortDescription => ref Refs.ShortDescription(_component);
    public ref string ShortUIName => ref Refs.ShortUIName(_component);
    public ref Sprite SmallImage => ref Refs.SmallImage(_component);
    public ref IHullSocket Socket => ref Refs.Socket(_component);
    public ref StatValue StatCrewWorkSpeed => ref Refs.StatCrewWorkSpeed(_component);
    public ref StatValue StatDamageThreshold => ref Refs.StatDamageThreshold(_component);
    public ref StatValue StatMaxHP => ref Refs.StatMaxHP(_component);
    public ref StatValue StatRareDebuffChance => ref Refs.StatRareDebuffChance(_component);
    public ref bool StatsInitialized => ref Refs.StatsInitialized(_component);
    public ref int TiledSizeMultiplier => ref Refs.TiledSizeMultiplier(_component);
    public ref HullSocketType Type => ref Refs.Type(_component);
    public ref ComponentDebuff[] UniqueDebuffs => ref Refs.UniqueDebuffs(_component);
    public IEnumerable ActiveDebuffs => Refs.ActiveDebuffs(_component);
    public static ComponentDebuff GetInstanceDebuff(object instance) => Refs.InstanceDebuff(instance);
    public static int GetDebuffInstanceId(object instance) => Refs.DebuffInstanceId(instance);
}
