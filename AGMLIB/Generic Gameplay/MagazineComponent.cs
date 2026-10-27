using Ships.SaveGame;
using Utility.Localization;

/// <summary>Optional clipboard metadata for custom component data containing a magazine load.</summary>
public interface IMagazineClipboardData : IMagazineLoadComponentData
{
    float? ClipboardTotalVolume { get; }
}

/// <summary>Reusable magazine configuration, contents, persistence, and reporting for a hull component.</summary>
[DisallowMultipleComponent]
public class MagazineComponent : MonoBehaviour
{
    [Header("Shared Magazine")]
    [Min(0), SerializeField] private int _availableVolume = 100;
    [SerializeField] private MunitionType[] _storageRestrictions = [];
    [SerializeField] private HullComponent? _parent;

    private MagazineCollection _magazines = null!;
    public HullComponent Parent => _parent ??= GetComponent<HullComponent>();
    public int AvailableVolume => Math.Max(0, _availableVolume) * Parent.Internals().TileMultiplier;
    public IReadOnlyList<MunitionType> StorageRestrictions => _storageRestrictions;
    public MagazineCollection Collection => _magazines;
    public int Capacity => _magazines.Provider.Capacity;
    public int UsedCapacity => Mathf.RoundToInt(_magazines.UsedCapacity);
    public float RemainingCapacity => Capacity - _magazines.UsedCapacity;

    // Use the registered hull component as the provider so magazine identity and deltas stay native.
    public void Initialize<TComponent>(TComponent owner) where TComponent : HullComponent, IMagazineProvider
    {
        if (_magazines != null)
        {
            if (!ReferenceEquals(_magazines.Provider, owner))
                throw new InvalidOperationException("A magazine cannot be rebound to another component.");
            return;
        }
        _parent = owner;
        _magazines = new(owner);
        Parent.OnIsFunctionalChanged += HandleFunctionalChanged;
        Parent.OnDestroyedChanged += HandleFunctionalChanged;
    }

    private void OnDestroy()
    {
        if (_parent == null)
            return;
        _parent.OnIsFunctionalChanged -= HandleFunctionalChanged;
        _parent.OnDestroyedChanged -= HandleFunctionalChanged;
    }
    private void HandleFunctionalChanged(HullPart part) =>
        _magazines.Magazines.Cast<Magazine>().ToList().ForEach(magazine => magazine.ProviderCanProvideChanged());

    public bool RestrictionCheck(IMunition ammo) => AmmoCompatiblity.IsAmmoCompatible(ammo, Parent, out bool compatible)
        ? compatible : _storageRestrictions.Length == 0 || _storageRestrictions.Contains(ammo.Type);

    public BulkMagazineComponent.BulkMagazineData GetSaveData() => new()
    {
        Load = _magazines.SaveMagazineContents(),
        ClipboardTotalVolume = UsedCapacity
    };
    public void LoadSaveData(ComponentSaveData data)
    {
        if (data is not IMagazineLoadComponentData load)
            return;
        float? clipboardVolume = data switch
        {
            IMagazineClipboardData custom => custom.ClipboardTotalVolume,
            BulkMagazineComponent.BulkMagazineData bulk => bulk.ClipboardTotalVolume,
            _ => null
        };
        _magazines.ClearContents();
        _magazines.LoadMagazineContents([.. load.DefaultLoad], _magazines.Provider.AvailableMunitions, GetLoadScaling(clipboardVolume, Capacity));
    }
    internal static float? GetLoadScaling(float? clipboardVolume, int capacity) =>
        clipboardVolume is > 0 && clipboardVolume > capacity ? Math.Max(0, capacity) / clipboardVolume : null;

    public void WriteSaveState(SavedHullComponentStates state) => state.Write(Parent,
        new SavedComponentMagazineState { Mags = _magazines.SaveMagazineContents(), States = _magazines.SaveMagazineState() });
    public void RestoreFromSaveState(SavedHullComponentStates state)
    {
        if (state.Read<SavedComponentMagazineState>(Parent) is { } saved)
            _magazines.RestoreSavedState(saved.Mags, saved.States);
    }

    public int GetPointCost(HullComponentCostCalculator? saveData = null)
    {
        if (saveData == null)
            return _magazines.Magazines.Sum(magazine => magazine.AmmoType.GetPointCost(magazine.Quantity));
        if (saveData.ComponentData is not IMagazineLoadComponentData load)
            return 0;
        SavedComponentMagazineState? state = saveData.SavedState?.Components?.Read<SavedComponentMagazineState>(saveData.SocketKey);
        int cost = 0;
        foreach (Magazine.MagSaveData magazine in state?.Mags ?? load.DefaultLoad)
        {
            IPointCostItem? ammo = saveData.GetScopedCostItem(magazine.MunitionKey) ?? BundleManager.Instance.GetMunition(magazine.MunitionKey);
            uint expended = state?.States.Find(item => item.MagazineKey == magazine.MagazineKey).Expended ?? 0;
            cost = checked(cost + (ammo?.GetPointCost((int)(magazine.Quantity - Math.Min(magazine.Quantity, expended))) ?? 0));
        }
        return cost;
    }
    public void GetPointCostBreakdown(HullComponentCostBreakdownCalculator breakdown)
    {
        foreach (IMagazine magazine in _magazines.Magazines)
        {
            ComponentCostClass category = magazine.AmmoType.Type != MunitionType.Missile ? ComponentCostClass.Ammunition
                : magazine.AmmoType.Role.IsDefensive() ? ComponentCostClass.Defenses : ComponentCostClass.Missiles;
            breakdown.AddCost(category, magazine.AmmoType.GetPointCost(magazine.Quantity));
        }
    }
    public void CollectModDependencies(HashSet<ulong> mods)
    {
        foreach (IMagazine magazine in _magazines.Magazines)
        {
            if (magazine.AmmoType.SourceModId is { } mod)
                mods.Add(mod);
        }
    }
    public void GetDesignWarnings(List<DesignWarning> warnings)
    {
        if (!_magazines.CheckAmmoDesignsValid())
            warnings.Add(ShipDesignWarning.Warning("$UI_FLTED_WARNINGS_INVALIDMISSILEAMMO_TITLE",
                "$UI_FLTED_WARNINGS_INVALIDMISSILEAMMO_BODY".Localize(("compName", Parent.ComponentName), ("socketName", Parent.Socket.Name)), null));
        string[] unused = [.. _magazines.Magazines.Where(magazine => !Parent.Socket.Hull.AmmoTypeInUse(magazine.AmmoType, externalFeed: true))
            .Select(magazine => magazine.AmmoType.MunitionDisplayName)];
        if (unused.Length > 0)
            warnings.Add(ShipDesignWarning.Risk("$UI_FLTED_WARNINGS_UNUSEDAMMO_TITLE", "Magazine in " + Parent.Socket.Name
                + " contains ammunition not used by any weapons: " + string.Join(", ", unused), null));
    }
    public string GetDCTooltipDetails()
    {
        string[] contents = [.. _magazines.Magazines.Where(magazine => magazine.Quantity > 0)
            .Select(magazine => $"\n   {magazine.Quantity}x {magazine.AmmoType.MunitionDisplayName}")];
        return "$UI_SKIRMISH_DC_PART_STOREDAMMO:" + (contents.Length == 0 ? "\n   $UI_SKIRMISH_DC_PART_EMPTY" : string.Concat(contents));
    }
}
