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
        internal static readonly Action<
            HullComponent,
            ComponentDebuff,
            MunitionHitInfo,
            bool
        > AddDebuffToComponent = AccessTools.MethodDelegate<
            Action<HullComponent, ComponentDebuff, MunitionHitInfo, bool>
        >(
            AccessTools.Method(
                typeof(HullComponent),
                "AddDebuffToComponent",
                [typeof(ComponentDebuff), typeof(MunitionHitInfo), typeof(bool)]
            )
                ?? throw new MissingMethodException(
                    typeof(HullComponent).FullName,
                    "AddDebuffToComponent"
                )
        );
    }

    private readonly HullComponent _component;

    internal HullComponentInternals(HullComponent component)
    {
        _component = component ?? throw new ArgumentNullException(nameof(component));
    }

    public void AddDebuff(
        ComponentDebuff debuff,
        MunitionHitInfo hitInfo,
        bool checkValid = false
    )
    {
        Refs.AddDebuffToComponent(_component, debuff, hitInfo, checkValid);
    }
}
