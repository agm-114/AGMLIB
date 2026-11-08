using Munitions.ModularMissiles.Descriptors.Support;

public static partial class NativeInternalsExtensions
{
    public static JammerSupportDescriptorInternals Internals(this JammerSupportDescriptor target) => new(target);
}

public readonly struct JammerSupportDescriptorInternals(JammerSupportDescriptor target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<JammerSupportDescriptor, SignatureType> SigType =
            AccessTools.FieldRefAccess<JammerSupportDescriptor, SignatureType>("_sigType");
    }

    public ref SignatureType SigType => ref Refs.SigType(target);
}