# AGMLIB Agent Notes

## Line Endings

- Preserve the existing line-ending style when editing files.
- C# and VB files in this repo use Windows CRLF line endings, matching `.editorconfig` (`end_of_line = crlf`).
- Do not use editing commands or scripts that rewrite only part of a file with LF line endings. When a full-file rewrite is unavoidable, normalize the entire file to the repo's expected line endings before finishing.
- Before finishing any change to a `.cs` or `.vb` file, verify the touched file does not contain mixed line endings.

## Code Style

- Prefer concrete game/domain types over `object` where possible.
- Use `object` only at unavoidable reflection, Harmony, serialization, or unknown nested-type boundaries. Convert back to typed helpers immediately after that boundary.
- Keep Harmony patch entry points small and put reusable behavior in typed helper methods.

## Unity Runtime Components

- Runtime missile/component behaviours are often created on a finalized pattern and then cloned or pooled for live instances. Private runtime fields that must survive into the spawned instance, especially descriptor references and bound component references, should follow the vanilla pattern and be marked `[SerializeField]`.
- Do not rely on `OnAdded`-assigned private fields being present on launched/pooled runtime copies unless those fields are serialized or rebuilt in `OnUnpooled`/`OnLaunched`.
- When storing a typed descriptor field, prefer a small typed fallback helper that recovers from the base descriptor reference if the typed field is null.

## Local Testing Context

- Before Nebulous testing, read `.agents/neb-testing.local.md` for machine-, mod-, and fleet-specific paths and reproduction notes.
- `.agents/neb-testing.local.md` is intentionally git-ignored. Keep useful local testing discoveries there so future sessions do not have to rediscover them.
- Keep reusable testing procedures in `.agents/skills/neb-testing/SKILL.md`; keep installation-specific details in the local notes file.
