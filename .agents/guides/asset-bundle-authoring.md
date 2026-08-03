# AssetBundle production authoring

This is an interim safety rule based on an [unverified maintainer report](../../knowledge/asset-bundle-serialization.md), not an established claim about Unity or NEBULOUS behavior.

## Production rule

Do not use an AGMLIB-defined nested custom class or struct as a serialized field payload for AssetBundle-authored production content.

Until the real AssetBundle round-trip proves a specific alternative safe, represent complex authoring data with:

- fields and lists of already proven Unity or native-game serializable types;
- flattened parallel lists with stable IDs or indices; or
- separately attached components and serialized references.

These forms are compatibility precautions. Do not replace them with nested data-transfer objects merely because the replacement is cleaner in C#.

## Structural validation

Flattened data must validate:

- equal lengths for related parallel lists;
- stable unique IDs and duplicate keys;
- index ranges and missing targets;
- exactly one root where required;
- missing component or object references;
- cycles when the model requires a tree or DAG; and
- unreachable nodes and conflicting ownership.

Prefer an actionable authoring or load error over silently truncating lists or substituting defaults.

## Refactoring boundary

Inspector rendering, editor domain reload, direct in-editor prefab instantiation, and Unity's normal serialization API do not exercise the reported failure boundary. A replacement must pass the AssetBundle build, NEBULOUS mod load, prefab lookup, runtime instantiation, and applicable clone or pool lifecycle.

Relax this rule only after the [research plan's acceptance gate](../../planning/asset-bundle-serialization-plan.md) passes for the proposed representation.
