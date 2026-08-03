# AssetBundle custom-value serialization evidence

## Evidence status

**Unverified maintainer report.**

The focused AssetBundle round-trip fixture has not yet reproduced this behavior. The exact failing stage and root cause are unknown, so do not present the report or current hypotheses as established Unity or NEBULOUS behavior.

## Reported behavior

Maintainer experience reports that nested AGMLIB-defined custom classes and structs can display, edit, and retain data inside the Unity editor, then become null or otherwise invalid after NEBULOUS loads the built AssetBundle.

Avoiding those payloads in favor of already supported Unity or native types, flattened data, or component references is reported to avoid the failure. That workaround is a production precaution, not proof of the cause.

## Open questions

The following remain unestablished:

- whether classes and structs fail for the same reason;
- whether the issue is limited to nested non-`UnityEngine.Object` values;
- whether direct fields, arrays, `List<T>`, inheritance, or `[SerializeReference]` differ;
- whether custom top-level `MonoBehaviour` or `ScriptableObject` references have a different boundary;
- whether assembly or type identity, managed-reference metadata, stripping, bundle dependencies, compilation order, or Unity/game version causes the loss; and
- the first lifecycle stage at which the serialized value disappears.

## Evidence required

The [controlled fixture matrix and investigation sequence](../planning/asset-bundle-serialization-plan.md) must distinguish editor state, built-bundle contents, game load, prefab lookup, instantiation, finalization, cloning, and pooling. Record the first stage where each sentinel changes or disappears.

Until that research passes its acceptance gate, follow the separate [AssetBundle production authoring guide](../.agents/guides/asset-bundle-authoring.md). User-facing component guidance lives in [component authoring](../docs/component-authoring.md).
