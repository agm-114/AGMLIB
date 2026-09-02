using Game.UI;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

public static partial class NativeInternalsExtensions
{
    public static ShipStatusDetailPartInternals Internals(this ShipStatusDetailPart detailPart) =>
        new(detailPart);
}

public readonly struct ShipStatusDetailPartInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<ShipStatusDetailPart, GameObject> IconArea =
            AccessTools.FieldRefAccess<ShipStatusDetailPart, GameObject>("_iconArea");

        internal static readonly AccessTools.FieldRef<ShipStatusDetailPart, Image> FireIcon =
            AccessTools.FieldRefAccess<ShipStatusDetailPart, Image>("_fireIcon");
    }

    private readonly ShipStatusDetailPart _detailPart;

    internal ShipStatusDetailPartInternals(ShipStatusDetailPart detailPart)
    {
        _detailPart = detailPart ?? throw new ArgumentNullException(nameof(detailPart));
    }

    public ref GameObject IconArea => ref Refs.IconArea(_detailPart);

    public ref Image FireIcon => ref Refs.FireIcon(_detailPart);
}
