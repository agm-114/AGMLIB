using SmallCraft.Components;

public static partial class NativeInternalsExtensions
{
    public static CraftGunTurretWeaponInternals Internals(this CraftGunTurretWeapon target) => new(target);
}

public readonly struct CraftGunTurretWeaponInternals(CraftGunTurretWeapon target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<CraftGunTurretWeapon, TurretController> TurretControl =
            AccessTools.FieldRefAccess<CraftGunTurretWeapon, TurretController>("_turretControl");
    }

    public ref TurretController TurretControl => ref Refs.TurretControl(target);
}