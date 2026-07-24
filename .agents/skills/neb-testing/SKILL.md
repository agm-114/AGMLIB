---
name: neb-testing
description: Build and deploy AGMLIB, dump vanilla and modded Nebulous prefabs to YAML, launch Nebulous Fleet Command, reproduce fleet or missile editor bugs, and inspect Unity logs. Use for iterative Nebulous mod debugging, prefab inspection, editor UI reproduction, AGMLIB Harmony diagnostics, testing modded content, or locating Player.log output.
---

# Nebulous Testing

Use this workflow to build, launch, reproduce, and collect evidence from Nebulous without rediscovering local paths.

## Path conventions

Before testing, read `.agents/neb-testing.local.md`. Its resolved per-user paths and any installation overrides take precedence over the reusable defaults below.

Use these standard locations:

- Game root: `C:\Program Files (x86)\Steam\steamapps\common\Nebulous`
- Unity log template: `C:\Users\<user>\AppData\LocalLow\Eridanus Industries\Nebulous\Player.log`

Resolve them in PowerShell with:

```powershell
$gameRoot = 'C:\Program Files (x86)\Steam\steamapps\common\Nebulous'
$playerLog = Join-Path $env:USERPROFILE 'AppData\LocalLow\Eridanus Industries\Nebulous\Player.log'
```

Derive the remaining paths from `$gameRoot` rather than recording machine-specific absolute copies:

- Game executable: `Nebulous.exe`
- AGMLIB deployed DLL: `Mods\AGMLIB\Debug\net481\AGMLIB.dll`
- Fleet saves: `Saves\Fleets`
- BepInEx log: `BepInEx\LogOutput.log`

## Process control authorization

- The user authorizes terminating `Nebulous.exe` without asking again when testing requires an unlocked DLL, a clean game restart, or startup-only environment such as prefab dumping.
- Preserve logs or other evidence needed from the current run before terminating it.

## Refresh the prefab snapshot

Before runtime testing, refresh the complete vanilla and enabled-mod prefab snapshot:

```powershell
powershell -ExecutionPolicy Bypass -File .agents\skills\neb-testing\scripts\dump-neb-prefabs.ps1
```

The command builds AGMLIB, launches Nebulous with dumping enabled, waits for the atomic YAML snapshot, and leaves the game open for testing. Read [references/prefab-dumping.md](references/prefab-dumping.md) before changing dump scope, overriding paths, diagnosing a failed dump, or interpreting the YAML schema.

## Build and deploy

An ordinary build writes only to the repository-local `artifacts\AGMLIB`
directory. It does not rewrite the committed version baseline or source
`ModInfo.xml`, and it does not copy files into the game:

```powershell
dotnet build AGMLIB\AGMLIB.csproj --no-restore -v:minimal
```

The final version component is a change-aware local build revision.
`AGMLIB/Version.props` contains the committed baseline. The ignored
`.build-state/agmlib-version.json` ledger increments that revision when a
build-relevant project input fingerprint changes and reuses it for identical
inputs. Assembly and generated manifest versions always use the same derived
value. Promote an intended revision into `Version.props` deliberately for a
portable release baseline; the ignored ledger is local build history.

Deploy to the local game only with the explicit command:

```powershell
powershell -ExecutionPolicy Bypass -File scripts\Build\Deploy-Agmlib.ps1
```

Use `-Configuration Release` or `-GameRoot <path>` when needed. The script
builds first unless `-SkipBuild` is supplied, refuses to deploy while Nebulous
is running, copies to `Mods\AGMLIB\<Configuration>\net481`, and verifies the
deployed DLL, Harmony dependency, and manifest by SHA-256.

Run the isolation check after changing build logic:

```powershell
powershell -ExecutionPolicy Bypass -File scripts\Build\Test-AgmlibBuildIsolation.ps1
```

An ordinary build is safe while Nebulous is running because it no longer writes
to the game installation. Close Nebulous only before explicit deployment.
Preserve CRLF in modified C# files.

Nebulous loads mod assemblies only during startup. After any DLL rebuild, fully close and restart the game before testing; returning to the main menu or starting another match does not load the new DLL.

## Launch

Prefer launching the existing executable directly with Unity's documented standalone-player display arguments:

```powershell
$gameRoot = Join-Path ${env:ProgramFiles(x86)} 'Steam\steamapps\common\Nebulous'
Start-Process (Join-Path $gameRoot 'Nebulous.exe') -ArgumentList '-screen-width 1920 -screen-height 1080 -screen-fullscreen 0'
```

This makes a 1920x1080 windowed client the standard test environment. After launch, capture the game window and verify its actual dimensions; Unity, DPI scaling, or the desktop work area can change the logical capture size. For example, a 1920x1080 request appears near 1280x720 logical pixels at 150% Windows scaling. If the arguments are ignored, resize the window to a 1920x1080 client area when the display supports it. Continue to derive every click from normalized screenshot coordinates rather than relying on fixed 1080p pixel positions.

List apps after launch, then select the window titled `Nebulous`. Do not target the BepInEx console window for game navigation.

If window capture fails, stop automated input rather than clicking blind. Record the exact capture error and ask for a short manual navigation handoff.

## Prefer keyboard macros

Prefer a known keyboard shortcut or short key macro over an equivalent multi-step UI flow. Keyboard input is faster, avoids coordinate drift, and is generally more reliable once the correct game context and binding are verified.

- Activate the `Nebulous` window and verify the required context before sending keys.
- Prefer a single documented shortcut, then a short ordered macro, then UI navigation.
- Use UI navigation when no binding exists, the shortcut is remapped or context-sensitive, or the resulting state cannot be verified.
- When a reliable shortcut or macro is discovered, add its exact keys, required context, and observable result to this skill in the same change.
- Record user-specific remaps in `.agents/neb-testing.local.md`, not in this shared skill.

## Reproduce editor issues

1. Open the Fleet Editor.
2. Create or select a fleet for the requested faction.
3. Enter the relevant ship, missile, or spacecraft designer.
4. Navigate to the exact component/socket described by the bug.
5. Perform the smallest interaction that triggers the issue.
6. Read the focused diagnostic lines from `Player.log`.
7. Capture the component save key, socket index/type, missile designation, rule branch, native result, and patched result.

## Enter Testing Range

Prefer Testing Range for rapid fleet, weapon, missile, mine, and fighter iteration. It starts opposing units much closer together and avoids the skirmish lobby and deployment workflow.

1. Open `Fleet Editor` from the main menu.
2. Load or create the fleet needed for the test.
3. Press `Esc` after the fleet is open, then choose the testing-range action from the pause/menu screen.
4. Complete deployment if prompted, then select the relevant ship or object before reproducing the issue.

### Fire a weapon in Testing Range

To issue a position-targeted weapon order:

1. Select the firing ship.
2. Right-click open space at the intended target area to open the order menu.
3. Choose `WEP`.
4. Choose the `POS` tab. The weapons panel also exposes `TRK` and `VIZ`; use the mode supported by the weapon under test.
5. Select the exact weapon-mount row. For `POS`, this opens the target-position tool and displays the current range and estimated travel time.
6. Left-click the desired target position to confirm and fire.
7. Verify that the weapon-order icon appears, then allow enough time for launch, flight, end-of-path behavior, and submunition release before reading telemetry.

Do not start by clicking the weapon cards in the lower ship-status panel; the reliable firing path is the right-click order menu. When Computer Use is controlling the game, have it press `Esc` itself because a physical `Esc` press stops the automation session.

### Fire and count manual decoys

Use the order menu for a reliable manual decoy launch:

1. Select the ship.
2. Press `Shift+Z` while no order widget is open.
3. Verify that the decoy-order icon appears and that the focused diagnostic reports the accepted weapon groups.

If the shortcut is remapped or cannot be verified, right-click open space, away from the ship cards and other UI, and click `DCY`.

The `DECOY DSBL/LEAST/BEST/AUTO` control in the lower status panel configures automatic point-defense decoy policy; it is not the manual launch button.

Use live ammunition counts to verify how many shots actually fired:

1. Before firing, right-click open space and choose `WEP`.
2. Read the ammunition tile beside each weapon-mount row and record its quantity.
3. Fire one manual decoy order.
4. Reopen `WEP` and sample the quantities more than once, allowing for turret slew and reload time.

Do not treat an accepted order, the transient order icon, or an initial `event=fire` line as proof of repeated launches. Those signals can occur before the weapon bears and may only describe order acceptance. For a persistent firing cycle, require the ammunition count to continue decreasing across samples; if the count remains fixed and the weapon card reports `Idle`, the cycle stopped.

## Enter local skirmish

1. Open `Skirmish` > `New Skirmish` from the main menu.
2. Select the fleet, opponent, map, and match settings needed for the test.
3. Start the match.
4. Select and place every deployment group.
5. Click the separate `Deploy` button to begin the simulation.
6. Select the relevant ship or object before reproducing the issue.

Use local skirmish when testing the offline single-player path. It is distinct from Testing Range and from a locally hosted multiplayer server, so compare those modes when investigating lifecycle or authority differences.

## In-match debug console

Press `F2` during a match to open the developer console.

To select and control enemy ships during a test match, enter:

```text
dbgEnableEnemyControl true
```

Use enemy control when a reproduction requires both sides to maneuver, launch craft or missiles, or position targets inside a fuse or sensor range. Treat it as test-state setup and record when it was enabled in reproduction notes.

### Fast skirmish startup automation

After verifying the standardized main-menu layout, consolidate navigation into two Computer Use calls instead of capturing every intermediate screen:

1. Batch `Skirmish` > `New Skirmish` > the local-player fleet field, using short waits between transitions.
2. From the fleet selector, choose the requested fleet, confirm it, wait for the lobby to report that all mods loaded, and press `Start`.

For the verified 1920x1080 windowed layout, useful normalized references are `Skirmish (0.190, 0.532)`, `New Skirmish (0.430, 0.532)`, the fleet field `(0.412, 0.199)`, fleet confirmation `(0.370, 0.828)`, and lobby start `(0.870, 0.949)`. Fleet rows are content-dependent: locate the requested fleet visually and normalize its row position from that screenshot rather than storing a fleet-specific row coordinate. Refresh after each batch and stop if the expected next screen is absent.

### Fast deployment automation

After observing the deployment screen, count the visible unchecked deployment-group cards, then place all of them and press `Deploy` in one Computer Use call. Click a group card, click a valid point inside the green deployment region, and repeat for each remaining unchecked card before clicking `Deploy`. Use different nearby map points so groups do not overlap.

Represent click targets as normalized `(x / width, y / height)` positions from a verified deployment screenshot, then convert them using the current capture dimensions. For the currently verified three-group layout on The Pillars, the normalized references are:

- Three group cards: approximately `(0.278, 0.858)`, `(0.425, 0.858)`, and `(0.575, 0.858)`.
- Valid nearby placement points: approximately `(0.417, 0.548)`, `(0.447, 0.602)`, and `(0.477, 0.656)`.
- `Deploy`: approximately `(0.745, 0.762)` after all cards show check marks.

Use a helper such as `point = (nx, ny) => ({ x: Math.round(width * nx), y: Math.round(height * ny) })`, where `width` and `height` come from the latest Nebulous screenshot. Iterate the verified card and placement arrays with short waits, then click `Deploy` after the final placement. These are layout references, not blind universal targets: verify that the expected cards, deployment region, and button are present once before batching. If Computer Use reports user input, discard the remaining queued assumptions, refresh the Nebulous window, recount the unchecked cards, and continue from the visible check marks rather than placing completed groups again.

## Logs

The active Unity log uses Unity's standard per-user location:

```text
%USERPROFILE%\AppData\LocalLow\Eridanus Industries\Nebulous\Player.log
```

`BepInEx\LogOutput.log` under the game root may be stale; use `Player.log` unless its timestamp proves otherwise.

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

## Maintain testing knowledge

- Update this skill when a reliable reusable testing procedure or console command is discovered.
- Update `.agents/neb-testing.local.md` when a reliable reproduction fleet, mod dependency, installation override, or resolved local path is discovered.
- Record the exact fleet name and relevant ship, missile, component, or munition save key when it materially shortens a future reproduction.
