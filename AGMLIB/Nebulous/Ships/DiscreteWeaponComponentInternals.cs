using Game.Reports;
using HarmonyLib;
using Ships;

public static partial class NativeInternalsExtensions
{
    public static DiscreteWeaponComponentInternals Internals(this DiscreteWeaponComponent weapon) => new(weapon);
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
        internal static readonly AccessTools.FieldRef<DiscreteWeaponComponent, StatValue> StatRecycleTime =
            AccessTools.FieldRefAccess<DiscreteWeaponComponent, StatValue>("_statRecycleTime");
        internal static readonly Action<DiscreteWeaponComponent> StartReload =
            AccessTools.MethodDelegate<Action<DiscreteWeaponComponent>>(
                AccessTools.Method(typeof(DiscreteWeaponComponent), "StartReload")
                ?? throw new MissingMethodException(typeof(DiscreteWeaponComponent).FullName, "StartReload"));
        internal static readonly AccessTools.FieldRef<DiscreteWeaponComponent, Dictionary<string, DiscreteWeaponReport>> AmmoDefensiveReports =
            AccessTools.FieldRefAccess<DiscreteWeaponComponent, Dictionary<string, DiscreteWeaponReport>>("_ammoDefensiveReports");
        internal static readonly AccessTools.FieldRef<DiscreteWeaponComponent, Dictionary<string, DiscreteWeaponReport>> AmmoOffensiveReports =
            AccessTools.FieldRefAccess<DiscreteWeaponComponent, Dictionary<string, DiscreteWeaponReport>>("_ammoOffensiveReports");
        internal static readonly AccessTools.FieldRef<DiscreteWeaponComponent, float> CurrentFuzeTime =
            AccessTools.FieldRefAccess<DiscreteWeaponComponent, float>("_currentFuzeTime");
        internal static readonly AccessTools.FieldRef<DiscreteWeaponComponent, DiscreteWeaponReport> DefensiveReport =
            AccessTools.FieldRefAccess<DiscreteWeaponComponent, DiscreteWeaponReport>("_defensiveReport");
        internal static readonly AccessTools.FieldRef<DiscreteWeaponComponent, DiscreteWeaponReport> OffensiveReport =
            AccessTools.FieldRefAccess<DiscreteWeaponComponent, DiscreteWeaponReport>("_offensiveReport");
        internal static readonly AccessTools.FieldRef<DiscreteWeaponComponent, bool> RandomlyDeviateReloadTime =
            AccessTools.FieldRefAccess<DiscreteWeaponComponent, bool>("_randomlyDeviateReloadTime");
        internal static readonly AccessTools.FieldRef<DiscreteWeaponComponent, string> RecycleStatSubtype =
            AccessTools.FieldRefAccess<DiscreteWeaponComponent, string>("_recycleStatSubtype");
        internal static readonly AccessTools.FieldRef<DiscreteWeaponComponent, float> ReloadAccum =
            AccessTools.FieldRefAccess<DiscreteWeaponComponent, float>("_reloadAccum");
        internal static readonly AccessTools.FieldRef<DiscreteWeaponComponent, string> ReloadStatSubtype =
            AccessTools.FieldRefAccess<DiscreteWeaponComponent, string>("_reloadStatSubtype");
        internal static readonly AccessTools.FieldRef<DiscreteWeaponComponent, float> ReloadTime =
            AccessTools.FieldRefAccess<DiscreteWeaponComponent, float>("_reloadTime");
        internal static readonly AccessTools.FieldRef<DiscreteWeaponComponent, bool> ResetMuzzleIndexOnReload =
            AccessTools.FieldRefAccess<DiscreteWeaponComponent, bool>("_resetMuzzleIndexOnReload");
        internal static readonly AccessTools.FieldRef<DiscreteWeaponComponent, StatValue> StatReloadTime =
            AccessTools.FieldRefAccess<DiscreteWeaponComponent, StatValue>("_statReloadTime");
        internal static readonly AccessTools.FieldRef<DiscreteWeaponComponent, Coroutine[]> TemporaryEffectCoroutines =
            AccessTools.FieldRefAccess<DiscreteWeaponComponent, Coroutine[]>("_temporaryEffectCoroutines");
        internal static readonly AccessTools.FieldRef<DiscreteWeaponComponent, DiscreteWeaponComponent.TemporaryFiringModifier[]> TemporaryFiringEffects =
            AccessTools.FieldRefAccess<DiscreteWeaponComponent, DiscreteWeaponComponent.TemporaryFiringModifier[]>("_temporaryFiringEffects");
        internal static readonly AccessTools.FieldRef<DiscreteWeaponComponent, float> TimeBetweenMuzzles =
            AccessTools.FieldRefAccess<DiscreteWeaponComponent, float>("_timeBetweenMuzzles");
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
    public float RecycleTime => Refs.StatRecycleTime(_weapon).Value;

    public void StartReload() => Refs.StartReload(_weapon);
    public ref Dictionary<string, DiscreteWeaponReport> AmmoDefensiveReports => ref Refs.AmmoDefensiveReports(_weapon);
    public ref Dictionary<string, DiscreteWeaponReport> AmmoOffensiveReports => ref Refs.AmmoOffensiveReports(_weapon);
    public ref float CurrentFuzeTime => ref Refs.CurrentFuzeTime(_weapon);
    public ref DiscreteWeaponReport DefensiveReport => ref Refs.DefensiveReport(_weapon);
    public ref int MagazineSizeRef => ref Refs.MagazineSize(_weapon);
    public ref DiscreteWeaponReport OffensiveReport => ref Refs.OffensiveReport(_weapon);
    public ref bool RandomlyDeviateMuzzleTimeRef => ref Refs.RandomlyDeviateMuzzleTime(_weapon);
    public ref bool RandomlyDeviateReloadTime => ref Refs.RandomlyDeviateReloadTime(_weapon);
    public ref string RecycleStatSubtype => ref Refs.RecycleStatSubtype(_weapon);
    public ref float ReloadAccum => ref Refs.ReloadAccum(_weapon);
    public ref string ReloadStatSubtype => ref Refs.ReloadStatSubtype(_weapon);
    public ref float ReloadTime => ref Refs.ReloadTime(_weapon);
    public ref bool ResetMuzzleIndexOnReload => ref Refs.ResetMuzzleIndexOnReload(_weapon);
    public ref StatValue StatRecycleTime => ref Refs.StatRecycleTime(_weapon);
    public ref StatValue StatReloadTime => ref Refs.StatReloadTime(_weapon);
    public ref Coroutine[] TemporaryEffectCoroutines => ref Refs.TemporaryEffectCoroutines(_weapon);
    public ref DiscreteWeaponComponent.TemporaryFiringModifier[] TemporaryFiringEffects => ref Refs.TemporaryFiringEffects(_weapon);
    public ref float TimeBetweenMuzzles => ref Refs.TimeBetweenMuzzles(_weapon);
}