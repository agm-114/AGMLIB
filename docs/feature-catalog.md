# Feature catalog

This catalog describes the feature surface visible in the current source tree.
“Implemented” means source exists; it does not mean every configuration has been
runtime-tested against the currently installed game. The exhaustive file and
type evidence lives in `planning/generated/source-file-atlas.md` and
`planning/generated/component-inventory.md`.

## Requested feature status map

| Feature discussed in current work | Source family | Current reading | Validation still needed |
|---|---|---|---|
| Dynamic signature and power | `Dynamic Systems/DynamicActiveSignature.cs`, `DynamicReduction.cs`, `ResourceComponent.cs`, `ResourceModule.cs` | Runtime signature hooks and dynamic resource/power behavior are implemented. | Host/client authority, resource-tick ordering, clone state, and game-update target verification. |
| Power UI | `Dynamic Systems/UI/ShipStatusPowerBar*.cs`, `ResourceBar.cs`, `ShipSignatureDisplayReduction.cs` | Status-bar, status-board, icon-group, and signature-reduction display integrations exist. | UX binding coverage across tactical and status views; missing prefab bindings; per-frame allocation/log audit. |
| Sub-module dynamic filter and UI | `Editor/SocketFilterCore.cs`, `SocketPatches.cs`, `ShipEditorSocketUI.cs`, `Munitions/.../IndexedSocketFilter.cs`, `ScriptableFilter.cs` | Filter composition, indexed missile filters, faction filters, and editor socket patches exist. | Formal truth-table tests, filter conflict rules, and representative legacy fleet/prefab fixtures. |
| Sub-socket system | `Common/ChildSocket.cs`, `Editor/SocketPatches.cs`, `SocketRendering.cs`, `SocketClipboard.cs` | Child/sub-socket authoring and editor adaptations are present. | Save/copy/paste identity, nested initialization order, visual selection, and old-content compatibility. |
| Weapon charge-up delay | `Generic Gameplay/Muzzles/DelayedContinuousRaycastMuzzle.cs`, `DelayedRezzingMuzzle.cs`, `FX/LineBeamMuzzleEffects.cs` | Delayed/charge-up muzzle behavior and beam FX adapters exist. | Impact FX reliability, interruption/cancel paths, pooled muzzle reset, client visuals versus server damage. |
| Fixed spinal EWAR | `Generic Gameplay/Ewar/FixedEWarComponent.cs` and related sensor/track logic | A fixed EWAR component is implemented alongside turreted/internal sensor variants. | Firing arc, mount transform, authority, target loss, and editor stat parity. |
| Modified stacking multiplier | `Generic Gameplay/Modifer/CustomModiferScaling.cs`, dynamic modifier components | Custom modifier scaling and dynamic modifier sources exist. | Numeric parity with native stacking order and stable pure-math tests. |
| Projectile/missile ammo swap | `Munitions/.../AmmoModeCycleProfileModule.cs`, `AmmoModeFallbackProfileModule.cs`, `AmmoModeResourceProfileModule.cs`, `Generic Gameplay/Discrete/*` | Modular callbacks can react to ammo changes and fire checks; discrete magazine/launcher variants extend ammo handling. | Exact projectile-to-missile configurations, magazine bookkeeping, UI refresh, resource rollback, and multiplayer replication. |
| Drone command channel | `Generic Gameplay/Craft/FighterLimit.cs`, `ConfigurableBulkCraftHangarComponent.cs`, `LightweightCraftWorkSlotComponent.cs`, `Systems/DroneTester.cs` | Craft-limit and hangar experiments exist, but a single authoritative command-channel abstraction is not evident. | Decide whether limits apply to active craft, queued launches, or both; mixed drone/manned behavior; replacement queue semantics. |
| Debuff on impact | `Munitions/ModularMissile/ModularSystems/AreaDebuffProfileModule.cs`, `AreaShipDisableProfileModule.cs`, `AreaDamageControlTeamProfileModule.cs` | Scriptable impact callbacks and timed runtime state components implement area debuff/disruption effects. | Stacking/removal, repeated impacts, destruction cleanup, immunity rules, and server-only mutation. |
| SIGINT | `Generic Gameplay/Ewar/PassiveCommsSensorComponent.cs`, `TacticalModule.cs`, `TrackLogic.cs` | Passive communications sensing and tactical track support are substantial implemented systems. | Detection model calibration, false/stale track behavior, host/client visibility, performance, and log-volume baseline. |
| Wired guidance package | `Munitions/ModularMissile/CommandSeekers.cs` and modular command-seeker descriptors | Position and ranged command seekers, range-loss behavior, salvo integration, and debug overlay support exist. | End-to-end range loss, source destruction, guidance handoff, seeker pooling, and multiplayer testing. |

## Core boot and integration

`EntryPoint` implements the game mod entry point, guards against duplicate patch
application with the stable Harmony ID, and currently calls `PatchAll`. The file
also contains old dependency/load-order, asset-bundle recompression, Steam, and
debug experiments after unconditional returns. These are not active features but
are consolidation and deletion candidates.

`SetCopiedAssetId` supplies asset-copy identity behavior. `Common.Common`,
`OwnedTypeReflection`, `Filters`, and `ShipState` provide broad utilities,
reflection, filter primitives, and per-ship networked state foundations used by
many feature areas.

## Hull, materials, and authoring extensions

The active `Advanced/Hull` area provides complex hull behavior, automatic
collider sampling, paint schemes, paintable hull segments, and material-target
overrides. The material override files are current untracked work and should be
treated as work-in-progress until their serialization and renderer lifecycle are
tested.

`Editor/Materials` adds advanced paint schemes and material randomization.
`Editor/Lore` and string formatters add palette/fleet presentation metadata.
Point discounts, component tips, escort rules, and default missile templates
extend authoring and fleet composition.

## Dynamic ship systems

`ActiveSettings` is the shared activation/state base for many dynamic behaviors.
The family includes:

- active signatures and signature UI reduction;
- resource consumers, generators, fill behavior, and resource-aware reductions;
- dynamic component activation, animation, transform, glow, damage, reload,
  stun/knockback, modifiers, and throttle-time modifiers;
- area effects with pulsed, falloff, ammo-consuming, repair, resupply, ship
  modifier, jammer, seeker-jammer, and missile-softkill variants;
- boarding and assault-team behavior;
- user-order buttons and power/resource/status UI.

Most of this area crosses Unity lifecycle, Harmony, native private state, and
Mirror authority boundaries. It should be documented and tested as a cohesive
“ship state and resources” subsystem rather than as unrelated patches.

## Socket filtering and editor workflow

The editor family adds socket filter composition, rendering, clipboard support,
ship-editor UI, default missile templates, YAML loading, and custom component
tips. `ChildSocket` and socket patches form the sub-socket path.

Filters appear in both `Common`, `Editor`, and modular-missile code. Consolidation
should preserve released types while moving the shared evaluation model into one
typed filter core with adapters for hull sockets, missile sockets, factions, and
indexed choices.

## Weapons, magazines, launchers, and muzzles

The weapon family includes:

- continuous casemate weapons;
- fixed and turreted discrete magazine weapons;
- fixed cells, fixed tubes, kinematic ejectors, magazine loaders, ejector arrays,
  and custom lead logic;
- ammo filters, decoy-ammo sidecar behavior, weapon-check overrides, and generic
  weapon patches;
- delayed, pulsed, multi-beam, rezzing, and custom pulse-raycast muzzles;
- muzzle lists and feature hooks for modular ammo profiles.

Damage mutation must remain authoritative while charge-up, tracers, beams, audio,
and impact visuals may need client-side execution. Each custom muzzle needs a
state-reset checklist for clone/pool/reuse.

## Sensors, EWAR, and tactical information

The sensor/EWAR family contains advanced active fire control, multi-active fire
control, fixed EWAR, IFF, passive communications sensing, tactical modules,
track logic, Doppler radar, advanced radar, and turreted internal sensor
variants. It also provides jamming area effects and missile seeker disruption.

This family depends heavily on native sensor tracks, signatures, EWAR hosts,
player ownership, and visibility rules. The planned boundary is a typed detection
and track service plus thin native/Harmony adapters; runtime-only state should
not live in editor stat calculation.

## Craft and drones

Active craft extensions include advanced craft missiles and automatic craft gun
turrets. Generic gameplay adds configurable bulk hangars, craft weapon settings,
fighter limits, and lightweight work slots. `Systems/DroneTester` is an
experimental runtime test/control feature.

`Advanced/StrikeCraft` contains an older independent craft, kinematics,
formation, and movement model. It should not be treated as the canonical native
small-craft integration without an explicit revival decision.

The unresolved command-channel design needs separate counts for active craft,
reserved/queued launches, recoveries, and replacement launches. That avoids
encoding UI selection limits as the gameplay rule.

## Munitions and damage

The damager settings family supports multi-ray cones, single rays, spherecasts,
and spalling rays. Lightweight munitions provide kinetic, airburst, cluster,
proximity, splashing, selective, multi-shell, burst-container, and custom armor
damage variants.

The modular missile family adds:

- active, passive, passive-ARH, command, position, and ranged-command seekers;
- direct and cruise guidance;
- engines, decoy launchers, jammers, beam/fragmentation/impact-cone/kinetic and
  shell warheads;
- fuses, time-fuse state, loitering flight, angle thresholds, and illumination;
- module filters, faction filters, module limits, ammo-mode callbacks, resource
  profiles, and impact-triggered area effects.

The fuse patch for submunitions is associated with two focused historical
AGMLIB commits. It skips a proxy-fuse decision on the transient `IFF.None`
relationship and falls back to native behavior on exceptions. The approach
solved an observed issue but still needs null-specific handling, typed internals,
diagnostics, and regression fixtures rather than a silent blanket catch.

## Effects and presentation

The FX area provides dynamic effects, effect spawning, beam and pulsed-beam
modules, muzzle FX, following effects, spatial sound, thruster events, pulsed
thrusters, and shader material helpers. Treat spawned Unity objects and cloned
materials as owned resources: document who destroys them and what resets when a
pooled weapon or munition is reused.

## Server, inventory, and diagnostics

`Server/DamageCollector` builds inventory/after-action data. `CarrierSigPatch`
adapts signature behavior. `Server/Logging` patches Unity logging and is a
high-risk global diagnostic surface that should be opt-in, rate-limited, and
excluded from release behavior unless explicitly required.

`Systems/Class1.cs` defines salvage/inventory rules. Their current generic file
name obscures ownership and should be corrected only after checking public and
serialized identity compatibility.

## Testing support

The testing area provides opt-in component discovery/factories, prefab
inspectors, collider reports, and a YAML-oriented prefab dumper. It is useful
runtime infrastructure, not a conventional CI test suite. It should remain
isolated from normal gameplay and be paired with pure unit/contract tests for
logic that does not require Unity.

## Legacy and experimental code

`Advanced/Legacy` contains old missiles, warheads, magazines, physics toys,
strike-craft experiments, refactors, loose patches, and rework prototypes. It
also contains a duplicate `PaintableHullSegment` simple name. No new feature
should enter this tree by default. Every legacy file is listed in the source
atlas and should receive one disposition: retain for compatibility, isolate from
build, supersede with a shim, archive outside runtime compilation, or remove
after evidence proves it unreferenced.
