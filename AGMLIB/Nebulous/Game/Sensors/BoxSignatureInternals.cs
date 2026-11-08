using Game.Sensors;

public static partial class NativeInternalsExtensions
{
    public static BoxSignatureInternals Internals(this BoxSignature target) => new(target);
}

public readonly struct BoxSignatureInternals(BoxSignature target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<BoxSignature, BoxCollider> Collider =
            AccessTools.FieldRefAccess<BoxSignature, BoxCollider>("_collider");
        internal static readonly AccessTools.FieldRef<BoxSignature, Bounds> SigSize =
            AccessTools.FieldRefAccess<BoxSignature, Bounds>("_sigSize");
    }

    public ref BoxCollider Collider => ref Refs.Collider(target);
    public ref Bounds SigSize => ref Refs.SigSize(target);
}