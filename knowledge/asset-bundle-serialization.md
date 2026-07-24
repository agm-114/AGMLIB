# AssetBundle custom-value serialization

## Status

**Maintainer-observed, not yet reproduced by the research fixture.**

Nested AGMLIB-defined custom classes and structs can display, edit, and retain
their data correctly inside the Unity editor, then become null or otherwise
invalid when the built AssetBundle is loaded by NEBULOUS.

The exact failing stage and root cause are unknown. Do not present the current
hypotheses as established Unity behavior until the focused bundle round-trip
research reproduces and isolates the failure.

## Current authoring rule

Do not use an AGMLIB-defined nested custom class or struct as a serialized field
payload for AssetBundle-authored production content.

Until a built-bundle round-trip test proves a specific alternative safe,
represent complex authoring data with:

- fields and lists of already proven Unity or native-game serializable types;
- flattened parallel lists plus stable IDs or indices; or
- separately attached components and serialized references.

This is why some AGMLIB configuration forms look more like zipped lists or a
spiderweb of components than a conventional nested object tree. Those shapes
are compatibility workarounds, not evidence that a nested DTO was overlooked.

## Required validation for flattened data

Flattening moves structural guarantees out of the type system. Validate:

- equal lengths for related parallel lists;
- stable, unique IDs and duplicate keys;
- index ranges and missing targets;
- exactly one valid root where the model requires one;
- missing component or object references;
- cycles where the model requires a tree or DAG;
- unreachable nodes and conflicting ownership.

Prefer an actionable authoring/load error over silently truncating mismatched
lists or substituting defaults.

## Refactoring boundary

Do not replace an existing flattened or component-graph representation with a
custom serializable class/struct tree merely because the replacement:

- renders correctly in the inspector;
- survives an editor domain reload;
- works when the prefab is instantiated directly in-editor; or
- is accepted by Unity's normal serialization API.

Those checks do not exercise the failing boundary. A supported replacement must
survive the same AssetBundle build, mod load, prefab lookup, and runtime
instantiation path used by released AGMLIB content.

## Known versus unknown

Known from maintainer experience:

- the editor can make the custom payload appear valid;
- the value can be null or invalid after the game loads the built bundle;
- avoiding nested custom class/struct payloads and using existing supported
  types or components is the current practical workaround;
- this makes trees and other complex authoring data substantially harder to
  represent.

Not yet established:

- whether class and struct failures have exactly the same cause;
- whether the failure is limited to nested non-`UnityEngine.Object` values;
- whether arrays, `List<T>`, inheritance, or `[SerializeReference]` change it;
- whether custom top-level `MonoBehaviour` or `ScriptableObject` references have
  a different safety boundary;
- whether assembly/type identity, managed-reference metadata, stripping, bundle
  dependencies, compilation order, or Unity/game version causes the loss;
- the first lifecycle stage at which the serialized value disappears.

## Research and relaxation gate

The controlled fixture matrix and investigation sequence live in
`planning/asset-bundle-serialization-plan.md`.

Relax this rule only when the candidate representation:

1. survives a clean AssetBundle build;
2. loads correctly through NEBULOUS's real mod path;
3. retains its data after prefab lookup and instantiation;
4. survives any applicable finalization, clone, and pool lifecycle;
5. produces a detectable diagnostic rather than silent null data when its
   requirements are absent; and
6. is repeatable across the game/Unity versions AGMLIB intends to support.
