using HarmonyLib;
using System;
using UnityEngine;
using Object = UnityEngine.Object;


namespace AGMLIB.Server
{
    internal class Logging
    {
    }
    public class TimeStampedLogWrapper
    {
        public DateTime Timestamp { get; }
        public object Message { get; }

        public TimeStampedLogWrapper(object message)
        {
            Timestamp = DateTime.Now;
            Message = message;
        }

        public override string ToString()
        {
            return $"{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Message}";
        }
    }

    [HarmonyPatch(typeof(UnityEngine.Debug), nameof(UnityEngine.Debug.LogError), new Type[] { typeof(object) })]

    class DebugLogError
    {
        public static bool Prefix(ref object message)
        {
            //Debug.LogError("Prefix");
            message = new TimeStampedLogWrapper(message);
            return true;
        }



    }

    [HarmonyPatch(typeof(UnityEngine.Debug), nameof(UnityEngine.Debug.LogError), new Type[] { typeof(object), typeof(Object) })]
    class DebugLogErrorComplex
    {


        [HarmonyPrefix]
        public static bool Prefix(ref object message)
        {
            //Debug.LogError("Prefix");
            
            return DebugLogError.Prefix(ref message);
        }

    }

}
