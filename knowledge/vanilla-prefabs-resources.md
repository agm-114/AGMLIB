# Vanilla prefabs, bundles, resources, and code contracts

This page records the parts of the installed vanilla data that materially
change how AGMLIB code should be understood. It is not a content catalogue.
The raw, generated inventory is
`generated/vanilla-asset-resource-inventory.md`.

## Evidence identity and scope

This pass uses:

- game build `0.6.2.4:260609-1922`;
- the `Nebulous.dll` hash in `evidence-ledger.md`;
- pristine files under the installed `Assets/AssetBundles`, Addressables,
  player-data, and loose-resource directories;
- the runtime prefab snapshot regenerated on 2026-07-24 with AGMLIB assembly
  `6.2.2.860`;
- only `Vanilla` and `AGMLIB` prefab sources.

Content-mod prefabs are deliberately excluded. A conclusion about a vanilla
pattern does not prove that a separately authored content-mod prefab follows
it.

## Four data planes

The evidence has four different meanings:

| Plane | What it proves | Important limitation |
|---|---|---|
| Pristine runtime bundles | What stock manifests name and what serialized objects ship with the current install | Raw pointers and typetrees are harder to interpret than a live object graph |
| Addressables/player data/resources | Assets not represented in stock manifests, including maps, campaign data, UI, audio, textures, and scenes | Presence does not prove a particular gameplay path loads the asset |
| Runtime prefab YAML | Resolved hierarchy, components, serialized fields, layers, and references after the game has loaded its registries | The object may have been normalized or mutated after bundle load |
| Decompiled code | Which fields and callbacks consume the authored data | Source alone cannot prove the prefab supplies the required graph |

Use pristine bundle data and post-load YAML together. A raw asset shows the
released input; a runtime dump shows the resolved result; code explains which
parts are behaviorally significant.

## Stock bundle loader

The pinned native `BundleManager` loads exactly three stock gameplay bundles:

- `Assets/AssetBundles/stock`;
- `Assets/AssetBundles/stock-f1`;
- `Assets/AssetBundles/stock-f2`.

It uses `AssetBundle.LoadFromFileAsync`, reads `manifest.xml`, optionally reads
the manifest's resource-definition XML, processes the declared registries, and
then unloads the bundle while retaining loaded objects.

The installed `Assets/ComAssetBundles` files are older, differently hashed
copies. AGMLIB still declares both stock and compressed paths in
`EntryPoint.cs`, but the bundle experiment is unreachable because `PreLoad`
returns immediately after `Harmony.PatchAll()`. The `QuickLoad` Harmony patch
is commented out. Do not use the compressed copies as evidence of the live
loader.

The pristine manifests contain:

| Bundle | Principal manifest entries |
|---|---|
| `stock` | 40 components, 6 missile bodies, 24 missile components, 2 spaceframes, 14 debuffs, 6 scenarios, 1 tip list, and 2 HUD themes |
| `stock-f1` | 1 faction, 9 hulls, 51 components, 20 munitions, 2 missile bodies, and 3 spaceframes |
| `stock-f2` | 2 factions, 9 hulls, 51 components, 19 munitions, 2 missile bodies, and 3 spaceframes |

The expanded runtime dump reconciles those gameplay categories: 3 factions, 18
hulls, 142 stock hull components, 39 munitions, 10 missile bodies, 24 missile
components, 8 spaceframes, 14 debuffs, 6 scenarios, and 2 HUD themes. One
recursively referenced munition is dumped outside the direct registry list.

It also finds 8 live Addressables-backed skirmish maps. Mission sets, codex
entries, and supplementary AI role definitions were empty in the live
`BundleManager` collections for this load. The expanded manifest now records
those zero counts explicitly. It also records 15 loaded tip strings without
exporting their text; the scalar list has no per-item source identity, so the
stock manifest's one tip-list asset remains the stronger provenance evidence.

## Addressables and other resources

The raw inventory found 1,220 Addressables internal IDs and 3,000 logical asset
entries across scanned Unity containers. The logical Addressables groups
include campaign animatics, dialog, entities, maps, music, faction textures,
stock maps, voices, and a large default group containing ScriptableObjects such
as AI ship-role definitions.

Native code and the expanded runtime dump give presence a narrower meaning:

- `ProcessAllNewAddressableAssets` promotes assets labelled
  `SkirmishMapInfoBlock` into the bundle manager's map registry;
- the live registry contained 8 such map objects;
- `PreloadAddressableBundles` loads assets labelled `PreloadBundle` only to
  force their backing bundles resident;
- additional mod catalogs have their bundle IDs rewritten to the owning mod
  directory.

AGMLIB has no active Addressables or `Resources.Load` integration in the
current source. Its relevant interactions are through native registries and
the debug prefab dumper. Future work should trace a resource only when AGMLIB
touches the native consumer; a name in the inventory is not enough.

## Fixed EWAR: rotation is authored data

The native `EWarFollowingMuzzle.SpawnInstance` chooses between the muzzle's
rotation and `Quaternion.identity` using inherited serialized field
`_matchRotation`.

The vanilla prefab set establishes a consistent authored distinction:

- all seven directed/turreted EWAR prefabs using `EWarFollowingMuzzle` set
  `_matchRotation: true`;
- all three omnidirectional EWAR prefabs set `_matchRotation: false`.

The directed set includes the E55, E20, E71, E90, E57, J15, and L50. The
omnidirectional set includes the E70, J360, and J75.

AGMLIB `FixedEWarComponent` repairs native facing logic, but it does not enforce
the muzzle field. Therefore:

- a fixed directed EWAR prefab must author `_matchRotation: true`;
- code review of `FixedEWarComponent` alone cannot establish correct effect
  orientation;
- validation should compare weapon facing and spawned effect rotation at arc
  edges.

## Continuous beams: damage and presentation are separate graphs

Both stock spinal-beam prefabs, Mk600 and Mk610, author:

- `Ships.ContinuousRaycastMuzzle`;
- `_damagePeriod: 0.2`;
- `_beamSpeed: 750`;
- a non-null `_effects` reference to `Ships.LineBeamMuzzleEffects`;
- `_hitEffectsContinuous: true`.

Their effect component explicitly references the hit VFX, hit audio source,
hit sound, muzzle VFX, ramped firing audio, line renderer, and tactical-view
effect. The raycast ranges differ: Mk600 is 600 and Mk610 is 500.

The Mk90/P60 laser family demonstrates that the same native class is configured
differently: very large beam speed, and in Mk90's case non-continuous hit
effects. AGMLIB must preserve authored values rather than treating every
`ContinuousRaycastMuzzle` as a spinal beam.

This closes an important part of the charge-up investigation:

- authoritative damage is driven by the muzzle raycast loop;
- beam and impact presentation are owned by the separately referenced effect
  graph;
- a working damage delay does not prove the client effect graph starts,
  updates its hit point, or stops correctly;
- `_damagePeriod` must be positive because native and AGMLIB stat display divide
  damage by it.

The remaining evidence gap is runtime sequencing across server, remote client,
effect pool activation, cancellation, and reuse.

## Vanilla mines: serialized state-machine contract

The M-30, M-30-N, and M-50 vanilla mine prefabs all author:

- an activation child on layer 22;
- a trigger `SphereCollider` with radius 200;
- `_armingDelay: 90`;
- `_flyoutHalfAngle: 40`;
- `_loiterLifetime: 0`, meaning unlimited;
- `_pushoffFlightTime: 20`;
- `_pushoffTimeDeviation: 8`;
- a `Communicator` with `_connectionBandwidthNeeded: 100`;
- a `SimpleCommsAntenna`;
- explicit network transform, push effect/audio, and deployed-animation
  references.

The standard and sprint mines set native `_network: false`; the cooperative
M-30-N sets it to true. In native `LoiteringMissile`, that flag means a
mine-to-mine cooperative trigger network. It is not the Mirror networking
enable switch.

This data explains code that otherwise looks incidental:

- native friendly-IFF suppression depends on a reachable communications path;
- the communicator needs compatible antennas on the same object;
- the trigger layer and obstruction mask are part of collision behavior;
- the serialized `NetworkTransform` lets the native mine cut stationary
  transform traffic.

AGMLIB's modular loitering defaults intentionally differ: 2-second arming,
120-second life, and 1,000-unit activation radius. It can recover a
`Communicator` component and create/fix the trigger layer, but it cannot invent
a useful antenna configuration. Treat those defaults as a new feature policy,
not vanilla parity, and profile stationary network traffic because the custom
implementation does not currently mirror native transform suppression.

## Hull and missile sockets: identity and indices are behavioral

The Axford MT1 hull socket demonstrates that a native socket is more than size
and component type. Its resolved data includes:

- stable key `whDP9rtbukKsocnrqnqaLQ`;
- short name `MT1`;
- cap and hull-segment references;
- attach point;
- traverse/forward limits;
- socket dimensions and overhang permissions;
- craft approach and release data.

Native missile bodies encode their sockets as an ordered list. On the CM-4,
the warhead at index 3 and engine at index 4 use `ResizeLinkedTo` to point at
each other, with distinct min/max/default and fixed/resizable policy. Stage data
then refers to the engine socket by index.

Consequences for AGMLIB sub-sockets and filters:

- socket key, list index, and reference remapping are compatibility data;
- copy/paste and undo must preserve or deliberately regenerate identity;
- parallel per-socket authoring lists require equal-length and stable-index
  validation;
- legality cannot be reconstructed from socket type and size alone.

## Command guidance descriptor

The pristine stock `Command Receiver` resolves to
`CommandSeekerDescriptor`. Its runtime dump includes point cost 3.5, validator
cost 3, targeting mode, miss-turnaround distance 50, and non-limited aspect.

That establishes authoring input only. The descriptor creates/binds runtime
seeker behavior during missile finalization, so the descriptor prefab cannot
prove launch-time target ownership, EMCON integration, pooled reset, or ranged
seeker behavior. Those remain code and runtime questions in
`missile-guidance-loitering.md`.

## Debuffs: descriptor policy, native instances, and timed removal

The expanded live registry contains all 14 debuffs declared by the stock
manifest. The descriptors prove that debuff behavior is highly data-driven:
affected native class names, stat identifiers/modifiers, validation method
names, multiple-instance policy, hit-only policy, periodic damage, spreading,
repair work, effects, emergency actions, and catastrophic events are authored
fields.

Native loading indexes each descriptor under every `AffectedClasses` string.
`HullComponent` then builds a table across its class hierarchy. Class names and
stat identifiers are therefore serialized compatibility strings, not display
text.

The native application path adds several constraints relevant to AGMLIB's area
debuff profile:

- `checkValid: false` bypasses table-membership checking only; it still calls
  the descriptor's verification method, respects `TriggeredByHitOnly`, and
  respects `AllowMultiple`;
- a duplicate with `AllowMultiple: false` creates no new instance;
- `AllowMultiple: true` plus `MultipleInstancesMultiply: true` increments the
  existing native instance's severity but does not retain the newly allocated
  instance ID;
- active debuffs, elapsed periodic/spread timers, catastrophic state, hit
  coordinates, and repair work are part of the native component save state.

Consequences for `AreaDebuffProfileModule`:

- the current `ForcedDebuffs` name overstates what it bypasses;
- a repeated time-limited non-multiple debuff does not refresh its removal
  timer because no new instance ID appears;
- a multiplied stack also produces no new active ID, so the first scheduled
  timer removes the whole stacked instance;
- AGMLIB's `TimedAreaDebuffRemoval` timer is not represented in native save
  state, so save/reload can restore the debuff without its custom expiry.

The stock `Power Grid Fluctuations` descriptor demonstrates the multiplying
case: both `AllowMultiple` and `MultipleInstancesMultiply` are true. Most stock
descriptors allow only one instance.

## Rules derived for future code research

1. When code reads a serialized private field, search all relevant vanilla
   prefabs for value ranges and nullability before copying a default.
2. When code follows a serialized reference, inspect the entire referenced
   component graph and lifecycle, not only the root component.
3. Distinguish a native semantic flag from similarly named network concepts;
   the mine `_network` flag is the current cautionary example.
4. Compare pristine bundle input with post-load YAML when mutation or
   normalization is possible.
5. Record layer, tag, transform, stable key, list index, and reference topology
   when they participate in native queries.
6. Do not infer support for an asset family merely because it exists in an
   Addressables catalog.

## Remaining targeted data work

- decide whether a metadata-only tip-list/count export is useful; debuffs,
  scenarios, HUD themes, and maps are now covered, while the current live
  mission-set/codex/AI-role collections are empty;
- trace representative radar, jammer, hangar, spacecraft, damage/debuff, and
  power/resource prefabs beside their native consumers;
- compare stock raw objects with their post-load dump for fields the loader
  assigns or normalizes;
- add a safe, filtered serialized-object query tool so future source traces can
  reproduce field checks without exporting copyrighted payloads;
- rerun the inventory and focused correlations after each supported game
  assembly or asset-bundle hash change.
