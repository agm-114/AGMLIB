using Game.Docking;
using Game.UI;
using UnityEngine.UI;

public static partial class NativeInternalsExtensions
{
    public static ActionMenuInternals Internals(this ActionMenu menu) => new(menu);
}

public readonly struct ActionMenuInternals(ActionMenu menu)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<ActionMenu, IDockingPoint> DockingTarget =
            AccessTools.FieldRefAccess<ActionMenu, IDockingPoint>("_dockingTarget");
        internal static readonly AccessTools.FieldRef<ActionMenu, Button> DockButton =
            AccessTools.FieldRefAccess<ActionMenu, Button>("_dockButton");
        internal static readonly AccessTools.FieldRef<ActionMenu, Toggle> MovementButton =
            AccessTools.FieldRefAccess<ActionMenu, Toggle>("_movementButton");
    }

    public ref IDockingPoint DockingTarget => ref Refs.DockingTarget(menu);
    public Button DockButton => Refs.DockButton(menu);
    public Toggle MovementButton => Refs.MovementButton(menu);
}
