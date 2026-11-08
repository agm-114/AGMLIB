using Munitions.ModularMissiles.Runtime.Seekers;

public static partial class NativeInternalsExtensions
{
    public static RuntimeBeamSeekerInternals Internals(this RuntimeBeamSeeker target) => new(target);
}

public readonly struct RuntimeBeamSeekerInternals(RuntimeBeamSeeker target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<RuntimeBeamSeeker, bool> ValidationReliable =
            AccessTools.FieldRefAccess<RuntimeBeamSeeker, bool>("_validationReliable");
        internal static readonly AccessTools.FieldRef<RuntimeBeamSeeker, ReceivedJamming> JammingSources =
            AccessTools.FieldRefAccess<RuntimeBeamSeeker, ReceivedJamming>("_jammingSources");
    }

    public ref bool ValidationReliable => ref Refs.ValidationReliable(target);
    public ref ReceivedJamming JammingSources => ref Refs.JammingSources(target);
}