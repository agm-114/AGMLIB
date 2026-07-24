# Build, CI, package, and release plan

## Current behavior captured

`AGMLIB.csproj` currently:

- targets `net481` and C# 12;
- declares version `6.2.2.855` and game version `0.6.2`;
- sends `BaseOutputPath` directly to a hard-coded NEBULOUS mod directory;
- invokes `build.bat` before build;
- invokes `postbuild.bat` and copies `ModInfo.xml` after build;
- references 46 tracked assemblies and includes UnityEditor references;
- bundles `0Harmony.dll` as a private dependency;
- uses root namespace `Lib` while many types remain global.

`build.bat` writes the tracked `ModInfo.xml` and hard-codes
`Debug/net481/AGMLIB.dll` plus `0Harmony.dll`. This mixes source generation,
build configuration, and deployment.

## B1 design

Introduce explicit properties:

```text
AgmlibVersion        explicit committed/tag/release input
NebulousGameVersion supported game version metadata
ArtifactsDir         repository-local output root
PackageDir           disposable package layout
NebulousInstallDir   optional local installation
ModInstallDir        derived/overridable deployment destination
DeployToGame         false by default
```

Generate `ModInfo.xml` into `PackageDir`. Assembly version, manifest version, and
package metadata must use the same `AgmlibVersion`. Debug builds never increment
or rewrite release state.

Commands to deliver:

- restore;
- clean build;
- package;
- package validation;
- explicit local deploy;
- clean-build/no-Git-mutation verification.

The deploy command should detect a locked destination and report the likely game
process, source and destination hashes, and recovery action.

## B2 CI design

Build workflow:

- `windows-latest`;
- PR/push triggers as chosen by maintainers;
- read-only repository permission;
- restore, clean build, repository-script smoke tests, compatibility checks, and
  package validation;
- upload repository-local artifacts;
- no game deployment, launch, GitHub release, or Workshop action.

Release workflow:

- intentional semantic version tag or `workflow_dispatch`;
- unique concurrency group per version;
- build from the intended commit;
- verify version equality, API baseline policy, manifest, allowlist, and hashes;
- create the GitHub release with maintained tooling;
- grant `contents: write` only to this job;
- attach validated package and release notes.

## R1 package design

Start with an explicit allowlist, not an exclusion list. Candidate contents:

- `AGMLIB.dll`;
- generated `ModInfo.xml`;
- `0Harmony.dll` only after confirming the game/mod loader requirement;
- required preview/config assets that the Workshop layout actually needs;
- a generated package manifest with SHA-256, size, version, commit, and game
  compatibility.

Reject PDBs unless intentionally published, test components/fixtures, backup
projects, source dumps, logs, secrets, local notes, arbitrary DLLs, and build
intermediates. Validate in a disposable directory and retain the prior validated
package as the rollback artifact.

## R2 Workshop skill plan

Create `.agents/skills/agmlib-workshop-publish` with the `skill-creator` workflow
only after R1 is stable. Its mechanical script defaults to validate/dry-run and
requires a separate explicit publish switch plus a final authorization prompt.

The skill must:

- verify Workshop item `2960504230` from configuration and display it;
- report tree/commit/version/package/hash/release notes;
- keep credentials and guard codes outside repository/logs;
- distinguish local deploy, GitHub release, and Workshop publication;
- capture sanitized publish output and tool exit status;
- verify Workshop update metadata and downloaded content;
- document rollback to the retained last-known-good package.

No Workshop call is part of normal build, test, package, or deployment.

## Acceptance evidence

The package is complete when a clean checkout can:

1. restore configured inputs;
2. build Debug and Release without changing Git;
3. produce the same logical layout from the same version/source;
4. validate manifest, binary, version, allowlist, and hashes;
5. deploy only when explicitly requested;
6. exercise a release workflow without publishing to Workshop;
7. recover from a locked DLL and a rejected package cleanly.
