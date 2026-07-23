using HarmonyLib;
using Ships;

public static partial class NativeInternalsExtensions
{
    public static WeaponComponentInternals Internals(this WeaponComponent weapon) => new(weapon);
}

public readonly struct WeaponComponentInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<WeaponComponent, Muzzle[]> Muzzles =
            AccessTools.FieldRefAccess<WeaponComponent, Muzzle[]>("_muzzles");

        internal static readonly AccessTools.FieldRef<WeaponComponent, int> CurrentMuzzle =
            AccessTools.FieldRefAccess<WeaponComponent, int>("_currentMuzzle");
    }

    private readonly WeaponComponent _weapon;

    internal WeaponComponentInternals(WeaponComponent weapon)
    {
        _weapon = weapon ?? throw new ArgumentNullException(nameof(weapon));
    }

    public ref Muzzle[] Muzzles => ref Refs.Muzzles(_weapon);

    public ref int CurrentMuzzle => ref Refs.CurrentMuzzle(_weapon);
}
