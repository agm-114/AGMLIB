# Component authoring

Use this guide for new `MonoBehaviour`, `ScriptableObject`, native component
subclass, descriptor, runtime behavior, editor component, or serialized support
type.

## Component record

Every component needs a durable record containing:

1. full type name and source path;
2. category and owning feature;
3. whether it is public, serialized, networked, cloned, pooled, or save-keyed;
4. serialized fields with valid ranges, required references, and defaults;
5. authoring lifecycle and runtime lifecycle;
6. native types, private members, Harmony targets, and prefabs it depends on;
7. authority and replication responsibilities;
8. created objects, subscriptions, callbacks, registries, and cleanup owner;
9. known conflicts and failure behavior;
10. minimal setup example and evidence from editor/runtime tests.

The generated component list is `planning/generated/component-inventory.md`.
It is deliberately broad and must be refined against built assembly metadata
before becoming the compatibility baseline.

## Serialization and cloning

- Treat public type names, namespaces, serialized fields, save keys, enum numeric
  values, and XML identities as compatibility contracts.
- Give public serialized enums explicit numeric values.
- Do not serialize nested AGMLIB-defined custom classes or structs as authoring
  payloads. They can appear valid in Unity's editor but have been observed to
  become null or invalid after loading the built AssetBundle.
- For complex data, use fields or lists of already proven Unity/native
  serializable types, flattened node/edge data with stable integer indices, or
  separately attached components and references. Validate list alignment,
  indices, roots, cycles, duplicate IDs, and required references.
- A representation is not considered supported merely because it survives an
  editor domain reload. It must survive the released AssetBundle build/load path.
- Runtime missile/component behaviors are often authored on finalized patterns
  and then cloned or pooled. Private descriptor and component references needed
  by a live instance should be `[SerializeField]`, or rebuilt in
  `OnUnpooled`/`OnLaunched`.
- A typed descriptor field should have a narrow fallback that recovers from the
  base descriptor reference when null.
- Separate immutable authoring configuration from mutable runtime state.
- Reset timers, collections, cached owners, targets, subscriptions, and flags on
  every applicable pool/unpool/launch path.

## Native and Harmony boundaries

- Verify native behavior against the current assembly and relevant prefab.
- Use cached typed `Internals()` accessors for known non-public native members.
- Keep Harmony prefix/postfix/transpiler methods small; call typed feature
  helpers after availability, lifecycle, configuration, and authority checks.
- Preserve vanilla behavior by default. If a patch must replace it, document the
  invariant it reimplements and add a regression fixture.
- A missing native target should disable its owning feature with an actionable
  diagnostic, not leave unrelated features partially initialized.

## Network authority

Record which side owns:

- validation and command acceptance;
- ammo/resource consumption;
- spawning and despawning;
- target selection and random choices;
- damage, debuff, and state mutation;
- UI/presentation;
- replicated fields and RPCs.

Gameplay mutations normally belong on the server/host. Visual presentation may
run on clients, but it must not duplicate damage, resource, or spawn effects.

## Ownership and teardown

For every event, coroutine, delayed call, sidecar, dynamically created
`GameObject`, material, mesh, texture, and cache, identify the owner and teardown
path. Sidecars should be idempotently attached to the narrowest native owner and
use Unity ownership when possible. See `AGMLIB/Common/Sidecars.md`.

## Diagnostics

Use a stable feature prefix and include the native target or component identity,
the state transition, and the action taken. Do not log every `Update`,
`FixedUpdate`, sensor tick, or projectile interaction. Repeated failures need
rate limiting or one-time suppression.

## Completion checklist

- inventory and user docs updated;
- compatibility impact reviewed;
- configuration validation added;
- pure logic covered without game launch where possible;
- editor/prefab path exercised;
- clone/pool/launch/save/reload paths exercised when applicable;
- offline, host, client, and dedicated-server ownership recorded;
- native targets verified against the pinned assembly;
- logs inspected and no high-frequency noise introduced.
