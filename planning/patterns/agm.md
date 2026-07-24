# AGM implementation patterns

This page describes the architectural pressures and patterns visible in AGMLIB
source. It is not a style guide for the native game and does not infer intent
from recent commit history.

## Operating constraint

AGM cannot change the NEBULOUS source code. AGMLIB has to integrate through
public native APIs, Harmony patches, reflection or typed internals, Unity
components, serialized descriptors, and runtime sidecars. It also serves
modders whose requests arrive ad hoc rather than as parts of one fixed product
roadmap.

That makes removability and local configurability first-class requirements:

- a feature should be independently attachable, disableable, and replaceable;
- a downstream mod should be able to adjust one behavior without adopting an
  unrelated framework;
- a native update should break or disable the narrowest possible feature;
- runtime state and cleanup should remain owned by the feature that introduced
  them;
- feature deletion should not require editing a central mega-class.

## Preferred center of gravity

### Sidecars

A sidecar file gives one native live owner an optional behavior without changing
the native class, prefab contract, or unrelated AGMLIB systems. It is a good fit
when:

- the behavior applies only to selected content or can be rolled out behind a
  gate;
- the native type cannot be safely subclassed or all supplying prefabs cannot be
  edited;
- ownership and teardown naturally follow one native `GameObject`;
- the feature may need to be removed, swapped, or tuned independently.

The patch should only discover/attach or forward into the sidecar. Candidate
selection, state, lifecycle, and execution remain in the sidecar or its small
typed helpers. `DecoyAmmoSettings` and the living rules in
`AGMLIB/Common/Sidecars.md` are the current reference.

### Feature-local composition

Prefer a small set of roles that can be recombined:

- serialized configuration or descriptor using bundle-proven Unity/native field
  types rather than nested AGMLIB-defined custom classes or structs;
- runtime sidecar/state component;
- pure policy/calculation helper;
- native/Harmony adapter;
- optional UI or FX adapter;
- focused diagnostics and test fixture.

Not every feature needs every role. The point is that runtime policy should not
be trapped in a patch class or central registry when it can live beside its
owner.

AGMLIB's flattened lists, zipped parallel collections, and component-reference
graphs are also a response to an observed AssetBundle boundary: nested custom
classes and structs can serialize correctly in the Unity editor and then load as
null or invalid from the built bundle. Short-term cleanup must preserve that
constraint. Complex trees should use validated indices or component graphs until
the actual release pipeline proves a safer representation.

### Thin compatibility boundaries

Known native private members belong behind cached typed `Internals()` accessors.
Harmony entry points stay small and feature-owned. Dynamic reflection remains
only where the target type or member genuinely cannot be known in advance.

These boundaries let AGMLIB adapt to native changes without rewriting feature
logic and let a broken integration be isolated.

## Patterns visible in AGMLIB source

Strengths worth preserving:

- vertical slices that connect authoring, runtime behavior, UI/FX, and native
  adaptation;
- Unity-serialized components that downstream content can configure;
- pragmatic extensions of the closest native component or descriptor contract;
- feature-specific behavior that can be shipped without waiting for a central
  redesign;
- growing use of sidecars, typed internals, prefab inspection, and opt-in runtime
  fixtures.

Pressures to counter:

- `PatchAll` makes optional features share one activation/failure boundary;
- large files sometimes combine descriptor, runtime state, patches, UI, and
  debugging;
- global static switches and registries can turn removable features into global
  assumptions;
- direct reflection at call sites makes native updates expensive;
- duplicate experiments and legacy versions can obscure which implementation is
  actually optional or canonical.

## Practical removability test

A feature is genuinely removable or tweakable when:

1. its configuration and activation boundary are identifiable;
2. its Harmony targets are owned and can be skipped or unpatched independently;
3. its sidecars/components clean up subscriptions, created objects, and static
   entries;
4. disabling it restores vanilla behavior rather than leaving partial state;
5. no unrelated feature depends on its private runtime state;
6. downstream content can configure it without editing a central class;
7. its files can be removed with failures limited to explicit serialized/public
   compatibility references.

This is the architectural reason to prefer sidecar files and composition for
AGMLIB even when the native game uses a different shape.
