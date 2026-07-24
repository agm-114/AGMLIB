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
    }

    private readonly BundleManager _bundleManager;

    internal BundleManagerInternals(BundleManager bundleManager)
    {
        _bundleManager = bundleManager ?? throw new ArgumentNullException(nameof(bundleManager));
    }

    public ref Dictionary<string, List<ComponentDebuff>> Debuffs => ref Refs.Debuffs(_bundleManager);

    public ref List<string> Tips => ref Refs.Tips(_bundleManager);
}
