using HarmonyLib;
using Ships;

public static partial class NativeInternalsExtensions
{
    public static ResourcePoolInternals Internals(this ResourcePool pool) => new(pool);
}

public readonly struct ResourcePoolInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<
            ResourcePool,
            List<IResourceSystemConnected>
        > Providers = AccessTools.FieldRefAccess<
            ResourcePool,
            List<IResourceSystemConnected>
        >("_providers");

        internal static readonly AccessTools.FieldRef<
            ResourcePool,
            List<IResourceSystemConnected>
        > Consumers = AccessTools.FieldRefAccess<
            ResourcePool,
            List<IResourceSystemConnected>
        >("_consumers");

        internal static readonly AccessTools.FieldRef<ResourcePool, EditorResourceSummary> Summary =
            AccessTools.FieldRefAccess<ResourcePool, EditorResourceSummary>("_summary");
    }

    private readonly ResourcePool _pool;

    internal ResourcePoolInternals(ResourcePool pool)
    {
        _pool = pool ?? throw new ArgumentNullException(nameof(pool));
    }

    public ref List<IResourceSystemConnected> Providers => ref Refs.Providers(_pool);

    public ref List<IResourceSystemConnected> Consumers => ref Refs.Consumers(_pool);

    public ref EditorResourceSummary Summary => ref Refs.Summary(_pool);
}
