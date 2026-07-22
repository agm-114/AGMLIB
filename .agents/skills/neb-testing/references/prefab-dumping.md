# Prefab YAML dumping

Use the bundled dump command before runtime testing so code investigation can query one local snapshot instead of repeatedly reopening the game.

## Run a complete dump

Close Nebulous, then run from the repository root:

```powershell
powershell -ExecutionPolicy Bypass -File .agents\skills\neb-testing\scripts\dump-neb-prefabs.ps1
```

The script:

1. Verifies the repository, executable, output, and log paths.
2. Builds and deploys the Debug AGMLIB assembly.
3. Starts Nebulous with `AGMLIB_PREFAB_DUMP_DIR` set for that process.
4. Waits for a new `manifest.yaml` written after launch.
5. Leaves Nebulous open at the main menu for the test.

The runtime dumper replaces the previous snapshot atomically. A failed dump leaves the last completed snapshot intact.

Use overrides only when local notes differ from the standard paths:

```powershell
powershell -ExecutionPolicy Bypass -File .agents\skills\neb-testing\scripts\dump-neb-prefabs.ps1 `
    -GameRoot 'D:\SteamLibrary\steamapps\common\Nebulous' `
    -PlayerLog 'D:\LocalLow\Eridanus Industries\Nebulous\Player.log'
```

Use `-SkipBuild` only when the deployed DLL is already current. Use `-ValidateOnly` to check paths without building or launching.

## Snapshot contents

The default output is `.agents/cache/neb-prefabs/` and is git-ignored. `manifest.yaml` records the schema version, game and AGMLIB versions, UTC timestamp, source/category counts, and every generated file.

Registered content is captured from `BundleManager` after bundles load:

- Hulls and spaceframes
- Modular missile bodies
- Hull and missile components
- Munitions
- Factions

Files are grouped as `Vanilla/<category>/` or `<mod-name>/<category>/`. Each file contains identity data, its GameObject hierarchy, component types, enabled state, and serialized fields. Unity object references are emitted as stable hierarchy references when possible and are never recursively expanded. Collections and serializable nested values are capped at a safe depth.

This is a registered-prefab snapshot, not a raw extraction of textures, meshes, audio, scenes, or every transient scene object.

## Query before testing

Search the snapshot before adding runtime diagnostics:

```powershell
rg -n -i 'LoiteringModularMissile|Activation Trigger|Prox Detonator' .agents\cache\neb-prefabs
rg -l -i 'headon/C06' .agents\cache\neb-prefabs
```

Use the YAML to establish component hierarchy, serialized configuration, layer, trigger, and reference facts. Use Testing Range only for behavior that depends on launch, pooling, runtime-added components, physics callbacks, ownership, or live targeting state.

## Failure handling

The dump is enabled only when `AGMLIB_PREFAB_DUMP_DIR` is present at process startup. If the script times out:

1. Confirm Nebulous was fully closed before running the script.
2. Search `Player.log` for `[PrefabYamlDump]` and nearby exceptions.
3. Confirm the log reports the newly built AGMLIB version.
4. Keep the prior snapshot only as historical evidence; do not treat it as current.

Do not manually edit generated YAML. Fix the dumper or source prefab and regenerate it.
