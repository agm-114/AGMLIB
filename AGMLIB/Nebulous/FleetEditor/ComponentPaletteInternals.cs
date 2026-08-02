using FleetEditor;
using HarmonyLib;
using Ships;
using UI;

public static partial class NativeInternalsExtensions
{
    public static ComponentPaletteInternals Internals(this ComponentPalette palette) => new(palette);
}

public readonly struct ComponentPaletteInternals
{
    private static class Refs
    {
        internal static readonly Func<ComponentPalette, HullComponent, PaletteItem> CreateItem =
            AccessTools.MethodDelegate<Func<ComponentPalette, HullComponent, PaletteItem>>(
                AccessTools.Method(
                    typeof(ComponentPalette),
                    "CreateItem",
                    [typeof(HullComponent)]
                )
                    ?? throw new MissingMethodException(
                        typeof(ComponentPalette).FullName,
                        "CreateItem"
                    )
            );
    }

    private readonly ComponentPalette _palette;

    internal ComponentPaletteInternals(ComponentPalette palette)
    {
        _palette = palette ?? throw new ArgumentNullException(nameof(palette));
    }

    public PaletteItem CreateItem(HullComponent component) =>
        Refs.CreateItem(_palette, component);
}
