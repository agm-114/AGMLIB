using HarmonyLib;
using Ships;
using UnityEngine;

public static partial class NativeInternalsExtensions
{
    public static HullSocketInternals Internals(this HullSocket socket) => new(socket);
}

public readonly struct HullSocketInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<HullSocket, string> Key =
            AccessTools.FieldRefAccess<HullSocket, string>("_key");

        internal static readonly AccessTools.FieldRef<HullSocket, Vector3Int> Size =
            AccessTools.FieldRefAccess<HullSocket, Vector3Int>("_size");

        internal static readonly AccessTools.FieldRef<HullSocket, HullSocketType> Type =
            AccessTools.FieldRefAccess<HullSocket, HullSocketType>("_type");

        internal static readonly AccessTools.FieldRef<HullSocket, HullComponent> Component =
            AccessTools.FieldRefAccess<HullSocket, HullComponent>("_component");

        internal static readonly AccessTools.FieldRef<HullSocket, BaseHull> Hull =
            AccessTools.FieldRefAccess<HullSocket, BaseHull>("_hull");
    }

    private readonly HullSocket _socket;

    internal HullSocketInternals(HullSocket socket)
    {
        _socket = socket ?? throw new ArgumentNullException(nameof(socket));
    }

    public ref string Key => ref Refs.Key(_socket);

    public ref Vector3Int Size => ref Refs.Size(_socket);

    public ref HullSocketType Type => ref Refs.Type(_socket);

    public ref HullComponent Component => ref Refs.Component(_socket);

    public ref BaseHull Hull => ref Refs.Hull(_socket);
}
