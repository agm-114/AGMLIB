using Game.Sensors;

public static partial class NativeInternalsExtensions
{
    public static NetworkedSensorTrackInternals Internals(this NetworkedSensorTrack target) => new(target);
}

public readonly struct NetworkedSensorTrackInternals(NetworkedSensorTrack target)
{
    private static class Refs
    {
        internal static readonly Func<object, NetworkedSensorTrack> HandleParent =
            NativePrivateFields.Getter<NetworkedSensorTrack>(typeof(NetworkedSensorTrack), "Handle", "_parent");
        internal static readonly AccessTools.FieldRef<NetworkedSensorTrack, List<ITrack>> Contributors =
            AccessTools.FieldRefAccess<NetworkedSensorTrack, List<ITrack>>("_contributors");
    }

    public ref List<ITrack> Contributors => ref Refs.Contributors(target);
    public static NetworkedSensorTrack GetHandleParent(object handle) => Refs.HandleParent(handle);
}
