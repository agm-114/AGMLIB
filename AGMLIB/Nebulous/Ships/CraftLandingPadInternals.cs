public static partial class NativeInternalsExtensions
{
    public static CraftLandingPadInternals Internals(this CraftLandingPad pad) => new(pad);
}

public readonly struct CraftLandingPadInternals(CraftLandingPad pad)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<CraftLandingPad, HullPart> Part =
            AccessTools.FieldRefAccess<CraftLandingPad, HullPart>("_part");
        internal static readonly AccessTools.FieldRef<CraftLandingPad, Animator> Animations =
            AccessTools.FieldRefAccess<CraftLandingPad, Animator>("_animations");
        internal static readonly AccessTools.FieldRef<CraftLandingPad, string> OpenTag = AccessTools.FieldRefAccess<CraftLandingPad, string>("_openTagName");
        internal static readonly AccessTools.FieldRef<CraftLandingPad, string> ClosedTag = AccessTools.FieldRefAccess<CraftLandingPad, string>("_closedTagName");
        internal static readonly AccessTools.FieldRef<CraftLandingPad, string> OpenControl = AccessTools.FieldRefAccess<CraftLandingPad, string>("_openControlVariableName");
        internal static readonly AccessTools.FieldRef<CraftLandingPad, bool> Pipeline = AccessTools.FieldRefAccess<CraftLandingPad, bool>("_allowLandingPipelining");
    }

    public ref HullPart Part => ref Refs.Part(pad);
    public ref Animator Animations => ref Refs.Animations(pad);
    public string OpenControl => Refs.OpenControl(pad);
    internal void BindElevator(CraftLandingPad root)
    {
        Animations = Refs.Animations(root);
        Refs.OpenTag(pad) = Refs.OpenTag(root);
        Refs.ClosedTag(pad) = Refs.ClosedTag(root);
        Refs.OpenControl(pad) = Refs.OpenControl(root);
        // Finish the shared descent before granting the next landing clearance.
        Refs.Pipeline(pad) = false;
    }
}
