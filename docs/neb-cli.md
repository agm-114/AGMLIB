# NEBULOUS CLI

`AGMLIB.sln` includes two projects under **Nebulous CLI**. `Nebulous.Cli`
builds `neb-cli.exe`, a standalone .NET 8 console app. `Nebulous.GameBridge`
builds **Nebulous Game Bridge**, a separate local NEBULOUS mod with its own
manifest. The bridge uses the game's assemblies and can run without AGMLIB.

## Build and enable

Build from the repository root with NEBULOUS closed.

```powershell
dotnet build tools/NebulousCli/Nebulous.GameBridge -p:DeployToGame=true
dotnet build tools/NebulousCli/Nebulous.Cli
$neb = Resolve-Path ./tools/NebulousCli/Nebulous.Cli/bin/Debug/net8.0/neb-cli.exe
```

The bridge deploys to `Mods/NebulousGameBridge` in the default Steam
installation. For a different Steam library, pass `-p:NebulousGameRoot=<path>`.
A build without `DeployToGame=true` puts the bridge DLL and manifest together
under `artifacts/NebulousGameBridge/<Configuration>/net481` for manual installation.

Start the game, enable **Nebulous Game Bridge** in the mod manager, then restart
NEBULOUS. Rebuild the CLI whenever needed. Restart the game after replacing or
disabling the bridge.

## Control the game

```powershell
& $neb status
$ui = & $neb inspect | ConvertFrom-Json
$ui.controls | Format-Table id, type, label, interactable, path
$editor = $ui.controls | Where-Object { $_.label -eq 'Fleet Editor' -and $_.interactable }
& $neb click $ui.snapshot $editor.id
```

`inspect` lists on-screen Unity buttons, toggles and text fields. `click` sends a
left click to a button or toggle. `set-text SNAPSHOT ID TEXT` edits a text field.
Use a fresh `inspect` after each UI action. Controls can also become stale when
the game or another CLI process changes the screen or takes a new snapshot.
Buttons covered by another UI element are unavailable for clicks.

## Match controls

```powershell
& $neb pause
& $neb speed FastForward
& $neb resume
$fleet = & $neb ships | ConvertFrom-Json
$ship = $fleet.ships[0]
& $neb select-ship $ship.id
& $neb throttle $ship.id Full
& $neb ships
```

Time commands use the host's native controls and respect mission speed limits.
In solo skirmish and Testing Range, `pause` opens the game's pause menu and
`resume` closes it. `ships` lists the local player's fleet. `throttle` sends a
normal player order, so call `ships` again to read the applied throttle.

Use `neb-cli quit` from the main menu to close the game.

`neb-cli --help` lists all commands. Results go to standard output as JSON.
Errors go to standard error as JSON with a nonzero exit code. Put `--pid PID`
before the command if multiple copies of the game are running. `--timeout SECONDS`
controls how long the CLI waits for a reply. If it times out during an action,
inspect the current game state before retrying.

The CLI and bridge communicate through the current user's
`%LOCALAPPDATA%/NebulousCli` directory. Commands execute on Unity's main thread.
