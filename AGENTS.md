# AGMLIB Agent Notes

## Documentation Files

- Always prefer `AGENTS.md` over `README.md` for new documentation files and documentation edits.

## Code Style

- Prefer concrete game/domain types over `object` where possible.
- Use `object` only at unavoidable reflection, Harmony, serialization, or unknown nested-type boundaries. Convert back to typed helpers immediately after that boundary.
- Keep Harmony patch entry points small and put reusable behavior in typed helper methods.

## Native Internals

- Read [Native internals accessors](AGMLIB/Nebulous/AGENTS.md) before accessing a known non-public member on a native game type.
- Put typed `Internals()` accessors under `AGMLIB/Nebulous`, using subfolders that mirror the declaring type's namespace.
- Prefer these cached, typed accessors over `Common.GetVal`, `Common.SetVal`, `Common.RunFunc`, local `AccessTools` bindings, `Traverse`, or repeated `FieldInfo`/`PropertyInfo`/`MethodInfo` reflection for known native members.
- When modifying a function that already uses one of those older reflection approaches, migrate the known native-member accesses in that function to the `Internals()` pattern as part of the same change. 
- This is an incremental migration rule, not a requirement for unrelated repository-wide cleanup.
- Keep dynamic reflection only where the member or runtime type cannot be known in advance, and convert its result back to a concrete type immediately.

## Unity Runtime Components

- Runtime missile/component behaviours are often created on a finalized pattern and then cloned or pooled for live instances. Private runtime fields that must survive into the spawned instance, especially descriptor references and bound component references, should follow the vanilla pattern and be marked `[SerializeField]`.
- Do not rely on `OnAdded`-assigned private fields being present on launched/pooled runtime copies unless those fields are serialized or rebuilt in `OnUnpooled`/`OnLaunched`.
- When storing a typed descriptor field, prefer a small typed fallback helper that recovers from the base descriptor reference if the typed field is null.

## AssetBundle Serialization

- Read [AssetBundle custom-value serialization](knowledge/asset-bundle-serialization.md) before designing complex serialized authoring data. Nested AGMLIB-defined custom classes and structs are currently unsupported; follow that document's interim representation rules.

## Runtime Sidecars

- Read `AGMLIB/Common/Sidecars.md` before adding or changing behavior attached alongside a native runtime object.
- Treat that document as living guidance. When runtime work reveals a reusable lesson about sidecar ownership, lifecycle, rollout, patch boundaries, or testing, improve the document in the same change while keeping case-specific details out of the general pattern.

## Local Testing Context

- Before Nebulous testing, read `.agents/neb-testing.local.md` for machine-, mod-, and fleet-specific paths and reproduction notes.
- `.agents/neb-testing.local.md` is intentionally git-ignored. Keep useful local testing discoveries there so future sessions do not have to rediscover them.
- Keep reusable testing procedures in `.agents/skills/neb-testing/SKILL.md`; keep installation-specific details in the local notes file.
