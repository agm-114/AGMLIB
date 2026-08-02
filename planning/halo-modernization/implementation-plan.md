# Implementation plan

## Architectural target

The rebuilt package is content-first:

- native NEBULOUS components own behavior already present in the current game;
- centralized AGMLIB owns small reusable deltas;
- bundle prefabs contain final current-version values;
- no entry point clones private stock fields or rewrites global game behavior;
- narrow Harmony adapters activate only when an explicit AGMLIB component is present.

The local runtime shim documented in `poc-shim.md` is a proof and migration aid. Its prefab
normalization and legacy assembly identity are not part of this final architectural target.

## Phase 0 -- freeze evidence

Status: complete for the installed package.

Artifacts to retain locally:

- exact Workshop payload and hash;
- prefab dump against game 0.6.2.5;
- 52-type serialized script inventory;
- 170-file source atlas;
- current assembly hash and decompiled native source snapshot;
- load log showing the `ReflectionTypeLoadException`.

The deep audit additionally proves a 52/52 direct serialized-type match, a 170/170 source-atlas
match, and object counts for all three payload bundles. See `coverage-audit.md`.

Re-run this phase if the game or Workshop item changes. Never compare behavior against an
unrecorded moving target.

## Phase 1 -- establish a loadable shell

Goal: all assets register with no legacy assembly.

1. Create a new Unity authoring branch/project against the current game.
2. Remove `halo.dll` and bundled Harmony from the manifest.
3. Add centralized AGMLIB as a declared dependency.
4. Import both source content sets.
5. Remove every missing legacy script.
6. Rebind native/AGMLIB replacements using `script-inventory.md`.
7. Build both bundles and scan serialized script assembly identities.
8. Load the game with only AGMLIB and the rebuilt package.

Exit criteria:

- `Assembly.GetTypes()` succeeds;
- no `halo.dll` reference exists in any bundle;
- 92 expected assets register;
- no registered asset is missing its primary `HullComponent`, `IMunition`, or missile descriptor.

POC evidence now shows that an exact-identity shim can load the existing bundles and repair the
immediate icon, audio, drive, and material failures. Preserve those mappings as asset-migration
inputs, but author them into the rebuilt prefabs instead of retaining runtime repair code.

## Phase 2 -- first vertical slice

Use the Gladius plus one UNSC MAC, one current missile, one basic compartment, and one faction
definition.

- modernize the Gladius armor/segment/collider setup;
- rebind all its thrusters;
- rebind its MAC to `MultiModeFixedWeapon`;
- re-author one old cluster warhead with `ModernSubmunitionWarhead`;
- load into the fleet editor, build a valid fleet, launch a skirmish, fire, take damage, save, load,
  and finish the match.

This slice proves bundle references, current hull authoring, native reports, damage, pooling,
save-state ownership, and AGMLIB dependency loading before mass conversion.

## Phase 3 -- UNSC content

Recommended order:

1. Halberd;
2. Paris;
3. Halcyon;
4. Marathon;
5. Thanatos.

For each hull:

- complete its hull checklist;
- convert ordinary weapons to native components;
- configure dual/triple MAC profiles and charging effects;
- convert launchers and magazines;
- convert missile bodies/components and all relevant munitions;
- validate faction lock, point cost, fleet editor warnings, AI use, and reports;
- add it to the host/client regression fleet before moving on.

Do not postpone Thanatos performance validation: its 71 sockets, four visual/armor segments, 146
colliders, and triple MAC are the UNSC stress case.

## Phase 4 -- Covenant vertical slice

Use Mutan Et.

- modernize its hull and 17 sockets;
- bind socket whitelist/blacklist rules using stable save keys;
- replace stored-resource patches with ordinary power demand and explicit shield capacity;
- convert one shield to `ModernShieldComponent` and `ShieldHitVisuals`;
- replace both of that shield's legacy holder components with exact-collider `ShieldHitSurface`
  markers;
- convert one plasma cannon and lance to native weapons plus visual adapters;
- convert the torpedo magazine/launcher to native bulk magazine plus filter and
  `MultiEjectorTubeLauncher`;
- convert torpedo guidance to current missile APIs.
- use `ModernEvasiveCruiseMissile` for the two authored terminal-evasion profiles and
  `ModernDebuffKineticShell` for the one overload/shield-depleting MAC shell.

Required engineering before this phase exits:

- a current player order/action menu path calls shield toggle on the host;
- a current network relay replicates toggle, integrity, and hit presentation to remote clients;
- shield save/load, destruction, cooldown, and multiple shield components are deterministic.

## Phase 5 -- remaining Covenant content

Modernize Ester, then Ket. Ket is the final scale/performance gate.

- verify all socket rules in editor and runtime load;
- test plasma weapon resource starvation and recovery;
- test shield hit absorption for ballistic, beam, missile, and armor-only splash paths;
- test torpedo programming with position, track, dogleg, and jammed comms;
- run AI fleets with no removed `AICaptain` patches.

## Phase 6 -- saved design migration

### Stable assets

Preserve existing save keys for semantic equivalents. A fleet referencing an unchanged hull,
component, munition, missile body, or descriptor should resolve to the rebuilt asset.

### Submunition settings

The old mixed cluster loadout does not map one-to-one to the native selectable descriptor.

Migration policy:

- if exactly one submunition type has nonzero load, select it and map staging/range/spread/release
  settings;
- if several types are present, select the first valid entry by saved order, emit one migration
  warning naming the missile design, and require the user to review it;
- if no entry is valid, clear the selection and emit a blocking design warning;
- save the result in the current native settings format on the next user save.

Never silently create a different mixed loadout.

### Removed scripts

Old save-state payloads for shield manager, stored resource pools, guided-shell pool state, and old
seeker state cannot be deserialized into current types. Load the asset by stable key, initialize the
new component from its authored defaults, and log one sanitized migration notice per design/save.

### Point totals

Do not patch validation to accept old totals. Recalculate with current native point-cost APIs and
surface ordinary fleet validation errors. Provide a balance migration note if an old fleet exceeds
the current limit.

## Phase 7 -- delete compatibility scaffolding

After the rebuilt assets pass the test matrix:

- remove unused marker classes;
- remove `ModernCommandGuidedSeeker` if all missiles use current modular descriptors;
- remove `SocketComponentScaler` only if the rebuilt hierarchy makes all eight legacy visual-scale
  exceptions unnecessary;
- remove any temporary global rollout gate;
- keep the socket and shield patches only if their explicit components remain;
- ensure every retained public AGMLIB type has a non-mod-specific reusable purpose or is clearly
  isolated in the compatibility folder.

## Runtime ownership and lifecycle

### Thrusters

Owned by each hull part. The serialized particles/audio and behavior configs survive prefab
finalization; controller binding is idempotent and removed on destroy. Disable resets transient
throttle/effect/damage state. Flank damage is host-only and applies only while contributing lateral
thrust.

### Multi-ejector launcher

Owned by the installed launcher component. Native `BaseTubeLauncherComponent` owns programming,
ammo selection, cycle, resource state, reports, and RPC routing. The AGMLIB class owns the
full-burst coroutine, configured per-shot withdrawal, and ejector selection, and uses native
cycle/RPC operations for each shot.

### Multi-mode fixed weapon

Owned by the weapon component. Native targeting, ammo feed, reporting, muzzle spawning, and save
state remain in the base class. The subclass chooses stat-modifiable
magazine/reload/recycle/charge values and temporary firing modifiers from selected ammo tags.
Charging presentation reads replicated cycle state. Random and gameplay mutation happen on the
host through the native weapon RPC provider.

### Shields

Owned per installed `ModernShieldComponent`, registered by `ShipController`. The registry is
idempotent and removed on destroy/socket change. Damage interception runs only on the server and
only when the actual hit collider carries a bound `ShieldHitSurface`. Armor-only hits ricochet on
that surface without reducing shield health; ordinary hull colliders remain native. Save state
belongs to the component. Remote presentation is a separate relay; clients never decide absorption.

### Debuff shell

Owned by one current lightweight munition asset. Native `LightweightKineticShell` owns flight,
pooling, casts, ordinary component damage, dedicated structure damage, and effects. The AGMLIB
subclass adds only the legacy debuff sphere pass and shield-depletion marker. Debuff application
uses a cached typed native-internals delegate; shield detection does not inspect native private
damager queues.

### Evasive plasma torpedo

Owned by each of the two rebuilt cruise-missile prefabs. Native `CruiseMissile` owns programming,
cruise/search/seek transitions, detonation, save state, and network movement. The subclass applies
only terminal weave/corkscrew steering on the host after native state processing, with a
network-ID-derived direction so pooled instances do not depend on unsynchronized random state.

### Visual adapters

Owned by their prefab object. They cache only materials or parent components with matching Unity
lifetime, unsubscribe events on destroy/disable, and do not mutate gameplay.

## Known remaining code gates

The AGMLIB replacement slice compiles, but these are not release-complete without the asset project:

- shield action-menu/order command;
- remote-client shield state and hit VFX relay;
- prefab bindings and exact serialized value migration;
- current material/VFX property names;
- saved mixed-cluster settings converter;
- model-space armor map and collider sampler bakes;
- gameplay/balance validation.

These gates are deliberately documented rather than hidden behind broad reflection.
