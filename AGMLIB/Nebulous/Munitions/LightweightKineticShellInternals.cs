using Munitions;

public static partial class NativeInternalsExtensions
{
    public static LightweightKineticShellInternals Internals(this LightweightKineticShell target) => new(target);
}

public readonly struct LightweightKineticShellInternals(LightweightKineticShell target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<LightweightKineticShell, StandbyVisualEffect> TracerEffect =
            AccessTools.FieldRefAccess<LightweightKineticShell, StandbyVisualEffect>("_tracerEffect");
        internal static readonly AccessTools.FieldRef<LightweightKineticShell, AudioClip> Whine =
            AccessTools.FieldRefAccess<LightweightKineticShell, AudioClip>("_whine");
    }

    public ref StandbyVisualEffect TracerEffect => ref Refs.TracerEffect(target);
    public ref AudioClip Whine => ref Refs.Whine(target);
}