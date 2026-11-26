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
        internal static readonly Func<object, object> GetOrderType =
            NativePrivateFields.Getter<object>(typeof(CraftCarrierController), "TrafficOrder", "Type");
        internal static readonly Action<object, object> SetOrderType =
            NativePrivateFields.Setter<object>(typeof(CraftCarrierController), "TrafficOrder", "Type");
        internal static readonly AccessTools.FieldRef<CraftCarrierController, float> MinGreenDeckSpeed =
            AccessTools.FieldRefAccess<CraftCarrierController, float>("_minGreenDeckSpeed");
        internal static readonly AccessTools.FieldRef<CraftCarrierController, float> MaxGreenDeckSpeed =
            AccessTools.FieldRefAccess<CraftCarrierController, float>("_maxGreenDeckSpeed");
        internal static readonly AccessTools.FieldRef<CraftCarrierController, IEnumerable> TrafficQueue =
            AccessTools.FieldRefAccess<CraftCarrierController, IEnumerable>("_trafficQueue");
        internal static readonly AccessTools.FieldRef<CraftCarrierController, List<ICraftEntryExitPoint>> SharedPads =
            AccessTools.FieldRefAccess<CraftCarrierController, List<ICraftEntryExitPoint>>("_sharedPads");
        internal static readonly System.Reflection.FieldInfo ReservedPad =
            AccessTools.Field(AccessTools.Inner(typeof(CraftCarrierController), "TrafficOrder"), "ReservedPad");
    }

    private readonly CraftCarrierController _carrier;
    internal CraftCarrierControllerInternals(CraftCarrierController carrier)
    {
        _carrier = carrier ?? throw new ArgumentNullException(nameof(carrier));
    }

    public ref float MinGreenDeckSpeed => ref Refs.MinGreenDeckSpeed(_carrier);
    public ref float MaxGreenDeckSpeed => ref Refs.MaxGreenDeckSpeed(_carrier);
    public IEnumerable TrafficQueue => Refs.TrafficQueue(_carrier);
    public List<ICraftEntryExitPoint> SharedPads => Refs.SharedPads(_carrier);
    internal static System.Reflection.FieldInfo ReservedPadField => Refs.ReservedPad;
    public static object GetOrderType(object order) => Refs.GetOrderType(order);
    public static void SetOrderType(object order, object type) => Refs.SetOrderType(order, type);
}
