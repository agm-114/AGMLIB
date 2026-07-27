# Turreted discrete weapons using modular missile ammunition

This guide covers a gun-style turret that uses
`TurretedDiscreteMagWeaponComponent` for targeting and magazine behavior, then
redirects each discrete shot through native `MissileEjector` cells so a
finalized modular missile is launched correctly.

It assumes familiarity with authoring vanilla turret controllers, discrete
weapons, muzzles, missile ejectors, and modular missiles. Only the AGMLIB-specific
setup and relevant limitations are covered here.

## Support status

**Experimental: the setup exists in AGMLIB, but still needs in-game testing.**

The configuration has known defects and has not yet been tested as a complete
weapon prefab in single-player and multiplayer. Check the known issues below
before using it in a released mod.

## Runtime path

```text
WeaponComponent.FixedUpdate (host)
`-- TurretedDiscreteWeaponComponent aims and passes traversal checks
    `-- DiscreteWeaponComponent.OnTarget
        `-- AGMLIB DiscreteWeaponComponent.OnTarget prefix
            `-- DiscreteWeaponEjectors.OnTarget
                |-- advances discrete recycle/reload state
                |-- withdraws one missile from the ammo feed
                `-- native MissileEjector.Fire
                    |-- clones the finalized modular-missile pattern
                    |-- applies local and network ownership
                    |-- programs the current track
                    `-- calls IMissile.Launch
```

The bridge works for a turreted discrete weapon because
[`TurretedDiscreteMagWeaponComponent`](../../AGMLIB/Generic%20Gameplay/Discrete/TurretedDiscreteMagWeaponComponent.cs)
inherits the native `DiscreteWeaponComponent.OnTarget` implementation. The
generic
[`DiscreteWeaponComponent.OnTarget` patch](../../AGMLIB/Generic%20Gameplay/GenericWeaponPatches.cs)
therefore sees the turreted weapon and delegates missile ammunition to
[`DiscreteWeaponEjectors`](../../AGMLIB/Generic%20Gameplay/Discrete/DiscreteWeaponEjectors.cs).

## Prefab hierarchy

```text
Weapon prefab root
|-- TurretController
|-- TurretedDiscreteMagWeaponComponent
|-- DiscreteWeaponEjectors
|-- AmmoCompatiblity
|-- SimpleFilter
|-- turret traverse transform
|   `-- barrel elevation transform
|       |-- RezzingMuzzle
|       `-- launch points
|           |-- MissileEjector
|           `-- MissileEjector
```

Put the ejector launch points under the barrel/elevation hierarchy. The bridge
also rotates the selected ejector directly toward the current aim point before
calling the native launch coroutine.

## Configure the turreted weapon

1. Replace the vanilla weapon component with
   `TurretedDiscreteMagWeaponComponent`, retaining the normal
   `TurretController`, muzzle, and turret references.
2. Enable `_requireExternalAmmoFeed` and `_allowAmmoSelection`.
3. Set `_availableVolume` to the internal magazine capacity and initialize
   `_storageRestrictions` with `MunitionType.Missile`. Do not leave the array
   null.

The turret must be functional, authorized, supplied with its required
resources, trained inside its traversal limits, on target, loaded, and outside
its recycle/reload timers before `OnTarget` receives a firing opportunity.

## Keep a spawned-munition muzzle

Keep at least one spawned-munition muzzle, such as `RezzingMuzzle`, in the
weapon's `_muzzles` array.

The muzzle does not launch the missile in this setup. It remains required
because the native `WeaponComponent` uses `_muzzles[0]` during initialization,
range and accuracy display, and ammo simulation-method compatibility checks.

Keep `_muzzles` and `_compatibleAmmoTags` initialized and non-null. AGMLIB's ammo
filter changes the compatibility result only after the native method has read
both fields.

## Configure missile compatibility

Add:

- [`SimpleFilter`](../../AGMLIB/Editor/SocketFilterCore.cs);
- [`AmmoCompatiblity`](../../AGMLIB/Generic%20Gameplay/AmmoFilter.cs), using the
  spelling present in the public component type.

Assign the `SimpleFilter` to `AmmoCompatiblity._filter`.

To accept every missile:

```text
SimpleFilter
  _defaultvalue: false
  _whitelist:
    - Missile
  _blacklist: empty
```

`Missile` matches `MunitionType.Missile.ToString()`. For a narrower launcher,
whitelist a munition save key, munition name, faction key, role, tag class, or
tag subclass instead.

## Configure the ejector bridge

Add `DiscreteWeaponEjectors` on the weapon root or one of its children.

Configure:

- `DiscreteWeaponComponent`: explicitly assign the
  `TurretedDiscreteMagWeaponComponent`;
- `Cells`: add every native `MissileEjector` in firing order;
- at least one non-null cell is required.

Do not rely on automatic weapon discovery when the bridge is under the barrel:
its fallback searches its own children, not its parents.

The bridge's own `AmmoCompatiblity`, `BaseAccuracy`, and `TimeBetweenCells`
fields are not currently used. Configure compatibility on the weapon's attached
`AmmoCompatiblity`, and configure cadence through the discrete weapon's recycle
and reload fields.

## Add modular missile ammunition

In the fleet editor:

1. install the turret;
2. open its magazine loadout;
3. add ammunition;
4. select a compatible modular missile template;
5. add the desired quantity and select it for the weapon.

AGMLIB's
[`SettingsMagazineLoadout` patch](../../AGMLIB/Editor/SocketPatches.cs) exposes
compatible templates. The native editor converts the selected template into the
fleet's finalized `ModularMissile` instance before it is stored in the
magazine. The magazine therefore supplies an `IMissile`, not the
`MissileTemplate` wrapper.

Use track-targeting missile avionics. The bridge passes
`CurrentlyTargetedTrack()` to the ejector but does not build a waypoint path for
a position-only order.

## Known blockers and defects

| Severity | Evidence | Defect | Effect or workaround |
|---|---|---|---|
| Authoring blocker | **Verified in source and by [Unity's issue tracker](https://issuetracker.unity3d.com/issues/the-same-field-name-is-serialized-multiple-times-in-the-class-or-its-parent-class-error-is-thrown-when-private-variables-are-declared-in-the-class-and-its-parent-class-using-underscore); local AssetBundle reproduction still required** | `TurretedDiscreteMagWeaponComponent` redeclares serialized `_traverseRate` and `_elevationRate` fields already declared by native `TurretedDiscreteWeaponComponent`. | Unity reports duplicate serialized field names in an inheritance hierarchy, and AGMLIB's shadow fields are not the fields used by the native turret stat values. This should be fixed before authoring the component. |
| Launch blocker | **Verified in source** | `DiscreteWeaponEjectors` indexes `Cells[_index]` without checking that the list exists, contains an entry, or that the entry is non-null. | An empty or broken list throws before launch. Assign at least one valid cell. A code fix should validate before advancing weapon state. |
| Launch blocker | **Verified in source** | `DiscreteWeaponEjectors.SkipPatch` dereferences `DiscreteWeaponComponent` before validating it. Its automatic fallback searches children only. | Explicitly assign the turreted weapon reference. A bridge placed below the weapon can otherwise throw instead of firing. |
| Editor/load blocker | **Verified in source** | `TurretedDiscreteMagWeaponComponent.RestrictionCheck` reads `_storageRestrictions.Length` without a null check. | Serialize a non-null array containing `MunitionType.Missile`. |
| Salvo correctness | **Verified in source and native assembly** | Every shot passes hard-coded salvo ID `0`; native missile orders allocate IDs with `ActiveMissileSalvo.NextSalvoID`. | Concurrent or overlapping shots can be merged into one native salvo, corrupting salvo UI, reports, communications state, and submunition ownership. There is no authoring workaround; the bridge needs to allocate or receive the correct salvo ID. |
| Targeting limitation | **Verified in source** | Only the current track is forwarded. No position path or dogleg is supplied, and launch uses `immediateSearching: false`. | Position-only orders can launch an unprogrammed missile. Use track-capable avionics and track orders until the bridge implements the other launch forms. |
| Multiplayer presentation | **Inferred from the verified host gate and call path; client test required** | Weapon firing runs on the host, but the bridge calls `MissileEjector.FireEffect(true)` directly instead of a weapon/ejector RPC. | The missile itself should replicate through native spawning, but remote clients may miss door, particle, and sound effects. |
| Presentation defect | **Verified in source** | The bridge always calls `FireEffect(true)`. | It always selects hot-launch presentation even when the ejector performs a cold launch. Author the hot effect as the common effect until fixed. |
| Configuration mismatch | **Verified in source** | `BaseAccuracy`, `TimeBetweenCells`, and the bridge's `AmmoCompatiblity` reference are unused. Accuracy is hard-coded to a zero-width cone. | These inspector values have no effect. Use the weapon recycle fields and its separate ammo filter. |
| Reporting limitation | **Verified in source** | The bridge bypasses the normal muzzle firing/report path and supplies no launch callback. | Discrete weapon shot reports and callbacks may not match the actual missiles launched; native salvo reporting starts only after the missile joins salvo `0`. |

The first three rows can prevent an authored turret from working at all. The
hard-coded salvo ID does not normally prevent the first missile from launching,
but it makes the implementation unsafe for normal multi-weapon and repeated-fire
use.

## Minimal validation

Before treating a prefab as supported:

1. build and reload its AssetBundle, then check `Player.log` for duplicate
   serialized-field or missing-script errors;
2. load one track-guided modular missile into the turret in the fleet editor;
3. verify the selected ammo source is non-null and the live ammo counter
   decreases exactly once;
4. issue a track-target attack and confirm the turret trains before the ejector
   fires;
5. confirm the spawned missile has ownership, a programmed track, an active
   engine/guidance lifecycle, and a unique salvo;
6. repeat with two turrets firing simultaneously to expose salvo-ID collision;
7. repeat as host and remote client, comparing missile spawn, ammo count, door
   animation, particles, sound, and both `Player.log` files.

For the broader missile clone, pool, guidance, and launch lifecycle, see
[`missile-guidance-loitering.md`](../../knowledge/missile-guidance-loitering.md).
