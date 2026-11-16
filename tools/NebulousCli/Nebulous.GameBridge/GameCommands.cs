using Game;
using Game.Units;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;
using Speed = Game.TimeDilationManager.SpeedSelection;
using TimeLimit = Game.TimeDilationManager.TimeControlLimit;

namespace NebulousCli;

internal class GameCommands(int pid)
{
    private readonly UiControls _ui = new();

    internal BridgeReply Execute(BridgeRequest request) => request.Command switch
    {
        "status" => new() { Status = Status() },
        "inspect" => _ui.Inspect(Status()),
        "click" or "set-text" => _ui.Act(request),
        "ships" => ListShips(),
        "select-ship" or "throttle" => ActOnShip(request),
        "pause" => SetSpeed(Speed.Paused),
        "resume" => SetSpeed(Speed.Normal),
        "speed" => SetSpeed(ParseName<Speed>(request.Value)),
        "quit" => Quit(),
        _ => throw new InvalidOperationException($"Unknown bridge command '{request.Command}'.")
    };

    private GameStatus Status()
    {
        var scene = SceneManager.GetActiveScene();
        var menu = MenuController.SceneInstance(scene)?.GetTopMenu();
        var match = SkirmishGameManager.Instance;
        var gameClock = UnityEngine.Object.FindObjectOfType<TimeDilationManager>();
        return new()
        {
            Pid = pid,
            GameVersion = Application.version,
            Scene = scene.name,
            Menu = menu != null ? menu.MenuName : null,
            MatchState = match != null ? match.CurrentGameState.ToString() : null,
            ElapsedSeconds = match != null && match.IsGameActive ? match.GameTimeElapsedPrecise : null,
            Paused = match != null && match.IsGameActive ? match.GamePaused || match.ActivePause : null,
            Speed = gameClock != null && gameClock.TimeControlsAvailable ? gameClock.SelectedGameSpeed.ToString() : null,
            TimeScale = Time.timeScale
        };
    }

    private BridgeReply SetSpeed(Speed speed)
    {
        var match = RequireRunningMatch();
        var gameClock = UnityEngine.Object.FindObjectOfType<TimeDilationManager>();
        if (gameClock == null || !gameClock.isServer)
            throw new InvalidOperationException("This command requires the match host.");

        if (!gameClock.TimeControlsAvailable)
            return SetSoloPause(match, speed);
        if (match.GamePaused)
            throw new InvalidOperationException("Close the pause menu to use the match time controls.");

        var max = gameClock.Network_controlLimit switch
        {
            TimeLimit.PauseOnly => Speed.Paused,
            TimeLimit.PauseSlowPlay => Speed.Normal,
            TimeLimit.Limited => Speed.FastForward,
            TimeLimit.All => Speed.FastFastForward,
            _ => (Speed)(-1)
        };
        if (speed > max)
            throw new InvalidOperationException("The requested speed is unavailable under the current mission time-control limit.");

        gameClock.SetSpeedSelection(speed);
        return new() { Status = Status() };
    }

    private BridgeReply SetSoloPause(SkirmishGameManager match, Speed speed)
    {
        if (!match.IsSoloGame || speed is not (Speed.Paused or Speed.Normal))
            throw new InvalidOperationException("The match has no speed controls. Solo matches support pause and resume.");

        var pause = speed == Speed.Paused;
        if (match.GamePaused != pause)
            match.TogglePauseMenu();
        if (match.GamePaused != pause)
            throw new InvalidOperationException("The game cannot change its pause state while the current menu is open.");
        if (!pause && match.ActivePause && GameSettings.Instance.ActivePause)
            match.ToggleActivePause();

        return new() { Status = Status() };
    }

    private static BridgeReply Quit()
    {
        if (MenuController.SceneInstance(SceneManager.GetActiveScene())?.GetTopMenu() is not MainMenu menu)
            throw new InvalidOperationException("Return to the main menu before quitting.");

        menu.ButtonExit();
        return new() { Message = "Quit requested." };
    }

    private static BridgeReply ListShips()
    {
        var player = RequireLocalPlayer();
        return new()
        {
            Ships = OwnedShips(player).Select(ship => new ShipInfo
            {
                Id = ship.netId,
                Name = ship.Ship.ShipNameNoPrefix,
                Throttle = ship.Throttle.ToString(),
                Selected = ReferenceEquals(player.Selected, ship)
            }).OrderBy(ship => ship.Id).ToArray()
        };
    }

    private static BridgeReply ActOnShip(BridgeRequest request)
    {
        var player = RequireLocalPlayer();
        var ship = OwnedShips(player).FirstOrDefault(ship => ship.netId == request.Id);
        if (ship == null)
            throw new InvalidOperationException($"Ship {request.Id} is absent from the local fleet. Run ships again.");

        if (request.Command == "select-ship")
        {
            player.Select(ship, doubleClick: false);
            return new() { Message = $"Selected {ship.Ship.ShipNameNoPrefix}." };
        }

        if (!ship.PlayerCanOrder(player))
            throw new InvalidOperationException("The local player cannot give orders to this ship.");

        player.CmdSetShipThrottle(ship, ParseName<MovementSpeed>(request.Value));
        return new() { Message = $"Throttle order sent for {ship.Ship.ShipNameNoPrefix}." };
    }

    private static SkirmishGameManager RequireRunningMatch()
    {
        var match = SkirmishGameManager.Instance;
        if (match == null || !match.IsGameActive || match.InTransition)
            throw new InvalidOperationException("This command requires a running match.");

        return match;
    }

    private static HumanSkirmishPlayer RequireLocalPlayer()
    {
        var player = RequireRunningMatch().SkirmishLocalPlayer;
        if (player == null || player.PlayerFleet == null || player.IsSpectator)
            throw new InvalidOperationException("This command requires a local player fleet.");

        return player;
    }

    private static IEnumerable<ShipController> OwnedShips(HumanSkirmishPlayer player) =>
        player.PlayerFleet.FleetShipControllers.Where(ship => ship != null && ship.netId != 0 && ship.OwnedBy == player);

    private static T ParseName<T>(string? value) where T : struct, Enum =>
        Enum.TryParse<T>(value, true, out var result) && result.ToString().Equals(value, StringComparison.OrdinalIgnoreCase)
            ? result
            : throw new InvalidOperationException($"Use one of {string.Join(", ", Enum.GetNames(typeof(T)))}.");
}
