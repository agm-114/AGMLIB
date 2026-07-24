# Runtime feature validation plan

This plan converts the native traces in
`knowledge/feature-native-integration.md` into focused evidence tasks. It does
not change feature status in root `TODO.md`; that remains the backlog source of
truth.

## Evidence format

Every run records:

- AGMLIB commit or dirty-worktree description;
- native assembly hash from `knowledge/evidence-ledger.md`;
- prefab/design/fleet/scenario identity;
- offline, host, remote-client, or dedicated-server role;
- expected event sequence;
- observed event sequence and bounded log excerpt;
- pass, fail, or blocked result;
- follow-up issue or regression fixture.

Use structured transition logs during development and remove or gate noisy
per-frame output before release.

## Dynamic signature and power

### Matrix

- radar and communications signature;
- powered, unpowered, damaged, destroyed, EMCON on/off;
- occluded and unoccluded aspect;
- spawn, save/reload, and live modifier change;
- host and remote-client UI.

### Acceptance

- Native detection math and UI tooltip consume the same effective value.
- UI changes do not create or mutate authoritative tracks.
- Registration does not duplicate after scene/load transitions.
- A missing dynamic component falls back to the native value.

### Resource-demand parity fixture

- Author one always-on Power consumer and one operating-only consumer at each
  native priority.
- Record base demand, each reduction multiplier, integer rounding, editor
  effective demand, runtime `AmountConsumed`, allocation received, working
  state, and UI value.
- Cover no reduction, one `0.9` reduction, two stacked reductions, a filtered
  non-match, per-tile demand, under-supply, and recovery after supply returns.
- Acceptance requires the editor, runtime pool, component working state, and
  power tooltip to agree on the same effective demand. The current replacement
  editor summary is expected to fail because it applies the reduction twice;
  see `knowledge/power-resources-carriers.md`.

## Sub-module filters and UI

### Matrix

- allowed, denied, null, wrong size, wrong faction, and indexed socket;
- parent changes that alter child filters;
- copy/paste, undo/redo, fleet save/reload, and old fleet load;
- direct serialized load of an item the current UI would hide.

### Acceptance

- Palette visibility, installation, load validation, and runtime reconstruction
  agree on the core result.
- Denials have a stable reason code.
- Parent/child keys remain stable and unique.
- Undo/redo restores both component and child-socket state.

## Charge-up beam and impact effects

### Instrumented sequence

1. charge requested;
2. native readiness accepted;
3. charge started;
4. fire committed or cancelled with reason;
5. ray damage tick produced a hit/no-hit;
6. impact effect requested;
7. pooled effect activated;
8. effect stopped/repooled.

### Matrix

- normal hit, miss, target lost during charge, weapon disabled, resource loss,
  component destruction, repeated burst, and pooled reuse;
- offline, host, remote client, and dedicated server.

### Acceptance

- Damage occurs only on the authoritative path.
- Every committed visible hit produces exactly one appropriate remote effect.
- Cancellation consumes no unintended ammo/resource.
- No stale beam or impact effect survives cooldown, destruction, or reuse.

## Fixed EWAR

- Prefab gate: directed `EWarFollowingMuzzle` must author
  `_matchRotation: true`; omnidirectional behavior is a separate configuration.
- Validate fixed forward arc edges and behind-mount rejection.
- Validate target gain/loss, power loss, component destruction, and repool.
- Compare jamming effect and weapon facing state on host and remote client.
- Confirm the fixed adapter does not alter turreted/internal EWAR behavior.

## Custom modifier scaling

Create pure calculation tests for:

- native penalty parity;
- no penalty, linear, exponential, and power modes;
- modifier and literal sequences;
- positive/negative/tied absolute magnitude;
- clamping and invalid factors;
- stat with stacking disabled;
- multiple sources with no policy, one policy, and conflicting policy
  priorities.

Add a native-target check for `StatValue.CalculateModifier`, `_modifiers`, and
`Attribute`.

## Projectile/missile ammo swaps

- Swap both directions through `WeaponComponent.ChangeAmmoType`.
- Verify muzzle/ejector simulation method, compatibility, magazine count,
  resource profile, point-cost presentation, and fallback.
- Save/reload while each mode is selected.
- Test an unavailable fallback and a rejected module callback.
- Confirm the munition's network spawn key is registered before firing.

## Drone command channel

### Policy decisions required

- channel unit: group, craft, or weighted craft cost;
- whether manned and unmanned craft share a pool;
- whether pending launches reserve a channel;
- replacement queue ordering;
- when returning, landing, and postflight craft release a channel;
- AI behavior when capacity is unavailable.

### Implementation seams

- replace the current invalid-enum `CraftCarrierController.Tick` suspension;
- scope limits explicitly to player, ship, carrier, or channel provider instead
  of accidentally filtering only by `OwnedBy`;
- observe carrier traffic without replacing it;
- track `SpacecraftGroup` membership and active order lifecycle;
- enforce capacity on the server when a planned group becomes active;
- expose synchronized current/reserved/available counts and rejection reasons;
- cover both player `LaunchCraftFlight` and AI sortie paths;
- persist only policy/configuration that must survive save/load; reconstruct
  transient counts from authoritative groups and traffic where possible.

### Scenario

Launch a mixed sequence with more queued craft than active capacity, destroy one
active craft, verify one replacement promotes, order the group home, recover
part of it, cancel remaining queued launches, save/reload during the queue, and
repeat with AI control.

Add a two-carrier case under the same player and a landing request while the
launch cap is full. This catches the current owner-wide counter and reserved-pad
starvation defects.

## Debuff on impact

- normal, forced, and timed debuff sets;
- duplicate impact/stack/refresh policy, including `AllowMultiple: false` and
  `MultipleInstancesMultiply: true`;
- non-component impact and invalid target;
- target destruction before expiry;
- save/reload during a timed effect;
- host, remote client, and dedicated server.

Acceptance requires typed accessors for known native members and one
authoritative add/remove path. The test must prove whether "forced" means
membership bypass or validation bypass, whether a second hit refreshes expiry,
and how a restored timed debuff reacquires or reconstructs its expiry.

## SIGINT

- silent and transmitting comms targets;
- friendly, hostile, neutral/unowned, and ownership change;
- occlusion and range boundary;
- jamming and network isolation;
- target destruction and repeated gain/loss;
- false track and jamming LOB coexistence;
- dense-fleet CPU/allocation/log-volume sample;
- host and remote-client styling.

Track truth must remain server-owned. Client code may style an existing track
but must not create one.

## Advanced radar and Doppler

For advanced ping tracks, use controlled sensors where `UpdateTrack` always
succeeds, always fails, and alternates. Combine ping-only, ordinary-active,
passive, and locked contributors on one track. Record call counts, track age,
position, velocity, acceleration, quality, mode, stack depth, allocation, and
server/client role. Acceptance requires no recursive `SensorTrack.Update`, one
intended measurement update per source, and independent cycle timing per
sensor.

For `IgnoreLos`, compare the same target in range/out of range,
occluded/unoccluded, illuminated/unilluminated, jammed/clear, and inside/outside
the subclass sector or cone. For forced burnthrough, record radiated power,
self-damage rolls, resource state, and sweep reset.

For Doppler, test both sides of each angle and velocity threshold, stationary
targets, relative platform motion, track acquisition/loss, lock
acquisition/loss, persistence expiry, sensor destruction, target destruction,
scene replacement, host, and remote client. Build the prefab through the real
AssetBundle path and prove the separately attached settings component survives.

## Wired guidance

Test in this order:

1. author a missile and save it as a template;
2. reload the fleet and inspect descriptor/runtime references;
3. finalize the missile pattern;
4. clone/network-spawn and unpool it;
5. program path and track;
6. launch and observe stage/guidance handoff;
7. lose/reacquire or invalidate the target;
8. impact/terminate and repool;
9. launch the same pooled instance again;
10. repeat on host, remote client, and dedicated server.

The feature passes only when configuration, runtime references, programming,
authority, and second-use reset are all correct.

Add the focused cases from `knowledge/missile-guidance-loitering.md`, especially:

- retarget from track A to B and prove no A kinematics remain;
- set launch and in-flight source modes to deliberately different values;
- exercise missing-track bounds noise without an exception;
- verify first/best/worst/average with controlled seeker outputs;
- prove `ForceAllowEmcomLaunch` changes native `OverrideComms` and the launching
  ship's effective EMCON behavior;
- compare inherited `CurrentTarget` with the ranged seeker's actual result after
  range loss and comms loss;
- repool and launch the same finalized missile a second time.

## Modular loitering missile

- prefab gate: trigger is on layer 22, is a trigger collider, and any authored
  communicator has a compatible antenna;
- record whether custom arming/lifetime/range values intentionally differ from
  the vanilla 90-second/unlimited/200-radius baseline;
- verify the seeker-fuse list as a per-socket table, including empty and
  mismatched authored lists;
- exercise ship, fighter, missile, friendly reachable/unreachable comms,
  neutral/unowned, and self cases;
- test multi-collider targets and destroyed contacts;
- test every phase on host, remote client, dedicated server, and a late
  observer;
- verify phase/effect/trigger/drag reset on second pooled use;
- profile CPU and network traffic with a representative dense idle minefield.
