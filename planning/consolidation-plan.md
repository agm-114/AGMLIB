# Consolidation and target architecture

This plan reduces duplicated behavior while preserving released identities.
Consolidation means shared typed internals behind adapters, not mass renaming or
moving Unity types.

## Proposed feature boundaries

```text
Boot/Feature Registry
  -> Compatibility + Native Target Verification
  -> Ship State and Resources
  -> Sockets and Filters
  -> Weapons and Effects
  -> Sensors and EWAR
  -> Munitions and Modular Missiles
  -> Craft and Carrier Operations
  -> Editor/Authoring Adapters
  -> Diagnostics and Opt-in Testing
```

Each feature owns configuration, typed domain helpers, Harmony registration,
native targets, diagnostics, and tests. Existing public/serialized types may
remain where they are and delegate into the new core.

## Consolidation candidates

### Entry point and patch activation

Completed: `EntryPoint` is reduced to its live loader responsibilities, and the
unreachable dependency, Steam, asset-bundle, load-order, quick-load, and dump
experiments were removed after reference review. The active missing-resource
patch moved to a feature-neutral patch file. Remaining work is to replace the
single `PatchAll` with feature-owned registration incrementally while retaining
the stable Harmony ID or an explicit compatibility policy.

### Filters and sockets

Unify common filters, editor socket filters, child sockets, indexed missile
filters, scriptable faction filters, and UI explanations around one evaluation
result. Preserve existing public component types as adapters.

### Dynamic activation and resources

Create one state-transition engine for `ActiveSettings` descendants and one
resource transaction service. UI bindings observe state; they do not independently
recompute gameplay rules. Share modifier add/remove and teardown behavior.

### Weapons/muzzles/effects

Extract a reusable weapon readiness/ammo callback layer and a charge/sustain/
cooldown state machine. Separate server damage from visual/audio effect lifetime.
Centralize pooled effect ownership and reset.

### Sensors/EWAR

Extract observation, scoring, track policy, and presentation from
`PassiveCommsSensorComponent`; share target validation and effect application
between fixed, active, passive, and turreted variants.

### Modular missile profiles

Unify ammo-mode callbacks, impact target resolution, area falloff, timed state,
and authority checks. Keep descriptors and runtime states thin and typed.

### Craft/drone limits

Define a carrier operation ledger with active, reserved, queued, launching,
recovering, and lost counts. Editor selection and command UI become projections
of that rule. Decide mixed manned/drone policy in configuration rather than
encoding it in queue mechanics.

### Diagnostics/testing

Replace global noisy logging with a feature-tagged diagnostic sink and rate
limits. Keep testing components opt-in and move pure math/filter/serialization
logic into a conventional testable project.

## Large-file extraction order

1. `SocketFilterCore.cs`: pure evaluation model and explanation.
2. `PassiveCommsSensorComponent.cs`: observation/scoring/track/UI split.
3. `CommandSeekers.cs`: descriptor, runtime guidance, overlay/debug split.
4. `ShipStatusPowerBar.cs`: model, binding, view/updater split.
5. duplicated discrete weapon variants: shared magazine/ejector core.
6. prefab YAML dumper: traversal, serialization, redaction, output split.

Completed ahead of this queue: `EntryPoint.cs` unreachable-responsibility
removal.

Each extraction requires compatibility baseline, focused tests, and no public or
serialized move unless a shim is proven.

## Legacy disposition

For each `Advanced/Legacy` file choose:

- retain compiled for binary/serialized compatibility;
- retain as a delegating shim;
- isolate behind an explicit legacy feature;
- exclude from build but preserve history/documentation;
- delete only after assembly, prefab, fleet/save, and downstream reference
  evidence proves it safe.

Duplicate simple names, loose patches, and generic experimental filenames receive
priority because they complicate reflection and Unity/editor lookup.
