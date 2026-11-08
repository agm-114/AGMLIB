using Mirror;

public static partial class NativeInternalsExtensions
{
    public static NetworkBehaviourInternals Internals(this NetworkBehaviour target) => new(target);
}

public readonly struct NetworkBehaviourInternals(NetworkBehaviour target)
{
    private static class Refs
    {
        internal static readonly Func<NetworkBehaviour, int> GetComponentIndex =
            AccessTools.MethodDelegate<Func<NetworkBehaviour, int>>(
                AccessTools.PropertyGetter(typeof(NetworkBehaviour), "ComponentIndex")
                ?? throw new MissingMemberException(typeof(NetworkBehaviour).FullName, "ComponentIndex"));
        internal static readonly Action<NetworkBehaviour, int> SetComponentIndex =
            AccessTools.MethodDelegate<Action<NetworkBehaviour, int>>(
                AccessTools.PropertySetter(typeof(NetworkBehaviour), "ComponentIndex")
                ?? throw new MissingMemberException(typeof(NetworkBehaviour).FullName, "ComponentIndex"));
        internal static readonly Func<NetworkBehaviour, NetworkIdentity> GetNetIdentity =
            AccessTools.MethodDelegate<Func<NetworkBehaviour, NetworkIdentity>>(
                AccessTools.PropertyGetter(typeof(NetworkBehaviour), "netIdentity")
                ?? throw new MissingMemberException(typeof(NetworkBehaviour).FullName, "netIdentity"));
        internal static readonly Action<NetworkBehaviour, NetworkIdentity> SetNetIdentity =
            AccessTools.MethodDelegate<Action<NetworkBehaviour, NetworkIdentity>>(
                AccessTools.PropertySetter(typeof(NetworkBehaviour), "netIdentity")
                ?? throw new MissingMemberException(typeof(NetworkBehaviour).FullName, "netIdentity"));
    }

    public int ComponentIndex { get => Refs.GetComponentIndex(target); set => Refs.SetComponentIndex(target, value); }
    public NetworkIdentity NetIdentity { get => Refs.GetNetIdentity(target); set => Refs.SetNetIdentity(target, value); }
}