using Munitions.InstancedDamagers;

public static partial class NativeInternalsExtensions
{
    public static BaseThrowawayDamagerInternals Internals(this BaseThrowawayDamager target) => new(target);
}

public readonly struct BaseThrowawayDamagerInternals(BaseThrowawayDamager target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<BaseThrowawayDamager, IDamageCharacteristic> Damage =
            AccessTools.FieldRefAccess<BaseThrowawayDamager, IDamageCharacteristic>("_damage");
    }

    public ref IDamageCharacteristic Damage => ref Refs.Damage(target);
}