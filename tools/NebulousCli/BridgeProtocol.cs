using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace NebulousCli;

internal static class BridgeProtocol
{
    internal const int Version = 1;
    internal const int MaxTimeoutSeconds = 120;
    internal const int MaxTextLength = 1000;
    internal const int MaxRequestBytes = 16 * 1024;
    internal const string RequestExtension = ".request";
    internal const string ReplyExtension = ".reply";
    internal const string TemporaryExtension = ".tmp";

    internal static string DirectoryPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NebulousCli");

    internal static string EndpointPath(int pid) => Path.Combine(DirectoryPath, $"{pid}.session");
    internal static string RequestPrefix(int pid, Guid session) => $"{pid}-{session:N}-";
    internal static long UtcMilliseconds => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    internal static T Read<T>(Stream stream) => (T?)new DataContractJsonSerializer(typeof(T)).ReadObject(stream)
        ?? throw new SerializationException("The bridge message is empty.");

    internal static void Write<T>(Stream stream, T value) =>
        new DataContractJsonSerializer(typeof(T)).WriteObject(stream, value);

    internal static void Publish<T>(string path, T value)
    {
        var temporary = path + TemporaryExtension;
        try
        {
            using (var stream = File.Create(temporary))
                Write(stream, value);

            File.Move(temporary, path);
        }
        finally
        {
            TryDelete(temporary);
        }
    }

    internal static void TryDelete(string path)
    {
        try { File.Delete(path); }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}

[DataContract]
internal class BridgeRequest
{
    [DataMember(Name = "version", IsRequired = true)] public int Version { get; set; } = BridgeProtocol.Version;
    [DataMember(Name = "expires", IsRequired = true)] public long Expires { get; set; }
    [DataMember(Name = "command", IsRequired = true)] public string Command { get; set; } = "";
    [DataMember(Name = "snapshot", EmitDefaultValue = false)] public string? Snapshot { get; set; }
    [DataMember(Name = "id", EmitDefaultValue = false)] public uint Id { get; set; }
    [DataMember(Name = "value", EmitDefaultValue = false)] public string? Value { get; set; }
}

[DataContract]
internal class BridgeReply
{
    [DataMember(Name = "error", EmitDefaultValue = false)] public string? Error { get; set; }
    [DataMember(Name = "message", EmitDefaultValue = false)] public string? Message { get; set; }
    [DataMember(Name = "status", EmitDefaultValue = false)] public GameStatus? Status { get; set; }
    [DataMember(Name = "snapshot", EmitDefaultValue = false)] public string? Snapshot { get; set; }
    [DataMember(Name = "controls", EmitDefaultValue = false)] public ControlInfo[]? Controls { get; set; }
    [DataMember(Name = "ships", EmitDefaultValue = false)] public ShipInfo[]? Ships { get; set; }
}

[DataContract]
internal class GameStatus
{
    [DataMember(Name = "pid")] public int Pid { get; set; }
    [DataMember(Name = "gameVersion")] public string GameVersion { get; set; } = "";
    [DataMember(Name = "scene")] public string Scene { get; set; } = "";
    [DataMember(Name = "menu", EmitDefaultValue = false)] public string? Menu { get; set; }
    [DataMember(Name = "matchState", EmitDefaultValue = false)] public string? MatchState { get; set; }
    [DataMember(Name = "elapsedSeconds", EmitDefaultValue = false)] public float? ElapsedSeconds { get; set; }
    [DataMember(Name = "paused", EmitDefaultValue = false)] public bool? Paused { get; set; }
    [DataMember(Name = "speed", EmitDefaultValue = false)] public string? Speed { get; set; }
    [DataMember(Name = "timeScale")] public float TimeScale { get; set; }
}

[DataContract]
internal class ControlInfo
{
    [DataMember(Name = "id")] public uint Id { get; set; }
    [DataMember(Name = "type")] public string Type { get; set; } = "";
    [DataMember(Name = "path")] public string Path { get; set; } = "";
    [DataMember(Name = "label")] public string Label { get; set; } = "";
    [DataMember(Name = "value", EmitDefaultValue = false)] public string? Value { get; set; }
    [DataMember(Name = "checked", EmitDefaultValue = false)] public bool? Checked { get; set; }
    [DataMember(Name = "interactable")] public bool Interactable { get; set; }
}

[DataContract]
internal class ShipInfo
{
    [DataMember(Name = "id")] public uint Id { get; set; }
    [DataMember(Name = "name")] public string Name { get; set; } = "";
    [DataMember(Name = "throttle")] public string Throttle { get; set; } = "";
    [DataMember(Name = "selected")] public bool Selected { get; set; }
}
