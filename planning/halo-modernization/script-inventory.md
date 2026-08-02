# Serialized script inventory

The two declared bundles serialize 52 distinct custom `MonoScript` types from `halo.dll`. A direct
scan found a serialized `MonoBehaviour` use for every one; `coverage-audit.md` records the exact
occurrence counts. This table accounts for all 52. "Native" means the rebuilt prefab should contain
no AGMLIB replacement for that old script. "Implemented" means a compile-ready AGMLIB replacement
exists, but the asset still has to be rebound and tested.

| Legacy serialized type | Disposition | Replacement and migration notes |
|---|---|---|
| `ComplexCrewJobNames` | AGMLIB, implemented | `CrewJobLabels`; its opt-in current `Ship.EditorFormatCrew` adapter rebuilds only the 14 marked assets. Prefer current faction localization when it is sufficient. |
| `CustomBehaviorThrusterPartConfig` | Existing AGMLIB | `CustomBehaviorThrusterPartConfig`; copy direction, attitude influence, and effect flag. |
| `Factions.FactionDescriptionWithoutDefaults` | Native | Current `FactionDescription`; author all default lists explicitly. |
| `FleetEditor.ClusterMagazineAmmoItem` | Remove | Native selectable-submunition UI. |
| `FleetEditor.MissileEditor.ClusterWarheadSettings` | Remove | Native submunition settings panel. |
| `FleetEditor.SettingsClusterLoadout` | Remove | Native single selected submunition for first release. |
| `HullSegmentPatchIndicator` | Remove | Author primary mesh, armor maps, materials, and segment socket lists. |
| `HullSocketBuiltIn` | Remove | Author a default component or explicit built-in policy on the socket. |
| `Munitions.CustomCommandGuidedSeeker` | AGMLIB/native, implemented | Prefer current modular command seeker descriptor; for spawned legacy missiles use `ModernCommandGuidedSeeker`. |
| `Munitions.GuidedSplashingShellMunition` | Native redesign | Rebuild as current lightweight splashing shell or a current missile. The removed guided-shell inheritance chain must not return. |
| `Munitions.LightweightDebuffMACShell` | AGMLIB/native, implemented | `ModernDebuffKineticShell` over the current lightweight kinetic shell; native flight/pooling/cast/structure damage plus overload debuff and opt-in shield depletion. |
| `Munitions.LightweightMACShell` | Native | Current lightweight kinetic shell. |
| `Munitions.ModularMissiles.Descriptors.Warheads.ClusterWarheadDescriptor` | AGMLIB, implemented | `ModernSubmunitionWarhead`, a thin profile over native selectable submunitions. Four descriptor assets need re-authoring. |
| `Munitions.ModularMissiles.ModularMissileDetailStringReplacement` | Remove | Current descriptor/detail text pipeline. |
| `Munitions.PlasmaTorpedo` | AGMLIB/native, implemented | `ModernEvasiveCruiseMissile` retains hot launch and the two terminal-evasion modes while current `CruiseMissile`, seeker, warhead, pooling, and save APIs own the rest. Two runtime prefabs need re-authoring. |
| `RotationFollower` | AGMLIB, implemented | `RotationFollower`, moved to `LateUpdate` to avoid physics-tick visual jitter. |
| `Ships.BerthingComponentFractional` | Native | Current `BerthingComponent`; round the authored crew value deliberately and rebalance. |
| `Ships.ChargingRezzingMuzzle` | AGMLIB, implemented | `MultiModeChargingRezzingMuzzle`. |
| `Ships.ComponentHullPaintLODSharedIndexed` | AGMLIB, implemented | `IndexedComponentHullPaint`. |
| `Ships.CovenantBarrelGlow` | AGMLIB, implemented | `FunctionalBarrelGlow`; event-driven functionality and retained `FireInstant` hook. |
| `Ships.CustomBehaviorThrusterPart` | Existing AGMLIB, modernized | `Ships.CustomBehaviorThrusterPart`; current `IThruster`, controller unbinding, disable/destroy reset, null-safe VFX, lateral-only flank damage. |
| `Ships.CustomLineBeamMuzzleEffects` | Native | Current beam/continuous muzzle effects; move material/VFX references onto the native muzzle. |
| `Ships.FixedEWarComponent` | Existing AGMLIB | `Ships.FixedEWarComponent`; rebind and run the fixed-facing test matrix. |
| `Ships.HullComponentStorage` | Remove | Shield capacity is explicit; ordinary native `ResourcesRequired/Provided` handles power. Do not replace `ResourcePool`. |
| `Ships.HullComponentTileable` | Native | Current `HullComponent._canTile` on a compartment component. |
| `Ships.HullPartDrive` | Native | Author a normal `DriveComponent` in a real `HullSocket`; do not create/inject a socket at runtime. |
| `Ships.HullResources` | Native | Current `HullComponent` resource and stat-modifier lifecycle. |
| `Ships.HullSocketFixedWeaponGuidance` | Native | Current fixed/casemate weapon facing and native weapon-group unmask logic. |
| `Ships.InfiniteRezzingMuzzle` | Native | Current `RezzingMuzzle`/following-instance muzzle; express continuous presentation through current weapon cadence. |
| `Ships.MACFixedDiscreteWeaponComponent` | AGMLIB, implemented | `MultiModeFixedWeapon` with light/heavy profiles, stat-modifiable per-mode capacity/timing, temporary firing modifiers, and replicated-cycle charging VFX. |
| `Ships.MultipleEjectorTubeLauncherComponent` | AGMLIB, implemented | `MultiEjectorTubeLauncher`; native programming queue/cycle/RPC state plus complete authored bursts, per-shot withdrawal, and alternating ejectors. |
| `Ships.PassiveSensorComponentCustomWake` | Native | Current passive sensor/wake signature fields. |
| `Ships.PlasmaCannon` | Native plus adapters | Current discrete weapon, VFX, `FunctionalBarrelGlow`, and native resource demand. |
| `Ships.PlasmaLance` | Native plus adapters | Current fixed continuous weapon and native resource demand. |
| `Ships.PowerUsageEmissive` | AGMLIB, implemented | `ResourceUsageEmissive` reads `IReadOnlyResourcePool`; no storage subscriber or pool mutation. |
| `Ships.RestrictedBulkMagazineComponent` | Native plus existing AGMLIB | Current `BulkMagazineComponent` plus `AmmoCompatiblity`/`SimpleFilter` keyed by current save keys. |
| `Ships.Shield.ShieldComponent` | AGMLIB, gameplay implemented | `ModernShieldComponent`; host-authoritative exact-surface absorption, armor and missile behavior, cooldown, toggle core, save state, and opt-in damage patches. Network order/VFX binding remains an asset integration gate. |
| `Ships.Shield.ShieldComponentEffects` | AGMLIB, implemented locally | `ShieldHitVisuals`; bind current VFX property/event names and add a network relay before release. |
| `Ships.Shield.ShieldComponentHolder` | AGMLIB, implemented | Replace all six old holders with `ShieldHitSurface` on the shield collider objects and bind their owning `ModernShieldComponent`. Ordinary hull colliders must not receive this marker. |
| `Ships.Shield.ShieldNetworkBehavior` | Replace during asset integration | A small current network relay for toggle/integrity/hit presentation; gameplay stays in `ModernShieldComponent`. |
| `Ships.ShieldComponentHullPaint` | AGMLIB, implemented | `EmissiveComponentHullPaint`. |
| `Ships.SocketCapRemover` | AGMLIB/native, implemented | Prefer native `HullComponent.RemoveSocketCap`; use `SocketCapController` only for the named exception. |
| `Ships.SocketRestrictor.ComponentRequiresWhitelist` | AGMLIB, implemented | `ComponentRequiresSocketWhitelist`. |
| `Ships.SocketRestrictor.SocketComponentBlacklist` | AGMLIB, implemented | `SocketComponentBlacklist`, keyed by stable component save key. |
| `Ships.SocketRestrictor.SocketComponentWhitelist` | AGMLIB, implemented | `SocketComponentWhitelist`, keyed by stable component save key. |
| `Ships.TripleChargingRezzingMuzzle` | AGMLIB, implemented | The same `MultiModeChargingRezzingMuzzle` handles light, heavy, and super-heavy modes. |
| `Ships.TripleMACFixedDiscreteWeaponComponent` | AGMLIB, implemented | The same `MultiModeFixedWeapon` with both tag lists and all three stat/effect profiles. |
| `Ships.TurretedContinuousWeaponComponentExtra` | Native | Current turreted continuous weapon. Re-author stats and effects on the native component. |
| `Ships.TurretedDiscreteWeaponComponentExtra` | Native | Current turreted discrete weapon. Re-author stats and effects on the native component. |
| `SocketComponentScaler` | AGMLIB, implemented | `SocketComponentScaler`; its opt-in socket postfix applies the eight authored per-component visual scales after a successful surface-socket install. This is visual scaling, not point/cost scaling. |
| `SyncVisualEffect` | AGMLIB, implemented | `VisualEffectStateFollower`. |
| `TurretHideBase` | AGMLIB, implemented | `TurretBaseVisibilityMarker` plus `TurretBaseVisibilityAdapter` hide the configured base hierarchy only when a weapon is installed in one of the four marked sockets. |

## Nine immediate loader failures

| Failed family | First working migration |
|---|---|
| custom command seeker | Native modular command seeker or `ModernCommandGuidedSeeker` |
| guided splashing shell | Current lightweight splashing shell/current missile |
| cluster warhead | `ModernSubmunitionWarhead` |
| custom thruster | modernized existing AGMLIB thruster |
| dual-mode MAC | `MultiModeFixedWeapon` |
| multi-ejector tube | `MultiEjectorTubeLauncher` |
| restricted bulk magazine | native bulk magazine plus AGMLIB filter |
| shield | `ModernShieldComponent` plus VFX/network integration |
| triple-mode MAC | `MultiModeFixedWeapon` |

## Functionality intentionally not carried forward

- mixed submunition magazine editing in the first release;
- runtime cloning of private fields from stock prefabs;
- replacing or subclassing the native `ResourcePool`;
- global AI target assignment and return-fire overrides;
- global faction-useability patches;
- custom generic serializer registration;
- expanded-mount UI patches unless a current editor test demonstrates a real missing capability;
- old save-state base classes and reflection-based state injection.

These deletions are modernization decisions, not forgotten scripts. The file atlas records the
legacy source that implemented them.
