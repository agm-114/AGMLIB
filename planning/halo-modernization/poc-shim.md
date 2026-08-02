# Runtime POC shim

## Purpose and boundary

The local `HaloShim` project proves that the public 0.3.2-era bundles can be made usable on
NEBULOUS 0.6.2.5 without rewriting those bundles first. It emits the exact unsigned legacy
assembly identity (`halo`, version `0.0.0.0`) and restores all 52 serialized script identities.
Centralized AGMLIB remains the implementation dependency.

This is a migration bridge, not the release architecture:

- old fleet/save compatibility is intentionally disabled;
- the Workshop source is never modified;
- the local mod copy contains only the shim `halo.dll`;
- runtime repairs are limited to deterministic prefab normalization and narrow null guards;
- the final rebuilt package should author current fields directly and remove the shim.

## Implemented runtime repairs

| Area | Runtime repair | Verified result |
|---|---|---|
| Legacy scripts | Restore 52/52 serialized `halo.dll` identities, inheriting or composing current native/AGMLIB behavior where available. | Halo reaches `PostLoad`; all 92 registered assets remain available. |
| Hull icons | Fill missing normal and HUD silhouettes from each hull's surviving screenshot sprite. | 18 fields repaired; all nine hull rows show icons in the blank-hull selector. |
| Hull audio | Fill six null UNSC `BookendedAudioPlayer` effects from the closest stock hull class. | Six audio bindings repaired during `PostLoad`. |
| Grouped engine audio | When a legacy group has `_simpleSource` but no `_simpleSoundEffect`, clear the invalid simple source and retain its bookended player. | Nine groups repaired; the prior `GroupedAudioSource.CoroutineFadeIn`/thruster NRE does not recur in Testing Range. |
| Embedded drives | Move each old `Ships.HullPartDrive` under the current hull socket root and create/configure its native built-in module socket. | Six drives repaired; Gladius shows a `Fusion Drive` module and `1x Fusion Drive` in the propulsion panel. |
| Missing materials | Replace eight null legacy `_originalMaterials[0]` entries with a compatible current vanilla component material. | Eight material bindings repaired; affected launchers install without NRE or magenta missing-material rendering. |
| Armor material guard | Skip `HullComponent.ApplyArmorUVsForMaterial` only when the material argument is null. | Prevents the old null material from aborting socket installation before normalization can be completed. |
| Hull-paint guard | Skip single-material `ComponentHullPaintLODShared.SetColors` only when its material remains null. | Defensive fallback for an unmapped or irrecoverable legacy component; mapped test assets do not hit it. |
| Restricted component palettes | Preserve blacklist enforcement and allow a legacy requires-whitelist component only when the selected socket carries a whitelist; create recovered rows through native `ComponentPalette.CreateItem`. | Plasma Torpedo Launcher appears once on `TM1`, not on general `MT2`; all three Covenant DFGs appear on the Mutan shield module without duplicate rows. |
| Socket-outline lifecycle | Filter Unity-destroyed socket references before sorting or drawing the cached editor socket array. | The saved Ester/Mutan fleet loads and renders with zero frame-repeated `SocketRendering` NREs. |

## Vanilla material migration map

The old bundles serialize a one-element `_originalMaterials` array whose only entry is null on the
following eight component prefabs. The shim borrows a surviving current material from the closest
vanilla role:

| Halo component | Current vanilla material source |
|---|---|
| `Halo/United Rebel Front/MLS-4 Launcher` | `Stock/MLS-3 Launcher` |
| `Halo/UNSC/E55.2 'Beacon' Twin Illuminator` | `Stock/E55 'Spotlight' Illuminator` |
| `Halo/UNSC/E71.S 'Hush' Spinal Jammer` | `Stock/E71 'Hangup' Jammer` |
| `Halo/UNSC/E90.S 'Blindfold' Spinal Jammer` | `Stock/E90 'Blanket' Jammer` |
| `Halo/UNSC/VLS-0-43 Launcher` | `Stock/VLS-1-46 Launcher` |
| `Halo/UNSC/VLS-1-14 Launcher` | `Stock/VLS-1-23 Launcher` |
| `Halo/UNSC/VLS-2 Launcher` | `Stock/VLS-2 Launcher` |
| `Halo/UNSC/VLS-4 Launcher` | `Stock/VLS-3 Launcher` |

The final asset rebuild should assign an authored Halo material using a current shader and preserve
the source texture set. The borrowed vanilla materials are intentionally acceptable only for the
POC compatibility bridge.

## Reproduced launcher failure and fix

The original VLS-0-43 install failed in two consecutive modern native paths:

1. `HullComponent.ApplyArmorUVsForMaterial` called `HasProperty` on a null material.
2. After guarding that path, `ComponentHullPaintLODShared.SetColors` dereferenced
   `LODGroupSharedMaterial.Material`, which was also null.

The field has not merely been renamed. Current `LODGroupSharedMaterial` still uses
`_originalMaterials`, but modern stock prefabs populate it with a real material while the legacy
Halo prefabs deserialize a null entry. Reconstructing the value is therefore the primary fix; the
two guards remain as fail-closed protection.

## Live validation

Test environment:

- NEBULOUS 0.6.2.5 (`260727-1341`);
- centralized AGMLIB 6.2.2.971;
- deployed AGMLIB DLL SHA-256
  `7C0EB355DBD94BB74E7123B4A0591310B5C7F8E87B82D07BF8ACE0B87895E7CE`;
- deployed shim SHA-256
  `D17FCF60C9FB9DAC7818F65B948F108D91780DF6891954783D8426080D9C8AEE`;
- local Halo shim loaded as `halo, Version=0.0.0.0`;
- `PostLoad`: 52 façade types, 18 icon repairs, six audio repairs, six drive repairs, nine grouped
  audio repairs, and eight material repairs.

Fleet Editor results:

- created a fresh UNSC Paris-class ship;
- installed VLS-0-43 into Mount 3: component remained installed, editor responsive, no NRE;
- replaced it with VLS-2: component remained installed, editor responsive, no NRE;
- repeated both installs after vanilla material mapping: no NRE, no defensive paint warning, and
  no magenta missing-material geometry;
- opened both blank-hull selectors: all six UNSC and all three Covenant rows showed non-null icons;
- added a Gladius: no hard crash, built-in `Fusion Drive` row present, propulsion panel reported
  `1x Fusion Drive`;
- added a fresh Mutan: its four Emri Ka lances, Mutan Et bridge/bay, and Tamran-pattern drive
  remained installed;
- opened the Mutan 3x3x3 module palette: Corvex, Morelia, and Tantalus DFGs appeared, and Corvex
  previewed and installed;
- opened the Ester torpedo socket repeatedly: exactly one Plasma Torpedo Launcher appeared on
  authored `TM1`, it remained absent from general `MT2`, and installation succeeded;
- saved and cold-reloaded the two-ship Ester/Mutan fleet with no editor NRE.

Testing Range results:

- loaded the saved Ester/Mutan fleet against the stock 266-point enemy;
- both ships spawned and the range remained responsive through ship selection, focus, and
  additional runtime time;
- the final log contains zero `NullReferenceException`, `GroupedAudioSource`,
  `CustomBehaviorThrusterPart`, or `SocketRendering` stacks;
- the two visible C21/C22 duplicate decoy AssetId errors and the installed `sovereignincident`
  missing missile-body summary warnings come from other installed content, not Halo or the shim.

## Remaining POC gates

The following work remains before this can be called a broadly playable compatibility build:

1. Install every affected launcher, illuminator, and jammer at least once; verify paint and armor
   handling on each compatible socket.
2. Create each of the nine hulls and exercise every embedded drive under movement and damage.
3. Build representative UNSC and Covenant fleets and launch a skirmish.
4. Fire each custom weapon/launcher family, including dual/triple MAC modes, plasma, shields,
   multi-ejector torpedoes, and submunitions.
5. Verify missile designer warnings, programming, launch, pooling, and host/client behavior.
6. Test component destruction, repair, save/load, and scene unload for retained sidecars or pooled
   state.
7. Replace borrowed materials with current-shader Halo-authored materials in the rebuilt bundles.
8. Remove the POC shim after every retained serialized script is rebound to native or centralized
   AGMLIB components.
