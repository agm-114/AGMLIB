# Native research coverage

This page distinguishes structural coverage from semantic and runtime evidence.
It prevents a generated source inventory from being mistaken for a complete
behavioral model.

## Pinned corpus

The current source corpus is the `Nebulous.dll` snapshot recorded in
`evidence-ledger.md`:

- SHA-256 prefix: `26DE96E2E384`;
- 2,533 decompiled C# files;
- approximately 331,846 decompiled source lines;
- 5,514 ILSpy type-index entries, including compiler-generated types;
- 3,572 type-index entries after excluding names containing `<` or `>`.

The exhaustive structural products are:

- `generated/native-subsystem-inventory.md`;
- `generated/native-source-file-atlas.md`;
- `generated/native-research-index.json`;
- `generated/vanilla-asset-resource-inventory.md`.

Regenerate them with:

```powershell
.\scripts\Documentation\Export-NebulousKnowledgeInventory.ps1
.\scripts\Documentation\Export-NebulousKnowledgeInventory.ps1 -Check
.\scripts\NativeCode\Export-NebulousAssetInventory.ps1
.\scripts\Documentation\Export-AgmlibDataTouchpoints.ps1
.\scripts\Documentation\Export-AgmlibDataTouchpoints.ps1 -Check
```

The asset inventory combines two distinct data planes:

- runtime-dumped registered and recursively referenced prefab YAML, with
  resolved hierarchy, component, serialized-field, layer, and reference data,
  including live debuff/scenario/HUD-theme/map registry objects;
- raw Unity containers, Addressables, player-data assets, loose resources, and
  localization metadata indexed with UnityPy.

It exports metadata and logical paths, not texture, mesh, audio, video, or font
payload bytes.

## Coverage levels

| Level | Meaning |
|---|---|
| Indexed | Every source file has a path, area, namespace, type, lifecycle, serialization, and networking signal record where detectable. |
| Traced | The principal types and control flow were read and recorded in a curated knowledge page. |
| Correlated | The native flow is mapped to current AGMLIB features and patch/access boundaries. |
| Prefab-verified | Component placement and serialized references were checked in a pinned prefab dump. |
| Runtime-verified | The flow was reproduced with a dated scenario and log evidence. |

The first three levels are documentation work. The final two require the
`neb-testing` workflow and should never be inferred from decompiled source.

`planning/generated/native-data-touchpoints.md` maps every AGMLIB source file
to native type-name mentions and any indexed vanilla serialized instances. It
is the queue for code-plus-data review; lexical matches are not semantic proof.

## Current semantic coverage

| Domain | Indexed | Traced | Correlated | Prefab/runtime status | Curated page |
|---|---:|---:|---:|---|---|
| Bootstrap, bundles, mods, registries | Yes | Yes | Partial | Stock manifests/raw containers plus selected live registries verified | `boot-content-save-network.md`, `vanilla-prefabs-resources.md` |
| Ships, hulls, sockets, parts, damage | Yes | Yes | Yes | Hull-socket identity/reference graph partly prefab-verified; damage prefabs remain | `ships-components-damage.md`, `vanilla-prefabs-resources.md` |
| Power, resources, hangars, deck throughput | Yes | Yes | Yes | FR4800, bulk/C4 hangars, repair and strikedown stations prefab-verified; editor/runtime reduction parity remains | `power-resources-carriers.md` |
| Weapons, muzzles, magazines | Yes | Yes | Yes | Stock continuous-beam graph prefab-verified; charge/impact runtime scenario remains | `weapons-munitions.md`, `vanilla-prefabs-resources.md` |
| Munitions and modular missiles | Yes | Yes | Yes | Vanilla mine and missile-socket data partly prefab-verified; clone/pool/launch fixtures remain | `runtime-lifecycle.md`, `weapons-munitions.md`, `missile-guidance-loitering.md`, `vanilla-prefabs-resources.md` |
| Sensors, signatures, tracks, EWAR | Yes | Yes | Yes | Directed/omni EWAR muzzle rotation prefab-verified; host/client and dense-fleet fixtures remain | `sensors-ewar.md`, `vanilla-prefabs-resources.md` |
| Small craft, hangars, traffic, groups | Yes | Yes | Yes | Command-channel fixture not yet implemented | `small-craft-carriers-orders-ai.md` |
| Player orders and tactical AI | Yes | Yes | Partial | Order save/load and MP fixtures needed | `small-craft-carriers-orders-ai.md` |
| Mirror networking, pooling, bulk save | Yes | Yes | Yes | Remote-client/dedicated-server matrix needed | `boot-content-save-network.md`, `networking-authority.md` |
| Fleet and missile editors | Yes | Yes | Yes | Existing-fleet/prefab compatibility corpus needed | `editor-content.md` |
| Campaign, missions, scripting graphs | Yes | Yes | Low | No AGMLIB feature currently depends heavily on all paths | `campaign-missions-scripting.md` |
| UI, audio, cameras, localization | Yes | Structural only | Feature-local only | Not comprehensively traced | Generated atlas |
| Steam, Workshop, lobby transport | Yes | Bootstrap-level only | Publishing work remains planned | External publish not exercised | `planning/build-release-plan.md`, root `TODO.md` |

## What “every file” means here

Every decompiled C# file is represented in
`generated/native-source-file-atlas.md`. That record is intentionally concise;
it answers where a type lives and what risk signals it contains. It does not
claim every method has been manually interpreted.

The curated pages trace the cross-cutting paths most likely to affect AGMLIB:

1. content is discovered and keyed;
2. editor designs become finalized patterns;
3. patterns are cloned, spawned, or unpooled;
4. server-owned gameplay mutates state;
5. Mirror sync fields/RPCs project state and effects to clients;
6. bulk save objects restore identity and deferred references;
7. teardown, repool, or scene replacement clears state.

## Remaining deep-research queue

These are research gaps, not implementation authorization:

1. Correlate representative spacecraft, radar, damage, map, and scenario graphs
   with their consumers. Hangar/resource exemplars are now correlated. The
   prefab dump manifest now records zero-count registry categories and the
   scalar tip-list count; live mission-set/codex/AI-role collections were empty
   in the pinned run.
2. Run a built-AssetBundle round trip for the custom nested-value serialization
   failure.
3. Record host, remote-client, dedicated-server, and late-observer traces for
   damage, charge effects, SIGINT, timed debuffs, craft launch/recovery, command
   seekers, loitering phases, and pooled missiles.
4. Trace UI/audio/camera subsystems only when an AGMLIB feature enters those
   boundaries; the generated atlas is the starting index.
5. Re-run the inventory and target verifier after each supported game assembly
   update and diff the pinned hash.
