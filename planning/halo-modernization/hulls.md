# Hull modernization

## Shared finding

All nine registered hulls still use `_armorSystem: Legacy`. Every dumped `HullSegment` has a null
`__primaryMesh`, null model-space position map, and null model-space normal map. Current stock hulls
use `ModelSpaceDamageMap`, set a primary mesh, provide model-space maps, and generally let armor
zones collect colliders from their segment.

The existing hulls do have a collider sampler prefab, a `HullVolume`, explicit mesh colliders,
socket roots, subpart roots, status/DC board prefabs, and dynamic armor-zone assignments. This
means modernization is a controlled re-authoring job, not a total hull reconstruction.

| Hull | Sockets | Hull segments | Mesh colliders | Existing armor zones |
|---|---:|---:|---:|---:|
| Ester-pattern Armored Frigate | 36 | 2 | 152 | 2 |
| Ket-pattern Battlecruiser | 59 | 2 | 250 | 2 |
| Mutan Et-pattern Storm Cutter | 17 | 2 | 119 | 2 |
| Gladius Heavy Corvette | 10 | 4 | 49 | 3 |
| Halberd Light Destroyer | 14 | 1 | 30 | 1 |
| Halcyon Light Cruiser | 36 | 1 | 43 | 1 |
| Marathon Heavy Cruiser | 35 | 1 | 64 | 1 |
| Paris Heavy Frigate | 20 | 1 | 62 | 1 |
| Thanatos Heavy Battleship | 71 | 4 | 146 | 4 |

## Required work on every hull

1. Set `BaseHull._armorSystem` to `ModelSpaceDamageMap`.
2. Assign each `HullSegment.__primaryMesh` to its LOD0 render mesh.
3. Bake a model-space position map and object/model-space normal map from that exact primary mesh.
4. Verify each armor zone maps one logical damage surface and uses
   `GetCollidersFromSegment` where the segment owns the collider set.
5. Rebuild the mesh-collider sampler prefab against the final collision meshes; test entry UV,
   exit point, overpenetration, and random exterior point queries.
6. Populate each segment's `_segmentSockets`; remove `HullSegmentPatchIndicator` and the patch that
   tried to infer material/socket state at runtime.
7. Verify `_paintableMeshes`, outage effects, LOD shared-material tables, nameplate targets, and
   current HDRP material properties.
8. Rebind all old custom thrusters to `Ships.CustomBehaviorThrusterPart` plus one or more
   `CustomBehaviorThrusterPartConfig` components. Confirm main-engine flags, lateral direction,
   attitude influence, particle parameters, visibility damage effects, flank damage, disable,
   destroy, clone, and pool lifecycle.
9. Validate every socket key is unique and stable. Replace `HullSocketBuiltIn` with authored default
   components or an explicit built-in policy; do not use a global socket patch.
10. Verify socket attach points, size, armor UV bounds/rotation, collider activation, craft release
    points, and cap behavior.
11. Re-author faction key, equipment override faction, classification, weight class, crew, DR,
    mass, signature, wake, motor force, maximum speeds, fuel, lifeboats, and point cost against a
    current stock hull of comparable role.
12. Open the status display and DC board, damage every segment/component, destroy/restore parts,
    and confirm there are no orphaned status mounts.
13. Verify selection volume, shoulder camera, hull volume weighting, line extents, radar signature,
    screenshot, silhouettes, boarding points, docking points, and landing pattern.

## Per-hull priorities

### Gladius

Use this as the pilot hull. It has the smallest socket count and only 49 mesh colliders, but four
visual segments currently share three armor zones. Decide explicitly whether the nose shares the
forward hull zone or receives a fourth zone. Validate the extra-engine glow objects and its small
custom thruster set before applying the pattern elsewhere.

### Halberd

Single visual segment and 30 mesh colliders make this the second armor bake. Validate its spinal
mount, fixed-weapon facing, and the first full multi-mode MAC asset.

### Paris

Use after Halberd to validate the same UNSC surface/drive conventions at frigate scale. Audit its
20 sockets and launcher caps, then run missile programming and external-magazine tests.

### Halcyon

The 36-socket layout is the first large UNSC equipment/fleet editor validation case. Verify built-in
and hidden sockets, DC/status grouping, and current crew/point rules without the legacy editor
patches.

### Marathon

Audit all 35 sockets, spinal firing clearance, multi-ammo magazine reachability, and large-hull
motor/turn balance. Confirm overpenetration paths through the single armor segment are reasonable;
split the segment only if damage-map resolution proves inadequate.

### Thanatos

Treat as the final UNSC hull. It has 71 sockets, four segments, four zones, and 146 colliders. Bake
and validate per segment, then test the triple-mode MAC, all charging effects, point total, fleet
validation, performance, and heavy-damage VFX.

### Mutan Et

Use as the Covenant pilot. It has the fewest Covenant sockets but 119 colliders. Validate socket
whitelist/blacklist rules, plasma weapons, shields, power emissive behavior, and Covenant material
retinting.

### Ester

Validate the 36-socket Covenant rules, shield fit/scale, multiple ejector launcher, restricted
plasma-torpedo storage using a native `BulkMagazineComponent` plus AGMLIB ammo filter, and current
transfer/save behavior.

### Ket

Treat as the final hull and performance gate. It has 59 sockets and 250 colliders. Reduce redundant
collision meshes where safe, verify armor-map texel density and collider-sampler memory, then run
the full host/client shield, plasma, AI, fleet, save/load, and long-battle soak.

## Hull exit criteria

For each hull record:

- one prefab dump with no missing scripts or unresolved references;
- editor install/remove checks for every socket class and restricted socket;
- armor penetration at bow, side, stern, top, and at least one seam;
- overpenetration through the longest and shortest axes;
- all six degrees of translation/rotation and afterburner VFX;
- destruction/repair of drive, reactor, bridge, magazines, weapons, and shield;
- save/load with damaged armor, expended ammo, active cooldowns, and custom component state;
- one AI-controlled and one player-controlled battle on host and remote client.
