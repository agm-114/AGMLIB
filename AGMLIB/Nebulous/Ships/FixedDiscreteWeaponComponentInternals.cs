using Ships;

public static partial class NativeInternalsExtensions
{
    public static FixedDiscreteWeaponComponentInternals Internals(this FixedDiscreteWeaponComponent target) => new(target);
}

public readonly struct FixedDiscreteWeaponComponentInternals(FixedDiscreteWeaponComponent target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<FixedDiscreteWeaponComponent, Transform> AimCheckFrom =
            AccessTools.FieldRefAccess<FixedDiscreteWeaponComponent, Transform>("_aimCheckFrom");
    }

    public ref Transform AimCheckFrom => ref Refs.AimCheckFrom(target);
}