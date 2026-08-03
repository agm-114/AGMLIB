# Asset and bundle rebuild

## Manifest and dependency

Create a new manifest version targeting the current game. Declare the centralized AGMLIB Workshop
item as a dependency. Remove these files and references from the release:

- `halo.dll`;
- bundled `0Harmony.dll`;
- the old expanded-mount UI bundle unless a current UI audit proves it is still required;
- serializer registration and `ModEntryPoint.PostLoad()` bootstrapping.

Do not change public save keys merely to make the migration convenient. Preserve faction, hull,
component, munition, missile-body, and missile-component save keys where the semantic asset remains.

## Rebind procedure

For every prefab in both source bundles:

1. open the prefab in a Unity project built against the current game authoring assemblies and the
   same AGMLIB build intended for release;
2. record the old script and all serialized values;
3. remove the missing/legacy component;
4. add either the current native component or the mapped AGMLIB component from
   `script-inventory.md`;
5. copy values deliberately, translating units, enum meanings, and save keys;
6. bind every reference again; do not trust a retained object reference after changing script
   identity;
7. apply the prefab and reopen it to prove serialization survived;
8. validate the prefab before building the bundle.

Under the repository's interim [AssetBundle production rule](../../.agents/guides/asset-bundle-authoring.md),
do not use nested AGMLIB-defined serialized classes or structs for authoring data. The new scripts
use flat serialized fields for this reason.

## High-risk asset migrations

### Submunition warheads

Re-author all four old cluster descriptors as `ModernSubmunitionWarhead` or directly as native
`SelectableSubmunitionWarheadDescriptor`.

- preserve selected submunition compatibility tags, equal/greater-size policy, capacity curve,
  volume behavior, discount, cost modifiers, spread options, detonation range, and release times;
- map old automatic staging to the native end-of-path/target-acquisition choice;
- map early-staging distance to native acquisition distance where semantics match;
- discard mixed per-warhead magazines for the first release;
- remove `ClusterMagazineAmmoItem`, `ClusterWarheadSettings`, `SettingsClusterLoadout`, missile
  editor patches, custom serializer patches, and the custom Cobyla optimizer.

If mixed loadout is later restored, implement it as a separate AGMLIB descriptor/runtime extension
with explicit per-entry save data and tests. Do not put it back into the first compatibility
release.

### Weapons

- Rebind the four failed UNSC spinal weapons to `MultiModeFixedWeapon` and
  `MultiModeChargingRezzingMuzzle`.
- Copy each MAC profile's magazine size, reload/recycle timing, charge delay/effect, and temporary
  firing modifiers. Leave the inherited/base temporary firing effect array empty so an authored
  mode is not applied twice.
- Rebind the Covenant plasma torpedo launcher to `MultiEjectorTubeLauncher`; copy
  `_launchesPerLoad`, `_withdrawPerLaunch`, and both ejectors. Verify full bursts for both path and
  track programming.
- Use native fixed discrete/continuous components for plasma cannon/lance assets wherever their
  behavior is now equivalent; retain only VFX and resource adapters.
- Use native `RezzingMuzzle`, following-instance muzzle, beam muzzle effects, and barrel glow where
  possible.
- Configure an `AmmoCompatiblity`/`SimpleFilter` on a native `BulkMagazineComponent` instead of the
  old restricted-magazine reimplementation.

### Shields

Rebind three shield components to `ModernShieldComponent` plus `ShieldHitVisuals`. Replace the six
legacy `ShieldComponentHolder` instances on `Shield Collider` and `Shield Collider 2` with
`ShieldHitSurface`, and bind the owning shield explicitly. Do not place this marker on ordinary hull
colliders. Author shield capacity and the legacy missile multiplier directly; do not replace the
ship's native `ResourcePool`. Configure ordinary `ResourcesRequired` for steady power draw.

Keep the marked collider enabled only while its shield surface is meant to intercept hits. Prove
that armor-only impacts ricochet without consuming shield health and that ordinary damage consumes
the attacker's armor-penetration capacity when the shield absorbs it.

Before release, add a current order/action-menu integration and a network VFX relay keyed by ship
net ID plus component RPC key. Gameplay absorption remains host authoritative. Remote clients need
only replicated toggle/integrity/hit presentation.

### Seekers and munitions

- Modular command missiles should use the native command seeker descriptor and current cruise
  guidance descriptor.
- Legacy spawned command missiles that still need position/TRP behavior can use
  `ModernCommandGuidedSeeker`.
- Replace `GuidedSplashingShellMunition` with current lightweight splashing-shell authoring where
  possible. If a spawned guided shell is required, rebuild it as a current missile rather than
  restoring the removed guided-shell inheritance chain.
- Re-author plasma torpedoes on current missile/seeker/warhead components and remove seeker
  save-state patches. Use `ModernEvasiveCruiseMissile` only on the two torpedoes that retain
  authored weave/corkscrew behavior; copy terminal mode and start/end distances.
- Re-author ordinary lightweight MAC shells directly as current `LightweightKineticShell`.
- Rebind the one legacy debuff MAC shell to `ModernDebuffKineticShell`. Copy the debuff reference,
  trigger mode (`Never` = no debuff reference, `StructureBroken` = structure gate, `Always` =
  ungated), debuff radius, shield-depletion toggle, and inherited native cast/structure/damage
  fields.

## Bundle validation

After building:

- scan every serialized `MonoScript` and prove no assembly name is `halo.dll`;
- list every AGMLIB script identity and compare it with `script-inventory.md`;
- load with only centralized AGMLIB and the rebuilt package enabled;
- cold-restart and repeat to catch editor/runtime cache dependence;
- dump all prefabs and require 92 expected registered assets, zero dump errors, and no missing
  `HullComponent`, `IMunition`, or descriptor on the nine formerly failing families;
- compare save-key sets before and after and document intentional additions/removals.
