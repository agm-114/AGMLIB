using System.Runtime.Serialization.Json;
using System.Text;

namespace Lib.Diagnostics;

// Unity's runtime serializer can silently omit lists of mod-defined snapshot types.
internal static class SalvoTraceJson
{
    public static string Write<T>(T value)
    {
        using MemoryStream stream = new();
        new DataContractJsonSerializer(typeof(T)).WriteObject(stream, value);
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    public static T Read<T>(string json)
    {
        using MemoryStream stream = new(Encoding.UTF8.GetBytes(json));
        return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(stream);
    }
}
