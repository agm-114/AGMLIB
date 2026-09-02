using Game.UI;
using HarmonyLib;

public static partial class NativeInternalsExtensions
{
    public static ShipStatusIconGroupInternals Internals(this ShipStatusIconGroup iconGroup) =>
        new(iconGroup);
}

public readonly struct ShipStatusIconGroupInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<ShipStatusIconGroup, QuantityStatusIcon>
            PowerQuantityIcon = AccessTools.FieldRefAccess<ShipStatusIconGroup, QuantityStatusIcon>(
                "_powerQuantityIcon"
            );
    }

    private readonly ShipStatusIconGroup _iconGroup;

    internal ShipStatusIconGroupInternals(ShipStatusIconGroup iconGroup)
    {
        _iconGroup = iconGroup ?? throw new ArgumentNullException(nameof(iconGroup));
    }

    public ref QuantityStatusIcon PowerQuantityIcon => ref Refs.PowerQuantityIcon(_iconGroup);
}
