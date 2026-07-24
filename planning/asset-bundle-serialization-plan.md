# AssetBundle custom-value serialization research

## Problem statement

AGMLIB-defined nested custom classes and structs can serialize visibly and
correctly in the Unity editor but become null or otherwise invalid when the
built AssetBundle is loaded by NEBULOUS. This blocks conventional nested DTOs,
trees, and other complex authoring models.

The current evidence and production rule are maintained in
`knowledge/asset-bundle-serialization.md`. This page defines how to investigate
the failure rather than duplicating that guidance.

## Governing constraint

Do not relax the interim rules in
`knowledge/asset-bundle-serialization.md` while running experiments. A
successful editor inspection or domain reload is not a passing result; a case
must survive the released AssetBundle build and in-game load path.

## Questions to answer

1. Does failure occur for both reference-type classes and value-type structs?
2. Does it affect a direct field, array, `List<T>`, nested value, inherited
   value, and `[SerializeReference]` in the same way?
3. Are top-level custom `MonoBehaviour` or `ScriptableObject` references safe
   while nested non-`UnityEngine.Object` payloads fail?
4. Do the serialized type's assembly name, namespace, full name, visibility,
   generic shape, or compilation order change the result?
5. Does managed-code stripping or an AssetBundle dependency omit required type
   metadata?
6. Does the failure happen while building the bundle, loading it, resolving the
   script/type, deserializing the field, or instantiating/cloning the prefab?
7. Is behavior dependent on Unity editor version, target platform, game Unity
   version, or the AGMLIB assembly loaded by the game?

## Minimal fixture matrix

Create one opt-in test prefab with an identifying sentinel value in each case:

| Case | Example payload |
|---|---|
| Direct custom class | `[SerializeField] CustomClass value` |
| Direct custom struct | `[SerializeField] CustomStruct value` |
| Class list | `[SerializeField] List<CustomClass> values` |
| Struct list | `[SerializeField] List<CustomStruct> values` |
| Nested custom value | custom value containing another custom value |
| Inherited custom value | serialized base/derived payload |
| Managed reference | `[SerializeReference]` interface/base field |
| Unity object reference | separately created custom component/object |
| Native control | equivalent data using only known Unity/native types |
| Flattened control | primitive/native lists plus indices |

Keep every fixture tiny and give each field a distinct sentinel so a partial
failure is visible.

## Evidence captured per run

For each case record:

1. Unity and bundle target versions;
2. assembly name, type full name, and relevant build/stripping settings;
3. editor inspector value before build;
4. prefab/asset serialized representation before build;
5. bundle manifest and dependency information;
6. value immediately after `AssetBundle.LoadAsset`;
7. value after prefab instantiation;
8. value after the relevant AGMLIB registration/finalization path;
9. Player.log warnings about missing scripts, types, assemblies, or fields;
10. a pass/fail result with the first lifecycle stage where the sentinel was
    lost.

## Investigation order

1. Reproduce the smallest known failing class and struct without unrelated
   AGMLIB systems.
2. Add the native and flattened controls to confirm the harness itself.
3. Locate the first stage where data changes or disappears.
4. Vary one dimension at a time: container shape, type identity, compilation
   assembly, managed reference use, stripping, build target, and Unity version.
5. Inspect bundle and runtime type metadata only after the minimal comparison
   narrows the failure.
6. Test candidate workarounds through the full released mod pipeline, not a
   standalone editor-only loader.
7. Repeat the winning case against representative existing AGMLIB content and
   supported game versions.

## Acceptance gate for relaxing the rule

A custom value representation becomes supported only when:

- its fixture survives build, game load, prefab load, instantiation, and the
  applicable clone/pool lifecycle;
- the result is repeatable from a clean build;
- the reason it works is understood well enough to define its constraints;
- failure produces validation or a clear diagnostic rather than silent null
  data;
- migration from current flattened/component representations is documented;
- at least one realistic tree or complex configuration passes the same path.

Until all gates pass, new production content continues to use the current
flattened-list or component-graph pattern.
