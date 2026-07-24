# Risk register

## Advanced radar can recurse or double-update native tracks

- **Evidence:** the `SensorTrack.Update` prefix invokes `UpdatePing`, then calls
  the same patched `Update` method. If ping measurement fails, track age remains
  expired and the nested call re-enters indefinitely. If it succeeds on a
  mixed-contributor track, nested and outer native updates both run.
- **Impact:** stack overflow, duplicate measurement/acceleration updates,
  incorrect track quality, hot-path reflection/allocation, and multiplayer
  divergence.
- **Mitigation:** move periodic ping scheduling to a non-recursive acquisition
  boundary, make state per sensor, validate cycle time, and run controlled
  failed/successful/mixed track fixtures. See `knowledge/sensors-ewar.md`.

## Doppler settings and contact memory lack valid Unity ownership

- **Evidence:** `DopplerNotchSettings` is a `MonoBehaviour` but sensor fields
  instantiate it with `new`; the static contact list removes stale entries only
  when the overlay queries it.
- **Impact:** invalid/unattached configuration after bundle load or testing
  replacement, retained scene objects, stale visualization, and behavior that
  depends on whether a UI overlay was opened.
- **Mitigation:** author a separately attached settings sidecar, fail validation
  when missing, and add explicit sensor/target/scene cleanup plus lifecycle
  tests.

## Experimental IFF can permanently disable sensors and mutate stats

- **Evidence:** the branch labelled as re-enabling an internal sensor calls
  `SetSensorEnabled(false)`; the component directly writes
  `StatValue._valueCached`, assumes a selected radar and antenna exist, polls
  reflected mine communications every physics tick, and has no explicit server
  boundary.
- **Impact:** radar remains disabled, antenna power diverges, null/destroyed
  mine failures occur, logs flood, and clients can independently mutate
  presentation/gameplay state.
- **Mitigation:** keep the current component experimental, inventory serialized
  users, and replace it with typed native comms/emitter state plus
  server-authoritative transitions and deterministic teardown.

## Dynamic resource reduction disagrees between editor and runtime

- **Evidence:** the Harmony prefix on
  `HullPartResourceConnected.GetResourceDemand` first installs reduced runtime
  values. The replacement `ResourcePool.CalculateDemandForEditor` summary then
  multiplies the already-reduced result by the same factor again. A `0.9`
  reduction is therefore calculated as `0.81` in the custom editor path while
  runtime consumption uses `0.9`.
- **Impact:** fleet-editor power totals and fitting decisions can disagree with
  the launched ship; compounding reductions amplify the error.
- **Mitigation:** calculate base/effective demand once in a typed helper, use it
  from both paths, add pure multi-reduction/rounding tests, and record one live
  fleet-editor versus runtime comparison. See
  `knowledge/power-resources-carriers.md`.

## Craft launch limit can starve carrier traffic

- **Evidence:** `AGMLIB/Generic Gameplay/Craft/FighterLimit.cs` temporarily
  changes native launch traffic orders to an undefined enum value when at
  capacity. Existing pad reservations remain held while native preflight and
  launch processing are skipped. Active and pending counts are also scoped by
  player ownership rather than exact carrier.
- **Impact:** unrelated carriers can consume each other's capacity; a capped
  launch can block a landing pad; client-side patch execution has no explicit
  authority guard.
- **Mitigation:** treat the component as experimental, replace enum mutation
  with server-owned channel state at a stable carrier/group boundary, and run
  the two-carrier plus landing-at-cap scenarios in
  `feature-validation-plan.md`.

## SIGINT context changes can leave stale acquisitions

- **Evidence:** `PassiveCommsSensorComponent.HandleContextChanged` removes
  tracks directly from `SensorContext`; native `DeltaSensor` releases them
  through `SensorTrackableObject.Release`.
- **Impact:** stale sensor associations, incorrect reacquisition after network
  context changes, retained objects/delegates, and misleading UI contributors.
- **Mitigation:** mirror native release/unsubscribe order, add ownership/context
  transition fixtures, then optimize reflected UI traversal separately.

## Socket-filter authoring data is not validated

- **Evidence:** parallel lists are zipped into dictionaries on demand;
  `SocketFilters.Copy` is shallow/incomplete; null and `MonoBehaviour`
  construction paths exist.
- **Impact:** bundle-authored content can truncate rules, throw on duplicates,
  share mutable lists unexpectedly, or fail during child setup.
- **Mitigation:** preserve the bundle-safe flat representation, validate it at
  authoring/load boundaries, and build a typed non-Unity runtime view.

## Serialized prefab contracts can be mistaken for code defaults

- **Evidence:** fixed EWAR effect rotation depends on authored
  `_matchRotation`; continuous beam presentation depends on a referenced
  `LineBeamMuzzleEffects` graph; vanilla mine behavior depends on trigger
  layer, communicator, antenna, and network-transform references.
- **Impact:** a feature can compile and perform authoritative gameplay while
  facing incorrectly, losing client effects, triggering the wrong contacts, or
  producing excessive idle traffic.
- **Mitigation:** add prefab gates beside runtime tests, compare pristine bundle
  input with post-load YAML, and maintain the focused rules in
  `knowledge/vanilla-prefabs-resources.md`.

## Legacy compressed stock bundles can mislead research

- **Evidence:** `Assets/ComAssetBundles` contains older, differently hashed
  stock copies. AGMLIB's recompression/quick-load experiment is unreachable
  after the immediate `PreLoad` return and its Harmony patch is commented out.
- **Impact:** field values or payload identities from the old copies can be
  reported as current runtime behavior; dead loader code can be maintained as
  if it were active.
- **Mitigation:** use the three paths declared by native `BundleManager` as the
  live stock source, label compressed copies historical, and plan removal or
  archival of the unreachable AGMLIB experiment.

## Authored ammo/debuff options exceed implemented behavior

- **Evidence:** `AmmoModeResourceFailurePolicy` is not read by current source;
  timed debuffs with non-positive duration are never removed; non-multiple
  duplicate debuffs do not refresh expiry; multiplied stacks reuse one instance
  ID and the first custom timer removes the stack; custom expiry is absent from
  native save state; cycle profiles recalculate private stat values on every
  fire check.
- **Impact:** inspector configuration can promise behavior that does not occur,
  create accidental permanent effects, or add hot-path event/allocation cost.
- **Mitigation:** mark unsupported options in docs/validation, define policy
  semantics, and add pure plus runtime regression tests before release claims.

## Command seeker state and EMCON integration have confirmed drift

- **Evidence:** the position seeker uses old target state during some retargets,
  launch fields during in-flight mixing, and acceleration in two position
  comparisons. The ranged seeker duplicates native private track/comms state.
  The salvo patch writes absent field `_requiresComms`; the pinned native type
  uses private-set property `OverrideComms`, and `Common.SetVal` silently
  ignores the miss.
- **Impact:** wrong guidance estimates, stale current-target reporting, pooled
  state bleed, duplicated hot-path seeker work, divergent multiplayer noise,
  and an inspector EMCON option that does nothing.
- **Mitigation:** use one typed state owner, add the missing pool reset, move
  known native members to typed accessors, fail target verification loudly, and
  run the matrix in `missile-guidance-loitering.md`.

| Risk | Evidence | Impact | First control |
|---|---|---|---|
| Normal build mutates/deploys | project pre/post targets and `build.bat` | dirty trees, stale/locked installs, broken CI | B1 build isolation |
| Accidental API/serialized break | large global public surface, no assembly baseline | downstream mods/content fail | C1 release-DLL baseline |
| Native update breaks patches | 163 patch declarations, 88 reflection files | startup or feature failures | native target verifier |
| One patch failure affects all | single `PatchAll` | partial/unexplained initialization | feature-owned registration |
| Pool/clone state loss | runtime descriptor/component fields | intermittent missile/weapon bugs | lifecycle audit and fixtures |
| Custom value payload lost at bundle load | custom nested classes/structs work in editor but have loaded null/invalid | complex configuration silently disappears; tree models fail | prohibit for current authoring; bundle round-trip research matrix |
| Host-only correctness | many networked/native mutation hooks | duplicated or missing client/server behavior | authority records and matrix |
| Filter/sub-socket drift | multiple filter families and UI/runtime paths | invalid configurations or legacy load failures | shared evaluator plus fixtures |
| High-frequency overhead | large SIGINT/resource/UI files, reflection/logging | frame and log degradation | hot-path profile and rate limits |
| Legacy collision | duplicate/global types and compiled experiments | Unity/simple-name ambiguity | disposition and collision baseline |
| Unsafe release/Workshop | no package allowlist/skill | bad external update | R1 validation before R2 skill |
| Binary provenance gap | 46 tracked DLL references | licensing, reproducibility, silent updates | dependency manifest |
| Documentation drift | rapidly evolving components | misleading authoring/runtime claims | generated coverage plus curated records |
