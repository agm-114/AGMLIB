using Game.UI;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

public static partial class NativeInternalsExtensions
{
    public static QuantityStatusIconInternals Internals(this QuantityStatusIcon icon) => new(icon);
}

public readonly struct QuantityStatusIconInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<QuantityStatusIcon, Sprite> NormalIcon =
            AccessTools.FieldRefAccess<QuantityStatusIcon, Sprite>("_normalIcon");

        internal static readonly AccessTools.FieldRef<QuantityStatusIcon, Image> IconImage =
            AccessTools.FieldRefAccess<QuantityStatusIcon, Image>("_iconImage");
    }

    private readonly QuantityStatusIcon _icon;

    internal QuantityStatusIconInternals(QuantityStatusIcon icon)
    {
        _icon = icon ?? throw new ArgumentNullException(nameof(icon));
    }

    public ref Sprite NormalIcon => ref Refs.NormalIcon(_icon);

    public ref Image IconImage => ref Refs.IconImage(_icon);
}
