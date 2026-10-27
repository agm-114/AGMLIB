# Test Rig Patterns

## Activation Boundary

Use an exact, inspectable gate. Prefer an attached test component for asset-specific behavior and a separate local-only mod for global behavior.

```csharp
if (!config.Enabled || instance == null || !instance.isServer)
{
    return;
}

if (!MatchesConfiguredIdentity(instance) || !processed.Add(instance))
{
    return;
}
```

Do not use broad substring matching unless the configured fixture explicitly requires it. If matching several identity fields is necessary, log which field matched.

Resolve the actual spawned prefab reference first, then read its `SaveFileObject` key or equivalent stable identity from the refreshed prefab YAML. Similar display names are not interchangeable.

When a fixture replaces an inactive prefab root, verify required helper components
exist before configuring their fields. Explicitly attach missing helpers through
the prefab builder; a replacement subclass's `RequireComponent` attribute will only add them in the unity editor.
## Observational Probes

- Attach the probe to the GameObject that actually owns the callback, or observe the production method with a narrowly gated local-only Harmony patch.
- Do not assume a trigger on a child forwards `OnTriggerEnter` to a component on an unrelated parent or sibling.
- Do not add another collider merely to observe production physics; it can change overlap ordering and results.
- Deduplicate multiple colliders belonging to the same target into one stable contact identity.
- Record candidate classification in the same order as production code.
- Correlate the candidate with a production state transition or method observation; a state change alone may have another lifecycle cause.

## Bounded Readiness

Live objects may enter a lifecycle callback before their hull, controller, sensors, colliders, or network identity are ready. Wait for a concrete predicate and stop after a bounded number of attempts.

```csharp
private static IEnumerator ApplyWhenReady(Target target)
{
    const int maxAttempts = 20;
    for (int attempt = 0; attempt < maxAttempts && !IsReady(target); attempt++)
    {
        yield return new WaitForSeconds(0.25f);
    }

    if (!IsReady(target))
    {
        Telemetry.Warn("event=fixture-skipped reason=not-ready");
        yield break;
    }

    ApplyThroughNativeApi(target);
}
```

Avoid an arbitrary delay without a predicate; it makes fast and slow machines fail differently.

## Idempotence And Cleanup

- Track application per live object or network identity.
- Do not key by display name alone.
- Remove retained identities on despawn/session teardown, or use weak/live-object checks.
- Make Harmony registration idempotent across menu/match restarts.
- Ensure re-entering Fleet Editor or Testing Range does not stack runtime hosts or callbacks.

## Authority

- Mutate gameplay state only on the server/host unless the test explicitly concerns client presentation.
- Use the same authoritative path in single-player.
- Let vanilla networking replicate results after native state changes.
- Reject unknown ownership or session state instead of guessing.

## Automatic Testing Range Setup

When desktop control is unavailable, an explicitly gated local fixture can use
the game's native testing-range path. Inspect the installed assembly first:
`FleetEditorController.QuickTestBattle` passes `QuickFleetTestData` through
`CrossScenePipe` to the skirmish load scene. Resolve fleet summaries with
`Fleet.GetSummaryForFleet(FilePath)` and the stock testing map through
`BundleManager.GetMap`. Wait for bundle initialization before transitioning,
and bound all waits for a running match.

Keep the runner across scene changes with `DontDestroyOnLoad`, then use the
local player's live fleet to identify controlled test subjects. On the host,
`HullDockingPoint.QuickDockShip` follows native docking and replication. Native
player selection opens the docked ship menu; `TabbedControl.OpenTab` can expose
the repair pane. Verify the applied hull repair stat and visible pane text,
then destroy the setup runner while leaving the requested interactive game open.
This proves setup and presentation; undamaged ships do not establish healing.

For an editable fixture, pass `EditFleetData { Fleet = summary }` through
`CrossScenePipe` and load Fleet Editor (scene 1). Wait for
`FleetEditorController.EditorControllers` to contain the target ship before
selecting it with `FleetCompositionSubmodeController.SelectShip`.
`Fleet.FleetShipControllers` contains live-match controllers and cannot locate
`EditorShipController` objects. Keep graphics enabled for editor tests: native
nameplate baking and HDRP initialization can fail or crash with `-nographics`,
even when prefab loading succeeds. A normal rendered window may be launched
hidden for automated checks and closed after collecting evidence.

## Telemetry

Use a unique prefix and stable key-value fields:

```text
[AGMLIB Test] setup fixture=mine-fuse enabled=true role=server
[AGMLIB Test] event=candidate targetKind=fighter accepted=false reason=seeker-disabled
[AGMLIB Test] summary candidates=24 accepted=5 rejectedFriendly=7 rejectedType=12 peakTickMs=0.42
```

Keep high-volume events behind `DeepTelemetry=false`. A normal run should emit enough information to establish activation, outcome, and summary without logging every frame.

## Scenario Plan

Capture these fields in local notes or a local scenario file:

```yaml
name: light-mine-fighter-fuse
mode: fleet-editor-testing-range
durationSeconds: 90
playerFleet: carrier-test
enemyFleet: rail-test
setup:
  - launch one fighter group
  - deploy one light-mine salvo
observations:
  - mine reaches armed/loiter state
  - fighter crosses activation volume
acceptance:
  - exactly one server-side activation
  - target kind recorded as fighter
  - no duplicate client activation
```

Keep installation paths, private fleet details, and user-specific save keys in `.agents/neb-testing.local.md`, not tracked skill files.

For selective target logic, vary one target-class flag at a time and include an all-disabled negative control. Use a fresh or fully reset subject for each case so a successful activation cannot contaminate later crossings.

## Test Layers

- Unity adapter checks can run in an explicitly gated testing factory to verify
  hierarchy searches and native Harmony getters without starting a match. Use
  inert test subclasses to suppress unrelated initialization, frame callbacks,
  and teardown. Inspect `RequireComponent` dependencies too: auto-added native
  components may require live state during teardown. Add inert subclasses of
  those dependencies before the controller so Unity uses them. These checks prove
  adapter behavior, not docking, authoritative healing, or multiplayer replication.
- Pure tests: filters, list normalization, target classification, config precedence.
- Adapter tests: game-shaped interfaces without scene launch.
- Prefab tests: component/reference/field wiring from dumped YAML.
- Runtime fixture: lifecycle, pooling, triggers, server authority.
- Scenario comparison: broad behavior/performance metrics and log summaries.

Escalate only when the lower layer cannot represent the suspected failure.

## Damage Verification

- Separate collision/damage dispatch checks from real armor and component HP
  changes. A dummy IDamageable returning a fixed number does not exercise native
  armor, internal component selection, damage reduction, or damage frames.
- Native GPU armor painting and readback are asynchronous. Poll the sampled armor
  value with a bounded deadline after applying damage instead of asserting a
  change in the same frame.
- HullPart.CurrentHealth changes when native damage frames are applied;
  ISubDamageable.CurrentHealth also accounts for pending damage. When a fixture
  invokes a dealer directly, call the native controller's MarkAsDamaged and wait
  for the public HP value to change before claiming applied damage.
- Use different armor and component curves and an armor threshold that separates
  the expected penetration values. Explicitly selecting a real component proves
  its damage API and HP mutation; launched-blast component selection is a separate
  integration test.
