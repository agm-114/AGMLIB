using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using NebulousCli;

if (args is ["--help"] or ["-h"])
{
    Console.WriteLine("""
        Usage: neb-cli [--pid PID] [--timeout SECONDS] [COMMAND]

          status                       Show the active scene, menu and match clock (default).
          inspect                      List on-screen Unity buttons, toggles and text fields.
          click SNAPSHOT ID             Activate a button or toggle from inspect.
          set-text SNAPSHOT ID TEXT     Set a text field from inspect.
          ships                        List ships in the local player's fleet.
          select-ship ID                Select a ship from ships.
          throttle ID LEVEL             OneThird, TwoThirds, Full, Flank or FlightQuarters.
          pause | resume                Pause or return to Normal speed.
          speed LEVEL                   Paused, Slowmo, Normal, FastForward or FastFastForward.
          quit                         Close NEBULOUS from its main menu.

        Commands return JSON. Use --pid when several game instances are running.
        --timeout defaults to 10 wall-clock seconds and accepts 1–120.
        """);
    return 0;
}

using var cancellation = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellation.Cancel();
};

try
{
    var (pid, timeout, request) = Parse(args);
    var game = FindGame(pid);
    cancellation.CancelAfter(TimeSpan.FromSeconds(timeout));
    request.Expires = BridgeProtocol.UtcMilliseconds + timeout * 1000L;
    return Output(await Send(game.Pid, game.StartedUtc, request, cancellation.Token), 1);
}
catch (ArgumentException ex)
{
    return Output(new() { Error = $"{ex.Message} Run neb-cli --help for usage." }, 2);
}
catch (OperationCanceledException)
{
    return Output(new() { Error = "No reply from Nebulous. Check game state before retrying an action." }, 1);
}
catch (Exception ex) when (ex is InvalidOperationException or IOException or UnauthorizedAccessException or SerializationException)
{
    return Output(new() { Error = ex.Message }, 1);
}

static (int? Pid, int Timeout, BridgeRequest Request) Parse(string[] args)
{
    int? pid = null;
    var timeout = 10;
    var index = 0;
    while (index < args.Length && args[index].StartsWith("--", StringComparison.Ordinal))
    {
        var option = args[index++];
        if (index == args.Length)
            throw new ArgumentException($"{option} requires a value.");

        var value = args[index++];
        switch (option)
        {
            case "--pid" when int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var parsedPid) && parsedPid > 0:
                pid = parsedPid;
                break;
            case "--timeout" when int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var seconds) &&
                                  seconds is >= 1 and <= BridgeProtocol.MaxTimeoutSeconds:
                timeout = seconds;
                break;
            default:
                throw new ArgumentException($"Invalid option or value for {option}.");
        }
    }

    var request = args[index..] switch
    {
        [] => new BridgeRequest { Command = "status" },
        [("status" or "inspect" or "ships" or "pause" or "resume" or "quit") and var command] => new() { Command = command },
        ["click", var snapshot, var id] => new() { Command = "click", Snapshot = ParseSnapshot(snapshot), Id = ParseId(id) },
        ["set-text", var snapshot, var id, var value] when value.Length <= BridgeProtocol.MaxTextLength =>
            new() { Command = "set-text", Snapshot = ParseSnapshot(snapshot), Id = ParseId(id), Value = value },
        ["select-ship", var id] => new() { Command = "select-ship", Id = ParseId(id) },
        ["throttle", var id, var value] => new() { Command = "throttle", Id = ParseId(id), Value = value },
        ["speed", var value] => new() { Command = "speed", Value = value },
        _ => throw new ArgumentException("Unknown command or invalid command arguments.")
    };
    return (pid, timeout, request);
}

static uint ParseId(string value) => uint.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var id) && id > 0
    ? id : throw new ArgumentException("ID must be a positive integer.");

static string ParseSnapshot(string value) => Guid.TryParseExact(value, "N", out var snapshot)
    ? snapshot.ToString("N") : throw new ArgumentException("Use the snapshot token returned by inspect.");

static (int Pid, DateTime StartedUtc) FindGame(int? pid)
{
    var games = Process.GetProcessesByName("Nebulous");
    try
    {
        var game = (pid, games) switch
        {
            (int selected, _) => games.FirstOrDefault(game => game.Id == selected)
                ?? throw new InvalidOperationException($"There is no Nebulous process with PID {selected}."),
            (null, []) => throw new InvalidOperationException("Nebulous is not running."),
            (null, [var onlyGame]) => onlyGame,
            _ => throw new InvalidOperationException($"Multiple Nebulous processes are running. Use --pid with one of {string.Join(", ", games.Select(game => game.Id))}.")
        };
        return (game.Id, game.StartTime.ToUniversalTime());
    }
    finally
    {
        Array.ForEach(games, game => game.Dispose());
    }
}

static async Task<BridgeReply> Send(int pid, DateTime startedUtc, BridgeRequest request, CancellationToken cancellation)
{
    var endpoint = BridgeProtocol.EndpointPath(pid);
    Guid session;
    try
    {
        if (File.GetLastWriteTimeUtc(endpoint) < startedUtc)
            throw new FileNotFoundException();
        if (!Guid.TryParseExact(File.ReadAllText(endpoint), "N", out session))
            throw new InvalidOperationException("The bridge endpoint is invalid. Wait for startup to finish or restart the game.");
    }
    catch (Exception ex) when (ex is FileNotFoundException or DirectoryNotFoundException)
    {
        throw new InvalidOperationException($"No bridge for PID {pid}. Enable Nebulous Game Bridge in the game and restart it.");
    }

    var messagePath = Path.Combine(BridgeProtocol.DirectoryPath, BridgeProtocol.RequestPrefix(pid, session) + Guid.NewGuid().ToString("N"));
    var requestPath = messagePath + BridgeProtocol.RequestExtension;
    var replyPath = messagePath + BridgeProtocol.ReplyExtension;
    try
    {
        cancellation.ThrowIfCancellationRequested();
        BridgeProtocol.Publish(requestPath, request);
        while (true)
        {
            try
            {
                using var stream = File.Open(replyPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                return BridgeProtocol.Read<BridgeReply>(stream);
            }
            catch (IOException)
            {
                await Task.Delay(50, cancellation);
            }
        }
    }
    finally
    {
        BridgeProtocol.TryDelete(requestPath);
        BridgeProtocol.TryDelete(replyPath);
    }
}

static int Output(BridgeReply reply, int errorCode)
{
    using var stream = reply.Error == null ? Console.OpenStandardOutput() : Console.OpenStandardError();
    BridgeProtocol.Write(stream, reply);
    stream.WriteByte((byte)'\n');
    return reply.Error == null ? 0 : errorCode;
}
