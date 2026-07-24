# Weapons and munitions

## Native invariants

- `WeaponComponent` owns target mode, ammo source, muzzle index, aim validity,
  and effect/report routing.
- `WeaponComponent.ChangeAmmoType` is the native ammo transition boundary.
- Weapon muzzles are initialized with their owning weapon and an index.
- `CycledComponent` owns the cycle timer and saved cycle state.
- `DiscreteWeaponComponent` owns between-muzzle timing, magazine/reload state,
  muzzle firing, and shell callbacks.
- `ContinuousWeaponComponent` owns burst, cooldown, and single-cycle state and
  starts/stops all muzzle effects.
- `ContinuousRaycastMuzzle` applies damage periodically; its displayed armor
  and component values are normalized to damage per second.
- Missile launchers mainly follow the separate
  `BaseCellLauncherComponent : HullComponent` hierarchy.
- Modular missile socket installation checks `ComponentPermitted`.
- Runtime warhead fuse decisions are consulted on the server.

These are verified-source claims against the assembly in the evidence ledger.

Native evidence:

- `Ships/WeaponComponent.cs`;
- `Ships/CycledComponent.cs`;
- `Ships/DiscreteWeaponComponent.cs`;
- `Ships/ContinuousWeaponComponent.cs`;
- `Ships/Muzzle.cs`;
- `Ships/ContinuousRaycastMuzzle.cs`;
- `Ships/BaseCellLauncherComponent.cs`.

## Charge-up and beam behavior

AGMLIB delayed continuous and rezzing muzzles add a pre-fire phase. The safe
state machine is:

```text
idle -> charging -> committed/fire -> sustaining or impact -> cooldown -> idle
                  \-> cancelled --------------------------/
```

Cancellation must cover target loss, component disabled/destroyed, ammo/resource
failure, owner loss, and pool/reuse. Beam visuals and impact FX need stable
ownership independent of authoritative damage. An unreliable impact effect is
likely in the seam between ray hit production, pooled effect lifetime, and
client presentation; it should be diagnosed with event telemetry rather than
extra per-frame logs.

The smallest useful telemetry chain is: charge accepted, firing committed,
ray hit produced, impact effect requested, effect unpooled, and effect repooled.
Record peer role and weapon/muzzle identity on each event.

`DelayedContinuousRaycastMuzzle` currently mirrors the native raycast loop,
including retaining a pooled `MunitionHitInfo` when a hit occurs before the next
damage interval. Dispose every unused hit result when the implementation is
hardened. Also validate `_damagePeriod > 0`; it is used as a divisor for
per-second stats.

The stock Mk600 and Mk610 prefabs both set `_damagePeriod: 0.2`,
`_beamSpeed: 750`, and reference a `LineBeamMuzzleEffects` graph with continuous
hit effects. That graph explicitly binds hit VFX/audio, muzzle VFX, ramped
audio, the line renderer, and tactical-view effect. Their raycast ranges differ,
so copy the graph contract rather than assuming every serialized beam value is
universal. See `vanilla-prefabs-resources.md`.

## Ammo-mode modules

The modular system exposes callbacks for weapon ammo changes and discrete
check-fire decisions. Cycle, fallback, and resource profile modules should share
one transactional model:

1. resolve the intended mode;
2. validate ammo, launcher compatibility, resources, and authority;
3. reserve/consume only at the native mutation boundary;
4. change ammo through the native API;
5. roll back or select a documented fallback on failure;
6. refresh editor/runtime presentation;
7. emit one state-transition event.

Current source notes:

- the cycle profile reapplies private stat base values and recalculates on every
  discrete `CheckFire`, even when the profile is unchanged;
- `AmmoModeResourceFailurePolicy` is serialized but has no current consumer;
- unknown resource names silently remove the intended requirement;
- cached base cycle/resource values need an ownership rule when multiple
  features can change the same native fields.

## Fuses and submunitions

The current submunition patch treats a transient `IFF.None` relationship as a
reason to skip a proxy-fuse decision. It catches all exceptions and returns to
native behavior. Historical commits show this was a pragmatic fix for a real
parent/submunition interaction.

Planned hardening:

- verify hit object, root, trackable, owner, and proxy-fuse state explicitly;
- move `_proxFuze` to a typed native accessor;
- restrict the special case to a positively identified launcher/child relation;
- log one structured diagnostic on an unexpected state rather than silently
  swallowing all exceptions;
- test parent launcher, friendly non-parent, enemy, neutral/unowned, direct hit,
  proxy hit, and pooled reuse.

## Collision path

`LookaheadMunitionBase.FixedUpdate` can generate a hit through ray lookahead.
Unity collision and trigger callbacks can also build `MunitionHitInfo`; all
paths converge on virtual `ProcessCollision`. `Missile.ProcessCollision` adds
fuze/termination policy, while `ModularMissile` delegates terminal behavior to
its installed warhead.

Effects and gameplay mutation are separate responsibilities. A visible impact
is not proof that server damage ran, and server damage is not proof that a
remote client received an impact effect.

## Impact profile modules

Area debuff, ship-disable, and damage-control-team profiles are server-side
gameplay mutations with timed state components. Their shared helper should own:

- target resolution and distance/falloff;
- stacking/refresh/replace policy;
- immunity and invalid-target rules;
- authoritative application;
- expiry and destruction cleanup;
- compact telemetry.

For the current timed debuff profile, a non-positive duration still adds the
debuff but schedules no removal. Decide whether that configuration is invalid
or explicitly means permanent; do not leave it accidental. Native debuff
refresh/merge behavior must also define whether a second hit extends an
existing timer when no new instance ID is created.

The expanded vanilla registry closes that question. Native duplicate policy is
authored on `ComponentDebuff`:

- non-multiple duplicates produce no new instance, so AGMLIB currently does not
  refresh their timer;
- multiplied stacks reuse one active instance ID, so the first AGMLIB timer
  removes the full severity stack;
- `ForcedDebuffs` still run the native verification method and hit-only/multiple
  gates; they only bypass membership in the target component's normal table;
- native save state persists the debuff and its own damage/spread/event timers,
  but not AGMLIB's `TimedAreaDebuffRemoval` coroutine.

Timed debuffs therefore need an explicit refresh/stack/save policy rather than
instance-ID differencing alone.

## Command seekers and loitering missiles

The focused trace lives in `missile-guidance-loitering.md`.

The most important current compatibility defect is the position-seeker EMCON
patch: it writes a native field named `_requiresComms`, but the pinned
`ActiveMissileSalvo` instead uses the private-set `OverrideComms` property.
Because `Common.SetVal` silently ignores a missing member, the authored
`ForceAllowEmcomLaunch` option is currently a no-op against this assembly.

Position guidance also has retarget, source-selection, position/acceleration,
and pool-reset defects. Ranged command guidance duplicates native private
track/comms ownership instead of extending one state owner. Treat both as
research/testing features until the focused matrix passes.

The modular loitering missile is server-authoritative for target selection and
uses bundle-safe primitive/list authoring data. Its seeker-fuse query is
stateful, however, and its per-socket table, pooled phase state, manual
engagement RPC, and idle network cost need explicit validation.

The three vanilla mine prefabs all use a layer-22 trigger sphere of radius 200,
90-second arming, unlimited loiter life, and a communicator plus antenna. Only
the cooperative M-30-N enables native `_network`, which means cooperative
mine-to-mine triggering rather than Mirror networking. AGMLIB's shorter arming,
finite life, and larger trigger defaults are intentional new policy and must
not be described as vanilla parity. The full field/reference comparison is in
`vanilla-prefabs-resources.md`.
