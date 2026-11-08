using Game.Sensors;

public static partial class NativeInternalsExtensions
{
    public static BaseSignatureInternals Internals(this BaseSignature target) => new(target);
}

public readonly struct BaseSignatureInternals(BaseSignature target)
{
    private static class Refs
    {
        internal static readonly Action<BaseSignature> FireSignatureChangedEvent =
            AccessTools.MethodDelegate<Action<BaseSignature>>(
                AccessTools.Method(typeof(BaseSignature), "FireSignatureChangedEvent", Type.EmptyTypes)
                ?? throw new MissingMethodException(typeof(BaseSignature).FullName, "FireSignatureChangedEvent"));
    }

    public void FireSignatureChangedEvent() => Refs.FireSignatureChangedEvent(target);
}