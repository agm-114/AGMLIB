using Game;
using HarmonyLib;

public static partial class NativeInternalsExtensions
{
    public static NetworkedShortDurationEffectInternals Internals(
        this NetworkedShortDurationEffect effect) => new(effect);
}

public readonly struct NetworkedShortDurationEffectInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<NetworkedShortDurationEffect, float> Duration =
            AccessTools.FieldRefAccess<NetworkedShortDurationEffect, float>("_duration");
    }

    private readonly NetworkedShortDurationEffect _effect;

    internal NetworkedShortDurationEffectInternals(NetworkedShortDurationEffect effect)
    {
        _effect = effect ?? throw new ArgumentNullException(nameof(effect));
    }

    public float Duration => Refs.Duration(_effect);
}
