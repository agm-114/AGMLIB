using System.IO;
using System.Runtime.Serialization;
using UnityEngine;

namespace NebulousCli;

internal class BridgeDriver : MonoBehaviour
{
    private GameCommands _commands = null!;
    private string? _endpoint;
    private string _requestPrefix = "";
    private float _nextPoll;
    private float _nextCleanup;

    internal void StartSession()
    {
        using var process = System.Diagnostics.Process.GetCurrentProcess();
        var session = Guid.NewGuid();
        _commands = new(process.Id);
        _requestPrefix = BridgeProtocol.RequestPrefix(process.Id, session);

        Directory.CreateDirectory(BridgeProtocol.DirectoryPath);
        _endpoint = BridgeProtocol.EndpointPath(process.Id);
        File.WriteAllText(_endpoint, session.ToString("N"));
        Debug.Log($"[Nebulous Game Bridge] Ready (PID {process.Id})");
    }

    private void Update()
    {
        var now = Time.realtimeSinceStartup;
        if (_endpoint == null || now < _nextPoll)
            return;

        _nextPoll = now + 0.1f;
        try
        {
            if (now >= _nextCleanup)
            {
                _nextCleanup = now + 60f;
                DeleteAbandonedMessages();
            }

            var requestPath = Directory.EnumerateFiles(BridgeProtocol.DirectoryPath,
                    _requestPrefix + "*" + BridgeProtocol.RequestExtension)
                .FirstOrDefault(IsRequest);
            if (requestPath != null)
                HandleRequest(requestPath);
        }
        catch (FileNotFoundException) { } // The client may have cancelled the request.
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Debug.LogWarning($"[Nebulous Game Bridge] {ex.Message}");
            _nextPoll = now + 1f;
        }
    }

    private bool IsRequest(string path) =>
        Guid.TryParseExact(Path.GetFileNameWithoutExtension(path).Substring(_requestPrefix.Length), "N", out _);

    private void HandleRequest(string path)
    {
        BridgeRequest? request = null;
        BridgeReply reply;
        try
        {
            using var stream = File.OpenRead(path);
            if (stream.Length > BridgeProtocol.MaxRequestBytes)
                throw new SerializationException("The bridge request is too large.");

            request = BridgeProtocol.Read<BridgeRequest>(stream);
            reply = new();
        }
        catch (SerializationException ex)
        {
            reply = new() { Error = ex.Message };
        }

        // Consume the request before running an action so a failed reply cannot replay it.
        File.Delete(path);
        if (request != null)
            reply = Dispatch(request);

        BridgeProtocol.Publish(Path.ChangeExtension(path, BridgeProtocol.ReplyExtension), reply);
    }

    private BridgeReply Dispatch(BridgeRequest request)
    {
        try
        {
            var remaining = request.Expires - BridgeProtocol.UtcMilliseconds;
            if (remaining <= 0 || remaining > BridgeProtocol.MaxTimeoutSeconds * 1000L)
                return new() { Error = "The bridge request expired or has an invalid deadline." };
            if (request.Version != BridgeProtocol.Version)
                return new() { Error = "The CLI and bridge protocol versions differ. Rebuild both projects." };

            return _commands.Execute(request);
        }
        catch (InvalidOperationException ex)
        {
            return new() { Error = ex.Message };
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return new() { Error = "The game command failed. See Player.log for details." };
        }
    }

    private static void DeleteAbandonedMessages()
    {
        var cutoff = DateTime.UtcNow.AddMinutes(-5);
        foreach (var path in Directory.EnumerateFiles(BridgeProtocol.DirectoryPath))
        {
            var parts = Path.GetFileName(path).Split('-', '.');
            if (parts.Length is 4 or 5 && int.TryParse(parts[0], out _) &&
                Guid.TryParseExact(parts[1], "N", out _) && Guid.TryParseExact(parts[2], "N", out _) &&
                parts[3] is "request" or "reply" && (parts.Length == 4 || parts[4] == "tmp") &&
                File.GetLastWriteTimeUtc(path) < cutoff)
            {
                BridgeProtocol.TryDelete(path);
            }
        }
    }

    private void OnDestroy()
    {
        if (_endpoint != null)
            BridgeProtocol.TryDelete(_endpoint);
    }
}
