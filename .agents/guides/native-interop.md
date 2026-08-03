# Native interop guidance

Before accessing a known non-public member on a native game type, read the scoped [native internals accessor instructions](../../AGMLIB/Nebulous/AGENTS.md).

- Put typed `Internals()` accessors under `AGMLIB/Nebulous`, using subfolders that mirror the declaring type's namespace.
- For known native members, prefer cached typed accessors over `Common.GetVal`, `Common.SetVal`, `Common.RunFunc`, local `AccessTools` bindings, `Traverse`, or repeated `FieldInfo`, `PropertyInfo`, or `MethodInfo` reflection.
- When modifying a function that uses one of those older reflection approaches, migrate that function's known native-member accesses to `Internals()` in the same change. This is incremental, not a repository-wide cleanup requirement.
- Keep dynamic reflection only when the member or runtime type cannot be known in advance, and convert its result back to a concrete type immediately.
