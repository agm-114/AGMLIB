# Ships, components, and damage

All native paths refer to the pinned decompile recorded in
`evidence-ledger.md`.

## Object roles

| Type | Role |
|---|---|
| `Ships/BaseHull.cs` | Bundle/save-keyed prefab and content platform. |
| `Ships/Ship.cs` | Installed design, stat table, resources, magazines, and weapon-group owner. |
| `Game/Units/ShipController.cs` | Network authority and gameplay facade for orders, movement, damage, sensing, launching, docking, and presentation routing. |
| `Ships/HullSocket.cs` | Installation point binding a component to a hull and stat/resource context. |
| `Ships/HullPart.cs` | Damageable and repairable sub-object. |
| `Ships/HullComponent.cs` | Resource-connected part with bundle/save identity, modifiers, debuffs, status, and component lifecycle. |
| `Ships/DamageFrame.cs` | Batched structural/component damage delta replicated for the tick. |

The controller and design are not interchangeable. A feature asking “which
ship?” must state whether it needs the prefab hull, installed `Ship`, live
`ShipController`, socket, or damageable part.

## Damage contract

The central native contracts are:

- `Game/IDamageable.cs`;
- `Game/ISubDamageable.cs`;
- `Munitions/IDamageCharacteristic.cs`;
- `Munitions/IDamageDealer.cs`;
- `Game/HitResult.cs`.

`IDamageCharacteristic` exposes penetration, component damage, heat, brush, and
ricochet/overpenetration characteristics. `IDamageDealer` additionally chooses
component hits, applies component damage, and consumes remaining penetration
capacity.

This division matters: the munition/damager owns the distribution policy; the
ship owns armor geometry, structural fallback, crew pressure, and replicated
damage state.

## Authoritative ship hit flow

Verified from the explicit `IDamageable.DoDamage` implementation in
`Game/Units/ShipController.cs`:

1. return `HitResult.None` when not on the server;
2. record taking-hit/return-fire context;
3. resolve the dynamic armor cell from collider/UV data;
4. evaluate penetration and ricochet;
5. on ricochet, apply reduced armor damage and no heat;
6. on stop or ricochet, apply crew pressure and return;
7. on penetration, consume penetration capacity;
8. calculate interior travel and a possible exit/overpenetration;
9. ask the damager for component hit candidates;
10. route structural-only hits to structure first, or to a live component when
    structure is gone or spread is forced;
11. let the damager apply component damage;
12. accumulate component/structure damage and destruction into a damage frame;
13. derive crew pressure and total-damage state;
14. apply and replicate the frame.

`HitResult` distinguishes `None`, `Ricochet`, `Stopped`, `Penetrated`, and
`Overpenetrated`. AGMLIB impact modules should preserve that vocabulary when
they need to coexist with native armor and reporting.

## Damage frames

`HullPart.DoDamage` accumulates a `DamageFrame.HullPartDamage` rather than
immediately committing every health transition. `ApplyDamageFrame` commits
health, functioning, destruction, and events. The server then sends the frame
to clients.

Do not add a client-side postfix that independently damages a part after seeing
an impact visual. Do not assume a health field has its final value inside every
intermediate damager callback.

## Debuffs

`Ships/HullComponent.cs` owns active debuff state and modifier application.
The vanilla descriptor and instance rules are traced in
[Vanilla prefabs, bundles, resources, and code contracts](vanilla-prefabs-resources.md#debuffs-descriptor-policy-native-instances-and-timed-removal).
AGMLIB's
[`AreaDebuffProfileModule`](../AGMLIB/Munitions/ModularMissile/ModularSystems/AreaDebuffProfileModule.cs)
currently reflects into
`AddDebuffToComponent`, `_activeDebuffs`, and the RPC provider's
`RemoveDebuff`.

Those are known native members and are high-priority typed-accessor candidates.
Timed removal also needs server authority, destruction cleanup, duplicate
instance policy, and save/load behavior defined explicitly.

## Modifier stacking

Native stacking is implemented by:

- `Ships/ShipStatAttribute.cs`, which declares whether a stat is penalized and
  its penalty factor;
- `Ships/StatModifier.cs`, which applies the Gaussian drop-off;
- `Ships/StatValue.cs::CalculateModifier`, which sorts modifier and literal
  contributions by absolute magnitude and applies the penalty independently to
  each sequence.

AGMLIB's `CustomModiferScaling` is a postfix on this private calculation. It
selects the highest-priority scaling component among participating modifier
sources and replaces both totals.

Compatibility requirements:

- keep modifier and literal ordering by descending absolute magnitude;
- preserve the native total-multiplier convention where `1` means unchanged;
- decide deterministically which scaling policy wins when sources disagree;
- treat the `ShipStatAttribute` penalty flag/factor as part of the calculation;
- move `_modifiers` and `Attribute` to typed native accessors;
- add pure tests for positive, negative, tied-magnitude, literal-only,
  non-penalized, clamped, and multiple-policy cases.

## Feature rules

- Damage, debuffs, random hit selection, and modifier mutation are server-owned.
- Presentation should consume native hit/damage results rather than recalculate
  gameplay.
- A sidecar attached to `ShipController` must still discover the correct
  `Ship`, hull, socket, or component owner before mutating state.
- Any new saveable per-component state needs a stable component identity and a
  load phase that tolerates deferred object references.
