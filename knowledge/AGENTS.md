# NEBULOUS game knowledge

This folder records reusable, evidence-graded knowledge about how NEBULOUS:
Fleet Command works and how AGMLIB integrates with it.

## Evidence policy

- **Verified** means supported by the current managed assembly, prefab snapshot,
  or focused runtime log.
- **Inferred** means supported by AGMLIB call sites or decompiled control flow
  but not yet reproduced live.
- **Historical** means valid for an identified older game, mod, or assembly
  version.
- Always record assembly paths, hashes, prefab identities, and dates when a
  conclusion can drift across game updates.
- Decompiled C# is an approximation. Use IL or runtime evidence when exact
  dispatch or control flow matters.

## Map

- `evidence-ledger.md`: current assembly and snapshot identities.
- `runtime-lifecycle.md`: finalization, cloning, pooling, launch, teardown, and
  sidecar lifecycles.
- `asset-bundle-serialization.md`: observed custom class/struct bundle-load
  failure, current authoring constraint, and research boundary.
- `game-architecture.md`: ship, weapon, missile, sensor, craft, and editor roots.
- `research-coverage.md`: exact distinction between exhaustive indexing,
  semantic traces, AGMLIB correlation, prefab evidence, and runtime evidence.
- `vanilla-prefabs-resources.md`: how pristine bundles, Addressables, resources,
  and post-load prefab graphs constrain AGMLIB code.
- `boot-content-save-network.md`: main-menu bootstrap, mod/bundle load order,
  registry identity, pooling, save IDs, and deferred reference restoration.
- `ships-components-damage.md`: ship object roles, armor/component damage flow,
  damage frames, debuffs, and modifier stacking.
- `power-resources-carriers.md`: native resource scheduling, vanilla power and
  hangar authoring, dynamic reduction/UI parity, and command-channel
  integration.
- `small-craft-carriers-orders-ai.md`: hangar traffic, launches, recovery,
  command-channel design, group orders, and tactical AI.
- `campaign-missions-scripting.md`: legacy mission/scenario graphs, campaign
  XML, new scripting graphs, and campaign network projection.
- `feature-native-integration.md`: current AGMLIB feature families mapped to
  native ownership, lifecycle, editor, save, and verification boundaries.
- `editor-content.md`: prefab/content identity, sockets, filters, and YAML.
- `weapons-munitions.md`: weapons, muzzles, ammo modes, fuses, and impacts.
- `missile-guidance-loitering.md`: command/position seeker state, salvo EMCON,
  loitering phases, pooling, and multiplayer verification.
- `sensors-ewar.md`: sensors, jamming, SIGINT, signatures, and dynamic display.
- `networking-authority.md`: server/client ownership and replication questions.
- `native-boundaries.md`: Harmony, reflection, typed accessors, and target
  verification.
- `generated/native-subsystem-inventory.md`: deterministic area, subsystem,
  namespace, and hotspot counts from the pinned decompile.
- `generated/native-source-file-atlas.md`: one structural record for every
  decompiled `Nebulous.dll` C# file.
- `generated/native-research-index.json`: machine-readable source/type index
  consumed by future research passes.
- `generated/vanilla-asset-resource-inventory.md`: raw Unity container,
  Addressables, resource, localization, and registered-prefab metadata summary.
