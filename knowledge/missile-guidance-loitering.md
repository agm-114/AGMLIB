# Missile guidance, command seekers, and loitering munitions

This page records the focused source correlation for AGMLIB command/position
seekers and the modular loitering missile. Claims are against the pinned
`Nebulous.dll` corpus in `evidence-ledger.md`; prefab and live multiplayer
verification remain outstanding.

## Native runtime contract

The relevant native chain is:

```text
descriptor.FinalSetup(pattern)
  -> ModularMissile.AddRuntimeBehaviour
  -> finalized pattern is cloned/pooled
  -> RuntimeMissileBehaviour.OnCloned / OnUnpooled
  -> SetProgramPath or SetProgramTrack RPC projection
  -> RuntimeMissileBehaviour.OnLaunched
  -> ModularMissile guidance queries targeting seekers
  -> impact/death
  -> OnRepooled and seeker ResetSeeker
```

`RuntimeMissileBehaviour` serializes both its owning `ModularMissile` and base
descriptor reference. Native command seekers also serialize their typed
descriptor, communicator, and antenna because the runtime behavior is attached
to the finalized pattern before live instances are cloned. `RuntimeMissileSeeker`
resets track IDs, initial target position, validator memory, forced PD detection,
and search state on both unpool and repool.

Native evidence:

- `Munitions/ModularMissiles/ModularMissile.cs`;
- `Munitions/ModularMissiles/Runtime/RuntimeMissileBehaviour.cs`;
- `Munitions/ModularMissiles/Runtime/Seekers/RuntimeMissileSeeker.cs`;
- `Munitions/ModularMissiles/Runtime/Seekers/RuntimeCommandSeeker.cs`;
- `Munitions/ActiveMissileSalvo.cs`;
- `Munitions/LoiteringMissile.cs`.

## Position seeker authoring representation

`PositionSeekerDescriptor` stores bundle-authored configuration as individual
booleans, enums, and floats. Its nested `SeekerSettings`,
`SeekerSourceSettings`, and the runtime `SeekerPositionData` are readonly
temporary values returned or constructed by code; they are not serialized
descriptor fields. This follows the current AssetBundle rule: the authoring
payload uses proven field types while code may still use custom typed values
after load.

`RuntimePostionSeeker._comdesc` is marked `[SerializeField]`, matching the
finalized-pattern clone requirement.

## Position seeker behavior

At programming time the seeker chooses a launch position from true center,
sensor-known center, launch input, or a mixture. It can add a random point in
target bounds and sphere noise. During flight it optionally queries the other
targeting seekers, updates its kinematic estimate, and returns a predicted
position from its own `SearchForTargetInternal`.

Confirmed source defects:

- `SetTrackTarget` clears `_cachedTrack` but calls the base implementation
  before sampling only when the old `_trackTargetID` is null. On a retarget, the
  custom launch estimate can therefore sample the old track and then assign the
  new ID at the end.
- Bounds noise dereferences `TargetedTrack.Trackable` before the nullable
  fallback. A missing sensor track with `UseBounds` enabled can throw.
- `MixGetPostionWithNoise` accepts a settings value but checks
  `_comdesc.SecondarySource` and `_comdesc.SourceSelection`. The in-flight
  settings therefore still use launch-time source-selection switches.
- Best- and worst-source position updates compare the new position with
  `_startaccel`, not the previous `_startposition`.
- `PredictedPosition` records but does not apply `_startaccel`.
- The custom cached track, start position, velocity, acceleration, and debug
  lines are not cleared by an override of `ResetSeeker`; only `_age` is reset
  on launch. Base pooling resets only base-class state.

Performance and authority risks requiring runtime evidence:

- `FixedUpdate` reflects `_validationSeekers` and `_targetingSeekers` every
  physics tick and calls every other targeting seeker's search routine outside
  the native missile guidance query. Those searches can mutate seeker state,
  threat state, random offsets, and network dirty bits.
- launch and in-flight noise use `UnityEngine.Random` without a server-only
  gate or a synchronized result. `SetProgramTrack` is projected to clients, so
  host/remote estimates may diverge even if authoritative transforms later
  hide the difference.
- zero known/predicted directions reach `Quaternion.LookRotation` and aiming
  cone calculations without explicit validation.

## Salvo EMCON integration

`ForceAllowEmcomLaunch` currently does not reach the native state it intends to
change. The postfix writes a private field named `_requiresComms`, but the
pinned `ActiveMissileSalvo` has no such field. It exposes
`OverrideComms { get; private set; }`, assigned from
`missile.RequiresCommunicator` when the first missile establishes the salvo
type. `ShipController` later uses `OverrideComms` to force comms on for active
salvos.

`Common.SetVal` silently walks the inheritance chain and returns even when no
field or property exists, so this mismatch produces no failure signal. Treat
the feature as non-functional against the pinned assembly until a typed
`ActiveMissileSalvo` accessor sets `OverrideComms` and a host/server EMCON test
proves the full path.

## Ranged command seeker

The ranged seeker largely copies native `RuntimeCommandSeeker` tracking and
comms logic to add physical range and an out-of-range policy. The typed
descriptor and communicator references are serialized, and the descriptor has
a fallback from `base.Descriptor`; those choices follow the clone/pool pattern.

The inheritance strategy currently creates two state machines:

- the private target/provider/path fields inside native
  `RuntimeCommandSeeker`; and
- `_targetedTrack`, `_commPath`, and range state in
  `RuntimeRangedCommandSeeker`.

Calling `base.SetTrackTarget` populates native base state before the derived
state is populated. The overridden search and validation methods then update
only the derived state. Inherited `CurrentTarget` and `CurrentTargetTrack`
continue to report the base private track, which can become stale after range
loss, comms loss, or supersession. Base and derived code also open separate
comms paths and perform overlapping threat add/remove calls. The native
trackable deduplicates threat additions, but duplicated ownership remains
fragile and makes teardown harder to reason about.

This should become either:

1. a narrow extension of native state through typed accessors and small policy
   overrides; or
2. a direct `RuntimeMissileSeeker` implementation that owns exactly one track
   and one comms path.

Do not retain two partially synchronized state owners.

## Modular loitering missile

`LoiteringModularMissile` adds the phases:

```text
inactive -> flyout -> arming -> loitering -> turning -> attacking
```

The server alone chooses fuse targets, lifetime destruction, and the engagement
transition. `RpcBeginEngagement` projects the chosen attack direction to
clients. The component resets phase, timers, push-off state, effects, contacts,
trigger state, and drag on both unpool and repool.

The authoring representation is bundle-safe: seeker-fuse selection is a
`List<bool>` indexed by socket index, while other settings are individual
native/Unity fields. The tooltip currently says one entry per runtime seeker,
but the implementation uses `seeker.Descriptor.Socket.Index` and extends the
list to `Sockets.Count`; document and validate it as a per-socket table.

Source-level concerns:

- seeker-fuse polling calls `SearchForTarget` for every enabled runtime seeker
  during server physics updates, with no validators. This is not a pure query:
  it writes the seeker's networked last-search result and may update internal
  target/threat/random state.
- `SetForceDetectPDTargets` is persistent until the native seeker pool reset.
  A large-only seeker-fuse filter therefore also changes subsequent seeker
  behavior for the attacking phase.
- contact fusing stores collider instances, so multi-collider targets cause
  repeated target resolution and linecasts.
- phase and phase time are locally reconstructed rather than synchronized.
  This resembles native loitering behavior, but the custom manual RPC and
  pooled subclass still require late-spawn, host, remote-client, and second-use
  tests.
- unlike native `LoiteringMissile`, the modular subclass intentionally retains
  `ModularMissile.PositionTargetingNoInput == false`. Confirm whether the
  desired UX is target-position deployment or native-style no-input release;
  both are plausible, but they produce different order UI.
- native loitering disables its network transform while stationary. This
  subclass does not expose equivalent behavior, so a dense minefield needs a
  network-traffic profile.

The friendly-IFF rule matches the important native behavior: a friendly object
can still wake the mine when the mine cannot receive that object's comms
broadcast. The expanded implementation also supports fighters and missiles,
which need explicit ownership/IFF cases in tests.

## Verification matrix

### Position and ranged command seekers

- initial programming, retarget to a different track, missing track, superseded
  track, bearing-only/visual track, and destroyed target;
- launch settings versus deliberately different in-flight settings;
- first/best/worst/average update modes with two controlled seeker sources;
- bounds noise on a track with and without a trackable;
- comms open, jammed, closed, relayed, and unjammable;
- just inside/outside physical range for lose, jam, and self-destruct policies;
- EMCON launch with `ForceAllowEmcomLaunch` on the first and later missile in a
  salvo;
- finalize, clone, unpool, launch, repool, and second launch;
- offline, host, remote client, dedicated server, and late observer.

### Loitering missile

- guided flyout and push-off flyout;
- immediate-search launch, arming delay, unlimited and finite lifetime;
- ship, fighter, missile, friendly with reachable comms, friendly without
  reachable comms, hostile, neutral/unowned, and self;
- multiple colliders on one target and target destruction inside the trigger;
- obstruction layer hit/miss in both directions;
- each seeker-fuse socket and large/small/fighter/missile filter combination;
- engagement RPC, late observer, pool/reuse, and many idle mines for network and
  CPU cost.

Acceptance requires one authoritative engagement, matching host/remote phase
presentation, no stale track/threat/comms state after reuse, and no unsupported
inspector option that silently selects different behavior.
