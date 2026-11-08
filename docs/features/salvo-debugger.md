# Salvo debugger

A diagnostic recorder for missile flight paths and AGMLIB beam-warhead
damage. It records the actual authoritative damage call; inspecting a trace does
not fire another ray or apply damage again.

## Use

1. Run Testing Range or host a match. Firing a missile automatically opens the
   debugger with its salvo selected. **Ctrl+F8** also opens or hides it.
2. Capture is enabled by default. **Capture local salvos** can disable it;
   enabling or disabling capture clears the previous history. Setting the process
   environment variable `AGMLIB_SALVO_DEBUG=0` disables capture at startup.
3. The selected salvo updates as its missiles fly and detonate. Each new salvo
   opens the inspector once, regardless of warhead type. Click a finished salvo
   in the native salvo list, or use History to inspect an earlier launch.
4. Recorded paths and damage rays appear in world space in the battle view. Use
   the normal battle camera and **Focus hit** to move to a selected impact. The
   narrow inspector leaves the battle view available for navigation.
5. Hover an external or internal damage ray in the battle view to read its
   explanation. The last inspected ray stays selected after the pointer leaves
   it. Select a numbered ray to inspect coincident rays. **Flight paths** and
   **Internal rays** control the world overlay. Select an individual missile to
   isolate it.
6. Use the numbered ray selector to inspect coincident rays. Its count includes
   all recorded rays for the shown missiles; several rays can share the same path.

Cyan lines show sampled flight paths. Green rays reported damage, amber rays
hit without reporting damage, and gray rays missed. Internal and spalling rays
use the same outcome colors, based on their own native component-damage call.
A ray with intersections but no applied damage is amber. Selection thickens
the internal rays while preserving their outcome colors; the selected external
beam is white.
World-space geometry retains the recorded positions, so it does not follow ships
that move after the hit. Internal rays remain visible through hull geometry.

## Damage explanation

Beam-warhead traces include the actual ray origin and endpoint, hit target and
collider, weighted socket size, separate component and penetration curve inputs
and multipliers, the concrete damage dealer's characteristics, its settings,
the final `HitResult`, and the damage reported by `IDamageable.DoDamage`.
Each internal ray separately records its intersection count and the damage
reported by its `IDamageDealer.DamageComponents` call. JSON exports use schema 2.

Native ship damage steps additionally record armor penetration tests, slope,
effective remaining armor, ricochet candidates, damage reduction, internal ray
intersections before ship filtering, and component damage allocations. Component
health includes pending damage in the native damage frame. Allocated damage can
exceed the HP available to lose; the final reported damage remains a separate
measurement. A ricochet candidate is not necessarily the final result because
the damage dealer can suppress ricochets.

Range curves currently receive **world units**, while displayed distances use
**10 metres per world unit**. Both are shown. The trace records the current
calculation and does not change range scaling. Cone-area hits are explicitly
labelled as overlap hits because their current implementation does not perform
a line-of-sight raycast.

## Export and limits

**Export JSON** writes the selected traces under
`Application.persistentDataPath/AGMLIB/SalvoTraces/`. The file includes schema,
game-assembly, and AGMLIB versions. It contains snapshots rather than references
to pooled missiles or leased hit-info objects. Export is local; the debugger
does not upload it. There is no saved-trace importer yet.

Capture only covers missiles launched after activation by the local host's
player. Remote clients and other players' salvos are not recorded. All missile
types can have flight paths; detailed damage explanations currently cover
`BeamWarheadDescriptor` and its subclasses. Additional explosion prefabs,
critical-effect chains, crew casualties, and non-beam warheads are not a complete
damage ledger. The inspector labels its total as **traced HP** for that reason.

Paths are sampled at approximately 10 Hz in game time and progressively
decimated for long flights while retaining their endpoints. Retention is bounded
to 32 salvos, 64 missiles per salvo, 256 retained missiles, 1,024 path points per
missile, 256 rays per missile, 1,024 retained rays, and 128 steps per ray. Completed
salvos are evicted to make room. Dropped captures and decimation are shown in the
window. An exception during a damage call leaves its ray marked incomplete.
The world overlay displays at most 1,024 lines, prioritizing the selected ray;
the inspector reports additional hidden lines. Rendering adds no colliders.
History clears on scene changes and is not saved automatically.

Source: [recorder](../../AGMLIB/Diagnostics/SalvoDebugRecorder.cs),
[viewer](../../AGMLIB/Diagnostics/SalvoDebugWindow.cs), and
[beam integration](../../AGMLIB/Munitions/ModularMissile/BeamWarheadDescriptor.cs).

## Validation

**Verified** on game `0.6.2.6:260903-1430`: a launched beam missile recorded ten
rays against a live ship, including armor, damage reduction, internal rays, and
allocations. Traced damage matched measured ship health loss. The native finished
salvo button opened the inspector, and exporting preserved ray counts and totals.
World-space renderer positions, supported materials, hover mapping, absence of
colliders, and hiding the overlay with the inspector passed automated checks.
Persistent selection and native per-internal-ray damage outcomes pass the
23-check adapter suite; the interactive range exercised damage, zero-damage,
and missed internal rays. The viewer only renders in world space.
The ordinary Debug build excludes the opt-in local test fixtures.
