# Runtime sidecars and temporary global rollout

A runtime sidecar adds behavior to a native game object without changing that object's type,
prefab, serialized layout, or public contract. Use one when the behavior belongs to a live native
instance but cannot safely be implemented by subclassing it or editing every supplying prefab.

`DecoyAmmoSettings` is one example. Vanilla's decoy command only considers weapon groups whose
fixed `WeaponType` is `Decoy`; a gun does not change its fixed type when it carries decoy
ammunition. The sidecar supplements the command's availability and execution for those guns while
leaving ordinary weapon fire untouched.

The patches remain adapters. Candidate discovery, target filtering, and authoritative execution
live in the typed
[`DecoyAmmoSettings` behavior](../Generic%20Gameplay/DecoyAmmoSettings.cs). Its execution path is
deliberately narrow: temporarily bind an eligible muzzle to a decoy magazine, eject one round
forward, and restore the original binding in a `finally` block. It does not change selected ammo or
take ownership of the native order's lifecycle. This forced path intentionally bypasses the weapon
component's bearing, hull-mask, reload, and firing-cycle gates.

## Ownership and lifetime

- Key a sidecar by the narrowest native live object that owns the behavior.
- Prefer a `MonoBehaviour` attached to the native owner's `GameObject` when Unity should own its
  lifetime. Attach it idempotently with `GetComponent<T>() ?? AddComponent<T>()`, and use
  `[DisallowMultipleComponent]` when duplicates are never meaningful.
- Keep the sidecar typed after any unavoidable patch or reflection boundary.
- Do not store authoring or prefab references that a finalized, cloned, or pooled object will lose.
  Resolve live state from the owner or serialize/rebuild the reference at the appropriate lifecycle
  point.

## Temporary global rollout

Sometimes a sidecar is intended to become opt-in but needs broad coverage while the integration is
being proven. Keep that temporary policy explicit at the sidecar activation boundary and separate
from its execution logic:

- expose one plainly named global gate, defaulted deliberately;
- make every patch consult the same gate;
- keep the sidecar itself scoped per owner and idempotent;
- document the later activation boundary next to the gate;
- when the rollout ends, replace the global gate with an exact component, save key, feature
  registration, or configuration check without rewriting the sidecar.

`DecoyAmmoSettings.EnableGlobally` is currently `true`, so eligible ships receive the sidecar at
runtime. Once that default is false, only ships with an explicitly attached `DecoyAmmoSettings`
participate; the execution logic does not need another rewrite.

## Safety rules

- Query methods must remain free of gameplay side effects. Idempotently attaching the behavior is
  infrastructure; defer ammo switching and other gameplay mutation to the authoritative execution
  method.
- Mutate gameplay state on the server/host. Prefer native APIs so normal replication and bookkeeping
  run.
- If the feature intentionally bypasses a native state machine, keep the bypass narrowly scoped and
  document which validation, timing, effects, and bookkeeping it omits. Do not present that operation
  as an ordinary weapon shot.
- Restore temporary low-level bindings in a `finally` block, including when the forced operation
  throws.
- Make random gameplay decisions only inside the authoritative execution path. Bound retries,
  validate candidates with native domain checks, and fail closed when no valid result is found.
- Preserve vanilla behavior and combine results instead of replacing it.
- Exclude native cases already handled by vanilla; the decoy-ammo sidecar ignores groups whose
  `WeaponType` is already `Decoy`.
- Fail closed on ambiguous state. The decoy-ammo sidecar skips mixed-ammo groups so a decoy command
  cannot accidentally fire ordinary shells from another member of the same group.
- Keep patch entry points small. Version-sensitive method discovery belongs in one resolver and
  should fail clearly if the native contract changes.
- Emit compact event telemetry at the mutation boundary, not per frame.
