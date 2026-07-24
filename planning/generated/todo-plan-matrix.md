# Exhaustive TODO execution matrix

This generated matrix assigns all 266 unchecked items in TODO.md to a work package and completion-evidence rule. IDs are positional and should be regenerated when the source TODO changes.

Run `.\scripts\Documentation\Export-TodoPlan.ps1 -Check` to detect drift.

## Working Principles

| ID | Depth | TODO | Work package | Required completion evidence |
|---|---:|---|---|---|
| T001 | 0 | Preserve released public APIs, serialized Unity contracts, save keys, Harmony IDs, and Workshop compatibility unless a migration is explicitly designed. | G0 governance gate | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T002 | 0 | Make normal builds portable/reproducible across clean machines; tracked-file mutation is eliminated, but the change-aware local revision ledger intentionally carries local iteration history. | G0 governance gate | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T003 | 0 | Separate build, package, local deploy, game testing, and Workshop publishing into explicit operations; build and deploy are separated, while package/publish work remains. | G0 governance gate | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T004 | 0 | Preserve CRLF line endings in C# and VB files and verify touched files do not contain mixed line endings. | G0 governance gate | Changed-file checker passes and attributes/editor settings agree. |
| T005 | 0 | Keep new agent documentation concise and task-routed; do not copy One More Jump's full generated-document system unless AGMLIB grows enough to need it. | G0 governance gate | Task-routed document is linked, checked for freshness, and does not duplicate canonical detail. |

## P0 - Reproducible, Non-Mutating Builds

| ID | Depth | TODO | Work package | Required completion evidence |
|---|---:|---|---|---|
| T006 | 0 | Add configurable `NebulousInstallDir` and `ModInstallDir` properties with documented local defaults. | B1 build isolation and versioning | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |

## P0 - Repair CI and Release Automation

| ID | Depth | TODO | Work package | Required completion evidence |
|---|---:|---|---|---|
| T007 | 0 | Replace `.github/workflows/main.yml` with separate build and release workflows or clearly separated jobs. | B2 CI and GitHub release | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T008 | 0 | Run the initial build workflow on `windows-latest` while the project still requires Windows tooling. | B2 CI and GitHub release | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T009 | 0 | Give the PR/build job read-only repository permissions. | B2 CI and GitHub release | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T010 | 0 | Run restore and build without deploying to a local game installation. | B2 CI and GitHub release | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T011 | 0 | Remove the empty `dotnet test` step until a real test project exists, or add the test project first. | B2 CI and GitHub release | Deterministic test is repeatable, opt-in where runtime is required, and fails for the intended regression. |
| T012 | 0 | Upload artifacts from a repository-local build/package directory. | B2 CI and GitHub release | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T013 | 0 | Update GitHub Actions dependencies to currently supported versions. | B2 CI and GitHub release | One explicit version is shown to match assembly, manifest, package, and release metadata. |
| T014 | 0 | Remove the archived `actions/create-release@v1` action. | B2 CI and GitHub release | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T015 | 0 | Remove the unsupported `actions/upload-artifact@v3` action. | B2 CI and GitHub release | Workflow dry run or Actions run proves permissions, trigger, artifact, failure, and concurrency behavior. |
| T016 | 0 | Trigger releases only from an intentional version tag or `workflow_dispatch`, never from every push or pull request. | B2 CI and GitHub release | One explicit version is shown to match assembly, manifest, package, and release metadata. |
| T017 | 0 | Use a unique semantic version tag instead of the fixed `Automatic` tag. | B2 CI and GitHub release | One explicit version is shown to match assembly, manifest, package, and release metadata. |
| T018 | 0 | Generate release notes or require an explicit changelog/release-notes input. | B2 CI and GitHub release | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T019 | 0 | Use `gh release create` or another maintained release mechanism. | B2 CI and GitHub release | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T020 | 0 | Grant `contents: write` only to the release job. | B2 CI and GitHub release | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T021 | 0 | Validate the package before creating a GitHub release. | B2 CI and GitHub release | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T022 | 0 | Fail when the expected DLL, manifest, or package artifact is missing. | B2 CI and GitHub release | Workflow dry run or Actions run proves permissions, trigger, artifact, failure, and concurrency behavior. |
| T023 | 0 | Add workflow concurrency rules to prevent two releases for the same version. | B2 CI and GitHub release | One explicit version is shown to match assembly, manifest, package, and release metadata. |
| T024 | 0 | Add a manual CI-safe package build for maintainers to test release output without publishing. | B2 CI and GitHub release | Documented clean command succeeds and `git status --porcelain` is unchanged. |

## P1 - Agent Guidance and Repository Documentation

| ID | Depth | TODO | Work package | Required completion evidence |
|---|---:|---|---|---|
| T025 | 0 | Expand root `AGENTS.md` while keeping it short enough to remain always relevant. | D1 documentation routing | Task-routed document is linked, checked for freshness, and does not duplicate canonical detail. |
| T026 | 0 | Add a repository map covering: | D1 documentation routing | Task-routed document is linked, checked for freshness, and does not duplicate canonical detail. |
| T027 | 1 | `Advanced/` and `Advanced/Legacy/`. | D1 documentation routing | Compatibility review and focused tests precede extraction; public identities remain or receive shims. |
| T028 | 1 | `Common/`. | D1 documentation routing | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T029 | 1 | `Craft/`. | D1 documentation routing | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T030 | 1 | `Dynamic Systems/`. | D1 documentation routing | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T031 | 1 | `Editor/`. | D1 documentation routing | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T032 | 1 | `FX/`. | D1 documentation routing | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T033 | 1 | `Generic Gameplay/`. | D1 documentation routing | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T034 | 1 | `Munitions/`. | D1 documentation routing | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T035 | 1 | `Server/` and `Systems/`. | D1 documentation routing | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T036 | 0 | State that new work should not be added under `Advanced/Legacy/` unless a task explicitly targets legacy behavior. | D1 documentation routing | Compatibility review and focused tests precede extraction; public identities remain or receive shims. |
| T037 | 0 | Add canonical commands for restore, build, package, deploy, launch, reproduce, and log inspection. | D1 documentation routing | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T038 | 0 | Distinguish CI-safe validation from operations that modify the game installation or launch NEBULOUS. | D1 documentation routing | Opt-in local scenario records paths, process state, hashes, result, and recovery. |
| T039 | 0 | Add public API and serialization rules: | D1 documentation routing | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T040 | 1 | Do not casually rename released public types. | D1 documentation routing | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T041 | 1 | Treat `MonoBehaviour`, `ScriptableObject`, public enum, serialized-field, and save-key names as durable contracts. | D1 documentation routing | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T042 | 1 | Require migration or compatibility aliases for intentional breaking changes. | D1 documentation routing | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T043 | 1 | Require explicit numeric values for public serialized enums. | D1 documentation routing | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T044 | 1 | Prohibit nested AGMLIB-defined custom classes and structs as AssetBundle-authored serialized payloads until the released bundle-load path proves them safe. | D1 documentation routing | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T045 | 1 | Require complex authoring data to use bundle-proven Unity/native types, validated flattened lists/indices, or separately attached component references in the interim. | D1 documentation routing | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |
| T046 | 0 | Add namespace rules for new code: use a chosen `AGMLIB.*` namespace and do not introduce new global-namespace public types. | D1 documentation routing | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T047 | 0 | Add Harmony guidance for stable IDs, small patch entrypoints, typed helpers, lifecycle/authority checks, and feature ownership. | D1 documentation routing | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |
| T048 | 0 | Add generated-file, binary-reference, and packaging boundaries. | D1 documentation routing | Dependency manifest records source, version, license, hash, necessity, and clean-build result. |
| T049 | 0 | Route build/deploy/game reproduction tasks to `.agents/skills/neb-testing/SKILL.md`. | D1 documentation routing | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T050 | 0 | Create `.agents/AGENTS.md` as the deeper map for skills, compatibility baselines, durable notes, and task plans. | D1 documentation routing | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T051 | 0 | Decide when a finding deserves a durable `.agents/notes/` document instead of expanding `AGENTS.md`. | D1 documentation routing | Task-routed document is linked, checked for freshness, and does not duplicate canonical detail. |
| T052 | 0 | Expand the root `AGENTS.md` with: | D1 documentation routing | Task-routed document is linked, checked for freshness, and does not duplicate canonical detail. |
| T053 | 1 | Project purpose and supported NEBULOUS version. | D1 documentation routing | One explicit version is shown to match assembly, manifest, package, and release metadata. |
| T054 | 1 | Installation and Workshop information. | D1 documentation routing | Non-publishing test covers validation; authorized publish evidence records sanitized manifest and post-publish verification. |
| T055 | 1 | Prerequisites and dependency setup. | D1 documentation routing | Dependency manifest records source, version, license, hash, necessity, and clean-build result. |
| T056 | 1 | Local build, package, and deploy instructions. | D1 documentation routing | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T057 | 1 | Repository architecture and major feature areas. | D1 documentation routing | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T058 | 1 | Public API stability policy. | D1 documentation routing | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T059 | 1 | Testing and log-triage instructions. | D1 documentation routing | Deterministic test is repeatable, opt-in where runtime is required, and fails for the intended regression. |
| T060 | 1 | Contributor guidance and links to scoped `AGENTS.md` files. | D1 documentation routing | Task-routed document is linked, checked for freshness, and does not duplicate canonical detail. |
| T061 | 0 | Do not add `llms-full.txt` or generated agent indexes until the documentation set is large enough to justify their context and maintenance cost. | D1 documentation routing | Task-routed document is linked, checked for freshness, and does not duplicate canonical detail. |

## P1 - Create a Workshop Update Publishing Skill

| ID | Depth | TODO | Work package | Required completion evidence |
|---|---:|---|---|---|
| T062 | 0 | Use the skill-creator workflow to create a dedicated AGMLIB Workshop publishing skill. | R2 Workshop publishing skill | Workflow dry run or Actions run proves permissions, trigger, artifact, failure, and concurrency behavior. |
| T063 | 0 | Choose a clear trigger-oriented name, such as `agmlib-workshop-publish`. | R2 Workshop publishing skill | Non-publishing test covers validation; authorized publish evidence records sanitized manifest and post-publish verification. |
| T064 | 0 | Place the skill under `.agents/skills/` and add it to `.agents/AGENTS.md`. | R2 Workshop publishing skill | Task-routed document is linked, checked for freshness, and does not duplicate canonical detail. |
| T065 | 0 | Treat Workshop item `2960504230` as configuration that must be verified before publishing, not as an unchecked assumption. | R2 Workshop publishing skill | Non-publishing test covers validation; authorized publish evidence records sanitized manifest and post-publish verification. |
| T066 | 0 | Document ownership and authorization requirements for updating the Workshop item. | R2 Workshop publishing skill | Non-publishing test covers validation; authorized publish evidence records sanitized manifest and post-publish verification. |
| T067 | 0 | Keep Steam credentials, session tokens, guard codes, and machine-specific secrets outside Git and outside generated logs. | R2 Workshop publishing skill | Non-publishing test covers validation; authorized publish evidence records sanitized manifest and post-publish verification. |
| T068 | 0 | Decide whether publishing will use SteamCMD, an existing Workshop tool, or a maintained API/CLI wrapper. | R2 Workshop publishing skill | Non-publishing test covers validation; authorized publish evidence records sanitized manifest and post-publish verification. |
| T069 | 0 | Create or wrap a repository script for the mechanical publish operation rather than embedding a large command sequence directly in `SKILL.md`. | R2 Workshop publishing skill | Non-publishing test covers validation; authorized publish evidence records sanitized manifest and post-publish verification. |
| T070 | 0 | Require an explicit publish action; never publish as a side effect of build, package, test, or deploy. | R2 Workshop publishing skill | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T071 | 0 | Make dry-run/package-validation mode the default where the selected publishing tool permits it. | R2 Workshop publishing skill | Non-publishing test covers validation; authorized publish evidence records sanitized manifest and post-publish verification. |
| T072 | 0 | Require the following preflight checks: | R2 Workshop publishing skill | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T073 | 1 | Working tree and intended commit are reported. | R2 Workshop publishing skill | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T074 | 1 | Release version is explicit and matches the assembly and `ModInfo.xml`. | R2 Workshop publishing skill | One explicit version is shown to match assembly, manifest, package, and release metadata. |
| T075 | 1 | Release configuration builds successfully. | R2 Workshop publishing skill | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T076 | 1 | Package allowlist validation succeeds. | R2 Workshop publishing skill | Disposable package validation records allowlist, layout, hashes, and rollback artifact. |
| T077 | 1 | Required DLL, manifest, preview image, and configuration files exist. | R2 Workshop publishing skill | Disposable package validation records allowlist, layout, hashes, and rollback artifact. |
| T078 | 1 | No backup, debug-only, test, symbol, credential, or unrelated files are present. | R2 Workshop publishing skill | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T079 | 1 | The target Workshop item ID and content directory are shown before upload. | R2 Workshop publishing skill | Non-publishing test covers validation; authorized publish evidence records sanitized manifest and post-publish verification. |
| T080 | 1 | NEBULOUS and other processes are not locking package files. | R2 Workshop publishing skill | Disposable package validation records allowlist, layout, hashes, and rollback artifact. |
| T081 | 1 | Release notes/change description are supplied and reviewed. | R2 Workshop publishing skill | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T082 | 0 | Require a final explicit confirmation immediately before the external publish action. | R2 Workshop publishing skill | Non-publishing test covers validation; authorized publish evidence records sanitized manifest and post-publish verification. |
| T083 | 0 | Print or save a sanitized publish manifest containing file hashes, version, commit, item ID, and timestamp. | R2 Workshop publishing skill | One explicit version is shown to match assembly, manifest, package, and release metadata. |
| T084 | 0 | Capture SteamCMD/tool exit status and fail loudly on partial uploads or ambiguous results. | R2 Workshop publishing skill | Non-publishing test covers validation; authorized publish evidence records sanitized manifest and post-publish verification. |
| T085 | 0 | Add post-publish verification: | R2 Workshop publishing skill | Non-publishing test covers validation; authorized publish evidence records sanitized manifest and post-publish verification. |
| T086 | 1 | Confirm the Workshop item reports the expected update time/version. | R2 Workshop publishing skill | One explicit version is shown to match assembly, manifest, package, and release metadata. |
| T087 | 1 | Confirm a clean client can download the updated content. | R2 Workshop publishing skill | Non-publishing test covers validation; authorized publish evidence records sanitized manifest and post-publish verification. |
| T088 | 1 | Confirm the downloaded package passes the same validation checks. | R2 Workshop publishing skill | Disposable package validation records allowlist, layout, hashes, and rollback artifact. |
| T089 | 1 | Launch and verify that AGMLIB loads successfully when runtime validation is requested. | R2 Workshop publishing skill | Opt-in local scenario records paths, process state, hashes, result, and recovery. |
| T090 | 0 | Document rollback/recovery steps for a bad Workshop update, including preservation of the last known-good package. | R2 Workshop publishing skill | Non-publishing test covers validation; authorized publish evidence records sanitized manifest and post-publish verification. |
| T091 | 0 | Ensure the skill distinguishes GitHub release publishing, local mod deployment, and Steam Workshop publishing. | R2 Workshop publishing skill | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T092 | 0 | Add a non-publishing smoke test for the skill's scripts and validation logic. | R2 Workshop publishing skill | Non-publishing test covers validation; authorized publish evidence records sanitized manifest and post-publish verification. |
| T093 | 0 | Add AGENTS guidance stating that Workshop publishing is an external, irreversible operation requiring explicit user authorization. | R2 Workshop publishing skill | Non-publishing test covers validation; authorized publish evidence records sanitized manifest and post-publish verification. |

## P1 - Public API and Serialization Compatibility

| ID | Depth | TODO | Work package | Required completion evidence |
|---|---:|---|---|---|
| T094 | 0 | Generate an initial authoritative AGMLIB compatibility baseline from a known-good release assembly. | C1 compatibility baseline | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T095 | 0 | Record public type full names, including nested types where consumers can reference them. | C1 compatibility baseline | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T096 | 0 | Record public methods, properties, fields, constructors, and interfaces that are intended as supported API. | C1 compatibility baseline | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T097 | 0 | Record public enum member names and numeric values. | C1 compatibility baseline | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T098 | 0 | Record public `MonoBehaviour` and `ScriptableObject` type names. | C1 compatibility baseline | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T099 | 0 | Record serialized public fields, save keys, config keys, and other Unity/XML identity-sensitive names. | C1 compatibility baseline | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T100 | 0 | Record stable Harmony IDs and patch ownership. | C1 compatibility baseline | Pinned assembly verifier identifies owner and exact target; focused runtime evidence confirms the call path. |
| T101 | 0 | Record the assembly version, game version, and source commit associated with the baseline. | C1 compatibility baseline | One explicit version is shown to match assembly, manifest, package, and release metadata. |
| T102 | 0 | Compare the baseline against One More Jump's committed AGMLIB public-name snapshot and resolve discrepancies. | C1 compatibility baseline | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T103 | 0 | Create a CI-safe compatibility checker that fails on accidental removal, rename, namespace change, enum-value change, or serialized-field loss. | C1 compatibility baseline | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T104 | 0 | Require an explicit baseline update and migration note for intentional breaking changes. | C1 compatibility baseline | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T105 | 0 | Add collision checks for public serialized/editor-visible simple type names where NEBULOUS or Unity resolves by simple name. | C1 compatibility baseline | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T106 | 0 | Publish the compatibility policy in the appropriate scoped `AGENTS.md`. | C1 compatibility baseline | Non-publishing test covers validation; authorized publish evidence records sanitized manifest and post-publish verification. |

## P1 - Component Documentation, Audit, and Namespace Compatibility

| ID | Depth | TODO | Work package | Required completion evidence |
|---|---:|---|---|---|
| T107 | 0 | Create a `docs/` folder that inventories and documents every AGMLIB component. | D2 component catalog and audit | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |
| T108 | 0 | Define which public and serialized types qualify as components and generate the authoritative component inventory from source or assembly metadata. | D2 component catalog and audit | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T109 | 0 | Create a standard component-document template and require consistent identity, compatibility, lifecycle, configuration, usage, and testing sections. | D2 component catalog and audit | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T110 | 0 | For each component, document its purpose, configuration, serialized fields, lifecycle, native-game dependencies, runtime behavior, known limitations, and a minimal usage example. | D2 component catalog and audit | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T111 | 0 | Link each component document to its source type and record whether it is a `MonoBehaviour`, `ScriptableObject`, descriptor, runtime component, editor component, or supporting type. | D2 component catalog and audit | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |
| T112 | 0 | Add a CI-safe documentation coverage and freshness check so newly added, renamed, moved, or removed components cannot be omitted silently. | D2 component catalog and audit | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |
| T113 | 0 | Review every component systematically for bugs, including serialization, cloning/pooling, lifecycle ordering, null handling, multiplayer authority, editor/runtime parity, and native-version compatibility. | D2 component catalog and audit | One explicit version is shown to match assembly, manifest, package, and release metadata. |
| T114 | 0 | Record each audit result, reproduction evidence, severity, and follow-up action; add regression coverage when a bug is fixed. | D2 component catalog and audit | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T115 | 0 | Inventory component dependencies, initialization order, shared native/Harmony targets, optional integrations, and known component conflicts. | D2 component catalog and audit | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |
| T116 | 0 | Add authoring-time or load-time validation for missing references, invalid ranges, duplicate or missing save keys, incompatible combinations, and configuration that would otherwise fail only at runtime. | D2 component catalog and audit | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T117 | 0 | Define component diagnostics conventions with stable prefixes, useful identity and state context, configurable verbosity, and suppression or rate limiting for repeated messages. | D2 component catalog and audit | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |
| T118 | 0 | Remove or gate noisy development logging, especially logging reachable from `Update`, `FixedUpdate`, `LateUpdate`, or other high-frequency callbacks. | D2 component catalog and audit | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T119 | 0 | Inventory component namespaces and identify types that are missing, inconsistent, or incorrectly placed in namespaces. | D2 component catalog and audit | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T120 | 0 | Determine whether each proposed namespace correction can preserve existing Unity serialization, saved content, prefab references, XML identities, and downstream binary compatibility. | D2 component catalog and audit | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T121 | 0 | Test namespace migrations against representative existing fleets, prefabs, save data, and third-party mod references before adopting them. | D2 component catalog and audit | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T122 | 0 | Use compatibility shims, migration metadata, or retained legacy type aliases where proven safe; do not move a released component type when compatibility cannot be demonstrated. | D2 component catalog and audit | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T123 | 0 | Define a deprecation policy for legacy component types, namespace shims, serialized fields, save keys, and public APIs, including support windows, migration notes, and removal criteria. | D2 component catalog and audit | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T124 | 0 | Define a component change completion checklist requiring updated documentation, compatibility review, relevant automated tests, and runtime/multiplayer evidence proportional to risk. | D2 component catalog and audit | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |

## P1 - Native Runtime Access and Game-Update Compatibility

| ID | Depth | TODO | Work package | Required completion evidence |
|---|---:|---|---|---|
| T125 | 0 | Inventory known native-member access through `Common.GetVal`, `Common.SetVal`, `Common.RunFunc`, local reflection bindings, `Traverse`, and repeated `FieldInfo`, `PropertyInfo`, or `MethodInfo` lookup. | N1 native boundary verification | Pinned assembly verifier identifies owner and exact target; focused runtime evidence confirms the call path. |
| T126 | 0 | Migrate known native members incrementally to cached typed `Internals()` accessors under `AGMLIB/Nebulous`. | N1 native boundary verification | Pinned assembly verifier identifies owner and exact target; focused runtime evidence confirms the call path. |
| T127 | 0 | Record the declaring native type, member name, expected member type/signature, and owning AGMLIB feature for every native accessor and Harmony target. | N1 native boundary verification | Pinned assembly verifier identifies owner and exact target; focused runtime evidence confirms the call path. |
| T128 | 0 | Add a verifier that reports missing or changed native accessor and Harmony targets against the currently supported NEBULOUS assemblies. | N1 native boundary verification | Pinned assembly verifier identifies owner and exact target; focused runtime evidence confirms the call path. |
| T129 | 0 | Fail or disable only the owning feature with an actionable diagnostic when a required native target is unavailable; avoid unrelated partial initialization. | N1 native boundary verification | Pinned assembly verifier identifies owner and exact target; focused runtime evidence confirms the call path. |
| T130 | 0 | Create a repeatable NEBULOUS update workflow that: | N1 native boundary verification | Workflow dry run or Actions run proves permissions, trigger, artifact, failure, and concurrency behavior. |
| T131 | 1 | Records the previous and new game versions and assembly hashes. | N1 native boundary verification | One explicit version is shown to match assembly, manifest, package, and release metadata. |
| T132 | 1 | Refreshes configured native assemblies without silently committing unrelated binaries. | N1 native boundary verification | Pinned assembly verifier identifies owner and exact target; focused runtime evidence confirms the call path. |
| T133 | 1 | Compares native types, fields, methods, signatures, and relevant prefab YAML. | N1 native boundary verification | Pinned assembly verifier identifies owner and exact target; focused runtime evidence confirms the call path. |
| T134 | 1 | Rebuilds AGMLIB and runs compatibility validation. | N1 native boundary verification | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T135 | 1 | Runs component smoke tests and records regressions before declaring support. | N1 native boundary verification | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |

## P1 - Gameplay Correctness From Native and Prefab Correlation

| ID | Depth | TODO | Work package | Required completion evidence |
|---|---:|---|---|---|
| T136 | 0 | Make dynamic resource reduction use one typed effective-demand | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T137 | 1 | Remove the current double application in the replacement editor | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T138 | 1 | Define integer rounding and stacking order once and cover multiple | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T139 | 1 | Migrate known ship resource pools, required values, provider/consumer | F1 native/data gameplay parity | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |
| T140 | 1 | Profile root-wide reduction discovery and remove unused `AmountExtra` | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T141 | 0 | Finish power-status UX against native resource semantics. | F1 native/data gameplay parity | One explicit version is shown to match assembly, manifest, package, and release metadata. |
| T142 | 1 | Distinguish supply coverage from demand utilization in labels and math. | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T143 | 1 | Label peak values as lifetime maxima and decide whether they belong in | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T144 | 1 | Migrate known power-icon, image, tooltip, and status-display internals | F1 native/data gameplay parity | Pinned assembly verifier identifies owner and exact target; focused runtime evidence confirms the call path. |
| T145 | 1 | Verify ship switching, damage-control-panel close/reopen, throttle | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T146 | 0 | Replace the experimental craft launch limit with a server-owned command | F1 native/data gameplay parity | Opt-in local scenario records paths, process state, hashes, result, and recovery. |
| T147 | 1 | Choose player, ship, carrier, provider, group, craft, or weighted scope | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T148 | 1 | Define whether manned and unmanned craft share capacity. | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T149 | 1 | Allow queued replacements and promote them when active capacity frees. | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T150 | 1 | Keep command capacity independent from pad/storage reservation so | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T151 | 1 | Cover player launches, AI sorties, cancellation, recovery, loss, | F1 native/data gameplay parity | Opt-in local scenario records paths, process state, hashes, result, and recovery. |
| T152 | 1 | Remove private traffic-order invalid-enum mutation and process-wide | F1 native/data gameplay parity | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T153 | 0 | Validate configurable hangar work-slot references and avoid repeated | F1 native/data gameplay parity | Dependency manifest records source, version, license, hash, necessity, and clean-build result. |
| T154 | 0 | Add prefab validation gates for authored behavior that code alone cannot | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T155 | 1 | directed versus omnidirectional EWAR muzzle rotation; | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T156 | 1 | continuous-beam effect and audio reference graphs; | F1 native/data gameplay parity | Dependency manifest records source, version, license, hash, necessity, and clean-build result. |
| T157 | 1 | loitering trigger layer, communicator, antenna, network behavior, and | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T158 | 1 | socket stable keys, ordered indices, resize links, and stage references; | F1 native/data gameplay parity | Dependency manifest records source, version, license, hash, necessity, and clean-build result. |
| T159 | 1 | hangar storage, work-slot, pad, resource, animation, and traversal | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T160 | 0 | Replace reflected debuff add/remove/state access with typed accessors and | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T161 | 0 | Repair command-seeker state ownership, retarget math, launch/in-flight | F1 native/data gameplay parity | Opt-in local scenario records paths, process state, hashes, result, and recovery. |
| T162 | 0 | Make ammo-mode cycle/resource application change-driven, validate unknown | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T163 | 0 | Align SIGINT track release, provider subscription, ownership change, and | F1 native/data gameplay parity | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T164 | 0 | Replace `AdvancedRadar`'s recursive track-update prefix with an | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T165 | 1 | Prevent failed-ping recursion and successful mixed-track double updates. | F1 native/data gameplay parity | Changed-file checker passes and attributes/editor settings agree. |
| T166 | 1 | Make cycle time per sensor, validate positive finite values, and avoid | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T167 | 1 | Reuse typed native detection state instead of duplicating | F1 native/data gameplay parity | Pinned assembly verifier identifies owner and exact target; focused runtime evidence confirms the call path. |
| T168 | 1 | Verify forced burnthrough power, self-damage, resource, lock, and | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T169 | 0 | Make Doppler settings a validated separately attached component | F1 native/data gameplay parity | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |
| T170 | 1 | Add deterministic angular, radial-floor, minimum-speed, and | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T171 | 1 | Give the static contact registry explicit sensor/target/scene teardown. | F1 native/data gameplay parity | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |
| T172 | 1 | Keep the TacView overlay presentation-only and migrate its known native | F1 native/data gameplay parity | Pinned assembly verifier identifies owner and exact target; focused runtime evidence confirms the call path. |
| T173 | 0 | Stabilize the remaining fire-control and IFF experiments. | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T174 | 1 | Preserve `ActiveFireControlSensorOptions` modified-range intent with | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T175 | 1 | Determine whether `MultiSensor` and | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T176 | 1 | Validate track-logic weights, propagate update failure, and prohibit | F1 native/data gameplay parity | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T177 | 1 | Replace `IFFComponent`'s inverted re-enable path, direct stat-cache | F1 native/data gameplay parity | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |
| T178 | 0 | Prove charge-up beam server/client event ordering and pooled impact-effect | F1 native/data gameplay parity | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |
| T179 | 0 | Track the native `CraftLandingPad.SupportsLaunch` versus | F1 native/data gameplay parity | Opt-in local scenario records paths, process state, hashes, result, and recovery. |

## P1 - Namespace and Harmony Patch Governance

| ID | Depth | TODO | Work package | Required completion evidence |
|---|---:|---|---|---|
| T180 | 0 | Inventory current namespaces, global public types, Harmony patches, Harmony IDs, and patched native methods. | N2 namespace and patch governance | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T181 | 0 | Choose and document the namespace policy for all new code. | N2 namespace and patch governance | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T182 | 0 | Do not mass-rename released types solely for namespace consistency. | N2 namespace and patch governance | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T183 | 0 | Plan compatibility shims or migration paths before moving existing public types. | N2 namespace and patch governance | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T184 | 0 | Define a diagnostic patch-class naming convention such as `TargetClassTargetMethodPatch` for new patches. | N2 namespace and patch governance | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T185 | 0 | Replace blanket assembly-wide patch activation incrementally with feature-owned registration where safe. | N2 namespace and patch governance | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T186 | 0 | Assign stable Harmony IDs per feature area where isolation is useful. | N2 namespace and patch governance | Pinned assembly verifier identifies owner and exact target; focused runtime evidence confirms the call path. |
| T187 | 0 | Add explicit availability, configuration, lifecycle, and multiplayer-authority checks before behavior-changing patches run. | N2 namespace and patch governance | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |
| T188 | 0 | Create a generated or script-produced patch inventory for diagnostics. | N2 namespace and patch governance | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T189 | 0 | Make patch failures identify the owning feature and native target. | N2 namespace and patch governance | Pinned assembly verifier identifies owner and exact target; focused runtime evidence confirms the call path. |
| T190 | 0 | Ensure unpatch/reload behavior is defined for any feature that supports runtime toggling. | N2 namespace and patch governance | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |

## P2 - Tests and CI-Safe Validation

| ID | Depth | TODO | Work package | Required completion evidence |
|---|---:|---|---|---|
| T191 | 0 | Create a conventional test project that does not require launching Unity or NEBULOUS. | T1 test and fixture architecture | Opt-in local scenario records paths, process state, hashes, result, and recovery. |
| T192 | 0 | Add tests for socket-filter composition and lookup behavior. | T1 test and fixture architecture | Deterministic test is repeatable, opt-in where runtime is required, and fails for the intended regression. |
| T193 | 0 | Add tests for serialization and configuration parsing that can run without game assemblies where practical. | T1 test and fixture architecture | Deterministic test is repeatable, opt-in where runtime is required, and fails for the intended regression. |
| T194 | 0 | Build a minimal AssetBundle round-trip fixture that reproduces the editor-valid but bundle-loaded-null custom class/struct failure. | T1 test and fixture architecture | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T195 | 0 | Test custom class and struct fields, arrays/lists, nested values, inheritance, `[SerializeReference]`, top-level Unity object references, assembly/type identity, stripping, and bundle build settings through the real mod load path. | T1 test and fixture architecture | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T196 | 0 | Compare editor state, built bundle contents, post-load prefab state, instantiated state, and Player.log evidence for every fixture case. | T1 test and fixture architecture | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |
| T197 | 0 | Determine whether the failure is caused by type/assembly identity, Unity managed-reference metadata, build stripping, bundle dependency resolution, game/Unity version differences, or another pipeline stage. | T1 test and fixture architecture | One explicit version is shown to match assembly, manifest, package, and release metadata. |
| T198 | 0 | Document a supported complex-data representation and migration path only after it passes the released AssetBundle build/load path and target game versions. | T1 test and fixture architecture | One explicit version is shown to match assembly, manifest, package, and release metadata. |
| T199 | 0 | Add tests for pure targeting, scoring, geometry, and math helpers. | T1 test and fixture architecture | Deterministic test is repeatable, opt-in where runtime is required, and fails for the intended regression. |
| T200 | 0 | Add regression tests when extracting logic from Harmony patches into typed helpers. | T1 test and fixture architecture | Pinned assembly verifier identifies owner and exact target; focused runtime evidence confirms the call path. |
| T201 | 0 | Add public API baseline validation to CI. | T1 test and fixture architecture | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T202 | 0 | Add `ModInfo.xml` schema/content validation. | T1 test and fixture architecture | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T203 | 0 | Add package allowlist validation. | T1 test and fixture architecture | Disposable package validation records allowlist, layout, hashes, and rollback artifact. |
| T204 | 0 | Add validation that assembly, manifest, and package versions match. | T1 test and fixture architecture | One explicit version is shown to match assembly, manifest, package, and release metadata. |
| T205 | 0 | Add a mixed-line-ending check for touched C# and VB files. | T1 test and fixture architecture | Changed-file checker passes and attributes/editor settings agree. |
| T206 | 0 | Add a check that normal build/test commands leave tracked files unchanged. | T1 test and fixture architecture | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T207 | 0 | Add a CI-safe PowerShell smoke test for repository scripts. | T1 test and fixture architecture | Deterministic test is repeatable, opt-in where runtime is required, and fails for the intended regression. |
| T208 | 0 | Keep runtime/editor reproduction in the `neb-testing` workflow and record the smallest repeatable scenarios. | T1 test and fixture architecture | Workflow dry run or Actions run proves permissions, trigger, artifact, failure, and concurrency behavior. |
| T209 | 0 | Add log assertions for successful AGMLIB load and known high-risk Harmony patch failures. | T1 test and fixture architecture | Pinned assembly verifier identifies owner and exact target; focused runtime evidence confirms the call path. |
| T210 | 0 | Build a compatibility fixture corpus containing representative legacy prefabs, fleets, saves, and component configurations from known-good AGMLIB releases. | T1 test and fixture architecture | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T211 | 0 | Exercise applicable fixtures through load, instantiate, clone, pool/unpool, launch, save, and reload paths. | T1 test and fixture architecture | Opt-in local scenario records paths, process state, hashes, result, and recovery. |
| T212 | 0 | Add an opt-in component smoke-test matrix covering editor construction, runtime initialization, normal operation, teardown, scene transition, and mod unload/reload where supported. | T1 test and fixture architecture | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |
| T213 | 0 | Add a multiplayer component matrix covering offline play, host, remote client, and dedicated server where applicable. | T1 test and fixture architecture | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |
| T214 | 0 | Record for each networked component which side owns state mutation, validation, spawning, damage, resource consumption, and replication. | T1 test and fixture architecture | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |
| T215 | 0 | Add regression fixtures for every approved namespace, type-name, serialized-field, or save-key migration. | T1 test and fixture architecture | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |

## P2 - Runtime Lifecycle, Resource Ownership, and Performance

| ID | Depth | TODO | Work package | Required completion evidence |
|---|---:|---|---|---|
| T216 | 0 | Audit runtime sidecars for explicit ownership, one-instance-per-owner guarantees, rollout boundaries, and cleanup when the native owner is destroyed, pooled, replaced, or unloaded. | T2 lifecycle and performance audit | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |
| T217 | 0 | Audit component event subscriptions, callbacks, coroutines, delayed invocations, static registries, and caches for deterministic teardown and scene-transition safety. | T2 lifecycle and performance audit | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |
| T218 | 0 | Audit dynamically created `GameObject`, `Component`, `Material`, mesh, texture, and other Unity object ownership; destroy or transfer ownership explicitly. | T2 lifecycle and performance audit | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |
| T219 | 0 | Verify pooled and cloned components reset transient state, cached references, timers, collections, and network ownership before reuse. | T2 lifecycle and performance audit | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |
| T220 | 0 | Profile component hot paths for per-frame allocations, repeated reflection, LINQ, scene searches, redundant `GetComponent` calls, and avoidable cache rebuilds. | T2 lifecycle and performance audit | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T221 | 0 | Establish representative CPU, allocation, and log-volume baselines for high-risk components and record the test scenario used. | T2 lifecycle and performance audit | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |
| T222 | 0 | Add performance regression checks where stable automation is practical, and require before/after profiles for changes to known hot paths. | T2 lifecycle and performance audit | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |

## P2 - Packaging and Release Safety

| ID | Depth | TODO | Work package | Required completion evidence |
|---|---:|---|---|---|
| T223 | 0 | Define the exact allowlist for an AGMLIB Workshop/GitHub release package. | R1 package validation | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T224 | 0 | Decide and document whether `0Harmony.dll` must be bundled by AGMLIB. | R1 package validation | Pinned assembly verifier identifies owner and exact target; focused runtime evidence confirms the call path. |
| T225 | 0 | Reject backup projects, test fixtures, symbols, build logs, and unrelated dependencies from publishable packages. | R1 package validation | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T226 | 0 | Generate a package manifest with SHA-256 hashes. | R1 package validation | Disposable package validation records allowlist, layout, hashes, and rollback artifact. |
| T227 | 0 | Preserve the last known-good package for rollback. | R1 package validation | Non-publishing test covers validation; authorized publish evidence records sanitized manifest and post-publish verification. |
| T228 | 0 | Verify packages by extracting or copying them into a disposable directory before publication. | R1 package validation | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T229 | 0 | Validate Workshop and local-install layouts independently. | R1 package validation | Non-publishing test covers validation; authorized publish evidence records sanitized manifest and post-publish verification. |
| T230 | 0 | Add release notes covering game-version compatibility and public API changes. | R1 package validation | One explicit version is shown to match assembly, manifest, package, and release metadata. |

## P2 - Dependency and Binary Hygiene

| ID | Depth | TODO | Work package | Required completion evidence |
|---|---:|---|---|---|
| T231 | 0 | Inventory the 46 tracked assemblies under `AGMLIB/libs/` and the tracked BepInEx/UnityExplorer files. | B3 dependency provenance | Dependency manifest records source, version, license, hash, necessity, and clean-build result. |
| T232 | 0 | Record source, version, license/provenance, and expected hash for every tracked binary dependency. | B3 dependency provenance | One explicit version is shown to match assembly, manifest, package, and release metadata. |
| T233 | 0 | Add or update third-party notices as required. | B3 dependency provenance | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T234 | 0 | Review whether game and Unity assemblies may or should remain committed; document the decision. | B3 dependency provenance | Dependency manifest records source, version, license, hash, necessity, and clean-build result. |
| T235 | 0 | Consider a controlled sync script that copies required assemblies from a configured NEBULOUS installation and verifies hashes. | B3 dependency provenance | Dependency manifest records source, version, license, hash, necessity, and clean-build result. |
| T236 | 0 | Remove references that are not needed by the compiled runtime assembly after verifying a clean build. | B3 dependency provenance | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T237 | 0 | Specifically verify whether the UnityEditor references are necessary. | B3 dependency provenance | Dependency manifest records source, version, license, hash, necessity, and clean-build result. |
| T238 | 0 | Separate editor/authoring-only code into another project or assembly if it needs UnityEditor APIs. | B3 dependency provenance | Dependency manifest records source, version, license, hash, necessity, and clean-build result. |
| T239 | 0 | Avoid silently updating tracked binary dependencies without a compatibility review and manifest update. | B3 dependency provenance | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |

## P2 - Source and Project Organization

| ID | Depth | TODO | Work package | Required completion evidence |
|---|---:|---|---|---|
| T240 | 0 | Remove or archive `AGMLIB/AGMLIB - Backup.csproj` after confirming it has no unique required configuration. | A1 source consolidation | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T241 | 0 | Add `Directory.Build.props` or equivalent shared configuration if additional projects are introduced. | A1 source consolidation | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T242 | 0 | Review the largest source files and extract feature-focused helpers when they are next modified: | A1 source consolidation | Compatibility review and focused tests precede extraction; public identities remain or receive shims. |
| T243 | 1 | `Generic Gameplay/Ewar/PassiveCommsSensorComponent.cs`. | A1 source consolidation | Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition. |
| T244 | 1 | `Munitions/ModularMissile/CommandSeekers.cs`. | A1 source consolidation | Task-routed document is linked, checked for freshness, and does not duplicate canonical detail. |
| T245 | 1 | `Dynamic Systems/UI/ShipStatusPowerBar.cs`. | A1 source consolidation | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T246 | 1 | `Editor/SocketFilterCore.cs`. | A1 source consolidation | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T247 | 0 | Move reusable behavior out of Harmony patch classes into typed domain helpers. | A1 source consolidation | Pinned assembly verifier identifies owner and exact target; focused runtime evidence confirms the call path. |
| T248 | 0 | Inventory unfinished and legacy code and decide whether to maintain, isolate, exclude, or remove it. | A1 source consolidation | Compatibility review and focused tests precede extraction; public identities remain or receive shims. |
| T249 | 0 | Keep runtime, authoring/editor, diagnostics, and experimental code boundaries explicit. | A1 source consolidation | Compatibility review and focused tests precede extraction; public identities remain or receive shims. |

## P2 - Line Endings and Repository Hygiene

| ID | Depth | TODO | Work package | Required completion evidence |
|---|---:|---|---|---|
| T250 | 0 | Add explicit `*.cs text eol=crlf` and `*.vb text eol=crlf` rules to `.gitattributes`. | B4 repository hygiene | Changed-file checker passes and attributes/editor settings agree. |
| T251 | 0 | Verify `.editorconfig` and `.gitattributes` agree on line endings. | B4 repository hygiene | Changed-file checker passes and attributes/editor settings agree. |
| T252 | 0 | Add a script or CI check that detects mixed line endings in changed C# and VB files. | B4 repository hygiene | Changed-file checker passes and attributes/editor settings agree. |
| T253 | 0 | Confirm generated files and local game-install artifacts are ignored. | B4 repository hygiene | Workflow dry run or Actions run proves permissions, trigger, artifact, failure, and concurrency behavior. |
| T254 | 0 | Confirm Unity cache, logs, build outputs, and Workshop staging directories cannot be committed accidentally. | B4 repository hygiene | Documented clean command succeeds and `git status --porcelain` is unchanged. |

## P3 - Optional Documentation Generation

| ID | Depth | TODO | Work package | Required completion evidence |
|---|---:|---|---|---|
| T255 | 0 | Reassess documentation discovery after the scoped agent map, API policy, and release documentation exist. | D3 generated navigation review | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T256 | 0 | If navigation becomes difficult, add a small curated `llms.txt` or structured index generated from canonical metadata. | D3 generated navigation review | Task-routed document is linked, checked for freshness, and does not duplicate canonical detail. |
| T257 | 0 | Keep generated documentation reproducible and add a `-Check` mode to CI. | D3 generated navigation review | Task-routed document is linked, checked for freshness, and does not duplicate canonical detail. |
| T258 | 0 | Avoid committing duplicated full-context documents that materially inflate repository context. | D3 generated navigation review | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |
| T259 | 0 | Prefer task-first indexes and concise routing over loading every historical note into agent context. | D3 generated navigation review | The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up. |

## Completion Definition

| ID | Depth | TODO | Work package | Required completion evidence |
|---|---:|---|---|---|
| T260 | 0 | A clean checkout can build and package AGMLIB without editing tracked files. | G1 final release-readiness gate | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T261 | 0 | Pull requests receive a reliable build and compatibility result. | G1 final release-readiness gate | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T262 | 0 | Releases are intentional, versioned, validated, and reproducible. | G1 final release-readiness gate | One explicit version is shown to match assembly, manifest, package, and release metadata. |
| T263 | 0 | Workshop updates use a dedicated skill with explicit authorization and safety checks. | G1 final release-readiness gate | Non-publishing test covers validation; authorized publish evidence records sanitized manifest and post-publish verification. |
| T264 | 0 | Public and serialized contracts are protected by an authoritative baseline. | G1 final release-readiness gate | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
| T265 | 0 | New contributors and agents can find the correct build, test, deploy, and debugging paths quickly. | G1 final release-readiness gate | Documented clean command succeeds and `git status --porcelain` is unchanged. |
| T266 | 0 | Existing downstream mods retain compatibility or receive an explicit migration path. | G1 final release-readiness gate | Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change. |
