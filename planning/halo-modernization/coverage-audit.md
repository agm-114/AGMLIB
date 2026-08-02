# Deep coverage audit

This audit is the second-pass proof that the modernization work covers the complete subscribed
Workshop payload, not only the objects visible in a successful runtime prefab dump.

## Frozen inputs

| Input | Evidence |
|---|---|
| Workshop item | `3001949201`, public Halo 2.0.1.1 package |
| Workshop and local payload | 12 files, 166,695,003 bytes; the subscribed and local copies match |
| Legacy assembly | `halo.dll`, SHA-256 `3920E80C83C49A428CFA920431E3C4C25077D2486E67D942BD4E48337D022856` |
| Legacy game target | 0.3.2 |
| Current game | 0.6.2.5 (`260727-1341`) |
| Current managed assembly | `Nebulous.dll`, SHA-256 `651B9CFBAB8D5411B7D241EB176F640F11BFEA8AAF5E8674093C9AE3198B5CF2` |
| Deep-pass AGMLIB build | 6.2.2.953 |

The active investigation order is local AGMLIB, local Halo, Workshop AGMLIB, then the pre-existing
test mod. The duplicate AGMLIB entry is pre-existing local state. Halo is immediately below local
AGMLIB as requested.

## Bundle-object scan

A direct Unity serialization scan read all three package bundles, independent of whether the
current game could instantiate each object.

| Bundle | Objects | `MonoBehaviour` objects | Distinct Halo script types | Manifest status |
|---|---:|---:|---:|---|
| `unsc` | 17,032 | 4,439 | 21 | Declared |
| `covenant` | 11,061 | 2,374 | 37 | Declared |
| `expandedmountdisplay` | 66 | 20 | 0 | Not declared |
| **Total** | **28,159** | **6,833** | **52 distinct combined** | |

`expandedmountdisplay` is present in the payload but absent from `ModInfo.xml`, so it is not a
registered content bundle. It contains no `halo.dll` script identity. The modernization removes it
unless a current fleet-editor test proves a real missing feature.

The direct scan found exactly 52 distinct `halo.dll` `MonoScript` identities and every identity has
at least one serialized `MonoBehaviour` use. The live prefab dump exposed only 34 of them because
failed prefab construction and UI/settings/network-only objects do not all survive into registered
runtime prefabs. The direct bundle scan is therefore the authoritative serialized-script count;
the 34-type runtime view is only a loadability observation.

## Serialized identity occurrence proof

The occurrence count is the number of serialized `MonoBehaviour` objects bound to each legacy
script identity across the declared content bundles.

| Legacy type | Uses | Legacy type | Uses |
|---|---:|---|---:|
| `ComplexCrewJobNames` | 14 | `CustomBehaviorThrusterPartConfig` | 60 |
| `Factions.FactionDescriptionWithoutDefaults` | 1 | `FleetEditor.ClusterMagazineAmmoItem` | 1 |
| `FleetEditor.MissileEditor.ClusterWarheadSettings` | 1 | `FleetEditor.SettingsClusterLoadout` | 1 |
| `HullSegmentPatchIndicator` | 6 | `HullSocketBuiltIn` | 3 |
| `Munitions.CustomCommandGuidedSeeker` | 3 | `Munitions.GuidedSplashingShellMunition` | 1 |
| `Munitions.LightweightDebuffMACShell` | 1 | `Munitions.LightweightMACShell` | 2 |
| `Munitions.ModularMissiles.Descriptors.Warheads.ClusterWarheadDescriptor` | 4 | `Munitions.ModularMissiles.ModularMissileDetailStringReplacement` | 3 |
| `Munitions.PlasmaTorpedo` | 2 | `RotationFollower` | 4 |
| `Ships.BerthingComponentFractional` | 1 | `Ships.ChargingRezzingMuzzle` | 4 |
| `Ships.ComponentHullPaintLODSharedIndexed` | 18 | `Ships.CovenantBarrelGlow` | 6 |
| `Ships.CustomBehaviorThrusterPart` | 60 | `Ships.CustomLineBeamMuzzleEffects` | 4 |
| `Ships.FixedEWarComponent` | 2 | `Ships.HullComponentStorage` | 5 |
| `Ships.HullComponentTileable` | 1 | `Ships.HullPartDrive` | 6 |
| `Ships.HullResources` | 5 | `Ships.HullSocketFixedWeaponGuidance` | 7 |
| `Ships.InfiniteRezzingMuzzle` | 6 | `Ships.MACFixedDiscreteWeaponComponent` | 3 |
| `Ships.MultipleEjectorTubeLauncherComponent` | 1 | `Ships.PassiveSensorComponentCustomWake` | 1 |
| `Ships.PlasmaCannon` | 6 | `Ships.PlasmaLance` | 4 |
| `Ships.PowerUsageEmissive` | 6 | `Ships.RestrictedBulkMagazineComponent` | 1 |
| `Ships.Shield.ShieldComponent` | 3 | `Ships.Shield.ShieldComponentEffects` | 3 |
| `Ships.Shield.ShieldComponentHolder` | 6 | `Ships.Shield.ShieldNetworkBehavior` | 1 |
| `Ships.ShieldComponentHullPaint` | 3 | `Ships.SocketCapRemover` | 26 |
| `Ships.SocketRestrictor.ComponentRequiresWhitelist` | 7 | `Ships.SocketRestrictor.SocketComponentBlacklist` | 6 |
| `Ships.SocketRestrictor.SocketComponentWhitelist` | 44 | `Ships.TripleChargingRezzingMuzzle` | 3 |
| `Ships.TripleMACFixedDiscreteWeaponComponent` | 1 | `Ships.TurretedContinuousWeaponComponentExtra` | 1 |
| `Ships.TurretedDiscreteWeaponComponentExtra` | 2 | `SocketComponentScaler` | 8 |
| `SyncVisualEffect` | 4 | `TurretHideBase` | 4 |

The 52 rows above match `script-inventory.md` exactly: no serialized type is missing from the
migration map and the map has no invented serialized type.

## Decompiled-source proof

ILSpy 10.1.0 produced 170 C# files from the exact legacy assembly. A filename-set comparison
against `legacy-file-atlas.md` returned:

- 170 actual files;
- 170 atlas entries;
- zero missing entries;
- zero extra entries;
- zero duplicates.

This matters because 118 files are helpers, patches, monitors, save payloads, UI glue, algorithms,
or assembly metadata rather than serialized components. They still affect feature parity. The atlas
maps each of those families to a retained implementation, a current native replacement, an asset
authoring task, or an intentional deletion.

## Asset-context proof

The direct scan also verified the high-risk multiplicities used by the rebuild plan:

- 60 custom thruster parts and 60 matching behavior configs;
- three shield components, three shield effects, and six shield-collider holders on the two
  collider objects used by each shield;
- one plasma torpedo launcher and one restricted plasma torpedo silo;
- three dual-mode and one triple-mode fixed MAC components;
- 44 socket whitelists, six blacklists, and seven requires-whitelist markers;
- eight socket-component visual scalers;
- four turret-base visibility markers;
- six power-usage emissive components;
- 14 custom crew-label components.

The live dump still reports 92 registered Halo assets. Its `LoadedWithMinorErrors` state is evidence
of the old package's incompatibility, not an acceptable release result.

## Corrections made by this deep pass

The second pass found several provisional behaviors that compiled but did not fully preserve the
legacy delta:

1. `MultiEjectorTubeLauncher` now launches the complete authored `_launchesPerLoad` burst for path
   and track programming, validates before withdrawal, honors `_withdrawPerLaunch`, observes native
   cycle/reload gates, alternates ejectors, and cancels safely on ammo change/destruction.
2. `MultiModeFixedWeapon` now has stat-modifiable capacity/reload/recycle values for every profile,
   legacy-compatible stat subtypes, per-mode temporary firing modifiers, and charging VFX driven by
   replicated cycle state. Rebuilt prefabs must leave the base temporary-effect array empty.
3. Shield interception is restricted to an explicitly rebound `ShieldHitSurface`; ordinary hull
   colliders remain on the native damage path. Missile damage uses the legacy 0.2 multiplier,
   armor-only hits ricochet without consuming shield health, and armor penetration capacity is
   consumed on absorbed ordinary hits.
4. `CrewJobLabels`, `SocketComponentScaler`, and turret-base visibility now have narrow opt-in
   consumers rather than inert marker-only classes.
5. Socket palette filtering now rejects requires-whitelist components even on otherwise
   unrestricted sockets, and all rule arrays are null-safe.
6. Resource emissive binding can target current segment armor materials, owns renderer material
   instances correctly, and validates shader properties.
7. The non-`MonoScript` damager chain behind the debuff MAC shell is retained by
   `ModernDebuffKineticShell`: native lightweight damage remains primary, a typed current-internals
   call applies the overload debuff, and an explicit wrapper communicates shield depletion without
   reflecting into native chained-damager queues.
8. The two serialized plasma torpedoes retain their actual custom delta through
   `ModernEvasiveCruiseMissile`, which layers deterministic weave/corkscrew terminal steering over
   the current cruise-missile state, seeker, warhead, pooling, and save behavior.

## Coverage conclusion

The discovery and implementation plan now cover all three payload bundles, all 52 serialized script
identities, all 170 decompiled files, all 92 registered runtime assets, and all nine immediate loader
failure families. The remaining release gates are not undiscovered scripts: they are bundle
reauthoring, exact asset-reference/value migration, shield order/network presentation, save-format
migration, armor/collider baking, and gameplay/balance validation that require the source Unity
project or rebuilt bundles.

## Cold validation result

The final deep-pass deployment produced AGMLIB 6.2.2.953. The repository artifact and deployed DLL
are byte-identical:

`9D77C0DF398DF3B3E374E93949C9193EBD71B21EE3F1A5F49AD5D907E7C35C01`

A cold game launch generated a new snapshot at `2026-07-31T04:42:55Z`:

- game 0.6.2.5 (`260727-1341`);
- AGMLIB loaded successfully;
- all compatibility Harmony targets resolved with no patching, missing-method, or type-initializer
  exception;
- 515 total registered prefabs and zero dump errors;
- 92 registered Halo assets from the local `Mods\Halo` copy;
- Halo remained `LoadedWithMinorErrors` with the same legacy assembly and missing-primary-component
  failures documented in `current-state.md`.

This validates the central AGMLIB assembly and startup boundary. It cannot instantiate the new
compatibility components until the old `halo.dll` script references are removed and the prefabs are
rebound in a rebuilt Unity bundle.
