# Native game architecture relevant to AGMLIB

This is a task-oriented map, not a complete description of NEBULOUS. The
exhaustive file/type index and semantic coverage status are recorded in
`research-coverage.md`.

## Ship root and component graph

`Game.Units.ShipController` is a networked root with ownership, orders,
navigation, damage, EWAR hosting, launching, docking, missile-salvo, and UI
responsibilities. Its initialization discovers hull components, hangars, work
slots, sensors, weapons, resources, and carrier systems. It also triggers
resource ticks at several lifecycle points.

`Ships.HullPart` is the Unity damage/repair foundation.
`HullPartResourceConnected` adds resource-system connectivity.
`HullComponent` adds bundle/save identity, modifiers, status, faction locking,
and component lifecycle. Weapon and sensor components build on this graph.

AGMLIB consequences:

- attach per-ship state only after the native component graph exists;
- separate editor cost/resource calculation from runtime allocation;
- avoid scanning the whole scene when the owning `ShipController` or `Ship`
  already exposes the graph;
- resource and weapon patches must tolerate initialization, destruction, and
  save-load paths, not just normal play.

## Weapons

`WeaponComponent` owns ammunition changes and initializes numbered muzzles.
`DiscreteWeaponComponent` has timer/reload/check-fire flow.
`ContinuousWeaponComponent` has check-fire, start-firing, firing, and cooldown
phases. Native weapon state is also routed through `HullComponentRouter` for
network RPC behavior.

AGMLIB custom muzzles should not invent a second authoritative fire decision.
They should preserve native weapon readiness/ammo/resource checks and own only
their additional timing, ray/beam behavior, and presentation state.

## Modular missiles

`Munitions.ModularMissiles.ModularMissile` is simultaneously a missile,
fleet-template design, save-keyed object, munition, EWAR host, launching
platform, selectable entity, network-spawned object, and pooled runtime object.
Missile socket installation calls `MissileSocket.ComponentPermitted` and creates
runtime behaviors whose `OnAdded` methods bind the missile and descriptor.

On live reuse, `ModularMissile.OnUnpooled` participates in reset. Runtime
warheads also receive `OnUnpooled`. `RuntimeMissileWarhead.ShouldFuzeOnTarget`
delegates to its bound descriptor, and the missile only applies the warhead fuse
decision on the server path.

AGMLIB consequences:

- editor socket installation, finalized-pattern cloning, runtime pooling, launch,
  and save/load are distinct phases;
- private runtime descriptor references must survive cloning or be rebuilt;
- fuse/damage/debuff mutation is authoritative;
- a patch to module installation can affect editor, load, and reconstruction,
  not only a click in the missile editor.

## Sensors, signatures, and EWAR

Ships and spacecraft implement EWAR-host contracts. Native sensor tracks are
separate objects with ownership/visibility lifecycle. Active and passive sensor
components derive from hull components and participate in jamming and signature
systems. Signatures are registered objects, not merely values on the UI.

AGMLIB dynamic signatures and SIGINT must therefore distinguish:

- authoring-time component stats;
- registered runtime signatures;
- server-side acquisition and track truth;
- client-visible track/UI state;
- transient ownership and IFF relationships.

## Small craft

Native `SmallCraft.Spacecraft`, `SpacecraftGroup`, hangars, work slots, missile
managers, and EWAR managers form their own networked subsystem. A
command-channel limit should integrate with native reservation, queued
preflight, pad traffic, launch, recovery, postflight, and group state. It should
not reject editor/UI selection merely because the active limit is reached;
native traffic can queue replacement craft.

The detailed launch/order/AI trace is
`small-craft-carriers-orders-ai.md`.

## Bootstrap, content, and save

The main menu drives stock and mod bundle loading through `BundleManager`.
Managed assemblies load before the mod entry point's `PreLoad`; AssetBundles
and addressable catalogs load between `PreLoad` and `PostLoad`. Qualified
bundle/save keys and network spawn keys are compatibility identities.

Live save objects use numeric instance IDs and deferred promises to restore
cross-object references after instantiation. Pooled network objects are
activated before `OnUnpooled`, so per-use state belongs in pool/launch
callbacks, not only `Awake`.

See `boot-content-save-network.md`.

## Fleet and missile editors

Editor palettes, sockets, component lists, fleet templates, and save helpers use
bundle keys, save keys, simple/full type identities, and serialized data.
Socket/filter changes are consequently compatibility changes even when the
runtime component code is untouched.
