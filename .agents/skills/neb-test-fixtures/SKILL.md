---
name: neb-test-fixtures
description: Design or implement deterministic, opt-in NEBULOUS test components, local-only support mods, controlled fleet/scenario fixtures, and structured telemetry. Use when a bug is hard to reproduce manually, a runtime state must be created reliably, ships or craft need test-only modification, a feature needs performance or multiplayer evidence, or reusable testing support must remain isolated from normal AGMLIB gameplay.
---

# Nebulous Test Fixtures

Create the smallest controlled fixture that produces the state under test, then use `neb-testing` to build, launch, execute, and collect evidence.

## Choose The Lowest Useful Layer

Prefer, in order:

1. pure logic tests for filters, normalization, and decision rules;
2. adapter tests around small game-facing seams;
3. prefab/YAML inspection for serialized wiring;
4. an opt-in runtime component attached to the exact test asset;
5. a separate local-only support mod for global patches, transport changes, or broad state mutation;
6. a controlled in-game scenario when lifecycle, physics, AI, networking, or pooling is essential.

Do not launch the game for a claim that can be proven at a lower layer.

## Define The Experiment

Before coding, write down:

- one behavior under test;
- the exact fleet, ship, craft, missile, component, munition save key, or prefab;
- game mode and lifecycle state;
- host/server/client ownership;
- controlled inputs and expected observable result;
- failure evidence and acceptance criteria;
- telemetry fields required to explain a rejection or success.

Use broad before/after metrics rather than frame-perfect replay for performance and AI behavior.

## Isolate Test Behavior

- Require explicit activation through a test component, exact config flag, exact save key, or separate test mod.
- Keep passive installation behavior-free.
- Put global Harmony patches, network transport overrides, infinite health, mass damage, and similar intrusive behavior in a clearly named local-only support mod when practical.
- Keep test-only dependencies and helpers out of publishable packages.
- Make deep telemetry disabled by default.
- Fail closed when session authority, object identity, or activation scope is uncertain.

Read [references/test-rig-patterns.md](references/test-rig-patterns.md) before implementing a runtime fixture.

## Implement A Runtime Fixture

1. Hook the narrowest stable lifecycle point.
2. Gate gameplay mutation on `isServer` or the appropriate authoritative state.
3. Resolve the exact spawned prefab and `SaveFileObject` key from the current prefab dump. Match concrete identity fields such as save key or exact configured name; log which identity matched.
4. Make application idempotent per live object or network identity.
5. If Unity/game initialization is incomplete, wait with a bounded coroutine and an explicit readiness predicate.
6. Apply state through native APIs where possible so vanilla replication and bookkeeping remain intact.
7. Clean retained object identities when objects despawn or a session ends.
8. Emit one setup line, event lines only when useful, and one compact summary.

Keep observational probes observational. Do not add a second collider, change production state, or assume Unity forwards a child collider's message to an arbitrary component. Attach to the actual callback owner or use a narrowly gated local-only Harmony observer after a one-time callback-routing preflight.

Never access Unity objects from a worker thread. Capture or dispatch to the main thread when a fixture performs background parsing or orchestration.

## Build A Controlled Scenario

Fix the fleet, map, starting positions, doctrine/config, target mix, and run duration. Prefer spectator or AI-driven execution when manual input is irrelevant.

For routine AGMLIB tests, use the Fleet Editor testing range first. Use skirmish only when deployment, AI, authority, or match lifecycle is part of the reproduction. For multiplayer, preserve both host and client logs.

## Collect Stable Evidence

Prefer structured lines such as:

```text
[AGMLIB Test] event=fuse-candidate object=L01 target=Fighter accepted=false reason=target-type role=server
[AGMLIB Test] summary scenario=light-mine-fighter candidates=12 accepted=0 elapsedMs=60000 warnings=0
```

Include role, stable object identity, decision, reason, relevant config, and timing. Avoid full per-frame dumps and unstable instance IDs in golden summaries.

## Finish Cleanly

1. Run the same scenario before and after the change.
2. Compare compact summaries and focused exceptions.
3. Remove temporary noisy diagnostics.
4. Disable or uninstall intrusive local-only support.
5. Verify no test helper, path, private note, or deep-telemetry default entered release output.
6. Record reusable local fleet/mod details in `.agents/neb-testing.local.md` and reusable procedure in the appropriate skill.
