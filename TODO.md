# AGMLIB Improvement TODO

This backlog captures the repository, build, CI, compatibility, documentation, testing, and release improvements identified during the `rosscomrie/one-more-jump` comparison.

## Working Principles

- [ ] Preserve released public APIs, serialized Unity contracts, save keys, Harmony IDs, and Workshop compatibility unless a migration is explicitly designed.
- [ ] Keep normal builds reproducible and free of tracked-file mutations.
- [ ] Separate build, package, local deploy, game testing, and Workshop publishing into explicit operations.
- [ ] Preserve CRLF line endings in C# and VB files and verify touched files do not contain mixed line endings.
- [ ] Keep new agent documentation concise and task-routed; do not copy One More Jump's full generated-document system unless AGMLIB grows enough to need it.

## P0 - Reproducible, Non-Mutating Builds

- [ ] Record the current build inputs, outputs, version behavior, and deployment behavior before changing them.
- [ ] Stop `AGMLIB/build.bat` from rewriting `AGMLIB.csproj` during ordinary builds.
- [ ] Stop ordinary builds from rewriting tracked `AGMLIB/ModInfo.xml`.
- [ ] Replace the current automatic revision increment with an explicit version source:
  - [ ] Choose a committed version file, Git tag, or release-supplied version.
  - [ ] Ensure local Debug builds do not change the release version.
  - [ ] Ensure assembly, package, and `ModInfo.xml` versions are generated from the same value.
- [ ] Generate the publishable `ModInfo.xml` in the build/package output rather than editing the tracked source copy.
- [ ] Replace the hard-coded `BaseOutputPath` and post-build destination with configurable MSBuild properties.
- [ ] Add configurable `NebulousInstallDir` and `ModInstallDir` properties with documented local defaults.
- [ ] Add a `DeployToGame` property that defaults to `false`.
- [ ] Keep normal build output under `bin/` or a repository-local `artifacts/` directory.
- [ ] Make local deployment an explicit command instead of an unconditional build side effect.
- [ ] Replace Windows-only inline `call` behavior where practical with MSBuild or PowerShell that can be tested independently.
- [ ] Add a verification command that performs a clean build and fails if tracked files change.
- [ ] Confirm Debug and Release package layouts use the paths expected by NEBULOUS.
- [ ] Document what happens when NEBULOUS is running and an assembly is locked.

## P0 - Repair CI and Release Automation

- [ ] Replace `.github/workflows/main.yml` with separate build and release workflows or clearly separated jobs.
- [ ] Run the initial build workflow on `windows-latest` while the project still requires Windows tooling.
- [ ] Give the PR/build job read-only repository permissions.
- [ ] Run restore and build without deploying to a local game installation.
- [ ] Remove the empty `dotnet test` step until a real test project exists, or add the test project first.
- [ ] Upload artifacts from a repository-local build/package directory.
- [ ] Update GitHub Actions dependencies to currently supported versions.
- [ ] Remove the archived `actions/create-release@v1` action.
- [ ] Remove the unsupported `actions/upload-artifact@v3` action.
- [ ] Trigger releases only from an intentional version tag or `workflow_dispatch`, never from every push or pull request.
- [ ] Use a unique semantic version tag instead of the fixed `Automatic` tag.
- [ ] Generate release notes or require an explicit changelog/release-notes input.
- [ ] Use `gh release create` or another maintained release mechanism.
- [ ] Grant `contents: write` only to the release job.
- [ ] Validate the package before creating a GitHub release.
- [ ] Fail when the expected DLL, manifest, or package artifact is missing.
- [ ] Add workflow concurrency rules to prevent two releases for the same version.
- [ ] Add a manual CI-safe package build for maintainers to test release output without publishing.

## P1 - Agent Guidance and Repository Documentation

- [ ] Expand root `AGENTS.md` while keeping it short enough to remain always relevant.
- [ ] Add a repository map covering:
  - [ ] `Advanced/` and `Advanced/Legacy/`.
  - [ ] `Common/`.
  - [ ] `Craft/`.
  - [ ] `Dynamic Systems/`.
  - [ ] `Editor/`.
  - [ ] `FX/`.
  - [ ] `Generic Gameplay/`.
  - [ ] `Munitions/`.
  - [ ] `Server/` and `Systems/`.
- [ ] State that new work should not be added under `Advanced/Legacy/` unless a task explicitly targets legacy behavior.
- [ ] Add canonical commands for restore, build, package, deploy, launch, reproduce, and log inspection.
- [ ] Distinguish CI-safe validation from operations that modify the game installation or launch NEBULOUS.
- [ ] Add public API and serialization rules:
  - [ ] Do not casually rename released public types.
  - [ ] Treat `MonoBehaviour`, `ScriptableObject`, public enum, serialized-field, and save-key names as durable contracts.
  - [ ] Require migration or compatibility aliases for intentional breaking changes.
  - [ ] Require explicit numeric values for public serialized enums.
  - [ ] Prohibit nested AGMLIB-defined custom classes and structs as AssetBundle-authored serialized payloads until the released bundle-load path proves them safe.
  - [ ] Require complex authoring data to use bundle-proven Unity/native types, validated flattened lists/indices, or separately attached component references in the interim.
- [ ] Add namespace rules for new code: use a chosen `AGMLIB.*` namespace and do not introduce new global-namespace public types.
- [ ] Add Harmony guidance for stable IDs, small patch entrypoints, typed helpers, lifecycle/authority checks, and feature ownership.
- [ ] Add generated-file, binary-reference, and packaging boundaries.
- [ ] Route build/deploy/game reproduction tasks to `.agents/skills/neb-testing/SKILL.md`.
- [ ] Create `.agents/AGENTS.md` as the deeper map for skills, compatibility baselines, durable notes, and task plans.
- [ ] Decide when a finding deserves a durable `.agents/notes/` document instead of expanding `AGENTS.md`.
- [ ] Expand the root `AGENTS.md` with:
  - [ ] Project purpose and supported NEBULOUS version.
  - [ ] Installation and Workshop information.
  - [ ] Prerequisites and dependency setup.
  - [ ] Local build, package, and deploy instructions.
  - [ ] Repository architecture and major feature areas.
  - [ ] Public API stability policy.
  - [ ] Testing and log-triage instructions.
  - [ ] Contributor guidance and links to scoped `AGENTS.md` files.
- [ ] Do not add `llms-full.txt` or generated agent indexes until the documentation set is large enough to justify their context and maintenance cost.

## P1 - Create a Workshop Update Publishing Skill

- [ ] Use the skill-creator workflow to create a dedicated AGMLIB Workshop publishing skill.
- [ ] Choose a clear trigger-oriented name, such as `agmlib-workshop-publish`.
- [ ] Place the skill under `.agents/skills/` and add it to `.agents/AGENTS.md`.
- [ ] Treat Workshop item `2960504230` as configuration that must be verified before publishing, not as an unchecked assumption.
- [ ] Document ownership and authorization requirements for updating the Workshop item.
- [ ] Keep Steam credentials, session tokens, guard codes, and machine-specific secrets outside Git and outside generated logs.
- [ ] Decide whether publishing will use SteamCMD, an existing Workshop tool, or a maintained API/CLI wrapper.
- [ ] Create or wrap a repository script for the mechanical publish operation rather than embedding a large command sequence directly in `SKILL.md`.
- [ ] Require an explicit publish action; never publish as a side effect of build, package, test, or deploy.
- [ ] Make dry-run/package-validation mode the default where the selected publishing tool permits it.
- [ ] Require the following preflight checks:
  - [ ] Working tree and intended commit are reported.
  - [ ] Release version is explicit and matches the assembly and `ModInfo.xml`.
  - [ ] Release configuration builds successfully.
  - [ ] Package allowlist validation succeeds.
  - [ ] Required DLL, manifest, preview image, and configuration files exist.
  - [ ] No backup, debug-only, test, symbol, credential, or unrelated files are present.
  - [ ] The target Workshop item ID and content directory are shown before upload.
  - [ ] NEBULOUS and other processes are not locking package files.
  - [ ] Release notes/change description are supplied and reviewed.
- [ ] Require a final explicit confirmation immediately before the external publish action.
- [ ] Print or save a sanitized publish manifest containing file hashes, version, commit, item ID, and timestamp.
- [ ] Capture SteamCMD/tool exit status and fail loudly on partial uploads or ambiguous results.
- [ ] Add post-publish verification:
  - [ ] Confirm the Workshop item reports the expected update time/version.
  - [ ] Confirm a clean client can download the updated content.
  - [ ] Confirm the downloaded package passes the same validation checks.
  - [ ] Launch and verify that AGMLIB loads successfully when runtime validation is requested.
- [ ] Document rollback/recovery steps for a bad Workshop update, including preservation of the last known-good package.
- [ ] Ensure the skill distinguishes GitHub release publishing, local mod deployment, and Steam Workshop publishing.
- [ ] Add a non-publishing smoke test for the skill's scripts and validation logic.
- [ ] Add AGENTS guidance stating that Workshop publishing is an external, irreversible operation requiring explicit user authorization.

## P1 - Public API and Serialization Compatibility

- [ ] Generate an initial authoritative AGMLIB compatibility baseline from a known-good release assembly.
- [ ] Record public type full names, including nested types where consumers can reference them.
- [ ] Record public methods, properties, fields, constructors, and interfaces that are intended as supported API.
- [ ] Record public enum member names and numeric values.
- [ ] Record public `MonoBehaviour` and `ScriptableObject` type names.
- [ ] Record serialized public fields, save keys, config keys, and other Unity/XML identity-sensitive names.
- [ ] Record stable Harmony IDs and patch ownership.
- [ ] Record the assembly version, game version, and source commit associated with the baseline.
- [ ] Compare the baseline against One More Jump's committed AGMLIB public-name snapshot and resolve discrepancies.
- [ ] Create a CI-safe compatibility checker that fails on accidental removal, rename, namespace change, enum-value change, or serialized-field loss.
- [ ] Require an explicit baseline update and migration note for intentional breaking changes.
- [ ] Add collision checks for public serialized/editor-visible simple type names where NEBULOUS or Unity resolves by simple name.
- [ ] Publish the compatibility policy in the appropriate scoped `AGENTS.md`.

## P1 - Component Documentation, Audit, and Namespace Compatibility

- [ ] Create a `docs/` folder that inventories and documents every AGMLIB component.
- [ ] Define which public and serialized types qualify as components and generate the authoritative component inventory from source or assembly metadata.
- [ ] Create a standard component-document template and require consistent identity, compatibility, lifecycle, configuration, usage, and testing sections.
- [ ] For each component, document its purpose, configuration, serialized fields, lifecycle, native-game dependencies, runtime behavior, known limitations, and a minimal usage example.
- [ ] Link each component document to its source type and record whether it is a `MonoBehaviour`, `ScriptableObject`, descriptor, runtime component, editor component, or supporting type.
- [ ] Add a CI-safe documentation coverage and freshness check so newly added, renamed, moved, or removed components cannot be omitted silently.
- [ ] Review every component systematically for bugs, including serialization, cloning/pooling, lifecycle ordering, null handling, multiplayer authority, editor/runtime parity, and native-version compatibility.
- [ ] Record each audit result, reproduction evidence, severity, and follow-up action; add regression coverage when a bug is fixed.
- [ ] Inventory component dependencies, initialization order, shared native/Harmony targets, optional integrations, and known component conflicts.
- [ ] Add authoring-time or load-time validation for missing references, invalid ranges, duplicate or missing save keys, incompatible combinations, and configuration that would otherwise fail only at runtime.
- [ ] Define component diagnostics conventions with stable prefixes, useful identity and state context, configurable verbosity, and suppression or rate limiting for repeated messages.
- [ ] Remove or gate noisy development logging, especially logging reachable from `Update`, `FixedUpdate`, `LateUpdate`, or other high-frequency callbacks.
- [ ] Inventory component namespaces and identify types that are missing, inconsistent, or incorrectly placed in namespaces.
- [ ] Determine whether each proposed namespace correction can preserve existing Unity serialization, saved content, prefab references, XML identities, and downstream binary compatibility.
- [ ] Test namespace migrations against representative existing fleets, prefabs, save data, and third-party mod references before adopting them.
- [ ] Use compatibility shims, migration metadata, or retained legacy type aliases where proven safe; do not move a released component type when compatibility cannot be demonstrated.
- [ ] Define a deprecation policy for legacy component types, namespace shims, serialized fields, save keys, and public APIs, including support windows, migration notes, and removal criteria.
- [ ] Define a component change completion checklist requiring updated documentation, compatibility review, relevant automated tests, and runtime/multiplayer evidence proportional to risk.

## P1 - Native Runtime Access and Game-Update Compatibility

- [ ] Inventory known native-member access through `Common.GetVal`, `Common.SetVal`, `Common.RunFunc`, local reflection bindings, `Traverse`, and repeated `FieldInfo`, `PropertyInfo`, or `MethodInfo` lookup.
- [ ] Migrate known native members incrementally to cached typed `Internals()` accessors under `AGMLIB/Nebulous`.
- [ ] Record the declaring native type, member name, expected member type/signature, and owning AGMLIB feature for every native accessor and Harmony target.
- [ ] Add a verifier that reports missing or changed native accessor and Harmony targets against the currently supported NEBULOUS assemblies.
- [ ] Fail or disable only the owning feature with an actionable diagnostic when a required native target is unavailable; avoid unrelated partial initialization.
- [ ] Create a repeatable NEBULOUS update workflow that:
  - [ ] Records the previous and new game versions and assembly hashes.
  - [ ] Refreshes configured native assemblies without silently committing unrelated binaries.
  - [ ] Compares native types, fields, methods, signatures, and relevant prefab YAML.
  - [ ] Rebuilds AGMLIB and runs compatibility validation.
  - [ ] Runs component smoke tests and records regressions before declaring support.

## P1 - Gameplay Correctness From Native and Prefab Correlation

- [ ] Make dynamic resource reduction use one typed effective-demand
  calculation for runtime allocation and fleet-editor presentation.
  - [ ] Remove the current double application in the replacement editor
    resource summary.
  - [ ] Define integer rounding and stacking order once and cover multiple
    reductions.
  - [ ] Migrate known ship resource pools, required values, provider/consumer
    lists, and editor summary members to typed native accessors.
  - [ ] Profile root-wide reduction discovery and remove unused `AmountExtra`
    state if it has no intended role.
- [ ] Finish power-status UX against native resource semantics.
  - [ ] Distinguish supply coverage from demand utilization in labels and math.
  - [ ] Label peak values as lifetime maxima and decide whether they belong in
    the shared graph scale.
  - [ ] Migrate known power-icon, image, tooltip, and status-display internals
    to typed accessors.
  - [ ] Verify ship switching, damage-control-panel close/reopen, throttle
    changes, starvation, destruction, host, and remote-client behavior.
- [ ] Replace the experimental craft launch limit with a server-owned command
  channel while preserving the native launch queue.
  - [ ] Choose player, ship, carrier, provider, group, craft, or weighted scope
    explicitly.
  - [ ] Define whether manned and unmanned craft share capacity.
  - [ ] Allow queued replacements and promote them when active capacity frees.
  - [ ] Keep command capacity independent from pad/storage reservation so
    capped launches cannot starve landings.
  - [ ] Cover player launches, AI sorties, cancellation, recovery, loss,
    save/reload, and two carriers owned by the same player.
  - [ ] Remove private traffic-order invalid-enum mutation and process-wide
    transient counting.
- [ ] Validate configurable hangar work-slot references and avoid repeated
  combined-array allocation if profiling shows it is material.
- [ ] Add prefab validation gates for authored behavior that code alone cannot
  guarantee:
  - [ ] directed versus omnidirectional EWAR muzzle rotation;
  - [ ] continuous-beam effect and audio reference graphs;
  - [ ] loitering trigger layer, communicator, antenna, network behavior, and
    per-seeker fuse tables;
  - [ ] socket stable keys, ordered indices, resize links, and stage references;
  - [ ] hangar storage, work-slot, pad, resource, animation, and traversal
    contracts.
- [ ] Replace reflected debuff add/remove/state access with typed accessors and
  define duplicate, multiplication, refresh, expiry, destruction, and
  save/reload behavior.
- [ ] Repair command-seeker state ownership, retarget math, launch/in-flight
  mode separation, pool reset, ranged base-state drift, and native EMCON
  `OverrideComms` integration.
- [ ] Make ammo-mode cycle/resource application change-driven, validate unknown
  resources, and either implement or remove authored-but-unused failure policy.
- [ ] Align SIGINT track release, provider subscription, ownership change, and
  teardown with the native sensor lifecycle.
- [ ] Replace `AdvancedRadar`'s recursive track-update prefix with an
  authoritative per-sensor ping scheduler.
  - [ ] Prevent failed-ping recursion and successful mixed-track double updates.
  - [ ] Make cycle time per sensor, validate positive finite values, and avoid
    per-track reflection/LINQ allocation.
  - [ ] Reuse typed native detection state instead of duplicating
    `CanSeeSignature` and fix profiler-sample ownership.
  - [ ] Verify forced burnthrough power, self-damage, resource, lock, and
    multiplayer behavior.
- [ ] Make Doppler settings a validated separately attached component
  reference and remove every `new DopplerNotchSettings()` construction.
  - [ ] Add deterministic angular, radial-floor, minimum-speed, and
    relative-velocity boundary tests.
  - [ ] Give the static contact registry explicit sensor/target/scene teardown.
  - [ ] Keep the TacView overlay presentation-only and migrate its known native
    private UI state to a typed accessor.
- [ ] Stabilize the remaining fire-control and IFF experiments.
  - [ ] Preserve `ActiveFireControlSensorOptions` modified-range intent with
    typed native access and an acquisition/lock regression fixture.
  - [ ] Determine whether `MultiSensor` and
    `MultiActiveFireControlSensor` have serialized downstream users before
    implementing, deprecating, or isolating their current placeholder types.
  - [ ] Validate track-logic weights, propagate update failure, and prohibit
    unintended true-position leakage or fire-control side effects.
  - [ ] Replace `IFFComponent`'s inverted re-enable path, direct stat-cache
    writes, per-tick reflection/logging, missing null cleanup, and absent
    authority model with a native communications/sensor integration.
- [ ] Prove charge-up beam server/client event ordering and pooled impact-effect
  reset with structured telemetry.
- [ ] Track the native `CraftLandingPad.SupportsLaunch` versus
  `_supportLaunch` inconsistency in the supported-game target verifier.

## P1 - Namespace and Harmony Patch Governance

- [ ] Inventory current namespaces, global public types, Harmony patches, Harmony IDs, and patched native methods.
- [ ] Choose and document the namespace policy for all new code.
- [ ] Do not mass-rename released types solely for namespace consistency.
- [ ] Plan compatibility shims or migration paths before moving existing public types.
- [ ] Define a diagnostic patch-class naming convention such as `TargetClassTargetMethodPatch` for new patches.
- [ ] Replace blanket assembly-wide patch activation incrementally with feature-owned registration where safe.
- [ ] Assign stable Harmony IDs per feature area where isolation is useful.
- [ ] Add explicit availability, configuration, lifecycle, and multiplayer-authority checks before behavior-changing patches run.
- [ ] Create a generated or script-produced patch inventory for diagnostics.
- [ ] Make patch failures identify the owning feature and native target.
- [ ] Ensure unpatch/reload behavior is defined for any feature that supports runtime toggling.

## P2 - Tests and CI-Safe Validation

- [ ] Create a conventional test project that does not require launching Unity or NEBULOUS.
- [ ] Add tests for socket-filter composition and lookup behavior.
- [ ] Add tests for serialization and configuration parsing that can run without game assemblies where practical.
- [ ] Build a minimal AssetBundle round-trip fixture that reproduces the editor-valid but bundle-loaded-null custom class/struct failure.
- [ ] Test custom class and struct fields, arrays/lists, nested values, inheritance, `[SerializeReference]`, top-level Unity object references, assembly/type identity, stripping, and bundle build settings through the real mod load path.
- [ ] Compare editor state, built bundle contents, post-load prefab state, instantiated state, and Player.log evidence for every fixture case.
- [ ] Determine whether the failure is caused by type/assembly identity, Unity managed-reference metadata, build stripping, bundle dependency resolution, game/Unity version differences, or another pipeline stage.
- [ ] Document a supported complex-data representation and migration path only after it passes the released AssetBundle build/load path and target game versions.
- [ ] Add tests for pure targeting, scoring, geometry, and math helpers.
- [ ] Add regression tests when extracting logic from Harmony patches into typed helpers.
- [ ] Add public API baseline validation to CI.
- [ ] Add `ModInfo.xml` schema/content validation.
- [ ] Add package allowlist validation.
- [ ] Add validation that assembly, manifest, and package versions match.
- [ ] Add a mixed-line-ending check for touched C# and VB files.
- [ ] Add a check that normal build/test commands leave tracked files unchanged.
- [ ] Add a CI-safe PowerShell smoke test for repository scripts.
- [ ] Keep runtime/editor reproduction in the `neb-testing` workflow and record the smallest repeatable scenarios.
- [ ] Add log assertions for successful AGMLIB load and known high-risk Harmony patch failures.
- [ ] Build a compatibility fixture corpus containing representative legacy prefabs, fleets, saves, and component configurations from known-good AGMLIB releases.
- [ ] Exercise applicable fixtures through load, instantiate, clone, pool/unpool, launch, save, and reload paths.
- [ ] Add an opt-in component smoke-test matrix covering editor construction, runtime initialization, normal operation, teardown, scene transition, and mod unload/reload where supported.
- [ ] Add a multiplayer component matrix covering offline play, host, remote client, and dedicated server where applicable.
- [ ] Record for each networked component which side owns state mutation, validation, spawning, damage, resource consumption, and replication.
- [ ] Add regression fixtures for every approved namespace, type-name, serialized-field, or save-key migration.

## P2 - Runtime Lifecycle, Resource Ownership, and Performance

- [ ] Audit runtime sidecars for explicit ownership, one-instance-per-owner guarantees, rollout boundaries, and cleanup when the native owner is destroyed, pooled, replaced, or unloaded.
- [ ] Audit component event subscriptions, callbacks, coroutines, delayed invocations, static registries, and caches for deterministic teardown and scene-transition safety.
- [ ] Audit dynamically created `GameObject`, `Component`, `Material`, mesh, texture, and other Unity object ownership; destroy or transfer ownership explicitly.
- [ ] Verify pooled and cloned components reset transient state, cached references, timers, collections, and network ownership before reuse.
- [ ] Profile component hot paths for per-frame allocations, repeated reflection, LINQ, scene searches, redundant `GetComponent` calls, and avoidable cache rebuilds.
- [ ] Establish representative CPU, allocation, and log-volume baselines for high-risk components and record the test scenario used.
- [ ] Add performance regression checks where stable automation is practical, and require before/after profiles for changes to known hot paths.

## P2 - Packaging and Release Safety

- [ ] Define the exact allowlist for an AGMLIB Workshop/GitHub release package.
- [ ] Decide and document whether `0Harmony.dll` must be bundled by AGMLIB.
- [ ] Reject backup projects, test fixtures, symbols, build logs, and unrelated dependencies from publishable packages.
- [ ] Generate a package manifest with SHA-256 hashes.
- [ ] Preserve the last known-good package for rollback.
- [ ] Verify packages by extracting or copying them into a disposable directory before publication.
- [ ] Validate Workshop and local-install layouts independently.
- [ ] Add release notes covering game-version compatibility and public API changes.

## P2 - Dependency and Binary Hygiene

- [ ] Inventory the 46 tracked assemblies under `AGMLIB/libs/` and the tracked BepInEx/UnityExplorer files.
- [ ] Record source, version, license/provenance, and expected hash for every tracked binary dependency.
- [ ] Add or update third-party notices as required.
- [ ] Review whether game and Unity assemblies may or should remain committed; document the decision.
- [ ] Consider a controlled sync script that copies required assemblies from a configured NEBULOUS installation and verifies hashes.
- [ ] Remove references that are not needed by the compiled runtime assembly after verifying a clean build.
- [ ] Specifically verify whether the UnityEditor references are necessary.
- [ ] Separate editor/authoring-only code into another project or assembly if it needs UnityEditor APIs.
- [ ] Avoid silently updating tracked binary dependencies without a compatibility review and manifest update.

## P2 - Source and Project Organization

- [ ] Remove or archive `AGMLIB/AGMLIB - Backup.csproj` after confirming it has no unique required configuration.
- [ ] Add `Directory.Build.props` or equivalent shared configuration if additional projects are introduced.
- [ ] Review the largest source files and extract feature-focused helpers when they are next modified:
  - [ ] `Generic Gameplay/Ewar/PassiveCommsSensorComponent.cs`.
  - [ ] `Munitions/ModularMissile/CommandSeekers.cs`.
  - [ ] `EntryPoint.cs`.
  - [ ] `Dynamic Systems/UI/ShipStatusPowerBar.cs`.
  - [ ] `Editor/SocketFilterCore.cs`.
- [ ] Move reusable behavior out of Harmony patch classes into typed domain helpers.
- [ ] Reduce unrelated responsibilities in `EntryPoint.cs`.
- [ ] Inventory unfinished and legacy code and decide whether to maintain, isolate, exclude, or remove it.
- [ ] Keep runtime, authoring/editor, diagnostics, and experimental code boundaries explicit.

## P2 - Line Endings and Repository Hygiene

- [ ] Add explicit `*.cs text eol=crlf` and `*.vb text eol=crlf` rules to `.gitattributes`.
- [ ] Verify `.editorconfig` and `.gitattributes` agree on line endings.
- [ ] Add a script or CI check that detects mixed line endings in changed C# and VB files.
- [ ] Confirm generated files and local game-install artifacts are ignored.
- [ ] Confirm Unity cache, logs, build outputs, and Workshop staging directories cannot be committed accidentally.

## P3 - Optional Documentation Generation

- [ ] Reassess documentation discovery after the scoped agent map, API policy, and release documentation exist.
- [ ] If navigation becomes difficult, add a small curated `llms.txt` or structured index generated from canonical metadata.
- [ ] Keep generated documentation reproducible and add a `-Check` mode to CI.
- [ ] Avoid committing duplicated full-context documents that materially inflate repository context.
- [ ] Prefer task-first indexes and concise routing over loading every historical note into agent context.

## Completion Definition

- [ ] A clean checkout can build and package AGMLIB without editing tracked files.
- [ ] Pull requests receive a reliable build and compatibility result.
- [ ] Releases are intentional, versioned, validated, and reproducible.
- [ ] Workshop updates use a dedicated skill with explicit authorization and safety checks.
- [ ] Public and serialized contracts are protected by an authoritative baseline.
- [ ] New contributors and agents can find the correct build, test, deploy, and debugging paths quickly.
- [ ] Existing downstream mods retain compatibility or receive an explicit migration path.
