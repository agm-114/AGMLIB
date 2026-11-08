using Mirror;

public static partial class NativeInternalsExtensions
{
    public static NetworkIdentityInternals Internals(this NetworkIdentity target) => new(target);
}

public readonly struct NetworkIdentityInternals(NetworkIdentity target)
{
    private static class Refs
    {
        internal static readonly Func<NetworkIdentity, NetworkBehaviour[]> GetNetworkBehaviours =
            AccessTools.MethodDelegate<Func<NetworkIdentity, NetworkBehaviour[]>>(
                AccessTools.PropertyGetter(typeof(NetworkIdentity), "NetworkBehaviours")
                ?? throw new MissingMemberException(typeof(NetworkIdentity).FullName, "NetworkBehaviours"));
        internal static readonly Action<NetworkIdentity, NetworkBehaviour[]> SetNetworkBehaviours =
            AccessTools.MethodDelegate<Action<NetworkIdentity, NetworkBehaviour[]>>(
                AccessTools.PropertySetter(typeof(NetworkIdentity), "NetworkBehaviours")
                ?? throw new MissingMemberException(typeof(NetworkIdentity).FullName, "NetworkBehaviours"));
    }

    public NetworkBehaviour[] NetworkBehaviours { get => Refs.GetNetworkBehaviours(target); set => Refs.SetNetworkBehaviours(target, value); }
}