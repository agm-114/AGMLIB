# Editor, content, sockets, and filters

## Why editor code is compatibility-sensitive

Fleet and missile content persists bundle keys, save keys, type identities,
socket/module choices, and serialized component data. A change that looks like
an editor-only cleanup can prevent an existing fleet or prefab from loading.

## Native fleet-editor flow

`FleetEditor/FleetEditorController.cs` owns the loaded fleet, editor ship
instances, submodes, file save/load, and undo/redo delegation. Loading a fleet
constructs a `Fleet` and creates each ship through a template or saved hull;
saved ships call `LoadFromSave`, while new ships load hull defaults.

`FleetEditor/ShipEditorPane.cs::OpenPalette` builds a modal list for the native
`HullSocket`, initially selects by component save key, and installs list
filters. AGMLIB socket filtering is therefore an adapter to a native palette,
not the sole legality boundary.

The missile-editor submode loads templates into the fleet's
`AvailableMunitions`, edits active missile types, and removes deleted types from
ships. Save-key updates deliberately cover modular missiles and spacecraft.

Editor history is submode-owned. A custom socket edit must participate in the
same history and serialized design transaction or clearly document that it is
not undoable.

## Observed AssetBundle serialization boundary

Nested AGMLIB-defined custom classes and structs currently have a known
editor-versus-loaded-bundle failure. The evidence status, interim authoring
rules, and unknowns are maintained in
`knowledge/asset-bundle-serialization.md`; the reproduction plan is
`planning/asset-bundle-serialization-plan.md`.

## Socket model

AGMLIB touches native hull/missile sockets through:

- common filter primitives;
- editor filter composition and socket UI;
- child/sub-socket behavior;
- socket rendering and clipboard support;
- indexed modular missile filters;
- faction and availability filters;
- Harmony adapters around native socket/palette decisions.

The consolidation target is an evaluation result with:

- allowed/denied;
- stable reason code;
- optional user-facing explanation;
- contributing filter identities;
- context (faction, socket, parent, index, editor/runtime).

Do not let UI selection state become the authoritative rule. Runtime/load
validation must evaluate the same core.

## Sub-socket risks

- nested initialization order;
- parent/child save identity;
- copy/paste remapping;
- duplicate simple names or keys;
- filter context leaking from parent to child;
- renderer selection and hover state;
- loading content created before the child socket existed;
- component removal leaving orphaned serialized state.

## Current filter-core audit

The flattened key/filter/name/color lists in `KeyBasedFilterLookup` are
intentional bundle-safe authoring data under the current serialization rule.
They still need structural validation:

- all parallel lists must have a defined length relationship;
- keys must be non-empty and unique;
- missing filter/name/color entries need explicit fallback behavior;
- dependency socket/component lists must have equal lengths.

Current source creates dictionaries from `Zip` on demand. Mismatched lists are
silently truncated, duplicate keys throw, and repeated lookups allocate.
`SocketFilters.Copy` aliases source lists and omits several policy fields.
`EnsureChildSetup` can attempt to copy a null parent filter, and `CheckLegal`
uses `new SocketFilters()` as a fallback even though it is a `MonoBehaviour`.

The consolidation target is a validated typed view over the existing parallel
lists, not a nested custom DTO.

## YAML and prefab evidence

The source YAML loader is not proof of native prefab layout. Use the opt-in
prefab dumper and the `neb-testing` workflow to capture the exact component
ordering, field references, transforms, and asset identities used by a
reproduction. Keep local dumps out of Git unless a deliberately minimized,
licensed fixture is approved.

The pinned stock evidence confirms that socket identity is behavioral data.
Hull sockets carry stable keys, short names, caps, traversal limits, attach
points, and craft approach/release data. Modular missile sockets are ordered,
can link resizing by stable index, and are referenced by stage indices.
Sub-socket copy/paste, undo, and parallel-list validation must preserve that
topology. See `vanilla-prefabs-resources.md`.
