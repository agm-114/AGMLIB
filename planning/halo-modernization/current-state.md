# Current evidence

## Package and install

The Workshop package contains two declared content bundles (`unsc` and `covenant`), the legacy
`halo.dll`, an old bundled Harmony assembly, manifests, word lists, and an undeclared
`expandedmountdisplay` UI bundle. The exact subscribed package was copied into the local NEBULOUS
`Mods/Halo` directory and placed immediately after local AGMLIB in `.ActiveModList.xml`. The
Workshop and local payloads match: 12 files totaling 166,695,003 bytes.

The legacy DLL SHA-256 is:

`3920E80C83C49A428CFA920431E3C4C25077D2486E67D942BD4E48337D022856`

The current `Nebulous.dll` SHA-256 used for the native comparison is:

`651B9CFBAB8D5411B7D241EB176F640F11BFEA8AAF5E8674093C9AE3198B5CF2`

The working load order used for investigation was:

1. local AGMLIB;
2. local Halo package;
3. existing Workshop AGMLIB;
4. the pre-existing test mod.

The duplicate AGMLIB entry is pre-existing local state and was not removed.

## Reproduced load failure

The current game loads enough of each bundle to register 92 assets, but the mod reports
`LoadedWithMinorErrors`. The first deterministic fault is:

`ReflectionTypeLoadException` from `Assembly.GetTypes()` during
`Ships.StatTable.CollectStatsFromAssembly`.

Two concrete types fail at the enumeration boundary because they implement the removed
`Game.UI.IQuantityMonitor`:

- `ShieldMonitor`;
- `StorageMonitor`.

The interface now lives at `Utility.IQuantityMonitor`. Fixing those two identities is necessary but
not sufficient. The old assembly also has unresolved references to removed or moved types:

- `Game.AI.AICaptain`;
- legacy active, HOJ, and generic missile seeker save-state types;
- `Ships.BulkMagazineComponent.BulkMagazineState`;
- `Ships.DiscreteWeaponComponent.DiscreteWeaponReport`;
- `Ships.FixedDiscreteWeaponComponent.FixedDiscreteWeaponState`;
- `Ships.RezFollowingMuzzle.InstanceSpawned`;
- `Ships.Serialization.PersistentComponentState`.

Current equivalents include `Game.Reports.DiscreteWeaponReport`,
`Ships.FollowingInstanceMuzzle.InstanceSpawned`, XML/bulk-save seeker state, and
`Ships.SaveGame.SavedComponentMagazineState`. The remaining AI and save references should be
deleted with the obsolete patches, not mechanically renamed.

## Serialized failures

The bundle loader reports missing scripts or failed component construction for nine custom
families:

1. `Munitions.CustomCommandGuidedSeeker`;
2. `Munitions.GuidedSplashingShellMunition`;
3. `Munitions.ModularMissiles.Descriptors.Warheads.ClusterWarheadDescriptor`;
4. `Ships.CustomBehaviorThrusterPart`;
5. `Ships.MACFixedDiscreteWeaponComponent`;
6. `Ships.MultipleEjectorTubeLauncherComponent`;
7. `Ships.RestrictedBulkMagazineComponent`;
8. `Ships.Shield.ShieldComponent`;
9. `Ships.TripleMACFixedDiscreteWeaponComponent`.

Consequences observed in the registered assets include:

- the Covenant plasma torpedo silo has no `HullComponent`;
- the Covenant plasma torpedo launcher has no `HullComponent`;
- the Sono-pattern plasma shell has no `IMunition`;
- three Covenant shield components have no `HullComponent`;
- four UNSC spinal weapon assets fail their weapon component;
- custom behavior thrusters disappear from the affected hulls.

Other custom types sometimes instantiate because the assembly remains resident after the caught
loader exception, but `ModEntryPoint.PostLoad()` does not complete. That partial state is not a
supported fallback.

## Direct serialized coverage

A second-pass Unity serialization scan inspected 28,159 bundle objects and 6,833 serialized
`MonoBehaviour` objects. It found exactly 52 distinct `halo.dll` script identities with at least one
use. The runtime prefab dump exposes only 34 because failed prefab construction and
UI/settings/network-only objects do not all become registered runtime prefabs.

The detailed per-bundle and per-type proof is in `coverage-audit.md`. In particular, the undeclared
`expandedmountdisplay` bundle contains no `halo.dll` script identity, while the declared `unsc` and
`covenant` bundles account for all 52 types.

## Runtime architecture that must be removed

`ModEntryPoint.PostLoad()` regenerates serializers, patches the whole assembly, loads an old UI
bundle, and mutates prefab private fields at runtime by cloning stock values. This makes the mod
dependent on field names and initialization order from a much older game build.

The modernization target authors current values into rebuilt prefabs and limits Harmony to narrow,
opt-in adapters:

- socket rules only when a rule component exists;
- shield damage only when a modern shield is installed;
- no global faction, AI, fleet editor, point-cost, serializer, or resource-pool replacement.

## Native systems now available

The current game already supplies:

- selectable and fixed submunition warheads, including capacity, volume, cost, spread, detonation
  modes, release timing, salvo registration, targeting, pooling, and multiplayer spawning;
- modular command seeker descriptors with waypoint support;
- current fixed discrete and continuous weapon components;
- current tube and cell launcher programming queues;
- current bulk magazines with configurable loadout, transfer, persistence, and point-cost support;
- faction locks and current component/fleet validation;
- current resource pool summaries and current save-state APIs.

These native systems are the default migration target. AGMLIB code exists only for the behavioral
delta that remains.

## Evidence limits

The investigation used the subscribed payload, direct Unity bundle serialization, the installed
managed assembly, all 170 decompiled legacy source files, and a live prefab dump. It has not rebuilt
the Unity bundles because the source Unity project is not present in this workspace. Consequently:

- AGMLIB code is compiled but not yet rebound into a rebuilt bundle;
- shield remote-client VFX/order integration is intentionally marked for bundle/network work;
- balance values are preserved as migration inputs, not declared correct for the current meta;
- model-space armor textures must be regenerated from source meshes.

## Deep-pass runtime refresh

After the compatibility corrections, AGMLIB 6.2.2.953 was deployed and the game was cold-started.
The new prefab snapshot was generated at `2026-07-31T04:42:55Z` against the same game build. It
contains 515 total prefabs, 92 from local Halo, and zero dump errors. Local AGMLIB loaded
successfully, all new stats were collected, and the log contains no Harmony patching,
missing-method, or type-initializer exception.

Halo remains `LoadedWithMinorErrors` for the known old-assembly and missing-primary-component
failures. That unchanged result is expected: compile-ready replacement types do not rewrite the
assembly-qualified `MonoScript` references already serialized into the old bundles.

## Runtime-shim update

The later local POC shim supersedes the unmodified-legacy-assembly result above without changing
the frozen evidence. It supplies the exact serialized `halo.dll` identity, restores all 52 façade
types, and reaches `PostLoad` on the current game.

AGMLIB 6.2.2.971 plus the current shim reports:

- 18 hull silhouette repairs;
- six UNSC hull-audio repairs;
- six embedded-drive-to-native-socket repairs;
- nine grouped-audio normalizations;
- eight missing component-material repairs.

The first live VLS-0-43 install exposed null dereferences in current armor-UV handling and then
current LOD hull-paint handling. Both came from a null material entry serialized by the old bundle,
not from a renamed `_originalMaterials` field. The shim now reconstructs those eight entries from
the closest current vanilla component and retains narrow null guards as defensive fallback.

VLS-0-43 and VLS-2 now install into a fresh Paris without an NRE or magenta rendering. A fresh
Gladius creates without a hard crash and advertises its migrated built-in `Fusion Drive` through
the current propulsion discovery path. All nine hull selector rows now show icons.

The Covenant editor path also passes its first deep runtime gates. A fresh Mutan retains its
authored lances, bridge, bay, and drive. Its 3x3x3 module palette contains the Corvex, Morelia, and
Tantalus DFGs, and Corvex installs. The Plasma Torpedo Launcher appears once on the authored `TM1`
torpedo mount, remains absent from the general `MT2` mount, and installs without an NRE.

Loading the saved Ester/Mutan fleet exposed a separate AGMLIB renderer defect: the cached
`SocketOutlineManager` array could contain Unity-destroyed `HullSocket` references. Sorting that
array by `transform` produced a frame-repeated NRE. The renderer now filters invalid references
before both render passes. The same saved fleet then loaded with zero NREs and entered Testing
Range against the stock 266-point fleet.

All nine Halo hulls had a legacy `GroupedAudioSource` with `_simpleSource` populated while
`_simpleSoundEffect` was null. Current vanilla either populates both simple fields or leaves both
null for a bookended-only group. The shim normalizes the legacy combination to the latter form.
The final range session spawned both ships, exercised their runtime effect startup, and remained
responsive with zero `NullReferenceException`, `GroupedAudioSource`, `CustomBehaviorThrusterPart`,
or socket-render stacks. Two duplicate C21/C22 decoy AssetId errors remain from other installed
content and are not Halo/shim failures. See `poc-shim.md` for the exact map and validation record.
