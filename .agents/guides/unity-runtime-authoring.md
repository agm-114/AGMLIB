# Unity runtime authoring guidance

## Cloning and pooling

- Runtime missile and component behaviours are often created on a finalized pattern and then cloned or pooled for live instances.
- Mark private runtime fields `[SerializeField]` when descriptor or component references must survive into spawned instances.
- Do not assume private fields assigned in `OnAdded` survive into launched or pooled copies; serialize them or rebuild them in `OnUnpooled` or `OnLaunched`.
- For a typed descriptor field, use a small typed fallback helper that can recover it from the base descriptor reference when null.

## AssetBundle data

Before designing complex serialized authoring data, follow the [AssetBundle production authoring guide](asset-bundle-authoring.md). Consult the separate [evidence report](../../knowledge/asset-bundle-serialization.md) when the task depends on what has and has not been reproduced.

## Runtime sidecars

Read [`AGMLIB/Common/Sidecars.md`](../../AGMLIB/Common/Sidecars.md) before adding or changing behavior attached alongside a native runtime object. Keep that document limited to broadly reusable guidance about sidecar ownership, lifecycle, rollout, and patch boundaries; do not add implementation-specific discoveries or routine testing notes.
