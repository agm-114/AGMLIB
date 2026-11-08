using Utility;

public static partial class NativeInternalsExtensions
{
    public static DynamicVisibleParticlesInternals Internals(this DynamicVisibleParticles particles) => new(particles);
}

public readonly struct DynamicVisibleParticlesInternals(DynamicVisibleParticles particles)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<DynamicVisibleParticles, VisualEffect> Particles =
            AccessTools.FieldRefAccess<DynamicVisibleParticles, VisualEffect>("_particles");
    }

    public ref VisualEffect Particles => ref Refs.Particles(particles);
}