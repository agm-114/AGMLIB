using Ships;

public static partial class NativeInternalsExtensions
{
    public static HullComponentCostCalculatorInternals Internals(this HullComponentCostCalculator target) => new(target);
}

public readonly struct HullComponentCostCalculatorInternals(HullComponentCostCalculator target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<HullComponentCostCalculator, IHullSocket> Socket =
            AccessTools.FieldRefAccess<HullComponentCostCalculator, IHullSocket>("_socket");
    }

    public IHullSocket Socket => Refs.Socket(target);
}