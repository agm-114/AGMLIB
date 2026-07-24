# User documentation

This folder is the user-facing documentation surface for AGMLIB consumers and
content authors.

## Audience

- Mod authors configuring AGMLIB components in Unity or content bundles.
- Maintainers integrating AGMLIB behavior with custom factions, weapons,
  missiles, craft, effects, editor extensions, or runtime systems.
- Testers who need stable terminology, configuration guidance, and known
  limitations without reading implementation details.

## Documentation rules

- Describe released behavior and configuration, not speculative plans.
- Link to source and the relevant planning or knowledge page when behavior is
  version-sensitive or still under investigation.
- Treat public type names, serialized fields, save keys, enum values, Harmony
  IDs, and Unity component identities as compatibility-sensitive.
- Mark evidence as **verified**, **inferred**, or **planned** when the status is
  not obvious.
- Put implementation roadmaps in `planning/` and native-game research in
  `knowledge/`.

## Map

- `getting-started.md`: installation, dependency, build, and first-use overview.
- `feature-catalog.md`: user-facing catalog of every feature family.
- `component-authoring.md`: common Unity component authoring and lifecycle rules.
- `compatibility.md`: public API, serialization, game-version, and mod
  compatibility expectations.
- `testing-and-troubleshooting.md`: reproducible validation and diagnostic paths.
- `feature-catalog.md`: the canonical feature-family guide; split a family into
  `features/` only when configuration examples and troubleshooting become too
  large for the catalog.
