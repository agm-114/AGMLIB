# Bootstrap, content, save, and network object flow

All paths below are relative to the pinned decompile directory
`.native-code/current/assemblies/Nebulous-26DE96E2E384`.

## Main-menu bootstrap

Verified-source flow:

1. `UI/MainMenu.cs::Start` performs application/community setup, disposes an
   active game manager or leaked network state, and checks bundle
   initialization.
2. It invokes `Bundles/BundleManager.cs::LoadAllBundlesAsync` when content has
   not been loaded.
3. the bundle coroutine refreshes the mod database, loads stock bundles,
   processes stock addressables, loads every marked mod, preloads addressable
   bundles, and finally marks the manager initialized;
4. the main-menu completion callback calls the remaining game-loaded setup.

AGMLIB must not assume its entry point runs before stock registries exist.
`IModEntryPoint.PreLoad` and `PostLoad` are distinct integration boundaries.

## Mod load order

`Modding/ModRecord.cs::CoroutineLoadMod` performs this sequence:

1. reject missing mods or unsatisfied dependencies;
2. load each managed assembly with `Assembly.LoadFile`;
3. collect stat definitions from the assembly;
4. find and instantiate the first concrete `IModEntryPoint`;
5. call `PreLoad`;
6. load the mod's AssetBundles;
7. process each AssetBundle through `BundleManager`;
8. load additional Addressables catalogs;
9. call `PostLoad`;
10. publish loaded or minor-error status.

Consequences:

- static initialization can happen during assembly load, before `PreLoad`;
- registry-dependent work belongs after the relevant bundle phase;
- entry-point discovery depends on a concrete type and parameterless
  construction;
- one bad optional asset should report an owning feature instead of leaving
  unrelated AGMLIB systems half initialized.

## Bundle manifest and registry identity

`Bundles/BundleManager.cs::ProcessAssetBundleInternal` reads `manifest.xml` and
concurrently processes the manifest's factions, hulls, components, munitions,
debuffs, maps, missions, scenarios, missile bodies/components, HUD themes,
spaceframes, and supplementary AI definitions.

`Bundles/BundleManifest.cs` constructs qualified names using this precedence:

1. `OverrideNamespace/name`;
2. `Namespace/name`;
3. bare `name`.

For bundle-keyed objects, the loader:

- loads either a root `GameObject` component or a `ScriptableObject`;
- assigns the qualified key with `SetKey`;
- assigns the source mod ID when the object implements `IModSource`;
- inserts it with overwrite behavior;
- registers qualified aliases with the same overwrite behavior.

Later content can therefore replace an earlier key. Save keys, bundle keys,
aliases, namespaces, and mod load order are compatibility contracts rather than
editor-only labels.

Munitions have an additional network identity requirement: a non-empty
`NetworkSpawnKey`. Hulls and munitions implementing
`INetworkSpawnerRegistered` are registered for network spawning.

After processing, the AssetBundle is unloaded with loaded objects retained.
This is one reason an in-editor prefab check is not equivalent to the released
bundle-load path.

## Addressables

The bundle manager records which `ModInfo` owns catalog locations backed by
bundle resources and rewrites internal IDs toward the mod directory. In the
current source, only `SkirmishMapInfoBlock` locations are visibly promoted into
the bundle manager's map registry.

Do not assume arbitrary new addressable object types automatically become
discoverable merely because a catalog loads.

## Network object pooling

`Utility/NetworkPoolable.cs` is a Mirror `NetworkBehaviour` with a serialized
copied asset ID used as its spawn key.

The verified flow is:

1. register the prefab with `NetworkClient.RegisterPrefab`;
2. the server unpools at a position/rotation, optionally with velocity;
3. the host executes the local body and an RPC projects the same activation to
   non-owner clients;
4. the object is activated before `OnUnpooled`;
5. repooling is likewise server initiated and RPC projected;
6. delayed repool may disable immediately or complete after a coroutine;
7. subclasses reset feature state in `OnUnpooled`, `OnRepoolDelayed`, and
   `OnRepooled`.

An AGMLIB pooled component must reset timers, target/owner references,
collections, subscriptions, and pending delayed work. `Awake` is not a per-use
callback.

## Bulk save identity and deferred references

`Utility/SaveFileObject.cs` assigns a process-wide numeric ID to non-independent
save objects and keeps a lookup of spawned objects. Duplicate restored IDs are
rejected and replaced, with a warning that references will break.

`SaveAllObjects` asks every bulk save object for a typed `BulkObjectSaver`.
`LoadAllObjects` performs two phases:

1. instantiate each object, assign its restored ID, and load its object and
   component state;
2. resolve any promises that waited for referenced save IDs.

Objects such as spacecraft, groups, and carrier traffic queues deliberately use
`GetSavedObjectPromise<T>` because referenced objects may not exist yet when
their XML is read.

Consequences:

- do not resolve cross-object save references solely by scene search;
- stable save IDs are runtime instance identity, while save/bundle keys identify
  content/design types;
- load callbacks must tolerate references resolving later;
- state that needs all referenced components belongs in a post-component-state
  phase, not initial XML parsing.

## Game-manager replacement

`Game/GameManager.cs` is a scene-owned singleton that can persist and swap
through `PrepareToSwap` and `ReplaceWith`. Network manager, room, players, bot
authority, and local-player state can cross that boundary.

Static AGMLIB caches must not equate a scene change with a fresh process. Clear
or rebind them against the new manager and native owners.

