# AGMLIB agent guide

AGMLIB is a .NET Framework modding library and gameplay extension for NEBULOUS: Fleet Command, providing reusable runtime components and editor integrations for downstream mods.

## Essentials

- Build and type-check with `dotnet build AGMLIB.sln --configuration Debug --no-restore -p:DeployToGame=false -p:PowerShellExecutable=powershell`. There is no separate type-check command.
- Never commit information about private, unreleased, access-restricted, or locally shared mods. Treat uncertain visibility as private; see [private mod data and Workshop dependencies](.agents/guides/mod-data-and-workshop.md).

- Use concrete game or domain types except at unavoidable reflection, Harmony, serialization, or unknown nested-type boundaries. Convert values back to concrete types immediately after crossing such a boundary.
- Keep Harmony patch entry points small and put reusable behavior in typed helper methods.
# Documentation guidance

- Put agent-facing repository instructions in the nearest applicable `AGENTS.md`, not in a new `README.md`.
- Keep user-facing released behavior and configuration under [`docs/`](docs/), following [`docs/AGENTS.md`](docs/AGENTS.md).
- Keep native-game research under [`knowledge/`](knowledge/), implementation plans under [`planning/`](planning/), and machine-specific notes in git-ignored local files.
- Prefer links to canonical detail over copying it into higher-level guidance.

## Task-routed guidance
# NEBULOUS testing guidance

- Before NEBULOUS testing, read the git-ignored [`.agents/neb-testing.local.md`](.agents/neb-testing.local.md) for machine-, mod-, and fleet-specific paths and reproduction notes.
- Follow the [`neb-testing` skill](.agents/skills/neb-testing/SKILL.md) for reusable build, deployment, launch, prefab-dump, and log-inspection procedures.
- Keep installation-specific discoveries in `.agents/neb-testing.local.md`; keep reusable procedures, diagnostic distinctions, and reliable interactions in the relevant skill.
- When testing reveals reusable workflow knowledge, update the relevant testing skill during the same task.


Read only the guides relevant to the current task:

- [Native interop](.agents/guides/native-interop.md)
- [Unity runtime authoring](.agents/guides/unity-runtime-authoring.md)
- [Private mod data and Workshop dependencies](.agents/guides/mod-data-and-workshop.md)
- [Guide, skill, and scoped-instruction index](.agents/AGENTS.md)
