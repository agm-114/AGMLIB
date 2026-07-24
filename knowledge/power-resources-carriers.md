# Power, resources, hangars, and craft throughput

This page correlates the pinned native code with the vanilla prefab snapshot and
the current AGMLIB implementation. Assembly and dump identities are recorded in
[the evidence ledger](evidence-ledger.md).

## Vanilla resource contract

### Resource schedules and allocation

Verified from `Ships/ResourceType.cs`, `Ships/Ship.cs`,
`Ships/ResourcePool.cs`, `Ships/HullPartResourceConnected.cs`, and
`Ships/ResourceValue.cs` in the pinned decompile:

- a resource is either `Ticked` or `Pooled`;
- a ticked pool registers providers and consumers, sorts consumers, resets,
  produces, consumes, records peaks, and then asks the hull to update every
  component's working state;
- a pooled resource is allocated during ship setup rather than recomputed each
  resource tick;
- consumer priority is `Critical = 1000`, `High = 100`, `Medium = 10`, and
  `Low = 0`;
- consumers are sorted by descending priority, then descending demand within
  the same priority;
- ordinary `Consume` is all-or-nothing. If the currently remaining quantity is
  insufficient, that consumer receives zero and its demand is deferred into
  the displayed total;
- `ConsumeGreedy`, used by editor summarization, records the full demand but
  returns whatever remained;
- a component works only when every applicable `ResourceValue.HasAll` is true,
  it is not explicitly disabled, and it remains functional.

This means resource starvation is not a proportional throttle. A lower-priority
consumer can lose its complete allocation while an earlier consumer keeps
working.

### Serialized resource modifiers

`Ships.ResourceModifier` is a native serializable struct with four authored
fields:

- resource name;
- integer amount;
- per-tile-unit scaling;
- consume-only-while-operating policy.

`HullPartResourceConnected.Awake` resolves required resource names through
`ResourceDefinitions`, expands per-unit demand for tiled parts, and stores
runtime `ResourceValue` objects. Production separately multiplies a tiled
provider and applies its production-efficiency override.

The pinned vanilla
[`FR4800 Reactor` YAML](../.agents/cache/neb-prefabs/Vanilla/hull-components/Stock_FR4800%20Reactor-173daf03.yaml)
authors one `Ships.PowerplantComponent` with:

- `Power +4200`;
- `perUnit: false`;
- `onlyWhenOperating: false`;
- no required resources;
- critical resource-demand priority.

The priority has no effect while it is only a provider, but remains part of the
prefab's complete resource-connected contract.

### Peak and UI semantics

`ResourcePool.PeakQuantity` and `PeakDemand` are lifetime maxima for the pool
object; `Reset` does not clear them. `TotalAvailable` and `AmountConsumed` are
the current tick. The editor summary is separately calculated and is not the
runtime allocation record.

Any UI comparing current values to peaks must label those as historical maxima,
not capacity. A ratio of `available / demand` is coverage; `demand / available`
is utilization. They are reciprocal values and must not share one label.

## AGMLIB dynamic reduction and power display

This section depends on the
[vanilla resource contract](#vanilla-resource-contract). The current
implementation is in:

- [`DynamicReduction.cs`](../AGMLIB/Dynamic%20Systems/DynamicReduction.cs);
- [`ShipStatusPowerBar.cs`](../AGMLIB/Dynamic%20Systems/UI/ShipStatusPowerBar.cs);
- [`ShipStatusPowerBarBinding.cs`](../AGMLIB/Dynamic%20Systems/UI/ShipStatusPowerBarBinding.cs);
- [`ShipStatusPowerStatusBoardBinding.cs`](../AGMLIB/Dynamic%20Systems/UI/ShipStatusPowerStatusBoardBinding.cs);
- [`ShipStatusPowerBarPatches.cs`](../AGMLIB/Dynamic%20Systems/UI/ShipStatusPowerBarPatches.cs).

### Dynamic reduction behavior

`DynamicReduction` discovers active reduction components under the ship root,
filters them per target component and resource name, multiplies their factors,
and replaces the target's private runtime `ResourceValue[]` from a cached base
copy. Runtime consumption therefore applies the combined multiplier once.

The fleet-editor summary currently applies it twice:

1. its custom loop calls `consumer.GetResourceDemand`;
2. the Harmony prefix on that method first rebuilds `_requiredResources` with
   the reduction applied;
3. the custom summary then multiplies the returned demand by the same reduction
   again.

For example, a `0.9` reduction consumes `0.9` of base demand at runtime but is
presented as `0.81` of base demand by the replacement editor summary. This is a
confirmed code-path mismatch; a live editor fixture still needs to record the
visible number.

Additional implementation risks:

- the resource cache reads known native `_resources` state through
  `Common.GetVal` on every ship allocation/tick/editor recalculation;
- `AmountExtra` is populated but not consumed by current AGMLIB source;
- reduction discovery uses root-wide `GetComponentsInChildren` and LINQ during
  high-frequency resource paths;
- required-resource replacement writes a known private native field with
  uncached reflection;
- integer truncation occurs in `Reduce` before native demand use;
- the custom editor summary replaces native localization tokens with literal
  English labels.

The correct consolidation seam is a typed resource-demand calculator that
returns base demand, effective demand, contributing reductions, and rounding
once. Runtime allocation and editor presentation should consume that same
result.

### Power-bar behavior

The power bar reads native `IReadOnlyResourcePool` state without changing
allocation. It uses a persistent updater because the native power-status event
does not describe every intermediate production change. The local regression
scenario is recorded in `.agents/neb-testing.local.md`.

Current presentation semantics:

- generation and demand bars share a maximum derived from current generation,
  current demand, and optionally the lifetime peaks;
- color is red at zero generation, yellow when demand exceeds generation, and
  green otherwise;
- the tooltip's `Need Filled` value is actually `available / demand`;
- when supply exceeds demand, the tooltip can display more than 100 percent
  even though the graph clamps to one;
- `Generation vs Peak` and `Demand vs Peak` divide by the larger of current and
  peak, so they show current as a fraction of the lifetime maximum;
- the feature still reflects `_powerQuantityIcon`, `_iconImage`, and `_tooltip`
  through `Common.GetVal`.

Before calling the UX complete, rename the coverage/utilization fields,
separate current scale from historical peaks, migrate known UI internals to
typed accessors, and verify host/remote selection plus damage-control-panel
relinking.

## Vanilla hangar and deck contract

### Storage

Verified from `Ships/BaseCraftHangarComponent.cs` and
`Ships/BulkCraftHangarComponent.cs`:

- a hangar is both a craft store and a work-slot provider;
- its displayed list name combines the socket short name and authored hangar
  suffix;
- bulk capacity is `spaceUnitsPerTile * CalcTileMultiplier()`;
- occupied capacity includes craft storage units;
- landing reservation consumes capacity before physical recovery;
- save data stores craft template keys, while live save state stores full
  spacecraft savers and restores them through deferred object loading;
- the base hangar contributes one implicit postflight slot;
- bulk storage accepts any craft type unless a subclass overrides
  `CanFitCraft`.

The pinned vanilla
[`Bulk Internal Hangar` YAML](../.agents/cache/neb-prefabs/Vanilla/hull-components/Stock_Bulk%20Internal%20Hangar-0ec9618c.yaml)
authors three space units per tile, the `Hangars` container group, no resource
demand, and the default `Hangar` list suffix.

### Pads and work slots

The pinned
[`C4 Mount Hangar`](../.agents/cache/neb-prefabs/Vanilla/hull-components/Stock_C4%20Mount%20Hangar-873ada05.yaml)
and
[`C4 Medium Mount Hangar`](../.agents/cache/neb-prefabs/Vanilla/hull-components/Stock_C4%20Medium%20Mount%20Hangar-78125141.yaml)
both author:

- one always-on unit of Power demand;
- medium resource priority;
- an `ElevatorCraftLandingPad`;
- external use and landing pipelining enabled;
- a two-second launch/landing delay.

The pinned
[`Spacecraft Repair Station`](../.agents/cache/neb-prefabs/Vanilla/hull-components/Stock_Spacecraft%20Repair%20Station-67ebbf96.yaml)
authors one repair-only slot. The
[`Spacecraft Strikedown Station`](../.agents/cache/neb-prefabs/Vanilla/hull-components/Stock_Spacecraft%20Strikedown%20Station-39bd1760.yaml)
authors one preflight-and-postflight slot and no repair ability. Throughput is
therefore an authored composition of hangar storage, implicit postflight work,
separate work stations, pad geometry, animation state, deck state, and traversal
time.

`CraftLandingPad.LaunchCraftFromPad` starts a saveable coroutine. The craft
leaves storage, the pad opens, launch delay and green-deck conditions are
awaited, takeoff begins, a second delay runs, the carrier is notified, the pad
closes when appropriate, and traversal time elapses before reservation release.
The returned `true` means that operation started, not that the craft is already
flying.

One pinned native inconsistency should be tracked across updates:
`ICraftEntryExitPoint.SupportsLaunch` returns `_supportLanding`, while actual
availability checks use `_supportLaunch`. Do not infer launch capability from
only one of those paths without a runtime check.

## AGMLIB hangar and command-channel integration

This section depends on the
[vanilla hangar and deck contract](#vanilla-hangar-and-deck-contract). The
current implementation is in:

- [`ConfigurableBulkCraftHangarComponent.cs`](../AGMLIB/Generic%20Gameplay/Craft/ConfigurableBulkCraftHangarComponent.cs);
- [`LightweightCraftWorkSlotComponent.cs`](../AGMLIB/Generic%20Gameplay/Craft/LightweightCraftWorkSlotComponent.cs);
- [`FighterLimit.cs`](../AGMLIB/Generic%20Gameplay/Craft/FighterLimit.cs).

`ConfigurableBulkCraftHangarComponent` preserves native bulk storage and adds a
flattened list of separately attached work-slot providers. This is compatible
with the current AssetBundle serialization constraint: it stores Unity
component references rather than a nested AGMLIB-defined value tree. At
runtime it allocates a new combined array each time the native
`ICraftWorkSlotProvider.WorkSlots` property is read; cache/invalidation behavior
needs a measured decision rather than an assumed optimization.

`CraftLaunchLimit` does not prevent fleet-control selection. It leaves launch
orders in the native queue and temporarily hides their type from native tick
processing while capacity is full. That preserves queued replacement intent,
but the mechanism is unsafe:

- active and pending counts are owner-wide, not exact-ship or exact-carrier;
- all `Spacecraft` count together, so manned/unmanned separation is not a
  policy;
- a suspended order can retain its pad reservation and preflight state;
- traffic orders are private native objects mutated through repeated dynamic
  reflection;
- there is no explicit server guard in the patch;
- pending state is a static process-wide `HashSet` and is not reconstructed
  directly from saved carrier state;
- `GetQueuedLaunches` and `IsLaunchCandidate` are currently unused.

The replacement design should keep the queue, define channel scope and craft
classes explicitly, reserve channel capacity separately from pad capacity, and
promote replacements from authoritative carrier/group lifecycle events. The
full behavior and test matrix is in
[small craft, carriers, orders, and tactical AI](small-craft-carriers-orders-ai.md#command-channel-design-consequence)
and the
[runtime feature validation plan](../planning/feature-validation-plan.md#drone-command-channel).

