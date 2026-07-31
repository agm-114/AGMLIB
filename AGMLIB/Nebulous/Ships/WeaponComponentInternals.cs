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

        internal static readonly AccessTools.FieldRef<
            WeaponComponent,
            WeaponComponent.IWeaponComponentRPC> RpcProvider =
            AccessTools.FieldRefAccess<
                WeaponComponent,
                WeaponComponent.IWeaponComponentRPC>("_weaponRpcProvider");

        internal static readonly Func<WeaponComponent, bool> OnTarget =
            AccessTools.MethodDelegate<Func<WeaponComponent, bool>>(
                AccessTools.PropertyGetter(typeof(WeaponComponent), "_onTarget")
                ?? throw new MissingMemberException(
                    typeof(WeaponComponent).FullName,
                    "_onTarget"));

        internal static readonly Func<WeaponComponent, bool> TargetBlocked =
            AccessTools.MethodDelegate<Func<WeaponComponent, bool>>(
                AccessTools.PropertyGetter(typeof(WeaponComponent), "_targetBlocked")
                ?? throw new MissingMemberException(
                    typeof(WeaponComponent).FullName,
                    "_targetBlocked"));
    }

    private readonly WeaponComponent _weapon;

    internal WeaponComponentInternals(WeaponComponent weapon)
    {
        _weapon = weapon ?? throw new ArgumentNullException(nameof(weapon));
    }

    public ref Muzzle[] Muzzles => ref Refs.Muzzles(_weapon);

    public ref int CurrentMuzzle => ref Refs.CurrentMuzzle(_weapon);

    public WeaponComponent.IWeaponComponentRPC RpcProvider => Refs.RpcProvider(_weapon);

    public bool OnTarget => Refs.OnTarget(_weapon);

    public bool TargetBlocked => Refs.TargetBlocked(_weapon);
}
