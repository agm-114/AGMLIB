# Campaign, mission, and scripting systems

NEBULOUS currently contains two graph families. They solve related problems but
have different serialization and runtime models.

## Legacy mission/scenario graphs

`Missions/Mission.cs` is an addressable `ScriptableObject` wrapper around an
XNode `MissionGraph`. It loads the graph asynchronously and releases it when
unloaded.

`Missions/MissionGraph.cs`:

1. locates a single `MissionStartNode`;
2. initializes every `BaseMissionNode` and collects `IUpdateNode`s;
3. configures the human player, bots, fleets, colors, strategies, and map;
4. executes setup steps;
5. updates special nodes each tick;
6. calls the current flow node and advances when it returns a next node.

`Missions/ScenarioGraph.cs` is also XNode-based. It locates setup, master,
update, and option nodes; creates/latches match options; runs host/client setup;
evaluates end conditions; and saves only nodes implementing `IStateNode`, keyed
by a stable string.

Consequences:

- graph asset identity and node key identity both matter;
- host and client setup are explicitly different paths;
- adding runtime state to a node requires an `IStateNode` implementation and
  stable key;
- an editor-visible node is not automatically part of live save state.

## Campaign data

`Campaign/NebCampaign.cs` is XML-serializable content, not a network behavior.
It owns campaign metadata, factions, acts, map, characters, tracks, ship
designs, variables, media addresses, and mod dependencies.

It supports local-file and addressable references. Saving removes orphaned ship
designs, writes XML, and updates the file reference. Loading can come from local
files, text assets, or compressed buffers.

Asset addresses and referenced content participate in mod dependency
collection. A campaign can deserialize successfully but still be unusable when
required mods or addresses are missing.

## New scripting graph model

`Scripting/ScriptingGraph.cs` is an XML graph with:

- a GUID graph key;
- polymorphic `ScriptingNode` instances;
- GUID node keys;
- named typed input/output ports;
- separately serialized edge records;
- clipboard duplication with node-key remapping;
- live node-state save/load;
- room-load, reset, and script-stop callbacks;
- mod dependency and editor-warning collection.

`Scripting/Nodes/ScriptingNode.cs` builds its port and serialized-field metadata
from attributes and reflection. Output ports invoke an evaluator method.
`[SerializeField]` fields on node classes are included in XML node data.

`Scripting/Nodes/SequenceFlow/SequencedNode.cs` executes a chain until a node
blocks by returning itself, reaches a stable node, or ends. The no-blocking path
resets nodes and keeps following next steps.

`ScriptingGraphCollection` constrains a collection to a graph base type, creates
graphs through the expected campaign/room constructor, and saves live graph
states by GUID.

## Campaign network projection

`Game/CampaignStateSynchronizer.cs` is the network bridge. It synchronizes
difficulty, act, destination room, and flagship; links spawned campaign
entities to network identities; projects camera/dialog/music/map actions with
RPCs; and transfers the compressed `CampaignSaveState` through the large-file
transfer manager.

The server owns the campaign save state. Clients wait on a transfer token,
decode the completed buffer, and replace their local projected state.

## Extension rules

- Do not treat XNode mission nodes and new scripting nodes as interchangeable.
- Give new graph/node types stable polymorphic type identities and stable keys.
- Constructor shape is part of scripting-graph deserialization.
- Separate authored configuration from live node state.
- Keep gameplay mutation on server graph controls; RPC methods should project
  presentation or synchronized results.
- Collect mod dependencies for every asset/content address introduced by a
  custom node.
- Validate unconnected ports, missing polymorphic types, missing referenced
  content, and duplicate keys before publishing a campaign.

