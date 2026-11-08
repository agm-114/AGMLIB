using Ships;

public static partial class NativeInternalsExtensions
{
    public static StatValueInternals Internals(this StatValue target) => new(target);
}

public readonly struct StatValueInternals(StatValue target)
{
    private static class Refs
    {
        internal static readonly Action<StatValue> Recalculate =
            AccessTools.MethodDelegate<Action<StatValue>>(
                AccessTools.Method(typeof(StatValue), "Recalculate", Type.EmptyTypes)
                ?? throw new MissingMethodException(typeof(StatValue).FullName, "Recalculate"));
        internal static readonly AccessTools.FieldRef<StatValue, float> BaseValue =
            AccessTools.FieldRefAccess<StatValue, float>("_baseValue");
        internal static readonly AccessTools.FieldRef<StatValue, Dictionary<IModifierSource, StatModifier>> Modifiers =
            AccessTools.FieldRefAccess<StatValue, Dictionary<IModifierSource, StatModifier>>("_modifiers");
        internal static readonly AccessTools.FieldRef<StatValue, float> ValueCached =
            AccessTools.FieldRefAccess<StatValue, float>("_valueCached");
    }

    public void Recalculate() => Refs.Recalculate(target);
    public ref float BaseValue => ref Refs.BaseValue(target);
    public ref Dictionary<IModifierSource, StatModifier> Modifiers => ref Refs.Modifiers(target);
    public ref float ValueCached => ref Refs.ValueCached(target);
}