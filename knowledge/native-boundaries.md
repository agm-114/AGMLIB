# Native boundaries

## Current structural inventory

The generated source scan found:

- 163 active-looking `HarmonyPatch` attributes;
- 88 files containing known reflection-boundary tokens;
- only two current typed `Internals()` accessor files, for
  `ModularMissile` and `WeaponComponent`.

An AST-aware cross-check found 96 directly decorated patch classes and 189
generic `Common.GetVal<T>` call shapes. These do not replace the broader counts:
one class can have multiple attributes, patches can be method-level or dynamic,
and reflection also includes setters, invocations, `Traverse`, and cached
metadata types.

See:

- `planning/generated/harmony-patch-inventory.md`;
- `planning/generated/reflection-migration-inventory.md`;
- `planning/generated/namespace-inventory.md`.

Counts are conservative structural signals. Overloads and dynamically resolved
targets still require assembly inspection.

## Boundary record format

For every known native member or patch target, record:

- owning AGMLIB feature;
- declaring native full type;
- member name and complete expected signature/type;
- access form (public call, Harmony target, typed internals, dynamic reflection);
- pinned native assembly hash;
- availability/lifecycle/authority preconditions;
- failure policy and diagnostic;
- source callers and runtime fixture.

## Migration order

1. Hot-path repeated reflection.
2. Gameplay-mutating private members.
3. Patch target resolvers and overload-sensitive methods.
4. Shared members used by multiple features.
5. Cold editor/diagnostic reflection.
6. Truly dynamic/unknown nested types, which remain contained reflection
   boundaries and convert immediately back to concrete types.

Do not perform a repository-wide mechanical rewrite. When a function using an
old reflection helper is modified, migrate its known native members in the same
change.

## Target verification

A verifier should load the configured native assemblies without launching the
game and validate type/member existence, static/instance form, field/property
type, method parameters, return type, and Harmony overload resolution. Its output
should be grouped by owning feature so one incompatible target can disable or
block that feature rather than obscure the rest of the library.
