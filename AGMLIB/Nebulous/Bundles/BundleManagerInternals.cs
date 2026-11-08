using Munitions.ModularMissiles;
using SmallCraft;
using Bundles;
using HarmonyLib;
using Ships;

public static partial class NativeInternalsExtensions
{
    public static BundleManagerInternals Internals(this BundleManager bundleManager) => new(bundleManager);
}

public readonly struct BundleManagerInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<BundleManager, Dictionary<string, List<ComponentDebuff>>> Debuffs =
            AccessTools.FieldRefAccess<BundleManager, Dictionary<string, List<ComponentDebuff>>>("_debuffs");
        internal static readonly AccessTools.FieldRef<BundleManager, List<string>> Tips =
            AccessTools.FieldRefAccess<BundleManager, List<string>>("_tips");
        internal static readonly AccessTools.FieldRef<BundleManager, Dictionary<string, HullComponent>> Components =
            AccessTools.FieldRefAccess<BundleManager, Dictionary<string, HullComponent>>("_components");
        internal static readonly AccessTools.FieldRef<BundleManager, Dictionary<string, BaseHull>> Hulls =
            AccessTools.FieldRefAccess<BundleManager, Dictionary<string, BaseHull>>("_hulls");
        internal static readonly AccessTools.FieldRef<BundleManager, Dictionary<string, ModularMissile>> MissileBodies =
            AccessTools.FieldRefAccess<BundleManager, Dictionary<string, ModularMissile>>("_missileBodies");
        internal static readonly AccessTools.FieldRef<BundleManager, Dictionary<string, IMunition>> MunitionsBySaveKey =
            AccessTools.FieldRefAccess<BundleManager, Dictionary<string, IMunition>>("_munitionsBySaveKey");
        internal static readonly AccessTools.FieldRef<BundleManager, Dictionary<string, Spacecraft>> Spaceframes =
            AccessTools.FieldRefAccess<BundleManager, Dictionary<string, Spacecraft>>("_spaceframes");
    }

    private readonly BundleManager _bundleManager;
    internal BundleManagerInternals(BundleManager bundleManager)
    {
        _bundleManager = bundleManager ?? throw new ArgumentNullException(nameof(bundleManager));
    }

    public ref Dictionary<string, List<ComponentDebuff>> Debuffs => ref Refs.Debuffs(_bundleManager);
    public ref List<string> Tips => ref Refs.Tips(_bundleManager);
    public ref Dictionary<string, HullComponent> Components => ref Refs.Components(_bundleManager);
    public ref Dictionary<string, BaseHull> Hulls => ref Refs.Hulls(_bundleManager);
    public ref Dictionary<string, ModularMissile> MissileBodies => ref Refs.MissileBodies(_bundleManager);
    public ref Dictionary<string, IMunition> MunitionsBySaveKey => ref Refs.MunitionsBySaveKey(_bundleManager);
    public ref Dictionary<string, Spacecraft> Spaceframes => ref Refs.Spaceframes(_bundleManager);
}