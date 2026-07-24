# Small craft, carriers, orders, and tactical AI

## Native shape

The subsystem is not one “drone manager.” Its principal types are:

| Type | Responsibility |
|---|---|
| `SmallCraft/Spacecraft.cs` | Networked craft, loadout, damage, movement, save identity, launch/landing state, EWAR host, storage item. |
| `SmallCraft/SpacecraftGroup.cs` | Pooled network flight, group settings, current order, member/loadout monitors, save state. |
| `SmallCraft/BaseCraftMovement.cs` | Movement, landing target, mass, and movement save component. |
| `Ships/BaseCraftHangarComponent.cs` | Storage and work-slot contract for stored craft. |
| `Ships/BulkCraftHangarComponent.cs` and variants | Concrete storage and reservation policies. |
| `Ships/CraftCarrierController.cs` | Server-owned traffic queue, deck state, pads, pre/postflight work, launch, recovery, and sortie reports. |
| `Game/Orders/Tasks/OrderTask.cs` | Server-executed, serializable order lifecycle. |
| `Game/AI/Tactics/AIControlledShip.cs` | Tactical decision facade including weapons, missiles, damage control, and sorties. |

## Carrier traffic is a queue

`CraftCarrierController` models traffic orders for loadout work, preflight,
launch, landing, and postflight. Each order can reference a craft, hangar,
work slot, reserved pad, planned group, grouped craft, loadout coroutine, and
landing clearance.

The traffic tick:

1. assigns functional work slots for loadout/preflight/postflight work;
2. advances loadout-change coroutines using transfer rate;
3. chooses the smallest suitable available pad;
4. respects deck hold and green-deck envelope;
5. launches or grants landing clearance;
6. removes completed orders and synchronizes launch/landing counts.

`LaunchCraftFlight` validates command state, accepts only stored craft not
already in the traffic queue, requires the selected craft to share a craft type,
creates a planned pooled `SpacecraftGroup`, and enqueues every craft. It does
not mean every queued craft is already active in space.

`EnterLandingQueue` reserves storage before adding a landing order.
`PadRecoveryCompleted` removes the traffic order, commits the sortie report,
and raises recovery callbacks. Postflight work can continue after physical
recovery.

## Command-channel design consequence

A command-channel limit should count the state it intends to limit:

- active flight groups;
- active craft members;
- launched-but-mustering craft;
- pending launches;
- returning/landing craft;
- replacement capacity reserved for losses.

These are not equivalent.

Preventing selection beyond a cap would discard a useful native behavior:
multiple craft can remain queued to replace losses or wait for traffic/work
capacity. The safer design is:

1. allow users and AI to queue a complete sortie;
2. reserve a stable planned group identity;
3. enforce the channel cap at activation/command ownership, not palette
   selection;
4. promote queued replacements when an active member is destroyed, recovered,
   or released;
5. define whether manned and unmanned craft share a pool with a serialized
   policy rather than hard-coding it into selection UI;
6. replicate authoritative counts and rejection reason codes from the server.

The first implementation should observe `OnCraftLaunched`, `OnCraftRecovered`,
group membership changes, destruction, and order completion. It should not
replace the native traffic queue.

## Current `CraftLaunchLimit` audit

Verified from `AGMLIB/Generic Gameplay/Craft/FighterLimit.cs` against the
current native carrier flow. See the
[vanilla hangar and deck contract](power-resources-carriers.md#vanilla-hangar-and-deck-contract)
and the current
[`FighterLimit.cs`](../AGMLIB/Generic%20Gameplay/Craft/FighterLimit.cs):

- `AutoAttachToAllShips` defaults to false, so the Harmony initialization patch
  does not enable the system globally unless a ship already has the component
  or configuration changes that static flag.
- `GetActiveCraft` filters groups by `OwnedBy` and only checks that a craft's
  launching ship has the same owner. It does not require the exact carrier
  ship. One carrier can therefore count craft launched by every ship belonging
  to that player.
- `GetPendingLaunchCount` has the same owner-wide scope.
- The `CraftCarrierController.Tick` prefix suspends launches by changing the
  private traffic order's enum value to `-1`, then restores it in both postfix
  and finalizer.
- An invalid order type skips native pad assignment, preflight loadout progress,
  and launch processing. If the order already owns a reserved pad, it keeps that
  reservation while suspended, which can reduce or block landing traffic.
- The prefix has no explicit server guard even though native `Tick` immediately
  returns on non-server peers.
- `GetQueuedLaunches` and `IsLaunchCandidate` are not called by other current
  AGMLIB source.

This implementation is safe to treat as experimental/incomplete. Do not extend
the invalid-enum suspension technique. Replace it with explicit, server-owned
channel state at a stable carrier/group lifecycle boundary, preserving the
native queue and pad scheduler.

## Spacecraft group order lifecycle

`SpacecraftGroup` owns a synchronized current `OrderTask`. On the server fixed
tick it calls `Update` and then `FixedUpdate` while the order remains active.
The group saves its current order as a polymorphic `SavedOrderTask`; after all
component states and member references load, it reconstructs the order with
`OrderTask.LoadOrder` and calls `ExecuteServer`.

This deferred re-execution is why order features must be idempotent and capable
of resuming after referenced craft resolve.

## General player-order flow

`Game/Orders/PlayerOrder.cs` separates interactive inputs from executable
tasks:

1. apply task UI;
2. execute inputs until all required values exist;
3. latch input values into each task;
4. submit tasks with `OrderTask.ExecuteServer`.

`OrderTask.ExecuteServer` asks the receiver to accept/queue it, may wait for a
precondition, runs server initialization, updates until its assignees finish,
and completes normally or cancelled. Network serialization identifies player
and receiver by `NetworkIdentity`; save serialization uses a polymorphic saved
task and content key.

AGMLIB order extensions need both network and save identities. A task that only
works immediately after a UI click is incomplete.

## Tactical AI

`Game/AI/Tactics/AIControlledShip.cs` is a large orchestration class rather than
a reusable small-craft policy. It:

- evaluates gun and missile options;
- calculates missile launch/programming timing;
- plots salvos and paths;
- manages damage control;
- builds sortie options from stored craft and loadouts;
- limits a sortie using native single-cycle pad capacity;
- launches through `CraftCarrierController`;
- tracks mustering until all queued members launch;
- creates tasks for the launched group.

AGMLIB command-channel logic must cover both player and AI launch paths by
integrating at the carrier/group boundary. Patching only a fleet-editor or
player selection method will not constrain AI sorties.

## Save and authority

Craft, groups, carrier traffic orders, planned groups, landing clearances,
loadout progress, sockets, movement, and orders all carry save state. Cross
references use `SaveFileObject` promises because load order is not guaranteed.

Launch, reservation, landing, group settings, and active orders are
server-owned. Client UI should display synchronized state and reason codes
without independently reserving pads or changing group membership.
