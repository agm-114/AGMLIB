# Networking and authority

NEBULOUS uses Mirror network behaviors and native routers/RPC contracts. AGMLIB
contains `NetworkBehaviour` ship state and patches many types that can execute on
both server and clients.

## Native replication shapes

- `[SyncVar]` fields project persistent state and may invoke hooks on change.
- Server methods mutate authoritative state.
- `ClientRpc` projects effects or state transitions to clients.
- A host often executes the local RPC body directly and then sends it to
  non-owner clients.
- `NetworkPoolable` uses RPC activation/repool callbacks rather than treating a
  reused object as a fresh spawn.
- `DamageFrame` batches component/structure changes before client application.
- `SaveFileObject` numeric IDs are save-instance references, not Mirror net IDs.
- Large campaign save buffers use a transfer token instead of a normal RPC
  payload.

These shapes must not be collapsed into one generic "networked" assumption.

## Default authority policy

The server/host owns:

- damage and debuff application;
- ammo and resource consumption;
- authoritative spawn/despawn;
- random gameplay selection;
- track truth and command acceptance;
- state that affects victory, points, or availability.

Clients own local presentation unless the native API explicitly requires a
request/RPC path. A client visual must not re-run gameplay mutation.

## Per-feature authority record

Every networked component document should include:

| Concern | Owner | Native/AGMLIB entry point | Replication/failure behavior |
|---|---|---|---|
| Validation | server/client/both | method | rejection diagnostic |
| State mutation | server | method | sync field/RPC/native replication |
| Resource/ammo | server | method | rollback/fallback |
| Spawn/despawn | server | method | pool/network identity |
| Visual/audio | client/all | method | late join/replay behavior |
| Cleanup | server/all | method | disconnect/scene/destruction |

## Common hazards

- Harmony postfix runs on both peers and doubles an effect;
- a host test passes because host and server state share a process;
- client-only target/UI state is used for server gameplay;
- pooled objects retain network owner or target references;
- a sidecar is attached twice by server and client initialization paths;
- an RPC is sent before `NetworkIdentity` registration;
- random choice occurs on every peer.

The smoke-test matrix must include offline, host, remote client, and dedicated
server for gameplay-mutating features.

For pooled objects, add a second-use case to every applicable row. A first
launch can pass while the reused instance retains stale owner, target, timer, or
subscription state.

## Custom munition detonation effects

Custom munition detonation frequently runs under a server guard. A direct
`ObjectPooler` spawn in that path is therefore host-only even when the spawned
prefab is purely visual.

Keep prefabs containing `IDamageDealer` components on the server. Deliver
presentation-only prefabs through a client RPC so every peer renders them
without repeating gameplay mutation.

**Verified (2026-10-03, installed `Nebulous.dll` SHA-256
`86EF9681F3C6B08BD9BEAC2173F903CE29D8197DD4D5E49279EA9CA058F14461`):**
`ModularMissile.DamageableImpact` receives `noVfx` only from
`RuntimeMissileWarhead.TriggerDetonate`. Physical collisions call
`CollisionDetonate` instead, then send an ordinary impact RPC. Setting `noVfx`
inside a shared detonation implementation cannot suppress that collision event.
Do not spawn custom explosion effects for every ordinary impact RPC.

`AngleWarheadNetworkEffectsPatch` recognizes reserved effect sets in the native
`RpcRollImpactEffect`: `-1` plays presentation-only explosions; `-2` plays one
beam with the server's world position, direction, and exact length encoded in
`surfaceSetIndex`. Ordinary impact events retain native behavior. Beam damage
uses the actual raycast target on the server; the client handler only renders
the supplied result. Both peers must use the same AGMLIB implementation.

**Verified (2026-10-05, same installed assembly hash):**
`ModularMissile.GenericImpact` bypasses the warhead for scenery collisions.
The earlier blanket custom-effect receiver supplied their explosion visual;
restricting it to marked detonation events removed that presentation.
`AngleWarheadSceneryEffectsPatch` now sends the marked explosion event from
this server-owned path. It plays only presentation prefabs and does not invoke
the warhead or emit beams. A native testing-range reproduction failed with
zero of one expected visual before this fix, then passed all three pooled
launches with one visual, zero damage effects, and zero damage calls each.

The opt-in two-process loopback TCP fixture verified 40 detonations, 36 native
repools, and four destructions immediately following the effect RPC. Both the
host and remote client received exactly 40 explosions and 1,000 beams; 500 rays
hit the controlled target. Damage calls were 500 on the host and zero on the
client. This establishes native RPC serialization, component routing, delivery,
and authority for the adapter scenario. It does not establish Steam transport
behavior or reproduce an individual player's match.

Separate testing-range checks used finalized installed warhead assets and native
`NetworkObjectPooler`, `Launch`, arming, lookahead/collision, and repool paths.
Nine launches covered physical nuke and beam impacts plus proximity-fused nukes;
a further three launches explicitly verified `trigger=true`. Every launch
detonated once and damaged a controlled `IDamageable` target. Each beam launch
delivered all 25 configured beam events. These checks use a controlled collider
target, so they do not establish actual ship component damage or a full remote
multiplayer match.

**Verified follow-up (2026-10-05, same installed assembly hash):** the loopback
fixture now includes targetless cone hits and misses plus impact/proximity-style
targets to the side and rear at 100 game units. Targeted cases serialize
`Omnidirectional=true` and `SelectRandomPointInTarget=false`. Host and remote
client each received 40 explosions and 1,000 beams with zero direction errors;
750 beams hit as expected. Damage calls were 750 on the host and zero on the
client, with 36 repools and four immediate destructions. This exercises the
[target-aiming compatibility rule](weapons-munitions.md#custom-beam-warhead-targeting)
and confirms that client presentation preserves the server's aimed rays.

A further audit run added scheduler-driven delayed-fuse cancellation on repool,
duplicate trigger suppression, and native `GenericImpact` scenery presentation.
Both processes received 41 explosions (40 detonations plus one scenery impact)
and 1,000 beams, with 750 expected hits and zero direction errors. Damage calls
remained host 750 / client zero. All 38 native repools completed, including the
cancelled fuse and scenery impact; four missiles were destroyed immediately
after sending effects. This remains a loopback adapter test rather than a Steam
match or a measurement of actual ship component damage.
