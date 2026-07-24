# Repository-wide file review findings

`scripts/Documentation/Export-AgmlibInventory.ps1` analyzed every tracked and
untracked, non-ignored repository file. Generated planning output and build/cache
directories are excluded to avoid self-counting. The current generated summary
records the total, including 223 C# files and 61 tracked DLLs.

Canonical exhaustive outputs:

- `generated/file-inventory.md`: every repository file and inferred purpose;
- `generated/source-file-atlas.md`: every C# file, type/base, namespace,
  serialized/lifecycle/patch/reflection/log/TODO signals;
- `generated/component-inventory.md`: 124 component-like types;
- `generated/harmony-patch-inventory.md`: 163 patch attributes;
- `generated/reflection-migration-inventory.md`: 88 reflection-boundary files;
- `generated/namespace-inventory.md`: namespace/global-type distribution;
- `generated/binary-inventory.md`: size, managed/file version, and SHA-256 for
  every tracked DLL.

## Structural cross-check

AST-aware searches found 214 public derived-class declarations, 96 patch classes
whose class is directly decorated with `HarmonyPatch`, and 189 generic
`Common.GetVal<T>` call shapes. These counts intentionally differ from the
regex-generated tables: multiple attributes can decorate one patch class,
method-level/dynamic targets exist, and reflection inventory includes other
helpers. Any compatibility checker should use compiled metadata and Harmony
resolution rather than one source pattern.

## Area totals

| Area | Files | Lines | Reflection tokens | Patch attributes | Log calls | Serialized fields |
|---|---:|---:|---:|---:|---:|---:|
| Generic Gameplay | 49 | 8,601 | 149 | 79 | 147 | 125 |
| Munitions | 52 | 5,452 | 60 | 22 | 92 | 103 |
| Dynamic Systems | 34 | 4,974 | 57 | 29 | 164 | 14 |
| Editor | 18 | 3,260 | 63 | 27 | 122 | 30 |
| Advanced | 34 | 2,749 | 31 | 1 | 68 | 47 |
| Testing | 3 | 1,869 | 24 | 0 | 15 | 2 |
| FX | 12 | 795 | 3 | 0 | 22 | 18 |
| Common | 7 | 769 | 19 | 1 | 13 | 0 |
| Root project source | 4 | 1,225 | 6 | 1 | 54 | 0 |
| Systems/Server/Craft | 7 | 893 | 8 | 3 | 7 | 11 |
| Nebulous typed accessors | 2 | 96 | 8 | 0 | 0 | 0 |

Counts are prioritization signals, not bug counts.

## Findings by area

### Root/project

`EntryPoint.cs` now contains only the live loader contract: inventory debug
setup, the stable duplicate-patch guard and `PatchAll`, test bootstrap, and
modular-faction inventory setup. The active missing-resource patch lives under
`AGMLIB/Patches`. Ordinary builds now use repository-local artifacts and an
explicit deploy command; the mutating batch scripts were removed.
`AGMMULTITOOL.cs` remains a separate large utility surface and needs an explicit
build/runtime ownership decision.

### Common

`Common.cs` is a broad reflection/log/utility boundary. `Filters.cs` overlaps the
editor and missile filter families. `ShipState.cs` is the shared networked state
base and needs authority/teardown documentation. `ChildSocket.cs` is part of the
sub-socket vertical slice. `OwnedTypeReflection` needs target verification and a
clear distinction from native typed internals.

### Advanced

Active hull/material work is mixed beside a large compiled legacy tree. The
legacy tree contains duplicate/simple-name risks, incomplete physics and craft
experiments, and loose patches. Review file-by-file using the exhaustive atlas,
but make disposition decisions as one compatibility project. The current
untracked paint material files require renderer clone/material ownership and
serialization tests.

### Dynamic Systems

This is a cohesive ship-state/resource/effect subsystem despite being stored as
many components. High patch and log counts cluster around resource ticks, area
effects, and UI adapters. Consolidate activation, resource transactions,
modifier registration, and teardown. `ShipStatusPowerBar` should split view,
binding, updater, and model. Profile repeated reflection/logging in resource and
area paths. `DynamicReduction` currently applies a reduction once in runtime
allocation but twice in its replacement fleet-editor summary; use the shared
native/data trace in `knowledge/power-resources-carriers.md` as the extraction
contract.

### Editor

Socket filters, UI, rendering, clipboard, templates, lore, and YAML are strongly
coupled to serialized identity. `YamlLoader` is the largest reflection hotspot.
Do not "clean up" namespaces or type names before fixtures. The filter core is a
good pure-test extraction seam.

### Generic Gameplay

This is the largest and most patch-heavy area. It contains several coherent
subsystems that should become explicit feature owners: discrete weapons,
continuous/muzzles, launcher/magazine/ejector logic, craft, engines, sensors/EWAR,
decoy sidecars, and modifiers. `PassiveCommsSensorComponent` is the largest
production feature file. Custom weapon variants share enough structure to risk
drift. Muzzle FX reliability requires authority/pooling evidence, not only
editor configuration.

### Munitions

The modular missile vertical slice is feature-rich but crosses descriptor,
editor, runtime behavior, pooling, seeker, fuse, damage, and profile callbacks.
`CommandSeekers` should split descriptor/config, runtime guidance, salvo
integration, and debug overlay. Impact profiles should share target/falloff/
timed-state helpers. Every runtime private descriptor reference needs clone/pool
review. The AGMLIB submunition fuse patch needs a typed, narrow fallback.

### FX

The area is reasonably separated, but every spawned effect, following effect,
material, and audio object needs an explicit owner and reset/destroy path.
Presentation should not own gameplay damage decisions.

### Craft and Systems

Native craft extensions and independent legacy strike-craft models should not be
conflated. Fighter limit, configurable hangar/work slots, and `DroneTester`
provide pieces but not yet a documented command-channel ledger. Generic
`Class1.cs` obscures serialized/public salvage and inventory rule ownership.

### Server

After-action inventory and carrier signature behavior are small feature patches.
Global Unity log patching is disproportionate risk and should be opt-in with
feature prefixes and rate limits.

### Testing

Testing infrastructure is large and useful for opt-in runtime evidence. Split
prefab traversal, data model, YAML formatting, redaction, and output. Ensure test
components cannot enter normal gameplay/package output. Add a conventional pure
test project beside, not inside, this runtime support.

### Nebulous accessors

The two accessor files establish the correct pattern but cover a small fraction
of current native private access. Expand incrementally based on the generated
reflection priority list.

## Focused semantic findings from native correlation

These findings go beyond the generated structural signals. They were verified
by reading the AGMLIB implementation beside the pinned native control flow.

### `Generic Gameplay/Craft/FighterLimit.cs`

- Active and pending counts are player-wide because they compare `OwnedBy`, not
  the exact carrier.
- Capacity suspension mutates a private native traffic-order enum to `-1`.
- Suspended orders skip preflight and launch processing and can retain an
  already reserved pad.
- The tick prefix lacks an explicit server guard.
- Treat this feature as an experimental prototype; detailed migration is in
  `feature-validation-plan.md`.

### `Editor/SocketFilterCore.cs`

- `CheckLegal` constructs fallback `SocketFilters` with `new` even though it is
  a `MonoBehaviour`.
- `EnsureChildSetup` can call `Copy(null)` when a parent lookup has no matching
  or default filter.
- `Copy` aliases the source whitelist/blacklist lists and does not copy
  `AllowNullComponent`, `AllowIllegalSize`, or `CustomType`.
- Key/filter/name/color tables are flattened parallel lists, which is the
  required short-term bundle-safe representation, but they currently lack
  equal-length and duplicate-key validation.
- Dictionary properties allocate on every access, duplicate keys throw, and
  `Zip` silently truncates mismatched lists.
- `GetGropupName` can index `Filters` when it is shorter than `Keys`.

Do not replace the parallel lists with a custom nested DTO until the bundle
round-trip research passes. Add authoring/load validation and a typed runtime
view over the lists.

### `Generic Gameplay/Ewar/PassiveCommsSensorComponent.cs`

- Context changes remove tracks directly from `SensorContext` instead of
  releasing this sensor through `SensorTrackableObject.Release`; the old
  tracking record can retain a stale sensor/track association.
- Destruction clears `_provider` without visibly unsubscribing its
  `OnSensorContextChanged` delegate or releasing every acquisition.
- Owner changes do not explicitly clear/reacquire team-scoped tracks.
- Network/composite UI helpers reflect and allocate lists on repeated display
  queries and recurse without a visited set.
- Dynamic nested-handle patch targets have no explicit missing-target
  diagnostic.

Mirror the native `DeltaSensor` release lifecycle before adding more SIGINT
presentation behavior.

### `Generic Gameplay/Ewar/AdvancedRadar.cs`

- The prefix on `SensorTrack.Update` calls the same patched method recursively
  after `UpdatePing`.
- A failed ping leaves age expired and can recurse indefinitely.
- A successful ping on a mixed track can cause both nested and outer native
  tracking updates.
- The smallest custom cycle controls all ping sensors on the track.
- `IgnoreLos` duplicates native signal math through reflected private state and
  has unbalanced profiler calls.
- Forced burnthrough includes native self-damage behavior on every eligible
  acquisition.

Replace this with per-sensor acquisition scheduling; do not add guards around
the recursive design as the long-term seam.

### `Generic Gameplay/Ewar/DopplerRadar.cs`

- `DopplerNotchSettings` is correctly shaped as an attachable sidecar but is
  incorrectly instantiated with `new` in two sensor types and their null
  fallbacks.
- Static contact records are swept only by overlay queries, so cleanup depends
  on presentation activity.
- No explicit sensor destruction or scene reset clears records.
- The math preserves native visibility first, which is the right composition
  boundary; keep that separation while fixing configuration/lifecycle.

### Fire-control, track-logic, and IFF experiments

- `ActiveFireControlSensorOptions` correctly targets native raw-range versus
  modified-range drift, but repeats a hot visibility check with reflection.
- `MultiSensor` and `MultiActiveFireControlSensor` are behaviorless serialized
  placeholders in current source.
- `RatioTrackLogic` can divide by a zero weight sum and injects true target
  position.
- `FirecontrolTrackLogic` ignores the acquisition delta and update success, so
  an unlocked sensor can return zero kinematics after a stateful target change.
- `IFFComponent`'s re-enable branch disables the sensor again, directly mutates
  a private stat cache, lacks null/authority/teardown controls, and logs from
  `FixedUpdate`.

### `Generic Gameplay/Muzzles/DelayedContinuousRaycastMuzzle.cs`

- Server fire delay and client effect delay are independent coroutines with the
  same configured duration, so telemetry must prove their committed/cancelled
  sequence rather than assuming synchronization.
- No validation prevents a zero damage period, which also feeds per-second stat
  division.
- A non-null `MunitionHitInfo` produced before the damage interval is not
  disposed. The current native `ContinuousRaycastMuzzle` has the same pattern,
  but `MunitionHitInfo` is pooled and finalizer fallback creates avoidable
  retention/GC pressure.
- Effect reliability depends on client-local raycasts and pooled
  `BeamMuzzleEffects`, separately from server damage.

### Modular ammo-mode profiles

- `AmmoModeCycleProfileRuntime.Apply` runs from every discrete `CheckFire`,
  repeatedly reflects, writes stat base values, and recalculates even when the
  selected profile has not changed.
- `AmmoModeResourceFailurePolicy` is authored but not consumed by any current
  source.
- An unknown resource name silently results in no requirement.
- Base cycle/resource snapshots can become stale if another feature
  legitimately changes the same native fields after the first cache.
- Cycle-size reduction clamps `_magazineFired` to the new size but does not
  itself start a reload, so the exact ammo-change/check-fire ordering needs a
  regression test.

### Area debuff profile

- Impact callbacks inherit the native server-only collision path, but the
  profile itself has no defensive authority assertion.
- Known private debuff members are reflected on each application.
- A timed debuff with `DurationSeconds <= 0` is added and never scheduled for
  removal.
- The live stock descriptors and native instance path confirm that a
  non-multiple duplicate creates no ID and does not refresh a timer.
- A `MultipleInstancesMultiply` duplicate stacks into the existing ID, so the
  first timer removes the complete severity stack.
- `ForcedDebuffs` bypass only native table membership; verification,
  hit-only, and multiple-instance checks still run.
- Native save state restores descriptor instances and native elapsed timers,
  but not the `TimedAreaDebuffRemoval` coroutine or expiry.
- The zero-radius path re-discovers a component through a physics-layer query
  instead of directly preferring the known hit object.

### Fixed EWAR

The component closely mirrors native turreted EWAR parameter setup and repairs
weapon-group facing because overriding `WepType` to EWAR makes native
`FixedAiming` false. Rotation of the spawned following effect still depends on
the muzzle prefab's private `_matchRotation` setting; the code does not enforce
it. The pinned vanilla dump now confirms all seven directed EWAR muzzles set it
true and all three omnidirectional muzzles set it false. Add a prefab gate for
fixed directed content.

### Removed `EntryPoint.cs` bundle experiments

- History review confirmed the stock/compressed bundle recompression,
  dependency/load-order repair, quick-load cache, and hull CSV dump were
  unreachable or unpatched and had no external references.
- Those experiments were removed; `PreLoad` now ends naturally after
  `Harmony.PatchAll()`.
- Installed `Assets/ComAssetBundles` copies are older and have different hashes
  from the three runtime bundles declared by native `BundleManager`.

Extract any still-useful diagnostic into an opt-in tool, then delete or archive
the unreachable production code. Do not treat compressed-bundle paths as a
current runtime dependency.

### `Munitions/ModularMissile/CommandSeekers.cs`

- Position-seeker retargeting can sample the old track before assigning the new
  target ID, and bounds noise can dereference a missing track.
- In-flight mixing still consults launch-time source-selection fields.
- Best/worst position updates compare against acceleration rather than the
  previous position.
- Custom kinematic/cache/debug state has no `ResetSeeker` override and can
  survive native pool resets.
- The per-physics-tick update reflects native seeker lists and invokes other
  seeker searches outside the normal guidance query.
- The EMCON salvo postfix writes `_requiresComms`, which does not exist in the
  pinned `ActiveMissileSalvo`. The actual private-set property is
  `OverrideComms`, and `Common.SetVal` makes the failed write a silent no-op.
- `RuntimeRangedCommandSeeker` owns a second target/comms state machine beside
  the private one in `RuntimeCommandSeeker`; inherited `CurrentTarget` can
  report stale base state after the derived range/comms state clears.

The full trace and test matrix are in `missile-guidance-loitering.md`.

### `Munitions/ModularMissile/LoiteringModularMissile.cs`

- The bundle-authored representation uses proven booleans and a flat
  `List<bool>`, but that list is per socket index rather than per runtime seeker
  as its tooltip says.
- Fuse polling invokes stateful seeker searches every server physics tick.
- Large-only fuse filtering persists forced PD detection into the attack phase
  until native pool reset.
- Collider-level contact storage repeats work for multi-collider targets.
- The custom phase/RPC/pool path still needs late-observer and second-use tests,
  and stationary mine network traffic needs profiling.

The IFF/comms rule intentionally follows native mines: a friendly target can
trigger when its broadcast cannot reach the mine.

## Non-source files

All assets, workflows, solution/project files, settings, manifests, scripts,
skills, documentation, and binaries are included in `file-inventory.md`. The 61
DLL hashes are captured, but provenance/license/redistribution and necessity are
still open human decisions. The sole workflow combines obsolete release and
build behavior and should be replaced only after repository-local packaging
exists.
