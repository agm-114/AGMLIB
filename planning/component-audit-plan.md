# Component documentation and audit plan

The current structural scan identifies 124 component-like types. The exact
number will change when the authoritative release-assembly definition is
implemented.

## Definition

Include:

- public/internal `MonoBehaviour`, `ScriptableObject`, and `NetworkBehaviour`;
- native component subclasses, descriptors, runtime missile behaviors, effects,
  state components, and editor components;
- serialized supporting structs/enums that form component configuration;
- factories/patch adapters only when they own component lifecycle or identity.

Exclude pure internal helpers after confirming they have no serialized or
downstream identity.

## Audit batches

| Batch | Scope | Primary risks |
|---|---|---|
| A | `Common`, `Nebulous`, `EntryPoint` | shared contracts, reflection, global initialization |
| B | Dynamic systems and UI | resource order, authority, per-frame work, stale UI |
| C | sockets/editor/YAML | save identity, filters, copy/paste, old content |
| D | weapons/muzzles/FX | timing, damage versus visuals, pool reset, object ownership |
| E | sensors/EWAR/SIGINT | track truth, visibility, hot paths, native changes |
| F | munitions/modular missiles | descriptor cloning, sockets, fuse/impact authority |
| G | craft/drones | group reservation, spawn/recovery, network ownership |
| H | server/systems/testing | global logging, experimental isolation, fixture opt-in |
| L | `Advanced/Legacy` | disposition and compatibility-only maintenance |

## Per-component record

Use the template in `docs/component-authoring.md`. Record one of:

- reviewed/no issue;
- bug with severity and reproduction;
- needs runtime evidence;
- legacy-retain;
- legacy-shim/supersede;
- exclude/archive candidate;
- compatibility-blocked.

## Static checks

For every source file, use the generated atlas to review:

- namespaces and duplicate simple names;
- serialized field count and public enums;
- Unity/network lifecycle methods;
- Harmony attributes and native reflection;
- logs in potential hot paths;
- TODO/HACK/WIP markers;
- size and mixed responsibilities.

The generated inventory already covers every repository file; the audit adds
semantic disposition and evidence.

## Runtime checks

Apply only relevant phases: editor construct, save/load, instantiate, network
spawn, clone, pool/unpool, launch/activate, normal operation, target/owner loss,
destroy, scene transition, mod reload. Record offline/host/client/dedicated-server
ownership for gameplay-mutating components.

## Coverage and freshness

Generate the canonical list from a built release assembly plus source metadata.
Fail `-Check` when a qualifying type is new, removed, moved, or undocumented.
Do not let generated tables replace the curated purpose/configuration/runtime
record.

## Priority findings from structural analysis

- `PassiveCommsSensorComponent`, `CommandSeekers`, `ShipStatusPowerBar`, and
  `SocketFilterCore` have large responsibility surfaces. `EntryPoint` has been
  reduced to its live loader contract.
- 90 source files contain known reflection-boundary tokens.
- 162 patch declarations are activated through one `PatchAll`.
- duplicate and global simple type names create migration risk.
- runtime testing infrastructure is substantial but no conventional pure test
  project exists.
