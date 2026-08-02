using HarmonyLib;
using Ships;

public static partial class NativeInternalsExtensions
{
    public static ComponentHullPaintLODSharedInternals Internals(
        this ComponentHullPaintLODShared paint
    ) => new(paint);
}

public readonly struct ComponentHullPaintLODSharedInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<
            ComponentHullPaintLODShared,
            bool
        > MultipleMaterials = AccessTools.FieldRefAccess<
            ComponentHullPaintLODShared,
            bool
        >("_multipleMaterials");
    }

    private readonly ComponentHullPaintLODShared _paint;

    internal ComponentHullPaintLODSharedInternals(
        ComponentHullPaintLODShared paint
    )
    {
        _paint = paint ?? throw new ArgumentNullException(nameof(paint));
    }

    public ref bool MultipleMaterials => ref Refs.MultipleMaterials(_paint);
}
