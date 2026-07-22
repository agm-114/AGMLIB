---
name: neb-runtime-research
description: Trace and verify NEBULOUS runtime behavior from AGMLIB source, current managed assemblies, prefab YAML, inheritance, Unity lifecycle, Harmony/reflection targets, and Player.log evidence. Use when explaining what a native game class does, diagnosing a version-dependent or prefab-dependent failure, checking whether a callback or field is actually used, validating an override or Harmony patch target, or researching a bug before implementing a fix.
---

# Nebulous Runtime Research

Prove the relevant native, AGMLIB, prefab, and live-runtime paths before changing behavior. Use `neb-testing` afterward when the claim needs an in-game reproduction.

## Start With Current Evidence

1. Read `AGENTS.md` and `.agents/neb-testing.local.md`.
2. Check `git status --short` and preserve unrelated user changes.
3. Search AGMLIB source, existing notes, and current prefab YAML with `rg`.
4. Identify whether the question concerns a descriptor, prefab component, finalized pattern, spawned/pooled instance, or live network object.
5. Prefer the assembly loaded by the current game install. Compare it with `AGMLIB/libs` when stale references or an out-of-date DLL are plausible.

Do not treat source names, similarly named prefabs, or an old decompile as proof of the live path.

## Decompile Narrowly

Use the bundled helper for a specific native type:

```powershell
powershell -ExecutionPolicy Bypass -File .agents\skills\neb-runtime-research\scripts\inspect-neb-type.ps1 `
  -TypeName Munitions.LoiteringMissile `
  -Pattern 'LaunchInternal|OnTriggerEnter|TransitionLoiter'
```

Pass `-AssemblyPath` when inspecting a report from another game or AGMLIB build. The helper prints the assembly SHA-256 and writes only to `.agents/cache/neb-runtime-research` by default.

Use the `ilspy-cli` skill for assembly-wide work, IL inspection, project extraction, or complex nested/generic types.

If the exact namespace is unknown, list classes and filter by the short type name before decompiling. The helper prints likely class-name matches when a requested type cannot be resolved.

## Trace The Actual Path

1. Locate the entry point named by the report, exception, or user interaction.
2. Trace callers backward until the lifecycle or user action is clear.
3. Trace state transitions and callees forward until the observed effect is reached.
4. Record gates in execution order: lifecycle state, server/client authority, ownership/team, enabled flags, arming delay, target type, layer/collision interaction, obstruction, and cleanup.
5. Correlate serialized fields with the exact prefab that is spawned. Follow `_submunitionPrefab`, factory, pool, or bundle references rather than assuming the display name identifies the object.
6. Compare prefab/component data with code defaults. A serialized value or missing reference can replace the apparent source default.
7. Read focused `Player.log` context when the behavior was observed at runtime.

Read [references/evidence-checklist.md](references/evidence-checklist.md) when inheritance, Unity messages, prefab cloning, pooling, physics triggers, networking, or version drift could alter the conclusion.

## Verify Harmony And Reflection

For every patch or reflected member, verify:

- assembly, namespace, declaring type, and exact overload;
- static/instance and return/parameter types;
- whether the member exists in the game assembly currently being tested;
- whether the implementation is inherited, overridden, hidden with `new`, or invoked as a Unity message;
- whether the patch is installed at the relevant lifecycle point;
- whether another mod or assembly version could own the failing dynamic wrapper;
- whether a reflected Unity API is reached on the main thread.

Keep Harmony entry points small. Convert reflection results to concrete domain types immediately after the unavoidable reflection boundary.

## Account For Unity Runtime Copies

NEBULOUS often finalizes a pattern, then clones or pools it. Treat private fields assigned only during authoring/setup as suspect unless they are serialized or rebuilt in `OnUnpooled`, `OnLaunched`, or an equivalent live-instance lifecycle method.

When a typed descriptor field may be absent on an older prefab or cloned instance, verify whether it can be recovered from the base descriptor reference before concluding that the descriptor itself is missing.

## Report Before Editing

State:

- the exact prefab/component/type involved;
- the native and AGMLIB entry points;
- the ordered gates that pass or fail;
- the assembly hash/path used as evidence;
- what is proven, inferred, and still unknown;
- the smallest reproduction that would distinguish remaining hypotheses.

If the user asked only for diagnosis or explanation, stop there. If they asked for a change, implement the smallest fix and hand execution to `neb-testing`.
