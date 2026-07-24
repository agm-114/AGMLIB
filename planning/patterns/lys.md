# LYS implementation patterns

This page is based only on the pinned native NEBULOUS source reconstruction under
`.native-code/current`. It does not use AGMLIB commits or recent AGM changes as
evidence. The assembly identity is recorded in
`knowledge/evidence-ledger.md`.

## Attribution boundary

LYS is not inferred from Git history. AGMLIB commit authors other than AGM are
treated as other modders or repository contributors, not as LYS. Their commits
are not evidence for this page unless the maintainer explicitly supplies a
different identity and asks for that evidence to be included.

## Different authority boundary

LYS can change the game source. Native work can therefore alter a root class,
private state, lifecycle, serialization, networking, editor behavior, and every
caller together. It does not need a Harmony adapter or removable sidecar merely
to reach its own behavior.

AGMLIB cannot assume that authority. A pattern that is coherent inside the game
source can be a poor fit for an external compatibility library.

## Source-visible center of gravity

The native source favors deep object-oriented ownership:

- large authoritative classes own many related responsibilities and private
  fields;
- behavior is organized through inheritance, virtual/override lifecycle, and
  extensive interface contracts;
- root objects coordinate networks of subordinate managers and components;
- nested saver/state/helper types remain close to the owning domain class;
- networking, save/load, editor, runtime, and presentation contracts often meet
  at the same native owner.

Examples from the pinned `Nebulous.dll` reconstruction:

| Native class | Approx. lines | Private/protected declarations | Implemented interfaces |
|---|---:|---:|---:|
| `Game.Units.ShipController` | 8,709 | 434 | 26 |
| `SmallCraft.Spacecraft` | 5,663 | 267 | 22 |
| `Munitions.ModularMissiles.ModularMissile` | 2,903 | 146 | 27 |
| `Ships.HullComponent` | 1,469 | 94 | 8 |
| `Ships.WeaponComponent` | 1,089 | 87 | 1 directly, plus inherited contracts |

These counts are structural evidence, not defect counts. They show that
mega-class ownership is an architectural center of gravity rather than an
occasional outlier.

## Planned hierarchy versus ad-hoc extension

Maintainer context explains the tradeoff: LYS works from a plan for the game and
can extend the source hierarchy to fit that plan. Heavy OOP can keep one
authoritative model internally consistent when all call sites, serializers,
network routes, and lifecycle stages are under the same control.

The native source supports that reading:

- `ShipController` implements commands, navigation, damage, EWAR, launching,
  docking, selection, reporting, and network identity around one ship owner;
- `ModularMissile` combines design-template, save, munition, guidance, EWAR,
  spawning, pooling, selection, and reporting contracts;
- `Spacecraft` similarly combines fleet design, save, networking, damage,
  storage, EWAR, and runtime behavior;
- weapon, hull, sensor, and launcher families rely on inheritance from shared
  lifecycle-bearing base classes;
- `HullComponentRouter` centralizes many component RPC interfaces.

Composition still exists through components, managers, descriptors, and
interfaces, but authoritative orchestration remains concentrated in large
domain objects.

## What AGMLIB should learn—and not copy

Useful native lessons:

- identify the real authoritative owner and lifecycle;
- use native APIs so save, networking, bookkeeping, and presentation remain
  coherent;
- respect the existing inheritance and interface contracts;
- validate behavior across the whole native object lifecycle.

Patterns not to copy directly into AGMLIB:

- adding optional modder behaviors to a central mega-class;
- assuming every caller and serialized asset can be updated in one change;
- coupling unrelated optional features through one lifetime or activation path;
- using inheritance alone where a removable sidecar or feature-local policy is
  required;
- treating private native state as stable merely because the native source can
  change it atomically.

The difference is not that one approach is universally superior. LYS can evolve
the source as a planned integrated system; AGM maintains an external library
whose features must remain adaptable, independently removable, and safe for
third-party mod content.
