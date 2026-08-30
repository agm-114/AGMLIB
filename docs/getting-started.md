# Getting started with AGMLIB

AGMLIB is a NEBULOUS: Fleet Command modding library and gameplay extension. It
contains reusable runtime components, editor integrations, munitions, weapon and
sensor extensions, dynamic ship systems, Harmony patches, and opt-in testing
support. It is also a compatibility dependency for downstream content, so public
type names and Unity-serialized identities must be treated as durable.

## Current compatibility snapshot

The project currently declares:

- target framework: .NET Framework 4.8.1;
- project version: `6.2.2.855`;
- declared game version: `0.6.2`;
- Harmony ID used by the entry point: `neb.lib.harmony.product`;
- Workshop item mentioned by existing tooling: `2960504230`.

These values describe the working tree on 2026-07-24. They are not a release
guarantee. Before publishing or diagnosing a game update, compare the installed
assemblies with `knowledge/evidence-ledger.md`.

## Safe orientation commands

These commands inspect repository-local state. For a repository-only build, set
`DeployToGame=false`; otherwise a local build automatically updates the detected
default NEBULOUS installation.

```powershell
git status --short
dotnet build AGMLIB\AGMLIB.csproj --no-restore -p:DeployToGame=false
.\scripts\Documentation\Export-AgmlibInventory.ps1 -Check
.\scripts\NativeCode\Export-NebulousManagedCode.ps1 -ValidateOnly
```

Use `.agents/skills/neb-testing/SKILL.md` for game deployment, launch, prefab
dumping, and log inspection. Those operations can modify the local game install
or launch NEBULOUS and must not be confused with repository-only validation.

## Where to look

- `docs/feature-catalog.md`: user-facing feature map and maturity notes.
- `docs/component-authoring.md`: how to add and configure components safely.
- `docs/compatibility.md`: public API, serialization, namespace, and save rules.
- `docs/testing-and-troubleshooting.md`: validation levels and evidence.
- `knowledge/`: verified and inferred notes about native game behavior.
- `planning/`: implementation plans, risk registers, consolidation proposals,
  and exhaustive generated inventories.

## Important current limitations

- Local builds auto-deploy when the default NEBULOUS installation is detected;
  use `DeployToGame=false` for repository-only validation or packaging.
- the entry point activates all Harmony patches in one operation;
- much of the released API remains in the global namespace;
- native private-member access is split between new typed `Internals()` accessors
  and older reflection helpers;
- `Advanced/Legacy` contains prototypes, incomplete work, duplicate type names,
  and old patch experiments. New features should not be placed there.

The remediation sequence is planned in `planning/roadmap.md`.
