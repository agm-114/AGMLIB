# Private mod data and Workshop dependencies

## Private mod data

- Never commit information about private, unreleased, access-restricted, or locally shared mods.
- Sensitive information includes mod names and identifiers, manifests, file trees, hashes, logs, prefab or YAML dumps, decompiled output, screenshots, and derived implementation details.
- Keep private-mod investigation material only in git-ignored local notes, local caches, or temporary access-controlled CI artifacts. Sanitize public reports and commits.
- Add a mod to checked-in CI catalogs or fixtures only after verifying that its Workshop page or source is public. Treat uncertain visibility as private.

## Shared AGMLIB dependencies

- Public Workshop mods that depend on AGMLIB should declare the centralized AGMLIB Workshop mod as a dependency. Treat that shared dependency as the runtime AGMLIB implementation.
- For ordinary runtime diagnosis, inspect the enabled centralized AGMLIB assembly rather than an AGMLIB DLL bundled inside an individual mod.
- Inspect a mod-bundled AGMLIB DLL only when class identity, serialized fields, namespace moves, or other serialization changes make the likely authoring version relevant.
- A bundled DLL is evidence of the AGMLIB version against which the mod was likely serialized, not proof of the assembly loaded at runtime.
