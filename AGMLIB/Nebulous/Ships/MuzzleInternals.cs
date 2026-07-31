using HarmonyLib;
using Munitions;
using Ships;

public static partial class NativeInternalsExtensions
{
    public static MuzzleInternals Internals(this Muzzle muzzle) => new(muzzle);
}

public readonly struct MuzzleInternals
{
    private static class Refs
    {
        internal static readonly Func<Muzzle, IMagazine> AmmoSource =
            AccessTools.MethodDelegate<Func<Muzzle, IMagazine>>(
                AccessTools.PropertyGetter(typeof(Muzzle), "_ammoSource")
                ?? throw new MissingMemberException(typeof(Muzzle).FullName, "_ammoSource"));

        internal static readonly AccessTools.FieldRef<Muzzle, IMuzzleWeapon> Weapon =
            AccessTools.FieldRefAccess<Muzzle, IMuzzleWeapon>("_weapon");

        internal static readonly AccessTools.FieldRef<Muzzle, int> MuzzleIndex =
            AccessTools.FieldRefAccess<Muzzle, int>("_muzzleIndex");
    }

    private readonly Muzzle _muzzle;

    internal MuzzleInternals(Muzzle muzzle)
    {
        _muzzle = muzzle ?? throw new ArgumentNullException(nameof(muzzle));
    }

    public IMagazine AmmoSource => Refs.AmmoSource(_muzzle);

    public IMuzzleWeapon Weapon => Refs.Weapon(_muzzle);

    public int MuzzleIndex => Refs.MuzzleIndex(_muzzle);
}
