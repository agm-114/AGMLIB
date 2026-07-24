# AGMLIB improvement roadmap

This roadmap turns the 233-item backlog into ordered work packages. The exact
item-to-package and evidence mapping is generated at
`planning/generated/todo-plan-matrix.md`.

## Non-negotiable gates

- preserve released API, serialized, save, Harmony, and Workshop contracts unless
  a migration is explicitly designed and fixture-tested;
- make build, package, deploy, game test, GitHub release, and Workshop publish
  separate commands;
- never declare native compatibility without pinned assembly and focused runtime
  evidence;
- keep generated native code local and ignored;
- retain CRLF for touched C# and VB files.

## Sequence

| Order | Work package | Primary result | Depends on | Exit gate |
|---:|---|---|---|---|
| 0 | G0 governance | contract and evidence policy | none | contributors know what may not change casually |
| 1 | B1 build isolation | clean non-mutating build/package | G0 | clean build leaves Git unchanged |
| 2 | B3 dependency provenance | reproducible binary inputs | B1 | all tracked DLLs have source/version/license/hash decisions |
| 3 | B2 CI and release | read-only PR build and intentional release | B1, B3 | PR artifact and manual/tag release pass validation |
| 4 | C1 compatibility baseline | release-DLL API/serialization snapshot | B1 | accidental contract change fails locally and in CI |
| 5 | D1/D2 documentation | routed docs and component coverage | inventory, C1 | every component has an owned record or explicit legacy disposition |
| 6 | N1/N2 native boundaries | verified targets and feature-owned patches | native dump, C1 | missing target names its feature and fails/isolates safely |
| 7 | T1 tests/fixtures | pure tests plus opt-in runtime matrix | B1, C1, N1 | high-risk feature regressions are reproducible |
| 8 | T2 lifecycle/performance | ownership and hot-path baselines | D2, T1 | no unexplained sidecar/pool/subscription/resource ownership |
| 9 | A1 consolidation | clearer feature boundaries without contract churn | C1, tests | extracted helpers retain baselines and focused tests |
| 10 | R1 packaging | allowlisted hashed rollback-ready package | B1-B3, C1 | disposable-layout verification passes |
| 11 | R2 Workshop skill | explicitly authorized safe publishing | R1 | dry-run smoke test and documented rollback pass |
| 12 | G1 readiness | completion definition | all required packages | clean checkout-to-release evidence bundle |

## First three implementation slices

### Slice 1: build without side effects

Capture the current behavior, move generated `ModInfo.xml` to package output,
default output to repository-local artifacts, add explicit deploy properties, and
prove that a clean Debug and Release build leaves tracked files unchanged.

### Slice 2: compatibility and native target baselines

Build a known-good release assembly inventory, generate the public/serialized
baseline, map all patches and private members to features, and validate both
against configured assemblies. This must precede namespace cleanup or broad
refactors.

### Slice 3: high-risk feature fixtures

Prioritize socket filters/sub-sockets, delayed beam impact FX, command seekers,
submunition fuse behavior, dynamic resources/signatures, SIGINT, debuff expiry,
and craft command-channel semantics.

## Parallel-safe work

After B1 stabilizes, these streams can proceed independently:

- binary provenance and package allowlist;
- user/component documentation from generated metadata;
- pure filter/math extraction tests;
- native target verifier;
- fixture corpus curation.

Changes to public types, serialized identities, `EntryPoint`, or shared Harmony
activation remain serialized behind the compatibility baseline.
