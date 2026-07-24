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
