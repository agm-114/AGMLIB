---
name: neb-testing
description: Build and deploy AGMLIB, launch Nebulous Fleet Command, reproduce fleet or missile editor bugs, and inspect Unity logs. Use for iterative Nebulous mod debugging, editor UI reproduction, AGMLIB Harmony diagnostics, testing modded content, or locating Player.log output.
---

# Nebulous Testing

Use this workflow to build, launch, reproduce, and collect evidence from Nebulous without rediscovering local paths.

## Build and deploy

Run from the repository root:

```powershell
dotnet build AGMLIB\AGMLIB.csproj --no-restore -v:minimal
```

The project deploys directly to:

```text
C:\Program Files (x86)\Steam\steamapps\common\Nebulous\Mods\AGMLIB\Debug\net481\AGMLIB.dll
```

If deployment reports a mapped-section or locked-file error, Nebulous is running. Close it before rebuilding. Preserve CRLF in modified C# files.

Nebulous loads mod assemblies only during startup. After any DLL rebuild, fully close and restart the game before testing; returning to the main menu or starting another match does not load the new DLL.

## Launch

Prefer launching the existing executable directly with Unity's documented standalone-player display arguments:

```powershell
Start-Process 'C:\Program Files (x86)\Steam\steamapps\common\Nebulous\Nebulous.exe' -ArgumentList '-screen-width 1920 -screen-height 1080 -screen-fullscreen 0'
```

This makes a 1920x1080 windowed client the standard test environment. After launch, capture the game window and verify its actual dimensions; Unity, DPI scaling, or the desktop work area can change the logical capture size. For example, a 1920x1080 request appears near 1280x720 logical pixels at 150% Windows scaling. If the arguments are ignored, resize the window to a 1920x1080 client area when the display supports it. Continue to derive every click from normalized screenshot coordinates rather than relying on fixed 1080p pixel positions.

List apps after launch, then select the window titled `Nebulous`. Do not target the BepInEx console window for game navigation.

If window capture fails, stop automated input rather than clicking blind. Record the exact capture error and ask for a short manual navigation handoff.

## Reproduce editor issues

1. Open the Fleet Editor.
2. Create or select a fleet for the requested faction.
3. Enter the relevant ship, missile, or spacecraft designer.
4. Navigate to the exact component/socket described by the bug.
5. Perform the smallest interaction that triggers the issue.
6. Read the focused diagnostic lines from `Player.log`.
7. Capture the component save key, socket index/type, missile designation, rule branch, native result, and patched result.

## Enter Testing Range

1. Open `Fleet Editor` from the main menu.
2. Load or create the fleet needed for the test.
3. Use the Fleet Editor's testing-range action to launch the current fleet.
4. Complete deployment if prompted, then select the relevant ship or object before reproducing the issue.

## Enter local skirmish

1. Open `Skirmish` > `New Skirmish` from the main menu.
2. Select the fleet, opponent, map, and match settings needed for the test.
3. Start the match.
4. Select and place every deployment group.
5. Click the separate `Deploy` button to begin the simulation.
6. Select the relevant ship or object before reproducing the issue.

Use local skirmish when testing the offline single-player path. It is distinct from Testing Range and from a locally hosted multiplayer server, so compare those modes when investigating lifecycle or authority differences.

### Fast skirmish startup automation

After verifying the standardized main-menu layout, consolidate navigation into two Computer Use calls instead of capturing every intermediate screen:

1. Batch `Skirmish` > `New Skirmish` > the local-player fleet field, using short waits between transitions.
2. From the fleet selector, choose the requested fleet, confirm it, wait for the lobby to report that all mods loaded, and press `Start`.

For the verified 1920x1080 windowed layout, useful normalized references are `Skirmish (0.190, 0.532)`, `New Skirmish (0.430, 0.532)`, the fleet field `(0.412, 0.199)`, fleet confirmation `(0.370, 0.828)`, and lobby start `(0.870, 0.949)`. Fleet rows are content-dependent: locate the requested fleet visually and normalize its row position from that screenshot rather than storing a fleet-specific row coordinate. Refresh after each batch and stop if the expected next screen is absent.

### Fast deployment automation

After observing the deployment screen, place every visible deployment group and press `Deploy` in one Computer Use call. Click a group card, click a valid point inside the green deployment region, and repeat for each remaining card before clicking `Deploy`. Use different nearby map points so groups do not overlap.

Represent click targets as normalized `(x / width, y / height)` positions from a verified deployment screenshot, then convert them using the current capture dimensions. For the currently verified two-group layout on The Pillars, the normalized references are:

- First two group cards: approximately `(0.278, 0.858)` and `(0.425, 0.858)`.
- Valid nearby placement points: approximately `(0.417, 0.548)` and `(0.447, 0.602)`.
- `Deploy`: approximately `(0.745, 0.762)` after all cards show check marks.

Use a helper such as `point = (nx, ny) => ({ x: Math.round(width * nx), y: Math.round(height * ny) })`, where `width` and `height` come from the latest Nebulous screenshot. These are layout references, not blind universal targets: verify that the expected cards, deployment region, and button are present once before batching. If Computer Use reports user input, discard the remaining queued assumptions, refresh the Nebulous window, and continue from the visible check marks rather than placing completed groups again.

## Logs

The active Unity log is:

```text
%USERPROFILE%\AppData\LocalLow\Eridanus Industries\Nebulous\Player.log
```

`Nebulous\BepInEx\LogOutput.log` may be stale; use `Player.log` unless its timestamp proves otherwise.

Use the bundled script for focused output:

```powershell
powershell -ExecutionPolicy Bypass -File .agents\skills\neb-testing\scripts\read-neb-log.ps1 -Pattern '<diagnostic-prefix>' -Tail 500
```

Before launching, record the log length or last-write time. Nebulous recreates or truncates `Player.log` on startup, so do not rely only on the previous byte offset.

## Iteration loop

1. Add narrow, uniquely prefixed diagnostics.
2. Build and confirm `0 Error(s)`.
3. Launch and confirm the log contains `Finished Loading Mod 'AGMLIB'. Result: Loaded`.
4. Reproduce once.
5. Extract only the relevant prefixed lines plus nearby exceptions.
6. Explain the observed rule path before changing behavior.
7. Apply the smallest fix, remove or reduce noisy diagnostics, rebuild, and repeat the same reproduction.
