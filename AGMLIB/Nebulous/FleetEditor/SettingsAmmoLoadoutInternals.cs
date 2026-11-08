using FleetEditor;

public static partial class NativeInternalsExtensions
{
    public static SettingsAmmoLoadoutInternals<T> Internals<T>(this SettingsAmmoLoadout<T> panel)
        where T : class, IMagazineProvider => new(panel);
}

public readonly struct SettingsAmmoLoadoutInternals<T>(SettingsAmmoLoadout<T> panel)
    where T : class, IMagazineProvider
{
    private static class Refs
    {
        internal static readonly Func<SettingsAmmoLoadout<T>, T> Provider =
            AccessTools.MethodDelegate<Func<SettingsAmmoLoadout<T>, T>>(AccessTools.PropertyGetter(typeof(SettingsAmmoLoadout<T>), "_provider"));
        internal static readonly Func<SettingsAmmoLoadout<T>, BaseHull> Hull =
            AccessTools.MethodDelegate<Func<SettingsAmmoLoadout<T>, BaseHull>>(AccessTools.PropertyGetter(typeof(SettingsAmmoLoadout<T>), "_hull"));
    }

    public T Provider => Refs.Provider(panel);
    public BaseHull Hull => Refs.Hull(panel);
}