namespace AGMLIB.Server
{
    internal class Logging
    {
    }
    public class TimeWrapper
    {
        public DateTime Timestamp { get; }
        public object Message { get; }

        public TimeWrapper(object message)
        {
            Timestamp = DateTime.UtcNow;
            Message = message;
        }

        public override string ToString()
        {
            return $"{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Message}";
        }

        public static bool Wrap(ref object message)
        {
            //Debug.LogError("Prefix");
            message = new TimeWrapper(message);
            return true;
        }
    }

    //[HarmonyPatch(typeof(UnityEngine.Debug), nameof(UnityEngine.Debug.LogError), new Type[] { typeof(object) })]
    class DebugLogError
    {
        public static bool Prefix(ref object message) => TimeWrapper.Wrap(ref message);
    }

    //[HarmonyPatch(typeof(UnityEngine.Debug), nameof(UnityEngine.Debug.LogError), new Type[] { typeof(object), typeof(Object) })]
    class DebugLogErrorComplex
    {
        public static bool Prefix(ref object message) => DebugLogError.Prefix(ref message);
    }

    //[HarmonyPatch(typeof(UnityEngine.Debug), nameof(UnityEngine.Debug.Log), new Type[] { typeof(object) })]
    class DebugLog
    {
        public static bool Prefix(ref object message) => TimeWrapper.Wrap(ref message);
    }

    //[HarmonyPatch(typeof(UnityEngine.Debug), nameof(UnityEngine.Debug.Log), new Type[] { typeof(object), typeof(Object) })]
    class DebugLogComplex
    {
        public static bool Prefix(ref object message) => DebugLogError.Prefix(ref message);
    }
    //[HarmonyPatch(typeof(UnityEngine.Debug), nameof(UnityEngine.Debug.LogWarning), new Type[] { typeof(object) })]
    class DebugLogWarning
    {
        public static bool Prefix(ref object message) => TimeWrapper.Wrap(ref message);
    }

    //[HarmonyPatch(typeof(UnityEngine.Debug), nameof(UnityEngine.Debug.LogWarning), new Type[] { typeof(object), typeof(Object) })]
    class DebugLogWarningComplex
    {
        public static bool Prefix(ref object message) => DebugLogError.Prefix(ref message);
    }

}
