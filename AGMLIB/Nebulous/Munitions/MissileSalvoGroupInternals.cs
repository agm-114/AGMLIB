public static partial class NativeInternalsExtensions
{
    public static MissileSalvoGroupInternals Internals(this MissileSalvoGroup salvo) => new(salvo);
}

public readonly struct MissileSalvoGroupInternals(MissileSalvoGroup salvo)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<MissileSalvoGroup, List<ActiveMissileSalvo>> AllSalvos =
            AccessTools.FieldRefAccess<MissileSalvoGroup, List<ActiveMissileSalvo>>("_allSalvos");
    }

    public IReadOnlyList<ActiveMissileSalvo> AllSalvos => Refs.AllSalvos(salvo);
}
