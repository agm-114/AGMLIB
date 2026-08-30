using HarmonyLib;
using Ships;

public static partial class NativeInternalsExtensions
{
    public static CraftCarrierControllerInternals Internals(this CraftCarrierController carrier) => new(carrier);
}

public readonly struct CraftCarrierControllerInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<CraftCarrierController, float> MinGreenDeckSpeed =
            AccessTools.FieldRefAccess<CraftCarrierController, float>("_minGreenDeckSpeed");

        internal static readonly AccessTools.FieldRef<CraftCarrierController, float> MaxGreenDeckSpeed =
            AccessTools.FieldRefAccess<CraftCarrierController, float>("_maxGreenDeckSpeed");
    }

    private readonly CraftCarrierController _carrier;

    internal CraftCarrierControllerInternals(CraftCarrierController carrier)
    {
        _carrier = carrier ?? throw new ArgumentNullException(nameof(carrier));
    }

    public ref float MinGreenDeckSpeed => ref Refs.MinGreenDeckSpeed(_carrier);

    public ref float MaxGreenDeckSpeed => ref Refs.MaxGreenDeckSpeed(_carrier);
}
