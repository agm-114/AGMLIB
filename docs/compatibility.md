# Compatibility policy

AGMLIB is both a mod and a library consumed by content and downstream mods.
Compatibility therefore includes more than C# source compatibility.

## Durable contracts

Assume these identities are durable once released:

- public and protected types, nested types, constructors, members, interfaces,
  and enum values;
- full type names and, where Unity or editor lookup is involved, simple names;
- `MonoBehaviour`, `ScriptableObject`, descriptor, runtime-state, and patch
  types referenced by prefabs or assets;
- `[SerializeField]` and public serialized field names;
- save keys, XML elements/attributes, bundle keys, config keys, and Workshop
  layout;
- Harmony IDs and feature ownership where downstream code relies on them.

Do not mass-move global types into namespaces. New types should use a documented
`AGMLIB.*` namespace, while existing released types remain until an assembly and
fixture baseline proves a compatible migration.

## Required baseline

The authoritative baseline should be generated from a known-good release DLL,
not only from source. It must record:

- assembly and file hash, AGMLIB version, source commit, and supported game hash;
- public API including nested types and enum numeric values;
- component and serialized identities;
- save/config keys and Harmony IDs;
- intentional exclusions from the supported downstream API.

A source inventory already exists under `planning/generated`, but it is an
orientation aid rather than the release baseline.

## Intentional changes

An intentional breaking or serialized change requires:

1. an impact statement naming downstream binaries and content formats;
2. a compatibility shim, retained old type, migration metadata, or conversion
   tool where technically possible;
3. representative old fleet, prefab, save, and third-party reference fixtures;
4. an explicit baseline update and migration note;
5. a support window and removal criterion for deprecated identities.

If compatibility cannot be demonstrated, retain the old identity.

## Native game compatibility

Native private members and Harmony targets are versioned dependencies. Pin them
to assembly hashes, verify signatures after every game update, and fail only the
owning feature when possible. Decompiled source is evidence about the pinned
binary, not a license to redistribute the generated dump.

See `knowledge/evidence-ledger.md`, `knowledge/native-boundaries.md`, and
`planning/native-update-plan.md`.
