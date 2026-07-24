# Build, CI, package, and release plan

## B1 build isolation implemented

The previous build chain was captured before replacement:

- targets `net481` and C# 12;
- sends `BaseOutputPath` directly to a hard-coded NEBULOUS mod directory;
- invokes `build.bat` before build;
- invokes `postbuild.bat` and copies `ModInfo.xml` after build;
- rewrites the source `ModInfo.xml` on every build;
- increments `VersionPrefix` in the project after successful builds.

That behavior is now removed. `AGMLIB/Version.props` is the explicit committed
version baseline (`6.2.2.867`, game version `0.6.2`). Normal Debug and Release
builds write beneath `artifacts/AGMLIB`, generate the artifact `ModInfo.xml`
from `ModInfo.template.xml`, and never deploy or edit source files.

`Get-AgmlibBuildVersion.ps1` preserves the useful fourth-component build
revision without restoring source mutation. It fingerprints build-relevant
project inputs and records the fingerprint/revision in ignored
`.build-state/agmlib-version.json`. Identical inputs reuse the revision; the
next build after a relevant change increments it once. Assembly and manifest
consume that same derived version. The ledger is local iteration history, not a
portable release input: promote the intended baseline into `Version.props`
before a release or supply a future release/CI version explicitly.

The supported commands are:

```powershell
# Repository-local build
dotnet build AGMLIB\AGMLIB.csproj --no-restore -v:minimal

# Clean-build proof that tracked files and source ModInfo.xml do not change
powershell -ExecutionPolicy Bypass -File scripts\Build\Test-AgmlibBuildIsolation.ps1

# Explicit local build and deployment
powershell -ExecutionPolicy Bypass -File scripts\Build\Deploy-Agmlib.ps1
```

`Deploy-Agmlib.ps1` owns the machine-local default game path, accepts
`-GameRoot` and `-Configuration`, refuses to deploy while Nebulous is running,
and verifies the deployed DLL, Harmony dependency, and manifest against their
source SHA-256 hashes. The MSBuild `DeployToGame` property defaults to `false`;
only the explicit script enables the deployment target.

Verification evidence for the change-aware revision was
`867, 867, 868, 868`: initial input, identical rebuild, changed input, identical
rebuild. Debug and Release clean-build isolation checks both left tracked files
and the source manifest unchanged.

The loader entry point was reduced to its live contract: inventory debug setup,
the stable Harmony ID and `PatchAll`, prefab/test bootstrap, and modular-faction
inventory setup. The active missing-resource patch moved to `AGMLIB/Patches`.
Unreachable bundle recompression, dependency repair, load-order restart,
quick-load cache, and hull-dump experiments were removed.

Remaining work in this plan starts with a disposable publish/package layout and
package validation. Local build/deploy isolation is complete; it does not imply
that CI, GitHub release, or Workshop publishing is complete.

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
