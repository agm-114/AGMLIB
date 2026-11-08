using Ships;

public static partial class NativeInternalsExtensions
{
    public static BaseCellLauncherComponentInternals Internals(this BaseCellLauncherComponent target) => new(target);
}

public readonly struct BaseCellLauncherComponentInternals(BaseCellLauncherComponent target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<BaseCellLauncherComponent, float> CellAccum =
            AccessTools.FieldRefAccess<BaseCellLauncherComponent, float>("_cellAccum");
        internal static readonly AccessTools.FieldRef<BaseCellLauncherComponent, ICellMissileMagazine> Missiles =
            AccessTools.FieldRefAccess<BaseCellLauncherComponent, ICellMissileMagazine>("_missiles");
        internal static readonly AccessTools.FieldRef<BaseCellLauncherComponent, float> TimeBetweenCells =
            AccessTools.FieldRefAccess<BaseCellLauncherComponent, float>("_timeBetweenCells");
        internal static readonly AccessTools.FieldRef<BaseCellLauncherComponent, bool> WaitingForCell =
            AccessTools.FieldRefAccess<BaseCellLauncherComponent, bool>("_waitingForCell");
    }

    public ref float CellAccum => ref Refs.CellAccum(target);
    public ref ICellMissileMagazine Missiles => ref Refs.Missiles(target);
    public ref float TimeBetweenCells => ref Refs.TimeBetweenCells(target);
    public ref bool WaitingForCell => ref Refs.WaitingForCell(target);
}