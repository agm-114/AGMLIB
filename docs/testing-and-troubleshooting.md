# Testing and troubleshooting

AGMLIB needs three distinct validation levels.

## Repository-only validation

This level must not deploy to NEBULOUS or launch the game:

```powershell
git status --short
.\scripts\Documentation\Export-AgmlibInventory.ps1 -Check
.\scripts\NativeCode\Export-NebulousManagedCode.ps1 -ValidateOnly
```

The current MSBuild project is not yet safe for unattended CI because its
pre/post-build targets generate `ModInfo.xml` and deploy to a hard-coded game
path. Follow `planning/build-release-plan.md` before treating `dotnet build` as a
repository-only command.

## Local build and deployment

Read `.agents/neb-testing.local.md`, then follow
`.agents/skills/neb-testing/SKILL.md`. That workflow owns machine-specific paths,
deployment, launch, prefab dumping, and `Player.log` inspection.

If a build or copy fails while the game is running, first determine whether the
loaded AGMLIB DLL is locked. Do not interpret a stale deployed DLL as a source
regression. Record the source commit, built DLL hash, deployed DLL hash, game
assembly hash, and log session together.

## Runtime evidence ladder

For a behavioral claim, collect the narrowest sufficient chain:

1. AGMLIB call site and feature configuration;
2. native type/member from the pinned assembly;
3. relevant prefab YAML or serialized binding;
4. Unity lifecycle or Mirror authority path;
5. controlled reproduction;
6. focused log/telemetry output.

Do not infer that a field is live merely because it exists, or that a callback
runs merely because it overrides a plausible method.

## High-risk scenario matrix

| Feature family | Required scenarios |
|---|---|
| Muzzles and weapons | charge, cancel, target loss, fire, impact, pool/reuse, host/client visuals |
| Modular missiles | editor install, save/load, spawn, launch, seeker handoff, fuse, submunition, pool/reuse |
| Dynamic resources | editor cost, spawn allocation, tick, shortage, restore, destroy, save/load |
| Sensors and SIGINT | acquire, update, occlude, lose, stale track, owner/IFF change, multiplayer visibility |
| Debuffs and area effects | repeated impact, stacking, expiry, destroyed target, scene change, authority |
| Craft/drone limits | active cap, queued replacement, recovery, loss, mixed craft policy, host/client |
| Sub-sockets and filters | nested selection, copy/paste, invalid combination, save/reload, legacy content |

## Log triage

Prefer stable feature-prefixed events at state transitions. Flag:

- Harmony target resolution failures;
- `NullReferenceException` inside pool/launch callbacks;
- repeated errors from `Update`/sensor/resource ticks;
- server and client both applying damage or resource changes;
- missing descriptor references on cloned runtime objects;
- save-key/type lookup failures;
- fallback catches that suppress the native target or component identity.
