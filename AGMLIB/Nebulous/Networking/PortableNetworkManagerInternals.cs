using HarmonyLib;
using Networking;

public static partial class NativeInternalsExtensions
{
    public static PortableNetworkManagerInternals Internals(this PortableNetworkManager networkManager) =>
        new(networkManager);
}

public readonly struct PortableNetworkManagerInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<PortableNetworkManager, bool> UseSteamTransportForLobbies =
            AccessTools.FieldRefAccess<PortableNetworkManager, bool>("_useSteamTransportForLobbies");
    }

    private readonly PortableNetworkManager _networkManager;

    internal PortableNetworkManagerInternals(PortableNetworkManager networkManager)
    {
        _networkManager = networkManager ?? throw new ArgumentNullException(nameof(networkManager));
    }

    public ref bool UseSteamTransportForLobbies => ref Refs.UseSteamTransportForLobbies(_networkManager);
}
