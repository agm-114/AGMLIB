using Game.Orders.Tasks;

public static partial class NativeInternalsExtensions
{
    public static KeepFormationTaskInternals Internals(this KeepFormationTask target) => new(target);
}

public readonly struct KeepFormationTaskInternals(KeepFormationTask target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<KeepFormationTask, ShipController> Guide =
            AccessTools.FieldRefAccess<KeepFormationTask, ShipController>("_guide");
        internal static readonly AccessTools.FieldRef<KeepFormationTask, Vector3> RelativePosition =
            AccessTools.FieldRefAccess<KeepFormationTask, Vector3>("_relativePosition");
    }

    public ref ShipController Guide => ref Refs.Guide(target);
    public ref Vector3 RelativePosition => ref Refs.RelativePosition(target);
}