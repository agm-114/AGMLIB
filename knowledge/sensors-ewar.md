# Sensors, EWAR, signatures, and SIGINT

## Domain separation

Keep these concepts distinct:

- **signature**: a registered emitter/return surface used by sensing;
- **sensor observation**: a measurement by a sensor at a time;
- **track**: accumulated state with ownership and visibility;
- **IFF**: relationship from an observer/owner perspective;
- **EWAR effect**: an active or passive influence on acquisition/quality;
- **UI presentation**: a client-visible projection, not authoritative truth.

## Native ownership graph

- `SensorContextManager` owns contexts, team sensor networks, trackables, server
  acquisition/update cycles, and sensor-delta RPCs.
- `SensorHost` registers `IDeltaSensor` instances for a provider.
- `SensorContext` owns local tracks, false tracks, active jamming records,
  network connection, and local/remote merge policy.
- `SensorTrackableObject` owns signatures and per-team tracking records.
- `DeltaSensor` owns swept signatures, gained/lost tracks, and locks.
- `SignatureRegistry` is the spatial registry for signatures and EWAR targets.
- `SensorTrack` pools track objects and records contributing sensors, locks,
  pings, position, velocity, and tracking mode.
- `BaseActiveSensorComponent` bridges a hull component into active sensing and
  jamming.

These are verified against the pinned sources under `Game/Sensors`,
`Game/EWar`, and `Ships/BaseActiveSensorComponent.cs`.

## Detection math

`SensorMath` computes signal density from power, range, and gain; return signal
from the target cross-section; noise from ambient and jamming inputs; positional
deviation from signal/noise; and track quality as a bounded discrete value.

`Signature` owns cross-section/return calculations and participates in
illumination and occlusion. A UI-only signature reduction therefore cannot be
treated as detection truth.

## Vanilla active-radar contract

Verified from `Ships/BaseActiveSensorComponent.cs`,
`Ships/InternalActiveSensorComponent.cs`,
`Ships/OrderableActiveSensorComponent.cs`,
`Ships/FixedActiveSensorComponent.cs`, `Game/Sensors/DeltaSensor.cs`, and
`Game/Sensors/SensorTrack.cs` in the pinned decompile:

1. an active sensor registers its signatures and owns a `DeltaSensor`;
2. the acquisition cycle spatially gathers candidates, evaluates range,
   occlusion, returned power, sensitivity, ambient noise, jamming, and any
   subclass facing/sector gate;
3. gained or lost acquisitions add or remove that exact sensor from the
   target's `SensorTrack`;
4. ordinary active contributors produce continuously updated tracks, ping
   contributors produce `TrackingMode.Ping`, and locking sensors separately
   contribute lock updates;
5. burnthrough increases radiated power for one acquisition sweep, can damage
   the sensor, and then clears the sweep flag;
6. a resource-connected sensor consumes authored Power only according to its
   operating state and resource priority.

`SensorContext.UpdateCycle` calls `SensorTrack.Update(skipStacked: true)` for
each local track. A normal track update deliberately ignores ping contributors;
`DeltaSensor` calls `UpdatePing` when a sweep hits. Locked tracks receive
additional `UpdateLocks` calls. Extensions must not recursively call the public
track update from its own Harmony prefix.

Pinned vanilla prefab examples show that these code paths are heavily
data-driven:

| Prefab | Native type | Power | Priority | Range | Burnthrough | Lock |
|---|---|---:|---|---:|---|---|
| R400 Radar | `FixedActiveSensorComponent` | 3000 operating-only | Medium | 1400 | No | No |
| R550 Early Warning Radar | `TurretedActiveSensorComponent` | 4500 operating-only | Medium | 1800 | No | No |
| RM50 Parallax | `InternalActiveSensorComponent` | 3600 operating-only | Critical | 950 | 12x, enabled | Yes |
| RS35 Frontline | `InternalActiveSensorComponent` | 2000 operating-only | Critical | 800 | 8x, enabled | No |
| RS41 Spyglass | `InternalActiveSensorComponent` | 4000 operating-only | Critical | 1150 | No | No |

All five collect intel. Their gain, radiated power, sensitivity, aperture,
noise filtering, error curves, panel coverage, facing, and jamming-LOB fields
remain part of the effective behavior; the table is not a replacement prefab
specification. The source YAML is under the ignored, reproducible local snapshot
at `.agents/cache/neb-prefabs/Vanilla/hull-components/`.

## AGMLIB advanced radar

This section extends the
[vanilla active-radar contract](#vanilla-active-radar-contract). The current
implementation is
[`AdvancedRadar.cs`](../AGMLIB/Generic%20Gameplay/Ewar/AdvancedRadar.cs).

`PingTracksOnly` rewrites `SensorTrack.AddSensor` acquisition type from Active
to Ping. A prefix on every `SensorTrack.Update` then reads the private ping list
and, once track age exceeds the smallest configured cycle time, calls
`UpdatePing`, `Update`, and `UpdateLocks` itself.

Confirmed consequences:

- if `UpdatePing` obtains no valid measurement, `_lastUpdate` does not advance;
  the immediate nested `Update` re-enters the same prefix with the age still
  expired and can recurse without a bound;
- if the ping succeeds and the track also has ordinary active/passive
  contributors, the nested native `Update` runs once and the original outer
  native `Update` runs again;
- the minimum cycle time across all matching ping sensors refreshes the entire
  ping-sensor list, so `CycleTime` is track-wide rather than per sensor;
- non-positive or invalid cycle times have no authoring validation;
- the global track prefix repeatedly reflects `_pingSensors` and allocates
  converted/filtered lists;
- `IgnoreLos` duplicates native signal math and private stat access, so game
  updates can drift between the ordinary and custom paths;
- its profiler calls are unbalanced because the prefix ends the native
  `Can See Signature` sample even though the original method has not begun it;
- `ForceBurnthrough` marks every eligible acquisition as a burnthrough sweep,
  including native self-damage probability, rather than representing a
  harmless visibility override.

Replace the recursive prefix with an explicit per-sensor ping scheduler at the
acquisition boundary. Reuse typed native detection helpers or accessors rather
than cloning `CanSeeSignature`, and validate cycle, authority, power,
burnthrough, lock, and pooled/scene lifecycle behavior.

## AGMLIB Doppler sensors

This section also extends the
[vanilla active-radar contract](#vanilla-active-radar-contract). The current
implementation is
[`DopplerRadar.cs`](../AGMLIB/Generic%20Gameplay/Ewar/DopplerRadar.cs).

The notch math evaluates absolute radial velocity against a configured angular
gate and velocity floor, optionally subtracting the sensor platform velocity.
The internal-search subclass filters acquisition after native visibility; the
fire-control subclass additionally filters lock maintenance. This preserves
native range, sector/facing, occlusion, return-power, noise, and jamming gates.

The authoring representation needs correction. `DopplerNotchSettings` is a
`MonoBehaviour`, which is valid as a separately attached component reference
under the current AssetBundle rule. The two sensors, however, initialize and
recover the reference with `new DopplerNotchSettings()`. Unity components must
be attached with `AddComponent` or assigned from an authored component; `new`
does not create a valid attached component. Keep the sidecar representation,
remove constructor fallback, and add an authoring/load validation failure for a
missing settings reference.

The static contact registry also needs an owned lifecycle:

- released records remain until persistence expires and an overlay query
  happens to sweep the global list;
- if the overlay is never visible, stale sensor/target references can survive;
- there is no explicit scene reset or sensor-destroyed removal;
- records are presentation state derived from authoritative track callbacks and
  must never become detection truth;
- `ShipDetailOverlay._active` is a known private member still read with
  `Common.GetVal`.

Add pure notch-boundary tests, native acquisition/lock fixtures, explicit
registry teardown, a second-scene test, and host/remote presentation checks.

## AGMLIB fire-control, track logic, and IFF experiments

These features depend on the
[vanilla active-radar contract](#vanilla-active-radar-contract) and native
`FireControlSensor` lifecycle.

[`AdvancedActiveFireControlSensor.cs`](../AGMLIB/Generic%20Gameplay/Ewar/AdvancedActiveFireControlSensor.cs)
addresses a real native inconsistency: `ActiveFireControlSensor.CanSeeSignature`
uses raw `_maxRange`, while its formatted stat and other paths use
`_statMaxRange`. The optional postfix repeats the native range/FOV/linecast
gates using the modified stat. Preserve that narrow intent, but migrate the
known fields to typed accessors and add a native-target test; this is an
acquisition hot path.

[`MultiActiveFireControlSensor.cs`](../AGMLIB/Generic%20Gameplay/Ewar/MultiActiveFireControlSensor.cs)
contains two empty subclasses and an unused private delta-sensor field. No
current source gives them multi-sensor behavior. Treat the types as serialized
compatibility placeholders until content/API usage is established; do not
describe them as a working feature.

[`TrackLogic.cs`](../AGMLIB/Generic%20Gameplay/Ewar/TrackLogic.cs) uses
bundle-safe parallel classification and component-reference lists, but its
runtime policies are experimental:

- `RatioTrackLogic` permits a zero weight sum, which produces invalid vector
  values, and deliberately blends authoritative true position into known
  position;
- `FirecontrolTrackLogic` asks a fire-control sensor for an acquisition delta
  but does not apply that delta before calling `UpdateTrack`; without an
  existing lock the native method returns false and zero vectors;
- it ignores the `UpdateTrack` success value and mutates fire-control target
  state as a side effect of producing track kinematics;
- the classification lookup validates duplicate keys and a short value list,
  but not extra/null entries or missing target classification state.

[`IFFComponent.cs`](../AGMLIB/Generic%20Gameplay/Ewar/IFFComponent.cs) is not a
safe production IFF implementation:

- its "REENABLING SEARCH RADAR" path calls `SetSensorEnabled(false)`, so it
  disables the sensor again;
- it selects a highest-power internal sensor without handling an empty result;
- antenna, native communicator, and reflected stat references can be null;
- it writes `StatValue._valueCached` directly and forces recalculation through a
  temporary modifier;
- mine and ship jamming are polled every physics tick on every peer with noisy
  logging and no authority boundary;
- destroyed mines can remain in the trigger-owned hash set;
- it conflates communications state, sensor emission, IFF, and mine detection
  in one component.

Keep the public types for compatibility until usage is known, but classify
multi-sensor, track-logic, and IFF behavior as experimental. A replacement IFF
design should use native communications reachability and sensor/emitter
interfaces, explicit server authority, typed access, and presentation-only
client state.

## Communications and jamming

`Communicator` owns antennas, outbound paths, bandwidth, transmit state, and
received jamming. `ActiveEWarEffect` spatially gathers targets and cycles
effects; `ActiveJammingEffect` creates and updates `JammedVolume` records.
Communication/network power affects context isolation and remote-track sharing.

Jamming can create a bearing-only `JammingLOBTrack`; false tracks implement the
normal track interfaces but are not real target objects.

## Dynamic signatures

`DynamicActiveSignature` patches native signature calculations and occlusion
behavior. `ShipSignatureDisplayReduction` adapts UI display. The planned
architecture is one typed dynamic-signature service that computes the effective
runtime value, with separate adapters for native signature queries and UI. This
avoids independent formulas drifting between detection and presentation.

Validate:

- base versus modified radar/communications/other signature types;
- disabled, damaged, resource-starved, and destroyed components;
- occlusion and facing dependencies;
- editor estimate versus runtime result;
- host/client display;
- changes after spawn without re-registering stale signature objects.

## Passive communications/SIGINT

`PassiveCommsSensorComponent` is one of the largest source files and combines
patching, sensing, track logic, and presentation concerns. Consolidation should
extract:

1. observation collection;
2. deterministic scoring/detection math;
3. track creation/update/loss policy;
4. server authority and replication adapter;
5. UI/debug presentation.

This enables pure tests for scoring and stale-track logic and reduces native
reflection in hot sensor paths.

Required runtime scenarios include silent targets, active emitters, friendly and
hostile ownership, occlusion, range boundaries, source destruction, ownership
change, repeated acquisition/loss, host/client visibility, and dense-fleet
performance.

The current component's context-change path calls
`SensorContext.RemoveTrack` directly. Native `DeltaSensor` instead releases
each acquisition through `SensorTrackableObject.Release`, which removes the
sensor and only removes the context track when no acquisition remains. Direct
removal can leave stale tracking records. Destruction also needs to unsubscribe
the provider context event and release acquisitions before references are
cleared.

The UI styling helpers traverse reflected composite/network track structures,
allocate lists on queries, and do not maintain a visited set. Keep those
presentation risks separate from the acquisition fix.

## Fixed and turreted EWAR

Fixed EWAR should use the configured mount transform and arc without inheriting
turret assumptions. Turreted/internal variants must not be copy-pasted into
divergent target and lifecycle logic. Share typed target validation and effect
application; keep mount motion and presentation as adapters.

The pinned vanilla prefab set verifies an authored rotation contract. All seven
directed/turreted `EWarFollowingMuzzle` instances set inherited
`_matchRotation: true`; all three omnidirectional instances set it to false.
Native `SpawnInstance` uses that value to choose the muzzle rotation or world
identity. `FixedEWarComponent` does not override it.

A fixed directed EWAR prefab must therefore set `_matchRotation: true`.
Validate custom authored content separately; vanilla consistency does not prove
an external prefab was built correctly. The complete bundle/prefab evidence is
in `vanilla-prefabs-resources.md`.
