using FleetEditor;

public static partial class NativeInternalsExtensions
{
    public static SettingsMagazineLoadoutInternals Internals(this SettingsMagazineLoadout target) => new(target);
}

public readonly struct SettingsMagazineLoadoutInternals(SettingsMagazineLoadout target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<SettingsMagazineLoadout, Button> AddMagazineButton =
            AccessTools.FieldRefAccess<SettingsMagazineLoadout, Button>("_addMagazineButton");
        internal static readonly AccessTools.FieldRef<SettingsMagazineLoadout, TextMeshProUGUI> CapacityText =
            AccessTools.FieldRefAccess<SettingsMagazineLoadout, TextMeshProUGUI>("_capacityText");
        internal static readonly AccessTools.FieldRef<SettingsMagazineLoadout, TextMeshProUGUI> EmptyLoadText =
            AccessTools.FieldRefAccess<SettingsMagazineLoadout, TextMeshProUGUI>("_emptyLoadText");
        internal static readonly AccessTools.FieldRef<SettingsMagazineLoadout, Image> GaugeImage =
            AccessTools.FieldRefAccess<SettingsMagazineLoadout, Image>("_gaugeImage");
        internal static readonly AccessTools.FieldRef<SettingsMagazineLoadout, List<MagazineAmmoItem>> Magazines =
            AccessTools.FieldRefAccess<SettingsMagazineLoadout, List<MagazineAmmoItem>>("_magazines");
    }

    public ref Button AddMagazineButton => ref Refs.AddMagazineButton(target);
    public ref TextMeshProUGUI CapacityText => ref Refs.CapacityText(target);
    public ref TextMeshProUGUI EmptyLoadText => ref Refs.EmptyLoadText(target);
    public ref Image GaugeImage => ref Refs.GaugeImage(target);
    public ref List<MagazineAmmoItem> Magazines => ref Refs.Magazines(target);
}