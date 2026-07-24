# AGMLIB feature-to-native integration map

This is a design and verification map for the recently completed and in-progress
feature set. “Done” means the implementation exists; it does not waive the
runtime, multiplayer, compatibility, or documentation checks listed here.

## Dynamic signature and power

AGMLIB:

- `AGMLIB/Dynamic Systems/DynamicActiveSignature.cs`;
- `AGMLIB/Dynamic Systems/UI/ShipSignatureDisplayReduction.cs`;
- dynamic reduction/power components under `Dynamic Systems`.

Native boundaries:

- `Game/Sensors/Signature.cs` and `BaseSignature.cs`;
- `Game/Sensors/SignatureRegistry.cs`;
- `Game/Sensors/SensorMath.cs`;
- `Game/Sensors/DeltaSensor.cs`;
- `Game/Sensors/SensorTrack.cs`;
- `Game/Units/ShipController.cs` EMCON signature modifiers.

Finding: detection truth, registered signature state, track state, and client UI
are separate layers. The power tooltip and any future status-board bars should
consume one calculated effective value; they must not duplicate the detection
formula. Verify changes after power loss, EMCON changes, damage, spawn, and
remote-client observation.

## Sub-module filters and UI

AGMLIB:

- `AGMLIB/Editor/SocketFilterCore.cs`;
- `AGMLIB/Editor/SocketPatches.cs`;
- `AGMLIB/Editor/ShipEditorSocketUI.cs`;
- `AGMLIB/Common/ChildSocket.cs`;
- indexed modular-missile filters.

Native boundaries:

- `Ships/HullSocket.cs`;
- `FleetEditor/ShipEditorPane.cs::OpenPalette`;
- modal list filters;
- missile socket permission/finalization;
- serialized ship and missile design keys.

Finding: UI filtering, installation legality, load validation, and runtime
reconstruction need the same core decision. Child sockets also need stable
parent/child identity and copy/paste remapping. The current reflection of native
socket size and faction fields belongs behind typed accessors.

## Weapon charge-up and beam impact effects

AGMLIB:

- `AGMLIB/Generic Gameplay/Muzzles/DelayedContinuousRaycastMuzzle.cs`;
- related beam/muzzle helpers.

Native boundaries:

- `Ships/ContinuousWeaponComponent.cs`;
- `Ships/ContinuousRaycastMuzzle.cs`;
- `Ships/Muzzle.cs`;
- weapon effect/report RPC paths.

Finding: authoritative fire/damage cadence and client effect lifetime are
separate. The unreliable impact FX should be instrumented as a short event
chain—charge accepted, firing committed, ray hit produced, impact effect
requested, pooled effect activated/repooled—on host and remote client. Avoid a
second fire-readiness decision inside the custom muzzle.

Current audit: the damage period is not validated against zero, and pooled hit
records returned before the next damage tick are not disposed. The latter
matches native source but remains a measurable retention/GC risk.

## Fixed EWAR

AGMLIB:

- `AGMLIB/Generic Gameplay/Ewar/FixedEWarComponent.cs`.

Native boundaries:

- fixed continuous weapon aiming/facing;
- `Game/EWar/ActiveEWarEffect.cs`;
- `Game/EWar/ActiveJammingEffect.cs`;
- `Game/Sensors/SignatureRegistry.cs`;
- weapon group facing checks.

Finding: fixed mount facing is an adapter around shared EWAR target/effect
semantics. Target gain/loss and jamming mutation must retain native EWAR
authority and pooling cleanup.

The following effect rotates with the fixed muzzle only when its prefab has
`FollowingInstanceMuzzle._matchRotation` enabled. Current AGMLIB code leaves
that private setting to prefab configuration, so a built-prefab dump is required
before considering spinal rotation closed.

## Custom stacking multiplier

AGMLIB:

- `AGMLIB/Generic Gameplay/Modifer/CustomModiferScaling.cs`.

Native boundaries:

- `Ships/StatValue.cs::CalculateModifier`;
- `Ships/StatModifier.cs`;
- `Ships/ShipStatAttribute.cs`.

Finding: the postfix replaces native totals after selecting a policy from the
modifier sources. It preserves the native `1 + sum` convention and
absolute-magnitude ordering, but policy conflicts and private field access need
tests and typed accessors. See `ships-components-damage.md`.

## Projectile/missile ammo modes

AGMLIB:

- magazine and launcher components under `Generic Gameplay/Discrete` and
  `Generic Gameplay/Missiles`;
- `MuzzleList`;
- modular ammo-mode cycle, fallback, and resource profile modules;
- `IModular` ammo-change callbacks.

Native boundaries:

- `Ships/WeaponComponent.cs::ChangeAmmoType`;
- `Ships/DiscreteWeaponComponent.cs`;
- `Ships/BaseCellLauncherComponent.cs`;
- `IMunition.SimMethod`, magazine/storage, and network spawn keys.

Finding: a swap crosses simulation method, compatible muzzle/ejector, magazine
withdrawal, launcher programming, point-cost/editor display, and network spawn
registration. The native ammo-change call should remain the transaction
boundary, with a deterministic rollback/fallback when a module rejects the new
mode.

Current audit: the resource failure-policy enum is not implemented, and the
cycle profile performs private stat writes/recalculation on every fire check.
“Done” should therefore mean the basic swap works, not that all authored policy
options and performance behavior are complete.

## Drone/command channel

Status: design incomplete.

Native boundaries:

- `Ships/CraftCarrierController.cs`;
- `SmallCraft/Spacecraft.cs`;
- `SmallCraft/SpacecraftGroup.cs`;
- craft order tasks;
- player and `AIControlledShip` launch paths.

Finding: do not enforce the channel at selection. Native traffic deliberately
queues replacements and separates preflight, pad use, launch, group activation,
landing, and postflight. Define whether channels count groups or craft, how
queued replacements reserve capacity, and whether manned/unmanned craft share a
serialized pool. See `small-craft-carriers-orders-ai.md`.

Current implementation warning: `CraftLaunchLimit` counts active and pending
craft across the owning player rather than the exact carrier and suspends native
orders by temporarily assigning an invalid private enum value. This can freeze
preflight and retain a pad reservation at the cap. Treat it as a prototype to
replace, not the base for incremental UI restrictions.

## Debuff on impact

AGMLIB:

- `AreaDebuffProfileModule`;
- `TimedAreaDebuffRemoval`;
- related area impact profiles.

Native boundaries:

- server-owned munition impact;
- `ShipController` damage resolution;
- `HullComponent` debuff instances;
- component RPC provider removal.

Finding: the feature works but depends on several untyped private members.
Migrate them to typed internals, define stacking/refresh policy, and test timed
expiry across destruction, save/load, and remote clients.

`DurationSeconds <= 0` currently turns a nominally timed debuff into an
unscheduled permanent one. Make that invalid or explicitly supported.

## SIGINT

AGMLIB:

- `AGMLIB/Generic Gameplay/Ewar/PassiveCommsSensorComponent.cs`.

Native boundaries:

- `SensorContextManager`, `SensorContext`, `SensorHost`;
- `DeltaSensor`, `SensorTrackableObject`, `SensorTrack`;
- `Communicator`, signature registry, jamming volumes, false/LOB tracks.

Finding: the current component spans observation, detection math, track
creation/loss, native private access, and UI styling. Extract pure scoring and
track policy from native adapters before adding more behavior. Server owns track
truth; client styling must never create a track.

## Wired guidance package

Status: implementation requires focused runtime testing.

Native boundaries:

- modular missile finalization and runtime behavior construction;
- `RuntimeMissileGuidance`;
- program path/track RPCs;
- seeker validation and stage transitions;
- launch-platform and communicator/channel ownership.

Required test: editor creation, fleet save/reload, finalized-pattern clone,
network spawn, launch, path/track programming, guidance handoff, target loss,
pool/reuse, host, remote client, and dedicated server. A pass in the missile
editor alone does not validate the package.

Focused source correlation is in `missile-guidance-loitering.md`. It found:

- retargeting and launch/in-flight source-selection defects in the position
  seeker;
- best/worst position calculations that use acceleration as the prior position;
- custom cached kinematic state that is outside native seeker pool reset;
- duplicated native/derived track and comms ownership in the ranged seeker;
- a confirmed native-member mismatch that makes `ForceAllowEmcomLaunch` a
  silent no-op against the pinned assembly.

The descriptor's nested readonly settings structs are temporary code values,
not serialized authoring fields. The actual descriptor payload remains flat
booleans/enums/floats and follows the current AssetBundle constraint.
