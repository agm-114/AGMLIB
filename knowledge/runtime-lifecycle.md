# Runtime lifecycle model

## The four object forms

Many AGMLIB features cross four forms of the "same" configured object:

1. **Authoring object** - component or descriptor in an asset/prefab.
2. **Finalized pattern** - assembled design after editor/load processing.
3. **Live clone** - instantiated gameplay object.
4. **Pooled reuse** - a previously live object reset and launched again.

Assuming a private field assigned during authoring remains populated in forms
three and four is unsafe. Serialize durable references or rebuild them in the
native runtime callback.

## Ship/component sequence

The native ship controller initializes a broad component graph, then server
finalization and game start establish runtime behavior. Resource ticks occur
during and after initialization as well as normal runtime. Hull components have
`Awake`, `Start`, and `OnDestroy` hooks.

AGMLIB ship state should define:

- the earliest callback at which its owner and dependencies are valid;
- whether it is editor-only, both editor/runtime, or runtime-only;
- how it behaves if attached after initialization;
- how it unsubscribes and clears static/cache state;
- which actions are server-only.

## Missile sequence

The verified-source sequence is:

1. `ModularMissile.BuildPattern` instantiates the selected body and applies the
   design template;
2. `FinalizePattern` calls every installed descriptor's `FinalSetup`;
3. descriptor setup uses `AddRuntimeBehaviour<T>`, which adds a component,
   invokes `OnAdded`, and records it in serialized runtime behavior lists;
4. `MakeRuntimePrefab` finalizes the network-spawnable pattern;
5. a clone's `Awake` forwards `OnCloned`;
6. pool, unpool, and launch callbacks are forwarded to runtime behaviors;
7. guidance, seeker, fuze, and warhead behavior runs on the live instance;
8. collision processing and gameplay mutation remain server-owned.

Evidence:

- `Munitions/ModularMissiles/ModularMissile.cs`;
- descriptor and runtime types under
  `Munitions/ModularMissiles/Descriptors` and
  `Munitions/ModularMissiles/Runtime`.

For each runtime missile behavior, audit:

- typed descriptor recovery;
- owner/missile/component reference binding;
- transient state reset;
- target and guidance handoff;
- event/coroutine cleanup;
- damage/resource authority;
- visual execution on non-authoritative clients.

## Sidecars

A sidecar belongs to a native live owner without changing the native type or
prefab. Attach it idempotently, give Unity ownership where possible, and keep
global rollout policy separate from per-owner execution. Query patches may
attach infrastructure but should not mutate gameplay state. The detailed living
pattern is `AGMLIB/Common/Sidecars.md`.

## Network pool sequence

`Utility/NetworkPoolable.cs` activates the object before invoking
`OnUnpooled`, both locally on the host and through an RPC on clients.
Repooling may be immediate or delayed. Subclasses must cancel delayed work and
reset per-use state across `OnUnpooled`, `OnRepoolDelayed`, and `OnRepooled`.

## Lifecycle evidence checklist

Do not mark a component lifecycle-reviewed until evidence covers all applicable
rows:

| Phase | Questions |
|---|---|
| Asset load | Are serialized references and enum values valid? |
| Editor | Are filters, stats, validation, and UI consistent? |
| Save/load | Are keys and type identities stable? |
| Finalization | Which runtime components are copied or generated? |
| Instantiate | Are owner/dependencies present and initialization idempotent? |
| Network spawn | Which side initializes and mutates state? |
| Pool/unpool | Is every transient field reset? |
| Launch/activate | Are descriptor and target bindings current? |
| Update/tick | Are reflection, allocations, scene searches, and logs bounded? |
| Impact/destroy | Can callbacks outlive the target or owner? |
| Scene/mod teardown | Are subscriptions, statics, and created Unity objects released? |
