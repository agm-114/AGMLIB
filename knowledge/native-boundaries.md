# Native boundaries

## Verified accessor coverage (2026-11-08)

The installed `Nebulous_Data/Managed/Nebulous.dll` has SHA-256
`86EF9681F3C6B08BD9BEAC2173F903CE29D8197DD4D5E49279EA9CA058F14461`.
The opt-in `NativeAccessorTestingFixture` successfully initialized **390 cached
bindings across 95 accessor types**, including a closed generic ammo-loadout
accessor. Eight private-data getter/setter checks and the existing dynamic-stun
adapter fixture's 79 checks also passed.
This verifies binding compatibility and the stun regression, not every gameplay
feature that consumes these members.

Typed accessors under `AGMLIB/Nebulous` now cover the known reflection members
used by weapons, muzzles, missile stages/guidance, sensors, ship state, hull parts,
resources, carrier traffic, bundles, editor controls, status UI, and Mirror
identity setup. Related weapon/hull configuration and missile runtime state are
available for future work. Existing public native APIs are used directly where
appropriate. Source contains no `global::` qualifications; imports and aliases
disambiguate native types, including the game's `Utility.WeaponRole` from AGMLIB's
serialized avionics restriction enum.

A semantic inventory with Debug and local-fixture symbols reduced active
`Common.GetVal`, `Common.SetVal`, and `Common.RunFunc` calls from 284 to 19.
Private carrier traffic orders, ballistic bullets, debuff instances, network
track handles, and point-defense records use cached typed decoders compiled once
by `NativePrivateFields`. The inaccessible native types remain at that boundary;
the callers receive public domain values. Remaining dynamic boundaries include
configurable field names, unfamiliar board-piece implementations, and generic
serialization/inspection. Legacy member
names absent from the current native assembly require version-specific research
before binding; similar-looking fields are not sufficient evidence of equivalence.

Run the binding fixture when extending these accessors; the exact gate and
implementation rules are in [`AGMLIB/Nebulous/AGENTS.md`](../AGMLIB/Nebulous/AGENTS.md).
Machine-local logs, full prefab snapshots, and migration inventories remain in
the ignored cache and testing notes.

## Historical structural inventory

The initial generated source scan found:

- 163 active-looking `HarmonyPatch` attributes;
- 88 files containing known reflection-boundary tokens;
- two typed `Internals()` accessor files at that time, for
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
