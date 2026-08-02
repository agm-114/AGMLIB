# Halo Workshop modernization plan

This folder is the implementation handoff for modernizing the public Workshop package at item
`3001949201` from its 0.3.2-era runtime to the currently installed NEBULOUS build.

## Baseline and target

- Workshop package: Halo 2.0.1.1, 166,695,003 bytes installed.
- Legacy manifest game version: 0.3.2.
- Test game: 0.6.2.5 (`260727-1341`).
- Test AGMLIB: 6.2.2.971 for the local runtime-shim validation; the initial prefab snapshot used
  6.2.2.942 and the compile-only deep pass used 6.2.2.953.
- Original load result: `LoadedWithMinorErrors`; the assembly failed type enumeration and nine
  serialized runtime families failed to instantiate. The local POC shim historically demonstrated
  `PostLoad` with all 52 identities present, saved Halo ships in the Fleet Editor, and a live
  Testing Range with the documented compatibility repairs.
- POC distribution status: source-only and disabled by default as of 2026-08-02. Its generated and
  locally deployed `halo.dll` were removed. Never include the shim in an AGMLIB distribution.
- Target dependency model: rebuilt content bundles plus the centralized AGMLIB Workshop dependency;
  no bundled `0Harmony.dll`, `halo.dll`, or private AGMLIB copy.

The old bundles cannot be repaired by adding classes with matching names to AGMLIB. Unity records
the old `halo.dll` assembly identity in every `MonoScript`. Every retained custom script must be
removed and re-added from AGMLIB in the Unity authoring project before the bundles are rebuilt.

## Plan map

- [Current evidence](current-state.md) records the reproduced failures and compatibility boundary.
- [Coverage audit](coverage-audit.md) proves the bundle, serialized-script, decompiled-source, and
  implementation coverage counts.
- [Script inventory](script-inventory.md) maps all 52 serialized custom types to a native or AGMLIB
  replacement.
- [Legacy file atlas](legacy-file-atlas.md) accounts for all 170 decompiled source files and patch
  families.
- [Hull modernization](hulls.md) gives the per-hull armor, collider, socket, and systems work.
- [Asset rebuild](asset-rebuild.md) is the Unity/bundle migration checklist.
- [Implementation plan](implementation-plan.md) defines phases, ownership, migration, and exit
  criteria.
- [Test matrix](test-matrix.md) covers editor, runtime, save/load, host/client, and regression tests.
- [Runtime POC shim](poc-shim.md) records the local compatibility bridge, exact runtime repairs,
  launcher NRE root cause, vanilla material mappings, live validation, and remaining POC gates.

## Implemented AGMLIB slice

The compile-ready replacement folder is
`AGMLIB/Compatibility/HaloModernization`. It currently contains:

- a profile over native `SelectableSubmunitionWarheadDescriptor`;
- a current command seeker extension without private reflection;
- a current lightweight kinetic shell extension for overload debuff and shield depletion;
- a current cruise-missile extension for plasma-torpedo terminal evasion;
- a current `BaseTubeLauncherComponent` multi-ejector implementation that launches the complete
  authored burst from either path or track programming;
- a native-state-based multi-ammo fixed weapon and charging muzzle, including per-mode magazine
  stats and temporary firing modifiers;
- host-authoritative shield interception restricted to explicitly marked shield colliders, save
  state, missile/armor-only handling, and local VFX hooks;
- socket whitelist/blacklist enforcement;
- material, cap, VFX, glow, rotation, resource-emissive, crew-label, socket-scale, and turret-base
  adapters;
- documentation explaining the required bundle rebind.

The existing reusable `Ships.CustomBehaviorThrusterPart` was also brought up to current `IThruster`
lifecycle expectations.

## Definition of done

The modernization is releasable only when:

1. no asset in either rebuilt bundle references `halo.dll`;
2. all 92 registered assets load without a missing-script or loader exception;
3. all nine hulls pass the hull checklist and fleet editor validation;
4. old fleet and missile designs either migrate deterministically or fail with a precise message;
5. every authoritative feature is tested host-only and with one remote client;
6. a fresh prefab dump reports zero errors, and a cold restart plus save/load reproduces the same
   state;
7. the release manifest declares centralized AGMLIB and contains no bundled Harmony.

No commit is part of this work.
