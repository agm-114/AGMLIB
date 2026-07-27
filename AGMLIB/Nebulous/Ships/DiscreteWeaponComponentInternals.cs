using HarmonyLib;
using Ships;

public static partial class NativeInternalsExtensions
{
    public static DiscreteWeaponComponentInternals Internals(
        this DiscreteWeaponComponent weapon) => new(weapon);
}

public readonly struct DiscreteWeaponComponentInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<DiscreteWeaponComponent, bool> WaitingForMuzzle =
            AccessTools.FieldRefAccess<DiscreteWeaponComponent, bool>("_waitingForMuzzle");

        internal static readonly AccessTools.FieldRef<DiscreteWeaponComponent, float> MuzzleAccum =
            AccessTools.FieldRefAccess<DiscreteWeaponComponent, float>("_muzzleAccum");

        internal static readonly AccessTools.FieldRef<DiscreteWeaponComponent, bool> Reloading =
            AccessTools.FieldRefAccess<DiscreteWeaponComponent, bool>("_reloading");

        internal static readonly AccessTools.FieldRef<DiscreteWeaponComponent, int> MagazineFired =
            AccessTools.FieldRefAccess<DiscreteWeaponComponent, int>("_magazineFired");

        internal static readonly AccessTools.FieldRef<DiscreteWeaponComponent, int> MagazineSize =
            AccessTools.FieldRefAccess<DiscreteWeaponComponent, int>("_magazineSize");

        internal static readonly AccessTools.FieldRef<DiscreteWeaponComponent, bool> RandomlyDeviateMuzzleTime =
            AccessTools.FieldRefAccess<DiscreteWeaponComponent, bool>("_randomlyDeviateMuzzleTime");

        internal static readonly Action<DiscreteWeaponComponent> StartReload =
            AccessTools.MethodDelegate<Action<DiscreteWeaponComponent>>(
                AccessTools.Method(typeof(DiscreteWeaponComponent), "StartReload")
                ?? throw new MissingMethodException(
                    typeof(DiscreteWeaponComponent).FullName,
                    "StartReload"));
    }

    private readonly DiscreteWeaponComponent _weapon;

    internal DiscreteWeaponComponentInternals(DiscreteWeaponComponent weapon)
    {
        _weapon = weapon ?? throw new ArgumentNullException(nameof(weapon));
    }

    public ref bool WaitingForMuzzle => ref Refs.WaitingForMuzzle(_weapon);

    public ref float MuzzleAccum => ref Refs.MuzzleAccum(_weapon);

    public ref bool Reloading => ref Refs.Reloading(_weapon);

    public ref int MagazineFired => ref Refs.MagazineFired(_weapon);

    public int MagazineSize => Refs.MagazineSize(_weapon);

    public bool RandomlyDeviateMuzzleTime => Refs.RandomlyDeviateMuzzleTime(_weapon);

    public void StartReload() => Refs.StartReload(_weapon);
}
