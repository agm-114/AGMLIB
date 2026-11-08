using Ships;

public static partial class NativeInternalsExtensions
{
    public static ContinuousWeaponComponentInternals Internals(this ContinuousWeaponComponent target) => new(target);
}

public readonly struct ContinuousWeaponComponentInternals(ContinuousWeaponComponent target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<ContinuousWeaponComponent, ContinuousWeaponComponent.CooldownType> CooldownStyle =
            AccessTools.FieldRefAccess<ContinuousWeaponComponent, ContinuousWeaponComponent.CooldownType>("_cooldownStyle");
        internal static readonly AccessTools.FieldRef<ContinuousWeaponComponent, bool> MuzzlesActive =
            AccessTools.FieldRefAccess<ContinuousWeaponComponent, bool>("_muzzlesActive");
        internal static readonly AccessTools.FieldRef<ContinuousWeaponComponent, StatValue> StatBurstDuration =
            AccessTools.FieldRefAccess<ContinuousWeaponComponent, StatValue>("_statBurstDuration");
        internal static readonly AccessTools.FieldRef<ContinuousWeaponComponent, string> StatGroupSubtype =
            AccessTools.FieldRefAccess<ContinuousWeaponComponent, string>("_statGroupSubtype");
    }

    public ref ContinuousWeaponComponent.CooldownType CooldownStyle => ref Refs.CooldownStyle(target);
    public ref bool MuzzlesActive => ref Refs.MuzzlesActive(target);
    public ref StatValue StatBurstDuration => ref Refs.StatBurstDuration(target);
    public ref string StatGroupSubtype => ref Refs.StatGroupSubtype(target);
}