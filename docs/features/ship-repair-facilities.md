# Ship repair facilities

`Lib.Generic_Gameplay.ShipRepairFacility` adds station-style component repair services
to every native `Ships.ReplenishmentDockingPoint` on its owning ship. It is a
`MonoBehaviour` authored alongside native components; it does not replace the ship,
installed component, or docking rig.

## Placement

- **Installable upgrade:** attach the facility to a `HullComponent` GameObject or
  one of its children. It supplies service while that component is functional and
  not destroyed.
- **Built-in service:** attach it to a replenishment docking point, the hull, or
  another child under the ship controller with no enclosing `HullComponent`.
  It supplies service while the facility is enabled and its GameObject is active.

Discovery starts at the owning `ShipController` and searches its descendants,
including sibling branches and installed equipment. Placement on one berth enables
service at all replenishment berths on that ship. The facility must be inside the
ship hierarchy, rather than a separate scene object. Reparented facilities are
associated with their new ship. Hierarchy additions/removals are discovered within
0.25 seconds; cached facilities' enabled and functional states are checked each frame.

## Settings

| Field | Default | Behavior |
| --- | --- | --- |
| `ComponentMaxRepair` | `0.5` | Native station-style component repair allowance, from 0 to 1. Zero disables this facility. |
| `ComponentRepairRateBonus` | `0.75` | Additional repair-speed modifier, from 0 to 1; `0.75` means +75%. |

Multiple available facilities use the largest repair allowance and the largest
speed bonus independently. Identical upgrades do not stack. When an upgrade stops
working, service falls back to the remaining available facilities.

## Runtime behavior

Docked ships use their own native damage-control teams. The facility supplies the
repair allowance and speed bonus, while native damage control performs healing and
replicates it. Benefits are removed on undocking or facility loss. This does not
add docking berths, a tender repair workforce, or armor repair.

Existing tender restore supplies remain available through native storage. A tender
without restore lockers exposes an empty, zero-capacity restore pool so the native
repair panel can display component service without requiring restore supplies.
This pool grants no restores or storage capacity. The native panel captures repair
capability text when opened; reopen it after changing facilities to refresh that text.

## Skirmish docking marker

`Lib.Generic_Gameplay.SkirmishTenderDocking` is a separate, optional sidecar for a
tender with native replenishment berths. Add it to the hull, a docking point, or
an installable `HullComponent`, using the same placement rules as the repair facility.
It has no configuration fields and works independently of `ShipRepairFacility`.

Select the receiving ship and right-click the marked tender. In skirmish, the
marker opens the native movement commands so **DOCK** is presented immediately,
including when targeting a sensor track would otherwise open weapon commands.
The normal docking order performs approach, alignment, reservation, and docking;
the marker does not teleport ships or add berths. Native team, compatibility,
occupancy, and command restrictions still apply. A marker installed on a failed
module stops prioritizing the command.

A marked ship must have a native `Ships.ReplenishmentDockingPoint` registered in
the hull's subpart hierarchy. The native hull attachment positions describe how
that ship docks elsewhere; they do not provide accepting berths. At ship setup,
AGMLIB logs an authoring error naming the hull and ship if no replenishment berth
is registered. The marker does not generate docking geometry.

Vanilla already allows compatible friendly ships to dock with tenders in skirmish
through **MOV → DOCK**. The marker prioritizes this existing command; it does not
remove vanilla docking from unmarked tenders or alter campaign behavior.

## Verification

Build and Unity adapter verification cover hierarchy discovery, native repair
getters, module failure/recovery, disabled facilities, independent strongest values,
multiple berths, ship isolation, reparenting, cloned ship ownership, and the empty
storage pool. A live Testing Range run also verified the normal native docking
order in skirmish, applied repair stats, and the repair panel's 50% allowance.
Marker checks cover module failure/recovery, disabling, command presentation,
and docking without an enabled repair facility.
Live Testing Range verification also confirms that marked hulls with no registered
replenishment berths log the authoring error and have no native DOCK command.
Damaged-component healing, save/load, and multiplayer remain unverified.

Implementation: [`ShipRepairFacility.cs`](../../AGMLIB/Generic%20Gameplay/ShipRepairFacility.cs).
Docking marker: [`SkirmishTenderDocking.cs`](../../AGMLIB/Generic%20Gameplay/SkirmishTenderDocking.cs).
