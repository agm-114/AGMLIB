# Vanilla asset, bundle, resource, and prefab inventory

Generated metadata only. Payload bytes such as textures, meshes, audio,
video, and fonts are not exported.

- Game root: `C:\Program Files (x86)\Steam\steamapps\common\Nebulous`
- Generated UTC: `2026-07-24T03:54:24Z`
- Files hashed: 151
- Unity containers scanned: 41
- Unity scan errors: 0
- Unity objects indexed: 161,091
- Logical bundle entries indexed: 3,000
- Named Unity objects indexed: 59,136
- MonoBehaviour instances indexed: 41,110
- Registered/referenced prefab YAML files indexed: 275

## File families

| Family | Files | Bytes |
|---|---:|---:|
| addressables-bundle | 12 | 5,124,857,378 |
| legacy-com-asset-bundle | 4 | 1,150,222,096 |
| localization | 65 | 7,422,570 |
| loose-resource | 16 | 1,572,326 |
| metadata-or-player-data | 17 | 6,057,759 |
| player-data | 21 | 93,141,536 |
| runtime-asset-bundle | 4 | 1,193,035,097 |
| streamed-resource-payload | 12 | 860,427,418 |

## Largest Unity object classes

| Type | Objects |
|---|---:|
| `MonoBehaviour` | 41,110 |
| `GameObject` | 28,445 |
| `Transform` | 14,755 |
| `RectTransform` | 13,690 |
| `CanvasRenderer` | 11,147 |
| `AudioClip` | 8,718 |
| `MonoScript` | 5,237 |
| `MeshRenderer` | 5,054 |
| `MeshFilter` | 5,011 |
| `Texture2D` | 3,903 |
| `Mesh` | 3,349 |
| `ComputeShader` | 3,163 |
| `MeshCollider` | 2,827 |
| `VFXRenderer` | 2,551 |
| `VisualEffect` | 2,549 |
| `BoxCollider` | 2,338 |
| `Sprite` | 1,727 |
| `Shader` | 1,528 |
| `Material` | 851 |
| `AudioSource` | 566 |
| `VisualEffectAsset` | 397 |
| `CapsuleCollider` | 390 |
| `LODGroup` | 325 |
| `SkinnedMeshRenderer` | 223 |
| `Light` | 173 |
| `Animation` | 170 |
| `TextAsset` | 150 |
| `AnimationClip` | 111 |
| `SphereCollider` | 95 |
| `Rigidbody` | 81 |
| `AudioMixerGroupController` | 72 |
| `Canvas` | 63 |
| `Animator` | 62 |
| `Font` | 31 |
| `Camera` | 23 |
| `AssetBundle` | 20 |
| `AudioMixerController` | 18 |
| `AudioMixerSnapshotController` | 18 |
| `CanvasGroup` | 17 |
| `AnimatorController` | 15 |

## Prefab snapshot

### Sources

| Source | Prefabs |
|---|---:|
| Vanilla | 275 |

### Categories

| Category | Prefabs |
|---|---:|
| debuffs | 14 |
| factions | 3 |
| hud-themes | 2 |
| hull-components | 142 |
| hulls | 18 |
| maps | 8 |
| missile-bodies | 10 |
| missile-components | 24 |
| munitions | 39 |
| referenced-munitions | 1 |
| scenarios | 6 |
| spaceframes | 8 |

## Addressables

- Internal IDs: 1,220
- Resource types: 167
- Localization JSON files: 52
- Localization entries: 70,595

## Search products

- `index.json`: complete machine-readable metadata index.
- `files.csv`: hashes and scan status for every included file.
- `object-types.csv`: Unity object counts per container.
- `logical-assets.csv`: logical bundle/addressable asset paths.
- `named-assets.csv`: named Unity objects.
- `mono-scripts.csv`: assembly/namespace/class identities.
- `mono-behaviours.csv`: serialized script instances and GameObject IDs.
- `prefabs.json`: prefab identity, hierarchy, component, layer, and reference summary.

The registered-prefab YAML remains the stronger source for fully resolved
native component fields and hierarchy paths. The raw Unity inventory adds
unregistered assets, scenes, resources, addressables, and asset-path context.
