# Native source file atlas

Every decompiled C# file from pinned Nebulous.dll is listed. Generated from assembly SHA-256 `26DE96E2E384DA2F50AFF01CC1D2FDEE42DD2E46FE540083F96E249D5CF58365`.

| Source | Lines | Namespace | Declared types and bases | Lifecycle | Signals |
|---|---:|---|---|---|---|
| `Assets/Source/UI/HorizontalLayoutGroup_HeightFirst.cs` | 19 | Assets.Source.UI | class HorizontalLayoutGroup_HeightFirst : HorizontalLayoutGroup |  |  |
| `Assets/Source/UI/ScreenToWorldPositionIndicator.cs` | 60 | Assets.Source.UI | class ScreenToWorldPositionIndicator : Poolable | LateUpdate | pooling |
| `Bundles/BundleManager.cs` | 1054 | Bundles | class BundleManager<br>struct ModLoadReport |  | AssetBundle, Addressables, prefab |
| `Bundles/BundleManifest.cs` | 85 | Bundles | class BundleManifest<br>class Entry |  |  |
| `Bundles/IBundleKeyed.cs` | 9 | Bundles | interface IBundleKeyed : ISaveKeyed |  |  |
| `Bundles/IManagedAddressableAsset.cs` | 9 | Bundles | interface IManagedAddressableAsset |  |  |
| `Bundles/ISaveKeyed.cs` | 7 | Bundles | interface ISaveKeyed |  |  |
| `Bundles/PreloadBundleDummyAsset.cs` | 9 | Bundles | class PreloadBundleDummyAsset : ScriptableObject |  |  |
| `Bundles/ResourceDefinitions.cs` | 53 | Bundles | class ResourceDefinitions<br>class ResourceFile |  |  |
| `Cameras/CameraSettings.cs` | 38 | Cameras | class CameraSettings : MonoBehaviour | Awake, OnDestroy |  |
| `Cameras/CutsceneCamera.cs` | 59 | Cameras | class CutsceneCamera : MonoBehaviour, ICutsceneTrackTarget, ICampaignEditorGizmoDrawer | OnEnable |  |
| `Cameras/ImprovedOrbitCamera.cs` | 980 | Cameras | class ImprovedOrbitCamera : MonoBehaviour<br>class CameraSaveData<br>class Mode | Awake, LateUpdate, OnDestroy | Command, prefab |
| `Cameras/OrbitCamera.cs` | 201 | Cameras | class OrbitCamera : MonoBehaviour | Awake, FixedUpdate, LateUpdate |  |
| `Cameras/ShoulderCam.cs` | 96 | Cameras | class ShoulderCam : MonoBehaviour | OnDisable, Update | prefab |
| `Cameras/SkirmishCinematicCamera.cs` | 296 | Cameras | class SkirmishCinematicCamera : MonoBehaviour<br>enum OverworldTravelCameraMovement<br>struct OverworldTravelAnimation | OnDisable | prefab |
| `Cameras/WorkshopScreenshotCamera.cs` | 60 | Cameras | class WorkshopScreenshotCamera : MonoBehaviour | Awake, OnDestroy | prefab |
| `Campaign/ActChanged.cs` | 4 | Campaign |  |  |  |
| `Campaign/Animatic.cs` | 58 | Campaign | class Animatic : ScriptableObject<br>struct Subtitle<br>enum SubtitlePosition |  |  |
| `Campaign/CampaignAct.cs` | 252 | Campaign | class CampaignAct : IEditableOverworldElement, IEditableCampaignElement, IXmlDocSerializable, IModDependencySource |  | XML |
| `Campaign/CampaignAssetTags.cs` | 41 | Campaign | class CampaignAssetTags |  |  |
| `Campaign/CampaignEndResult.cs` | 9 | Campaign | enum CampaignEndResult |  |  |
| `Campaign/CampaignFaction.cs` | 409 | Campaign | class CampaignFaction : IXmlDocSerializable, ICampaignIDObject<br>enum FactionAllegiance<br>class Selectable : IInspectableDropdownParam, IXmlDocSerializable, IWarningGeneratingDropdownParam, ISyncDropdown |  | NetworkServer, XML |
| `Campaign/CampaignFleetSorter.cs` | 37 | Campaign | class CampaignFleetSorter : IComparer<ShipController> |  |  |
| `Campaign/CampaignFleetTemplates.cs` | 479 | Campaign | class CampaignFleetTemplates : IXmlDocSerializable<br>interface IMutableDesign<br>class Design : IMutableDesign<br>class DesignComparer : IComparer<Design> |  | XML |
| `Campaign/CampaignObjective.cs` | 425 | Campaign | class CampaignObjective : ICampaignIDObject, IXmlDocSerializable, ISyncObject<br>enum ObjectiveType<br>enum ObjectiveStatus<br>class SaveState<br>struct Timer |  | XML |
| `Campaign/CampaignStorableItemsSet.cs` | 180 | Campaign | class CampaignStorableItemsSet : IStorableItemSet, IMunitionCollection |  | prefab |
| `Campaign/CampaignVariable.cs` | 139 | Campaign | class CampaignVariable : ICampaignVariable, IXmlDocSerializable |  | XML |
| `Campaign/CampaignVariableHelpers.cs` | 64 | Campaign | class CampaignVariableHelpers |  | XML |
| `Campaign/CampaignVarSelectionSet.cs` | 146 | Campaign | class CampaignVarSelectionSet : IInspectableDropdownParam, IXmlDocSerializable, IWarningGeneratingDropdownParam |  | XML |
| `Campaign/Cast/CampaignCharacter.cs` | 272 | Campaign.Cast | class CampaignCharacter : IXmlDocSerializable, ICampaignIDObject, IModDependencySource |  | XML |
| `Campaign/Cast/CharacterPortrait.cs` | 97 | Campaign.Cast | class CharacterPortrait : MonoBehaviour |  | prefab |
| `Campaign/Cast/PortraitComponents.cs` | 96 | Campaign.Cast | class PortraitComponents : ScriptableObject<br>struct Selection : IXmlDocSerializable |  | XML |
| `Campaign/Cast/PortraitSet.cs` | 124 | Campaign.Cast | class PortraitSet : ScriptableObject<br>struct Selection : IXmlDocSerializable |  | XML |
| `Campaign/EditorSettings.cs` | 25 | Campaign | class EditorSettings : IXmlDocSerializable |  | XML |
| `Campaign/Entities/AreaDefinition.cs` | 148 | Campaign.Entities | class AreaDefinition : BaseIdentitiedEntityScript, IInternalParamsEntity, IInspectable, ICampaignEditorGizmoDrawer<br>enum AreaShape |  | Command, XML |
| `Campaign/Entities/BarnacleMinefieldSpawner.cs` | 76 | Campaign.Entities | class BarnacleMinefieldSpawner : MinefieldSpawner<br>enum Shape |  | XML |
| `Campaign/Entities/BaseIdentitiedEntityNetworkScript.cs` | 104 | Campaign.Entities | class BaseIdentitiedEntityNetworkScript : NetworkBehaviour, IIdentifiedEntityScript, ISpawnedFromEntity | Awake, OnDestroy, OnStartClient | NetworkBehaviour, prefab |
| `Campaign/Entities/BaseIdentitiedEntityScript.cs` | 75 | Campaign.Entities | class BaseIdentitiedEntityScript : MonoBehaviour, IIdentifiedEntityScript, ISpawnedFromEntity | Awake, OnDestroy | prefab |
| `Campaign/Entities/BasicEntity.cs` | 105 | Campaign.Entities | class BasicEntity : Entity<br>class BasicEntitySaveState : EntitySaveState |  | XML, prefab |
| `Campaign/Entities/DirectionalLightRig.cs` | 81 | Campaign.Entities | class DirectionalLightRig : MonoBehaviour, IInspectable, IInternalParamsEntity | Awake | XML |
| `Campaign/Entities/Entity.cs` | 801 | Campaign.Entities | class Entity : IXmlDocSerializable, IInspectable, ICutsceneTrackTarget, IModDependencySource<br>class EntitySaveState : IXmlDocSerializable<br>class InternalParamSaveHelper : IXmlDocSerializable |  | save-state, XML, prefab |
| `Campaign/Entities/EntityScriptReference.cs` | 65 | Campaign.Entities | class EntityScriptReference |  |  |
| `Campaign/Entities/IEntitySpawnResources.cs` | 39 | Campaign.Entities | interface IEntitySpawnResources |  |  |
| `Campaign/Entities/IIdentifiedEntityScript.cs` | 7 | Campaign.Entities | interface IIdentifiedEntityScript : ISpawnedFromEntity |  |  |
| `Campaign/Entities/IInternalParamsEntity.cs` | 12 | Campaign.Entities | interface IInternalParamsEntity : IInspectable |  | XML |
| `Campaign/Entities/ISpawnedEntityHandle.cs` | 13 | Campaign.Entities | interface ISpawnedEntityHandle |  | prefab |
| `Campaign/Entities/ISpawnedFromEntity.cs` | 7 | Campaign.Entities | interface ISpawnedFromEntity |  |  |
| `Campaign/Entities/MinefieldSpawner.cs` | 302 | Campaign.Entities | class MinefieldSpawner : NetworkBehaviour, ISpawnedFromEntity, IInternalParamsEntity, IInspectable, ICampaignEditorGizmoDrawer, IImbuedObjectSource, ILaunchingPla...<br>struct MinePlacement | Awake, OnDestroy, OnStartClient, OnStartServer | NetworkBehaviour, save-state, XML |
| `Campaign/Entities/NetworkedEntity.cs` | 120 | Campaign.Entities | class NetworkedEntity : Entity<br>class NetworkedEntitySaveState : EntitySaveState<br>struct SpawnedAddressableNetworkedEntity : ISpawnedEntityHandle |  | NetworkServer, XML, Addressables, prefab |
| `Campaign/Entities/PrototypingPrimitive.cs` | 95 | Campaign.Entities | class PrototypingPrimitive : MonoBehaviour, IInternalParamsEntity, IInspectable<br>enum PrimColor<br>enum Scale | Awake | XML |
| `Campaign/Entities/RemoteSpawnedEntity.cs` | 26 | Campaign.Entities | struct RemoteSpawnedEntity : ISpawnedEntityHandle |  | prefab |
| `Campaign/Entities/RockHaloSkybox.cs` | 216 | Campaign.Entities | class RockHaloSkybox : BaseIdentitiedEntityScript, IInternalParamsEntity, IInspectable | Awake | XML, prefab |
| `Campaign/Entities/SelectableJSONPlanetSpawner.cs` | 83 | Campaign.Entities | class SelectableJSONPlanetSpawner : MonoBehaviour, ISpawnedFromEntity, IInternalParamsEntity, IInspectable | Awake | XML, prefab |
| `Campaign/Entities/ShipEntity.cs` | 1279 | Campaign.Entities | class ShipEntity : Entity<br>class ShipEntitySaveState : EntitySaveState<br>class ShipReference<br>class ShipSpawnAllocation : IXmlDocSerializable, IInspectable, IInspectableDropdownParam<br>class ActiveAllocation : ShipSpawnAllocation<br>class InactiveStandbyAllocation : ShipSpawnAllocation<br>class DockedAllocation : ShipSpawnAllocation<br>class ConditionalSpawnAllocation : ShipSpawnAllocation<br>class WreckAllocation : ShipSpawnAllocation<br>class ShipPursuitCondition : IXmlDocSerializable<br>class ShipStatusChecker : IXmlDocSerializable<br>class HasOffensiveAbility : ShipStatusChecker<br>class ShipPursuitNever : ShipPursuitCondition<br>class ShipPursuitAlways : ShipPursuitCondition<br>class ShipPursuitVariable : ShipPursuitCondition<br>class ShipPursuitDestination : ShipPursuitCondition<br>class ShipPursuitDestinationNot : ShipPursuitCondition<br>class ShipPursuitDestNotOrigin : ShipPursuitCondition<br>struct SpawnedShipEntity : ISpawnedEntityHandle |  | NetworkServer, save-state, XML, Addressables, prefab |
| `Campaign/Entities/SimpleMinefieldSpawner.cs` | 89 | Campaign.Entities | class SimpleMinefieldSpawner : MinefieldSpawner<br>enum Shape |  | XML |
| `Campaign/Entities/SkyboxEntity.cs` | 167 | Campaign.Entities | class SkyboxEntity : Entity<br>class SkyboxLayerHandle : ISpawnedEntityHandle, ISkyboxLayerSpawner, ISkyboxLayer |  | XML, Addressables, prefab |
| `Campaign/Entities/SkyboxLocalStar.cs` | 64 | Campaign.Entities | class SkyboxLocalStar : MonoBehaviour, IInternalParamsEntity, IInspectable |  | XML |
| `Campaign/Entities/SpawnedAddressableGOEntity.cs` | 28 | Campaign.Entities | struct SpawnedAddressableGOEntity : ISpawnedEntityHandle |  | Addressables, prefab |
| `Campaign/Entities/ToroidalMinefieldSpawner.cs` | 59 | Campaign.Entities | class ToroidalMinefieldSpawner : MinefieldSpawner |  | XML |
| `Campaign/FleetFormationUtility.cs` | 150 | Campaign | class FleetFormationUtility<br>interface IFormationShip<br>struct LayoutPosition<br>class LayoutGroup<br>class Sorter : IComparer<IFormationShip> |  |  |
| `Campaign/FleetTransitFormation.cs` | 347 | Campaign | class FleetTransitFormation<br>class Serializable<br>class SavedLayoutGroup<br>struct SavedLayoutPosition<br>struct LayoutPosition<br>class LayoutGroup<br>class Sorter : IComparer<ShipController> |  |  |
| `Campaign/ICampaignGUIDObject.cs` | 11 | Campaign | interface ICampaignGUIDObject |  |  |
| `Campaign/ICampaignIDObject.cs` | 9 | Campaign | interface ICampaignIDObject |  |  |
| `Campaign/ICampaignLoader.cs` | 11 | Campaign | interface ICampaignLoader |  |  |
| `Campaign/ICampaignVariable.cs` | 24 | Campaign | interface ICampaignVariable : IXmlDocSerializable |  |  |
| `Campaign/IListableCampaign.cs` | 18 | Campaign | interface IListableCampaign : ICampaignLoader |  |  |
| `Campaign/Map/CampaignHexMap.cs` | 1120 | Campaign.Map | class CampaignHexMap : IXmlDocSerializable, INotifyCollectionChanged<br>interface IMutableMapVert<br>class Room : IMutableMapVert, IXmlDocSerializable, ICampaignWarningSource, ICampaignIDObject, IModDependencySource |  | XML |
| `Campaign/Map/CampaignMapTrack.cs` | 383 | Campaign.Map | class CampaignMapTrack : IXmlDocSerializable, ICampaignIDObject, ISyncObject, IXmlSaveState<int><br>class TrackSaveHelper : XmlSaveStateHelperWithID<CampaignMapTrack, int><br>class Movement : IXmlDocSerializable<br>class SteppedMovement : Movement<br>class Point2PointSteppedMovement : SteppedMovement |  | save-state, XML |
| `Campaign/Map/CampaignMapTrackFlag.cs` | 113 | Campaign.Map | class CampaignMapTrackFlag : MonoBehaviour | OnDestroy | prefab |
| `Campaign/Map/CampaignMapUnderlay.cs` | 115 | Campaign.Map | class CampaignMapUnderlay : IXmlDocSerializable, INotifyCollectionChanged, IModDependencySource<br>class RootElementData : UnderlayElementData |  | XML |
| `Campaign/Map/CampaignRoomLoader.cs` | 269 | Campaign.Map | class CampaignRoomLoader : ISkirmishBattlespaceLoader, IEntitySpawnResources<br>class LoadingData : MapLoadingData |  | NetworkServer, Addressables, prefab |
| `Campaign/Map/HexMapRenderer.cs` | 534 | Campaign.Map | class HexMapRenderer : MonoBehaviour | Update | prefab |
| `Campaign/Map/IMapUnderlayElementRenderer.cs` | 16 | Campaign.Map | interface IMapUnderlayElementRenderer |  | prefab |
| `Campaign/Map/ITileZone.cs` | 17 | Campaign.Map | interface ITileZone |  |  |
| `Campaign/Map/MapCalloutFlag.cs` | 123 | Campaign.Map | class MapCalloutFlag : NetworkPoolable | OnEnable, OnUnpooled | NetworkBehaviour, SyncVar, NetworkServer, pooling |
| `Campaign/Map/MapHexTile.cs` | 293 | Campaign.Map | class MapHexTile : MonoBehaviour, IEditableOverworldElement, IEditableCampaignElement | OnDestroy | prefab |
| `Campaign/Map/MapObjectiveFlag.cs` | 277 | Campaign.Map | class MapObjectiveFlag : MonoBehaviour<br>class ObjectiveItem : MonoBehaviour<br>struct Appearance | Awake, OnDestroy | prefab |
| `Campaign/Map/MapReplenishmentFlag.cs` | 93 | Campaign.Map | class MapReplenishmentFlag : MonoBehaviour<br>class InventoryItem : MonoBehaviour | Awake, OnDestroy | prefab |
| `Campaign/Map/MapRouteConnection.cs` | 122 | Campaign.Map | class MapRouteConnection : MonoBehaviour | OnDestroy | prefab |
| `Campaign/Map/MapUnderlayAsteroidBelt.cs` | 187 | Campaign.Map | class MapUnderlayAsteroidBelt : MapUnderlayElement<MapUnderlayAsteroidBelt.AsteroidBeltData><br>class AsteroidBeltData : UnderlayElementData<br>class AsteroidBeltRenderer : ImmediateModeShapeDrawer<br>struct Rock | Awake | XML, prefab |
| `Campaign/Map/MapUnderlayElement.cs` | 62 | Campaign.Map | class MapUnderlayElement : MonoBehaviour, IMapUnderlayElementRenderer where TElementData : UnderlayElementData | OnDestroy |  |
| `Campaign/Map/MapUnderlayGate.cs` | 25 | Campaign.Map | class MapUnderlayGate : MapUnderlayOrbitingObject<MapUnderlayGate.GateData><br>class GateData : OrbitingObjectData |  |  |
| `Campaign/Map/MapUnderlayLabeledElement.cs` | 100 | Campaign.Map | class MapUnderlayLabeledElement : MapUnderlayElement<TElementData> where TElementData : MapUnderlayLabeledElement<TElementData>.LabeledUnderlayElementData<br>class LabeledUnderlayElementData : UnderlayElementData |  | XML |
| `Campaign/Map/MapUnderlayOrbitingObject.cs` | 230 | Campaign.Map | class MapUnderlayOrbitingObject : MapUnderlayLabeledElement<TElementData> where TElementData : MapUnderlayOrbitingObject<TElementData>.OrbitingObjectData<br>class OrbitingObjectData : LabeledUnderlayElementData |  | XML |
| `Campaign/Map/MapUnderlayPlanet.cs` | 165 | Campaign.Map | class MapUnderlayPlanet : MapUnderlayOrbitingObject<MapUnderlayPlanet.PlanetData><br>class PlanetData : OrbitingObjectData |  | XML, prefab |
| `Campaign/Map/MapUnderlayStar.cs` | 174 | Campaign.Map | class MapUnderlayStar : MapUnderlayLabeledElement<MapUnderlayStar.StarData><br>class StarData : LabeledUnderlayElementData |  | XML |
| `Campaign/Map/MapUnitFlag.cs` | 44 | Campaign.Map | class MapUnitFlag : MonoBehaviour |  | prefab |
| `Campaign/Map/RoomSkybox.cs` | 81 | Campaign.Map | class RoomSkybox<br>class Layer : ISkyboxLayerSpawner |  | prefab |
| `Campaign/Map/RoomTag.cs` | 12 | Campaign.Map | enum RoomTag |  |  |
| `Campaign/Map/UnderlayElementData.cs` | 182 | Campaign.Map | class UnderlayElementData : IXmlDocSerializable, ICampaignIDObject, IComparable<UnderlayElementData>, IHierarchyElement, IInspectable |  | XML, Addressables, prefab |
| `Campaign/NebCampaign.cs` | 1284 | Campaign | class NebCampaign : IXmlDocSerializable, ICampaignWarningSource, ISteamWorkshopItem<br>class CampaignFileSummary : IFileSummary, IComparable<CampaignFileSummary>, IListableCampaign, ICampaignLoader<br>class CampaignFileSummary_LocalFile : CampaignFileSummary<br>class CampaignFileSummary_Addressable : CampaignFileSummary<br>class Reference : IXmlDocSerializable, ICampaignLoader<br>class LocalFileRef : Reference<br>class AddressableRef : Reference |  | XML, Addressables |
| `Campaign/PublishedNebCampaign.cs` | 62 | Campaign | class PublishedNebCampaign : IXmlDocSerializable |  | XML |
| `Campaign/QuickTestSettings.cs` | 73 | Campaign | class QuickTestSettings |  |  |
| `Campaign/Saving/CampaignSaveCollection.cs` | 244 | Campaign.Saving | class CampaignSaveCollection : IXmlDocSerializable, ISavedGameFile, IFileSummary |  | XML |
| `Campaign/Saving/CampaignSaveState.cs` | 529 | Campaign.Saving | class CampaignSaveState : IXmlDocSerializable<br>enum SaveType<br>class Summary : ISavedGameFile, IFileSummary, IComparable<Summary><br>class SavedShip : IXmlDocSerializable<br>class LiveRoomState : IXmlDocSerializable<br>class MidTransitState : IXmlDocSerializable |  | save-state, XML |
| `Campaign/Saving/RoomSaveState.cs` | 63 | Campaign.Saving | class RoomSaveState : IXmlDocSerializable |  | XML |
| `Campaign/Saving/SavedVariableState.cs` | 9 | Campaign.Saving | struct SavedVariableState |  |  |
| `Campaign/Sequencing/AnimationBlockTrack.cs` | 240 | Campaign.Sequencing | class AnimationBlockTrack : BlockTrack<br>class CreateTarget : ICutsceneTrackTarget<br>class AnimationBlock : Block |  | XML |
| `Campaign/Sequencing/AudioBlockTrack.cs` | 441 | Campaign.Sequencing | class AudioBlockTrack : BlockTrack<br>class CreateTarget : ICutsceneTrackTarget<br>class OneShotBlock : AudioClipBlock<br>class LoopingBlock : AudioClipBlock<br>class AudioClipBlock : Block, IPreloadableAsset<br>enum MixerSelection |  | XML, Addressables, prefab |
| `Campaign/Sequencing/BlockTrack.cs` | 446 | Campaign.Sequencing | class BlockTrack : Track<br>class Block : TrackElement, IBlockTrackBlock, IXmlDocSerializable, IModDependencySource<br>class PositionalBlock : Block<br>class PositionalBlockEditor : TriadWidget.ITransformable, ISequenceElementEditor |  | XML |
| `Campaign/Sequencing/CameraTrack.cs` | 157 | Campaign.Sequencing | class CameraTrack : TransformTrack |  | XML |
| `Campaign/Sequencing/CutsceneSequence.cs` | 193 | Campaign.Sequencing | class CutsceneSequence : IXmlDocSerializable, ICampaignIDObject, ICampaignEditorGizmoDrawer, IModDependencySource |  | XML |
| `Campaign/Sequencing/DialogTrack.cs` | 354 | Campaign.Sequencing | class DialogTrack : Track<br>class CreateTarget : ICutsceneTrackTarget<br>class Line : TrackElement, IXmlDocSerializable, IModDependencySource, IPreloadableAsset |  | XML |
| `Campaign/Sequencing/EntityTrack.cs` | 206 | Campaign.Sequencing | class EntityTrack : KeyframedTrack, TriadWidget.ITransformable |  | XML |
| `Campaign/Sequencing/IBlockTrackBlock.cs` | 44 | Campaign.Sequencing | interface IBlockTrackBlock |  |  |
| `Campaign/Sequencing/ICutsceneTrackTarget.cs` | 13 | Campaign.Sequencing | interface ICutsceneTrackTarget |  |  |
| `Campaign/Sequencing/IKeyframe.cs` | 13 | Campaign.Sequencing | interface IKeyframe |  |  |
| `Campaign/Sequencing/IKeyframedParam.cs` | 22 | Campaign.Sequencing | interface IKeyframedParam |  |  |
| `Campaign/Sequencing/IKeyframeRelativePositionScope.cs` | 17 | Campaign.Sequencing | interface IKeyframeRelativePositionScope : IKeyframeRelativeScope |  |  |
| `Campaign/Sequencing/IKeyframeRelativeScope.cs` | 11 | Campaign.Sequencing | interface IKeyframeRelativeScope |  |  |
| `Campaign/Sequencing/KeyframeChanged.cs` | 4 | Campaign.Sequencing |  |  |  |
| `Campaign/Sequencing/KeyframeDerivative.cs` | 9 | Campaign.Sequencing | enum KeyframeDerivative |  |  |
| `Campaign/Sequencing/KeyframedFloat.cs` | 26 | Campaign.Sequencing | class KeyframedFloat : KeyframedParameter<float> |  |  |
| `Campaign/Sequencing/KeyframedParameter.cs` | 412 | Campaign.Sequencing | class KeyframedParameter : IKeyframedParam, IXmlDocSerializable<br>class Keyframe : IKeyframe, IXmlDocSerializable<br>class WorldScope : IKeyframeRelativePositionScope, IKeyframeRelativeScope |  | XML |
| `Campaign/Sequencing/KeyframedQuaternion.cs` | 28 | Campaign.Sequencing | class KeyframedQuaternion : KeyframedParameter<Quaternion> |  |  |
| `Campaign/Sequencing/KeyframedTrack.cs` | 375 | Campaign.Sequencing | class KeyframedTrack : Track<br>class EntityRelativeScope : TrackElement, IKeyframeRelativePositionScope, IKeyframeRelativeScope, IXmlDocSerializable<br>enum RelativeMode |  | XML |
| `Campaign/Sequencing/KeyframedVector3.cs` | 63 | Campaign.Sequencing | class KeyframedVector3 : KeyframedParameter<Vector3> |  |  |
| `Campaign/Sequencing/MusicBlockTrack.cs` | 255 | Campaign.Sequencing | class MusicBlockTrack : BlockTrack<br>class CreateTarget : ICutsceneTrackTarget<br>class MusicBlock : Block, IPreloadableAsset |  | XML, Addressables |
| `Campaign/Sequencing/ScreenFadeBlockTrack.cs` | 142 | Campaign.Sequencing | class ScreenFadeBlockTrack : BlockTrack<br>class CreateTarget : ICutsceneTrackTarget<br>class HoldBlack : Block<br>class FadeIn : Block<br>class FadeOut : Block<br>class SnapInImmediate : Block |  |  |
| `Campaign/Sequencing/ScreenTextBlockTrack.cs` | 231 | Campaign.Sequencing | class ScreenTextBlockTrack : BlockTrack<br>class CreateTarget : ICutsceneTrackTarget<br>class ScreenText : Block |  | XML |
| `Campaign/Sequencing/ScreenTextLocation.cs` | 11 | Campaign.Sequencing | enum ScreenTextLocation |  |  |
| `Campaign/Sequencing/ScriptTrack.cs` | 196 | Campaign.Sequencing | class ScriptTrack : Track<br>class CreateTarget : ICutsceneTrackTarget<br>class Keyframe : TrackElement, IXmlDocSerializable |  | XML |
| `Campaign/Sequencing/SFXBlockTrack.cs` | 230 | Campaign.Sequencing | class SFXBlockTrack : BlockTrack<br>class CreateTarget : ICutsceneTrackTarget<br>class SFXClipBlock : PositionalBlock, IPreloadableAsset |  | XML, Addressables |
| `Campaign/Sequencing/ShipTrack.cs` | 108 | Campaign.Sequencing | class ShipTrack : TransformTrack |  | XML |
| `Campaign/Sequencing/Track.cs` | 180 | Campaign.Sequencing | class Track : IXmlDocSerializable, IEditableRoomElement, IEditableCampaignElement, IModDependencySource<br>class TrackElement |  | XML |
| `Campaign/Sequencing/TransformPathGizmo.cs` | 183 | Campaign.Sequencing | class TransformPathGizmo<br>struct GizmoPositionSegment<br>struct GizmoRotationSegment |  |  |
| `Campaign/Sequencing/TransformTrack.cs` | 183 | Campaign.Sequencing | class TransformTrack : KeyframedTrack, TriadWidget.ITransformable |  | XML |
| `Campaign/Stations/AlphaClippedDockingCollar.cs` | 166 | Campaign.Stations | class AlphaClippedDockingCollar : BaseDockingCollar<br>class CoroutineMoveCollar : SaveableCoroutine | Awake | XML, prefab |
| `Campaign/Stations/AnimatorDockingCollar.cs` | 122 | Campaign.Stations | class AnimatorDockingCollar : BaseDockingCollar |  | XML |
| `Campaign/Stations/BaseDockingCollar.cs` | 22 | Campaign.Stations | class BaseDockingCollar : MonoBehaviour |  | XML |
| `Campaign/Stations/ModularStationPiece.cs` | 57 | Campaign.Stations | class ModularStationPiece : MonoBehaviour, ISpawnedFromEntity |  |  |
| `Campaign/Stations/SpaceStation.cs` | 1247 | Campaign.Stations | class SpaceStation : BaseIdentitiedEntityNetworkScript, IInternalParamsEntity, IInspectable, IBoardPiece, ISelectable, IOwned, IBulkSaveComponent, IPlatform, ... | Awake, OnStopClient | NetworkBehaviour, SyncVar, NetworkServer, save-state, XML, prefab |
| `Campaign/Stations/SpaceStationComponent.cs` | 474 | Campaign.Stations | class SpaceStationComponent : BaseIdentitiedEntityNetworkScript, IBoardPiece, IInternalParamsEntity, IInspectable, ICampaignEditorGizmoDrawer, IBulkSaveComponent, IInt... | Awake, Start | NetworkBehaviour, SyncVar, NetworkServer, save-state, XML, prefab |
| `Campaign/Stations/StationBoardingPoint.cs` | 221 | Campaign.Stations | class StationBoardingPoint : StationExternalDockingPoint<br>struct GrapplePoint |  | NetworkBehaviour, ClientRpc, NetworkClient, prefab |
| `Campaign/Stations/StationCraftStorage.cs` | 438 | Campaign.Stations | class StationCraftStorage : NetworkBehaviour, IInstanceStorageContainer, IStorageContainer, IEditableStorageContainer, ICraftHangar<br>struct SavedCraftQuantity<br>class StationHangarReference : CraftHangarReference | Awake | NetworkBehaviour, NetworkServer, save-state, XML |
| `Campaign/Stations/StationDockingBay.cs` | 361 | Campaign.Stations | class StationDockingBay : StationDockingPoint |  | NetworkBehaviour, SyncVar, NetworkServer, XML |
| `Campaign/Stations/StationDockingCollar.cs` | 215 | Campaign.Stations | class StationDockingCollar : StationExternalDockingPoint |  | NetworkBehaviour, ClientRpc, NetworkClient, XML |
| `Campaign/Stations/StationDockingPoint.cs` | 588 | Campaign.Stations | class StationDockingPoint : SpaceStationComponent, IDockingOrderTarget, IDockingPoint, IModifierSource |  | NetworkBehaviour, SyncVar, NetworkServer, XML |
| `Campaign/Stations/StationExternalDockingPoint.cs` | 354 | Campaign.Stations | class StationExternalDockingPoint : StationDockingPoint |  | XML |
| `Campaign/Stations/StationSocket.cs` | 309 | Campaign.Stations | class StationSocket : BaseIdentitiedEntityNetworkScript, IHullSocket, IBoardPiece, IInternalParamsEntity, IInspectable, ICampaignEditorGizmoDrawer | Start | XML |
| `Campaign/Stations/StationStorage.cs` | 203 | Campaign.Stations | class StationStorage : NetworkBehaviour, ISimpleStorageContainer, IStorageContainer, IEditableStorageContainer<br>struct SavedResourceQuantity | Awake | NetworkBehaviour, save-state, XML |
| `Campaign/Stations/TargetableSpaceStationComponent.cs` | 340 | Campaign.Stations | class TargetableSpaceStationComponent : SpaceStationComponent, ITargetIntel, IDamageable, ISubDamageable | Awake, OnStartServer | NetworkBehaviour, SyncVar, ClientRpc, NetworkServer, NetworkClient, XML, prefab |
| `CampaignEditor/CampaignEditorController.cs` | 677 | CampaignEditor | class CampaignEditorController : SingletonMonobehaviour<CampaignEditorController>, IWidgetGameManager | Awake, Start | prefab |
| `CampaignEditor/CampaignEditorSubmode.cs` | 68 | CampaignEditor | class CampaignEditorSubmode : ImmediateModeShapeDrawer |  | prefab |
| `CampaignEditor/CampaignWarning.cs` | 46 | CampaignEditor | class CampaignWarning |  |  |
| `CampaignEditor/CampaignWarningSorter.cs` | 17 | CampaignEditor | class CampaignWarningSorter : IComparer<CampaignWarning> |  |  |
| `CampaignEditor/DevPhaseVisualizationLayer.cs` | 38 | CampaignEditor | class DevPhaseVisualizationLayer : FilledTileVisualizationLayer |  |  |
| `CampaignEditor/EditCampaignFleetData.cs` | 21 | CampaignEditor | class EditCampaignFleetData : ReturnToCampaignEditorData |  |  |
| `CampaignEditor/EditorActions/ConnectLocationsEditorAction.cs` | 78 | CampaignEditor.EditorActions | class ConnectLocationsEditorAction : SelectTileEditorAction | Start |  |
| `CampaignEditor/EditorActions/MoveRoomEditorAction.cs` | 52 | CampaignEditor.EditorActions | class MoveRoomEditorAction : SelectTileEditorAction | Start |  |
| `CampaignEditor/EditorActions/MultiMoveRoomEditorAction.cs` | 103 | CampaignEditor.EditorActions | class MultiMoveRoomEditorAction : OverworldEditorAction | Start |  |
| `CampaignEditor/EditorActions/OverworldEditorAction.cs` | 54 | CampaignEditor.EditorActions | class OverworldEditorAction | Start |  |
| `CampaignEditor/EditorActions/PaintTilesEditorAction.cs` | 83 | CampaignEditor.EditorActions | class PaintTilesEditorAction : OverworldEditorAction | Start |  |
| `CampaignEditor/EditorActions/SelectTileEditorAction.cs` | 46 | CampaignEditor.EditorActions | class SelectTileEditorAction : OverworldEditorAction |  |  |
| `CampaignEditor/EditorActions/SelectTileSetEditorAction.cs` | 96 | CampaignEditor.EditorActions | class SelectTileSetEditorAction : SelectTileEditorAction |  |  |
| `CampaignEditor/EditorVisualizationLayer.cs` | 74 | CampaignEditor | class EditorVisualizationLayer : ImmediateModeShapeDrawer | Awake, OnDestroy | prefab |
| `CampaignEditor/Entities/EditorEntity.cs` | 419 | CampaignEditor.Entities | class EditorEntity : MonoBehaviour, IEditableRoomElement, IEditableCampaignElement, ICustomInspectable, IInspectable, IHierarchyElement, TriadWidget.ITransfor... | OnDestroy, Update | prefab |
| `CampaignEditor/FilledTileVisualizationLayer.cs` | 59 | CampaignEditor | class FilledTileVisualizationLayer : EditorVisualizationLayer |  | prefab |
| `CampaignEditor/HexMapEditorLayer.cs` | 335 | CampaignEditor | class HexMapEditorLayer : MonoBehaviour<br>class CheckpointFlag : MonoBehaviour | Awake, OnDestroy, OnEnable | prefab |
| `CampaignEditor/ICampaignWarningSource.cs` | 9 | CampaignEditor | interface ICampaignWarningSource |  |  |
| `CampaignEditor/IEditableCampaignElement.cs` | 20 | CampaignEditor | interface IEditableCampaignElement |  |  |
| `CampaignEditor/IEditableOverworldElement.cs` | 13 | CampaignEditor | interface IEditableOverworldElement : IEditableCampaignElement |  |  |
| `CampaignEditor/IEditableRoomElement.cs` | 13 | CampaignEditor | interface IEditableRoomElement : IEditableCampaignElement |  |  |
| `CampaignEditor/Inspector/AddressableAssetSelectionSet.cs` | 75 | CampaignEditor.Inspector | class AddressableAssetSelectionSet : IInspectableDropdownParam, IXmlDocSerializable, IWarningGeneratingDropdownParam |  | XML, Addressables |
| `CampaignEditor/Inspector/BaseKeyedSelectionSet.cs` | 165 | CampaignEditor.Inspector | class BaseKeyedSelectionSet : IInspectableDropdownParam, IXmlDocSerializable, IWarningGeneratingDropdownParam |  | XML |
| `CampaignEditor/Inspector/CampaignGUIDObjectSelectionSet.cs` | 134 | CampaignEditor.Inspector | class CampaignGUIDObjectSelectionSet : IInspectableDropdownParam, IXmlDocSerializable, IWarningGeneratingDropdownParam where T : class, ICampaignGUIDObject<br>class NoComparer : IComparer<T> |  | XML |
| `CampaignEditor/Inspector/CampaignIDObjectSelectionSet.cs` | 134 | CampaignEditor.Inspector | class CampaignIDObjectSelectionSet : IInspectableDropdownParam, IXmlDocSerializable, IWarningGeneratingDropdownParam where T : class, ICampaignIDObject<br>class NoComparer : IComparer<T> |  | XML |
| `CampaignEditor/Inspector/CraftLoadoutSelectionSet.cs` | 20 | CampaignEditor.Inspector | class CraftLoadoutSelectionSet : CustomStringKeyedSelectionSet<SerializedCraftLoadout> |  |  |
| `CampaignEditor/Inspector/CustomParamInspectorAttribute.cs` | 15 | CampaignEditor.Inspector | class CustomParamInspectorAttribute : Attribute |  |  |
| `CampaignEditor/Inspector/CustomStringKeyedSelectionSet.cs` | 37 | CampaignEditor.Inspector | class CustomStringKeyedSelectionSet : BaseKeyedSelectionSet<string, T> |  |  |
| `CampaignEditor/Inspector/DialogClipSelectionSet.cs` | 66 | CampaignEditor.Inspector | class DialogClipSelectionSet : AddressableAssetSelectionSet<AudioClip>, ICustomInspectable, IInspectable |  | Addressables |
| `CampaignEditor/Inspector/EditorInspector.cs` | 1320 | CampaignEditor.Inspector | class EditorInspector : MonoBehaviour<br>class ParameterField : MonoBehaviour<br>class StringParameterField : ParameterField<br>class Vector2ParameterField : ParameterField<br>class Vector3ParameterField : ParameterField<br>class HexCoordinateParameterField : ParameterField<br>class IntegerParameterField : ParameterField<br>class UnsignedIntegerParameterField : ParameterField<br>class FloatParameterField : ParameterField<br>class DoubleParameterField : ParameterField<br>class BoolParameterField : ParameterField<br>class FixedTextField : ParameterField<br>class ColorParameterField : ParameterField<br>class EnumParameterField : ParameterField<br>class AssetRefParameterField : ParameterField<br>class SubParameterField : ParameterField<br>class ButtonParameterField : ParameterField<br>class BoundsParameterField : ParameterField<br>class DifficultyMaskParameterField : ParameterField<br>class DropdownParameterField : ParameterField | Awake, OnDestroy | Addressables, prefab |
| `CampaignEditor/Inspector/EntitySelectionSet.cs` | 151 | CampaignEditor.Inspector | class EntitySelectionSet : IInspectableDropdownParam, IXmlDocSerializable, IWarningGeneratingDropdownParam where TEnt : Entity |  | XML |
| `CampaignEditor/Inspector/HullPartSelectionSet.cs` | 107 | CampaignEditor.Inspector | class HullPartSelectionSet : IInspectableDropdownParam, IXmlDocSerializable, IWarningGeneratingDropdownParam |  | XML |
| `CampaignEditor/Inspector/ICustomInspectable.cs` | 11 | CampaignEditor.Inspector | interface ICustomInspectable : IInspectable |  |  |
| `CampaignEditor/Inspector/IdentifiedEntityScriptSelectionSet.cs` | 139 | CampaignEditor.Inspector | class IdentifiedEntityScriptSelectionSet : IInspectableDropdownParam, IXmlDocSerializable, IWarningGeneratingDropdownParam where TScript : MonoBehaviour, IIdentifiedEntityScript |  | XML |
| `CampaignEditor/Inspector/IInspectable.cs` | 7 | CampaignEditor.Inspector | interface IInspectable |  |  |
| `CampaignEditor/Inspector/IInspectableDropdownParam.cs` | 19 | CampaignEditor.Inspector | interface IInspectableDropdownParam |  |  |
| `CampaignEditor/Inspector/InspectableAssetRefAttribute.cs` | 13 | CampaignEditor.Inspector | class InspectableAssetRefAttribute : InspectableParamAttribute |  |  |
| `CampaignEditor/Inspector/InspectableAttributeHelper.cs` | 78 | CampaignEditor.Inspector | class InspectableAttributeHelper |  |  |
| `CampaignEditor/Inspector/InspectableButtonParamAttribute.cs` | 20 | CampaignEditor.Inspector | class InspectableButtonParamAttribute : InspectableParamAttribute |  |  |
| `CampaignEditor/Inspector/InspectableParamAttribute.cs` | 31 | CampaignEditor.Inspector | class InspectableParamAttribute : Attribute |  |  |
| `CampaignEditor/Inspector/InspectableParameter.cs` | 31 | CampaignEditor.Inspector | struct InspectableParameter |  |  |
| `CampaignEditor/Inspector/InspectableValueAttribute.cs` | 13 | CampaignEditor.Inspector | class InspectableValueAttribute : InspectableParamAttribute |  |  |
| `CampaignEditor/Inspector/InspectorStaticSelectionSet.cs` | 78 | CampaignEditor.Inspector | class InspectorStaticSelectionSet : IInspectableDropdownParam, IXmlDocSerializable |  | XML |
| `CampaignEditor/Inspector/IWarningGeneratingDropdownParam.cs` | 7 | CampaignEditor.Inspector | interface IWarningGeneratingDropdownParam |  |  |
| `CampaignEditor/Inspector/SaveKeyedSelectionSet.cs` | 43 | CampaignEditor.Inspector | class SaveKeyedSelectionSet : BaseKeyedSelectionSet<string, T> where T : class, ISaveKeyed |  |  |
| `CampaignEditor/Inspector/ShipReferenceSelectionSet.cs` | 165 | CampaignEditor.Inspector | class ShipReferenceSelectionSet : IInspectableDropdownParam, IXmlDocSerializable, IWarningGeneratingDropdownParam |  | XML |
| `CampaignEditor/Inspector/WeaponAmmoSelectionSet.cs` | 78 | CampaignEditor.Inspector | class WeaponAmmoSelectionSet : IInspectableDropdownParam, IXmlDocSerializable, IWarningGeneratingDropdownParam |  | XML |
| `CampaignEditor/Inspector/WeaponGroupSelectionSet.cs` | 90 | CampaignEditor.Inspector | class WeaponGroupSelectionSet : IInspectableDropdownParam, IXmlDocSerializable, IWarningGeneratingDropdownParam |  | XML |
| `CampaignEditor/ObjectiveVisualizer.cs` | 18 | CampaignEditor | class ObjectiveVisualizer : TileFlagVisualizerLayer<MapObjectiveFlag> |  | prefab |
| `CampaignEditor/OverworldEditorSubmode.cs` | 323 | CampaignEditor | class OverworldEditorSubmode : CampaignEditorSubmode | Awake, Update | Addressables, prefab |
| `CampaignEditor/ReplenishmentVisualizationLayer.cs` | 18 | CampaignEditor | class ReplenishmentVisualizationLayer : TileFlagVisualizerLayer<MapReplenishmentFlag> |  | prefab |
| `CampaignEditor/ReturnToCampaignEditorData.cs` | 9 | CampaignEditor | class ReturnToCampaignEditorData : CrossScenePipe.CrossSceneData |  |  |
| `CampaignEditor/RoomEditorSubmode.cs` | 988 | CampaignEditor | class RoomEditorSubmode : CampaignEditorSubmode, IHorizontalPlane, IEntitySpawnResources | Awake, OnDestroy, Update | Addressables, prefab |
| `CampaignEditor/RoomTagVisualizationLayer.cs` | 46 | CampaignEditor | class RoomTagVisualizationLayer : FilledTileVisualizationLayer |  |  |
| `CampaignEditor/ShipEntityVisualizer.cs` | 153 | CampaignEditor | class ShipEntityVisualizer : TileFlagVisualizerLayer<ShipEntityVisualizer.ShipCountFlag><br>class ShipCountFlag : MonoBehaviour | Awake, OnDestroy | prefab |
| `CampaignEditor/TileFlagVisualizerLayer.cs` | 100 | CampaignEditor | class TileFlagVisualizerLayer : EditorVisualizationLayer where TFlag : MonoBehaviour | Awake, OnDestroy | prefab |
| `CampaignEditor/TravelTimeVisualizer.cs` | 180 | CampaignEditor | class TravelTimeVisualizer : EditorVisualizationLayer<br>class RouteFlag | Awake, OnDestroy | prefab |
| `CampaignEditor/UI/CampaignDebugOverlay.cs` | 60 | CampaignEditor.UI | class CampaignDebugOverlay : MonoBehaviour |  |  |
| `CampaignEditor/UI/CampaignEditorGizmoUtility.cs` | 47 | CampaignEditor.UI | class CampaignEditorGizmoUtility : ImmediateModeShapeDrawer |  |  |
| `CampaignEditor/UI/CampaignEditorMainUI.cs` | 385 | CampaignEditor.UI | class CampaignEditorMainUI : BaseMenu | Awake, OnEnable, Update | prefab |
| `CampaignEditor/UI/CampaignEditorOverworldUI.cs` | 118 | CampaignEditor.UI | class CampaignEditorOverworldUI : MonoBehaviour, ICampaignEditorUI | Update | prefab |
| `CampaignEditor/UI/CampaignEditorRoomUI.cs` | 120 | CampaignEditor.UI | class CampaignEditorRoomUI : MonoBehaviour |  | prefab |
| `CampaignEditor/UI/EntityList.cs` | 188 | CampaignEditor.UI | class EntityList : MonoBehaviour<br>class EntityGroup : MonoBehaviour<br>class DifficultyTableRow : MonoBehaviour<br>struct DifficultyRow | Awake | prefab |
| `CampaignEditor/UI/ICampaignEditorGizmoDrawer.cs` | 11 | CampaignEditor.UI | interface ICampaignEditorGizmoDrawer |  |  |
| `CampaignEditor/UI/ICampaignEditorUI.cs` | 9 | CampaignEditor.UI | interface ICampaignEditorUI |  |  |
| `CampaignEditor/UI/ModalBalancingTools.cs` | 742 | CampaignEditor.UI | class ModalBalancingTools : BaseMenu<br>enum GraphType<br>class SnapshotListItem : MonoBehaviour<br>class ProcessedSnapshot<br>class ShipHistory | Awake | prefab |
| `CampaignEditor/UI/ModalQuickTestSetup.cs` | 492 | CampaignEditor.UI | class ModalQuickTestSetup : BaseMenu<br>class VariableItem : MonoBehaviour<br>class ShipGroup : MonoBehaviour<br>class ShipItem : MonoBehaviour | Awake, Start | prefab |
| `CampaignEditor/UI/ModalSpawnShip.cs` | 156 | CampaignEditor.UI | class ModalSpawnShip : BaseMenu |  |  |
| `CampaignEditor/UI/Sequencing/AdjustableDurationTimelineElement.cs` | 54 | CampaignEditor.UI.Sequencing | class AdjustableDurationTimelineElement : DurationTimelineElement |  |  |
| `CampaignEditor/UI/Sequencing/BlockElement.cs` | 272 | CampaignEditor.UI.Sequencing | class BlockElement : AdjustableDurationTimelineElement<br>class InOutHandle : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IEndDragHandler, IDragHandler | Awake, OnDestroy | prefab |
| `CampaignEditor/UI/Sequencing/BlockTrackTimeline.cs` | 79 | CampaignEditor.UI.Sequencing | class BlockTrackTimeline : TrackTimeline<BlockTrack, SimpleTimelineLeader> | Awake | prefab |
| `CampaignEditor/UI/Sequencing/DialogEditorPulldown.cs` | 206 | CampaignEditor.UI.Sequencing | class DialogEditorPulldown : Pulldown | Awake | Addressables |
| `CampaignEditor/UI/Sequencing/DialogElement.cs` | 132 | CampaignEditor.UI.Sequencing | class DialogElement : DurationTimelineElement | Awake, OnDestroy |  |
| `CampaignEditor/UI/Sequencing/DialogTrackTimeline.cs` | 75 | CampaignEditor.UI.Sequencing | class DialogTrackTimeline : TrackTimeline<DialogTrack, SimpleTimelineLeader> | Awake | prefab |
| `CampaignEditor/UI/Sequencing/DirectionKeyframeEditor.cs` | 85 | CampaignEditor.UI.Sequencing | class DirectionKeyframeEditor : ISequenceElementEditor, TriadWidget.ITransformable |  |  |
| `CampaignEditor/UI/Sequencing/DopeSheetEditor.cs` | 649 | CampaignEditor.UI.Sequencing | class DopeSheetEditor : MonoBehaviour<br>class TimeHeader : MonoBehaviour<br>struct TimelineType | Awake, Update | prefab |
| `CampaignEditor/UI/Sequencing/DurationTimelineElement.cs` | 16 | CampaignEditor.UI.Sequencing | class DurationTimelineElement : TimelineElement |  |  |
| `CampaignEditor/UI/Sequencing/ISequenceElementEditor.cs` | 9 | CampaignEditor.UI.Sequencing | interface ISequenceElementEditor |  |  |
| `CampaignEditor/UI/Sequencing/ITrackTimeline.cs` | 17 | CampaignEditor.UI.Sequencing | interface ITrackTimeline : IClearable |  | prefab |
| `CampaignEditor/UI/Sequencing/KeyframedTrackTimeline.cs` | 194 | CampaignEditor.UI.Sequencing | class KeyframedTrackTimeline : TrackTimeline<KeyframedTrack, MultiChannelTimelineLeader><br>class TrackChannel : MonoBehaviour | Awake, OnDestroy | prefab |
| `CampaignEditor/UI/Sequencing/KeyframeEntityScopeElement.cs` | 96 | CampaignEditor.UI.Sequencing | class KeyframeEntityScopeElement : AdjustableDurationTimelineElement | OnDestroy |  |
| `CampaignEditor/UI/Sequencing/MultiChannelTimelineLeader.cs` | 44 | CampaignEditor.UI.Sequencing | class MultiChannelTimelineLeader : SimpleTimelineLeader | Awake | prefab |
| `CampaignEditor/UI/Sequencing/ParamKeyframeElement.cs` | 81 | CampaignEditor.UI.Sequencing | class ParamKeyframeElement : TimelineElement | OnDestroy |  |
| `CampaignEditor/UI/Sequencing/PositionKeyframeEditor.cs` | 104 | CampaignEditor.UI.Sequencing | class PositionKeyframeEditor : ISequenceElementEditor<br>class CenterPoint : TriadWidget.ITransformable |  |  |
| `CampaignEditor/UI/Sequencing/ScriptKeyframeElement.cs` | 53 | CampaignEditor.UI.Sequencing | class ScriptKeyframeElement : TimelineElement | OnDestroy |  |
| `CampaignEditor/UI/Sequencing/ScriptTrackTimeline.cs` | 75 | CampaignEditor.UI.Sequencing | class ScriptTrackTimeline : TrackTimeline<ScriptTrack, SimpleTimelineLeader> | Awake | prefab |
| `CampaignEditor/UI/Sequencing/SimpleTimelineLeader.cs` | 65 | CampaignEditor.UI.Sequencing | class SimpleTimelineLeader : MonoBehaviour | OnDestroy |  |
| `CampaignEditor/UI/Sequencing/TimelineElement.cs` | 122 | CampaignEditor.UI.Sequencing | class TimelineElement : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler, IDragHandler | Awake |  |
| `CampaignEditor/UI/Sequencing/TimelineScrubHandle.cs` | 65 | CampaignEditor.UI.Sequencing | class TimelineScrubHandle : Selectable, IPointerDownHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler, IDragHandler |  |  |
| `CampaignEditor/UI/Sequencing/TrackTimeline.cs` | 103 | CampaignEditor.UI.Sequencing | class TrackTimeline : MonoBehaviour, ITrackTimeline, IClearable, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler, IPointerU... | Awake, OnDestroy, Update |  |
| `CampaignEditor/UI/ShipStatusDetailPartEditor.cs` | 51 | CampaignEditor.UI | class ShipStatusDetailPartEditor : ShipStatusDetailPart, IScrollHandler, IEventSystemHandler |  |  |
| `CampaignEditor/UI/Windows/CutsceneViewfinderWindow.cs` | 17 | CampaignEditor.UI.Windows | class CutsceneViewfinderWindow : WindowContent |  |  |
| `CampaignEditor/UI/Windows/EditCampaignActWindow.cs` | 216 | CampaignEditor.UI.Windows | class EditCampaignActWindow : OverworldEditorWindowContent<br>class LocationListItem : MonoBehaviour | Awake, OnDestroy | prefab |
| `CampaignEditor/UI/Windows/EditCampaignObjectivesWindow.cs` | 206 | CampaignEditor.UI.Windows | class EditCampaignObjectivesWindow : WindowContent<br>class ObjectiveItem : MonoBehaviour | Awake, OnDestroy | prefab |
| `CampaignEditor/UI/Windows/EditCampaignVariablesWindow.cs` | 226 | CampaignEditor.UI.Windows | class EditCampaignVariablesWindow : WindowContent<br>class VariableItem : MonoBehaviour | Awake, OnDestroy, Start | prefab |
| `CampaignEditor/UI/Windows/EditCastWindow.cs` | 209 | CampaignEditor.UI.Windows | class EditCastWindow : OverworldEditorWindowContent<br>class CharacterItem : MonoBehaviour | Awake, OnDestroy, Start | prefab |
| `CampaignEditor/UI/Windows/EditCharacterAppearanceWindow.cs` | 332 | CampaignEditor.UI.Windows | class EditCharacterAppearanceWindow : WindowContent<br>class Option | Awake, OnDestroy | Addressables, prefab |
| `CampaignEditor/UI/Windows/EditFactionListWindow.cs` | 282 | CampaignEditor.UI.Windows | class EditFactionListWindow : OverworldEditorWindowContent<br>class FactionItem : MonoBehaviour | Awake, OnDestroy, Start | prefab |
| `CampaignEditor/UI/Windows/EditorStorageWindow.cs` | 168 | CampaignEditor.UI.Windows | class EditorStorageWindow : WindowContent<br>class StoredItem : MonoBehaviour | Awake | prefab |
| `CampaignEditor/UI/Windows/EditPursuitRulesWindow.cs` | 165 | CampaignEditor.UI.Windows | class EditPursuitRulesWindow : WindowContent<br>class ShipItem : MonoBehaviour | Awake | prefab |
| `CampaignEditor/UI/Windows/EditRoomWindow.cs` | 136 | CampaignEditor.UI.Windows | class EditRoomWindow : OverworldEditorWindowContent | OnDestroy |  |
| `CampaignEditor/UI/Windows/EditShipExpendedAmmunitionWindow.cs` | 225 | CampaignEditor.UI.Windows | class EditShipExpendedAmmunitionWindow : WindowContent<br>class MagazineItem : MonoBehaviour<br>class AmmoItem : MonoBehaviour | Awake | save-state, prefab |
| `CampaignEditor/UI/Windows/EditShipFuelWindow.cs` | 81 | CampaignEditor.UI.Windows | class EditShipFuelWindow : WindowContent |  |  |
| `CampaignEditor/UI/Windows/GeneralCampaignDetailsWindow.cs` | 228 | CampaignEditor.UI.Windows | class GeneralCampaignDetailsWindow : OverworldEditorWindowContent | OnDestroy | Addressables, prefab |
| `CampaignEditor/UI/Windows/OverworldEditorWindowContent.cs` | 19 | CampaignEditor.UI.Windows | class OverworldEditorWindowContent : WindowContent |  |  |
| `CampaignEditor/UI/Windows/OverworldMapTracksWindow.cs` | 223 | CampaignEditor.UI.Windows | class OverworldMapTracksWindow : OverworldEditorWindowContent<br>class TrackItem : MonoBehaviour, IClearable | Awake, OnDestroy | prefab |
| `CampaignEditor/UI/Windows/RoomMapRotationWindow.cs` | 112 | CampaignEditor.UI.Windows | class RoomMapRotationWindow : WindowContent<br>struct OriginalEntityTransform |  |  |
| `CampaignEditor/UI/Windows/ShipArmorDamageWindow.cs` | 204 | CampaignEditor.UI.Windows | class ShipArmorDamageWindow : LineDrawingWindowContent | Awake, Update | prefab |
| `CampaignEditor/UI/Windows/ShipDesignListWindow.cs` | 681 | CampaignEditor.UI.Windows | class ShipDesignListWindow : OverworldEditorWindowContent<br>class CategoryGroup : MonoBehaviour, IComparable<CategoryGroup><br>class ShipDesignItem : MonoBehaviour, IComparable<ShipDesignItem><br>class AssociatedTemplateItem : MonoBehaviour | Awake, OnDestroy, Start | prefab |
| `CampaignEditor/UI/Windows/ShipInitialComponentDamageWindow.cs` | 82 | CampaignEditor.UI.Windows | class ShipInitialComponentDamageWindow : WindowContent |  |  |
| `CampaignEditor/UI/Windows/SystemUnderlayWindow.cs` | 217 | CampaignEditor.UI.Windows | class SystemUnderlayWindow : OverworldEditorWindowContent | Awake, OnDestroy | XML |
| `CampaignEditor/WarningType.cs` | 9 | CampaignEditor | enum WarningType |  |  |
| `Codex/CodexEntry.cs` | 11 | Codex | class CodexEntry : MonoBehaviour |  |  |
| `Codex/IndexListGroup.cs` | 53 | Codex | class IndexListGroup : MonoBehaviour |  |  |
| `Codex/IndexListItem.cs` | 36 | Codex | class IndexListItem : MonoBehaviour |  | prefab |
| `Codex/ModalCodex.cs` | 131 | Codex | class ModalCodex : BaseMenu | Awake | prefab |
| `DebugExtension.cs` | 643 |  | class DebugExtension |  |  |
| `Effects/CustomLensFlare.cs` | 117 | Effects | class CustomLensFlare : MonoBehaviour<br>struct FlareElement | LateUpdate, Start | prefab |
| `Effects/FollowingModularEffect.cs` | 40 | Effects | class FollowingModularEffect : ModularEffect | FixedUpdate, OnRepooled, OnUnpooled | pooling |
| `Effects/IEffectModule.cs` | 11 | Effects | interface IEffectModule |  |  |
| `Effects/ISpawnedEffect.cs` | 7 | Effects | interface ISpawnedEffect |  |  |
| `Effects/ModularEffect.cs` | 62 | Effects | class ModularEffect : Poolable, ISpawnedEffect | Awake, OnRepooled, OnUnpooled | pooling |
| `Effects/Modules/AnimationEffectModule.cs` | 24 | Effects.Modules | class AnimationEffectModule : MonoBehaviour, IEffectModule |  |  |
| `Effects/Modules/ChessboardDistortionEffectModule.cs` | 24 | Effects.Modules | class ChessboardDistortionEffectModule : MonoBehaviour, IEffectModule |  |  |
| `Effects/Modules/LightEffectModule.cs` | 48 | Effects.Modules | class LightEffectModule : MonoBehaviour, IEffectModule | Awake, Update |  |
| `Effects/Modules/ParticleEffectModule.cs` | 29 | Effects.Modules | class ParticleEffectModule : MonoBehaviour, IEffectModule |  |  |
| `Effects/Modules/SoundEffectModule.cs` | 40 | Effects.Modules | class SoundEffectModule : MonoBehaviour, IEffectModule | Awake |  |
| `Effects/Modules/SpawnEWarEffectModule.cs` | 91 | Effects.Modules | class SpawnEWarEffectModule : MonoBehaviour, IEffectModule, ITuneableEWar |  | pooling, prefab |
| `Effects/NetworkedModularEffect.cs` | 62 | Effects | class NetworkedModularEffect : NetworkPoolable, ISpawnedEffect | Awake, OnRepooled, OnUnpooled | pooling |
| `Effects/ShortDurationModularEffect.cs` | 16 | Effects | class ShortDurationModularEffect : ModularEffect | OnUnpooled | pooling |
| `Factions/FactionDescription.cs` | 99 | Factions | class FactionDescription : ScriptableObject, IBundleKeyed, ISaveKeyed, IModSource |  |  |
| `FleetEditor/Clipboard/FleetCompositionClipboardItem.cs` | 11 | FleetEditor.Clipboard | class FleetCompositionClipboardItem |  |  |
| `FleetEditor/Clipboard/FullFittingClipboardItem.cs` | 55 | FleetEditor.Clipboard | class FullFittingClipboardItem : FleetCompositionClipboardItem |  |  |
| `FleetEditor/Clipboard/IFleetEditorClipboard.cs` | 13 | FleetEditor.Clipboard | interface IFleetEditorClipboard |  |  |
| `FleetEditor/Clipboard/SocketClipboardItem.cs` | 84 | FleetEditor.Clipboard | class SocketClipboardItem : FleetCompositionClipboardItem |  |  |
| `FleetEditor/ComponentPalette.cs` | 168 | FleetEditor | class ComponentPalette<br>class ComponentGroup |  | prefab |
| `FleetEditor/CraftDesignWarning.cs` | 55 | FleetEditor | class CraftDesignWarning : DesignWarning |  |  |
| `FleetEditor/CraftEditor/BaseLoadoutSlotItem.cs` | 69 | FleetEditor.CraftEditor | class BaseLoadoutSlotItem : MonoBehaviour, ILoadoutSlotItem where TRow : LoadoutMatrixRow | Awake | prefab |
| `FleetEditor/CraftEditor/CraftConfigurationEditor.cs` | 118 | FleetEditor.CraftEditor | class CraftConfigurationEditor : MonoBehaviour, ISelectList |  | prefab |
| `FleetEditor/CraftEditor/CraftListItem.cs` | 89 | FleetEditor.CraftEditor | class CraftListItem : SelectableListItem, ISettable<Spacecraft> | OnDestroy | prefab |
| `FleetEditor/CraftEditor/CraftLoadoutRow.cs` | 51 | FleetEditor.CraftEditor | class CraftLoadoutRow : MonoBehaviour |  | prefab |
| `FleetEditor/CraftEditor/CraftSchematicDisplay.cs` | 128 | FleetEditor.CraftEditor | class CraftSchematicDisplay : MonoBehaviour |  | prefab |
| `FleetEditor/CraftEditor/CraftStaticComponentItem.cs` | 105 | FleetEditor.CraftEditor | class CraftStaticComponentItem : SelectableListItem | Awake | prefab |
| `FleetEditor/CraftEditor/CraftStaticSocketItem.cs` | 158 | FleetEditor.CraftEditor | class CraftStaticSocketItem : SelectableListItem, ISelectList, IClearable, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler | OnDestroy | prefab |
| `FleetEditor/CraftEditor/CraftStatsList.cs` | 64 | FleetEditor.CraftEditor | class CraftStatsList : MonoBehaviour |  | prefab |
| `FleetEditor/CraftEditor/CraftTemplateListItem.cs` | 70 | FleetEditor.CraftEditor | class CraftTemplateListItem : SelectableListItem, ISettable<SpacecraftTemplate> |  | prefab |
| `FleetEditor/CraftEditor/EditingCraftList.cs` | 240 | FleetEditor.CraftEditor | class EditingCraftList : EditorTemplateableObjectList<Spacecraft, SpacecraftTemplate, CraftListItem, CraftTemplateListItem> |  | prefab |
| `FleetEditor/CraftEditor/GenericLoadoutRow.cs` | 6 | FleetEditor.CraftEditor | class GenericLoadoutRow : CraftLoadoutRow |  |  |
| `FleetEditor/CraftEditor/GenericLoadoutSlotItem.cs` | 17 | FleetEditor.CraftEditor | class GenericLoadoutSlotItem : BaseLoadoutSlotItem<LoadoutMatrixRow> |  | prefab |
| `FleetEditor/CraftEditor/ILoadoutSlotItem.cs` | 9 | FleetEditor.CraftEditor | interface ILoadoutSlotItem |  |  |
| `FleetEditor/CraftEditor/LoadoutMatrixEditor.cs` | 446 | FleetEditor.CraftEditor | class LoadoutMatrixEditor : MonoBehaviour, ISelectList |  | prefab |
| `FleetEditor/CraftEditor/MissileLoadoutRow.cs` | 6 | FleetEditor.CraftEditor | class MissileLoadoutRow : CraftLoadoutRow |  |  |
| `FleetEditor/CraftEditor/MissileLoadoutSlotItem.cs` | 42 | FleetEditor.CraftEditor | class MissileLoadoutSlotItem : BaseLoadoutSlotItem<LoadoutMatrixMissile> |  | prefab |
| `FleetEditor/CraftEditor/ModalSpacecraftIdentity.cs` | 76 | FleetEditor.CraftEditor | class ModalSpacecraftIdentity : BaseMenu |  |  |
| `FleetEditor/CraftEditor/SpacecraftSchematic.cs` | 106 | FleetEditor.CraftEditor | class SpacecraftSchematic : MonoBehaviour<br>struct SocketHighlight | Awake, OnDestroy | prefab |
| `FleetEditor/CraftEditor/SpacecraftSchematicLayer.cs` | 8 | FleetEditor.CraftEditor | class SpacecraftSchematicLayer : MonoBehaviour |  |  |
| `FleetEditor/DesignTemplateDatabase.cs` | 87 | FleetEditor | class DesignTemplateDatabase |  |  |
| `FleetEditor/DesignWarning.cs` | 57 | FleetEditor | class DesignWarning |  |  |
| `FleetEditor/EditFleetData.cs` | 10 | FleetEditor | class EditFleetData : CrossScenePipe.CrossSceneData |  |  |
| `FleetEditor/EditLobbyFleetData.cs` | 15 | FleetEditor | class EditLobbyFleetData : CrossScenePipe.CrossSceneData |  |  |
| `FleetEditor/EditorShipController.cs` | 471 | FleetEditor | class EditorShipController : MonoBehaviour, IInitialFormationSaveData, IFormationUnit, IHandleTransformable, ISelectable, IOwned, IWidgetPositionSource | Awake, OnDestroy | prefab |
| `FleetEditor/EditorStatsGroup.cs` | 70 | FleetEditor | class EditorStatsGroup : MonoBehaviour |  | prefab |
| `FleetEditor/EditorSubmodeController.cs` | 234 | FleetEditor | class EditorSubmodeController : MonoBehaviour | OnDestroy, Start, Update | prefab |
| `FleetEditor/EditorTemplateableObjectList.cs` | 244 | FleetEditor | class EditorTemplateableObjectList : MonoBehaviour where TActive : class, IFleetTemplateableActive where TTemplate : class, IFleetTemplateableDesign where TActiveItem : Selec...<br>class SubList : ISelectList | Awake | prefab |
| `FleetEditor/EditorWarningDisplay.cs` | 185 | FleetEditor | class EditorWarningDisplay : MonoBehaviour<br>class WarningItem : MonoBehaviour | Awake, OnDestroy | prefab |
| `FleetEditor/FactionListItem.cs` | 46 | FleetEditor | class FactionListItem : SelectableListItem |  |  |
| `FleetEditor/FleetCompHistory/AddComponentEvent.cs` | 63 | FleetEditor.FleetCompHistory | class AddComponentEvent : FleetCompositionHistoryEvent |  |  |
| `FleetEditor/FleetCompHistory/ChangedShipIdentityEvent.cs` | 55 | FleetEditor.FleetCompHistory | class ChangedShipIdentityEvent : FleetCompositionHistoryEvent |  |  |
| `FleetEditor/FleetCompHistory/ChangeMagazineContentsEvent.cs` | 78 | FleetEditor.FleetCompHistory | class ChangeMagazineContentsEvent : FleetCompositionHistoryEvent |  |  |
| `FleetEditor/FleetCompHistory/ChangeSlotCraftHangarContentsEvent.cs` | 45 | FleetEditor.FleetCompHistory | class ChangeSlotCraftHangarContentsEvent : FleetCompositionHistoryEvent |  |  |
| `FleetEditor/FleetCompHistory/ChangeSlotMagazineAmmoTypeEvent.cs` | 50 | FleetEditor.FleetCompHistory | class ChangeSlotMagazineAmmoTypeEvent : FleetCompositionHistoryEvent |  |  |
| `FleetEditor/FleetCompHistory/ChangeSlotMagazineContentsEvent.cs` | 47 | FleetEditor.FleetCompHistory | class ChangeSlotMagazineContentsEvent : ChangeMagazineContentsEvent |  |  |
| `FleetEditor/FleetCompHistory/ChangeWeaponGroupEvent.cs` | 53 | FleetEditor.FleetCompHistory | class ChangeWeaponGroupEvent : FleetCompositionHistoryEvent |  |  |
| `FleetEditor/FleetCompHistory/FleetCompositionHistoryEvent.cs` | 8 | FleetEditor.FleetCompHistory | class FleetCompositionHistoryEvent : HistoryEvent |  |  |
| `FleetEditor/FleetCompHistory/PasteWholeFittingEvent.cs` | 32 | FleetEditor.FleetCompHistory | class PasteWholeFittingEvent : FleetCompositionHistoryEvent |  |  |
| `FleetEditor/FleetCompHistory/RemoveComponentEvent.cs` | 56 | FleetEditor.FleetCompHistory | class RemoveComponentEvent : FleetCompositionHistoryEvent |  |  |
| `FleetEditor/FleetCompositionSubmodeController.cs` | 563 | FleetEditor | class FleetCompositionSubmodeController : EditorSubmodeController, IFleetEditorClipboard | OnDestroy, Start, Update | prefab |
| `FleetEditor/FleetEditorController.cs` | 1023 | FleetEditor | class FleetEditorController : MonoBehaviour, IWidgetGameManager | Awake, OnDestroy, Start, Update | Command, prefab |
| `FleetEditor/FleetEditorMainUI.cs` | 37 | FleetEditor | class FleetEditorMainUI : BaseMenu | Awake |  |
| `FleetEditor/FleetListItem.cs` | 79 | FleetEditor | class FleetListItem : SelectableWorkshopListItem |  | prefab |
| `FleetEditor/FleetListPane.cs` | 93 | FleetEditor | class FleetListPane : MonoBehaviour | OnDestroy | prefab |
| `FleetEditor/FleetStatsPane.cs` | 41 | FleetEditor | class FleetStatsPane : MonoBehaviour |  | prefab |
| `FleetEditor/FleetVersioning.cs` | 189 | FleetEditor | class FleetVersioning<br>enum StepResult |  |  |
| `FleetEditor/FormationSettingsPane.cs` | 18 | FleetEditor | class FormationSettingsPane : MonoBehaviour |  |  |
| `FleetEditor/FormationSetupButtons.cs` | 65 | FleetEditor | class FormationSetupButtons : MonoBehaviour | LateUpdate | prefab |
| `FleetEditor/FormationShipItem.cs` | 114 | FleetEditor | class FormationShipItem : MonoBehaviour | OnDestroy | prefab |
| `FleetEditor/FormationShipList.cs` | 150 | FleetEditor | class FormationShipList : MonoBehaviour | Awake, OnDestroy | prefab |
| `FleetEditor/FormationSubmodeController.cs` | 266 | FleetEditor | class FormationSubmodeController : EditorSubmodeController | Start, Update | prefab |
| `FleetEditor/HangarSlotItem.cs` | 83 | FleetEditor | class HangarSlotItem : Poolable | OnRepooled | pooling |
| `FleetEditor/HangarSpinnerItem.cs` | 96 | FleetEditor | class HangarSpinnerItem : Poolable | Awake, OnDestroy, OnRepooled | pooling |
| `FleetEditor/HeaderItem.cs` | 21 | FleetEditor | class HeaderItem : SelectableListItem |  |  |
| `FleetEditor/HullListItem.cs` | 55 | FleetEditor | class HullListItem : SelectableListItem |  |  |
| `FleetEditor/IConfigurableMagazineLoadout.cs` | 9 | FleetEditor | interface IConfigurableMagazineLoadout |  |  |
| `FleetEditor/IFleetTemplateableActive.cs` | 13 | FleetEditor | interface IFleetTemplateableActive |  |  |
| `FleetEditor/IFleetTemplateableDesign.cs` | 13 | FleetEditor | interface IFleetTemplateableDesign |  |  |
| `FleetEditor/InitialFormationsHistoryEvent.cs` | 8 | FleetEditor | class InitialFormationsHistoryEvent : HistoryEvent |  |  |
| `FleetEditor/LobbyPlayerCountDisplay.cs` | 37 | FleetEditor | class LobbyPlayerCountDisplay : MonoBehaviour | Awake |  |
| `FleetEditor/MagazineAmmoItem.cs` | 102 | FleetEditor | class MagazineAmmoItem : Poolable | OnDestroy, OnRepooled, Start | pooling, prefab |
| `FleetEditor/MagazineSlotItem.cs` | 163 | FleetEditor | class MagazineSlotItem : Poolable | OnDestroy, OnRepooled, Start | pooling, prefab |
| `FleetEditor/MissileDesignWarning.cs` | 55 | FleetEditor | class MissileDesignWarning : DesignWarning |  |  |
| `FleetEditor/MissileEditor/BaseSettingsPane.cs` | 31 | FleetEditor.MissileEditor | class BaseSettingsPane : MonoBehaviour, IMissileSettingsPane where TComponent : MissileComponentDescriptor |  | prefab |
| `FleetEditor/MissileEditor/DoctrineSettingsBlock.cs` | 154 | FleetEditor.MissileEditor | class DoctrineSettingsBlock : MonoBehaviour<br>struct MatrixRow |  | prefab |
| `FleetEditor/MissileEditor/EditingMissileList.cs` | 244 | FleetEditor.MissileEditor | class EditingMissileList : EditorTemplateableObjectList<ModularMissile, MissileTemplate, MissileListItem, MissileTemplateListItem> |  | prefab |
| `FleetEditor/MissileEditor/IMissileSettingsPane.cs` | 15 | FleetEditor.MissileEditor | interface IMissileSettingsPane |  |  |
| `FleetEditor/MissileEditor/MissileAntiRadiationSeekerSettings.cs` | 24 | FleetEditor.MissileEditor | class MissileAntiRadiationSeekerSettings : MissileBaseSeekerSettings<PassiveARHSeekerDescriptor> |  |  |
| `FleetEditor/MissileEditor/MissileBaseAvionicsSettings.cs` | 57 | FleetEditor.MissileEditor | class MissileBaseAvionicsSettings : BaseSettingsPane<TAvionics> where TAvionics : BaseAvionicsDescriptor |  | prefab |
| `FleetEditor/MissileEditor/MissileBaseSeekerSettings.cs` | 58 | FleetEditor.MissileEditor | class MissileBaseSeekerSettings : BaseSettingsPane<TSeeker> where TSeeker : BaseSeekerDescriptor |  |  |
| `FleetEditor/MissileEditor/MissileBaseSubmunitionSettings.cs` | 78 | FleetEditor.MissileEditor | class MissileBaseSubmunitionSettings : BaseSettingsPane<TWarhead> where TWarhead : BaseSubmunitionWarheadDescriptor |  | prefab |
| `FleetEditor/MissileEditor/MissileBasicSeekerSettings.cs` | 8 | FleetEditor.MissileEditor | class MissileBasicSeekerSettings : MissileBaseSeekerSettings<BaseSeekerDescriptor> |  |  |
| `FleetEditor/MissileEditor/MissileBeamSeekerSettings.cs` | 24 | FleetEditor.MissileEditor | class MissileBeamSeekerSettings : MissileBaseSeekerSettings<BeamSeekerDescriptor> |  |  |
| `FleetEditor/MissileEditor/MissileComponentPalette.cs` | 152 | FleetEditor.MissileEditor | class MissileComponentPalette : MonoBehaviour, ISelectList | Awake, OnDestroy | prefab |
| `FleetEditor/MissileEditor/MissileCruiseAvionicsSettings.cs` | 8 | FleetEditor.MissileEditor | class MissileCruiseAvionicsSettings : MissileBaseAvionicsSettings<CruiseGuidanceDescriptor> |  |  |
| `FleetEditor/MissileEditor/MissileDirectAvionicsSettings.cs` | 24 | FleetEditor.MissileEditor | class MissileDirectAvionicsSettings : MissileBaseAvionicsSettings<DirectGuidanceDescriptor> |  |  |
| `FleetEditor/MissileEditor/MissileEngineSettings.cs` | 76 | FleetEditor.MissileEditor | class MissileEngineSettings : BaseSettingsPane<MissileEngineDescriptor> | Awake |  |
| `FleetEditor/MissileEditor/MissileFixedSubmunitionSettings.cs` | 8 | FleetEditor.MissileEditor | class MissileFixedSubmunitionSettings : MissileBaseSubmunitionSettings<FixedSubmunitionWarheadDescriptor> |  |  |
| `FleetEditor/MissileEditor/MissileListItem.cs` | 104 | FleetEditor.MissileEditor | class MissileListItem : SelectableListItem, ISettable<ModularMissile> | OnDestroy | prefab |
| `FleetEditor/MissileEditor/MissilePaletteItem.cs` | 121 | FleetEditor.MissileEditor | class MissilePaletteItem : SelectableListItem | Awake | prefab |
| `FleetEditor/MissileEditor/MissileSchematic.cs` | 52 | FleetEditor.MissileEditor | class MissileSchematic : MonoBehaviour | Awake |  |
| `FleetEditor/MissileEditor/MissileSchematicSocket.cs` | 98 | FleetEditor.MissileEditor | class MissileSchematicSocket : MonoBehaviour | OnDestroy | prefab |
| `FleetEditor/MissileEditor/MissileSelectableSubmunitionSettings.cs` | 109 | FleetEditor.MissileEditor | class MissileSelectableSubmunitionSettings : MissileBaseSubmunitionSettings<SelectableSubmunitionWarheadDescriptor> |  | prefab |
| `FleetEditor/MissileEditor/MissileSettingsPane.cs` | 161 | FleetEditor.MissileEditor | class MissileSettingsPane : MonoBehaviour | Awake | prefab |
| `FleetEditor/MissileEditor/ModalMissileIdentity.cs` | 76 | FleetEditor.MissileEditor | class ModalMissileIdentity : BaseMenu |  |  |
| `FleetEditor/MissileEditor/ResizableSchematicSocket.cs` | 141 | FleetEditor.MissileEditor | class ResizableSchematicSocket : MonoBehaviour | Awake, OnDestroy, OnEnable |  |
| `FleetEditor/MissileEditor/ResizableSchematicSocketHandle.cs` | 98 | FleetEditor.MissileEditor | class ResizableSchematicSocketHandle : Selectable<br>class NotchEvent : UnityEvent<int> | Awake, OnEnable, Update |  |
| `FleetEditor/MissileEditor/SchematicContainer.cs` | 268 | FleetEditor.MissileEditor | class SchematicContainer : MonoBehaviour |  | prefab |
| `FleetEditor/MissileEditorSubmodeController.cs` | 197 | FleetEditor | class MissileEditorSubmodeController : EditorSubmodeController | Awake |  |
| `FleetEditor/MissileTemplateDatabase.cs` | 26 | FleetEditor | class MissileTemplateDatabase : DesignTemplateDatabase<MissileTemplate, ModularMissile.MissileSummary> |  |  |
| `FleetEditor/MissileTemplateListItem.cs` | 74 | FleetEditor | class MissileTemplateListItem : SelectableListItem, ISettable<MissileTemplate> |  | prefab |
| `FleetEditor/ModalShipIdentity.cs` | 87 | FleetEditor | class ModalShipIdentity : BaseMenu |  |  |
| `FleetEditor/PaletteItem.cs` | 171 | FleetEditor | class PaletteItem : SelectableListItem |  | prefab |
| `FleetEditor/PositionEscortEvent.cs` | 36 | FleetEditor | class PositionEscortEvent : InitialFormationsHistoryEvent |  |  |
| `FleetEditor/QuickFleetTestData.cs` | 17 | FleetEditor | class QuickFleetTestData : CrossScenePipe.CrossSceneData |  |  |
| `FleetEditor/ResourceItem.cs` | 42 | FleetEditor | class ResourceItem : Poolable |  | pooling |
| `FleetEditor/ReturnFromQuickFleetTestData.cs` | 10 | FleetEditor | class ReturnFromQuickFleetTestData : CrossScenePipe.CrossSceneData |  |  |
| `FleetEditor/SettingsAmmoLoadout.cs` | 121 | FleetEditor | class SettingsAmmoLoadout : SettingsPanel where TMagProvider : class, IMagazineProvider | OnRepooled | pooling, prefab |
| `FleetEditor/SettingsDeception.cs` | 37 | FleetEditor | class SettingsDeception : SettingsPanel | OnRepooled | pooling |
| `FleetEditor/SettingsHangarBase.cs` | 61 | FleetEditor | class SettingsHangarBase : SettingsPanel |  | prefab |
| `FleetEditor/SettingsHangarLoadout.cs` | 138 | FleetEditor | class SettingsHangarLoadout : SettingsHangarBase | OnRepooled | pooling, prefab |
| `FleetEditor/SettingsMagazineLoadout.cs` | 158 | FleetEditor | class SettingsMagazineLoadout : SettingsAmmoLoadout<IMagazineProvider> | OnDestroy, OnRepooled | pooling, prefab |
| `FleetEditor/SettingsPanel.cs` | 31 | FleetEditor | class SettingsPanel : Poolable | OnRepooled | pooling |
| `FleetEditor/SettingsPanelTypes.cs` | 14 | FleetEditor | enum SettingsPanelTypes |  |  |
| `FleetEditor/SettingsResizable.cs` | 58 | FleetEditor | class SettingsResizable : SettingsPanel | Awake, OnDestroy, OnRepooled | pooling |
| `FleetEditor/SettingsSequentialOptionList.cs` | 73 | FleetEditor | class SettingsSequentialOptionList : SettingsPanel<br>struct OptionListItem<br>interface IOptionList | OnRepooled | pooling, prefab |
| `FleetEditor/SettingsSlotHangar.cs` | 58 | FleetEditor | class SettingsSlotHangar : SettingsHangarBase | OnRepooled | pooling, prefab |
| `FleetEditor/SettingsSlotMagazineLoadout.cs` | 82 | FleetEditor | class SettingsSlotMagazineLoadout : SettingsAmmoLoadout<IMagazineSlotProvider> | OnRepooled | pooling, prefab |
| `FleetEditor/SettingsWeaponGroup.cs` | 147 | FleetEditor | class SettingsWeaponGroup : SettingsPanel<br>class GroupOptionData : TMP_Dropdown.OptionData | OnDestroy, OnRepooled | pooling |
| `FleetEditor/ShipDesignWarning.cs` | 55 | FleetEditor | class ShipDesignWarning : DesignWarning |  |  |
| `FleetEditor/ShipEditorPane.cs` | 189 | FleetEditor | class ShipEditorPane : MonoBehaviour | Awake | prefab |
| `FleetEditor/ShipListItem.cs` | 160 | FleetEditor | class ShipListItem : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler | Awake, OnDestroy | prefab |
| `FleetEditor/ShipStatsPane.cs` | 167 | FleetEditor | class ShipStatsPane : MonoBehaviour | Awake | pooling, prefab |
| `FleetEditor/ShipTemplateListItem.cs` | 46 | FleetEditor | class ShipTemplateListItem : SelectableWorkshopListItem |  |  |
| `FleetEditor/SocketInfoTooltip.cs` | 97 | FleetEditor | class SocketInfoTooltip : MonoBehaviour | Start, Update | prefab |
| `FleetEditor/SocketItem.cs` | 203 | FleetEditor | class SocketItem : Poolable, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler | Awake, OnDestroy, OnRepooled | pooling, prefab |
| `FleetEditor/SocketOutlineManager.cs` | 182 | FleetEditor | class SocketOutlineManager : ImmediateModeShapeDrawer<br>class ChangedFlash |  | prefab |
| `FleetEditor/SpacecraftEditorSubmodeController.cs` | 224 | FleetEditor | class SpacecraftEditorSubmodeController : EditorSubmodeController | Awake |  |
| `FleetEditor/SpacecraftTemplateDatabase.cs` | 26 | FleetEditor | class SpacecraftTemplateDatabase : DesignTemplateDatabase<SpacecraftTemplate, Spacecraft.SpacecraftSummary> |  |  |
| `FleetEditor/Tips/AmmoDiversityTip.cs` | 25 | FleetEditor.Tips | class AmmoDiversityTip : ShipDesignTip |  |  |
| `FleetEditor/Tips/BespokeDesignTip.cs` | 20 | FleetEditor.Tips | struct BespokeDesignTip |  |  |
| `FleetEditor/Tips/CICIntegrityTip.cs` | 24 | FleetEditor.Tips | class CICIntegrityTip : ShipDesignTip |  |  |
| `FleetEditor/Tips/CommonTipAttribute.cs` | 9 | FleetEditor.Tips | class CommonTipAttribute : Attribute |  |  |
| `FleetEditor/Tips/DoubleSensorTip.cs` | 24 | FleetEditor.Tips | class DoubleSensorTip : ShipDesignTip |  |  |
| `FleetEditor/Tips/NoDCTip.cs` | 18 | FleetEditor.Tips | class NoDCTip : ShipDesignTip |  |  |
| `FleetEditor/Tips/NoPDTip.cs` | 20 | FleetEditor.Tips | class NoPDTip : ShipDesignTip |  |  |
| `FleetEditor/Tips/NoSearchSensorTip.cs` | 22 | FleetEditor.Tips | class NoSearchSensorTip : ShipDesignTip |  |  |
| `FleetEditor/Tips/OffensiveFiringTimeTip.cs` | 33 | FleetEditor.Tips | class OffensiveFiringTimeTip : ShipDesignTip |  |  |
| `FleetEditor/Tips/ParameterizedDesignTip.cs` | 28 | FleetEditor.Tips | struct ParameterizedDesignTip<br>struct Param |  |  |
| `FleetEditor/Tips/PDFiringTimeTip.cs` | 33 | FleetEditor.Tips | class PDFiringTimeTip : ShipDesignTip |  |  |
| `FleetEditor/Tips/SharedEquipmentTip.cs` | 18 | FleetEditor.Tips | class SharedEquipmentTip : ShipDesignTip |  |  |
| `FleetEditor/Tips/ShipDesignTip.cs` | 47 | FleetEditor.Tips | class ShipDesignTip |  |  |
| `FleetEditor/Tips/SpecializedComponentTip.cs` | 28 | FleetEditor.Tips | class SpecializedComponentTip : ShipDesignTip |  |  |
| `FleetEditor/Tips/TooManyBuffsTip.cs` | 17 | FleetEditor.Tips | class TooManyBuffsTip : ShipDesignTip |  |  |
| `FleetEditor/Tips/UnbuffedCarrierTip.cs` | 22 | FleetEditor.Tips | class UnbuffedCarrierTip : ShipDesignTip |  |  |
| `FleetEditor/WarningType.cs` | 10 | FleetEditor | enum WarningType |  |  |
| `Game/AI/AIUtility.cs` | 283 | Game.AI | class AIUtility<br>struct ScoreElement |  |  |
| `Game/AI/BotDifficulty.cs` | 10 | Game.AI | enum BotDifficulty |  |  |
| `Game/AI/BotDifficultyExtensions.cs` | 75 | Game.AI | class BotDifficultyExtensions |  |  |
| `Game/AI/CraftMissions/AttackCraftFlightMission.cs` | 93 | Game.AI.CraftMissions | class AttackCraftFlightMission : SkirmishCraftMission, IAIUnitDebugDrawer |  |  |
| `Game/AI/CraftMissions/GuardShipMission.cs` | 161 | Game.AI.CraftMissions | class GuardShipMission : SkirmishCraftMission, IAIUnitDebugDrawer |  |  |
| `Game/AI/CraftMissions/PaintTargetWithCraftMission.cs` | 73 | Game.AI.CraftMissions | class PaintTargetWithCraftMission : SkirmishCraftMission, IAIUnitDebugDrawer |  |  |
| `Game/AI/CraftMissions/RTBMission.cs` | 51 | Game.AI.CraftMissions | class RTBMission |  |  |
| `Game/AI/CraftMissions/ScoutWithCraftMission.cs` | 73 | Game.AI.CraftMissions | class ScoutWithCraftMission : SkirmishCraftMission, IAIUnitDebugDrawer |  |  |
| `Game/AI/CraftMissions/SkirmishCraftMission.cs` | 110 | Game.AI.CraftMissions | class SkirmishCraftMission : MissionBidPool<SkirmishCraftMission>.IMission, IDebuggableAIMission, IComparable<SkirmishCraftMission> |  |  |
| `Game/AI/CraftMissions/StageInAreaMission.cs` | 96 | Game.AI.CraftMissions | class StageInAreaMission : SkirmishCraftMission, IAIUnitDebugDrawer |  |  |
| `Game/AI/CraftMissions/StrikeShipMission.cs` | 101 | Game.AI.CraftMissions | class StrikeShipMission : SkirmishCraftMission, IAIUnitDebugDrawer |  |  |
| `Game/AI/CraftTactics/FlightUsage.cs` | 10 | Game.AI.CraftTactics | enum FlightUsage |  |  |
| `Game/AI/CraftTactics/SortieOption.cs` | 74 | Game.AI.CraftTactics | class SortieOption |  |  |
| `Game/AI/CraftTactics/TaskFlight.cs` | 399 | Game.AI.CraftTactics | class TaskFlight : PathfindingHelper.IPathfindingHelperSubject, IDebuggableAIUnit |  | prefab |
| `Game/AI/CruiseSalvoHelper.cs` | 6 | Game.AI | class CruiseSalvoHelper |  |  |
| `Game/AI/Debugging/AIDebugSnapshot.cs` | 129 | Game.AI.Debugging | class AIDebugSnapshot<br>enum BidResult<br>class Group<br>class Flight<br>class Mission |  |  |
| `Game/AI/Debugging/AIStateViewer.cs` | 549 | Game.AI.Debugging | class AIStateViewer : MonoBehaviour<br>class TaskUnitItem : SelectableListItem<br>class RoundList : MonoBehaviour<br>class MissionItem : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler | Awake, Start | prefab |
| `Game/AI/Debugging/AIUnitDebugFrame.cs` | 35 | Game.AI.Debugging | class AIUnitDebugFrame |  |  |
| `Game/AI/Debugging/AIUnitDebugWork.cs` | 16 | Game.AI.Debugging | class AIUnitDebugWork : IAIUnitDebugDrawer |  |  |
| `Game/AI/Debugging/AIVisualizer.cs` | 261 | Game.AI.Debugging | class AIVisualizer : MonoBehaviour | OnDisable, OnEnable | prefab |
| `Game/AI/Debugging/DebugComparison.cs` | 106 | Game.AI.Debugging | class DebugComparison<br>struct Step<br>class DebugComparison : DebugComparison |  |  |
| `Game/AI/Debugging/DebuggableUnitChanged.cs` | 4 | Game.AI.Debugging |  |  |  |
| `Game/AI/Debugging/DrawNodeQueryResult.cs` | 41 | Game.AI.Debugging | class DrawNodeQueryResult : AIUnitDebugWork |  |  |
| `Game/AI/Debugging/DrawProtectedNodesResult.cs` | 38 | Game.AI.Debugging | class DrawProtectedNodesResult : AIUnitDebugWork |  |  |
| `Game/AI/Debugging/DrawScoredNodeResult.cs` | 55 | Game.AI.Debugging | class DrawScoredNodeResult : AIUnitDebugWork |  |  |
| `Game/AI/Debugging/IAIUnitDebugDrawer.cs` | 9 | Game.AI.Debugging | interface IAIUnitDebugDrawer |  |  |
| `Game/AI/Debugging/IDebuggableAIMission.cs` | 15 | Game.AI.Debugging | interface IDebuggableAIMission |  |  |
| `Game/AI/Debugging/IDebuggableAIStrategy.cs` | 20 | Game.AI.Debugging | interface IDebuggableAIStrategy |  |  |
| `Game/AI/Debugging/IDebuggableAIUnit.cs` | 17 | Game.AI.Debugging | interface IDebuggableAIUnit |  |  |
| `Game/AI/DifficultyMask.cs` | 14 | Game.AI | enum DifficultyMask |  |  |
| `Game/AI/Knowledge/BaseEnemyKnowledge.cs` | 72 | Game.AI.Knowledge | class BaseEnemyKnowledge |  |  |
| `Game/AI/Knowledge/EnemyFlightKnowledge.cs` | 59 | Game.AI.Knowledge | class EnemyFlightKnowledge : BaseEnemyKnowledge |  |  |
| `Game/AI/Knowledge/EnemyGroupKnowledge.cs` | 97 | Game.AI.Knowledge | class EnemyGroupKnowledge : BaseEnemyKnowledge |  |  |
| `Game/AI/Knowledge/MapKnowledge.cs` | 69 | Game.AI.Knowledge | class MapKnowledge |  |  |
| `Game/AI/Knowledge/ObjectiveKnowledge.cs` | 140 | Game.AI.Knowledge | class ObjectiveKnowledge<br>enum PointType<br>enum Risk |  |  |
| `Game/AI/MissionBidPool.cs` | 212 | Game.AI | class MissionBidPool<br>interface IMission : IDebuggableAIMission |  |  |
| `Game/AI/MissionGenerator.cs` | 136 | Game.AI | class MissionGenerator |  |  |
| `Game/AI/Missions/AttackGroupAmbushMission.cs` | 22 | Game.AI.Missions | class AttackGroupAmbushMission : AttackGroupMission |  |  |
| `Game/AI/Missions/AttackGroupFromAreaMission.cs` | 225 | Game.AI.Missions | class AttackGroupFromAreaMission : AttackGroupMission |  |  |
| `Game/AI/Missions/AttackGroupMission.cs` | 277 | Game.AI.Missions | class AttackGroupMission : SkirmishMission, IAIUnitDebugDrawer |  |  |
| `Game/AI/Missions/CaptureControlPointMission.cs` | 84 | Game.AI.Missions | class CaptureControlPointMission : CaptureObjectiveMission<CapturePoint.Knowledge> |  |  |
| `Game/AI/Missions/CaptureObjectiveMission.cs` | 158 | Game.AI.Missions | class CaptureObjectiveMission : SkirmishMission where TObjective : ObjectiveKnowledge |  |  |
| `Game/AI/Missions/DefendAreaMission.cs` | 198 | Game.AI.Missions | class DefendAreaMission : SkirmishMission, IAIUnitDebugDrawer |  |  |
| `Game/AI/Missions/DefendObjectiveMission.cs` | 106 | Game.AI.Missions | class DefendObjectiveMission : SkirmishMission |  |  |
| `Game/AI/Missions/EstablishFrontlineMission.cs` | 74 | Game.AI.Missions | class EstablishFrontlineMission : SkirmishMission |  |  |
| `Game/AI/Missions/FireSupportFromAreaMission.cs` | 30 | Game.AI.Missions | class FireSupportFromAreaMission : AttackGroupFromAreaMission |  |  |
| `Game/AI/Missions/HoldInAreaMission.cs` | 121 | Game.AI.Missions | class HoldInAreaMission : SkirmishMission, IAIUnitDebugDrawer |  |  |
| `Game/AI/Missions/LaunchOffboardDeceptionMission.cs` | 68 | Game.AI.Missions | class LaunchOffboardDeceptionMission : SkirmishMission |  |  |
| `Game/AI/Missions/MissionCategory.cs` | 14 | Game.AI.Missions | enum MissionCategory |  |  |
| `Game/AI/Missions/MissionSet.cs` | 100 | Game.AI.Missions | class MissionSet |  |  |
| `Game/AI/Missions/PersistentFireSupportMission.cs` | 83 | Game.AI.Missions | class PersistentFireSupportMission : SkirmishMission, IAIUnitDebugDrawer |  |  |
| `Game/AI/Missions/PickupFlagMission.cs` | 111 | Game.AI.Missions | class PickupFlagMission : CaptureObjectiveMission<FlagStandPoint.Knowledge> |  |  |
| `Game/AI/Missions/PreserveUnitMission.cs` | 88 | Game.AI.Missions | class PreserveUnitMission : SkirmishMission |  |  |
| `Game/AI/Missions/RetreatUnitMission.cs` | 88 | Game.AI.Missions | class RetreatUnitMission : SkirmishMission |  |  |
| `Game/AI/Missions/ReturnFlagMission.cs` | 49 | Game.AI.Missions | class ReturnFlagMission : SkirmishMission |  |  |
| `Game/AI/Missions/ScoutLocationMission.cs` | 97 | Game.AI.Missions | class ScoutLocationMission : SkirmishMission, IAIUnitDebugDrawer |  |  |
| `Game/AI/Missions/ScoutTargetMission.cs` | 100 | Game.AI.Missions | class ScoutTargetMission : SkirmishMission, IAIUnitDebugDrawer |  |  |
| `Game/AI/Missions/SkirmishMission.cs` | 332 | Game.AI.Missions | class SkirmishMission : MissionBidPool<SkirmishMission>.IMission, IDebuggableAIMission, IComparable<SkirmishMission><br>enum OrderPhase<br>class DebugComparison : DebugComparison<SkirmishMission> |  |  |
| `Game/AI/Missions/SpotForMission.cs` | 141 | Game.AI.Missions | class SpotForMission : SkirmishMission, IAIUnitDebugDrawer |  |  |
| `Game/AI/Missions/TargetedFireSupportMission.cs` | 192 | Game.AI.Missions | class TargetedFireSupportMission : SkirmishMission, IAIUnitDebugDrawer |  |  |
| `Game/AI/MovementOptions.cs` | 13 | Game.AI | enum MovementOptions |  |  |
| `Game/AI/PathfindingHelper.cs` | 128 | Game.AI | class PathfindingHelper<br>interface IPathfindingHelperSubject |  | prefab |
| `Game/AI/ShipCapabilities.cs` | 68 | Game.AI | class ShipCapabilities<br>enum Capes |  |  |
| `Game/AI/ShipRoleDefinition.cs` | 654 | Game.AI | class ShipRoleDefinition : ScriptableObject, IModSource<br>enum LogicOp<br>enum ComparisonOp<br>class BaseScoreElement<br>class SimpleCapabilityScore : BaseScoreElement<br>class StrictCapabilityScore : BaseScoreElement<br>class MultiCapabilityScore : BaseScoreElement<br>class ComparisonScore : BaseScoreElement<br>class WeightClassScore : ComparisonScore<int><br>enum WeightAspect<br>class PointValueBaseScore : ComparisonScore<float><br>enum CostAspect<br>class PointValueRatioScore : PointValueBaseScore<br>class PointValueComparisonScore : PointValueBaseScore<br>class DesignPropertyScore : BaseScoreElement<br>enum Property<br>class SensorRangeScore : ComparisonScore<float><br>class WeaponRangeScore : ComparisonScore<float><br>class HasComponentScore : BaseScoreElement<br>class SpecificHullScore : BaseScoreElement<br>class CraftCountScore : ComparisonScore<int><br>class CraftStorageScore : ComparisonScore<int><br>class CraftPadCountScore : ComparisonScore<int><br>enum Grouping<br>enum GroupPosition<br>enum Positioning<br>enum ObjectiveRisk<br>enum StartPosition<br>enum FlankSpeed<br>class CaptainParameterTweak |  |  |
| `Game/AI/Strategy/AnnihilationStrategy.cs` | 57 | Game.AI.Strategy | class AnnihilationStrategy : SkirmishStrategy |  |  |
| `Game/AI/Strategy/CaptureTheFlagStrategy.cs` | 193 | Game.AI.Strategy | class CaptureTheFlagStrategy : AnnihilationStrategy, IScenarioGraphAIStrategy |  |  |
| `Game/AI/Strategy/ControlStrategy.cs` | 436 | Game.AI.Strategy | class ControlStrategy : SkirmishStrategy, IScenarioGraphAIStrategy<br>struct ScoreSummary<br>enum GamePhase |  |  |
| `Game/AI/Strategy/IScenarioGraphAIStrategy.cs` | 9 | Game.AI.Strategy | interface IScenarioGraphAIStrategy |  |  |
| `Game/AI/Strategy/SkirmishStrategy.cs` | 1670 | Game.AI.Strategy | class SkirmishStrategy : IDebuggableAIStrategy<br>class UnitsByMission<br>class FlightsByMission<br>struct FireSupportRequest |  | prefab |
| `Game/AI/Strategy/StationCaptureStrategy.cs` | 120 | Game.AI.Strategy | class StationCaptureStrategy : AnnihilationStrategy |  |  |
| `Game/AI/Strategy/TugOfWarStrategy.cs` | 15 | Game.AI.Strategy | class TugOfWarStrategy : ControlStrategy |  |  |
| `Game/AI/Tactics/AIControlledShip.cs` | 2782 | Game.AI.Tactics | class AIControlledShip : PathfindingHelper.IPathfindingHelperSubject<br>class MissileSalvoOption<br>class MissilePoolOption : MissileSalvoOption<br>class ReloadableMissileLauncherOption : MissileSalvoOption<br>class MixedMissileOption : MissileSalvoOption<br>class CraftTypeInfo<br>class MusteringSortie |  | prefab |
| `Game/AI/Tactics/AirDefenseAICaptain.cs` | 110 | Game.AI.Tactics | class AirDefenseAICaptain |  |  |
| `Game/AI/Tactics/AmbusherAICaptain.cs` | 30 | Game.AI.Tactics | class AmbusherAICaptain : ForeheadAICaptain |  |  |
| `Game/AI/Tactics/BacklineAICaptain.cs` | 77 | Game.AI.Tactics | class BacklineAICaptain : AIControlledShip |  |  |
| `Game/AI/Tactics/CappingShipAICaptain.cs` | 12 | Game.AI.Tactics | class CappingShipAICaptain : ForeheadAICaptain |  |  |
| `Game/AI/Tactics/CombatScoutAICaptain.cs` | 12 | Game.AI.Tactics | class CombatScoutAICaptain : ForeheadAICaptain |  |  |
| `Game/AI/Tactics/EngagementRange.cs` | 10 | Game.AI.Tactics | enum EngagementRange |  |  |
| `Game/AI/Tactics/FastAttackShipAICaptain.cs` | 91 | Game.AI.Tactics | class FastAttackShipAICaptain : ForeheadAICaptain |  |  |
| `Game/AI/Tactics/ForeheadAICaptain.cs` | 293 | Game.AI.Tactics | class ForeheadAICaptain : AIControlledShip |  |  |
| `Game/AI/Tactics/IntellitorAICaptain.cs` | 135 | Game.AI.Tactics | class IntellitorAICaptain : BacklineAICaptain |  |  |
| `Game/AI/Tactics/SniperAICaptain.cs` | 51 | Game.AI.Tactics | class SniperAICaptain : ForeheadAICaptain |  |  |
| `Game/AI/Tactics/WeaponDivison.cs` | 9 | Game.AI.Tactics | enum WeaponDivison |  |  |
| `Game/AI/Tactics/YubNubAICaptain.cs` | 203 | Game.AI.Tactics | class YubNubAICaptain : BacklineAICaptain |  |  |
| `Game/AI/TaskUnit.cs` | 455 | Game.AI | class TaskUnit : IDebuggableAIUnit |  |  |
| `Game/AI/TaskUnitChanged.cs` | 4 | Game.AI |  |  |  |
| `Game/Armor/BaseArmorInstance.cs` | 45 | Game.Armor | class BaseArmorInstance : IArmorSection |  |  |
| `Game/Armor/DynamicArmorCPUFallbackInstance.cs` | 84 | Game.Armor | class DynamicArmorCPUFallbackInstance : BaseArmorInstance |  |  |
| `Game/Armor/DynamicArmorCPUInstance.cs` | 351 | Game.Armor | class DynamicArmorCPUInstance : BaseArmorInstance<br>struct DoDamageJob : IJob<br>struct DoMSDMDamageJob : IJob<br>struct DoHealAllJob : IJob<br>struct PendingDamage |  |  |
| `Game/Armor/DynamicArmorGPUInstance.cs` | 241 | Game.Armor | class DynamicArmorGPUInstance : BaseArmorInstance, IDisposable<br>struct HIT_INFO |  |  |
| `Game/Armor/DynamicArmorManager.cs` | 101 | Game.Armor | class DynamicArmorManager : MonoBehaviour | LateUpdate, Start | Command |
| `Game/Armor/DynamicArmorMSDMInstance.cs` | 380 | Game.Armor | class DynamicArmorMSDMInstance : BaseArmorInstance, IDisposable<br>struct HIT_INFO<br>struct CalculateSocketArmorCondition : IJob<br>class AverageConditionQuery |  |  |
| `Game/Armor/IArmorSection.cs` | 26 | Game.Armor | interface IArmorSection |  |  |
| `Game/Armor/ModelSpaceDamageMap.cs` | 153 | Game.Armor | class ModelSpaceDamageMap |  |  |
| `Game/Armor/MSDMTester.cs` | 83 | Game.Armor | class MSDMTester : MonoBehaviour |  |  |
| `Game/BaseLobbyManager.cs` | 185 | Game | class BaseLobbyManager : GameManager where TPlayer : LobbyPlayer where TSettings : BaseLobbySettings |  | NetworkClient |
| `Game/BaseLobbySettings.cs` | 635 | Game | class BaseLobbySettings : NetworkBehaviour, IMatchOptionBuilder | Awake, OnStartClient, Start | NetworkBehaviour, SyncVar, ClientRpc, NetworkServer, NetworkClient |
| `Game/BotSkirmishPlayer.cs` | 231 | Game | class BotSkirmishPlayer : SkirmishPlayer | OnDestroy, Start |  |
| `Game/CalloutManager.cs` | 196 | Game | class CalloutManager : MonoBehaviour<br>struct CalloutSymbol<br>class Callout : IBoardPiece | Update |  |
| `Game/CampaignLobbyManager.cs` | 357 | Game | class CampaignLobbyManager : BaseLobbyManager<CampaignLobbyPlayer, CampaignLobbySettings> | Start | NetworkServer |
| `Game/CampaignLobbyPlayer.cs` | 424 | Game | class CampaignLobbyPlayer : LobbyPlayer, IStatusProgress<float>, IProgress<float><br>enum State |  | NetworkBehaviour, SyncVar, Command, NetworkServer, Addressables, prefab |
| `Game/CampaignLobbySettings.cs` | 363 | Game | class CampaignLobbySettings : BaseLobbySettings | Awake, Start | NetworkBehaviour, SyncVar, NetworkServer |
| `Game/CampaignPlayerHandoffData.cs` | 7 | Game | class CampaignPlayerHandoffData : PlayerHandoffData |  |  |
| `Game/CampaignStateSynchronizer.cs` | 1028 | Game | class CampaignStateSynchronizer : NetworkBehaviour | Awake | NetworkBehaviour, SyncVar, ClientRpc, NetworkServer, NetworkClient, prefab |
| `Game/CapturableFlag.cs` | 709 | Game | class CapturableFlag : NetworkBehaviour, IBoardPiece<br>enum FlagState | Awake, Update | NetworkBehaviour, SyncVar, ClientRpc, NetworkServer, NetworkClient, prefab |
| `Game/CapturePoint.cs` | 445 | Game | class CapturePoint : ObjectivePoint<br>class CapturePointState : ObjectivePointState<br>struct CaptureStatus<br>class Knowledge : ObjectiveKnowledge | Update | NetworkBehaviour, SyncVar, NetworkServer |
| `Game/Cutscenes/AnimaticUIElements.cs` | 169 | Game.Cutscenes | class AnimaticUIElements : BaseMenu | Awake | Addressables, prefab |
| `Game/Cutscenes/CutsceneController.cs` | 716 | Game.Cutscenes | class CutsceneController : NetworkBehaviour<br>enum CameraStartMotion<br>enum CameraEndMotion<br>struct CutsceneProperties<br>struct AnimaticProperties | Awake | NetworkBehaviour, ClientRpc, NetworkServer, NetworkClient, prefab |
| `Game/Cutscenes/CutsceneUIElements.cs` | 255 | Game.Cutscenes | class CutsceneUIElements : BaseMenu<br>class TitleText : MonoBehaviour<br>struct TitleTextElements | Awake | Command, prefab |
| `Game/Docking/DockingClamp.cs` | 42 | Game.Docking | class DockingClamp : MonoBehaviour, IXmlDocSerializable |  | XML |
| `Game/Docking/DockingHelpers.cs` | 17 | Game.Docking | class DockingHelpers |  |  |
| `Game/Docking/DockingPointChanged.cs` | 4 | Game.Docking |  |  |  |
| `Game/Docking/DockingPointReference.cs` | 31 | Game.Docking | class DockingPointReference : IPolySer, IXmlDocSerializable |  | XML |
| `Game/Docking/EntityDockingPointReference.cs` | 47 | Game.Docking | class EntityDockingPointReference : DockingPointReference |  | XML |
| `Game/Docking/IDockingOrderTarget.cs` | 9 | Game.Docking | interface IDockingOrderTarget |  |  |
| `Game/Docking/IDockingPoint.cs` | 64 | Game.Docking | interface IDockingPoint |  |  |
| `Game/Docking/IDockingProvider.cs` | 43 | Game.Docking | interface IDockingProvider : INetIdentifiedScript, IStorageContainerProvider, IOwned |  |  |
| `Game/Docking/ShipDockingPointReference.cs` | 73 | Game.Docking | class ShipDockingPointReference : DockingPointReference |  | save-state, XML |
| `Game/Docking/SlidingArmIKDockingClamp.cs` | 153 | Game.Docking | class SlidingArmIKDockingClamp : SlidingTrackDockingClamp |  |  |
| `Game/Docking/SlidingScissorDockingClamp.cs` | 159 | Game.Docking | class SlidingScissorDockingClamp : SlidingTrackDockingClamp |  | prefab |
| `Game/Docking/SlidingTrackDockingClamp.cs` | 196 | Game.Docking | class SlidingTrackDockingClamp : DockingClamp, IInspectable, ICampaignEditorGizmoDrawer | Start | XML |
| `Game/DoubleSwapAutoBalancer.cs` | 73 | Game | class DoubleSwapAutoBalancer : SingleSwapAutoBalancer<TPlayer> where TPlayer : class, IAutoBalancePlayer |  |  |
| `Game/DriveFlareCallout.cs` | 124 | Game | class DriveFlareCallout : IBoardPiece |  |  |
| `Game/EWar/ActiveCommsJammer.cs` | 9 | Game.EWar | class ActiveCommsJammer : ActiveJammingEffect |  |  |
| `Game/EWar/ActiveEWarEffect.cs` | 337 | Game.EWar | class ActiveEWarEffect : NetworkPoolable, IImbued, IOwned, ISettableEWarParameters | FixedUpdate, OnDestroy, OnRepooled, OnUnpooled | NetworkBehaviour, ClientRpc, NetworkServer, NetworkClient, pooling, prefab |
| `Game/EWar/ActiveJammingEffect.cs` | 112 | Game.EWar | class ActiveJammingEffect : ActiveEWarEffect, IJammingSource | OnRepooled, OnUnpooled | pooling |
| `Game/EWar/ActiveSensorJammer.cs` | 9 | Game.EWar | class ActiveSensorJammer : ActiveJammingEffect |  |  |
| `Game/EWar/Communicator.cs` | 308 | Game.EWar | class Communicator : NetworkBehaviour, IJammable, IEWarTarget, IOwned<br>class CommsPath : ICommPath | Awake, OnDestroy, OnDisable, OnEnable | NetworkBehaviour, ClientRpc, NetworkServer, NetworkClient, prefab |
| `Game/EWar/DecoyCloud.cs` | 77 | Game.EWar | class DecoyCloud : NetworkedShortDurationEffect | Awake, OnRepooled, OnUnpooled, Update | pooling |
| `Game/EWar/ELINTCategory.cs` | 12 | Game.EWar | enum ELINTCategory |  |  |
| `Game/EWar/ELINTCategoryExtensions.cs` | 28 | Game.EWar | class ELINTCategoryExtensions |  |  |
| `Game/EWar/EWarPrefabCollection.cs` | 42 | Game.EWar | class EWarPrefabCollection : SingletonMonobehaviour<EWarPrefabCollection><br>enum EwarType |  |  |
| `Game/EWar/EWarWeaponType.cs` | 11 | Game.EWar | enum EWarWeaponType |  |  |
| `Game/EWar/FalseTrack.cs` | 306 | Game.EWar | class FalseTrack : ITrack, IBoardPiece, ISensorTrackable | Update |  |
| `Game/EWar/ICommPath.cs` | 17 | Game.EWar | interface ICommPath |  |  |
| `Game/EWar/IEWarHost.cs` | 15 | Game.EWar | interface IEWarHost : IOwned |  |  |
| `Game/EWar/IEWarTarget.cs` | 12 | Game.EWar | interface IEWarTarget |  |  |
| `Game/EWar/IIlluminationSource.cs` | 16 | Game.EWar | interface IIlluminationSource |  |  |
| `Game/EWar/IJammable.cs` | 20 | Game.EWar | interface IJammable : IEWarTarget, IOwned |  |  |
| `Game/EWar/IJammingSource.cs` | 26 | Game.EWar | interface IJammingSource |  |  |
| `Game/EWar/ISettableEWarParameters.cs` | 9 | Game.EWar | interface ISettableEWarParameters |  |  |
| `Game/EWar/ITuneableCommsAntenna.cs` | 6 | Game.EWar | interface ITuneableCommsAntenna |  |  |
| `Game/EWar/ITuneableEWar.cs` | 6 | Game.EWar | interface ITuneableEWar |  |  |
| `Game/EWar/JammedVolume.cs` | 81 | Game.EWar | class JammedVolume : IVolume |  |  |
| `Game/EWar/MultiPointDecoyCloud.cs` | 263 | Game.EWar | class MultiPointDecoyCloud : NetworkPoolable, IDamageable<br>class CloudPoint | FixedUpdate, OnUnpooled | NetworkBehaviour, SyncVar, NetworkServer, pooling, prefab |
| `Game/EWar/ReceivedJamming.cs` | 146 | Game.EWar | class ReceivedJamming |  |  |
| `Game/EWar/SensorCategory.cs` | 10 | Game.EWar | enum SensorCategory |  |  |
| `Game/EWar/SensorIlluminator.cs` | 63 | Game.EWar | class SensorIlluminator : ActiveEWarEffect, IIlluminationSource | OnRepooled | pooling |
| `Game/FadingArmorBrush.cs` | 44 | Game | class FadingArmorBrush : MonoBehaviour | Update |  |
| `Game/FlagStandPoint.cs` | 290 | Game | class FlagStandPoint : ObjectivePoint, IFlagCarrier<br>class FlagPointState : ObjectivePointState<br>class Knowledge : ObjectiveKnowledge |  | NetworkBehaviour, SyncVar, NetworkServer, save-state, prefab |
| `Game/FlareCalloutMessage.cs` | 11 | Game | struct FlareCalloutMessage : NetworkMessage |  |  |
| `Game/FlareCalloutSerializers.cs` | 30 | Game | class FlareCalloutSerializers |  |  |
| `Game/FleetFactory.cs` | 90 | Game | class FleetFactory : MonoBehaviour |  | NetworkServer, prefab |
| `Game/GameManager.cs` | 672 | Game | class GameManager : MonoBehaviour, INetManagerEventHandler<br>struct RoomIdentity | Awake, OnDestroy | NetworkServer, prefab |
| `Game/GameMode.cs` | 9 | Game | enum GameMode |  |  |
| `Game/GameModeExtensions.cs` | 7 | Game | class GameModeExtensions |  |  |
| `Game/GreedyAutoBalancer.cs` | 61 | Game | class GreedyAutoBalancer : TeamAutoBalancer<TPlayer> where TPlayer : class, IAutoBalancePlayer |  |  |
| `Game/HitReport.cs` | 15 | Game | struct HitReport |  |  |
| `Game/HitResult.cs` | 11 | Game | enum HitResult |  |  |
| `Game/HumanSkirmishPlayer.cs` | 2591 | Game | class HumanSkirmishPlayer : SkirmishPlayer | OnDestroy, OnStartClient, Start, Update | NetworkBehaviour, ClientRpc, NetworkServer, NetworkClient, prefab |
| `Game/IAutoBalancePlayer.cs` | 13 | Game | interface IAutoBalancePlayer |  |  |
| `Game/IDamageable.cs` | 32 | Game | interface IDamageable |  | prefab |
| `Game/IFF.cs` | 11 | Game | enum IFF |  |  |
| `Game/IFFExtensions.cs` | 90 | Game | class IFFExtensions |  |  |
| `Game/IFlagCarrier.cs` | 32 | Game | interface IFlagCarrier |  |  |
| `Game/IIdentifiable.cs` | 25 | Game | interface IIdentifiable |  |  |
| `Game/Intel/BaseIntelReport.cs` | 9 | Game.Intel | class BaseIntelReport |  |  |
| `Game/Intel/DeceptiveIdentity.cs` | 248 | Game.Intel | class DeceptiveIdentity : NetworkBehaviour, IIdentifiable, IIntelReportable | Awake, OnEnable, OnStartServer | NetworkBehaviour, SyncVar |
| `Game/Intel/DeceptiveIdentityNamelist.cs` | 37 | Game.Intel | class DeceptiveIdentityNamelist : MonoBehaviour |  |  |
| `Game/Intel/DelayedRevealFact.cs` | 105 | Game.Intel | class DelayedRevealFact |  |  |
| `Game/Intel/DiscriminationTiers.cs` | 30 | Game.Intel | struct DiscriminationTiers |  |  |
| `Game/Intel/FriendlyShipIntelReport.cs` | 14 | Game.Intel | class FriendlyShipIntelReport : BaseIntelReport |  |  |
| `Game/Intel/IIntelIdentity.cs` | 16 | Game.Intel | interface IIntelIdentity |  |  |
| `Game/Intel/IIntelReportable.cs` | 9 | Game.Intel | interface IIntelReportable : IIdentifiable |  |  |
| `Game/Intel/IMissileReportSource.cs` | 9 | Game.Intel | interface IMissileReportSource |  |  |
| `Game/Intel/IntelligenceTargetObject.cs` | 831 | Game.Intel | class IntelligenceTargetObject : NetworkBehaviour, ITargetIntel, IIntelIdentity, IBulkSaveComponent<br>struct ContactInfo | Awake, OnDestroy, Update | NetworkBehaviour, SyncVar, NetworkServer, save-state, XML, prefab |
| `Game/Intel/ITargetIntel.cs` | 30 | Game.Intel | interface ITargetIntel |  |  |
| `Game/Intel/MissileIntelReport.cs` | 42 | Game.Intel | class MissileIntelReport : BaseIntelReport |  |  |
| `Game/Intel/ShipIntelReport.cs` | 62 | Game.Intel | class ShipIntelReport : BaseIntelReport |  |  |
| `Game/Intel/TieredDelayedRevealFact.cs` | 74 | Game.Intel | class TieredDelayedRevealFact |  |  |
| `Game/Intel/TrackTag.cs` | 9 | Game.Intel | enum TrackTag |  |  |
| `Game/InventoryManagement/BaseStorageContainerPool.cs` | 237 | Game.InventoryManagement | class BaseStorageContainerPool : IStorageContainerPool, IStorageContainer where TContainer : IStorageContainer |  |  |
| `Game/InventoryManagement/BuiltinStorableItem.cs` | 51 | Game.InventoryManagement | class BuiltinStorableItem : IStorable, ISaveKeyed |  |  |
| `Game/InventoryManagement/FluidUnit.cs` | 14 | Game.InventoryManagement | class FluidUnit : BuiltinStorableItem |  |  |
| `Game/InventoryManagement/GeneralItem.cs` | 14 | Game.InventoryManagement | class GeneralItem : BuiltinStorableItem |  |  |
| `Game/InventoryManagement/IEditableStorageContainer.cs` | 13 | Game.InventoryManagement | interface IEditableStorageContainer |  |  |
| `Game/InventoryManagement/IInstanceStorageContainer.cs` | 9 | Game.InventoryManagement | interface IInstanceStorageContainer : IStorageContainer |  |  |
| `Game/InventoryManagement/InstancedStorageContainerPool.cs` | 82 | Game.InventoryManagement | class InstancedStorageContainerPool : BaseStorageContainerPool<IInstanceStorageContainer>, IInstanceStorageContainer, IStorageContainer |  |  |
| `Game/InventoryManagement/InventoryHelpers.cs` | 136 | Game.InventoryManagement | class InventoryHelpers |  |  |
| `Game/InventoryManagement/InventorySerializers.cs` | 58 | Game.InventoryManagement | class InventorySerializers |  |  |
| `Game/InventoryManagement/InventoryTransferManager.cs` | 280 | Game.InventoryManagement | class InventoryTransferManager : NetworkBehaviour | Awake, FixedUpdate, OnDestroy, Start | NetworkBehaviour, NetworkServer |
| `Game/InventoryManagement/ISimpleStorageContainer.cs` | 9 | Game.InventoryManagement | interface ISimpleStorageContainer : IStorageContainer |  |  |
| `Game/InventoryManagement/IStorable.cs` | 22 | Game.InventoryManagement | interface IStorable : ISaveKeyed |  |  |
| `Game/InventoryManagement/IStorableItemSet.cs` | 11 | Game.InventoryManagement | interface IStorableItemSet |  |  |
| `Game/InventoryManagement/IStorageContainer.cs` | 42 | Game.InventoryManagement | interface IStorageContainer |  |  |
| `Game/InventoryManagement/IStorageContainerPool.cs` | 11 | Game.InventoryManagement | interface IStorageContainerPool : IStorageContainer |  |  |
| `Game/InventoryManagement/IStorageContainerProvider.cs` | 19 | Game.InventoryManagement | interface IStorageContainerProvider : INetIdentifiedScript |  |  |
| `Game/InventoryManagement/PlatformStorageContainerReference.cs` | 40 | Game.InventoryManagement | class PlatformStorageContainerReference : StorageContainerReference |  |  |
| `Game/InventoryManagement/SingleItemContainer.cs` | 275 | Game.InventoryManagement | class SingleItemContainer : ISimpleStorageContainer, IStorageContainer, IEditableStorageContainer, IInspectable, IXmlDocSerializable, IQuantityHolder |  | XML |
| `Game/InventoryManagement/StorableComparer.cs` | 17 | Game.InventoryManagement | class StorableComparer : EqualityComparer<IStorable> |  |  |
| `Game/InventoryManagement/StorageChanged.cs` | 4 | Game.InventoryManagement |  |  |  |
| `Game/InventoryManagement/StorageContainerPool.cs` | 60 | Game.InventoryManagement | class StorageContainerPool : BaseStorageContainerPool<ISimpleStorageContainer>, ISimpleStorageContainer, IStorageContainer |  |  |
| `Game/InventoryManagement/StorageContainerReference.cs` | 14 | Game.InventoryManagement | class StorageContainerReference : IPolySer |  |  |
| `Game/InventoryManagement/StorageItemClass.cs` | 10 | Game.InventoryManagement | enum StorageItemClass |  |  |
| `Game/InventoryManagement/StorageItemTransfer.cs` | 192 | Game.InventoryManagement | class StorageItemTransfer |  |  |
| `Game/InventoryManagement/StorageItemTransferComparer.cs` | 28 | Game.InventoryManagement | class StorageItemTransferComparer : IEqualityComparer<StorageItemTransfer> |  |  |
| `Game/InventoryManagement/SyncSingleItemContainer.cs` | 92 | Game.InventoryManagement | class SyncSingleItemContainer : SyncObject, ICustomInspectable, IInspectable, IXmlDocSerializable |  | XML |
| `Game/InventoryManagement/TransferChanged.cs` | 4 | Game.InventoryManagement |  |  |  |
| `Game/IOwned.cs` | 9 | Game | interface IOwned |  |  |
| `Game/IPlayer.cs` | 57 | Game | interface IPlayer : IPlayerInfo |  | prefab |
| `Game/IPlayerActionAvailable.cs` | 33 | Game | interface IPlayerActionAvailable |  |  |
| `Game/IPlayerInfo.cs` | 35 | Game | interface IPlayerInfo |  |  |
| `Game/ISelectable.cs` | 58 | Game | interface ISelectable : IOwned |  | prefab |
| `Game/ISubDamageable.cs` | 19 | Game | interface ISubDamageable |  |  |
| `Game/LobbyPlayer.cs` | 1281 | Game | class LobbyPlayer : NetworkBehaviour, IPlayer, IPlayerInfo, IAutoBalancePlayer | Awake, OnDestroy, OnStartAuthority, Start | NetworkBehaviour, SyncVar, Command, NetworkServer, NetworkClient, prefab |
| `Game/Map/Battlespace.cs` | 280 | Game.Map | class Battlespace : MonoBehaviour, ISkirmishBattlespaceInfo, ISkirmishBattlespaceLoader, IModSource<br>class SimpleBattlespaceReference : SavedBattlespaceLoaderReference | Awake | XML, prefab |
| `Game/Map/BattlespaceInfoBlock.cs` | 231 | Game.Map | class BattlespaceInfoBlock : ScriptableObject, ISkirmishBattlespaceInfo, ISkirmishBattlespaceLoader, IModSource, IManagedAddressableAsset<br>class InfoBlockBattlespaceReference : SavedBattlespaceLoaderReference |  | XML, Addressables, prefab |
| `Game/Map/CameraStackedSkybox.cs` | 49 | Game.Map | class CameraStackedSkybox : MonoBehaviour, ISkyboxLayer, ISkyboxLayerSpawner |  | prefab |
| `Game/Map/CameraStackedSkyboxRenderer.cs` | 134 | Game.Map | class CameraStackedSkyboxRenderer : SingletonMonobehaviour<CameraStackedSkyboxRenderer> | Awake, LateUpdate, OnDestroy | prefab |
| `Game/Map/CreateSkyboxJSONPlanet.cs` | 67 | Game.Map | class CreateSkyboxJSONPlanet : MonoBehaviour | Awake | prefab |
| `Game/Map/IReadOnlySpacePartition.cs` | 60 | Game.Map | interface IReadOnlySpacePartition |  |  |
| `Game/Map/ISkirmishBattlespaceInfo.cs` | 39 | Game.Map | interface ISkirmishBattlespaceInfo : ISkirmishBattlespaceLoader, IModSource |  |  |
| `Game/Map/ISkirmishBattlespaceLoader.cs` | 21 | Game.Map | interface ISkirmishBattlespaceLoader |  |  |
| `Game/Map/ISkyboxLayer.cs` | 17 | Game.Map | interface ISkyboxLayer |  |  |
| `Game/Map/ISkyboxLayerSpawner.cs` | 9 | Game.Map | interface ISkyboxLayerSpawner |  |  |
| `Game/Map/ITStarVertex.cs` | 9 | Game.Map | interface ITStarVertex |  |  |
| `Game/Map/IVisibilityObject.cs` | 27 | Game.Map | interface IVisibilityObject |  |  |
| `Game/Map/NodeQueryResult.cs` | 16 | Game.Map | class NodeQueryResult |  |  |
| `Game/Map/Octree.cs` | 934 | Game.Map | class Octree<br>class Serializable |  | XML |
| `Game/Map/OctreeData.cs` | 24 | Game.Map | class OctreeData : ScriptableObject |  |  |
| `Game/Map/OctreeNode.cs` | 1076 | Game.Map | class OctreeNode<br>class Serializable |  |  |
| `Game/Map/OTVertex.cs` | 103 | Game.Map | class OTVertex : ITStarVertex |  |  |
| `Game/Map/PreferredScenarioSettings.cs` | 20 | Game.Map | struct PreferredScenarioSettings<br>struct Setting |  |  |
| `Game/Map/PrimaryMapLighting.cs` | 44 | Game.Map | class PrimaryMapLighting : MonoBehaviour | Awake, OnDestroy | prefab |
| `Game/Map/SavedBattlespaceLoaderReference.cs` | 25 | Game.Map | class SavedBattlespaceLoaderReference : IXmlDocSerializable |  | XML |
| `Game/Map/SpacePartitioner.cs` | 476 | Game.Map | class SpacePartitioner : MonoBehaviour, IReadOnlySpacePartition<br>interface ISpatialTask<br>struct PathfindingTask<br>struct GenericQueryTask | OnDestroy, Update |  |
| `Game/Map/SpawnGroup.cs` | 47 | Game.Map | class SpawnGroup : MonoBehaviour |  |  |
| `Game/Map/SpawnPoint.cs` | 36 | Game.Map | class SpawnPoint : MonoBehaviour |  |  |
| `Game/Map/SpawnZone.cs` | 17 | Game.Map | struct SpawnZone |  |  |
| `Game/Map/TemporaryPathfindingGraph.cs` | 103 | Game.Map | class TemporaryPathfindingGraph : IImplicitUndirectedGraph<OTVertex, SEdge<OTVertex>>, IImplicitVertexSet<OTVertex>, IGraph<OTVertex, SEdge<OTVertex>> |  |  |
| `Game/Map/ThetaStarShortestPath.cs` | 152 | Game.Map | class ThetaStarShortestPath<br>struct Step |  |  |
| `Game/Map/VisibilityCell.cs` | 261 | Game.Map | class VisibilityCell |  |  |
| `Game/Map/VisibilityMatrix.cs` | 161 | Game.Map | class VisibilityMatrix : MonoBehaviour | Awake, OnDestroy |  |
| `Game/Map/VisibleObject.cs` | 405 | Game.Map | class VisibleObject : NetworkBehaviour, IVisibilityObject<br>enum EyeType | Awake, OnDestroy, OnDisable, OnEnable, OnStartServer, Update | NetworkBehaviour, SyncVar, NetworkServer, prefab |
| `Game/MapBoundaryLimiter.cs` | 428 | Game | class MapBoundaryLimiter : NetworkBehaviour | OnDestroy, OnDisable, Start, Update | NetworkBehaviour, SyncVar, ClientRpc, NetworkServer, NetworkClient |
| `Game/NetworkedShortDurationEffect.cs` | 81 | Game | class NetworkedShortDurationEffect : NetworkPoolable | Awake, OnRepooled, OnUnpooled, Update | pooling |
| `Game/ObjectivePoint.cs` | 266 | Game | class ObjectivePoint : PointOfInterest<br>class ObjectivePointState | OnStartClient | NetworkBehaviour, SyncVar, NetworkServer, prefab |
| `Game/ObjectivePointHullColor.cs` | 75 | Game | class ObjectivePointHullColor : MonoBehaviour<br>struct MaterialSet | Awake |  |
| `Game/Orders/CraftTasks/AttackTargetTask.cs` | 621 | Game.Orders.CraftTasks | class AttackTargetTask : CraftOrderTask<br>class SavedAttackTargetTask : SavedOrderTask<br>class AttackTargetSyncState : CraftOrderSyncState | FixedUpdate | XML |
| `Game/Orders/CraftTasks/CraftOrderSyncStateSerializers.cs` | 18 | Game.Orders.CraftTasks | class CraftOrderSyncStateSerializers |  |  |
| `Game/Orders/CraftTasks/CraftOrderTask.cs` | 65 | Game.Orders.CraftTasks | class CraftOrderTask : OrderTask, INavPathTask<br>class CraftOrderSyncState : IPolySer |  |  |
| `Game/Orders/CraftTasks/GuardCraftTask.cs` | 176 | Game.Orders.CraftTasks | class GuardCraftTask : CraftOrderTask<br>class SavedGuardCraftTask : SavedOrderTask | FixedUpdate | save-state, XML |
| `Game/Orders/CraftTasks/GuardShipTask.cs` | 200 | Game.Orders.CraftTasks | class GuardShipTask : CraftOrderTask<br>class SavedGuardShipTask : SavedOrderTask | FixedUpdate | save-state, XML |
| `Game/Orders/CraftTasks/JoinGroupTask.cs` | 131 | Game.Orders.CraftTasks | class JoinGroupTask : CraftOrderTask<br>class SavedJoinGroupTask : SavedOrderTask |  | save-state, XML |
| `Game/Orders/CraftTasks/LandingStatusSerializers.cs` | 22 | Game.Orders.CraftTasks | class LandingStatusSerializers |  |  |
| `Game/Orders/CraftTasks/LandOnShipTask.cs` | 481 | Game.Orders.CraftTasks | class LandOnShipTask : CraftOrderTask<br>enum LandingStage<br>class LandingStatus : IXmlDocSerializable<br>class SavedLandOnShipTask : SavedOrderTask<br>class LandingSyncState : CraftOrderSyncState<br>class TeleportSentinel | FixedUpdate | save-state, XML |
| `Game/Orders/CraftTasks/PaintTargetTask.cs` | 341 | Game.Orders.CraftTasks | class PaintTargetTask : CraftOrderTask<br>class SavedPaintTargetTask : SavedOrderTask<br>class PaintTargetSyncState : CraftOrderSyncState | FixedUpdate | XML |
| `Game/Orders/CraftTasks/SearchWreckTask.cs` | 246 | Game.Orders.CraftTasks | class SearchWreckTask : CraftOrderTask<br>class SavedSearchWreckTask : SavedOrderTask | FixedUpdate | save-state, XML |
| `Game/Orders/DoublePieceOrderFeedbackLine.cs` | 53 | Game.Orders | class DoublePieceOrderFeedbackLine : OrderFeedbackLine |  |  |
| `Game/Orders/FixedOriginOrderFeedbackLine.cs` | 41 | Game.Orders | class FixedOriginOrderFeedbackLine : OrderFeedbackLine |  |  |
| `Game/Orders/FixedPositionOrderFeedbackLine.cs` | 40 | Game.Orders | class FixedPositionOrderFeedbackLine : OrderFeedbackLine |  |  |
| `Game/Orders/ICraftOrderReceiver.cs` | 36 | Game.Orders | interface ICraftOrderReceiver : IOrderReceiver, IOwned, INavigationTaskReceiver |  |  |
| `Game/Orders/ILockedHeadingTarget.cs` | 9 | Game.Orders | interface ILockedHeadingTarget |  |  |
| `Game/Orders/INavigationTaskReceiver.cs` | 15 | Game.Orders | interface INavigationTaskReceiver : IOrderReceiver, IOwned |  |  |
| `Game/Orders/INavPathTask.cs` | 15 | Game.Orders | interface INavPathTask |  |  |
| `Game/Orders/Inputs/ConstantInput.cs` | 14 | Game.Orders.Inputs | class ConstantInput : OrderInput<TDataType> |  |  |
| `Game/Orders/Inputs/CraftSelectionInput.cs` | 94 | Game.Orders.Inputs | class CraftSelectionInput : OrderInput<SpacecraftGroup>, IWidgetPositionSource |  |  |
| `Game/Orders/Inputs/IOrderInput.cs` | 19 | Game.Orders.Inputs | interface IOrderInput |  |  |
| `Game/Orders/Inputs/OrderInput.cs` | 52 | Game.Orders.Inputs | class OrderInput : IOrderInput |  |  |
| `Game/Orders/Inputs/OrientationInput.cs` | 57 | Game.Orders.Inputs | class OrientationInput : OrderInput<OrientationInput.Output><br>class Output |  |  |
| `Game/Orders/Inputs/ShipSelectionInput.cs` | 98 | Game.Orders.Inputs | class ShipSelectionInput : OrderInput<ShipController>, IWidgetPositionSource |  |  |
| `Game/Orders/Inputs/TrackSelectionInput.cs` | 210 | Game.Orders.Inputs | class TrackSelectionInput : OrderInput<TrackSelectionInput.Output>, IWidgetPositionSource<br>class Output |  |  |
| `Game/Orders/Inputs/WorldPositionInput.cs` | 188 | Game.Orders.Inputs | class WorldPositionInput : OrderInput<WorldPositionInput.Output>, IWidgetPositionSource<br>class Output |  |  |
| `Game/Orders/IOrderAssignable.cs` | 11 | Game.Orders | interface IOrderAssignable |  |  |
| `Game/Orders/IOrderReceiver.cs` | 46 | Game.Orders | interface IOrderReceiver : IOwned |  | prefab |
| `Game/Orders/IWarshipOrderReceiver.cs` | 56 | Game.Orders | interface IWarshipOrderReceiver : IOrderReceiver, IOwned |  |  |
| `Game/Orders/OrderFeedbackLine.cs` | 15 | Game.Orders | class OrderFeedbackLine |  |  |
| `Game/Orders/PieceOrderFeedbackLine.cs` | 53 | Game.Orders | class PieceOrderFeedbackLine : OrderFeedbackLine |  |  |
| `Game/Orders/PlayerOrder.cs` | 146 | Game.Orders | class PlayerOrder |  |  |
| `Game/Orders/PlayerOrderTemplate.cs` | 6 | Game.Orders | class PlayerOrderTemplate |  |  |
| `Game/Orders/PrePlannedResponseSet.cs` | 6 | Game.Orders | class PrePlannedResponseSet |  |  |
| `Game/Orders/Tasks/CalloutTask.cs` | 76 | Game.Orders.Tasks | class CalloutTask : OrderTask |  |  |
| `Game/Orders/Tasks/ChangeAmmoTask.cs` | 114 | Game.Orders.Tasks | class ChangeAmmoTask : WarshipOrderTask<br>class SavedChangeAmmoTask : SavedOrderTask |  | XML |
| `Game/Orders/Tasks/DockingTask.cs` | 244 | Game.Orders.Tasks | class DockingTask : NavigationTask<br>class SavedDockingTask : SavedNavigationTask | FixedUpdate | XML |
| `Game/Orders/Tasks/DriftNavigationTask.cs` | 47 | Game.Orders.Tasks | class DriftNavigationTask : NavigationTask | FixedUpdate |  |
| `Game/Orders/Tasks/DriveCourseTask.cs` | 157 | Game.Orders.Tasks | class DriveCourseTask : NavigationTask<br>class SavedDriveCourseTask : SavedNavigationTask | FixedUpdate | XML |
| `Game/Orders/Tasks/FireDecoyTask.cs` | 90 | Game.Orders.Tasks | class FireDecoyTask : WarshipOrderTask |  |  |
| `Game/Orders/Tasks/FireMissilePositionTask.cs` | 273 | Game.Orders.Tasks | class FireMissilePositionTask : FireMissileTask<br>class FireMissilePositionTaskWeb : IEngagementWebTarget<br>class SavedFireMissilePositionTask : SavedFireMissileTask |  | XML |
| `Game/Orders/Tasks/FireMissileTask.cs` | 79 | Game.Orders.Tasks | class FireMissileTask : FireTask<br>class SavedFireMissileTask : SavedFireTask |  | XML |
| `Game/Orders/Tasks/FireMissileTrackTask.cs` | 322 | Game.Orders.Tasks | class FireMissileTrackTask : FireMissileTask<br>class FireMissileTrackTaskWeb : IEngagementWebTarget<br>class SavedFireMissileTrackTask : SavedFireMissileTask |  | XML |
| `Game/Orders/Tasks/FirePositionTask.cs` | 215 | Game.Orders.Tasks | class FirePositionTask : FireTask<br>class FirePositionTaskWeb : IEngagementWebTarget<br>class FireDirectionTaskWeb : IEngagementWebTarget<br>class SavedFirePositionTask : SavedFireTask |  | XML |
| `Game/Orders/Tasks/FireTask.cs` | 154 | Game.Orders.Tasks | class FireTask : WarshipOrderTask<br>class SavedFireTask : SavedOrderTask |  | XML |
| `Game/Orders/Tasks/FireTrackTask.cs` | 276 | Game.Orders.Tasks | class FireTrackTask : FireTask<br>class FireTrackTaskWeb : IEngagementWebTarget<br>class SavedFireTrackTask : SavedFireTask |  | XML |
| `Game/Orders/Tasks/HoldPositionTask.cs` | 139 | Game.Orders.Tasks | class HoldPositionTask : NavigationTask<br>class SavedHoldPositionTask : SavedNavigationTask | FixedUpdate | XML |
| `Game/Orders/Tasks/KeepFormationTask.cs` | 269 | Game.Orders.Tasks | class KeepFormationTask : NavigationTask<br>class SavedKeepFormationTask : SavedNavigationTask | FixedUpdate | XML, prefab |
| `Game/Orders/Tasks/LockTargetTask.cs` | 230 | Game.Orders.Tasks | class LockTargetTask : WarshipOrderTask<br>class LockTargetTaskWeb : IEngagementWebTarget<br>class SavedLockTargetTask : SavedOrderTask |  | XML |
| `Game/Orders/Tasks/MoveToTask.cs` | 217 | Game.Orders.Tasks | class MoveToTask : NavigationTask<br>class SavedMoveToTask : SavedNavigationTask | FixedUpdate | XML |
| `Game/Orders/Tasks/NavigationTask.cs` | 117 | Game.Orders.Tasks | class NavigationTask : OrderTask, INavPathTask<br>class SavedNavigationTask : SavedOrderTask | FixedUpdate |  |
| `Game/Orders/Tasks/OrbitPositionTask.cs` | 110 | Game.Orders.Tasks | class OrbitPositionTask : OrbitTask<br>class SavedOrbitPositionTask : SavedOrbitTask |  | XML |
| `Game/Orders/Tasks/OrbitTask.cs` | 161 | Game.Orders.Tasks | class OrbitTask : NavigationTask<br>class SavedOrbitTask : SavedNavigationTask | FixedUpdate | XML |
| `Game/Orders/Tasks/OrbitTrackTask.cs` | 171 | Game.Orders.Tasks | class OrbitTrackTask : OrbitTask<br>class SavedOrbitTrackTask : SavedOrbitTask |  | XML |
| `Game/Orders/Tasks/OrderTask.cs` | 436 | Game.Orders.Tasks | class OrderTask : IPolySer<br>struct Input<br>class SavedOrderTask : IXmlDocSerializable | FixedUpdate, Update | NetworkServer, XML, prefab |
| `Game/Orders/Tasks/OrderTaskSerializers.cs` | 18 | Game.Orders.Tasks | class OrderTaskSerializers |  |  |
| `Game/Orders/Tasks/PointDefensePriorityTask.cs` | 125 | Game.Orders.Tasks | class PointDefensePriorityTask : WarshipOrderTask<br>class SavedPDPriorityTask : SavedOrderTask |  | XML |
| `Game/Orders/Tasks/SensorBurnthroughTask.cs` | 30 | Game.Orders.Tasks | class SensorBurnthroughTask : WarshipOrderTask |  |  |
| `Game/Orders/Tasks/SetLockedHeadingTask.cs` | 126 | Game.Orders.Tasks | class SetLockedHeadingTask : WarshipOrderTask<br>class SavedLockedHeadingTask : SavedOrderTask<br>class Source : ILockedHeadingTarget |  | XML |
| `Game/Orders/Tasks/SetLockedHeadingTrackTask.cs` | 160 | Game.Orders.Tasks | class SetLockedHeadingTrackTask : WarshipOrderTask<br>class SavedLockedHeadingTrackTask : SavedOrderTask<br>class Source : ILockedHeadingTarget |  | XML |
| `Game/Orders/Tasks/SetLockedUpTask.cs` | 105 | Game.Orders.Tasks | class SetLockedUpTask : WarshipOrderTask<br>class SavedLockedUpTask : SavedOrderTask |  | XML |
| `Game/Orders/Tasks/WarshipOrderTask.cs` | 28 | Game.Orders.Tasks | class WarshipOrderTask : OrderTask |  |  |
| `Game/PersistentEffect.cs` | 102 | Game | class PersistentEffect : Poolable | Awake, OnRepooled, OnUnpooled | pooling |
| `Game/PlayerAction.cs` | 26 | Game | enum PlayerAction |  |  |
| `Game/PlayerActionLimiter.cs` | 68 | Game | class PlayerActionLimiter : IPlayerActionAvailable |  |  |
| `Game/PlayerColors.cs` | 16 | Game | struct PlayerColors |  |  |
| `Game/PlayerHandoffData.cs` | 31 | Game | class PlayerHandoffData |  |  |
| `Game/PointOfInterest.cs` | 409 | Game | class PointOfInterest : NetworkBehaviour, IBoardPiece | Awake, OnDestroy, OnStartClient | NetworkBehaviour, SyncVar, ClientRpc, NetworkServer, NetworkClient |
| `Game/Reports/AARPlayerReport.cs` | 83 | Game.Reports | class AARPlayerReport : IPlayerInfo |  |  |
| `Game/Reports/AARTeamReport.cs` | 21 | Game.Reports | class AARTeamReport |  |  |
| `Game/Reports/AfterActionReport.cs` | 38 | Game.Reports | class AfterActionReport : CrossScenePipe.CrossSceneData<br>class TeamReport : AARTeamReport<TShipReport, TCraftReport> |  |  |
| `Game/Reports/AfterActionSummary.cs` | 6 | Game.Reports | class AfterActionSummary : AfterActionReport<ShipBattleSummary, CraftBattleSummary> |  |  |
| `Game/Reports/AntiShipReport.cs` | 39 | Game.Reports | class AntiShipReport |  |  |
| `Game/Reports/ContinuousWeaponReport.cs` | 105 | Game.Reports | class ContinuousWeaponReport : WeaponReport |  |  |
| `Game/Reports/CraftBattleReport.cs` | 39 | Game.Reports | class CraftBattleReport : CraftBattleSummary |  |  |
| `Game/Reports/CraftBattleSummary.cs` | 31 | Game.Reports | class CraftBattleSummary |  |  |
| `Game/Reports/CraftDefenseReport.cs` | 6 | Game.Reports | class CraftDefenseReport |  |  |
| `Game/Reports/CraftEngagementReport.cs` | 32 | Game.Reports | class CraftEngagementReport<br>struct Participant |  |  |
| `Game/Reports/CraftMissileReport.cs` | 115 | Game.Reports | class CraftMissileReport : WeaponReport |  |  |
| `Game/Reports/CraftSortieReport.cs` | 16 | Game.Reports | class CraftSortieReport |  |  |
| `Game/Reports/CraftSuperiorityReport.cs` | 42 | Game.Reports | class CraftSuperiorityReport : CraftWeaponReport |  |  |
| `Game/Reports/CraftWeaponReport.cs` | 45 | Game.Reports | class CraftWeaponReport |  |  |
| `Game/Reports/DecoyReport.cs` | 52 | Game.Reports | class DecoyReport : SimpleSubReport |  |  |
| `Game/Reports/DefensiveMissileReport.cs` | 45 | Game.Reports | class DefensiveMissileReport : SimpleSubReport |  |  |
| `Game/Reports/DefensiveReport.cs` | 60 | Game.Reports | class DefensiveReport |  |  |
| `Game/Reports/DefensiveWeaponReport.cs` | 34 | Game.Reports | class DefensiveWeaponReport : SimpleSubReport |  |  |
| `Game/Reports/DiscreteWeaponReport.cs` | 81 | Game.Reports | class DiscreteWeaponReport : WeaponReport |  |  |
| `Game/Reports/EfficiencyRating.cs` | 12 | Game.Reports | enum EfficiencyRating |  |  |
| `Game/Reports/EfficiencyRatingExtensions.cs` | 87 | Game.Reports | class EfficiencyRatingExtensions |  |  |
| `Game/Reports/EngineeringReport.cs` | 56 | Game.Reports | class EngineeringReport |  |  |
| `Game/Reports/FullAfterActionReport.cs` | 56 | Game.Reports | class FullAfterActionReport : AfterActionReport<ShipBattleReport, CraftBattleReport> |  | XML |
| `Game/Reports/IWeaponStatReportReceiver.cs` | 9 | Game.Reports | interface IWeaponStatReportReceiver |  |  |
| `Game/Reports/MissileReport.cs` | 46 | Game.Reports | class MissileReport |  |  |
| `Game/Reports/OffensiveMissileReport.cs` | 35 | Game.Reports | class OffensiveMissileReport |  |  |
| `Game/Reports/PartDamage.cs` | 11 | Game.Reports | struct PartDamage |  |  |
| `Game/Reports/SensorReport.cs` | 49 | Game.Reports | class SensorReport |  |  |
| `Game/Reports/ShipBattleReport.cs` | 61 | Game.Reports | class ShipBattleReport : ShipBattleSummary<br>struct EnemyEngagement |  |  |
| `Game/Reports/ShipBattleSummary.cs` | 83 | Game.Reports | class ShipBattleSummary |  |  |
| `Game/Reports/SimpleSubReport.cs` | 19 | Game.Reports | class SimpleSubReport |  |  |
| `Game/Reports/WeaponReport.cs` | 82 | Game.Reports | class WeaponReport : SimpleSubReport |  |  |
| `Game/SavedSkirmishGame.cs` | 107 | Game | class SavedSkirmishGame : IXmlDocSerializable<br>class PlayerInfo : IXmlDocSerializable |  | save-state, XML |
| `Game/SavedSkirmishScenarioGame.cs` | 46 | Game | class SavedSkirmishScenarioGame : SavedSkirmishGame |  | XML |
| `Game/SceneRegistry.cs` | 16 | Game | enum SceneRegistry |  |  |
| `Game/Scorekeeper.cs` | 234 | Game | class Scorekeeper : NetworkBehaviour |  | NetworkBehaviour, SyncVar, NetworkServer |
| `Game/SelectionChanged.cs` | 4 | Game |  |  |  |
| `Game/Sensors/AcquisitionType.cs` | 9 | Game.Sensors | enum AcquisitionType |  |  |
| `Game/Sensors/BaseSignature.cs` | 242 | Game.Sensors | class BaseSignature : MonoBehaviour, ISignature, IRandomPointVolume | Awake, OnDestroy, OnDisable, OnEnable | prefab |
| `Game/Sensors/BoxSignature.cs` | 70 | Game.Sensors | class BoxSignature : BaseSignature | Awake | prefab |
| `Game/Sensors/CachedCrossSectionData.cs` | 11 | Game.Sensors | struct CachedCrossSectionData |  |  |
| `Game/Sensors/ConePassiveSignature.cs` | 23 | Game.Sensors | class ConePassiveSignature : PassiveSignature |  |  |
| `Game/Sensors/ContactClassification.cs` | 13 | Game.Sensors | enum ContactClassification |  |  |
| `Game/Sensors/ContactClassificationExtensions.cs` | 36 | Game.Sensors | class ContactClassificationExtensions |  |  |
| `Game/Sensors/CrossSection.cs` | 432 | Game.Sensors | class CrossSection : ScriptableObject |  |  |
| `Game/Sensors/DeltaSensor.cs` | 365 | Game.Sensors | class DeltaSensor : IDisposable where TSigType : class, ISignature |  |  |
| `Game/Sensors/DynamicPassiveSignature.cs` | 96 | Game.Sensors | class DynamicPassiveSignature : PassiveSignature | OnDestroy | prefab |
| `Game/Sensors/GainedTrack.cs` | 11 | Game.Sensors | struct GainedTrack |  |  |
| `Game/Sensors/HugeActiveSignature.cs` | 14 | Game.Sensors | class HugeActiveSignature : Signature |  |  |
| `Game/Sensors/IActiveSignature.cs` | 19 | Game.Sensors | interface IActiveSignature : ISignature |  |  |
| `Game/Sensors/IDeltaSensor.cs` | 24 | Game.Sensors | interface IDeltaSensor : ISensor, IJammable, IEWarTarget, IOwned |  |  |
| `Game/Sensors/IFastDeltaSensor.cs` | 9 | Game.Sensors | interface IFastDeltaSensor : IDeltaSensor, ISensor, IJammable, IEWarTarget, IOwned |  |  |
| `Game/Sensors/IPassiveSignature.cs` | 19 | Game.Sensors | interface IPassiveSignature : ISignature |  |  |
| `Game/Sensors/IPDTargetInfo.cs` | 13 | Game.Sensors | interface IPDTargetInfo |  |  |
| `Game/Sensors/ISensor.cs` | 39 | Game.Sensors | interface ISensor : IJammable, IEWarTarget, IOwned |  |  |
| `Game/Sensors/ISensorDetector.cs` | 13 | Game.Sensors | interface ISensorDetector |  |  |
| `Game/Sensors/ISensorProvider.cs` | 43 | Game.Sensors | interface ISensorProvider |  |  |
| `Game/Sensors/ISensorTrackable.cs` | 86 | Game.Sensors | interface ISensorTrackable |  |  |
| `Game/Sensors/ISignalEmitter.cs` | 20 | Game.Sensors | interface ISignalEmitter |  |  |
| `Game/Sensors/ISignature.cs` | 42 | Game.Sensors | interface ISignature |  |  |
| `Game/Sensors/IThreat.cs` | 24 | Game.Sensors | interface IThreat |  |  |
| `Game/Sensors/ITrack.cs` | 55 | Game.Sensors | interface ITrack : IBoardPiece |  |  |
| `Game/Sensors/ITrackProvider.cs` | 15 | Game.Sensors | interface ITrackProvider |  |  |
| `Game/Sensors/ITuneableSensor.cs` | 6 | Game.Sensors | interface ITuneableSensor |  |  |
| `Game/Sensors/JammingLOBTrack.cs` | 289 | Game.Sensors | class JammingLOBTrack : ITrack, IBoardPiece, ISensorTrackable |  |  |
| `Game/Sensors/NetworkedSensorTrack.cs` | 747 | Game.Sensors | class NetworkedSensorTrack : IDisposable<br>interface ITrackHandle : ITrack, IBoardPiece<br>class Handle : ITrackHandle, ITrack, IBoardPiece, IDisposable | Update |  |
| `Game/Sensors/OnEmitterChanged.cs` | 4 | Game.Sensors |  |  |  |
| `Game/Sensors/PassiveSignature.cs` | 51 | Game.Sensors | class PassiveSignature : BoxSignature, IPassiveSignature, ISignature |  | prefab |
| `Game/Sensors/SensorContext.cs` | 811 | Game.Sensors | class SensorContext : ITrackProvider<br>class TrackEqualityComparer : IEqualityComparer<ITrack><br>class JammingRecord<br>class SavedSensorState |  |  |
| `Game/Sensors/SensorContextChanged.cs` | 4 | Game.Sensors |  |  |  |
| `Game/Sensors/SensorContextManager.cs` | 569 | Game.Sensors | class SensorContextManager : NetworkBehaviour | Awake, OnDestroy | NetworkBehaviour, ClientRpc, NetworkServer, NetworkClient, prefab |
| `Game/Sensors/SensorDelta.cs` | 20 | Game.Sensors | struct SensorDelta |  |  |
| `Game/Sensors/SensorHost.cs` | 183 | Game.Sensors | class SensorHost : NetworkBehaviour, ISensorProvider | Awake, OnDestroy | NetworkBehaviour |
| `Game/Sensors/SensorMath.cs` | 110 | Game.Sensors | class SensorMath |  |  |
| `Game/Sensors/SensorNetwork.cs` | 264 | Game.Sensors | class SensorNetwork : ITrackProvider<br>class SensorNetMember |  |  |
| `Game/Sensors/SensorTrack.cs` | 645 | Game.Sensors | class SensorTrack : ITrack, IBoardPiece, IDisposable | Update |  |
| `Game/Sensors/SensorTrackableChanged.cs` | 4 | Game.Sensors |  |  |  |
| `Game/Sensors/SensorTrackableObject.cs` | 786 | Game.Sensors | class SensorTrackableObject : NetworkBehaviour, ISensorTrackable, IBulkSaveComponent<br>class TeamTrackingRecord<br>struct STOTrackingRecord | FixedUpdate, OnDestroy, OnDisable, OnEnable, Start | NetworkBehaviour, SyncVar, ClientRpc, NetworkServer, NetworkClient, save-state, XML, prefab |
| `Game/Sensors/SensorType.cs` | 9 | Game.Sensors | enum SensorType |  |  |
| `Game/Sensors/Signature.cs` | 223 | Game.Sensors | class Signature : BoxSignature, IActiveSignature, ISignature, IEWarTarget<br>enum DetailedOcclusionTest | OnDestroy, OnDisable, OnEnable |  |
| `Game/Sensors/SignatureChanged.cs` | 4 | Game.Sensors |  |  |  |
| `Game/Sensors/SignatureRegistry.cs` | 258 | Game.Sensors | class SignatureRegistry |  | Command |
| `Game/Sensors/SignatureType.cs` | 12 | Game.Sensors | enum SignatureType |  |  |
| `Game/Sensors/SignatureTypeExtensions.cs` | 151 | Game.Sensors | class SignatureTypeExtensions |  |  |
| `Game/Sensors/TrackChanged.cs` | 4 | Game.Sensors |  |  |  |
| `Game/Sensors/TrackedStatus.cs` | 10 | Game.Sensors | enum TrackedStatus |  |  |
| `Game/Sensors/TrackExtensions.cs` | 14 | Game.Sensors | class TrackExtensions |  |  |
| `Game/Sensors/TrackIdentifier.cs` | 88 | Game.Sensors | struct TrackIdentifier : IXmlSerializable |  |  |
| `Game/Sensors/TrackingMode.cs` | 13 | Game.Sensors | enum TrackingMode |  |  |
| `Game/Sensors/TrackingModeExtensions.cs` | 26 | Game.Sensors | class TrackingModeExtensions |  |  |
| `Game/Sensors/TrackNumberPool.cs` | 56 | Game.Sensors | class TrackNumberPool<br>class TrackPoolState |  |  |
| `Game/Sensors/TrailingPassiveSignature.cs` | 245 | Game.Sensors | class TrailingPassiveSignature : BaseSignature, IPassiveSignature, ISignature<br>class TrailPoint : IDisposable | FixedUpdate |  |
| `Game/ShipArrivalInfo.cs` | 15 | Game | struct ShipArrivalInfo |  |  |
| `Game/ShortDurationEffect.cs` | 99 | Game | class ShortDurationEffect : Poolable, ISpawnedEffect | Awake, OnRepooled, OnUnpooled, Update | pooling |
| `Game/SimpleSelectable.cs` | 90 | Game | class SimpleSelectable : MonoBehaviour, ISelectable, IOwned |  | prefab |
| `Game/SingleSwapAutoBalancer.cs` | 60 | Game | class SingleSwapAutoBalancer : GreedyAutoBalancer<TPlayer> where TPlayer : class, IAutoBalancePlayer |  |  |
| `Game/SkirmishCampaignHost.cs` | 1184 | Game | class SkirmishCampaignHost : SkirmishGameHost |  | NetworkServer, save-state, pooling, prefab |
| `Game/SkirmishCampaignLaunchOptions.cs` | 24 | Game | class SkirmishCampaignLaunchOptions : SkirmishGameLaunchOptions |  |  |
| `Game/SkirmishCampaignLaunchQuickTestOptions.cs` | 9 | Game | class SkirmishCampaignLaunchQuickTestOptions : SkirmishCampaignLaunchOptions |  |  |
| `Game/SkirmishCampaignLoadOptions.cs` | 12 | Game | class SkirmishCampaignLoadOptions : CrossScenePipe.CrossSceneData |  |  |
| `Game/SkirmishDebriefingLobbyController.cs` | 339 | Game | class SkirmishDebriefingLobbyController : GameManager | OnDestroy, Start | NetworkServer, NetworkClient, prefab |
| `Game/SkirmishDebriefingPlayer.cs` | 29 | Game | class SkirmishDebriefingPlayer : LobbyPlayer |  |  |
| `Game/SkirmishFleetTestLaunchOptions.cs` | 9 | Game | class SkirmishFleetTestLaunchOptions : SkirmishScenarioLaunchOptions |  |  |
| `Game/SkirmishGameHost.cs` | 831 | Game | class SkirmishGameHost | Update | NetworkServer, prefab |
| `Game/SkirmishGameLaunchOptions.cs` | 8 | Game | class SkirmishGameLaunchOptions : CrossScenePipe.CrossSceneData |  |  |
| `Game/SkirmishGameManager.cs` | 3451 | Game | class SkirmishGameManager : GameManager, SkirmishGameManager.ISkirmishManager, IMissionGame, IWidgetGameManager, IScriptingGraphControls, IScriptingGraphAccess<br>interface ISkirmishManager<br>class OverworldViewReturnValues | Awake, Update | NetworkServer, NetworkClient, save-state, Addressables, pooling, prefab |
| `Game/SkirmishGameSaveFile.cs` | 199 | Game | class SkirmishGameSaveFile : IXmlDocSerializable<br>class SaveSummary : IComparable<SaveSummary>, ISavedGameFile, IFileSummary |  | XML |
| `Game/SkirmishGameSettings.cs` | 379 | Game | class SkirmishGameSettings : BaseLobbySettings<br>struct TeamNameOverrides | Start | NetworkBehaviour, SyncVar, NetworkServer |
| `Game/SkirmishGameSPMissionHost.cs` | 120 | Game | class SkirmishGameSPMissionHost : SkirmishGameHost |  |  |
| `Game/SkirmishGameState.cs` | 19 | Game | enum SkirmishGameState |  |  |
| `Game/SkirmishLoadGameOptions.cs` | 9 | Game | class SkirmishLoadGameOptions : CrossScenePipe.CrossSceneData |  |  |
| `Game/SkirmishLoadManager.cs` | 263 | Game | class SkirmishLoadManager : GameManager | Start |  |
| `Game/SkirmishLobbyManager.cs` | 1522 | Game | class SkirmishLobbyManager : BaseLobbyManager<SkirmishLobbyPlayer, SkirmishGameSettings> | OnDestroy, Start | NetworkServer, NetworkClient, prefab |
| `Game/SkirmishLobbyPlayer.cs` | 637 | Game | class SkirmishLobbyPlayer : LobbyPlayer<br>struct SelectedFleetInfo<br>enum ValidationPoints<br>class Initializers | OnStartAuthority | NetworkBehaviour, SyncVar, Command, ClientRpc, NetworkServer, NetworkClient |
| `Game/SkirmishMissionLaunchOptions.cs` | 12 | Game | class SkirmishMissionLaunchOptions : SkirmishGameLaunchOptions |  |  |
| `Game/SkirmishPlayer.cs` | 1284 | Game | class SkirmishPlayer : NetworkBehaviour, IPlayer, IPlayerInfo | OnDestroy, Start | NetworkBehaviour, SyncVar, Command, ClientRpc, NetworkServer, NetworkClient, prefab |
| `Game/SkirmishPlayerDepartingHandoffData.cs` | 9 | Game | class SkirmishPlayerDepartingHandoffData : PlayerHandoffData |  |  |
| `Game/SkirmishPlayerHandoffData.cs` | 17 | Game | class SkirmishPlayerHandoffData : PlayerHandoffData |  |  |
| `Game/SkirmishScenarioHost.cs` | 173 | Game | class SkirmishScenarioHost : SkirmishGameHost |  | save-state |
| `Game/SkirmishScenarioLaunchOptions.cs` | 16 | Game | class SkirmishScenarioLaunchOptions : SkirmishGameLaunchOptions |  |  |
| `Game/SkirmishTeam.cs` | 44 | Game | class SkirmishTeam : Team<SkirmishPlayer> |  |  |
| `Game/SkirmishViewMode.cs` | 9 | Game | enum SkirmishViewMode |  |  |
| `Game/SocketPersistentEffect.cs` | 26 | Game | class SocketPersistentEffect : PersistentEffect | OnUnpooled | pooling |
| `Game/Team.cs` | 51 | Game | class Team |  |  |
| `Game/TeamAutoBalancer.cs` | 71 | Game | class TeamAutoBalancer<br>struct AutoBalanceResult |  |  |
| `Game/TimeDilationManager.cs` | 795 | Game | class TimeDilationManager : NetworkBehaviour, IPIDHost<br>enum SpeedSelection<br>enum TimeControlLimit<br>interface ITimeControlLimitHandle<br>class TimeControlLimitSource : ITimeControlLimitHandle | Awake, FixedUpdate, LateUpdate, OnStartClient | NetworkBehaviour, SyncVar, NetworkServer, prefab |
| `Game/UI/AbandonShipButton.cs` | 22 | Game.UI | class AbandonShipButton : EmergencyActionButton |  |  |
| `Game/UI/ActionMenu.cs` | 884 | Game.UI | class ActionMenu : MonoBehaviour<br>class LootListItem : MonoBehaviour | Start, Update | prefab |
| `Game/UI/ActionMenuButtonHover.cs` | 36 | Game.UI | class ActionMenuButtonHover : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler |  |  |
| `Game/UI/ActionMenuWeaponTargetingMode.cs` | 13 | Game.UI | class ActionMenuWeaponTargetingMode : MonoBehaviour |  |  |
| `Game/UI/ActiveOrderItem.cs` | 100 | Game.UI | class ActiveOrderItem : MonoBehaviour | Update | prefab |
| `Game/UI/ActiveOrderList.cs` | 113 | Game.UI | class ActiveOrderList : MonoBehaviour |  | prefab |
| `Game/UI/ArrivalCinematicMenu.cs` | 83 | Game.UI | class ArrivalCinematicMenu : BaseMenu |  | prefab |
| `Game/UI/BaseHUDMarker.cs` | 399 | Game.UI | class BaseHUDMarker : Poolable | LateUpdate, OnDestroy, OnRepooled, OnUnpooled | pooling, prefab |
| `Game/UI/BaseResourceTransferPane.cs` | 350 | Game.UI | class BaseResourceTransferPane : MonoBehaviour<br>struct ProviderGroup |  | prefab |
| `Game/UI/BaseWeaponItem.cs` | 176 | Game.UI | class BaseWeaponItem : MonoBehaviour | OnDestroy | prefab |
| `Game/UI/BattleCraftLoadoutEditorMenu.cs` | 128 | Game.UI | class BattleCraftLoadoutEditorMenu : BaseMenu, IMunitionCollection |  |  |
| `Game/UI/BattleReport/AARCraftItem.cs` | 47 | Game.UI.BattleReport | class AARCraftItem : SelectableListItem |  | prefab |
| `Game/UI/BattleReport/AARPlayerHeader.cs` | 43 | Game.UI.BattleReport | class AARPlayerHeader : MonoBehaviour |  |  |
| `Game/UI/BattleReport/AARShipItem.cs` | 53 | Game.UI.BattleReport | class AARShipItem : SelectableListItem |  | prefab |
| `Game/UI/BattleReport/BattleReportMenu.cs` | 119 | Game.UI.BattleReport | class BattleReportMenu : BaseMenu | Start | prefab |
| `Game/UI/BattleReport/CraftEngagementSubItem.cs` | 107 | Game.UI.BattleReport | class CraftEngagementSubItem : Poolable | Awake, OnRepooled | pooling, prefab |
| `Game/UI/BattleReport/CraftReportDisplay.cs` | 160 | Game.UI.BattleReport | class CraftReportDisplay : DetailedReportDisplay |  | prefab |
| `Game/UI/BattleReport/DetailedBattleReportMenu.cs` | 153 | Game.UI.BattleReport | class DetailedBattleReportMenu : BaseMenu | Awake | prefab |
| `Game/UI/BattleReport/DetailedBattleReportViewer.cs` | 200 | Game.UI.BattleReport | class DetailedBattleReportViewer : MonoBehaviour, ISelectList |  | prefab |
| `Game/UI/BattleReport/DetailedReportDisplay.cs` | 60 | Game.UI.BattleReport | class DetailedReportDisplay : MonoBehaviour |  | pooling, prefab |
| `Game/UI/BattleReport/EfficiencyGauge.cs` | 63 | Game.UI.BattleReport | class EfficiencyGauge : MonoBehaviour |  |  |
| `Game/UI/BattleReport/ShipReportDisplay.cs` | 307 | Game.UI.BattleReport | class ShipReportDisplay : DetailedReportDisplay |  | prefab |
| `Game/UI/BattleReport/SimpleReportSubItem.cs` | 32 | Game.UI.BattleReport | class SimpleReportSubItem : Poolable |  | pooling, prefab |
| `Game/UI/BattleReport/StrikeReportSubItem.cs` | 70 | Game.UI.BattleReport | class StrikeReportSubItem : Poolable |  | pooling, prefab |
| `Game/UI/BerthListItem.cs` | 114 | Game.UI | class BerthListItem : IconOnlyListItem, IClearable | Awake, OnDestroy | prefab |
| `Game/UI/CampaignEndScreen.cs` | 103 | Game.UI | class CampaignEndScreen : BaseMenu | OnDestroy | Addressables, prefab |
| `Game/UI/CampaignTACONTable.cs` | 491 | Game.UI | class CampaignTACONTable : Pulldown<br>class PlayerItem : MonoBehaviour, IClearable<br>class ShipLeader : MonoBehaviour<br>class ShipRow : MonoBehaviour, IClearable<br>class ShipColumn : MonoBehaviour | OnDestroy, OnEnable | prefab |
| `Game/UI/CaptureProgressBar.cs` | 109 | Game.UI | class CaptureProgressBar : MonoBehaviour | LateUpdate, OnDisable | prefab |
| `Game/UI/CenterCTFStatusDisplay.cs` | 107 | Game.UI | class CenterCTFStatusDisplay : ObjectiveStatusDisplay |  | prefab |
| `Game/UI/Chessboard/BorderedSlicedSphere.cs` | 57 | Game.UI.Chessboard | class BorderedSlicedSphere : SlicedSphere<br>struct BorderingPosition |  | prefab |
| `Game/UI/Chessboard/ChessboardManager.cs` | 456 | Game.UI.Chessboard | class ChessboardManager : MonoBehaviour | Awake, OnDestroy, Start, Update | pooling, prefab |
| `Game/UI/Chessboard/CraftDetailOverlay.cs` | 210 | Game.UI.Chessboard | class CraftDetailOverlay : ImmediateModeShapeDrawer | OnDisable, OnEnable, Update |  |
| `Game/UI/Chessboard/DirectionIndicator.cs` | 89 | Game.UI.Chessboard | class DirectionIndicator : ImmediateModeShapeDrawer |  |  |
| `Game/UI/Chessboard/HeightIndicator.cs` | 122 | Game.UI.Chessboard | class HeightIndicator : ImmediateModeShapeDrawer |  | prefab |
| `Game/UI/Chessboard/IEngagementWebTarget.cs` | 9 | Game.UI.Chessboard | interface IEngagementWebTarget |  |  |
| `Game/UI/Chessboard/IProjectionSphere.cs` | 20 | Game.UI.Chessboard | interface IProjectionSphere |  |  |
| `Game/UI/Chessboard/IRenderableMissileSeeker.cs` | 18 | Game.UI.Chessboard | interface IRenderableMissileSeeker |  |  |
| `Game/UI/Chessboard/Lollipop.cs` | 93 | Game.UI.Chessboard | class Lollipop : Poolable | LateUpdate, OnRepooled | pooling |
| `Game/UI/Chessboard/MaskedRangeSphere.cs` | 53 | Game.UI.Chessboard | class MaskedRangeSphere : MonoBehaviour | Awake, OnDestroy |  |
| `Game/UI/Chessboard/MaskedRangeSphereRenderer.cs` | 148 | Game.UI.Chessboard | class MaskedRangeSphereRenderer : ImmediateModeShapeDrawer | Awake | prefab |
| `Game/UI/Chessboard/MissileDetailOverlay.cs` | 183 | Game.UI.Chessboard | class MissileDetailOverlay : ImmediateModeShapeDrawer |  | prefab |
| `Game/UI/Chessboard/RadarCoverageDisplay.cs` | 79 | Game.UI.Chessboard | class RadarCoverageDisplay : ImmediateModeShapeDrawer |  |  |
| `Game/UI/Chessboard/RangeCircle.cs` | 57 | Game.UI.Chessboard | class RangeCircle : ImmediateModeShapeDrawer |  |  |
| `Game/UI/Chessboard/RangePlane.cs` | 254 | Game.UI.Chessboard | class RangePlane : SingletonMonobehaviour<RangePlane> | Awake, OnDisable, OnEnable, Update | prefab |
| `Game/UI/Chessboard/ShipDetailOverlay.cs` | 224 | Game.UI.Chessboard | class ShipDetailOverlay : MonoBehaviour | LateUpdate, OnEnable |  |
| `Game/UI/Chessboard/ShipEngagementWeb.cs` | 78 | Game.UI.Chessboard | class ShipEngagementWeb : ImmediateModeShapeDrawer<br>struct LineStyle |  |  |
| `Game/UI/Chessboard/SlicedShapeDrawer.cs` | 23 | Game.UI.Chessboard | class SlicedShapeDrawer : ImmediateModeShapeDrawer |  |  |
| `Game/UI/Chessboard/SlicedSphere.cs` | 95 | Game.UI.Chessboard | class SlicedSphere : SlicedShapeDrawer |  |  |
| `Game/UI/Chessboard/SphereLineProjector.cs` | 109 | Game.UI.Chessboard | class SphereLineProjector : ImmediateModeShapeDrawer |  |  |
| `Game/UI/Chessboard/TrackHistory.cs` | 28 | Game.UI.Chessboard | class TrackHistory : MonoBehaviour |  |  |
| `Game/UI/Chessboard/TravelRouteSlice.cs` | 320 | Game.UI.Chessboard | class TravelRouteSlice : MonoBehaviour | Awake, OnEnable | prefab |
| `Game/UI/Chessboard/TravelRouteSlicedSphere.cs` | 226 | Game.UI.Chessboard | class TravelRouteSlicedSphere : SlicedShapeDrawer<br>struct RouteInput | Awake, LateUpdate | prefab |
| `Game/UI/CommandStatusIcon.cs` | 74 | Game.UI | class CommandStatusIcon : StatusIcon |  |  |
| `Game/UI/ContainerSelector.cs` | 221 | Game.UI | class ContainerSelector : MonoBehaviour<br>class ContainerTab : MonoBehaviour, IClearable | OnDestroy | prefab |
| `Game/UI/ContainerStatusGroup.cs` | 105 | Game.UI | class ContainerStatusGroup : MonoBehaviour |  | prefab |
| `Game/UI/CraftActivityStatusIcon.cs` | 73 | Game.UI | class CraftActivityStatusIcon : StatusIcon |  |  |
| `Game/UI/CraftHangarItem.cs` | 188 | Game.UI | class CraftHangarItem : Poolable | OnRepooled | pooling, prefab |
| `Game/UI/CraftInfoBar.cs` | 568 | Game.UI | class CraftInfoBar : MonoBehaviour<br>struct GridSizeGroup<br>struct ThrottleAppearance<br>enum GroupSelectionMode | Update | pooling, prefab |
| `Game/UI/CraftInfoBarSilhouette.cs` | 149 | Game.UI | class CraftInfoBarSilhouette : Poolable, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler | Awake, OnDestroy, OnRepooled, Update | pooling, prefab |
| `Game/UI/CraftLoadoutSelectionItem.cs` | 121 | Game.UI | class CraftLoadoutSelectionItem : SelectableListItem | Awake | prefab |
| `Game/UI/CraftLoadoutSelectionList.cs` | 152 | Game.UI | class CraftLoadoutSelectionList : MonoBehaviour, ISelectList | LateUpdate | prefab |
| `Game/UI/CraftWeaponItem.cs` | 198 | Game.UI | class CraftWeaponItem : Poolable | Awake, OnDestroy, OnRepooled | pooling, prefab |
| `Game/UI/CrewStatusIcon.cs` | 23 | Game.UI | class CrewStatusIcon : StatusIcon |  |  |
| `Game/UI/CrewStatusIndicator.cs` | 75 | Game.UI | class CrewStatusIndicator : MonoBehaviour |  |  |
| `Game/UI/CriticalWarningCountdown.cs` | 102 | Game.UI | class CriticalWarningCountdown : MonoBehaviour | Awake, Update | prefab |
| `Game/UI/DamageControlBoard.cs` | 149 | Game.UI | class DamageControlBoard : ShipStatusDisplay |  | prefab |
| `Game/UI/DamageControlBoardResources.cs` | 69 | Game.UI | class DamageControlBoardResources : MonoBehaviour | Awake | prefab |
| `Game/UI/DCBoardFilters.cs` | 14 | Game.UI | enum DCBoardFilters |  |  |
| `Game/UI/DCBoardTravelExtension.cs` | 40 | Game.UI | class DCBoardTravelExtension : MonoBehaviour |  |  |
| `Game/UI/DCTeamPiece.cs` | 227 | Game.UI | class DCTeamPiece : MonoBehaviour, IClearable | Awake, OnDestroy, OnEnable | prefab |
| `Game/UI/DockedAmmoTransferPane.cs` | 20 | Game.UI | class DockedAmmoTransferPane : DockedResourceTransferPane |  |  |
| `Game/UI/DockedCraftTransferPane.cs` | 20 | Game.UI | class DockedCraftTransferPane : DockedResourceTransferPane |  |  |
| `Game/UI/DockedFuelTransferPane.cs` | 314 | Game.UI | class DockedFuelTransferPane : MonoBehaviour |  | prefab |
| `Game/UI/DockedRepairPane.cs` | 547 | Game.UI | class DockedRepairPane : MonoBehaviour<br>class ShipLockerItem : MonoBehaviour, IClearable |  | prefab |
| `Game/UI/DockedResourceTransferPane.cs` | 96 | Game.UI | class DockedResourceTransferPane : BaseResourceTransferPane | Awake |  |
| `Game/UI/DockedShipMenu.cs` | 232 | Game.UI | class DockedShipMenu : MonoBehaviour, ISelectList | Awake | prefab |
| `Game/UI/EmergencyActionButton.cs` | 77 | Game.UI | class EmergencyActionButton : MonoBehaviour | OnDestroy | prefab |
| `Game/UI/EntryVectorArrow.cs` | 163 | Game.UI | class EntryVectorArrow : MonoBehaviour | Awake, OnDestroy | prefab |
| `Game/UI/EntryVectorWidget.cs` | 125 | Game.UI | class EntryVectorWidget : EntryVectorArrow | OnDestroy, Start, Update |  |
| `Game/UI/FleetDeploymentGroupItem.cs` | 142 | Game.UI | class FleetDeploymentGroupItem : MonoBehaviour | Awake | prefab |
| `Game/UI/FQIndicator.cs` | 124 | Game.UI | class FQIndicator : MonoBehaviour | Update | prefab |
| `Game/UI/FriendlySalvoItem.cs` | 146 | Game.UI | class FriendlySalvoItem : Poolable | OnEnable, OnRepooled | pooling, prefab |
| `Game/UI/FriendlyShipItem.cs` | 293 | Game.UI | class FriendlyShipItem : MonoBehaviour, ISortableShipListItem, IClearable | OnDestroy, OnEnable | prefab |
| `Game/UI/FriendlyShipList.cs` | 213 | Game.UI | class FriendlyShipList : MonoBehaviour<br>class FriendlyUnitListComparer : IComparer<ISortableShipListItem> | OnDestroy | pooling, prefab |
| `Game/UI/FriendlySmallCraftGroupItem.cs` | 364 | Game.UI | class FriendlySmallCraftGroupItem : Poolable, ISortableShipListItem, IClearable<br>struct GridSizeGroup | Awake, OnDestroy, OnEnable, OnRepooled | pooling, prefab |
| `Game/UI/FurballTrackerItem.cs` | 286 | Game.UI | class FurballTrackerItem : Poolable | OnRepooled, Update | pooling, prefab |
| `Game/UI/GameTimeDisplay.cs` | 49 | Game.UI | class GameTimeDisplay : MonoBehaviour | Update | prefab |
| `Game/UI/GenericWeaponItem.cs` | 161 | Game.UI | class GenericWeaponItem : BaseWeaponItem |  | prefab |
| `Game/UI/GroupedFixedPieceCollection.cs` | 164 | Game.UI | class GroupedFixedPieceCollection : IBoardPieceGroup, IBoardPiece |  |  |
| `Game/UI/GroupedTrackCollection.cs` | 213 | Game.UI | class GroupedTrackCollection : IBoardPieceGroup, IBoardPiece |  |  |
| `Game/UI/HeadsUpDisplay.cs` | 690 | Game.UI | class HeadsUpDisplay : MonoBehaviour, IHeadsUpDisplay<br>enum HUDPieceType<br>class MarkerSorter : IComparer<BaseHUDMarker> | LateUpdate, OnDestroy, Start, Update | pooling, prefab |
| `Game/UI/HexMapGameplayLayer.cs` | 292 | Game.UI | class HexMapGameplayLayer : MonoBehaviour | Awake, OnEnable | NetworkServer, pooling, prefab |
| `Game/UI/HighlightManager.cs` | 119 | Game.UI | class HighlightManager : MonoBehaviour | Awake, OnDestroy | pooling, prefab |
| `Game/UI/HighlightRing.cs` | 160 | Game.UI | class HighlightRing : PoolableImmediateModeShapeDrawer | OnRepooled, Update | pooling, prefab |
| `Game/UI/HookTrackPrompt.cs` | 175 | Game.UI | class HookTrackPrompt : MonoBehaviour | Awake, Update | prefab |
| `Game/UI/HUDCalloutMarker.cs` | 46 | Game.UI | class HUDCalloutMarker : BaseHUDMarker<br>struct CalloutSymbol |  |  |
| `Game/UI/HUDDangerMarker.cs` | 54 | Game.UI | class HUDDangerMarker : Poolable | OnRepooled, OnUnpooled | pooling |
| `Game/UI/HUDDriveFlareMarker.cs` | 104 | Game.UI | class HUDDriveFlareMarker : BaseHUDMarker | OnEnable | prefab |
| `Game/UI/HUDGroupedSensorMarker.cs` | 151 | Game.UI | class HUDGroupedSensorMarker : HUDSensorMarker | LateUpdate | prefab |
| `Game/UI/HUDMarkerBadge.cs` | 23 | Game.UI | class HUDMarkerBadge : Poolable | OnRepooled | pooling |
| `Game/UI/HUDSensorMarker.cs` | 328 | Game.UI | class HUDSensorMarker : BaseHUDMarker, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler | OnRepooled | pooling, prefab |
| `Game/UI/IBoardPiece.cs` | 90 | Game.UI | interface IBoardPiece |  |  |
| `Game/UI/IBoardPieceGroup.cs` | 15 | Game.UI | interface IBoardPieceGroup : IBoardPiece |  |  |
| `Game/UI/IHeadsUpDisplay.cs` | 27 | Game.UI | interface IHeadsUpDisplay |  |  |
| `Game/UI/ImmediateInventoryTransferItem.cs` | 182 | Game.UI | class ImmediateInventoryTransferItem : Poolable<br>struct SideGroup<br>class ItemComparer : IComparer<ImmediateInventoryTransferItem> | Awake, OnRepooled | pooling, prefab |
| `Game/UI/InventoryTransferItem.cs` | 411 | Game.UI | class InventoryTransferItem : Poolable<br>struct SideGroup<br>class ItemComparer : IComparer<InventoryTransferItem> | Awake, OnRepooled | pooling, prefab |
| `Game/UI/ISelectableMissile.cs` | 36 | Game.UI | interface ISelectableMissile : ISelectable, IOwned |  |  |
| `Game/UI/ISortableShipListItem.cs` | 18 | Game.UI | interface ISortableShipListItem |  |  |
| `Game/UI/LampStatusIcon.cs` | 29 | Game.UI | class LampStatusIcon : StatusIcon |  |  |
| `Game/UI/MarkerType.cs` | 23 | Game.UI | enum MarkerType |  |  |
| `Game/UI/MessageLogItem.cs` | 26 | Game.UI | class MessageLogItem : MonoBehaviour |  |  |
| `Game/UI/MissileGlanceBar.cs` | 95 | Game.UI | class MissileGlanceBar : MonoBehaviour | Awake, OnDestroy, Update | prefab |
| `Game/UI/MissilePhaseIcon.cs` | 90 | Game.UI | class MissilePhaseIcon : StatusIcon |  |  |
| `Game/UI/MissileWeaponItem.cs` | 138 | Game.UI | class MissileWeaponItem : BaseWeaponItem | Awake, OnDestroy | prefab |
| `Game/UI/MixedSalvoDisplay.cs` | 39 | Game.UI | class MixedSalvoDisplay : MonoBehaviour |  |  |
| `Game/UI/ModalMessageLog.cs` | 30 | Game.UI | class ModalMessageLog : BaseMenu | Awake | prefab |
| `Game/UI/ModularDCBoardBuilder.cs` | 71 | Game.UI | class ModularDCBoardBuilder : ModularStatusDisplayBuilder<br>struct DetailGroupBinding |  |  |
| `Game/UI/ModularStatusDisplayBuilder.cs` | 65 | Game.UI | class ModularStatusDisplayBuilder : MonoBehaviour<br>struct SegmentBuildInfo |  | prefab |
| `Game/UI/ModularStatusDisplayPart.cs` | 24 | Game.UI | class ModularStatusDisplayPart : MonoBehaviour |  |  |
| `Game/UI/MountStatus.cs` | 224 | Game.UI | class MountStatus : Poolable<br>struct StatusAppearance | OnDestroy, OnEnable, OnRepooled, Update | pooling, prefab |
| `Game/UI/MountStatusDisplay.cs` | 53 | Game.UI | class MountStatusDisplay : MonoBehaviour | OnDestroy | pooling, prefab |
| `Game/UI/MovementStatusIcon.cs` | 100 | Game.UI | class MovementStatusIcon : StatusIcon |  | prefab |
| `Game/UI/MultiCaptureStatusDisplay.cs` | 81 | Game.UI | class MultiCaptureStatusDisplay : ObjectiveStatusDisplay |  | prefab |
| `Game/UI/ObjectiveItem.cs` | 207 | Game.UI | class ObjectiveItem : MonoBehaviour<br>struct Appearance<br>class SubObjectiveItem : MonoBehaviour | Awake, OnDestroy, Update | prefab |
| `Game/UI/ObjectiveList.cs` | 130 | Game.UI | class ObjectiveList : MonoBehaviour | Awake, OnDestroy | prefab |
| `Game/UI/ObjectiveNotifications.cs` | 298 | Game.UI | class ObjectiveNotifications : MonoBehaviour<br>class Notification<br>class NewObjectiveNotification : Notification<br>class CompletedObjectiveNotification : Notification<br>class UpdatedObjectiveNotification : Notification<br>class TimerNotification : Notification | OnDisable, OnEnable | prefab |
| `Game/UI/ObjectiveStatusDisplay.cs` | 9 | Game.UI | class ObjectiveStatusDisplay : MonoBehaviour |  |  |
| `Game/UI/OrderFeedbackLineRenderer.cs` | 62 | Game.UI | class OrderFeedbackLineRenderer : PoolableImmediateModeShapeDrawer | OnRepooled, Update | pooling |
| `Game/UI/OrderIcon.cs` | 12 | Game.UI | enum OrderIcon |  |  |
| `Game/UI/OverworldShipItem.cs` | 66 | Game.UI | class OverworldShipItem : MonoBehaviour, IClearable | OnDestroy | prefab |
| `Game/UI/OverworldTravelMenu.cs` | 335 | Game.UI | class OverworldTravelMenu : BaseMenu, IStatusProgress<float>, IProgress<float> | Awake | prefab |
| `Game/UI/PercentageStatusText.cs` | 57 | Game.UI | class PercentageStatusText : MonoBehaviour<br>struct Step |  |  |
| `Game/UI/PieceGroupingChanged.cs` | 4 | Game.UI |  |  |  |
| `Game/UI/PlayerDeploymentReadyIndicator.cs` | 31 | Game.UI | class PlayerDeploymentReadyIndicator : MonoBehaviour |  |  |
| `Game/UI/PlayerList.cs` | 50 | Game.UI | class PlayerList : MonoBehaviour | Awake | prefab |
| `Game/UI/PlayerListItem.cs` | 76 | Game.UI | class PlayerListItem : MonoBehaviour |  | prefab |
| `Game/UI/QuantityStatusIcon.cs` | 146 | Game.UI | class QuantityStatusIcon : StatusIcon | Awake, OnDestroy |  |
| `Game/UI/RightClickContextIcons.cs` | 133 | Game.UI | class RightClickContextIcons : MonoBehaviour<br>struct ActionIconAppearance<br>struct ActionIcon<br>enum ActionType | Awake, Update | prefab |
| `Game/UI/SalvageShipPane.cs` | 55 | Game.UI | class SalvageShipPane : MonoBehaviour |  | prefab |
| `Game/UI/ScramReactorButton.cs` | 65 | Game.UI | class ScramReactorButton : EmergencyActionButton | Update | prefab |
| `Game/UI/SegmentedBar.cs` | 133 | Game.UI | class SegmentedBar : MonoBehaviour | Awake | prefab |
| `Game/UI/ShipFlightControlMenu.cs` | 636 | Game.UI | class ShipFlightControlMenu : MonoBehaviour, ISelectList<br>class PadIndicator : MonoBehaviour | Awake, OnDisable | prefab |
| `Game/UI/ShipGlanceBar.cs` | 176 | Game.UI | class ShipGlanceBar : MonoBehaviour | Awake, OnDestroy, Update | prefab |
| `Game/UI/ShipInfoBar.cs` | 622 | Game.UI | class ShipInfoBar : MonoBehaviour | Awake, Update | prefab |
| `Game/UI/ShipStatusDetailGroup.cs` | 54 | Game.UI | class ShipStatusDetailGroup : MonoBehaviour |  | prefab |
| `Game/UI/ShipStatusDetailPart.cs` | 241 | Game.UI | class ShipStatusDetailPart : MonoBehaviour, IClearable | OnDestroy, Update | prefab |
| `Game/UI/ShipStatusDetailPartTravel.cs` | 26 | Game.UI | class ShipStatusDetailPartTravel : ShipStatusDetailPart | Awake | prefab |
| `Game/UI/ShipStatusDisplay.cs` | 144 | Game.UI | class ShipStatusDisplay : MonoBehaviour, IClearable | OnDestroy | prefab |
| `Game/UI/ShipStatusDisplayPart.cs` | 247 | Game.UI | class ShipStatusDisplayPart : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IClearable | OnDestroy, OnEnable | prefab |
| `Game/UI/ShipStatusIconGroup.cs` | 343 | Game.UI | class ShipStatusIconGroup : MonoBehaviour |  |  |
| `Game/UI/SingleBoardPieceGroup.cs` | 165 | Game.UI | class SingleBoardPieceGroup : IBoardPieceGroup, IBoardPiece |  |  |
| `Game/UI/SingleCaptureStatusDisplay.cs` | 22 | Game.UI | class SingleCaptureStatusDisplay : ObjectiveStatusDisplay |  |  |
| `Game/UI/SkirmishEscapeMenu.cs` | 45 | Game.UI | class SkirmishEscapeMenu : BaseMenu |  | prefab |
| `Game/UI/SkirmishUIDeployment.cs` | 503 | Game.UI | class SkirmishUIDeployment : BaseMenu, IHeadsUpDisplay |  | pooling, prefab |
| `Game/UI/SkirmishUIMain.cs` | 421 | Game.UI | class SkirmishUIMain : BaseMenu<br>class WarningMessage | Awake, OnEnable, Update | prefab |
| `Game/UI/SkirmishUIOverworld.cs` | 86 | Game.UI | class SkirmishUIOverworld : MonoBehaviour |  | prefab |
| `Game/UI/SmallCraftFuelBar.cs` | 108 | Game.UI | class SmallCraftFuelBar : MonoBehaviour | OnEnable, Update | prefab |
| `Game/UI/SpectatorCraftItem.cs` | 127 | Game.UI | class SpectatorCraftItem : Poolable | OnDestroy, OnRepooled | pooling, prefab |
| `Game/UI/SpectatorHUDMarker.cs` | 109 | Game.UI | class SpectatorHUDMarker : IBoardPiece |  |  |
| `Game/UI/SpectatorPlayerListItem.cs` | 47 | Game.UI | class SpectatorPlayerListItem : MonoBehaviour |  | prefab |
| `Game/UI/SpectatorShipItem.cs` | 137 | Game.UI | class SpectatorShipItem : MonoBehaviour | OnDestroy | prefab |
| `Game/UI/SpectatorShipList.cs` | 43 | Game.UI | class SpectatorShipList : MonoBehaviour |  | prefab |
| `Game/UI/StatusDisplaySelector.cs` | 118 | Game.UI | class StatusDisplaySelector : MonoBehaviour | OnDestroy, Start | prefab |
| `Game/UI/StatusIcon.cs` | 155 | Game.UI | class StatusIcon : MonoBehaviour<br>struct FlashRecord | OnEnable | prefab |
| `Game/UI/StoredCraftItem.cs` | 202 | Game.UI | class StoredCraftItem : Poolable | Awake, OnRepooled | pooling, prefab |
| `Game/UI/StreamlinedWeaponAmmoButton.cs` | 163 | Game.UI | class StreamlinedWeaponAmmoButton : MonoBehaviour | Awake, OnDestroy | prefab |
| `Game/UI/StreamlinedWeaponItem.cs` | 105 | Game.UI | class StreamlinedWeaponItem : BaseWeaponItem | Awake | prefab |
| `Game/UI/TextStatusIcon.cs` | 53 | Game.UI | class TextStatusIcon : StatusIcon |  |  |
| `Game/UI/TrackedStatusIcon.cs` | 89 | Game.UI | class TrackedStatusIcon : StatusIcon |  |  |
| `Game/UI/TransferSegmentedBar.cs` | 96 | Game.UI | class TransferSegmentedBar : SegmentedBar |  |  |
| `Game/UI/TransferStatusIcon.cs` | 156 | Game.UI | class TransferStatusIcon : StatusIcon | Update |  |
| `Game/UI/TravelRepairsMenu.cs` | 155 | Game.UI | class TravelRepairsMenu : MonoBehaviour |  | prefab |
| `Game/UI/TravelReplenishmentMenu.cs` | 408 | Game.UI | class TravelReplenishmentMenu : MonoBehaviour | Awake | prefab |
| `Game/UI/TravelReplenishmentShipItem.cs` | 81 | Game.UI | class TravelReplenishmentShipItem : SelectableListItem, IClearable |  |  |
| `Game/UI/TwoCTFStatusDisplay.cs` | 89 | Game.UI | class TwoCTFStatusDisplay : ObjectiveStatusDisplay |  | prefab |
| `Game/UI/UIDisplayMode.cs` | 9 | Game.UI | enum UIDisplayMode |  |  |
| `Game/UI/VariableActionButton.cs` | 63 | Game.UI | class VariableActionButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler<br>struct ActionItem |  |  |
| `Game/UI/VariableActionMenu.cs` | 68 | Game.UI | class VariableActionMenu : MonoBehaviour | Update | prefab |
| `Game/UI/WeaponSelectList.cs` | 352 | Game.UI | class WeaponSelectList : MonoBehaviour |  | prefab |
| `Game/Units/ShipController.cs` | 8709 | Game.Units | class ShipController : NetworkBehaviour, IOwned, IPlatformController, INetIdentifiedScript, IStorageContainerProvider, IDockingProvider, IBoardPiece, ISelectabl...<br>class LoadedOrders<br>class WarpCoroutine : SaveableCoroutine<br>class WarpArriveCoroutine : WarpCoroutine<br>class WarpDepartCoroutine : WarpCoroutine | Awake, FixedUpdate, OnDestroy, OnStartClient, Update | NetworkBehaviour, SyncVar, ClientRpc, NetworkServer, NetworkClient, save-state, Addressables, pooling, prefab |
| `Game/Units/ShipDataRecorder.cs` | 224 | Game.Units | class ShipDataRecorder : MonoBehaviour<br>class EngagementRecord | OnDestroy | prefab |
| `Game/WaitingOperationType.cs` | 14 | Game | enum WaitingOperationType |  |  |
| `Game/WaypointPath.cs` | 178 | Game | class WaypointPath<br>class Waypoint<br>class SavedWaypointPath |  |  |
| `Mirror/GeneratedNetworkCode.cs` | 2405 | Mirror | class GeneratedNetworkCode |  | NetworkBehaviour, prefab |
| `Missions/Character.cs` | 46 | Missions | class Character : ScriptableObject |  |  |
| `Missions/CharacterMood.cs` | 9 | Missions | enum CharacterMood |  |  |
| `Missions/IMissionGame.cs` | 48 | Missions | interface IMissionGame |  | prefab |
| `Missions/IStateNode.cs` | 11 | Missions | interface IStateNode |  |  |
| `Missions/Mission.cs` | 79 | Missions | class Mission : ScriptableObject |  | Addressables |
| `Missions/MissionGraph.cs` | 149 | Missions | class MissionGraph : NodeGraph |  |  |
| `Missions/MissionOutcome.cs` | 9 | Missions | enum MissionOutcome |  |  |
| `Missions/MissionSet.cs` | 38 | Missions | class MissionSet : ScriptableObject, IModSource |  |  |
| `Missions/Nodes/AccessArrayNode.cs` | 35 | Missions.Nodes | class AccessArrayNode : Node |  |  |
| `Missions/Nodes/AccessVectorArrayNode.cs` | 36 | Missions.Nodes | class AccessVectorArrayNode : Node |  |  |
| `Missions/Nodes/AccumulateChange.cs` | 59 | Missions.Nodes | class AccumulateChange : BaseMissionNode |  |  |
| `Missions/Nodes/ActionMenuDetails.cs` | 32 | Missions.Nodes | class ActionMenuDetails : Node |  |  |
| `Missions/Nodes/ActiveWidgetInfoNode.cs` | 42 | Missions.Nodes | class ActiveWidgetInfoNode : BaseMissionNode |  |  |
| `Missions/Nodes/BaseEquals.cs` | 27 | Missions.Nodes | class BaseEquals : Node |  |  |
| `Missions/Nodes/BaseMissionNode.cs` | 27 | Missions.Nodes | class BaseMissionNode : Node |  |  |
| `Missions/Nodes/BattlespaceDetails.cs` | 59 | Missions.Nodes | class BattlespaceDetails : Node |  |  |
| `Missions/Nodes/CameraStatus.cs` | 43 | Missions.Nodes | class CameraStatus : BaseMissionNode |  |  |
| `Missions/Nodes/CapturePointOwnedBy.cs` | 28 | Missions.Nodes | class CapturePointOwnedBy : Node |  |  |
| `Missions/Nodes/CombineVector.cs` | 29 | Missions.Nodes | class CombineVector : Node |  |  |
| `Missions/Nodes/CountBools.cs` | 45 | Missions.Nodes | class CountBools : Node |  |  |
| `Missions/Nodes/CountOwnedCapturePointsNode.cs` | 38 | Missions.Nodes | class CountOwnedCapturePointsNode : Node |  |  |
| `Missions/Nodes/CraftActivityEquals.cs` | 34 | Missions.Nodes | class CraftActivityEquals : Node |  |  |
| `Missions/Nodes/CraftGroupDetails.cs` | 116 | Missions.Nodes | class CraftGroupDetails : Node |  |  |
| `Missions/Nodes/CraftGroupHasLoadout.cs` | 41 | Missions.Nodes | class CraftGroupHasLoadout : Node |  |  |
| `Missions/Nodes/CraftGuardTargetEquals.cs` | 37 | Missions.Nodes | class CraftGuardTargetEquals : Node |  |  |
| `Missions/Nodes/Distance.cs` | 28 | Missions.Nodes | class Distance : Node |  |  |
| `Missions/Nodes/EMCONSetNode.cs` | 36 | Missions.Nodes | class EMCONSetNode : Node |  |  |
| `Missions/Nodes/FloatComparison.cs` | 58 | Missions.Nodes | class FloatComparison : Node<br>enum Operation |  |  |
| `Missions/Nodes/FloatMath.cs` | 57 | Missions.Nodes | class FloatMath : Node<br>enum Operation |  |  |
| `Missions/Nodes/Flow/BranchNode.cs` | 27 | Missions.Nodes.Flow | class BranchNode : MissionFlowNode |  |  |
| `Missions/Nodes/Flow/ChainedNode.cs` | 23 | Missions.Nodes.Flow | class ChainedNode : MissionFlowNode |  |  |
| `Missions/Nodes/Flow/ConditionalBranchNode.cs` | 21 | Missions.Nodes.Flow | class ConditionalBranchNode : BranchNode |  |  |
| `Missions/Nodes/Flow/DeadEndNode.cs` | 16 | Missions.Nodes.Flow | class DeadEndNode : MissionFlowNode |  |  |
| `Missions/Nodes/Flow/EvaluateTriggeredSequencesNode.cs` | 78 | Missions.Nodes.Flow | class EvaluateTriggeredSequencesNode : ChainedNode |  |  |
| `Missions/Nodes/Flow/ExecuteSequenceNode.cs` | 63 | Missions.Nodes.Flow | class ExecuteSequenceNode : ChainedNode |  |  |
| `Missions/Nodes/Flow/MissionEndNode.cs` | 34 | Missions.Nodes.Flow | class MissionEndNode : MissionFlowNode |  |  |
| `Missions/Nodes/Flow/MissionFlowNode.cs` | 7 | Missions.Nodes.Flow | class MissionFlowNode : BaseMissionNode |  |  |
| `Missions/Nodes/Flow/MissionStartNode.cs` | 140 | Missions.Nodes.Flow | class MissionStartNode : MissionFlowNode<br>class AddPlayer<br>class AddBotPlayer : AddPlayer |  |  |
| `Missions/Nodes/Flow/TriggeredSequenceNode.cs` | 106 | Missions.Nodes.Flow | class TriggeredSequenceNode : BaseMissionNode |  |  |
| `Missions/Nodes/Flow/WaitForConditionNode.cs` | 23 | Missions.Nodes.Flow | class WaitForConditionNode : ChainedNode |  |  |
| `Missions/Nodes/GameTimeNode.cs` | 46 | Missions.Nodes | class GameTimeNode : Node |  |  |
| `Missions/Nodes/GetCraftFlight.cs` | 50 | Missions.Nodes | class GetCraftFlight : Node |  |  |
| `Missions/Nodes/GetEnemyPlayer.cs` | 43 | Missions.Nodes | class GetEnemyPlayer : BaseMissionNode |  |  |
| `Missions/Nodes/GetFleetShip.cs` | 34 | Missions.Nodes | class GetFleetShip : Node |  |  |
| `Missions/Nodes/IntComparison.cs` | 58 | Missions.Nodes | class IntComparison : Node<br>enum Operation |  |  |
| `Missions/Nodes/IntMath.cs` | 57 | Missions.Nodes | class IntMath : Node<br>enum Operation |  |  |
| `Missions/Nodes/IUpdateNode.cs` | 7 | Missions.Nodes | interface IUpdateNode | Update |  |
| `Missions/Nodes/KeyedNode.cs` | 21 | Missions.Nodes | class KeyedNode : Node |  |  |
| `Missions/Nodes/LocalPlayerNode.cs` | 24 | Missions.Nodes | class LocalPlayerNode : Node |  |  |
| `Missions/Nodes/Logic.cs` | 60 | Missions.Nodes | class Logic : Node<br>enum Operation |  |  |
| `Missions/Nodes/LogicInvert.cs` | 22 | Missions.Nodes | class LogicInvert : Node |  |  |
| `Missions/Nodes/LogicLatch.cs` | 44 | Missions.Nodes | class LogicLatch : Node<br>enum DesiredState |  |  |
| `Missions/Nodes/MakeNameNode.cs` | 63 | Missions.Nodes | class MakeNameNode : Node<br>enum NameStyle |  |  |
| `Missions/Nodes/MoveOrderEquals.cs` | 28 | Missions.Nodes | class MoveOrderEquals : Node |  |  |
| `Missions/Nodes/OrderListDetails.cs` | 34 | Missions.Nodes | class OrderListDetails : Node |  |  |
| `Missions/Nodes/PlayerDetails.cs` | 36 | Missions.Nodes | class PlayerDetails : Node |  |  |
| `Missions/Nodes/POIDetails.cs` | 33 | Missions.Nodes | class POIDetails : Node |  |  |
| `Missions/Nodes/RangeNode.cs` | 26 | Missions.Nodes | class RangeNode : Node |  |  |
| `Missions/Nodes/Scenario/IntegerOptionNode.cs` | 82 | Missions.Nodes.Scenario | class IntegerOptionNode : ScenarioOptionNode<br>class SavedIntegerOptionNode : SavedNodeState |  |  |
| `Missions/Nodes/Scenario/IntegerSliderOptionNode.cs` | 96 | Missions.Nodes.Scenario | class IntegerSliderOptionNode : ScenarioOptionNode<br>class SavedIntegerSliderOptionNode : SavedNodeState |  |  |
| `Missions/Nodes/Scenario/ScenarioMasterNode.cs` | 43 | Missions.Nodes.Scenario | class ScenarioMasterNode : Node |  |  |
| `Missions/Nodes/Scenario/ScenarioOptionNode.cs` | 33 | Missions.Nodes.Scenario | class ScenarioOptionNode : KeyedNode, IStateNode |  |  |
| `Missions/Nodes/Scenario/ScenarioSetupNode.cs` | 45 | Missions.Nodes.Scenario | class ScenarioSetupNode : Node |  |  |
| `Missions/Nodes/Scenario/ScenarioUpdateNode.cs` | 70 | Missions.Nodes.Scenario | class ScenarioUpdateNode : BaseMissionNode, IStateNode<br>class SavedScenarioUpdateNode : SavedNodeState |  |  |
| `Missions/Nodes/Sequenced/AccumulateInt.cs` | 36 | Missions.Nodes.Sequenced | class AccumulateInt : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/AccumulateScoreNode.cs` | 76 | Missions.Nodes.Sequenced | class AccumulateScoreNode : SequencedNode, IStateNode<br>class SavedAccumulatedScoreNode : SavedNodeState |  |  |
| `Missions/Nodes/Sequenced/BotPausedNode.cs` | 25 | Missions.Nodes.Sequenced | class BotPausedNode : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/BuildArrayNode.cs` | 46 | Missions.Nodes.Sequenced | class BuildArrayNode : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/ClearAllObjectives.cs` | 11 | Missions.Nodes.Sequenced | class ClearAllObjectives : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/CompleteObjective.cs` | 22 | Missions.Nodes.Sequenced | class CompleteObjective : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/CreateCapturePoint.cs` | 119 | Missions.Nodes.Sequenced | class CreateCapturePoint : SequencedNode, IStateNode<br>class SavedCapturePointNode : SavedNodeState |  | prefab |
| `Missions/Nodes/Sequenced/CreateFlagStandPoint.cs` | 132 | Missions.Nodes.Sequenced | class CreateFlagStandPoint : SequencedNode, IStateNode<br>class SavedFlagStandPointNode : SavedNodeState |  | prefab |
| `Missions/Nodes/Sequenced/CreateObjective.cs` | 43 | Missions.Nodes.Sequenced | class CreateObjective : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/CreatePointOfInterest.cs` | 55 | Missions.Nodes.Sequenced | class CreatePointOfInterest : SequencedNode |  | prefab |
| `Missions/Nodes/Sequenced/DamageShipComponent.cs` | 45 | Missions.Nodes.Sequenced | class DamageShipComponent : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/DialogSequence.cs` | 64 | Missions.Nodes.Sequenced | class DialogSequence : SequencedNode<br>struct Line |  |  |
| `Missions/Nodes/Sequenced/DialogWithOptions.cs` | 70 | Missions.Nodes.Sequenced | class DialogWithOptions : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/FireMissilesAtShip.cs` | 76 | Missions.Nodes.Sequenced | class FireMissilesAtShip : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/MarkShipCallout.cs` | 29 | Missions.Nodes.Sequenced | class MarkShipCallout : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/MarkTime.cs` | 34 | Missions.Nodes.Sequenced | class MarkTime : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/OrderMoveShip.cs` | 55 | Missions.Nodes.Sequenced | class OrderMoveShip : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/OrderShipFirePosition.cs` | 60 | Missions.Nodes.Sequenced | class OrderShipFirePosition : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/OrderShipFormation.cs` | 59 | Missions.Nodes.Sequenced | class OrderShipFormation : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/PlaceCallout.cs` | 34 | Missions.Nodes.Sequenced | class PlaceCallout : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/RemovePointOfInterest.cs` | 26 | Missions.Nodes.Sequenced | class RemovePointOfInterest : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/SequencedNode.cs` | 27 | Missions.Nodes.Sequenced | class SequencedNode : BaseMissionNode |  |  |
| `Missions/Nodes/Sequenced/SequenceLoop.cs` | 63 | Missions.Nodes.Sequenced | class SequenceLoop : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/SetCapturePointLocked.cs` | 24 | Missions.Nodes.Sequenced | class SetCapturePointLocked : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/SetCraftAttackDestroyedTargets.cs` | 15 | Missions.Nodes.Sequenced | class SetCraftAttackDestroyedTargets : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/SetDynamicMusic.cs` | 16 | Missions.Nodes.Sequenced | class SetDynamicMusic : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/SetHighlightedUIElement.cs` | 27 | Missions.Nodes.Sequenced | class SetHighlightedUIElement : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/SetObjectiveOverviewUI.cs` | 17 | Missions.Nodes.Sequenced | class SetObjectiveOverviewUI : SequencedNode |  | prefab |
| `Missions/Nodes/Sequenced/SetPlayerLimitedActions.cs` | 26 | Missions.Nodes.Sequenced | class SetPlayerLimitedActions : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/SetShipDisableLifeboatLaunch.cs` | 25 | Missions.Nodes.Sequenced | class SetShipDisableLifeboatLaunch : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/SetShipEMCONStatus.cs` | 26 | Missions.Nodes.Sequenced | class SetShipEMCONStatus : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/SetShipPointDefenseDoctrine.cs` | 66 | Missions.Nodes.Sequenced | class SetShipPointDefenseDoctrine : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/SetShipPositions.cs` | 35 | Missions.Nodes.Sequenced | class SetShipPositions : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/SetShipWCONStatus.cs` | 26 | Missions.Nodes.Sequenced | class SetShipWCONStatus : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/SetupScorekeeperNode.cs` | 22 | Missions.Nodes.Sequenced | class SetupScorekeeperNode : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/ToggleShipBattleshort.cs` | 25 | Missions.Nodes.Sequenced | class ToggleShipBattleshort : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/ToggleShipDamageControl.cs` | 25 | Missions.Nodes.Sequenced | class ToggleShipDamageControl : SequencedNode |  |  |
| `Missions/Nodes/Sequenced/WaitTime.cs` | 31 | Missions.Nodes.Sequenced | class WaitTime : SequencedNode |  |  |
| `Missions/Nodes/ShipDamageDetails.cs` | 60 | Missions.Nodes | class ShipDamageDetails : Node |  |  |
| `Missions/Nodes/ShipDetails.cs` | 116 | Missions.Nodes | class ShipDetails : Node<br>struct WeaponGroupPasser |  |  |
| `Missions/Nodes/ShipFlightDetails.cs` | 51 | Missions.Nodes | class ShipFlightDetails : Node |  |  |
| `Missions/Nodes/ShipGeneralDetails.cs` | 43 | Missions.Nodes | class ShipGeneralDetails : Node |  |  |
| `Missions/Nodes/ShipMovementDetails.cs` | 71 | Missions.Nodes | class ShipMovementDetails : Node |  |  |
| `Missions/Nodes/ShipShortDetails.cs` | 75 | Missions.Nodes | class ShipShortDetails : Node |  |  |
| `Missions/Nodes/SkirmishUIDetails.cs` | 40 | Missions.Nodes | class SkirmishUIDetails : Node |  |  |
| `Missions/Nodes/SplitVector.cs` | 31 | Missions.Nodes | class SplitVector : Node |  |  |
| `Missions/Nodes/StopwatchNode.cs` | 45 | Missions.Nodes | class StopwatchNode : BaseMissionNode, IUpdateNode |  |  |
| `Missions/Nodes/StringEquals.cs` | 6 | Missions.Nodes | class StringEquals : BaseEquals<string> |  |  |
| `Missions/Nodes/TeamDetails.cs` | 47 | Missions.Nodes | class TeamDetails : Node |  |  |
| `Missions/Nodes/WeaponGroupDetails.cs` | 93 | Missions.Nodes | class WeaponGroupDetails : BaseMissionNode<br>enum GetMethod |  |  |
| `Missions/SavedNodeState.cs` | 17 | Missions | class SavedNodeState |  |  |
| `Missions/ScenarioGraph.cs` | 264 | Missions | class ScenarioGraph : NodeGraph, IModSource<br>class ScenarioSaveData<br>struct AITip |  |  |
| `Modding/IModDependencySource.cs` | 9 | Modding | interface IModDependencySource |  |  |
| `Modding/IModEntryPoint.cs` | 9 | Modding | interface IModEntryPoint |  |  |
| `Modding/IModRecord.cs` | 17 | Modding | interface IModRecord |  |  |
| `Modding/IModSource.cs` | 7 | Modding | interface IModSource |  |  |
| `Modding/ModDatabase.cs` | 477 | Modding | class ModDatabase<br>struct ModDownloadReport<br>struct PersistentActiveMods |  | XML |
| `Modding/ModDependencyGraph.cs` | 73 | Modding | class ModDependencyGraph |  |  |
| `Modding/ModDependencyHelpers.cs` | 93 | Modding | class ModDependencyHelpers |  |  |
| `Modding/ModInfo.cs` | 219 | Modding | class ModInfo : ISteamWorkshopItem |  | XML |
| `Modding/ModLoadResult.cs` | 10 | Modding | enum ModLoadResult |  |  |
| `Modding/ModRecord.cs` | 350 | Modding | class ModRecord |  | AssetBundle, Addressables |
| `Modding/ModSourceExtensions.cs` | 16 | Modding | class ModSourceExtensions |  |  |
| `Munitions/ActiveMissileSalvo.cs` | 206 | Munitions | class ActiveMissileSalvo : IActiveMissileSalvo, IWeaponStatReportReceiver<br>enum MissileStatus<br>class MissileRecord |  |  |
| `Munitions/ActiveSeeker.cs` | 540 | Munitions | class ActiveSeeker : MissileSeeker, ISensor, IJammable, IEWarTarget, IOwned, ITuneableSensor<br>struct WeightedSigHit | Awake, OnDisable, OnEnable | NetworkBehaviour, ClientRpc, NetworkServer, NetworkClient, XML |
| `Munitions/AirburstDecoy.cs` | 25 | Munitions | class AirburstDecoy : AirburstRocket |  | prefab |
| `Munitions/AirburstRocket.cs` | 214 | Munitions | class AirburstRocket : LegacyMissile | FixedUpdate | NetworkBehaviour, SyncVar, XML, prefab |
| `Munitions/AirburstShell.cs` | 44 | Munitions | class AirburstShell : ShellMunition, ITimeDelayFuzed |  |  |
| `Munitions/AOEExplosionEffectModule.cs` | 291 | Munitions | class AOEExplosionEffectModule : MonoBehaviour, IDamageDealer, IDamageCharacteristic, IEffectModule, ILocalImbued, IOwned |  | prefab |
| `Munitions/AvailableMunitionsSet.cs` | 361 | Munitions | class AvailableMunitionsSet : IMunitionCollection |  | prefab |
| `Munitions/CommandGuidedSeeker.cs` | 176 | Munitions | class CommandGuidedSeeker : MissileSeeker | Awake | XML |
| `Munitions/CruiseMissile.cs` | 500 | Munitions | class CruiseMissile : GuidedMissile, IProgrammableMissile, IMissile, IImbued, IOwned, ILocalImbued, IMunition, IStorable, ISaveKeyed, IModSource, IFactionLocke...<br>enum FlightPhase | Awake, FixedUpdate, OnDestroy, OnRepooled | NetworkBehaviour, ClientRpc, NetworkServer, NetworkClient, XML, pooling, prefab |
| `Munitions/DamageSpreadingMethod.cs` | 8 | Munitions | enum DamageSpreadingMethod |  |  |
| `Munitions/DecoyCapabilities.cs` | 38 | Munitions | struct DecoyCapabilities |  |  |
| `Munitions/DirectGuidedMissile.cs` | 341 | Munitions | class DirectGuidedMissile : GuidedMissile, IProgrammableMissile, IMissile, IImbued, IOwned, ILocalImbued, IMunition, IStorable, ISaveKeyed, IModSource, IFactionLocke...<br>enum FlightPhase | Awake, FixedUpdate, OnDestroy, OnRepooled | NetworkBehaviour, ClientRpc, NetworkServer, NetworkClient, XML, pooling |
| `Munitions/DumbfireRocket.cs` | 180 | Munitions | class DumbfireRocket : LegacyMissile | FixedUpdate, OnRepooled, OnUnpooled | NetworkBehaviour, SyncVar, XML, pooling |
| `Munitions/ExplosiveShellMunition.cs` | 33 | Munitions | class ExplosiveShellMunition : ShellMunition |  |  |
| `Munitions/GlideBomb.cs` | 249 | Munitions | class GlideBomb : LegacyMissile, IProgrammableMissile, IMissile, IImbued, IOwned, ILocalImbued, IMunition, IStorable, ISaveKeyed, IModSource, IFactionLocke... | OnRepooled | NetworkBehaviour, SyncVar, pooling |
| `Munitions/GuidedDecoy.cs` | 70 | Munitions | class GuidedDecoy : DirectGuidedMissile | OnUnpooled | pooling |
| `Munitions/GuidedMissile.cs` | 244 | Munitions | class GuidedMissile : LegacyMissile | Awake, OnUnpooled | pooling |
| `Munitions/HomeOnJamSeeker.cs` | 143 | Munitions | class HomeOnJamSeeker : MissileSeeker, IJammable, IEWarTarget, IOwned | Awake, OnDisable, OnEnable |  |
| `Munitions/IActiveMissileSalvo.cs` | 33 | Munitions | interface IActiveMissileSalvo |  |  |
| `Munitions/IDamageCharacteristic.cs` | 31 | Munitions | interface IDamageCharacteristic |  |  |
| `Munitions/IDamageDealer.cs` | 17 | Munitions | interface IDamageDealer : IDamageCharacteristic |  |  |
| `Munitions/IImbued.cs` | 11 | Munitions | interface IImbued : IOwned |  |  |
| `Munitions/IImbuedObjectSource.cs` | 30 | Munitions | interface IImbuedObjectSource : ILaunchingPlatform, INetIdentifiedScript, IOwned |  |  |
| `Munitions/ILaunchingPlatform.cs` | 21 | Munitions | interface ILaunchingPlatform : INetIdentifiedScript |  |  |
| `Munitions/ILocalImbued.cs` | 12 | Munitions | interface ILocalImbued : IOwned |  |  |
| `Munitions/IMagazine.cs` | 27 | Munitions | interface IMagazine : IQuantityHolder |  |  |
| `Munitions/IMagazineLoadComponentData.cs` | 9 | Munitions | interface IMagazineLoadComponentData |  |  |
| `Munitions/IMissile.cs` | 86 | Munitions | interface IMissile : IImbued, IOwned, ILocalImbued, IMunition, IStorable, ISaveKeyed, IModSource, IFactionLocked, INetworkSpawnerRegistered, IPointCostItem, I... |  |  |
| `Munitions/IMissileSalvoSource.cs` | 11 | Munitions | interface IMissileSalvoSource |  |  |
| `Munitions/IMunition.cs` | 75 | Munitions | interface IMunition : IStorable, ISaveKeyed, IModSource, IFactionLocked, INetworkSpawnerRegistered, IPointCostItem |  |  |
| `Munitions/IMunitionCollection.cs` | 21 | Munitions | interface IMunitionCollection |  |  |
| `Munitions/INonphysicalMunition.cs` | 29 | Munitions | interface INonphysicalMunition : IMunition, IStorable, ISaveKeyed, IModSource, IFactionLocked, INetworkSpawnerRegistered, IPointCostItem |  | pooling |
| `Munitions/InstancedDamagers/ArmorOnlyDamager.cs` | 26 | Munitions.InstancedDamagers | class ArmorOnlyDamager : BaseThrowawayDamager |  |  |
| `Munitions/InstancedDamagers/BaseThrowawayDamager.cs` | 83 | Munitions.InstancedDamagers | class BaseThrowawayDamager : IDamageDealer, IDamageCharacteristic |  |  |
| `Munitions/InstancedDamagers/ChainedThrowawayDamager.cs` | 88 | Munitions.InstancedDamagers | class ChainedThrowawayDamager : IDamageDealer, IDamageCharacteristic |  |  |
| `Munitions/InstancedDamagers/ComponentOnlyDamager.cs` | 39 | Munitions.InstancedDamagers | class ComponentOnlyDamager : IDamageCharacteristic |  |  |
| `Munitions/InstancedDamagers/MultiRayConeDamager.cs` | 63 | Munitions.InstancedDamagers | class MultiRayConeDamager : BaseThrowawayDamager |  |  |
| `Munitions/InstancedDamagers/PenetratingExplosiveDamager.cs` | 60 | Munitions.InstancedDamagers | class PenetratingExplosiveDamager : BaseThrowawayDamager |  |  |
| `Munitions/InstancedDamagers/SingleRayDamager.cs` | 63 | Munitions.InstancedDamagers | class SingleRayDamager : BaseThrowawayDamager |  |  |
| `Munitions/InstancedDamagers/SingleSpherecastDamager.cs` | 28 | Munitions.InstancedDamagers | class SingleSpherecastDamager : SingleRayDamager |  |  |
| `Munitions/InstancedDamagers/SpallingRayDamager.cs` | 50 | Munitions.InstancedDamagers | class SpallingRayDamager : SingleRayDamager |  |  |
| `Munitions/InstancedDamagers/SphereOverlapDamager.cs` | 36 | Munitions.InstancedDamagers | class SphereOverlapDamager : BaseThrowawayDamager |  |  |
| `Munitions/InstancedDamagers/SphereOverlapDamagerUnlimited.cs` | 42 | Munitions.InstancedDamagers | class SphereOverlapDamagerUnlimited |  |  |
| `Munitions/InstancedDamagers/StructureOnlyDamager.cs` | 48 | Munitions.InstancedDamagers | class StructureOnlyDamager : SingleRayDamager |  |  |
| `Munitions/InterceptableShellMunition.cs` | 185 | Munitions | class InterceptableShellMunition : ShellMunition, IDamageable, ISubDamageable, IIntelIdentity | Awake, OnRepooled, OnUnpooled | pooling, prefab |
| `Munitions/IProgrammableMissile.cs` | 19 | Munitions | interface IProgrammableMissile : IMissile, IImbued, IOwned, ILocalImbued, IMunition, IStorable, ISaveKeyed, IModSource, IFactionLocked, INetworkSpawnerRegistered, IPointC... |  |  |
| `Munitions/IProxProbFuzeTrigger.cs` | 7 | Munitions | interface IProxProbFuzeTrigger |  |  |
| `Munitions/IReadOnlyMissileSalvo.cs` | 19 | Munitions | interface IReadOnlyMissileSalvo |  |  |
| `Munitions/ITimeDelayFuzed.cs` | 15 | Munitions | interface ITimeDelayFuzed |  |  |
| `Munitions/IUserConfigurableMunition.cs` | 12 | Munitions | interface IUserConfigurableMunition |  |  |
| `Munitions/LaunchedLookaheadMunition.cs` | 217 | Munitions | class LaunchedLookaheadMunition : TargetableLookaheadMunition, IImbued, IOwned | OnDestroy, OnRepooled | NetworkBehaviour, ClientRpc, NetworkServer, NetworkClient, save-state, XML, pooling |
| `Munitions/LegacyMissile.cs` | 300 | Munitions | class LegacyMissile : Missile, IMissileReportSource, IIntelReportable, IIdentifiable | Awake, OnUnpooled | pooling, prefab |
| `Munitions/LightweightAirburstFragShell.cs` | 120 | Munitions | class LightweightAirburstFragShell : LightweightAirburstShell |  |  |
| `Munitions/LightweightAirburstShell.cs` | 38 | Munitions | class LightweightAirburstShell : LightweightKineticShell, ITimeDelayFuzed |  |  |
| `Munitions/LightweightClusterShell.cs` | 48 | Munitions | class LightweightClusterShell : LightweightKineticShell |  | prefab |
| `Munitions/LightweightExplosiveShell.cs` | 35 | Munitions | class LightweightExplosiveShell : LightweightKineticShell |  |  |
| `Munitions/LightweightKineticMunitionContainer.cs` | 315 | Munitions | class LightweightKineticMunitionContainer : LookaheadMunitionBase, ITimeDelayFuzed<br>class LWKMunitionSaver : SaveFileObject.BulkObjectSaver | OnDestroy, OnRepooled, OnUnpooled | NetworkBehaviour, ClientRpc, NetworkServer, NetworkClient, save-state, XML, pooling, prefab |
| `Munitions/LightweightKineticShell.cs` | 137 | Munitions | class LightweightKineticShell : LightweightMunitionBase |  |  |
| `Munitions/LightweightKineticSplashDamageShell.cs` | 208 | Munitions | class LightweightKineticSplashDamageShell : LightweightKineticShell<br>class SplashDamageCharacteristic : IDamageCharacteristic | Awake | prefab |
| `Munitions/LightweightMunitionBase.cs` | 320 | Munitions | class LightweightMunitionBase : ScriptableObject, IMunition, IStorable, ISaveKeyed, IModSource, IFactionLocked, INetworkSpawnerRegistered, IPointCostItem, IDamageCharact... |  |  |
| `Munitions/LightweightProximityShell.cs` | 122 | Munitions | class LightweightProximityShell : LightweightKineticShell |  |  |
| `Munitions/LightweightSplashingShell.cs` | 67 | Munitions | class LightweightSplashingShell : LightweightKineticShell |  |  |
| `Munitions/LoiteringMissile.cs` | 726 | Munitions | class LoiteringMissile : GuidedMissile<br>enum FlightPhase | Awake, FixedUpdate, OnDestroy, OnRepooled, OnUnpooled | NetworkBehaviour, SyncVar, ClientRpc, NetworkClient, pooling, prefab |
| `Munitions/LookaheadMunition.cs` | 313 | Munitions | class LookaheadMunition : LookaheadMunitionBase, IMunition, IStorable, ISaveKeyed, IModSource, IFactionLocked, INetworkSpawnerRegistered, IPointCostItem<br>class LookaheadMunitionSaver : SaveFileObject.BulkObjectSaver |  | NetworkClient, save-state, XML, prefab |
| `Munitions/LookaheadMunitionBase.cs` | 487 | Munitions | class LookaheadMunitionBase : NetworkPoolable, ILocalImbued, IOwned, IBulkSaveObject | Awake, FixedUpdate, OnDestroy, OnRepooled, OnUnpooled | NetworkBehaviour, ClientRpc, NetworkServer, NetworkClient, save-state, XML, pooling, prefab |
| `Munitions/Magazine.cs` | 309 | Munitions | class Magazine : IMagazine, IQuantityHolder<br>struct MagSaveData<br>struct MagStateData<br>struct MagChange |  |  |
| `Munitions/MagazineCollection.cs` | 327 | Munitions | class MagazineCollection |  |  |
| `Munitions/MagazinePool.cs` | 155 | Munitions | class MagazinePool : IMagazine, IQuantityHolder |  |  |
| `Munitions/MIRVWarhead.cs` | 99 | Munitions | class MIRVWarhead : SubmunitionWarhead |  |  |
| `Munitions/Missile.cs` | 543 | Munitions | class Missile : LaunchedLookaheadMunition, IMissile, IImbued, IOwned, ILocalImbued, IMunition, IStorable, ISaveKeyed, IModSource, IFactionLocked, INetwor... | FixedUpdate, OnDestroy, OnRepooled, OnUnpooled | NetworkBehaviour, ClientRpc, NetworkServer, NetworkClient, pooling |
| `Munitions/MissileFlightPhase.cs` | 13 | Munitions | enum MissileFlightPhase |  |  |
| `Munitions/MissileFragmentationWarhead.cs` | 110 | Munitions | class MissileFragmentationWarhead : MissileWarhead, IDamageDealer, IDamageCharacteristic |  |  |
| `Munitions/MissileImpactWarhead.cs` | 162 | Munitions | class MissileImpactWarhead : MissileWarhead, IDamageDealer, IDamageCharacteristic<br>enum DamageMethod |  |  |
| `Munitions/MissilePool.cs` | 550 | Munitions | class MissilePool : IWeaponGroup, IOrderAssignable, ICycled, IQuantityHolder, IUnmaskingDriver |  |  |
| `Munitions/MissileProgrammer.cs` | 209 | Munitions | class MissileProgrammer<br>interface IReservedChannel<br>class ProgrammingTask : IReservedChannel<br>class NoProgramming : IReservedChannel |  |  |
| `Munitions/MissileProximityBurstWarhead.cs` | 126 | Munitions | class MissileProximityBurstWarhead : MissileWarhead, IDamageCharacteristic |  |  |
| `Munitions/MissileSalvoChanged.cs` | 4 | Munitions |  |  |  |
| `Munitions/MissileSalvoGroup.cs` | 75 | Munitions | class MissileSalvoGroup : IActiveMissileSalvo |  |  |
| `Munitions/MissileSeeker.cs` | 201 | Munitions | class MissileSeeker : NetworkBehaviour, IOwned, IBulkSaveComponent | Awake | NetworkBehaviour, save-state, XML |
| `Munitions/MissileUsageCharacteristics.cs` | 14 | Munitions | enum MissileUsageCharacteristics |  |  |
| `Munitions/MissileWarhead.cs` | 59 | Munitions | class MissileWarhead : MonoBehaviour |  |  |
| `Munitions/ModularMissiles/Descriptors/Controls/BaseAvionicsDescriptor.cs` | 228 | Munitions.ModularMissiles.Descriptors.Controls | class BaseAvionicsDescriptor : MissileComponentDescriptor<br>enum TerminalManeuver<br>class BaseAvionicsSettings : MissileComponentSettings |  |  |
| `Munitions/ModularMissiles/Descriptors/Controls/CruiseGuidanceDescriptor.cs` | 42 | Munitions.ModularMissiles.Descriptors.Controls | class CruiseGuidanceDescriptor : BaseAvionicsDescriptor<br>class CruiseGuidanceSettings : BaseAvionicsSettings |  |  |
| `Munitions/ModularMissiles/Descriptors/Controls/DirectGuidanceDescriptor.cs` | 85 | Munitions.ModularMissiles.Descriptors.Controls | class DirectGuidanceDescriptor : BaseAvionicsDescriptor<br>class DirectGuidanceSettings : BaseAvionicsSettings |  |  |
| `Munitions/ModularMissiles/Descriptors/MissileComponentChanged.cs` | 4 | Munitions.ModularMissiles.Descriptors |  |  |  |
| `Munitions/ModularMissiles/Descriptors/MissileComponentDescriptor.cs` | 230 | Munitions.ModularMissiles.Descriptors | class MissileComponentDescriptor : ScriptableObject, IBundleKeyed, ISaveKeyed, IModSource, IFactionLocked<br>class MissileComponentSettings |  |  |
| `Munitions/ModularMissiles/Descriptors/MissileEngineDescriptor.cs` | 203 | Munitions.ModularMissiles.Descriptors | class MissileEngineDescriptor : MissileComponentDescriptor<br>class MissileEngineSettings : MissileComponentSettings |  |  |
| `Munitions/ModularMissiles/Descriptors/Seekers/ActiveSeekerDescriptor.cs` | 94 | Munitions.ModularMissiles.Descriptors.Seekers | class ActiveSeekerDescriptor : BeamSeekerDescriptor<br>class ActiveSeekerSettings : BeamSeekerSettings |  |  |
| `Munitions/ModularMissiles/Descriptors/Seekers/BaseSeekerDescriptor.cs` | 207 | Munitions.ModularMissiles.Descriptors.Seekers | class BaseSeekerDescriptor : MissileComponentDescriptor<br>class BaseSeekerSettings : MissileComponentSettings | OnDestroy |  |
| `Munitions/ModularMissiles/Descriptors/Seekers/BeamSeekerDescriptor.cs` | 164 | Munitions.ModularMissiles.Descriptors.Seekers | class BeamSeekerDescriptor : BaseSeekerDescriptor<br>class BeamSeekerSettings : BaseSeekerSettings |  |  |
| `Munitions/ModularMissiles/Descriptors/Seekers/CommandSeekerDescriptor.cs` | 91 | Munitions.ModularMissiles.Descriptors.Seekers | class CommandSeekerDescriptor : BaseSeekerDescriptor<br>class CommandSeekerSettings : BaseSeekerSettings |  |  |
| `Munitions/ModularMissiles/Descriptors/Seekers/DatalinkSeekerDescriptor.cs` | 62 | Munitions.ModularMissiles.Descriptors.Seekers | class DatalinkSeekerDescriptor : BaseSeekerDescriptor<br>class DatalinkSeekerSettings : BaseSeekerSettings |  |  |
| `Munitions/ModularMissiles/Descriptors/Seekers/PassiveARHSeekerDescriptor.cs` | 142 | Munitions.ModularMissiles.Descriptors.Seekers | class PassiveARHSeekerDescriptor : PassiveSeekerDescriptor<br>enum ARHTarget<br>class PassiveARHSeekerSettings : BaseSeekerSettings |  |  |
| `Munitions/ModularMissiles/Descriptors/Seekers/PassiveSeekerDescriptor.cs` | 119 | Munitions.ModularMissiles.Descriptors.Seekers | class PassiveSeekerDescriptor : BeamSeekerDescriptor<br>class PassiveSeekerSettings : BeamSeekerSettings |  |  |
| `Munitions/ModularMissiles/Descriptors/Seekers/SeekerMode.cs` | 8 | Munitions.ModularMissiles.Descriptors.Seekers | enum SeekerMode |  |  |
| `Munitions/ModularMissiles/Descriptors/Support/BaseSupportDescriptor.cs` | 68 | Munitions.ModularMissiles.Descriptors.Support | class BaseSupportDescriptor : MissileComponentDescriptor, IModifierSource |  |  |
| `Munitions/ModularMissiles/Descriptors/Support/DecoyLaunchingSupportDescriptor.cs` | 77 | Munitions.ModularMissiles.Descriptors.Support | class DecoyLaunchingSupportDescriptor : BaseSupportDescriptor |  | prefab |
| `Munitions/ModularMissiles/Descriptors/Support/JammerSupportDescriptor.cs` | 63 | Munitions.ModularMissiles.Descriptors.Support | class JammerSupportDescriptor : BaseSupportDescriptor, ITuneableEWar |  | prefab |
| `Munitions/ModularMissiles/Descriptors/Warheads/BaseSubmunitionWarheadDescriptor.cs` | 466 | Munitions.ModularMissiles.Descriptors.Warheads | class BaseSubmunitionWarheadDescriptor : BaseWarheadDescriptor<br>enum DetonationMode<br>class BaseSubmunitionWarheadSettings : MissileComponentSettings<br>struct AngleOption |  | prefab |
| `Munitions/ModularMissiles/Descriptors/Warheads/BaseWarheadDescriptor.cs` | 119 | Munitions.ModularMissiles.Descriptors.Warheads | class BaseWarheadDescriptor : MissileComponentDescriptor |  |  |
| `Munitions/ModularMissiles/Descriptors/Warheads/FixedSubmunitionWarheadDescriptor.cs` | 30 | Munitions.ModularMissiles.Descriptors.Warheads | class FixedSubmunitionWarheadDescriptor : BaseSubmunitionWarheadDescriptor |  | prefab |
| `Munitions/ModularMissiles/Descriptors/Warheads/FragmentationWarheadDescriptor.cs` | 218 | Munitions.ModularMissiles.Descriptors.Warheads | class FragmentationWarheadDescriptor : BaseWarheadDescriptor, IDamageDealer, IDamageCharacteristic |  | prefab |
| `Munitions/ModularMissiles/Descriptors/Warheads/ImpactConeWarheadDescriptor.cs` | 127 | Munitions.ModularMissiles.Descriptors.Warheads | class ImpactConeWarheadDescriptor : BaseWarheadDescriptor, IDamageCharacteristic |  |  |
| `Munitions/ModularMissiles/Descriptors/Warheads/KineticPenetratorWarheadDescriptor.cs` | 268 | Munitions.ModularMissiles.Descriptors.Warheads | class KineticPenetratorWarheadDescriptor : BaseWarheadDescriptor<br>struct DamageBlock : IDamageCharacteristic |  |  |
| `Munitions/ModularMissiles/Descriptors/Warheads/SelectableSubmunitionWarheadDescriptor.cs` | 194 | Munitions.ModularMissiles.Descriptors.Warheads | class SelectableSubmunitionWarheadDescriptor : BaseSubmunitionWarheadDescriptor<br>class SelectableSubmunitionWarheadSettings : BaseSubmunitionWarheadSettings<br>struct IntervalOption |  |  |
| `Munitions/ModularMissiles/Descriptors/Warheads/SimpleKineticWarheadDescriptor.cs` | 108 | Munitions.ModularMissiles.Descriptors.Warheads | class SimpleKineticWarheadDescriptor : BaseWarheadDescriptor, IDamageCharacteristic |  |  |
| `Munitions/ModularMissiles/IMissileFlightControl.cs` | 62 | Munitions.ModularMissiles | interface IMissileFlightControl |  |  |
| `Munitions/ModularMissiles/MissileSocket.cs` | 134 | Munitions.ModularMissiles | class MissileSocket |  |  |
| `Munitions/ModularMissiles/MissileSocketType.cs` | 15 | Munitions.ModularMissiles | enum MissileSocketType |  |  |
| `Munitions/ModularMissiles/MissileSocketTypeExtensions.cs` | 40 | Munitions.ModularMissiles | class MissileSocketTypeExtensions |  |  |
| `Munitions/ModularMissiles/MissileTemplate.cs` | 98 | Munitions.ModularMissiles | class MissileTemplate : IFleetTemplateableDesign |  |  |
| `Munitions/ModularMissiles/ModularMissile.cs` | 2903 | Munitions.ModularMissiles | class ModularMissile : Missile, IFleetTemplateableActive, IBundleKeyed, ISaveKeyed, IProgrammableMissile, IMissile, IImbued, IOwned, ILocalImbued, IMunition, IS...<br>class ModularMissileSaver : SaveFileObject.BulkObjectSaver<br>struct Stage<br>class MissileSummary : IPointCostItem, ISaveKeyed | Awake, FixedUpdate, OnDestroy, OnRepooled, OnUnpooled | NetworkBehaviour, ClientRpc, NetworkServer, NetworkClient, save-state, XML, pooling, prefab |
| `Munitions/ModularMissiles/ModularMissileEvent.cs` | 4 | Munitions.ModularMissiles |  |  |  |
| `Munitions/ModularMissiles/RandomMissileSkin.cs` | 78 | Munitions.ModularMissiles | class RandomMissileSkin : NetworkBehaviour |  | NetworkBehaviour, ClientRpc, NetworkClient |
| `Munitions/ModularMissiles/Runtime/Controls/CruiseGuidanceController.cs` | 462 | Munitions.ModularMissiles.Runtime.Controls | class CruiseGuidanceController : RuntimeMissileGuidance | Awake, OnAdded, OnDead, OnDestroy, OnLaunched, OnRepooled | XML, pooling |
| `Munitions/ModularMissiles/Runtime/Controls/DirectGuidanceController.cs` | 336 | Munitions.ModularMissiles.Runtime.Controls | class DirectGuidanceController : RuntimeMissileGuidance | Awake, OnAdded, OnDead, OnDestroy, OnLaunched, OnRepooled | XML, pooling |
| `Munitions/ModularMissiles/Runtime/Controls/RuntimeMissileGuidance.cs` | 464 | Munitions.ModularMissiles.Runtime.Controls | class RuntimeMissileGuidance : RuntimeMissileBehaviour | OnAdded, OnCloned, OnDestroy, OnUnpooled | NetworkBehaviour, SyncVar, XML, pooling |
| `Munitions/ModularMissiles/Runtime/RuntimeMissileBehaviour.cs` | 84 | Munitions.ModularMissiles.Runtime | class RuntimeMissileBehaviour : NetworkBehaviour, IBulkSaveComponent | OnAdded, OnCloned, OnDead, OnDestroy, OnLaunched, OnRepooled, OnUnpooled | NetworkBehaviour, save-state, XML, pooling |
| `Munitions/ModularMissiles/Runtime/RuntimeMissileWarhead.cs` | 167 | Munitions.ModularMissiles.Runtime | class RuntimeMissileWarhead : RuntimeMissileBehaviour | FixedUpdate, OnAdded, OnCloned, OnUnpooled | XML, pooling, prefab |
| `Munitions/ModularMissiles/Runtime/Seekers/RuntimeActiveSeeker.cs` | 342 | Munitions.ModularMissiles.Runtime.Seekers | class RuntimeActiveSeeker : RuntimeBeamSeeker | Awake, OnAdded, OnDestroy | NetworkBehaviour, ClientRpc, NetworkServer, NetworkClient, XML |
| `Munitions/ModularMissiles/Runtime/Seekers/RuntimeBeamSeeker.cs` | 452 | Munitions.ModularMissiles.Runtime.Seekers | class RuntimeBeamSeeker : RuntimeMissileSeeker, ISensor, IJammable, IEWarTarget, IOwned<br>struct WeightedSigHit | OnAdded, OnDisable, OnEnable | XML |
| `Munitions/ModularMissiles/Runtime/Seekers/RuntimeCommandSeeker.cs` | 296 | Munitions.ModularMissiles.Runtime.Seekers | class RuntimeCommandSeeker : RuntimeMissileSeeker | OnAdded, OnDestroy, OnLaunched | NetworkBehaviour, SyncVar, XML |
| `Munitions/ModularMissiles/Runtime/Seekers/RuntimeDatalinkSeeker.cs` | 73 | Munitions.ModularMissiles.Runtime.Seekers | class RuntimeDatalinkSeeker : RuntimeMissileSeeker | OnAdded | XML |
| `Munitions/ModularMissiles/Runtime/Seekers/RuntimeMissileSeeker.cs` | 423 | Munitions.ModularMissiles.Runtime.Seekers | class RuntimeMissileSeeker : RuntimeMissileBehaviour, IOwned, IRenderableMissileSeeker<br>enum SeekerSearchResult<br>enum SeekerValidationResult | OnAdded, OnDead, OnRepooled, OnUnpooled | NetworkBehaviour, SyncVar, XML, pooling |
| `Munitions/ModularMissiles/Runtime/Seekers/RuntimePassiveSeeker.cs` | 420 | Munitions.ModularMissiles.Runtime.Seekers | class RuntimePassiveSeeker : RuntimeBeamSeeker | Awake, OnAdded, OnDestroy | NetworkBehaviour, ClientRpc, NetworkServer, NetworkClient, XML |
| `Munitions/ModularMissiles/Runtime/Support/RuntimeDecoyLaunchingSupport.cs` | 102 | Munitions.ModularMissiles.Runtime.Support | class RuntimeDecoyLaunchingSupport : RuntimeMissileBehaviour | OnAdded, OnUnpooled | XML, pooling |
| `Munitions/ModularMissiles/Runtime/Support/RuntimeJammerSupport.cs` | 90 | Munitions.ModularMissiles.Runtime.Support | class RuntimeJammerSupport : RuntimeMissileBehaviour | FixedUpdate, OnAdded, OnDead, OnLaunched | XML, pooling |
| `Munitions/ModularMissiles/SerializedMissileSocket.cs` | 15 | Munitions.ModularMissiles | class SerializedMissileSocket |  |  |
| `Munitions/ModularMissiles/SerializedMissileTemplate.cs` | 79 | Munitions.ModularMissiles | class SerializedMissileTemplate : IXmlDocSerializable |  | XML |
| `Munitions/ModularMissiles/TestComponent.cs` | 48 | Munitions.ModularMissiles | class TestComponent : NetworkBehaviour |  | NetworkBehaviour, ClientRpc, NetworkClient, prefab |
| `Munitions/MunitionHitInfo.cs` | 87 | Munitions | class MunitionHitInfo : IDisposable |  | prefab |
| `Munitions/MunitionMonitorType.cs` | 9 | Munitions | enum MunitionMonitorType |  |  |
| `Munitions/MunitionsHelpers.cs` | 342 | Munitions | class MunitionsHelpers<br>struct ColliderComparer : IEqualityComparer<Collider> |  | prefab |
| `Munitions/MunitionSimulationMethod.cs` | 9 | Munitions | enum MunitionSimulationMethod |  |  |
| `Munitions/MunitionTags.cs` | 24 | Munitions | struct MunitionTags |  |  |
| `Munitions/MunitionType.cs` | 10 | Munitions | enum MunitionType |  |  |
| `Munitions/NonphysicalMunition.cs` | 64 | Munitions | class NonphysicalMunition : LightweightMunitionBase, INonphysicalMunition, IMunition, IStorable, ISaveKeyed, IModSource, IFactionLocked, INetworkSpawnerRegistered, I... |  | pooling |
| `Munitions/PDTargetType.cs` | 10 | Munitions | enum PDTargetType |  |  |
| `Munitions/PointCalculateScopedCostItems.cs` | 158 | Munitions | class PointCalculateScopedCostItems : IMunitionCollection, IDisposable |  | prefab |
| `Munitions/ProximityBurstShellMunition.cs` | 71 | Munitions | class ProximityBurstShellMunition : ShellMunition |  |  |
| `Munitions/ShellMunition.cs` | 289 | Munitions | class ShellMunition : LookaheadMunition, IDamageCharacteristic | Awake, OnRepooled, OnUnpooled | pooling |
| `Munitions/SimpleProxFuzeProbabilityModifier.cs` | 15 | Munitions | class SimpleProxFuzeProbabilityModifier : MonoBehaviour, IProxProbFuzeTrigger |  |  |
| `Munitions/StatusProgrammingTimer.cs` | 24 | Munitions | struct StatusProgrammingTimer |  |  |
| `Munitions/SubmunitionWarhead.cs` | 252 | Munitions | class SubmunitionWarhead : MissileWarhead |  | prefab |
| `Munitions/TargetableLookaheadMunition.cs` | 324 | Munitions | class TargetableLookaheadMunition : TrackableLookaheadMunition, IDamageable, ISubDamageable, IIdentifiable | Awake, OnDestroy, OnRepooled, OnUnpooled | XML, pooling, prefab |
| `Munitions/TrackableLookaheadMunition.cs` | 262 | Munitions | class TrackableLookaheadMunition : LookaheadMunition, IBoardPiece, ISelectable, IOwned | OnDestroy, OnRepooled | pooling, prefab |
| `Munitions/UtilityWeaponType.cs` | 10 | Munitions | enum UtilityWeaponType |  |  |
| `Munitions/WeaponEffectGroup.cs` | 78 | Munitions | class WeaponEffectGroup |  | pooling, prefab |
| `Munitions/WeaponEffectSet.cs` | 83 | Munitions | class WeaponEffectSet |  | pooling |
| `Networking/AggregatedSyncCollection.cs` | 119 | Networking | class AggregatedSyncCollection : SyncObject |  |  |
| `Networking/AggregatedSyncListDictionary.cs` | 90 | Networking | class AggregatedSyncListDictionary : SyncObject |  |  |
| `Networking/AggSyncLinkedList.cs` | 252 | Networking | class AggSyncLinkedList : IAggSyncCollectionElement<T>, ICollection<T>, IEnumerable<T>, IEnumerable<br>struct Change |  |  |
| `Networking/AggSyncListElement.cs` | 437 | Networking | class AggSyncListElement : IAggSyncCollectionElement<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyList<T>, IReadOnlyCollection<T><br>struct Change<br>struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable |  |  |
| `Networking/AggSyncOperation.cs` | 11 | Networking | enum AggSyncOperation : byte |  |  |
| `Networking/AggSyncSetElement.cs` | 318 | Networking | class AggSyncSetElement : ISet<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IAggSyncCollectionElement<T><br>struct Change |  |  |
| `Networking/BaseNetBehaviourSyncList.cs` | 313 | Networking | class BaseNetBehaviourSyncList : SyncObject, IList<TScript>, ICollection<TScript>, IEnumerable<TScript>, IEnumerable, IReadOnlyList<TScript>, IReadOnlyCollection<TScript>...<br>struct Enumerator : IEnumerator<TScript>, IEnumerator, IDisposable |  |  |
| `Networking/CalloutMessage.cs` | 15 | Networking | struct CalloutMessage : NetworkMessage |  |  |
| `Networking/CampaignEndGameMessage.cs` | 14 | Networking | struct CampaignEndGameMessage : NetworkMessage |  |  |
| `Networking/CampaignSerializers.cs` | 27 | Networking | class CampaignSerializers |  |  |
| `Networking/ChatLog.cs` | 53 | Networking | class ChatLog |  |  |
| `Networking/ChatMessage.cs` | 17 | Networking | struct ChatMessage : NetworkMessage |  |  |
| `Networking/ChatService.cs` | 251 | Networking | class ChatService : NetworkBehaviour, IChatService<br>struct ChatCommand | Awake, OnDestroy, OnStartClient, OnStartServer | NetworkBehaviour, NetworkServer, NetworkClient |
| `Networking/ClientAuthFailedData.cs` | 7 | Networking | class ClientAuthFailedData |  |  |
| `Networking/CommonMatchInfoKeys.cs` | 17 | Networking | class CommonMatchInfoKeys |  |  |
| `Networking/Community/CommunityManagementCore.cs` | 456 | Networking.Community | class CommunityManagementCore<br>struct QueryScarletLetterReport<br>struct ScarletLetterReport<br>struct QueryBanEntry<br>struct BanEntry<br>class QueryResult<br>class CommunityData |  | JSON |
| `Networking/Community/PlayerConductReport.cs` | 61 | Networking.Community | class PlayerConductReport |  |  |
| `Networking/CustomAuthenticator.cs` | 307 | Networking | class CustomAuthenticator : NetworkAuthenticator<br>struct AuthRequestMessage : NetworkMessage<br>struct AuthResponseMessage : NetworkMessage<br>struct AuthTicketProcessing | OnStartClient, OnStartServer, OnStopClient, OnStopServer | NetworkServer, NetworkClient |
| `Networking/DedicatedServerBanList.cs` | 88 | Networking | class DedicatedServerBanList |  |  |
| `Networking/DedicatedServerBootstrap.cs` | 128 | Networking | class DedicatedServerBootstrap : MonoBehaviour, IStatusProgress<float>, IProgress<float> | Awake, Start |  |
| `Networking/DedicatedServerPlayerShell.cs` | 115 | Networking | class DedicatedServerPlayerShell : IPlayer, IPlayerInfo |  | NetworkServer, prefab |
| `Networking/DeploymentTimerMessage.cs` | 11 | Networking | struct DeploymentTimerMessage : NetworkMessage |  |  |
| `Networking/DownloadBattleReportMessage.cs` | 9 | Networking | struct DownloadBattleReportMessage : NetworkMessage |  |  |
| `Networking/DownloadCampaignBattleReportMessage.cs` | 9 | Networking | struct DownloadCampaignBattleReportMessage : NetworkMessage |  |  |
| `Networking/ForceDeploymentMessage.cs` | 10 | Networking | struct ForceDeploymentMessage : NetworkMessage |  |  |
| `Networking/FragmentedMessage.cs` | 97 | Networking | struct FragmentedMessage : NetworkMessage |  |  |
| `Networking/FragmentedMessageSerializer.cs` | 19 | Networking | class FragmentedMessageSerializer |  |  |
| `Networking/FreeMovementNetworkTransform.cs` | 429 | Networking | class FreeMovementNetworkTransform : NetworkBehaviour<br>struct FMSnapshot : Snapshot | Awake, FixedUpdate, OnEnable, OnStartClient | NetworkBehaviour, SyncVar, ClientRpc, NetworkServer, NetworkClient |
| `Networking/GenericSerializers.cs` | 500 | Networking | class GenericSerializers |  | prefab |
| `Networking/HostCampaignLobbyData.cs` | 22 | Networking | class HostCampaignLobbyData : CrossScenePipe.CrossSceneData |  |  |
| `Networking/HostClientAuthData.cs` | 17 | Networking | class HostClientAuthData |  |  |
| `Networking/HostSkirmishDedicatedServerData.cs` | 9 | Networking | class HostSkirmishDedicatedServerData : CrossScenePipe.CrossSceneData |  |  |
| `Networking/HostSkirmishLobbyData.cs` | 21 | Networking | class HostSkirmishLobbyData : CrossScenePipe.CrossSceneData |  |  |
| `Networking/HullComponentRouter.cs` | 2024 | Networking | class HullComponentRouter : NetworkBehaviour, IHullComponentRouter, HullPart.IHullPartRPC, HullComponent.IHullComponentRPC, CrewedComponent.ICrewedComponentRPC, Cycl... | OnDestroy | NetworkBehaviour, ClientRpc, NetworkServer, NetworkClient |
| `Networking/IAggSyncCollectionElement.cs` | 34 | Networking | interface IAggSyncCollectionElement : ICollection<T>, IEnumerable<T>, IEnumerable |  |  |
| `Networking/IChatService.cs` | 23 | Networking | interface IChatService |  |  |
| `Networking/IHullComponentRouter.cs` | 15 | Networking | interface IHullComponentRouter |  |  |
| `Networking/IMatchOptionBuilder.cs` | 13 | Networking | interface IMatchOptionBuilder |  |  |
| `Networking/IMultiplayerGameInfo.cs` | 47 | Networking | interface IMultiplayerGameInfo |  |  |
| `Networking/INetIdentifiedScript.cs` | 11 | Networking | interface INetIdentifiedScript |  |  |
| `Networking/INetManagerEventHandler.cs` | 34 | Networking | interface INetManagerEventHandler |  |  |
| `Networking/INetworkSpawnerRegistered.cs` | 13 | Networking | interface INetworkSpawnerRegistered |  |  |
| `Networking/IPolySer.cs` | 11 | Networking | interface IPolySer |  |  |
| `Networking/ISyncDropdown.cs` | 9 | Networking | interface ISyncDropdown |  |  |
| `Networking/ISyncObject.cs` | 18 | Networking | interface ISyncObject |  |  |
| `Networking/IVoteTracker.cs` | 11 | Networking | interface IVoteTracker |  |  |
| `Networking/JoinCampaignLobbyData.cs` | 15 | Networking | class JoinCampaignLobbyData : CrossScenePipe.CrossSceneData |  |  |
| `Networking/JoinSkirmishDedicatedServerData.cs` | 15 | Networking | class JoinSkirmishDedicatedServerData : CrossScenePipe.CrossSceneData |  |  |
| `Networking/JoinSkirmishLobbyData.cs` | 15 | Networking | class JoinSkirmishLobbyData : CrossScenePipe.CrossSceneData |  |  |
| `Networking/KickMessage.cs` | 9 | Networking | struct KickMessage : NetworkMessage |  |  |
| `Networking/LFTManager.cs` | 479 | Networking | class LFTManager : MonoBehaviour<br>enum TransferStatus<br>struct TransferCompletion<br>struct TransferToken<br>struct CancelMessage : NetworkMessage<br>class Transfer<br>class OutboundTransfer : Transfer<br>class InboundTransfer | Awake, OnDestroy, Update | NetworkServer, NetworkClient, prefab |
| `Networking/LoadMapByKey.cs` | 31 | Networking | class LoadMapByKey : LoadMapCommand |  |  |
| `Networking/LoadMapCommand.cs` | 16 | Networking | class LoadMapCommand : IPolySer |  |  |
| `Networking/LoadMapCommandSerializers.cs` | 17 | Networking | class LoadMapCommandSerializers |  |  |
| `Networking/LoadMapFromLocation.cs` | 31 | Networking | class LoadMapFromLocation : LoadMapCommand |  |  |
| `Networking/LoadMapFromMission.cs` | 27 | Networking | class LoadMapFromMission : LoadMapCommand |  |  |
| `Networking/LoadMapMessage.cs` | 9 | Networking | struct LoadMapMessage : NetworkMessage |  |  |
| `Networking/LobbyVoteTracker.cs` | 437 | Networking | class LobbyVoteTracker : NetworkBehaviour, IVoteTracker<br>enum VoteType<br>enum VoteAttributes<br>class ActiveVote<br>enum VoteResult<br>struct VoteRecord<br>struct SubmitVote : NetworkMessage<br>struct RequestVoteKick : NetworkMessage | Awake, OnStartServer, Update | NetworkBehaviour, NetworkServer, NetworkClient |
| `Networking/MapLoadingData.cs` | 6 | Networking | class MapLoadingData |  |  |
| `Networking/MatchListRefreshStatus.cs` | 9 | Networking | enum MatchListRefreshStatus |  |  |
| `Networking/MatchOptionType.cs` | 10 | Networking | enum MatchOptionType |  |  |
| `Networking/MultiplayerFilters.cs` | 42 | Networking | struct MultiplayerFilters |  |  |
| `Networking/NetBehaviourSyncList.cs` | 22 | Networking | class NetBehaviourSyncList : BaseNetBehaviourSyncList<TScript> where TScript : NetworkBehaviour |  | NetworkBehaviour |
| `Networking/NetInterfaceSyncList.cs` | 21 | Networking | class NetInterfaceSyncList : BaseNetBehaviourSyncList<TScript> where TScript : class, INetIdentifiedScript |  |  |
| `Networking/NetScriptAddress.cs` | 64 | Networking | struct NetScriptAddress |  | NetworkBehaviour, NetworkServer, NetworkClient |
| `Networking/NetScriptRefSerializers.cs` | 61 | Networking | class NetScriptRefSerializers |  |  |
| `Networking/NetworkHelpers.cs` | 21 | Networking | class NetworkHelpers |  | NetworkServer |
| `Networking/OrderShipMessage.cs` | 10 | Networking | struct OrderShipMessage : NetworkMessage |  |  |
| `Networking/PlayerRecordWrapper.cs` | 168 | Networking | class PlayerRecordWrapper : IPlayer, IPlayerInfo |  | prefab |
| `Networking/PolymorphicSerializer.cs` | 45 | Networking | class PolymorphicSerializer |  |  |
| `Networking/PortableNetworkManager.cs` | 565 | Networking | class PortableNetworkManager : NetworkManager<br>enum SessionConnectionState | OnDestroy, OnStartClient, OnStartServer, OnStopClient, OnStopServer, Start | NetworkServer, NetworkClient, prefab |
| `Networking/RequestClientFleetMessage.cs` | 11 | Networking | struct RequestClientFleetMessage : NetworkMessage |  |  |
| `Networking/RequestFleetMessage.cs` | 12 | Networking | struct RequestFleetMessage : NetworkMessage |  |  |
| `Networking/RichPresenceActivity.cs` | 11 | Networking | enum RichPresenceActivity |  |  |
| `Networking/ServerCommandFile.cs` | 53 | Networking | class ServerCommandFile<br>enum ServerCommand |  | XML |
| `Networking/SetPlayerFleetMessage.cs` | 17 | Networking | struct SetPlayerFleetMessage : NetworkMessage |  |  |
| `Networking/SkirmishClientReadyMessage.cs` | 10 | Networking | struct SkirmishClientReadyMessage : NetworkMessage |  |  |
| `Networking/SkirmishDedicatedServerConfig.cs` | 135 | Networking | class SkirmishDedicatedServerConfig<br>enum MapRotationType<br>enum AutoBalanceType<br>struct Bot<br>struct Setting<br>struct RankRestrictionRules |  | XML |
| `Networking/SkirmishDedicatedServerContinuity.cs` | 92 | Networking | class SkirmishDedicatedServerContinuity : SingletonMonobehaviour<SkirmishDedicatedServerContinuity> | Awake | prefab |
| `Networking/SkirmishEndGameMessage.cs` | 10 | Networking | struct SkirmishEndGameMessage : NetworkMessage |  |  |
| `Networking/SkirmishGameStateMessage.cs` | 10 | Networking | struct SkirmishGameStateMessage : NetworkMessage |  |  |
| `Networking/SplineNetworkTransform.cs` | 206 | Networking | class SplineNetworkTransform : NetworkBehaviour<br>struct SplineSnapshot : Snapshot | Awake, OnDisable, OnEnable, Update | NetworkBehaviour, ClientRpc, NetworkServer, NetworkClient |
| `Networking/SteamAchievementsManager.cs` | 215 | Networking | class SteamAchievementsManager |  |  |
| `Networking/SteamChatService.cs` | 131 | Networking | class SteamChatService : IChatService |  |  |
| `Networking/SteamLobbyInfo.cs` | 131 | Networking | class SteamLobbyInfo : IMultiplayerGameInfo |  |  |
| `Networking/SteamLobbyList.cs` | 133 | Networking | class SteamLobbyList |  |  |
| `Networking/SteamManager.cs` | 209 | Networking | class SteamManager : MonoBehaviour | Awake, OnDestroy, Update | prefab |
| `Networking/SteamServerInfo.cs` | 121 | Networking | class SteamServerInfo : IMultiplayerGameInfo |  |  |
| `Networking/SteamServerList.cs` | 134 | Networking | class SteamServerList |  |  |
| `Networking/SyncCollectionElementChanged.cs` | 4 | Networking |  |  |  |
| `Networking/SyncDropdownParam.cs` | 153 | Networking | class SyncDropdownParam : SyncObject, IInspectableDropdownParam, IXmlDocSerializable where TOption : IInspectableDropdownParam, IXmlDocSerializable, ISyncDropdown |  | XML |
| `Networking/SyncedKeyedListOption.cs` | 57 | Networking | class SyncedKeyedListOption : SyncedListOption |  |  |
| `Networking/SyncedListOption.cs` | 83 | Networking | class SyncedListOption : SyncedOption |  |  |
| `Networking/SyncedOption.cs` | 72 | Networking | class SyncedOption |  | prefab |
| `Networking/SyncedOptionSerializer.cs` | 35 | Networking | class SyncedOptionSerializer |  |  |
| `Networking/SyncedSliderOption.cs` | 122 | Networking | class SyncedSliderOption : SyncedOption |  |  |
| `Networking/SyncedToggleOption.cs` | 55 | Networking | class SyncedToggleOption : SyncedOption |  |  |
| `Networking/SyncListGameSettings.cs` | 10 | Networking | class SyncListGameSettings : SyncList<SyncedOption> |  |  |
| `Networking/SyncObjectList.cs` | 95 | Networking | class SyncObjectList : SyncObject |  |  |
| `Networking/SyncroutineManager.cs` | 487 | Networking | class SyncroutineManager : MonoBehaviour<br>struct SRToken<br>struct ServerStepMessage : NetworkMessage<br>struct ClientStepMessage : NetworkMessage<br>struct CancelMessage : NetworkMessage<br>struct ClientWaitForSpawnMessage : NetworkMessage<br>class Syncroutine<br>enum StepResult<br>class ServerProcess : Syncroutine<br>class ClientProcess : Syncroutine<br>class WaitForSpawnRecord | Awake, OnDestroy, Update | NetworkServer, NetworkClient, prefab |
| `Networking/TimeHackMessage.cs` | 11 | Networking | struct TimeHackMessage : NetworkMessage |  |  |
| `Networking/TransferShipGroupMessage.cs` | 14 | Networking | struct TransferShipGroupMessage : NetworkMessage |  |  |
| `Networking/UnloadMapMessage.cs` | 10 | Networking | struct UnloadMapMessage : NetworkMessage |  |  |
| `Networking/Workshop/ISteamWorkshopItem.cs` | 20 | Networking.Workshop | interface ISteamWorkshopItem |  |  |
| `Networking/Workshop/WorkshopHelpers.cs` | 174 | Networking.Workshop | class WorkshopHelpers<br>struct WorkshopPaths |  |  |
| `Procedural/Map/AsteroidFieldGenerator.cs` | 410 | Procedural.Map | class AsteroidFieldGenerator : MonoBehaviour<br>enum AsteroidSize<br>class AsteroidPrefabs<br>class ExclusionArea<br>enum AreaShape<br>enum PlacementType |  | prefab |
| `Procedural/Map/GridAsteroidPlacer.cs` | 33 | Procedural.Map | class GridAsteroidPlacer : MonoBehaviour |  | prefab |
| `Procedural/Naming/ShipNameGenerator.cs` | 153 | Procedural.Naming | class ShipNameGenerator |  |  |
| `Procedural/PlanetRings.cs` | 48 | Procedural | class PlanetRings |  |  |
| `Procedural/ProceduralMesh.cs` | 157 | Procedural | class ProceduralMesh |  |  |
| `Properties/AssemblyInfo.cs` | 6 |  |  |  |  |
| `Scripting/Editor/NodeGraphDebugViewer.cs` | 324 | Scripting.Editor | class NodeGraphDebugViewer : MonoBehaviour, ISelectList<br>class BaseGraphItem : SelectableListItem, IClearable where TGraph : ScriptingGraph<br>class TriggeredEventGraphItem : BaseGraphItem<TriggeredEventScriptingGraph><br>class ObjectiveGraphItem : BaseGraphItem<EvaluatePassFailScriptingGraph><br>class StrategyGraphItem : BaseGraphItem<AIStrategyScriptingGraph> | Awake, OnDestroy | prefab |
| `Scripting/Editor/NodeGraphEditorWindow.cs` | 302 | Scripting.Editor | class NodeGraphEditorWindow : WindowContent | Awake | prefab |
| `Scripting/Editor/ScriptingGraphManagerExtension.cs` | 453 | Scripting.Editor | class ScriptingGraphManagerExtension : MonoBehaviour, IPointerDownHandler, IEventSystemHandler | Awake, OnDisable, OnEnable, Update | prefab |
| `Scripting/Editor/ScriptingPortMatchRule.cs` | 28 | Scripting.Editor | class ScriptingPortMatchRule : PortMatchRule |  |  |
| `Scripting/Editor/ScriptingUINode.cs` | 617 | Scripting.Editor | class ScriptingUINode : MonoBehaviour<br>interface IPortItem<br>class InputPortItem : MonoBehaviour, IPortItem<br>class OutputPortItem : MonoBehaviour, IPortItem<br>class PortGroupItem : MonoBehaviour | Awake, OnDestroy | prefab |
| `Scripting/InputPortAttribute.cs` | 21 | Scripting | class InputPortAttribute : Attribute |  |  |
| `Scripting/IScriptingGraphAccess.cs` | 31 | Scripting | interface IScriptingGraphAccess |  |  |
| `Scripting/IScriptingGraphControls.cs` | 54 | Scripting | interface IScriptingGraphControls |  | prefab |
| `Scripting/MasterNodeScriptingGraph.cs` | 61 | Scripting | class MasterNodeScriptingGraph : ScriptingGraph where TMaster : MasterNode |  | XML |
| `Scripting/NodeBlockingAttribute.cs` | 9 | Scripting | class NodeBlockingAttribute : Attribute |  |  |
| `Scripting/NodeColorAttribute.cs` | 21 | Scripting | class NodeColorAttribute : Attribute |  |  |
| `Scripting/NodeGraphRestrictedTypeAttribute.cs` | 15 | Scripting | class NodeGraphRestrictedTypeAttribute : Attribute |  |  |
| `Scripting/NodePortTypeColorAttribute.cs` | 21 | Scripting | class NodePortTypeColorAttribute : Attribute |  |  |
| `Scripting/Nodes/AI/AIScriptingNode.cs` | 29 | Scripting.Nodes.AI | class AIScriptingNode : ScriptingNode |  |  |
| `Scripting/Nodes/AI/CollectFriendlyUnitsIf.cs` | 30 | Scripting.Nodes.AI | class CollectFriendlyUnitsIf : AIScriptingNode |  |  |
| `Scripting/Nodes/AI/CollectMissionsIf.cs` | 36 | Scripting.Nodes.AI | class CollectMissionsIf : AIScriptingNode |  |  |
| `Scripting/Nodes/AI/CraftMissions/CraftMissionGenerationNode.cs` | 88 | Scripting.Nodes.AI.CraftMissions | class CraftMissionGenerationNode : AIScriptingNode |  | XML |
| `Scripting/Nodes/AI/CraftMissions/InterceptEnemyFlights.cs` | 60 | Scripting.Nodes.AI.CraftMissions | class InterceptEnemyFlights : CraftMissionGenerationNode |  | XML |
| `Scripting/Nodes/AI/CraftMissions/PaintWithCraft.cs` | 30 | Scripting.Nodes.AI.CraftMissions | class PaintWithCraft : CraftMissionGenerationNode |  |  |
| `Scripting/Nodes/AI/CraftMissions/ProvideCAP.cs` | 26 | Scripting.Nodes.AI.CraftMissions | class ProvideCAP : CraftMissionGenerationNode |  |  |
| `Scripting/Nodes/AI/CraftMissions/ScoutWithCraft.cs` | 30 | Scripting.Nodes.AI.CraftMissions | class ScoutWithCraft : CraftMissionGenerationNode |  |  |
| `Scripting/Nodes/AI/CraftMissions/StageCraftInArea.cs` | 23 | Scripting.Nodes.AI.CraftMissions | class StageCraftInArea : CraftMissionGenerationNode |  |  |
| `Scripting/Nodes/AI/CraftMissions/StrikeWithCraft.cs` | 31 | Scripting.Nodes.AI.CraftMissions | class StrikeWithCraft |  |  |
| `Scripting/Nodes/AI/DeclareIndividualTaskUnits.cs` | 58 | Scripting.Nodes.AI | class DeclareIndividualTaskUnits |  |  |
| `Scripting/Nodes/AI/DeclareTaskUnit.cs` | 86 | Scripting.Nodes.AI | class DeclareTaskUnit |  |  |
| `Scripting/Nodes/AI/ForceCommandCycle.cs` | 28 | Scripting.Nodes.AI | class ForceCommandCycle : SimpleSequencedNode |  |  |
| `Scripting/Nodes/AI/FriendlyUnitsEliminated.cs` | 35 | Scripting.Nodes.AI | class FriendlyUnitsEliminated : AIScriptingNode |  |  |
| `Scripting/Nodes/AI/GetAIShip.cs` | 102 | Scripting.Nodes.AI | class GetAIShip : AIScriptingNode |  | XML, Addressables |
| `Scripting/Nodes/AI/Goals/AttackInSupportOf.cs` | 66 | Scripting.Nodes.AI.Goals | class AttackInSupportOf : GoalGenerationNodeSinglePriority |  |  |
| `Scripting/Nodes/AI/Goals/BasicAnnihilationGoal.cs` | 81 | Scripting.Nodes.AI.Goals | class BasicAnnihilationGoal |  |  |
| `Scripting/Nodes/AI/Goals/DefendDockingPoints.cs` | 57 | Scripting.Nodes.AI.Goals | class DefendDockingPoints |  |  |
| `Scripting/Nodes/AI/Goals/FireOffboardDeception.cs` | 111 | Scripting.Nodes.AI.Goals | class FireOffboardDeception |  | XML |
| `Scripting/Nodes/AI/Goals/GoalGenerationNode.cs` | 53 | Scripting.Nodes.AI.Goals | class GoalGenerationNode : AIScriptingNode |  |  |
| `Scripting/Nodes/AI/Goals/GoalGenerationNodeSinglePriority.cs` | 17 | Scripting.Nodes.AI.Goals | class GoalGenerationNodeSinglePriority : GoalGenerationNode |  |  |
| `Scripting/Nodes/AI/Goals/PickOffIsolated.cs` | 40 | Scripting.Nodes.AI.Goals | class PickOffIsolated |  |  |
| `Scripting/Nodes/AI/Goals/RetreatIneffectiveUnits.cs` | 38 | Scripting.Nodes.AI.Goals | class RetreatIneffectiveUnits : GoalGenerationNodeSinglePriority |  |  |
| `Scripting/Nodes/AI/Goals/SpotTargetsForFrontline.cs` | 27 | Scripting.Nodes.AI.Goals | class SpotTargetsForFrontline : GoalGenerationNodeSinglePriority |  |  |
| `Scripting/Nodes/AI/ITaskUnitDeclarationNode.cs` | 10 | Scripting.Nodes.AI | interface ITaskUnitDeclarationNode |  |  |
| `Scripting/Nodes/AI/Missions/AttackEnemyUnits.cs` | 21 | Scripting.Nodes.AI.Missions | class AttackEnemyUnits : TargetSetMissionNode |  |  |
| `Scripting/Nodes/AI/Missions/AttackFromArea.cs` | 31 | Scripting.Nodes.AI.Missions | class AttackFromArea : TargetSetMissionNode |  |  |
| `Scripting/Nodes/AI/Missions/DefendArea.cs` | 49 | Scripting.Nodes.AI.Missions | class DefendArea |  |  |
| `Scripting/Nodes/AI/Missions/EmptyMissionSet.cs` | 22 | Scripting.Nodes.AI.Missions | class EmptyMissionSet : AIScriptingNode |  |  |
| `Scripting/Nodes/AI/Missions/FireSupportFromArea.cs` | 35 | Scripting.Nodes.AI.Missions | class FireSupportFromArea : TargetSetMissionNode |  |  |
| `Scripting/Nodes/AI/Missions/HoldInArea.cs` | 29 | Scripting.Nodes.AI.Missions | class HoldInArea : MissionGenerationNode |  |  |
| `Scripting/Nodes/AI/Missions/MissionGenerationNode.cs` | 47 | Scripting.Nodes.AI.Missions | class MissionGenerationNode : AIScriptingNode |  |  |
| `Scripting/Nodes/AI/Missions/RetreatUnits.cs` | 34 | Scripting.Nodes.AI.Missions | class RetreatUnits : MissionGenerationNode |  |  |
| `Scripting/Nodes/AI/Missions/ScoutEnemyUnits.cs` | 19 | Scripting.Nodes.AI.Missions | class ScoutEnemyUnits : MissionGenerationNode |  |  |
| `Scripting/Nodes/AI/Missions/SpotTargetsFor.cs` | 21 | Scripting.Nodes.AI.Missions | class SpotTargetsFor : MissionGenerationNode |  |  |
| `Scripting/Nodes/AI/Missions/TargetSetMissionNode.cs` | 30 | Scripting.Nodes.AI.Missions | class TargetSetMissionNode : MissionGenerationNode |  |  |
| `Scripting/Nodes/AI/SelectEnemyShips.cs` | 54 | Scripting.Nodes.AI | class SelectEnemyShips<br>enum Operation |  |  |
| `Scripting/Nodes/AI/SelectFriendlyUnits.cs` | 62 | Scripting.Nodes.AI | class SelectFriendlyUnits : AIScriptingNode |  |  |
| `Scripting/Nodes/AI/SelectPursuingFriendlyUnits.cs` | 22 | Scripting.Nodes.AI | class SelectPursuingFriendlyUnits : AIScriptingNode |  |  |
| `Scripting/Nodes/AI/TaskUnitInfo.cs` | 101 | Scripting.Nodes.AI | class TaskUnitInfo : AIScriptingNode |  |  |
| `Scripting/Nodes/AI/TaskUnitSetInfo.cs` | 100 | Scripting.Nodes.AI | class TaskUnitSetInfo : AIScriptingNode |  |  |
| `Scripting/Nodes/AI/TaskUnitTarget.cs` | 84 | Scripting.Nodes.AI | class TaskUnitTarget : AIScriptingNode |  |  |
| `Scripting/Nodes/Camera/CurrentViewModeIs.cs` | 21 | Scripting.Nodes.Camera | class CurrentViewModeIs |  |  |
| `Scripting/Nodes/Camera/FaceCameraAngle.cs` | 35 | Scripting.Nodes.Camera | class FaceCameraAngle |  |  |
| `Scripting/Nodes/Camera/FreezeCameraInputs.cs` | 47 | Scripting.Nodes.Camera | class FreezeCameraInputs |  |  |
| `Scripting/Nodes/Camera/PositionCamera.cs` | 66 | Scripting.Nodes.Camera | class PositionCamera<br>enum Mode |  |  |
| `Scripting/Nodes/Camera/SetViewMode.cs` | 24 | Scripting.Nodes.Camera | class SetViewMode |  |  |
| `Scripting/Nodes/CommonNodeColors.cs` | 29 | Scripting.Nodes | class CommonNodeColors |  |  |
| `Scripting/Nodes/Craft/BaseLaunchCraftGroup.cs` | 165 | Scripting.Nodes.Craft | class BaseLaunchCraftGroup : SimpleSequencedNode, IInspectable |  | save-state, XML |
| `Scripting/Nodes/Craft/FlightSetDetails.cs` | 55 | Scripting.Nodes.Craft | class FlightSetDetails : ScriptingNode |  |  |
| `Scripting/Nodes/Craft/ImmediateLaunchCraftGroup.cs` | 40 | Scripting.Nodes.Craft | class ImmediateLaunchCraftGroup : BaseLaunchCraftGroup |  |  |
| `Scripting/Nodes/Craft/LaunchCraftGroup.cs` | 30 | Scripting.Nodes.Craft | class LaunchCraftGroup : BaseLaunchCraftGroup |  |  |
| `Scripting/Nodes/Craft/SelectCraftFlights.cs` | 53 | Scripting.Nodes.Craft | class SelectCraftFlights : ScriptingNode |  | XML |
| `Scripting/Nodes/Difficulty/DifficultySwitch.cs` | 44 | Scripting.Nodes.Difficulty | class DifficultySwitch : ScriptingNode |  |  |
| `Scripting/Nodes/Difficulty/FloatByDifficulty.cs` | 10 | Scripting.Nodes.Difficulty | class FloatByDifficulty : DifficultySwitch<float> |  |  |
| `Scripting/Nodes/Difficulty/IntByDifficulty.cs` | 10 | Scripting.Nodes.Difficulty | class IntByDifficulty : DifficultySwitch<int> |  |  |
| `Scripting/Nodes/Entities/BaseEntityNode.cs` | 45 | Scripting.Nodes.Entities | class BaseEntityNode : ScriptingNode |  | XML |
| `Scripting/Nodes/Entities/EntityInfo.cs` | 175 | Scripting.Nodes.Entities | class EntityInfo : BaseEntityNode |  |  |
| `Scripting/Nodes/Entities/EntitySequencedNode.cs` | 46 | Scripting.Nodes.Entities | class EntitySequencedNode : SimpleSequencedNode |  | XML |
| `Scripting/Nodes/Entities/PlayEntityAnimation.cs` | 50 | Scripting.Nodes.Entities | class PlayEntityAnimation |  |  |
| `Scripting/Nodes/Entities/SelectRoomEntity.cs` | 48 | Scripting.Nodes.Entities | class SelectRoomEntity : ScriptingNode |  | XML |
| `Scripting/Nodes/Entities/SelectWorldArea.cs` | 48 | Scripting.Nodes.Entities | class SelectWorldArea : ScriptingNode |  | XML |
| `Scripting/Nodes/Entities/SetEntityActive.cs` | 33 | Scripting.Nodes.Entities | class SetEntityActive : EntitySequencedNode |  |  |
| `Scripting/Nodes/Entities/ToggleEntityPhysics.cs` | 34 | Scripting.Nodes.Entities | class ToggleEntityPhysics : EntitySequencedNode |  |  |
| `Scripting/Nodes/Entities/ToggleEntitySignatures.cs` | 37 | Scripting.Nodes.Entities | class ToggleEntitySignatures : EntitySequencedNode |  |  |
| `Scripting/Nodes/General/AutoSaveGame.cs` | 19 | Scripting.Nodes.General | class AutoSaveGame : SimpleSequencedNode |  |  |
| `Scripting/Nodes/General/GameTime.cs` | 45 | Scripting.Nodes.General | class GameTime : ScriptingNode |  |  |
| `Scripting/Nodes/General/OtherGraphState.cs` | 52 | Scripting.Nodes.General | class OtherGraphState : ScriptingNode |  | XML |
| `Scripting/Nodes/General/PreventInterruption.cs` | 22 | Scripting.Nodes.General | class PreventInterruption : SimpleSequencedNode |  |  |
| `Scripting/Nodes/General/SetTimeControl.cs` | 20 | Scripting.Nodes.General | class SetTimeControl |  |  |
| `Scripting/Nodes/General/WorldDirection.cs` | 45 | Scripting.Nodes.General | class WorldDirection : ScriptingNode |  |  |
| `Scripting/Nodes/General/WorldOrbitPath.cs` | 83 | Scripting.Nodes.General | class WorldOrbitPath |  |  |
| `Scripting/Nodes/General/WorldPosition.cs` | 40 | Scripting.Nodes.General | class WorldPosition |  |  |
| `Scripting/Nodes/Interface/HighlightUIElement.cs` | 25 | Scripting.Nodes.Interface | class HighlightUIElement : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Logic/Comparison.cs` | 54 | Scripting.Nodes.Logic | class Comparison : ScriptingNode where T : IComparable<T><br>enum Operation |  |  |
| `Scripting/Nodes/Logic/DifficultySwitch.cs` | 43 | Scripting.Nodes.Logic | class DifficultySwitch : ScriptingNode |  |  |
| `Scripting/Nodes/Logic/FloatComparison.cs` | 10 | Scripting.Nodes.Logic | class FloatComparison : Comparison<float> |  |  |
| `Scripting/Nodes/Logic/FloatDifficultySwitch.cs` | 10 | Scripting.Nodes.Logic | class FloatDifficultySwitch : DifficultySwitch<float> |  |  |
| `Scripting/Nodes/Logic/IntComparison.cs` | 10 | Scripting.Nodes.Logic | class IntComparison : Comparison<int> |  |  |
| `Scripting/Nodes/Logic/IntDifficultySwitch.cs` | 10 | Scripting.Nodes.Logic | class IntDifficultySwitch : DifficultySwitch<int> |  |  |
| `Scripting/Nodes/Logic/Invert.cs` | 26 | Scripting.Nodes.Logic | class Invert : ScriptingNode |  |  |
| `Scripting/Nodes/Logic/Latch.cs` | 56 | Scripting.Nodes.Logic | class Latch<br>enum Condition |  | XML |
| `Scripting/Nodes/Logic/Logic.cs` | 50 | Scripting.Nodes.Logic | class Logic : ScriptingNode<br>enum Operation |  |  |
| `Scripting/Nodes/MasterNode.cs` | 12 | Scripting.Nodes | class MasterNode : ScriptingNode |  |  |
| `Scripting/Nodes/Masters/AIStrategyMaster.cs` | 78 | Scripting.Nodes.Masters | class AIStrategyMaster : MasterNode |  |  |
| `Scripting/Nodes/Masters/AIStrategyScriptingGraph.cs` | 290 | Scripting.Nodes.Masters | class AIStrategyScriptingGraph : MasterNodeScriptingGraph<AIStrategyMaster><br>class Wrapper : AnnihilationStrategy |  | XML |
| `Scripting/Nodes/Masters/CutsceneEventMaster.cs` | 53 | Scripting.Nodes.Masters | class CutsceneEventMaster : MasterNode |  |  |
| `Scripting/Nodes/Masters/CutsceneEventScriptingGraph.cs` | 55 | Scripting.Nodes.Masters | class CutsceneEventScriptingGraph : MasterNodeScriptingGraph<CutsceneEventMaster> |  | XML |
| `Scripting/Nodes/Masters/EvaluateBooleanMaster.cs` | 19 | Scripting.Nodes.Masters | class EvaluateBooleanMaster : MasterNode |  |  |
| `Scripting/Nodes/Masters/EvaluateBooleanScriptingGraph.cs` | 23 | Scripting.Nodes.Masters | class EvaluateBooleanScriptingGraph : MasterNodeScriptingGraph<EvaluateBooleanMaster> |  |  |
| `Scripting/Nodes/Masters/EvaluatePassFailMaster.cs` | 73 | Scripting.Nodes.Masters | class EvaluatePassFailMaster<br>enum Result |  |  |
| `Scripting/Nodes/Masters/EvaluatePassFailScriptingGraph.cs` | 35 | Scripting.Nodes.Masters | class EvaluatePassFailScriptingGraph : MasterNodeScriptingGraph<EvaluatePassFailMaster> |  |  |
| `Scripting/Nodes/Masters/TriggeredEventMaster.cs` | 45 | Scripting.Nodes.Masters | class TriggeredEventMaster |  |  |
| `Scripting/Nodes/Masters/TriggeredEventScriptingGraph.cs` | 112 | Scripting.Nodes.Masters | class TriggeredEventScriptingGraph : MasterNodeScriptingGraph<TriggeredEventMaster> |  | XML |
| `Scripting/Nodes/Math/Arithmetic.cs` | 48 | Scripting.Nodes.Math | class Arithmetic : ScriptingNode<br>enum Operation |  |  |
| `Scripting/Nodes/Math/DifficultySwitch.cs` | 6 | Scripting.Nodes.Math | class DifficultySwitch |  |  |
| `Scripting/Nodes/Math/DistanceVec2.cs` | 31 | Scripting.Nodes.Math | class DistanceVec2 : ScriptingNode |  |  |
| `Scripting/Nodes/Math/DistanceVec3.cs` | 31 | Scripting.Nodes.Math | class DistanceVec3 : ScriptingNode |  |  |
| `Scripting/Nodes/Math/FloatMath.cs` | 23 | Scripting.Nodes.Math | class FloatMath : Arithmetic<float, float> |  |  |
| `Scripting/Nodes/Math/FloatSum.cs` | 17 | Scripting.Nodes.Math | class FloatSum : Sum<float> |  |  |
| `Scripting/Nodes/Math/FloatToIntMath.cs` | 46 | Scripting.Nodes.Math | class FloatToIntMath<br>enum RoundMethod |  |  |
| `Scripting/Nodes/Math/IntMath.cs` | 23 | Scripting.Nodes.Math | class IntMath : Arithmetic<int, int> |  |  |
| `Scripting/Nodes/Math/IntSum.cs` | 17 | Scripting.Nodes.Math | class IntSum : Sum<int> |  |  |
| `Scripting/Nodes/Math/IntToFloatMath.cs` | 23 | Scripting.Nodes.Math | class IntToFloatMath : Arithmetic<int, float> |  |  |
| `Scripting/Nodes/Math/MakeVector2.cs` | 31 | Scripting.Nodes.Math | class MakeVector2 : ScriptingNode |  |  |
| `Scripting/Nodes/Math/MakeVector3.cs` | 36 | Scripting.Nodes.Math | class MakeVector3 : ScriptingNode |  |  |
| `Scripting/Nodes/Math/NormalizeVec2.cs` | 26 | Scripting.Nodes.Math | class NormalizeVec2 : ScriptingNode |  |  |
| `Scripting/Nodes/Math/NormalizeVec3.cs` | 26 | Scripting.Nodes.Math | class NormalizeVec3 : ScriptingNode |  |  |
| `Scripting/Nodes/Math/PointInsideArea.cs` | 54 | Scripting.Nodes.Math | class PointInsideArea : ScriptingNode |  |  |
| `Scripting/Nodes/Math/RandomRangeFloat.cs` | 32 | Scripting.Nodes.Math | class RandomRangeFloat : ScriptingNode |  |  |
| `Scripting/Nodes/Math/RandomRangeInt.cs` | 32 | Scripting.Nodes.Math | class RandomRangeInt : ScriptingNode |  |  |
| `Scripting/Nodes/Math/SplitVector3.cs` | 42 | Scripting.Nodes.Math | class SplitVector3 : ScriptingNode |  |  |
| `Scripting/Nodes/Math/Sum.cs` | 26 | Scripting.Nodes.Math | class Sum : ScriptingNode |  |  |
| `Scripting/Nodes/Math/Vector3Math.cs` | 58 | Scripting.Nodes.Math | class Vector3Math : ScriptingNode<br>enum Operation |  |  |
| `Scripting/Nodes/Music/ChangeDynamicMusicSet.cs` | 43 | Scripting.Nodes.Music | class ChangeDynamicMusicSet : SimpleSequencedNode |  | XML, Addressables |
| `Scripting/Nodes/Music/ClearMusicOverrideLayer.cs` | 22 | Scripting.Nodes.Music | class ClearMusicOverrideLayer |  |  |
| `Scripting/Nodes/Music/EnableDynamicMusicSwitching.cs` | 25 | Scripting.Nodes.Music | class EnableDynamicMusicSwitching : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Music/PlayMusicTrack.cs` | 53 | Scripting.Nodes.Music | class PlayMusicTrack : SimpleSequencedNode where TTrack : MusicTrack |  | XML, Addressables |
| `Scripting/Nodes/Music/PlaySegmentedMusicTrack.cs` | 12 | Scripting.Nodes.Music | class PlaySegmentedMusicTrack : PlayMusicTrack<SegmentedMusicTrack> |  |  |
| `Scripting/Nodes/Music/PlaySingleMusicTrack.cs` | 12 | Scripting.Nodes.Music | class PlaySingleMusicTrack : PlayMusicTrack<SingleMusicTrack> |  |  |
| `Scripting/Nodes/Narrative/ClearRouteOnMap.cs` | 20 | Scripting.Nodes.Narrative | class ClearRouteOnMap : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Narrative/DeletePOI.cs` | 49 | Scripting.Nodes.Narrative | class DeletePOI : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Narrative/EnablePlayerRoutePlottingUI.cs` | 20 | Scripting.Nodes.Narrative | class EnablePlayerRoutePlottingUI |  |  |
| `Scripting/Nodes/Narrative/EndCampaign.cs` | 58 | Scripting.Nodes.Narrative | class EndCampaign : SimpleSequencedNode |  | XML, Addressables |
| `Scripting/Nodes/Narrative/EndOverworldMapBriefing.cs` | 25 | Scripting.Nodes.Narrative | class EndOverworldMapBriefing |  |  |
| `Scripting/Nodes/Narrative/IndicatePositionsFromDialog.cs` | 49 | Scripting.Nodes.Narrative | class IndicatePositionsFromDialog : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Narrative/IndicateRoomsOnMap.cs` | 51 | Scripting.Nodes.Narrative | class IndicateRoomsOnMap |  |  |
| `Scripting/Nodes/Narrative/MainUIScreenBlackout.cs` | 29 | Scripting.Nodes.Narrative | class MainUIScreenBlackout |  |  |
| `Scripting/Nodes/Narrative/MapTracks/SelectMapTrack.cs` | 49 | Scripting.Nodes.Narrative.MapTracks | class SelectMapTrack : ScriptingNode |  | XML |
| `Scripting/Nodes/Narrative/MapTracks/ShowMapTracks.cs` | 28 | Scripting.Nodes.Narrative.MapTracks | class ShowMapTracks |  |  |
| `Scripting/Nodes/Narrative/MapTracks/StartTrackMotion.cs` | 48 | Scripting.Nodes.Narrative.MapTracks | class StartTrackMotion |  |  |
| `Scripting/Nodes/Narrative/MapTracks/StartTrackPoint2PointMotion.cs` | 53 | Scripting.Nodes.Narrative.MapTracks | class StartTrackPoint2PointMotion |  |  |
| `Scripting/Nodes/Narrative/MapTracks/StepTrackMotion.cs` | 30 | Scripting.Nodes.Narrative.MapTracks | class StepTrackMotion : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Narrative/MarkShipCallout.cs` | 38 | Scripting.Nodes.Narrative | class MarkShipCallout : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Narrative/PlaceCallout.cs` | 32 | Scripting.Nodes.Narrative | class PlaceCallout : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Narrative/PlacePOI.cs` | 70 | Scripting.Nodes.Narrative | class PlacePOI |  |  |
| `Scripting/Nodes/Narrative/PlayAnimatic.cs` | 90 | Scripting.Nodes.Narrative | class PlayAnimatic : SimpleSequencedNode |  | XML, Addressables |
| `Scripting/Nodes/Narrative/PlayCutscene.cs` | 158 | Scripting.Nodes.Narrative | class PlayCutscene : SimpleSequencedNode |  | XML |
| `Scripting/Nodes/Narrative/PlayDialogClip.cs` | 112 | Scripting.Nodes.Narrative | class PlayDialogClip : SimpleSequencedNode |  | XML |
| `Scripting/Nodes/Narrative/PlayDialogClipChoice.cs` | 55 | Scripting.Nodes.Narrative | class PlayDialogClipChoice |  | XML |
| `Scripting/Nodes/Narrative/PlotRouteOnMap.cs` | 33 | Scripting.Nodes.Narrative | class PlotRouteOnMap |  |  |
| `Scripting/Nodes/Narrative/PlotWaypointRouteOnMap.cs` | 31 | Scripting.Nodes.Narrative | class PlotWaypointRouteOnMap |  |  |
| `Scripting/Nodes/Narrative/StartOverworldMapBriefing.cs` | 19 | Scripting.Nodes.Narrative | class StartOverworldMapBriefing : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Narrative/TransferShipOwner.cs` | 58 | Scripting.Nodes.Narrative | class TransferShipOwner : SimpleSequencedNode |  | XML |
| `Scripting/Nodes/Objectives/ClearObjectiveTimer.cs` | 23 | Scripting.Nodes.Objectives | class ClearObjectiveTimer : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Objectives/CompleteObjective.cs` | 34 | Scripting.Nodes.Objectives | class CompleteObjective : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Objectives/ObjectiveInfo.cs` | 102 | Scripting.Nodes.Objectives | class ObjectiveInfo : ScriptingNode |  |  |
| `Scripting/Nodes/Objectives/OverrideObjectiveName.cs` | 30 | Scripting.Nodes.Objectives | class OverrideObjectiveName : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Objectives/ResetObjectiveName.cs` | 24 | Scripting.Nodes.Objectives | class ResetObjectiveName : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Objectives/RevealObjective.cs` | 27 | Scripting.Nodes.Objectives | class RevealObjective : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Objectives/SelectObjective.cs` | 49 | Scripting.Nodes.Objectives | class SelectObjective : ScriptingNode |  | XML |
| `Scripting/Nodes/Objectives/StartObjectiveTimer.cs` | 29 | Scripting.Nodes.Objectives | class StartObjectiveTimer |  |  |
| `Scripting/Nodes/Players/FactionFleetInfo.cs` | 146 | Scripting.Nodes.Players | class FactionFleetInfo : ScriptingNode |  | XML |
| `Scripting/Nodes/Players/GetLocalPlayer.cs` | 20 | Scripting.Nodes.Players | class GetLocalPlayer : ScriptingNode |  |  |
| `Scripting/Nodes/Players/GetNPCFactionPlayer.cs` | 53 | Scripting.Nodes.Players | class GetNPCFactionPlayer : ScriptingNode |  | XML |
| `Scripting/Nodes/Players/SetAllBotsPaused.cs` | 31 | Scripting.Nodes.Players | class SetAllBotsPaused : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Players/SetBotPaused.cs` | 33 | Scripting.Nodes.Players | class SetBotPaused : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Room/ArrivedFromRoom.cs` | 30 | Scripting.Nodes.Room | class ArrivedFromRoom : ScriptingNode |  |  |
| `Scripting/Nodes/Room/GetRoom.cs` | 78 | Scripting.Nodes.Room | class GetRoom : ScriptingNode |  | XML |
| `Scripting/Nodes/Room/PlayerDepartedDestination.cs` | 41 | Scripting.Nodes.Room | class PlayerDepartedDestination : PlayerDepartedRoom |  |  |
| `Scripting/Nodes/Room/PlayerDepartedRoom.cs` | 38 | Scripting.Nodes.Room | class PlayerDepartedRoom : ScriptingNode |  |  |
| `Scripting/Nodes/Room/RoomVisitCount.cs` | 22 | Scripting.Nodes.Room | class RoomVisitCount : ScriptingNode |  |  |
| `Scripting/Nodes/ScriptingNode.cs` | 837 | Scripting.Nodes | class ScriptingNode : IXmlDocSerializable, IModDependencySource<br>struct InputPortInfo<br>struct OutputPortInfo<br>class NodeTypeInfo<br>class NodePort<br>struct SavedConnection<br>class NodeSet |  | XML |
| `Scripting/Nodes/SequenceFlow/ParallelSequence.cs` | 120 | Scripting.Nodes.SequenceFlow | class ParallelSequence : SequencedNode |  | XML |
| `Scripting/Nodes/SequenceFlow/SequenceBranch.cs` | 51 | Scripting.Nodes.SequenceFlow | class SequenceBranch : SequencedNode |  |  |
| `Scripting/Nodes/SequenceFlow/SequenceDifficultySwitch.cs` | 58 | Scripting.Nodes.SequenceFlow | class SequenceDifficultySwitch : SequencedNode |  |  |
| `Scripting/Nodes/SequenceFlow/SequencedNode.cs` | 59 | Scripting.Nodes.SequenceFlow | class SequencedNode : ScriptingNode |  |  |
| `Scripting/Nodes/SequenceFlow/SequenceForLoop.cs` | 100 | Scripting.Nodes.SequenceFlow | class SequenceForLoop |  | XML |
| `Scripting/Nodes/SequenceFlow/SequenceSwitch.cs` | 48 | Scripting.Nodes.SequenceFlow | class SequenceSwitch : SequencedNode |  |  |
| `Scripting/Nodes/SequenceFlow/SimpleSequencedNode.cs` | 26 | Scripting.Nodes.SequenceFlow | class SimpleSequencedNode : SequencedNode |  |  |
| `Scripting/Nodes/SequenceFlow/WaitTime.cs` | 62 | Scripting.Nodes.SequenceFlow | class WaitTime |  | XML |
| `Scripting/Nodes/SequenceFlow/WaitUntil.cs` | 24 | Scripting.Nodes.SequenceFlow | class WaitUntil : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Ships/DistancesFromStart.cs` | 74 | Scripting.Nodes.Ships | class DistancesFromStart : ScriptingNode |  |  |
| `Scripting/Nodes/Ships/ForceEliminateShip.cs` | 29 | Scripting.Nodes.Ships | class ForceEliminateShip |  |  |
| `Scripting/Nodes/Ships/GetAnyShip.cs` | 89 | Scripting.Nodes.Ships | class GetAnyShip : ScriptingNode |  | XML |
| `Scripting/Nodes/Ships/GetRoomShip.cs` | 83 | Scripting.Nodes.Ships | class GetRoomShip : ScriptingNode |  | XML |
| `Scripting/Nodes/Ships/GetShipPart.cs` | 82 | Scripting.Nodes.Ships | class GetShipPart : ScriptingNode, IInspectable |  | XML |
| `Scripting/Nodes/Ships/GetShipTrack.cs` | 40 | Scripting.Nodes.Ships | class GetShipTrack : ScriptingNode |  |  |
| `Scripting/Nodes/Ships/GetShipWeaponGroup.cs` | 113 | Scripting.Nodes.Ships | class GetShipWeaponGroup : ScriptingNode, IInspectable |  | XML |
| `Scripting/Nodes/Ships/MissilesDetected.cs` | 76 | Scripting.Nodes.Ships | class MissilesDetected |  |  |
| `Scripting/Nodes/Ships/OrderCeaseFire.cs` | 27 | Scripting.Nodes.Ships | class OrderCeaseFire : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Ships/OrderFireWeapons.cs` | 140 | Scripting.Nodes.Ships | class OrderFireWeapons : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Ships/OrderHoldHeading.cs` | 68 | Scripting.Nodes.Ships | class OrderHoldHeading : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Ships/OrderLockTarget.cs` | 54 | Scripting.Nodes.Ships | class OrderLockTarget : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Ships/OrderShipFormation.cs` | 65 | Scripting.Nodes.Ships | class OrderShipFormation : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Ships/OrderShipMove.cs` | 56 | Scripting.Nodes.Ships | class OrderShipMove : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Ships/OrderShipOrbit.cs` | 83 | Scripting.Nodes.Ships | class OrderShipOrbit : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Ships/SetComponentDamage.cs` | 36 | Scripting.Nodes.Ships | class SetComponentDamage |  |  |
| `Scripting/Nodes/Ships/SetPowerplantScram.cs` | 33 | Scripting.Nodes.Ships | class SetPowerplantScram : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Ships/SetShipBattleshort.cs` | 28 | Scripting.Nodes.Ships | class SetShipBattleshort |  |  |
| `Scripting/Nodes/Ships/SetShipPDSettings.cs` | 83 | Scripting.Nodes.Ships | class SetShipPDSettings |  |  |
| `Scripting/Nodes/Ships/SetShipWCON.cs` | 29 | Scripting.Nodes.Ships | class SetShipWCON |  |  |
| `Scripting/Nodes/Ships/ShipCarrierStatus.cs` | 113 | Scripting.Nodes.Ships | class ShipCarrierStatus : ScriptingNode |  |  |
| `Scripting/Nodes/Ships/ShipDamageStatus.cs` | 94 | Scripting.Nodes.Ships | class ShipDamageStatus : ScriptingNode |  |  |
| `Scripting/Nodes/Ships/ShipDockedTo.cs` | 51 | Scripting.Nodes.Ships | class ShipDockedTo : ScriptingNode |  |  |
| `Scripting/Nodes/Ships/ShipInTransitBurn.cs` | 54 | Scripting.Nodes.Ships | class ShipInTransitBurn : ScriptingNode |  |  |
| `Scripting/Nodes/Ships/ShipMovement.cs` | 148 | Scripting.Nodes.Ships | class ShipMovement : ScriptingNode |  |  |
| `Scripting/Nodes/Ships/ShipSalvoInfo.cs` | 82 | Scripting.Nodes.Ships | class ShipSalvoInfo : ScriptingNode |  |  |
| `Scripting/Nodes/Ships/ShipSetDetails.cs` | 73 | Scripting.Nodes.Ships | class ShipSetDetails : ScriptingNode |  |  |
| `Scripting/Nodes/Ships/ShipsInsideArea.cs` | 121 | Scripting.Nodes.Ships | class ShipsInsideArea<br>enum Condition |  |  |
| `Scripting/Nodes/Ships/TriggerShipGroupArrival.cs` | 163 | Scripting.Nodes.Ships | class TriggerShipGroupArrival<br>enum ArrivalDirection |  | XML |
| `Scripting/Nodes/Ships/TriggerSingleShipArrival.cs` | 80 | Scripting.Nodes.Ships | class TriggerSingleShipArrival : SimpleSequencedNode<br>enum ArrivalDirection |  | XML |
| `Scripting/Nodes/Ships/UndockShips.cs` | 31 | Scripting.Nodes.Ships | class UndockShips : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Ships/WasShipCollected.cs` | 37 | Scripting.Nodes.Ships | class WasShipCollected : ScriptingNode |  |  |
| `Scripting/Nodes/Ships/WasShipDestroyed.cs` | 47 | Scripting.Nodes.Ships | class WasShipDestroyed |  |  |
| `Scripting/Nodes/Ships/WeaponGroupInfo.cs` | 63 | Scripting.Nodes.Ships | class WeaponGroupInfo : ScriptingNode |  |  |
| `Scripting/Nodes/Stations/AddAmmoToStation.cs` | 91 | Scripting.Nodes.Stations | class AddAmmoToStation |  | XML |
| `Scripting/Nodes/Stations/AddCraftToStation.cs` | 89 | Scripting.Nodes.Stations | class AddCraftToStation |  | XML |
| `Scripting/Nodes/Stations/SelectDockingPoint.cs` | 81 | Scripting.Nodes.Stations | class SelectDockingPoint : ScriptingNode |  | XML |
| `Scripting/Nodes/Stations/SelectStation.cs` | 74 | Scripting.Nodes.Stations | class SelectStation : ScriptingNode |  | XML |
| `Scripting/Nodes/Stations/SelectStationComponent.cs` | 61 | Scripting.Nodes.Stations | class SelectStationComponent : ScriptingNode |  | XML |
| `Scripting/Nodes/Stations/SetDockingPointEnabled.cs` | 42 | Scripting.Nodes.Stations | class SetDockingPointEnabled : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Stations/SetDockingPointVisible.cs` | 42 | Scripting.Nodes.Stations | class SetDockingPointVisible : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Stations/SetIgnoreBoardingShips.cs` | 33 | Scripting.Nodes.Stations | class SetIgnoreBoardingShips : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Stations/SetStationBoardingEnabled.cs` | 33 | Scripting.Nodes.Stations | class SetStationBoardingEnabled : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Stations/StationDockingStatus.cs` | 58 | Scripting.Nodes.Stations | class StationDockingStatus : ScriptingNode |  |  |
| `Scripting/Nodes/Stations/TransferStationOwner.cs` | 60 | Scripting.Nodes.Stations | class TransferStationOwner : SimpleSequencedNode |  | XML |
| `Scripting/Nodes/Variables/ReadRoomVar.cs` | 27 | Scripting.Nodes.Variables | class ReadRoomVar : ScriptingNode |  |  |
| `Scripting/Nodes/Variables/ReadVarBool.cs` | 10 | Scripting.Nodes.Variables | class ReadVarBool : ReadRoomVar<bool> |  |  |
| `Scripting/Nodes/Variables/ReadVarInt.cs` | 10 | Scripting.Nodes.Variables | class ReadVarInt : ReadRoomVar<int> |  |  |
| `Scripting/Nodes/Variables/ReadVarSingle.cs` | 10 | Scripting.Nodes.Variables | class ReadVarSingle : ReadRoomVar<float> |  |  |
| `Scripting/Nodes/Variables/SelectGlobalVar.cs` | 48 | Scripting.Nodes.Variables | class SelectGlobalVar : ScriptingNode |  | XML |
| `Scripting/Nodes/Variables/SelectRoomVar.cs` | 48 | Scripting.Nodes.Variables | class SelectRoomVar : ScriptingNode |  | XML |
| `Scripting/Nodes/Variables/WriteVar.cs` | 33 | Scripting.Nodes.Variables | class WriteVar : SimpleSequencedNode |  |  |
| `Scripting/Nodes/Variables/WriteVarBool.cs` | 10 | Scripting.Nodes.Variables | class WriteVarBool : WriteVar<bool> |  |  |
| `Scripting/Nodes/Variables/WriteVarInt.cs` | 10 | Scripting.Nodes.Variables | class WriteVarInt : WriteVar<int> |  |  |
| `Scripting/Nodes/Variables/WriteVarSingle.cs` | 10 | Scripting.Nodes.Variables | class WriteVarSingle : WriteVar<float> |  |  |
| `Scripting/NodeTooltipAttribute.cs` | 15 | Scripting | class NodeTooltipAttribute : Attribute |  |  |
| `Scripting/NodeWidthAttribute.cs` | 15 | Scripting | class NodeWidthAttribute : Attribute |  |  |
| `Scripting/OutputPortAttribute.cs` | 18 | Scripting | class OutputPortAttribute : Attribute |  |  |
| `Scripting/ScriptingGraph.cs` | 444 | Scripting | class ScriptingGraph : IXmlDocSerializable, IXmlSaveState<Guid>, ICampaignGUIDObject, IModDependencySource<br>class Clipboard : IXmlDocSerializable<br>class GraphSaveHelper : XmlSaveStateHelperWithID<ScriptingGraph, Guid> |  | save-state, XML |
| `Scripting/ScriptingGraphCollection.cs` | 146 | Scripting | class ScriptingGraphCollection : IXmlDocSerializable |  | XML |
| `Scripting/SelectionSet.cs` | 216 | Scripting | class SelectionSet : IEnumerable<T>, IEnumerable<br>struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable |  |  |
| `Scripting/TypeColoring.cs` | 112 | Scripting | class TypeColoring |  |  |
| `Ships/AccuracyMuzzle.cs` | 42 | Ships | class AccuracyMuzzle : Muzzle |  |  |
| `Ships/ActiveFireControlSensor.cs` | 166 | Ships | class ActiveFireControlSensor : FireControlSensor, ITuneableSensor, ISignalEmitter | Awake, OnDestroy |  |
| `Ships/AmmoFeeder.cs` | 181 | Ships | class AmmoFeeder |  |  |
| `Ships/AnimatedSectorSensorPart.cs` | 45 | Ships | class AnimatedSectorSensorPart : SectorSensorPart | Awake |  |
| `Ships/ArmorDamage.cs` | 6 | Ships |  |  |  |
| `Ships/BallisticRaycastMuzzle.cs` | 316 | Ships | class BallisticRaycastMuzzle : AccuracyMuzzle, IRangedMuzzle<br>class RaycastBullet | Awake, FixedUpdate | prefab |
| `Ships/BanditLauncherComponent.cs` | 428 | Ships | class BanditLauncherComponent : BaseTurretedLauncherComponent<br>interface IBanditLauncherRPC : ITurretedLauncherRPC, ICellLauncherRPC, IHullComponentRPC, IHullPartRPC, IHullComponentRouter<br>class BanditLauncherState : TurretedLauncherState |  |  |
| `Ships/BarrelGlow.cs` | 51 | Ships | class BarrelGlow : MonoBehaviour | Update |  |
| `Ships/BaseActiveSensorComponent.cs` | 491 | Ships | class BaseActiveSensorComponent : HullComponent, ISensorComponent, IDeltaSensor, ISensor, IJammable, IEWarTarget, IOwned, IHullComponent, ITuneableSensor, ISignalEmitter<br>interface ISensorComponentRPC : IHullComponentRPC, IHullPartRPC, IHullComponentRouter | Awake, OnDestroy, OnDisable, OnEnable | prefab |
| `Ships/BaseCellLauncherComponent.cs` | 1128 | Ships | class BaseCellLauncherComponent : HullComponent, IMissileLauncher, IWeapon, IHullComponent, INeedsShipIdentity, IMagazineProvider, ISimpleStorageContainer, IStorageContain...<br>interface ICellLauncherRPC : IHullComponentRPC, IHullPartRPC, IHullComponentRouter<br>class CellLauncherData : ComponentSaveData, IMagazineLoadComponentData<br>class CellLauncherState : SavedHullComponentStates.StateElement | FixedUpdate, OnDestroy, Start, Update | save-state, XML |
| `Ships/BaseCellMissileMagazine.cs` | 417 | Ships | class BaseCellMissileMagazine : ICellMissileMagazine, IMagazineProvider, ISimpleStorageContainer, IStorageContainer |  |  |
| `Ships/BaseColliderSampler.cs` | 247 | Ships | class BaseColliderSampler : MonoBehaviour | Start | prefab |
| `Ships/BaseCraftHangarComponent.cs` | 549 | Ships | class BaseCraftHangarComponent : CrewOperatedComponent, ICraftHangar, IInstanceStorageContainer, IStorageContainer, ICraftWorkSlotProvider, IQuantityHolder<br>class CraftHangarData : ComponentSaveData<br>class CraftHangarState : SavedHullComponentStates.StateElement<br>class HangarPostflightSlot : ICraftWorkSlot<br>class HangarComponentReference : CraftHangarReference |  | NetworkServer, save-state, XML |
| `Ships/BaseHull.cs` | 1797 | Ships | class BaseHull : MonoBehaviour, IPlatform, IBundleKeyed, ISaveKeyed, IModSource, IModifierSource, IFactionLocked, IShoulderCamMount, INetworkSpawnerRegist...<br>struct PlateBakingNegative<br>class ArmorZone<br>struct Aspects<br>struct DockingPoint<br>struct ExtraBoardingPoint<br>struct DockingClamp<br>enum ArmorSystem<br>class HullConfiguration | LateUpdate, OnDestroy | prefab |
| `Ships/BaseTubeLauncherComponent.cs` | 634 | Ships | class BaseTubeLauncherComponent : CycledComponent, IWeapon, IHullComponent, INeedsShipIdentity, LauncherProgrammingQueue.ILauncherProgCallbacks<br>interface ITubeLauncherRPC : ICycledComponentRPC, IHullComponentRPC, IHullPartRPC, IHullComponentRouter | OnDestroy, Update |  |
| `Ships/BaseTurretedLauncherComponent.cs` | 376 | Ships | class BaseTurretedLauncherComponent : BaseCellLauncherComponent, SettingsSequentialOptionList.IOptionList<br>interface ITurretedLauncherRPC : ICellLauncherRPC, IHullComponentRPC, IHullPartRPC, IHullComponentRouter<br>class TurretedLauncherData : CellLauncherData<br>class TurretedLauncherState : CellLauncherState | FixedUpdate | save-state |
| `Ships/BeamMuzzleEffects.cs` | 167 | Ships | class BeamMuzzleEffects : MonoBehaviour | Awake, Update |  |
| `Ships/BerthingComponent.cs` | 26 | Ships | class BerthingComponent : CrewedComponent |  |  |
| `Ships/BulkCraftHangarComponent.cs` | 171 | Ships | class BulkCraftHangarComponent : BaseCraftHangarComponent, ISyncCraftHangar |  | NetworkServer |
| `Ships/BulkMagazineComponent.cs` | 505 | Ships | class BulkMagazineComponent : HullComponent, IMagazineProvider, ISimpleStorageContainer, IStorageContainer, IConfigurableMagazineLoadout<br>class BulkMagazineData : ComponentSaveData, IMagazineLoadComponentData | Awake, OnDestroy | save-state |
| `Ships/BurstCellLauncherComponent.cs` | 67 | Ships | class BurstCellLauncherComponent : CellLauncherComponent |  |  |
| `Ships/BurstTubeLauncherComponent.cs` | 100 | Ships | class BurstTubeLauncherComponent : BaseTubeLauncherComponent |  |  |
| `Ships/CasemateController.cs` | 228 | Ships | class CasemateController : MonoBehaviour<br>struct CasemateControllerState | Awake |  |
| `Ships/CasualtyType.cs` | 10 | Ships | enum CasualtyType |  |  |
| `Ships/CellLauncherComponent.cs` | 41 | Ships | class CellLauncherComponent : BaseCellLauncherComponent |  |  |
| `Ships/ColliderSampler.cs` | 46 | Ships | class ColliderSampler : BaseColliderSampler | Awake, OnDestroy | prefab |
| `Ships/CommandComponent.cs` | 70 | Ships | class CommandComponent : CrewOperatedComponent, IIntelComponent |  |  |
| `Ships/CommsAntennaComponent.cs` | 56 | Ships | class CommsAntennaComponent : HullComponent, ICommsAntenna, ITuneableCommsAntenna |  |  |
| `Ships/CommsAntennaPart.cs` | 73 | Ships | class CommsAntennaPart : HullPart, ICommsAntenna, ITuneableCommsAntenna | Awake |  |
| `Ships/ComponentActivity.cs` | 17 | Ships | enum ComponentActivity |  |  |
| `Ships/ComponentCostClass.cs` | 19 | Ships | enum ComponentCostClass |  |  |
| `Ships/ComponentDebuff.cs` | 190 | Ships | class ComponentDebuff : ScriptableObject, IModSource<br>struct Target<br>enum PeriodicDamageType |  | prefab |
| `Ships/ComponentHullPaint.cs` | 22 | Ships | class ComponentHullPaint : MonoBehaviour |  |  |
| `Ships/ComponentHullPaintLODShared.cs` | 36 | Ships | class ComponentHullPaintLODShared : ComponentHullPaint |  |  |
| `Ships/ContinuousRaycastMuzzle.cs` | 75 | Ships | class ContinuousRaycastMuzzle : RaycastMuzzle, IDirectDamageMuzzle | FixedUpdate |  |
| `Ships/ContinuousWeaponComponent.cs` | 690 | Ships | class ContinuousWeaponComponent : WeaponComponent<br>enum CooldownType<br>interface IContinuousWeaponComponentRPC : IWeaponComponentRPC, ICycledComponentRPC, IHullComponentRPC, IHullPartRPC, IHullComponentRouter<br>class ContinuousWeaponState : SavedHullComponentStates.StateElement | Awake | save-state, XML |
| `Ships/Controls/CraftEntryExitPointStatus.cs` | 9 | Ships.Controls | enum CraftEntryExitPointStatus |  |  |
| `Ships/Controls/CraftHangarChanged.cs` | 4 | Ships.Controls |  |  |  |
| `Ships/Controls/CraftStorageChanged.cs` | 6 | Ships.Controls |  |  |  |
| `Ships/Controls/ICommsAntenna.cs` | 23 | Ships.Controls | interface ICommsAntenna |  |  |
| `Ships/Controls/ICraftEntryExitPoint.cs` | 78 | Ships.Controls | interface ICraftEntryExitPoint |  |  |
| `Ships/Controls/ICraftHangar.cs` | 66 | Ships.Controls | interface ICraftHangar : IInstanceStorageContainer, IStorageContainer |  |  |
| `Ships/Controls/ICraftWorkSlot.cs` | 19 | Ships.Controls | interface ICraftWorkSlot |  |  |
| `Ships/Controls/ICraftWorkSlotProvider.cs` | 7 | Ships.Controls | interface ICraftWorkSlotProvider |  |  |
| `Ships/Controls/ICycled.cs` | 7 | Ships.Controls | interface ICycled |  |  |
| `Ships/Controls/IEWarWeapon.cs` | 9 | Ships.Controls | interface IEWarWeapon : IWeapon, IHullComponent |  |  |
| `Ships/Controls/IHasIntegratedSensor.cs` | 7 | Ships.Controls | interface IHasIntegratedSensor |  |  |
| `Ships/Controls/IHasStandaloneCraftPad.cs` | 7 | Ships.Controls | interface IHasStandaloneCraftPad |  |  |
| `Ships/Controls/IIntelComponent.cs` | 15 | Ships.Controls | interface IIntelComponent |  |  |
| `Ships/Controls/IMagazineProvider.cs` | 52 | Ships.Controls | interface IMagazineProvider : ISimpleStorageContainer, IStorageContainer |  |  |
| `Ships/Controls/IMagazineSlotProvider.cs` | 22 | Ships.Controls | interface IMagazineSlotProvider : IMagazineProvider, ISimpleStorageContainer, IStorageContainer |  |  |
| `Ships/Controls/IMissileLauncher.cs` | 34 | Ships.Controls | interface IMissileLauncher : IWeapon, IHullComponent, INeedsShipIdentity |  |  |
| `Ships/Controls/INeedsSensorData.cs` | 9 | Ships.Controls | interface INeedsSensorData |  |  |
| `Ships/Controls/INeedsShipIdentity.cs` | 9 | Ships.Controls | interface INeedsShipIdentity |  |  |
| `Ships/Controls/ISensorComponent.cs` | 29 | Ships.Controls | interface ISensorComponent : IDeltaSensor, ISensor, IJammable, IEWarTarget, IOwned, IHullComponent |  |  |
| `Ships/Controls/ISyncCraftHangar.cs` | 10 | Ships.Controls | interface ISyncCraftHangar |  |  |
| `Ships/Controls/IThrustController.cs` | 24 | Ships.Controls | interface IThrustController |  |  |
| `Ships/Controls/IThruster.cs` | 24 | Ships.Controls | interface IThruster |  |  |
| `Ships/Controls/IWeapon.cs` | 150 | Ships.Controls | interface IWeapon : IHullComponent |  |  |
| `Ships/Controls/IWeaponGroup.cs` | 170 | Ships.Controls | interface IWeaponGroup : IOrderAssignable, ICycled |  |  |
| `Ships/Controls/MagazineChanged.cs` | 6 | Ships.Controls |  |  |  |
| `Ships/Controls/MagazineProviderChanged.cs` | 4 | Ships.Controls |  |  |  |
| `Ships/Controls/MagazineSlotChanged.cs` | 6 | Ships.Controls |  |  |  |
| `Ships/Controls/WeaponChanged.cs` | 4 | Ships.Controls |  |  |  |
| `Ships/CraftCarrierController.cs` | 1556 | Ships | class CraftCarrierController : NetworkBehaviour<br>struct LaunchOrder<br>enum DeckStatus<br>enum TrafficOrderType<br>class TrafficOrder<br>class LandingClearance<br>class FallbackPostflightSlot : ICraftWorkSlot<br>class HullRepairSlot : ICraftWorkSlot<br>class SavedLandingClearance<br>class SavedCarrierState : IXmlDocSerializable<br>class SavedTrafficOrder : IXmlDocSerializable |  | NetworkBehaviour, SyncVar, ClientRpc, NetworkServer, NetworkClient, save-state, XML |
| `Ships/CraftLandingPad.cs` | 609 | Ships | class CraftLandingPad : MonoBehaviour, ICraftEntryExitPoint, IBulkSaveComponent<br>class PadOperation : SaveableCoroutine<br>class LaunchFromPad : PadOperation<br>class RecoverFromPad : PadOperation | Awake, OnDisable | NetworkServer, save-state, XML |
| `Ships/CraftLandingPadPart.cs` | 58 | Ships | class CraftLandingPadPart : HullPartResourceConnected, IHasStandaloneCraftPad |  |  |
| `Ships/CraftWorkSlotComponent.cs` | 64 | Ships | class CraftWorkSlotComponent : CrewOperatedComponent, ICraftWorkSlotProvider<br>class WorkSlot : ICraftWorkSlot |  |  |
| `Ships/Crew.cs` | 261 | Ships | class Crew<br>struct CrewSummary | Update |  |
| `Ships/CrewedComponent.cs` | 72 | Ships | class CrewedComponent : HullComponent, ICrewed<br>interface ICrewedComponentRPC : IHullComponentRPC, IHullPartRPC, IHullComponentRouter | Start |  |
| `Ships/CrewOperatedComponent.cs` | 42 | Ships | class CrewOperatedComponent : CrewedComponent |  |  |
| `Ships/CycledComponent.cs` | 113 | Ships | class CycledComponent : CrewOperatedComponent<br>interface ICycledComponentRPC : IHullComponentRPC, IHullPartRPC, IHullComponentRouter<br>class CycledComponentState : SavedHullComponentStates.StateElement | Update | save-state, XML |
| `Ships/DamageControlDispatcher.cs` | 1088 | Ships | class DamageControlDispatcher : NetworkBehaviour<br>class RepairJobSummary : IRepairJob<br>class SavedDCState<br>struct TeamLocation | Awake, OnDestroy | NetworkBehaviour, SyncVar, ClientRpc, NetworkServer, NetworkClient, prefab |
| `Ships/DamageControlTeam.cs` | 233 | Ships | class DamageControlTeam : ICrewed<br>enum Status |  |  |
| `Ships/DamagedPartRecord.cs` | 49 | Ships | class DamagedPartRecord : IComparable<DamagedPartRecord> |  |  |
| `Ships/DamageFrame.cs` | 123 | Ships | class DamageFrame<br>class HullPartDamage<br>class StructureDamage |  |  |
| `Ships/DCLockerComponent.cs` | 276 | Ships | class DCLockerComponent : CrewOperatedComponent<br>interface IDCLockerComponentRPC : ICrewedComponentRPC, IHullComponentRPC, IHullPartRPC, IHullComponentRouter<br>class DCLockerState : SavedHullComponentStates.StateElement |  | save-state, XML |
| `Ships/DebuffTriggerEvent.cs` | 57 | Ships | class DebuffTriggerEvent : ScriptableObject |  |  |
| `Ships/DeceptionComponent.cs` | 138 | Ships | class DeceptionComponent : HullComponent, IIdentifiable<br>class DeceptionComponentData : ComponentSaveData<br>struct Identity |  |  |
| `Ships/DestroyedShipBoardingPoint.cs` | 210 | Ships | class DestroyedShipBoardingPoint : HullDockingPoint<br>struct GrapplePoint |  | prefab |
| `Ships/DiscreteWeaponComponent.cs` | 516 | Ships | class DiscreteWeaponComponent : WeaponComponent<br>class DiscreteWeaponState : SavedHullComponentStates.StateElement<br>struct TemporaryFiringModifier | OnDestroy, Start | save-state, XML, prefab |
| `Ships/DriveComponent.cs` | 6 | Ships | class DriveComponent : PowerplantComponent |  |  |
| `Ships/EditorResourceSummary.cs` | 21 | Ships | struct EditorResourceSummary |  |  |
| `Ships/ElevatorCraftLandingPad.cs` | 12 | Ships | class ElevatorCraftLandingPad : CraftLandingPad |  |  |
| `Ships/EliminationReason.cs` | 11 | Ships | enum EliminationReason |  |  |
| `Ships/EliminationReasonExtensions.cs` | 14 | Ships | class EliminationReasonExtensions |  |  |
| `Ships/EmergencyActionType.cs` | 8 | Ships | enum EmergencyActionType |  |  |
| `Ships/EmissionOutageEffect.cs` | 18 | Ships | class EmissionOutageEffect : EmissionOutageEffect_Base |  |  |
| `Ships/EmissionOutageEffect_Base.cs` | 71 | Ships | class EmissionOutageEffect_Base : OutageEffect |  | prefab |
| `Ships/EmissionOutageEffect_HullSegment.cs` | 43 | Ships | class EmissionOutageEffect_HullSegment : EmissionOutageEffect_Base |  |  |
| `Ships/EWarFollowingMuzzle.cs` | 17 | Ships | class EWarFollowingMuzzle : FollowingInstanceMuzzle |  | prefab |
| `Ships/ExternallyFedCellMissileMagazine.cs` | 296 | Ships | class ExternallyFedCellMissileMagazine : ICellMissileMagazine, IMagazineProvider, ISimpleStorageContainer, IStorageContainer |  |  |
| `Ships/FireControlSensor.cs` | 365 | Ships | class FireControlSensor : MonoBehaviour, ISensorComponent, IDeltaSensor, ISensor, IJammable, IEWarTarget, IOwned, IHullComponent, IFastDeltaSensor | Awake, OnDestroy, OnDisable, OnEnable |  |
| `Ships/FixedActiveSensorComponent.cs` | 92 | Ships | class FixedActiveSensorComponent : OrderableActiveSensorComponent, IFixedWeapon |  |  |
| `Ships/FixedContinuousWeaponComponent.cs` | 58 | Ships | class FixedContinuousWeaponComponent : ContinuousWeaponComponent, IFixedWeapon |  |  |
| `Ships/FixedDiscreteWeaponComponent.cs` | 183 | Ships | class FixedDiscreteWeaponComponent : DiscreteWeaponComponent, IFixedWeapon |  | save-state |
| `Ships/Fleet.cs` | 930 | Ships | class Fleet<br>class FleetSummary : ISteamWorkshopItem, IFileSummary, IComparable<FleetSummary><br>struct Silhouette<br>class FleetLobbySummary<br>class FleetDetailSummary<br>class StagedFleetData<br>class FormationSetup<br>class Formation<br>class Escorting |  | prefab |
| `Ships/FollowingInstanceMuzzle.cs` | 86 | Ships | class FollowingInstanceMuzzle : Muzzle | FixedUpdate | pooling |
| `Ships/FormationWeaponGroup.cs` | 335 | Ships | class FormationWeaponGroup : IWeaponGroup, IOrderAssignable, ICycled |  |  |
| `Ships/FreeModifierSource.cs` | 17 | Ships | class FreeModifierSource : IModifierSource |  |  |
| `Ships/GrappleLauncherEffect.cs` | 158 | Ships | class GrappleLauncherEffect : MonoBehaviour | OnEnable | prefab |
| `Ships/Hull.cs` | 92 | Ships | class Hull : BaseHull |  | prefab |
| `Ships/HullComponent.cs` | 1469 | Ships | class HullComponent : HullPartResourceConnected, IBundleKeyed, ISaveKeyed, IModSource, IHullComponent, IModifierSource, IStatusComponent, IFactionLocked<br>interface IHullComponentRPC : IHullPartRPC, IHullComponentRouter<br>enum RotateAxis<br>enum TestAxis<br>class DebuffInstance : IModifierSource, ICountdownEvent, IComparable<DebuffInstance>, IRepairJob | Awake, OnDestroy, Start, Update | Command, save-state |
| `Ships/HullComponentCostBreakdownCalculator.cs` | 59 | Ships | class HullComponentCostBreakdownCalculator |  |  |
| `Ships/HullComponentCostCalculator.cs` | 91 | Ships | class HullComponentCostCalculator |  |  |
| `Ships/HullDockingPoint.cs` | 490 | Ships | class HullDockingPoint : HullPart, IDockingPoint<br>interface IHullDockingPointRPC : IHullPartRPC, IHullComponentRouter | Awake, FixedUpdate, OnDisable | NetworkServer, prefab |
| `Ships/HullPart.cs` | 655 | Ships | class HullPart : MonoBehaviour, ISubDamageable, IRepairJob<br>interface IHullPartRPC : IHullComponentRouter | Awake, OnDestroy, Start | prefab |
| `Ships/HullPartResourceConnected.cs` | 219 | Ships | class HullPartResourceConnected : HullPart, IResourceSystemConnected | Awake |  |
| `Ships/HullSegment.cs` | 53 | Ships | class HullSegment : HullSegmentBasic |  |  |
| `Ships/HullSegmentBasic.cs` | 291 | Ships | class HullSegmentBasic : MonoBehaviour | OnDestroy | prefab |
| `Ships/HullSegmentFastNameplate.cs` | 98 | Ships | class HullSegmentFastNameplate : HullSegmentMultiMaterial |  |  |
| `Ships/HullSegmentMultiMaterial.cs` | 97 | Ships | class HullSegmentMultiMaterial : HullSegmentBasic<br>class AddressableMaterialTextures<br>struct LazyLoadedTexture |  | Addressables |
| `Ships/HullSocket.cs` | 362 | Ships | class HullSocket : MonoBehaviour, IHullSocket | Awake, OnDestroy | prefab |
| `Ships/HullSocketType.cs` | 9 | Ships | enum HullSocketType |  |  |
| `Ships/HullSocketTypeExtensions.cs` | 42 | Ships | class HullSocketTypeExtensions |  |  |
| `Ships/HullStructure.cs` | 169 | Ships | class HullStructure : MonoBehaviour, ISubDamageable | Awake, OnDestroy | prefab |
| `Ships/HullSubsystemTargetType.cs` | 11 | Ships | enum HullSubsystemTargetType |  |  |
| `Ships/HullVolume.cs` | 86 | Ships | class HullVolume : MonoBehaviour, IRandomPointVolume<br>struct WeightedBounds | Awake |  |
| `Ships/ICellMissileMagazine.cs` | 54 | Ships | interface ICellMissileMagazine : IMagazineProvider, ISimpleStorageContainer, IStorageContainer |  |  |
| `Ships/ICrewed.cs` | 25 | Ships | interface ICrewed |  |  |
| `Ships/IDirectDamageMuzzle.cs` | 21 | Ships | interface IDirectDamageMuzzle |  |  |
| `Ships/IFacingDriver.cs` | 24 | Ships | interface IFacingDriver |  |  |
| `Ships/IFireRateMuzzle.cs` | 9 | Ships | interface IFireRateMuzzle |  |  |
| `Ships/IFixedWeapon.cs` | 13 | Ships | interface IFixedWeapon |  |  |
| `Ships/IHullComponent.cs` | 16 | Ships | interface IHullComponent |  |  |
| `Ships/IHullSocket.cs` | 60 | Ships | interface IHullSocket |  |  |
| `Ships/ImmediateLaunchMissileMuzzle.cs` | 64 | Ships | class ImmediateLaunchMissileMuzzle : AccuracyMuzzle |  |  |
| `Ships/IModifierSource.cs` | 7 | Ships | interface IModifierSource |  |  |
| `Ships/IModularHullPiece.cs` | 23 | Ships | interface IModularHullPiece |  | prefab |
| `Ships/IMuzzleWeapon.cs` | 15 | Ships | interface IMuzzleWeapon |  |  |
| `Ships/IntelligenceComponent.cs` | 70 | Ships | class IntelligenceComponent : CrewOperatedComponent, IIntelComponent |  |  |
| `Ships/InternalActiveSensorComponent.cs` | 95 | Ships | class InternalActiveSensorComponent : BaseActiveSensorComponent | OnDestroy, Start |  |
| `Ships/InternalExplosionTrigger.cs` | 109 | Ships | class InternalExplosionTrigger : SpawnTriggerEvent, IDamageCharacteristic |  | prefab |
| `Ships/IPDControllerStatus.cs` | 30 | Ships | interface IPDControllerStatus |  |  |
| `Ships/IPlatform.cs` | 68 | Ships | interface IPlatform |  |  |
| `Ships/IPlatformController.cs` | 66 | Ships | interface IPlatformController : INetIdentifiedScript, IStorageContainerProvider, IDockingProvider, IOwned, IBoardPiece |  | prefab |
| `Ships/IRangedMuzzle.cs` | 7 | Ships | interface IRangedMuzzle |  |  |
| `Ships/IReadOnlyResourcePool.cs` | 25 | Ships | interface IReadOnlyResourcePool |  |  |
| `Ships/IReadOnlyStat.cs` | 25 | Ships | interface IReadOnlyStat |  |  |
| `Ships/IRepairJob.cs` | 21 | Ships | interface IRepairJob |  |  |
| `Ships/IResizableComponent.cs` | 18 | Ships | interface IResizableComponent |  |  |
| `Ships/IResourceSystemConnected.cs` | 23 | Ships | interface IResourceSystemConnected |  |  |
| `Ships/ISensorPanel.cs` | 19 | Ships | interface ISensorPanel |  |  |
| `Ships/IShoulderCamMount.cs` | 13 | Ships | interface IShoulderCamMount |  |  |
| `Ships/IStatProvider.cs` | 15 | Ships | interface IStatProvider |  |  |
| `Ships/IStatusComponent.cs` | 27 | Ships | interface IStatusComponent |  |  |
| `Ships/ISubsystemLocations.cs` | 10 | Ships | interface ISubsystemLocations |  |  |
| `Ships/IUnmaskingDriver.cs` | 32 | Ships | interface IUnmaskingDriver |  |  |
| `Ships/LauncherProgrammingQueue.cs` | 264 | Ships | class LauncherProgrammingQueue<br>interface ILauncherProgCallbacks<br>class PendingLaunch<br>class PendingPositionLaunch : PendingLaunch<br>class PendingTrackLaunch : PendingLaunch |  |  |
| `Ships/LifeboatController.cs` | 429 | Ships | class LifeboatController : LaunchedLookaheadMunition | Awake, FixedUpdate, OnUnpooled | NetworkBehaviour, ClientRpc, NetworkServer, NetworkClient, XML, pooling, prefab |
| `Ships/LifeboatEjector.cs` | 88 | Ships | class LifeboatEjector : MonoBehaviour | Awake, OnDestroy | prefab |
| `Ships/LightsOutageEffect.cs` | 17 | Ships | class LightsOutageEffect : LightsOutageEffect_Base |  |  |
| `Ships/LightsOutageEffect_Base.cs` | 120 | Ships | class LightsOutageEffect_Base : OutageEffect<br>enum Flicker |  | prefab |
| `Ships/LightsOutageEffect_HullSegment.cs` | 42 | Ships | class LightsOutageEffect_HullSegment : LightsOutageEffect_Base |  |  |
| `Ships/LineBeamMuzzleEffects.cs` | 62 | Ships | class LineBeamMuzzleEffects : BeamMuzzleEffects |  |  |
| `Ships/LiteralStatValue.cs` | 48 | Ships | class LiteralStatValue : IReadOnlyStat |  |  |
| `Ships/MagazineMonitor.cs` | 157 | Ships | class MagazineMonitor : IQuantityMonitor<br>class Record |  |  |
| `Ships/MassiveInternalExplosionTrigger.cs` | 43 | Ships | class MassiveInternalExplosionTrigger : InternalExplosionTrigger |  |  |
| `Ships/MissileEjector.cs` | 223 | Ships | class MissileEjector : MonoBehaviour<br>struct FiringEffect | OnDestroy | prefab |
| `Ships/MissileEjectorGroup.cs` | 67 | Ships | class MissileEjectorGroup : MonoBehaviour, ILODGroupBuilderInput |  |  |
| `Ships/MixedSalvoWeaponGroup.cs` | 459 | Ships | class MixedSalvoWeaponGroup : IWeaponGroup, IOrderAssignable, ICycled |  |  |
| `Ships/ModularColliderSampler.cs` | 28 | Ships | class ModularColliderSampler : BaseColliderSampler |  | prefab |
| `Ships/ModularHullDressing.cs` | 40 | Ships | class ModularHullDressing : MonoBehaviour |  | prefab |
| `Ships/ModularHullPrimaryStructure.cs` | 304 | Ships | class ModularHullPrimaryStructure : MonoBehaviour, IModularHullPiece<br>struct SegmentConfiguration<br>struct ModularDockingPointAirlock<br>struct ModularDockingPointClamp |  | prefab |
| `Ships/ModularHullSecondaryStructure.cs` | 68 | Ships | class ModularHullSecondaryStructure : MonoBehaviour, IModularHullPiece |  | prefab |
| `Ships/MultiCraftHangarComponent.cs` | 111 | Ships | class MultiCraftHangarComponent : SlotCraftHangarComponent, ICraftWorkSlotProvider<br>class WorkSlot : ICraftWorkSlot |  |  |
| `Ships/MultiInternalExplosionTrigger.cs` | 166 | Ships | class MultiInternalExplosionTrigger : DebuffTriggerEvent, IDamageCharacteristic<br>struct SavedHit |  | prefab |
| `Ships/MultiPadBulkCraftHangarComponent.cs` | 54 | Ships | class MultiPadBulkCraftHangarComponent : BulkCraftHangarComponent |  |  |
| `Ships/MultiSourceStatValue.cs` | 118 | Ships | class MultiSourceStatValue : IReadOnlyStat<br>enum SelectCriteria<br>struct StatSource |  |  |
| `Ships/Muzzle.cs` | 55 | Ships | class Muzzle : MonoBehaviour |  |  |
| `Ships/OmnidirectionalEWarComponent.cs` | 100 | Ships | class OmnidirectionalEWarComponent : ContinuousWeaponComponent, IEWarWeapon, IWeapon, IHullComponent, ITuneableEWar | Awake |  |
| `Ships/OrderableActiveSensorComponent.cs` | 475 | Ships | class OrderableActiveSensorComponent : BaseActiveSensorComponent, IWeapon, IHullComponent<br>interface IOrderableActiveSensorComponentRPC : ISensorComponentRPC, IHullComponentRPC, IHullPartRPC, IHullComponentRouter | FixedUpdate, OnDestroy | prefab |
| `Ships/OutageEffect.cs` | 11 | Ships | class OutageEffect : MonoBehaviour |  |  |
| `Ships/PassiveFireControlSensor.cs` | 64 | Ships | class PassiveFireControlSensor : FireControlSensor |  |  |
| `Ships/PassiveSensorComponent.cs` | 316 | Ships | class PassiveSensorComponent : HullComponent, ISensorComponent, IDeltaSensor, ISensor, IJammable, IEWarTarget, IOwned, IHullComponent | Awake, OnDestroy |  |
| `Ships/PDMissileBids.cs` | 49 | Ships | class PDMissileBids<br>struct Bid |  |  |
| `Ships/PointDefenseController.cs` | 1895 | Ships | class PointDefenseController : IPDControllerStatus<br>class SavedPDState<br>class PDTarget : IClusterablePoint, IComparable<PDTarget>, IDisposable<br>struct ThreatCluster<br>class PDTurret<br>class PDMissile : IWeaponStatReportReceiver<br>class PDDecoy : IWeaponStatReportReceiver<br>struct PendingDecoyLaunch : IComparable<PendingDecoyLaunch><br>struct PendingMissileLaunch<br>class MissileInFlight<br>class MissileDoctrineComparer : IComparer<PDTarget> | Update |  |
| `Ships/PowerplantComponent.cs` | 13 | Ships | class PowerplantComponent : CrewOperatedComponent |  |  |
| `Ships/PreppedCell.cs` | 13 | Ships | struct PreppedCell |  |  |
| `Ships/Priority.cs` | 10 | Ships | enum Priority |  |  |
| `Ships/PulsedRaycastMuzzle.cs` | 81 | Ships | class PulsedRaycastMuzzle : RaycastMuzzle, IDirectDamageMuzzle | FixedUpdate |  |
| `Ships/QuadModeTurretController.cs` | 101 | Ships | class QuadModeTurretController : TurretController<br>enum Mode<br>struct FixedModeAngles |  |  |
| `Ships/RackMissileEjector.cs` | 84 | Ships | class RackMissileEjector : MissileEjector | Awake | prefab |
| `Ships/RandomizeHullSegmentTextureOffset.cs` | 22 | Ships | class RandomizeHullSegmentTextureOffset : MonoBehaviour | Start |  |
| `Ships/RandomModularHull.cs` | 524 | Ships | class RandomModularHull : BaseHull<br>class RandomHullConfiguration : HullConfiguration<br>struct SecondaryStructureConfig | OnDestroy | prefab |
| `Ships/RaycastMuzzle.cs` | 243 | Ships | class RaycastMuzzle : AccuracyMuzzle, IDamageDealer, IDamageCharacteristic, IRangedMuzzle<br>enum RaycastType | Awake, Start | prefab |
| `Ships/RepairJobCategory.cs` | 10 | Ships | enum RepairJobCategory |  |  |
| `Ships/RepairJobSummarySerializer.cs` | 41 | Ships | class RepairJobSummarySerializer |  |  |
| `Ships/RepeatingBallisticRaycastMuzzle.cs` | 155 | Ships | class RepeatingBallisticRaycastMuzzle : BallisticRaycastMuzzle, IFireRateMuzzle | FixedUpdate, Start |  |
| `Ships/ReplenishmentDockingPoint.cs` | 117 | Ships | class ReplenishmentDockingPoint : HullDockingPoint |  |  |
| `Ships/ResizableCellLauncherComponent.cs` | 250 | Ships | class ResizableCellLauncherComponent : BaseCellLauncherComponent<br>class ResizableCellLauncherData : CellLauncherData |  | prefab |
| `Ships/ResizingPadBulkCraftHangarComponent.cs` | 24 | Ships | class ResizingPadBulkCraftHangarComponent : MultiPadBulkCraftHangarComponent |  |  |
| `Ships/ResourceModifier.cs` | 31 | Ships | struct ResourceModifier |  |  |
| `Ships/ResourcePool.cs` | 223 | Ships | class ResourcePool : IReadOnlyResourcePool |  |  |
| `Ships/ResourceType.cs` | 17 | Ships | class ResourceType<br>enum Schedule |  |  |
| `Ships/ResourceValue.cs` | 25 | Ships | class ResourceValue |  |  |
| `Ships/RestorePartJob.cs` | 58 | Ships | class RestorePartJob : IRepairJob |  |  |
| `Ships/RezFollowingMuzzle.cs` | 16 | Ships | class RezFollowingMuzzle : FollowingInstanceMuzzle |  | prefab |
| `Ships/RezzingMuzzle.cs` | 76 | Ships | class RezzingMuzzle : AccuracyMuzzle |  |  |
| `Ships/SalvageShipJob.cs` | 143 | Ships | class SalvageShipJob |  | NetworkServer |
| `Ships/SalvageShipJobSerializer.cs` | 22 | Ships | class SalvageShipJobSerializer |  |  |
| `Ships/SaveGame/SavedComponentCasemateState.cs` | 20 | Ships.SaveGame | class SavedComponentCasemateState : SavedHullComponentStates.StateElement |  | save-state, XML |
| `Ships/SaveGame/SavedComponentDebuffState.cs` | 49 | Ships.SaveGame | class SavedComponentDebuffState : SavedHullComponentStates.StateElement<br>class SavedDebuff |  | save-state, XML |
| `Ships/SaveGame/SavedComponentMagazineState.cs` | 26 | Ships.SaveGame | class SavedComponentMagazineState : SavedHullComponentStates.StateElement |  | save-state, XML |
| `Ships/SaveGame/SavedComponentTurretState.cs` | 20 | Ships.SaveGame | class SavedComponentTurretState : SavedHullComponentStates.StateElement |  | save-state, XML |
| `Ships/SaveGame/SavedCrewState.cs` | 24 | Ships.SaveGame | class SavedCrewState<br>struct Assignment |  |  |
| `Ships/SaveGame/SavedHullComponentStates.cs` | 86 | Ships.SaveGame | class SavedHullComponentStates : IXmlDocSerializable<br>class StateElement : IXmlDocSerializable |  | save-state, XML |
| `Ships/SaveGame/SavedShipDamage.cs` | 23 | Ships.SaveGame | class SavedShipDamage<br>class ArmorSection |  |  |
| `Ships/SaveGame/SavedShipGameState.cs` | 101 | Ships.SaveGame | class SavedShipGameState : IXmlDocSerializable |  | save-state, XML |
| `Ships/SaveGame/SavedShipPosture.cs` | 23 | Ships.SaveGame | class SavedShipPosture |  |  |
| `Ships/SaveGame/SavedShipResources.cs` | 10 | Ships.SaveGame | class SavedShipResources |  |  |
| `Ships/SaveGame/SavedShipState.cs` | 62 | Ships.SaveGame | class SavedShipState : IXmlDocSerializable |  | save-state, XML |
| `Ships/SaveGame/SerializedPartDamage.cs` | 18 | Ships.SaveGame | class SerializedPartDamage |  |  |
| `Ships/ScissorBones.cs` | 139 | Ships | class ScissorBones : MonoBehaviour |  |  |
| `Ships/SectorSensorPart.cs` | 42 | Ships | class SectorSensorPart : HullPart, ISensorPanel |  |  |
| `Ships/SensorTurretComponent.cs` | 415 | Ships | class SensorTurretComponent : HullComponent, ISensorComponent, IDeltaSensor, ISensor, IJammable, IEWarTarget, IOwned, IHullComponent, INeedsSensorData, IUnmaskingDrive...<br>interface ISensorTurretComponentRPC : IHullComponentRPC, IHullPartRPC, IHullComponentRouter | Awake, FixedUpdate, OnDestroy, OnDisable, OnEnable | save-state |
| `Ships/Serialization/ComponentSaveData.cs` | 16 | Ships.Serialization | class ComponentSaveData |  |  |
| `Ships/Serialization/IInitialFormationSaveData.cs` | 13 | Ships.Serialization | interface IInitialFormationSaveData : IFormationUnit |  |  |
| `Ships/Serialization/SerializedFleet.cs` | 103 | Ships.Serialization | class SerializedFleet : IXmlDocSerializable |  | XML |
| `Ships/Serialization/SerializedFormation.cs` | 25 | Ships.Serialization | class SerializedFormation |  |  |
| `Ships/Serialization/SerializedHullSocket.cs` | 16 | Ships.Serialization | class SerializedHullSocket |  |  |
| `Ships/Serialization/SerializedShip.cs` | 112 | Ships.Serialization | class SerializedShip : IXmlDocSerializable |  | XML |
| `Ships/Serialization/SerializedWeaponGroup.cs` | 16 | Ships.Serialization | class SerializedWeaponGroup |  |  |
| `Ships/Ship.cs` | 1976 | Ships | class Ship : NetworkBehaviour, IStatProvider, ISubsystemLocations<br>class ShipSummary : ISteamWorkshopItem, IFileSummary, IComparable<ShipSummary><br>class ShipLobbySummary<br>struct MountGroup<br>class ShipDetailSummary<br>struct MountGroup | OnDestroy | NetworkBehaviour, ClientRpc, NetworkServer, NetworkClient, save-state, prefab |
| `Ships/ShipGroupTriggerArrivalCoroutine.cs` | 167 | Ships | class ShipGroupTriggerArrivalCoroutine : SaveableCoroutine |  | NetworkServer, save-state, XML |
| `Ships/ShipStatAttribute.cs` | 49 | Ships | class ShipStatAttribute : Attribute |  |  |
| `Ships/ShipStatusSummary.cs` | 13 | Ships | struct ShipStatusSummary |  |  |
| `Ships/SimpleCommsAntenna.cs` | 47 | Ships | class SimpleCommsAntenna : MonoBehaviour, ICommsAntenna, ITuneableCommsAntenna |  |  |
| `Ships/SingleCraftHangarComponent.cs` | 65 | Ships | class SingleCraftHangarComponent : SlotCraftHangarComponent |  |  |
| `Ships/SinglePulseRaycastMuzzle.cs` | 65 | Ships | class SinglePulseRaycastMuzzle : RaycastMuzzle, IDirectDamageMuzzle |  | prefab |
| `Ships/SlidingRailMissileEjector.cs` | 128 | Ships | class SlidingRailMissileEjector : MissileEjector<br>class MissileMesh |  | prefab |
| `Ships/SlotCellMissileMagazine.cs` | 301 | Ships | class SlotCellMissileMagazine : BaseCellMissileMagazine, IMagazineSlotProvider, IMagazineProvider, ISimpleStorageContainer, IStorageContainer<br>class ModuleMagazine |  |  |
| `Ships/SlotCraftHangarComponent.cs` | 204 | Ships | class SlotCraftHangarComponent : BaseCraftHangarComponent, ISyncCraftHangar |  |  |
| `Ships/SocketBezelSizer.cs` | 44 | Ships | class SocketBezelSizer : MonoBehaviour |  |  |
| `Ships/SocketSize.cs` | 18 | Ships | struct SocketSize |  |  |
| `Ships/SoundOutageEffect.cs` | 21 | Ships | class SoundOutageEffect : OutageEffect |  |  |
| `Ships/SpawnTriggerEvent.cs` | 58 | Ships | class SpawnTriggerEvent : DebuffTriggerEvent<br>enum RotationMode |  | pooling, prefab |
| `Ships/SpecialRepairJob.cs` | 67 | Ships | class SpecialRepairJob : IRepairJob |  |  |
| `Ships/StandardCellMissileMagazine.cs` | 233 | Ships | class StandardCellMissileMagazine : BaseCellMissileMagazine |  |  |
| `Ships/StatChanged.cs` | 4 | Ships |  |  |  |
| `Ships/StatIdentifier.cs` | 59 | Ships | struct StatIdentifier |  |  |
| `Ships/StatModifier.cs` | 151 | Ships | struct StatModifier |  |  |
| `Ships/StatTable.cs` | 290 | Ships | class StatTable<br>class StatRecord |  |  |
| `Ships/StatValue.cs` | 243 | Ships | class StatValue : IReadOnlyStat |  |  |
| `Ships/ThrusterPart.cs` | 451 | Ships | class ThrusterPart : HullPart, IThruster<br>enum Throttle | Awake, FixedUpdate, OnDestroy, OnDisable, Update |  |
| `Ships/TubeLauncherComponent.cs` | 53 | Ships | class TubeLauncherComponent : BaseTubeLauncherComponent |  |  |
| `Ships/TurretController.cs` | 334 | Ships | class TurretController : MonoBehaviour<br>struct TurretControllerState | Awake |  |
| `Ships/TurretedActiveSensorComponent.cs` | 106 | Ships | class TurretedActiveSensorComponent : OrderableActiveSensorComponent |  | save-state |
| `Ships/TurretedContinuousWeaponComponent.cs` | 139 | Ships | class TurretedContinuousWeaponComponent : ContinuousWeaponComponent | Awake | save-state |
| `Ships/TurretedDiscreteWeaponComponent.cs` | 133 | Ships | class TurretedDiscreteWeaponComponent : DiscreteWeaponComponent | Awake | save-state |
| `Ships/TurretedEWarComponent.cs` | 84 | Ships | class TurretedEWarComponent : TurretedContinuousWeaponComponent, IEWarWeapon, IWeapon, IHullComponent, ITuneableEWar | Awake |  |
| `Ships/TurretedLauncherComponent.cs` | 22 | Ships | class TurretedLauncherComponent : BaseTurretedLauncherComponent |  |  |
| `Ships/VisualSensor.cs` | 272 | Ships | class VisualSensor : MonoBehaviour, ISensorComponent, IDeltaSensor, ISensor, IJammable, IEWarTarget, IOwned, IHullComponent |  |  |
| `Ships/WeaponComponent.cs` | 1089 | Ships | class WeaponComponent : CycledComponent, IWeapon, IHullComponent, INeedsSensorData, INeedsShipIdentity, IHasIntegratedSensor, IMuzzleWeapon, IShoulderCamMount, I...<br>interface IWeaponComponentRPC : ICycledComponentRPC, IHullComponentRPC, IHullPartRPC, IHullComponentRouter<br>class GenericWeaponState : SavedHullComponentStates.StateElement | FixedUpdate, OnDestroy | save-state, XML |
| `Ships/WeaponGroup.cs` | 688 | Ships | class WeaponGroup : IWeaponGroup, IOrderAssignable, ICycled, IUnmaskingDriver, IFacingDriver |  |  |
| `SmallCraft/AnimatedMissileBay.cs` | 69 | SmallCraft | class AnimatedMissileBay : CraftAnimatedPart |  |  |
| `SmallCraft/AvailableCraftSet.cs` | 355 | SmallCraft | class AvailableCraftSet |  | prefab |
| `SmallCraft/BaseCraftMovement.cs` | 1042 | SmallCraft | class BaseCraftMovement : NetworkBehaviour, IModifiableStats, IBulkSaveComponent<br>enum MovementMode<br>interface ICraftThruster | Awake, OnStartServer, Update | NetworkBehaviour, SyncVar, NetworkServer, save-state, XML |
| `SmallCraft/Components/BaseCraftMissileSlot.cs` | 32 | SmallCraft.Components | class BaseCraftMissileSlot : MonoBehaviour |  |  |
| `SmallCraft/Components/CraftAnimatedPart.cs` | 52 | SmallCraft.Components | class CraftAnimatedPart : MonoBehaviour, IBulkSaveComponent |  | save-state, XML |
| `SmallCraft/Components/CraftComponent.cs` | 254 | SmallCraft.Components | class CraftComponent : MonoBehaviour, ILODGroupBuilderInput, IModifierSource | OnAdded, OnCloned |  |
| `SmallCraft/Components/CraftDecoyLauncherComponent.cs` | 69 | SmallCraft.Components | class CraftDecoyLauncherComponent : CraftComponent |  |  |
| `SmallCraft/Components/CraftEWarPodComponent.cs` | 145 | SmallCraft.Components | class CraftEWarPodComponent : CraftLoadoutSlotComponent, ITargetPaintingComponent<br>class SteeringArea |  | prefab |
| `SmallCraft/Components/CraftFuelPodComponent.cs` | 30 | SmallCraft.Components | class CraftFuelPodComponent : CraftLoadoutSlotComponent |  |  |
| `SmallCraft/Components/CraftGunTurretWeapon.cs` | 45 | SmallCraft.Components | class CraftGunTurretWeapon : CraftGunWeapon | Awake |  |
| `SmallCraft/Components/CraftGunWeapon.cs` | 366 | SmallCraft.Components | class CraftGunWeapon : CraftComponent, IModifiableStats<br>enum FiringMode |  |  |
| `SmallCraft/Components/CraftLoadoutSlotComponent.cs` | 38 | SmallCraft.Components | class CraftLoadoutSlotComponent : CraftComponent |  |  |
| `SmallCraft/Components/CraftMasqueradeComponent.cs` | 64 | SmallCraft.Components | class CraftMasqueradeComponent : CraftComponent |  |  |
| `SmallCraft/Components/CraftMissileBayWeapon.cs` | 82 | SmallCraft.Components | class CraftMissileBayWeapon : CraftMissileComponent<br>struct BayMapping |  |  |
| `SmallCraft/Components/CraftMissileComponent.cs` | 342 | SmallCraft.Components | class CraftMissileComponent : CraftComponent |  |  |
| `SmallCraft/Components/CraftMissilePylonWeapon.cs` | 14 | SmallCraft.Components | class CraftMissilePylonWeapon : CraftMissileComponent |  |  |
| `SmallCraft/Components/CraftSensorComponent.cs` | 134 | SmallCraft.Components | class CraftSensorComponent : CraftLoadoutSlotComponent<br>enum SensorType |  |  |
| `SmallCraft/Components/CraftSupportLauncherComponent.cs` | 96 | SmallCraft.Components | class CraftSupportLauncherComponent : CraftComponent, ICraftMissileSupportComponent |  | prefab |
| `SmallCraft/Components/CraftTargetingPodComponent.cs` | 67 | SmallCraft.Components | class CraftTargetingPodComponent : CraftSensorComponent, ITargetPaintingComponent |  |  |
| `SmallCraft/Components/CraftTrackingSensorComponent.cs` | 67 | SmallCraft.Components | class CraftTrackingSensorComponent : CraftSensorComponent, ITuneableSensor<br>enum SensorShape |  |  |
| `SmallCraft/Components/ICraftMissileSupportComponent.cs` | 14 | SmallCraft.Components | interface ICraftMissileSupportComponent |  |  |
| `SmallCraft/Components/ITargetPaintingComponent.cs` | 15 | SmallCraft.Components | interface ITargetPaintingComponent |  |  |
| `SmallCraft/Components/PylonMissileEjector.cs` | 134 | SmallCraft.Components | class PylonMissileEjector : BaseCraftMissileSlot, ILODGroupBuilderInput<br>class MissileMesh |  | pooling, prefab |
| `SmallCraft/Components/Runtime/CraftDecoyLauncher.cs` | 248 | SmallCraft.Components.Runtime | class CraftDecoyLauncher : RuntimeCraftBehaviour | OnAdded | NetworkBehaviour, SyncVar, ClientRpc, NetworkServer, NetworkClient, prefab |
| `SmallCraft/Components/Runtime/CraftSupportLauncher.cs` | 150 | SmallCraft.Components.Runtime | class CraftSupportLauncher : RuntimeCraftBehaviour | OnAdded | NetworkBehaviour, SyncVar, prefab |
| `SmallCraft/Components/Runtime/RuntimeCraftAmmoWeapon.cs` | 814 | SmallCraft.Components.Runtime | class RuntimeCraftAmmoWeapon : RuntimeCraftBehaviour, ICraftWeapon, IMagazine, IQuantityHolder, IMuzzleWeapon, IThreat, IWeaponStatReportReceiver, IBulkSaveComponent | Awake, FixedUpdate, OnAdded, OnCloned | NetworkBehaviour, SyncVar, ClientRpc, NetworkServer, NetworkClient, save-state, XML |
| `SmallCraft/Components/Runtime/RuntimeCraftBehaviour.cs` | 48 | SmallCraft.Components.Runtime | class RuntimeCraftBehaviour : NetworkBehaviour | OnAdded, OnCloned, OnDestroy | NetworkBehaviour |
| `SmallCraft/Components/Runtime/RuntimeCraftBurstWeapon.cs` | 42 | SmallCraft.Components.Runtime | class RuntimeCraftBurstWeapon : RuntimeCraftContinuousWeapon |  |  |
| `SmallCraft/Components/Runtime/RuntimeCraftContinuousWeapon.cs` | 70 | SmallCraft.Components.Runtime | class RuntimeCraftContinuousWeapon : RuntimeCraftAmmoWeapon |  |  |
| `SmallCraft/Components/Runtime/RuntimeCraftDiscreteWeapon.cs` | 110 | SmallCraft.Components.Runtime | class RuntimeCraftDiscreteWeapon : RuntimeCraftAmmoWeapon | FixedUpdate | XML |
| `SmallCraft/Components/Runtime/RuntimeCraftEWarPod.cs` | 155 | SmallCraft.Components.Runtime | class RuntimeCraftEWarPod : RuntimeCraftBehaviour, ICraftWeapon | FixedUpdate, OnAdded | pooling |
| `SmallCraft/Components/SerializedCraftSocket.cs` | 12 | SmallCraft.Components | class SerializedCraftSocket |  |  |
| `SmallCraft/Components/SpacecraftSocket.cs` | 234 | SmallCraft.Components | class SpacecraftSocket : MonoBehaviour, IXmlSaveState<string> | OnCloned | save-state, XML |
| `SmallCraft/Components/StaticSpacecraftSocket.cs` | 148 | SmallCraft.Components | class StaticSpacecraftSocket : SpacecraftSocket |  | prefab |
| `SmallCraft/Components/VariableSpacecraftSocket.cs` | 379 | SmallCraft.Components | class VariableSpacecraftSocket : SpacecraftSocket<br>class VariableSocketOption | OnCloned | save-state, XML, prefab |
| `SmallCraft/CraftActiveSensor.cs` | 237 | SmallCraft | class CraftActiveSensor : CraftBaseSensor, ISignalEmitter, ITuneableSensor | Awake, OnAdded |  |
| `SmallCraft/CraftActivityType.cs` | 13 | SmallCraft | enum CraftActivityType |  |  |
| `SmallCraft/CraftAttackCoherence.cs` | 9 | SmallCraft | enum CraftAttackCoherence |  |  |
| `SmallCraft/CraftAttackRange.cs` | 9 | SmallCraft | enum CraftAttackRange |  |  |
| `SmallCraft/CraftAttackStyle.cs` | 8 | SmallCraft | enum CraftAttackStyle |  |  |
| `SmallCraft/CraftAttackSurvival.cs` | 8 | SmallCraft | enum CraftAttackSurvival |  |  |
| `SmallCraft/CraftBaseSensor.cs` | 226 | SmallCraft | class CraftBaseSensor : RuntimeCraftBehaviour, IDeltaSensor, ISensor, IJammable, IEWarTarget, IOwned | Awake, OnDisable, OnEnable | NetworkBehaviour, ClientRpc, NetworkClient, prefab |
| `SmallCraft/CraftCallsignGroup.cs` | 10 | SmallCraft | enum CraftCallsignGroup |  |  |
| `SmallCraft/CraftConeActiveSensor.cs` | 107 | SmallCraft | class CraftConeActiveSensor : CraftActiveSensor |  |  |
| `SmallCraft/CraftCountermeasureUse.cs` | 9 | SmallCraft | enum CraftCountermeasureUse |  |  |
| `SmallCraft/CraftDefensiveTactic.cs` | 10 | SmallCraft | enum CraftDefensiveTactic |  |  |
| `SmallCraft/CraftDefensiveTrigger.cs` | 8 | SmallCraft | enum CraftDefensiveTrigger |  |  |
| `SmallCraft/CraftEMCON.cs` | 8 | SmallCraft | enum CraftEMCON |  |  |
| `SmallCraft/CraftEvasion_EngageThreat.cs` | 57 | SmallCraft | class CraftEvasion_EngageThreat : CraftEvasionManeuver |  |  |
| `SmallCraft/CraftEvasion_MaxTransverse.cs` | 29 | SmallCraft | class CraftEvasion_MaxTransverse : CraftEvasionManeuver |  |  |
| `SmallCraft/CraftEvasion_Notch.cs` | 40 | SmallCraft | class CraftEvasion_Notch : CraftEvasionManeuver |  |  |
| `SmallCraft/CraftEvasion_Scoot.cs` | 30 | SmallCraft | class CraftEvasion_Scoot : CraftEvasionManeuver |  |  |
| `SmallCraft/CraftEvasion_Scram.cs` | 28 | SmallCraft | class CraftEvasion_Scram : CraftEvasionManeuver |  |  |
| `SmallCraft/CraftEvasion_SweepingTurn.cs` | 41 | SmallCraft | class CraftEvasion_SweepingTurn : CraftEvasion_MaxTransverse |  |  |
| `SmallCraft/CraftEvasionManeuver.cs` | 64 | SmallCraft | class CraftEvasionManeuver |  |  |
| `SmallCraft/CraftEvasionSettings.cs` | 197 | SmallCraft | class CraftEvasionSettings : ScriptableObject<br>struct EvasionGroup |  |  |
| `SmallCraft/CraftFormationType.cs` | 10 | SmallCraft | enum CraftFormationType |  |  |
| `SmallCraft/CraftGuardStyle.cs` | 9 | SmallCraft | enum CraftGuardStyle |  |  |
| `SmallCraft/CraftHangarReference.cs` | 15 | SmallCraft | class CraftHangarReference : IPolySer |  |  |
| `SmallCraft/CraftLoadoutMatrix.cs` | 212 | SmallCraft | class CraftLoadoutMatrix |  |  |
| `SmallCraft/CraftLoadoutSet.cs` | 174 | SmallCraft | class CraftLoadoutSet |  |  |
| `SmallCraft/CraftMainThruster.cs` | 63 | SmallCraft | class CraftMainThruster : MonoBehaviour, BaseCraftMovement.ICraftThruster |  |  |
| `SmallCraft/CraftManeuverThruster.cs` | 78 | SmallCraft | class CraftManeuverThruster : MonoBehaviour, BaseCraftMovement.ICraftThruster | Awake |  |
| `SmallCraft/CraftMovementStyle.cs` | 9 | SmallCraft | enum CraftMovementStyle |  |  |
| `SmallCraft/CraftPassiveSensor.cs` | 173 | SmallCraft | class CraftPassiveSensor : CraftBaseSensor | Awake, OnAdded |  |
| `SmallCraft/CraftSerializers.cs` | 127 | SmallCraft | class CraftSerializers |  |  |
| `SmallCraft/CraftSingleLoadoutMonitor.cs` | 178 | SmallCraft | class CraftSingleLoadoutMonitor : IQuantityMonitor<br>class Record |  |  |
| `SmallCraft/CraftSphereActiveSensor.cs` | 70 | SmallCraft | class CraftSphereActiveSensor : CraftActiveSensor |  |  |
| `SmallCraft/CraftSpherePassiveSensor.cs` | 24 | SmallCraft | class CraftSpherePassiveSensor : CraftPassiveSensor |  |  |
| `SmallCraft/CraftStance.cs` | 9 | SmallCraft | enum CraftStance |  |  |
| `SmallCraft/CraftTargetingActiveSensor.cs` | 46 | SmallCraft | class CraftTargetingActiveSensor : CraftActiveSensor |  |  |
| `SmallCraft/CraftTargetingPassiveSensor.cs` | 92 | SmallCraft | class CraftTargetingPassiveSensor : CraftPassiveSensor, IIntelComponent | FixedUpdate, OnAdded |  |
| `SmallCraft/CraftWeaponCategory.cs` | 9 | SmallCraft | enum CraftWeaponCategory |  |  |
| `SmallCraft/CraftWeaponGroup.cs` | 199 | SmallCraft | class CraftWeaponGroup |  |  |
| `SmallCraft/CraftWeaponRTB.cs` | 9 | SmallCraft | enum CraftWeaponRTB |  |  |
| `SmallCraft/CraftWeaponTargetPreference.cs` | 13 | SmallCraft | enum CraftWeaponTargetPreference |  |  |
| `SmallCraft/EvasionBehaviour.cs` | 12 | SmallCraft | enum EvasionBehaviour |  |  |
| `SmallCraft/FighterMovement.cs` | 626 | SmallCraft | class FighterMovement : BaseCraftMovement, IPIDHost<br>struct AttackEgress | Awake | NetworkBehaviour, SyncVar, XML |
| `SmallCraft/FurballTracker.cs` | 521 | SmallCraft | class FurballTracker : NetworkPoolable | Awake, OnRepooled, OnUnpooled, Update | NetworkBehaviour, SyncVar, NetworkServer, pooling |
| `SmallCraft/ICraftGuardTarget.cs` | 17 | SmallCraft | interface ICraftGuardTarget : INetIdentifiedScript |  |  |
| `SmallCraft/ICraftWeapon.cs` | 76 | SmallCraft | interface ICraftWeapon |  |  |
| `SmallCraft/LoadoutMatrixGeneralMunition.cs` | 31 | SmallCraft | class LoadoutMatrixGeneralMunition : LoadoutMatrixRow |  |  |
| `SmallCraft/LoadoutMatrixMissile.cs` | 31 | SmallCraft | class LoadoutMatrixMissile : LoadoutMatrixRow |  |  |
| `SmallCraft/LoadoutMatrixPod.cs` | 27 | SmallCraft | class LoadoutMatrixPod : LoadoutMatrixRow |  |  |
| `SmallCraft/LoadoutMatrixRow.cs` | 54 | SmallCraft | class LoadoutMatrixRow |  |  |
| `SmallCraft/LoadoutMatrixSlot.cs` | 91 | SmallCraft | class LoadoutMatrixSlot |  |  |
| `SmallCraft/LoadoutSlotDefinition.cs` | 12 | SmallCraft | struct LoadoutSlotDefinition |  |  |
| `SmallCraft/LoadoutSlotUsage.cs` | 12 | SmallCraft | struct LoadoutSlotUsage |  |  |
| `SmallCraft/SavedStoredCraft.cs` | 14 | SmallCraft | struct SavedStoredCraft |  |  |
| `SmallCraft/SerializedCraftLoadout.cs` | 329 | SmallCraft | class SerializedCraftLoadout<br>class GeneralLoadoutElement : IPolySer<br>class VariableSocketLoadout : GeneralLoadoutElement<br>class SimpleOccupiedElement : GeneralLoadoutElement<br>class SimpleAmmoSelection : GeneralLoadoutElement<br>class MissileSelection : GeneralLoadoutElement |  |  |
| `SmallCraft/SerializedCraftTemplate.cs` | 84 | SmallCraft | class SerializedCraftTemplate : IXmlDocSerializable |  | XML |
| `SmallCraft/SkiffMovement.cs` | 233 | SmallCraft | class SkiffMovement : BaseCraftMovement, IPIDHost | Awake |  |
| `SmallCraft/SortieCapabilities.cs` | 47 | SmallCraft | class SortieCapabilities<br>enum Capes |  |  |
| `SmallCraft/Spacecraft.cs` | 5663 | SmallCraft | class Spacecraft : NetworkBehaviour, IFleetTemplateableActive, IBundleKeyed, ISaveKeyed, IModSource, IFactionLocked, IBoardPiece, ISelectable, IOwned, IIden...<br>enum FlightState<br>enum HangarStatus<br>enum HUDIconType<br>class ThreatComparer : IComparer<IThreat><br>class SameTemplateSourceComparer : IEqualityComparer<Spacecraft><br>class ServerEvasionData<br>class EvasionDirection<br>struct LoadoutSelection<br>class SpacecraftSummary : IPointCostItem, ISaveKeyed<br>struct CarriedLoadoutDetails<br>class LoadoutChanger : SaveableCoroutine<br>enum Phase<br>class SpacecraftSaver : SaveFileObject.BulkObjectSaver<br>class SpacecraftSocketSaver : XmlSaveStateHelperWithID<SpacecraftSocket, string><br>class PathfindingRoute | Awake, FixedUpdate, OnDestroy, OnDisable, OnStartClient, OnStartServer | NetworkBehaviour, SyncVar, ClientRpc, NetworkServer, NetworkClient, save-state, XML, prefab |
| `SmallCraft/SpacecraftCommsPath.cs` | 35 | SmallCraft | class SpacecraftCommsPath : ICommPath |  |  |
| `SmallCraft/SpacecraftEwarManager.cs` | 15 | SmallCraft | class SpacecraftEwarManager : NetworkBehaviour |  | NetworkBehaviour |
| `SmallCraft/SpacecraftGroup.cs` | 4300 | SmallCraft | class SpacecraftGroup : NetworkPoolable, ISelectable, IOwned, IBoardPieceGroup, IBoardPiece, ICraftOrderReceiver, IOrderReceiver, INavigationTaskReceiver, IWidge...<br>class CraftFormationStyle<br>struct ReturnToBaseStatus<br>struct PairedEnumerator<br>class SpacecraftGroupSaver : SaveFileObject.BulkObjectSaver | Awake, FixedUpdate, OnDestroy, OnRepooled, OnStartClient, OnStartServer, OnUnpooled | NetworkBehaviour, SyncVar, ClientRpc, NetworkServer, NetworkClient, save-state, XML, pooling, prefab |
| `SmallCraft/SpacecraftGroupPooler.cs` | 200 | SmallCraft | class SpacecraftGroupPooler : SingletonMonobehaviour<SpacecraftGroupPooler><br>class GroupPool : IObjectPool<NetworkPoolable> |  | NetworkServer, pooling, prefab |
| `SmallCraft/SpacecraftMissileManager.cs` | 961 | SmallCraft | class SpacecraftMissileManager : NetworkBehaviour, IMissileSalvoSource, IModifiableStats, IBulkSaveComponent<br>class MissileSlotRecord<br>class SavedMissileSlotLoad<br>class CraftMissilePool : ICraftWeapon, IDisposable, IQuantityHolder<br>class MissileInFlight : IDisposable | FixedUpdate | NetworkBehaviour, ClientRpc, NetworkServer, NetworkClient, save-state, XML |
| `SmallCraft/SpacecraftTemplate.cs` | 84 | SmallCraft | class SpacecraftTemplate : IFleetTemplateableDesign |  |  |
| `Sound/AnnouncerVoiceSet.cs` | 62 | Sound | class AnnouncerVoiceSet : ScriptableObject<br>struct TimeHack<br>struct Objective<br>struct Flag |  |  |
| `Sound/AudioGroupBinding.cs` | 38 | Sound | class AudioGroupBinding : ScriptableObject |  |  |
| `Sound/AudioHelpers.cs` | 12 | Sound | class AudioHelpers |  |  |
| `Sound/AudioMetadataManager.cs` | 138 | Sound | class AudioMetadataManager<br>class LanguageCatalog |  | JSON, Addressables |
| `Sound/BaseSoundEffect.cs` | 89 | Sound | class BaseSoundEffect : ScriptableObject |  | prefab |
| `Sound/BookendedAudioPlayer.cs` | 171 | Sound | class BookendedAudioPlayer : MonoBehaviour<br>enum PlayerState | Awake | prefab |
| `Sound/BookendedSoundEffect.cs` | 69 | Sound | class BookendedSoundEffect : ScriptableObject |  |  |
| `Sound/CraftVoiceCallbackSet.cs` | 44 | Sound | class CraftVoiceCallbackSet : ScriptableObject |  |  |
| `Sound/DynamicSkirmishMusicPlayer.cs` | 252 | Sound | class DynamicSkirmishMusicPlayer : MonoBehaviour<br>enum MusicLayer | Awake | Addressables |
| `Sound/DynamicSkirmishMusicSet.cs` | 14 | Sound | class DynamicSkirmishMusicSet : ScriptableObject |  |  |
| `Sound/GlobalSFX.cs` | 95 | Sound | class GlobalSFX : MonoBehaviour | Awake, OnDestroy | prefab |
| `Sound/GroupedAudioSource.cs` | 137 | Sound | class GroupedAudioSource : MonoBehaviour | Awake | prefab |
| `Sound/IgnoreMute.cs` | 19 | Sound | class IgnoreMute : MonoBehaviour | Awake |  |
| `Sound/IMusicSource.cs` | 18 | Sound | interface IMusicSource |  |  |
| `Sound/MusicManager.cs` | 296 | Sound | class MusicManager : SingletonMonobehaviour<MusicManager><br>enum CrossfadeType<br>class MusicSource : IMusicSource<br>class MusicCrossfader<br>class SymmetricCrossfade : MusicCrossfader<br>class OverlappedCrossfade : MusicCrossfader | Awake | prefab |
| `Sound/MusicTrack.cs` | 15 | Sound | class MusicTrack : ScriptableObject |  |  |
| `Sound/SegmentedMusicTrack.cs` | 106 | Sound | class SegmentedMusicTrack : MusicTrack |  |  |
| `Sound/SingleMusicTrack.cs` | 46 | Sound | class SingleMusicTrack : MusicTrack |  |  |
| `Sound/SoundEffect.cs` | 18 | Sound | class SoundEffect : BaseSoundEffect |  |  |
| `Sound/SpatialSoundEffectPlayer.cs` | 52 | Sound | class SpatialSoundEffectPlayer : Poolable | Awake, OnRepooled, OnUnpooled, Update | pooling |
| `Sound/UISFXPipe.cs` | 12 | Sound | class UISFXPipe : MonoBehaviour |  |  |
| `Sound/VariedSoundEffect.cs` | 41 | Sound | class VariedSoundEffect : BaseSoundEffect |  |  |
| `Sound/VoiceCallbackLine.cs` | 57 | Sound | class VoiceCallbackLine : ScriptableObject |  |  |
| `Sound/VoiceCallbackManager.cs` | 227 | Sound | class VoiceCallbackManager : MonoBehaviour<br>struct QueuedClip | Awake, OnDestroy, Update | prefab |
| `Sound/VoiceCallbackSet.cs` | 94 | Sound | class VoiceCallbackSet : ScriptableObject, IModSource |  |  |
| `Sound/VoiceCallbackType.cs` | 9 | Sound | enum VoiceCallbackType |  |  |
| `Source/Testing/SplineAnimator.cs` | 80 | Source.Testing | class SplineAnimator : MonoBehaviour<br>struct AnimationTriggers |  | prefab |
| `Testing/AvoiderTester.cs` | 33 | Testing | class AvoiderTester : MonoBehaviour | Update |  |
| `Testing/BroadsideTestRig.cs` | 27 | Testing | class BroadsideTestRig : MonoBehaviour | Update |  |
| `Testing/CameraGun.cs` | 29 | Testing | class CameraGun : MonoBehaviour<br>struct Wep |  | prefab |
| `Testing/CircleMergerTester.cs` | 59 | Testing | class CircleMergerTester : ImmediateModeShapeDrawer |  |  |
| `Testing/ConeEnvelopeTest.cs` | 20 | Testing | class ConeEnvelopeTest : MonoBehaviour |  |  |
| `Testing/DrawNormals.cs` | 27 | Testing | class DrawNormals : MonoBehaviour |  |  |
| `Testing/Grapher.cs` | 42 | Testing | class Grapher : MonoBehaviour |  |  |
| `Testing/LeadCalcTester.cs` | 39 | Testing | class LeadCalcTester : MonoBehaviour | Update |  |
| `Testing/MissilePushoffTester.cs` | 79 | Testing | class MissilePushoffTester : MonoBehaviour<br>struct FakeShipController | Update |  |
| `Testing/MissileShooter.cs` | 38 | Testing | class MissileShooter : MonoBehaviour | Start | prefab |
| `Testing/ProceduralMeshTest.cs` | 30 | Testing | class ProceduralMeshTest : MonoBehaviour | Start |  |
| `Testing/TestCalculateRocketAccel.cs` | 33 | Testing | class TestCalculateRocketAccel : MonoBehaviour |  |  |
| `Testing/TestFleetList.cs` | 21 | Testing | class TestFleetList : MonoBehaviour | Start |  |
| `Testing/TestSteamDedi.cs` | 28 | Testing | class TestSteamDedi : MonoBehaviour | Awake |  |
| `Testing/TurretTester.cs` | 74 | Testing | class TurretTester : MonoBehaviour | Update |  |
| `Testing/VisibilityCellTester.cs` | 75 | Testing | class VisibilityCellTester : MonoBehaviour, IVisibilityObject | Awake, Update |  |
| `UI/Accordion.cs` | 77 | UI | class Accordion : MonoBehaviour | Awake | prefab |
| `UI/AccordionGroup.cs` | 41 | UI | class AccordionGroup : MonoBehaviour | Awake | prefab |
| `UI/AddressableTextureSelectItem.cs` | 56 | UI | class AddressableTextureSelectItem : SelectableListItem | OnDestroy | Addressables |
| `UI/AnimatedImage.cs` | 42 | UI | class AnimatedImage : MonoBehaviour | Start, Update |  |
| `UI/AnimatedText.cs` | 112 | UI | class AnimatedText : MonoBehaviour<br>enum Animation |  |  |
| `UI/AspectRatioFromHeightLayoutFitter.cs` | 51 | UI | class AspectRatioFromHeightLayoutFitter : MonoBehaviour, ILayoutElement |  |  |
| `UI/BackgroundVideo.cs` | 83 | UI | class BackgroundVideo : SingletonMonobehaviour<BackgroundVideo> | Awake | prefab |
| `UI/BadgeListItem.cs` | 30 | UI | class BadgeListItem : SelectableListItem |  |  |
| `UI/BaseLobbyPlayerSlot.cs` | 187 | UI | class BaseLobbyPlayerSlot : MonoBehaviour where TPlayer : LobbyPlayer | OnDestroy | prefab |
| `UI/BaseLobbySettingsPane.cs` | 93 | UI | class BaseLobbySettingsPane : MonoBehaviour |  | prefab |
| `UI/BaseMenu.cs` | 42 | UI | class BaseMenu : MonoBehaviour |  |  |
| `UI/Blinker.cs` | 71 | UI | class Blinker : MonoBehaviour | OnDisable, OnEnable |  |
| `UI/ButtonClickSound.cs` | 17 | UI | class ButtonClickSound : MonoBehaviour, IPointerDownHandler, IEventSystemHandler |  |  |
| `UI/ButtonRolloverSound.cs` | 17 | UI | class ButtonRolloverSound : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler |  |  |
| `UI/CampaignBrowser.cs` | 308 | UI | class CampaignBrowser : BaseMenu, ISelectList<br>class FullCampaignRecord |  | Addressables, prefab |
| `UI/CampaignEditorListItem.cs` | 43 | UI | class CampaignEditorListItem : SelectableFileListItem |  |  |
| `UI/CampaignListItem.cs` | 101 | UI | class CampaignListItem : SelectableListItem |  | prefab |
| `UI/CampaignLobbyMenu.cs` | 180 | UI | class CampaignLobbyMenu : BaseMenu | OnDestroy | prefab |
| `UI/CampaignLobbyPlayerSlot.cs` | 61 | UI | class CampaignLobbyPlayerSlot : BaseLobbyPlayerSlot<CampaignLobbyPlayer, CampaignLobbyMenu> |  | prefab |
| `UI/CampaignLobbySettingsPane.cs` | 136 | UI | class CampaignLobbySettingsPane : BaseLobbySettingsPane | OnDestroy | prefab |
| `UI/ChatVote.cs` | 89 | UI | class ChatVote : MonoBehaviour | Update | prefab |
| `UI/ChatWindow.cs` | 523 | UI | class ChatWindow : MonoBehaviour<br>class Line | OnDestroy, Start, Update | prefab |
| `UI/Chessboard/DeploymentLineDrawer.cs` | 65 | UI.Chessboard | class DeploymentLineDrawer : ImmediateModeShapeDrawer |  |  |
| `UI/ColorblindPaletteDisplay.cs` | 47 | UI | class ColorblindPaletteDisplay : MonoBehaviour | Start |  |
| `UI/ColorSelectBase.cs` | 100 | UI | class ColorSelectBase : MonoBehaviour | Awake, Update |  |
| `UI/ColorSelectShader.cs` | 16 | UI | class ColorSelectShader : ColorSelectBase |  |  |
| `UI/ColorSelectTexture.cs` | 90 | UI | class ColorSelectTexture : ColorSelectBase | Awake, OnDestroy |  |
| `UI/Controls/BindableControlAttribute.cs` | 21 | UI.Controls | class BindableControlAttribute : Attribute |  |  |
| `UI/Controls/BindableControlGroupAttribute.cs` | 15 | UI.Controls | class BindableControlGroupAttribute : Attribute |  |  |
| `UI/Controls/BindableControls.cs` | 389 | UI.Controls | enum BindableControls |  |  |
| `UI/Controls/Chord.cs` | 49 | UI.Controls | struct Chord |  |  |
| `UI/Controls/ControlContext.cs` | 10 | UI.Controls | enum ControlContext |  |  |
| `UI/Controls/ControlRebindingItem.cs` | 62 | UI.Controls | class ControlRebindingItem : MonoBehaviour |  |  |
| `UI/Controls/KeyboardShortcutText.cs` | 53 | UI.Controls | class KeyboardShortcutText : MonoBehaviour | Awake, OnDestroy |  |
| `UI/Controls/KeyCodeExtensions.cs` | 293 | UI.Controls | class KeyCodeExtensions |  |  |
| `UI/Controls/Keymap.cs` | 325 | UI.Controls | class Keymap<br>struct Control<br>class SerializedControls |  | XML |
| `UI/Controls/RebindingPrompt.cs` | 94 | UI.Controls | class RebindingPrompt : MonoBehaviour | OnDisable, OnEnable | prefab |
| `UI/CoordinatePrinter.cs` | 53 | UI | class CoordinatePrinter : MonoBehaviour | Update | prefab |
| `UI/CreditsMenu.cs` | 25 | UI | class CreditsMenu : BaseMenu | Update |  |
| `UI/CutsceneSubtitleSpeaker.cs` | 23 | UI | class CutsceneSubtitleSpeaker : SubtitleSpeaker |  |  |
| `UI/Desktop/ContentLoadingCover.cs` | 119 | UI.Desktop | class ContentLoadingCover : MonoBehaviour, IProgress<float><br>interface ILoadingTask<br>class LoadingTask : ILoadingTask | OnDestroy | prefab |
| `UI/Desktop/ContextMenuFoldoutItem.cs` | 50 | UI.Desktop | class ContextMenuFoldoutItem : InvertGraphicSelectable | Awake | prefab |
| `UI/Desktop/ContextMenuItem.cs` | 45 | UI.Desktop | class ContextMenuItem : MonoBehaviour |  | prefab |
| `UI/Desktop/ContextMenuToggleItem.cs` | 20 | UI.Desktop | class ContextMenuToggleItem : ContextMenuItem |  | prefab |
| `UI/Desktop/DesktopContextMenu.cs` | 328 | UI.Desktop | class DesktopContextMenu : SingletonMonobehaviour<DesktopContextMenu><br>enum CommonIcons<br>interface ICtxtMenuEntry<br>struct CMDivider : ICtxtMenuEntry<br>struct CMOption : ICtxtMenuEntry<br>struct CMToggle : ICtxtMenuEntry<br>struct CMFoldout | Awake, LateUpdate, OnDestroy | prefab |
| `UI/Desktop/DesktopWindow.cs` | 324 | UI.Desktop | class DesktopWindow : MonoBehaviour, IPointerDownHandler, IEventSystemHandler | Awake, OnDestroy | prefab |
| `UI/Desktop/DockedDesktopWindow.cs` | 135 | UI.Desktop | class DockedDesktopWindow : MonoBehaviour | OnDestroy | prefab |
| `UI/Desktop/ExpandableWindowContent.cs` | 54 | UI.Desktop | class ExpandableWindowContent : WindowContent |  | prefab |
| `UI/Desktop/LineDrawingWindowContent.cs` | 38 | UI.Desktop | class LineDrawingWindowContent : WindowContent | OnDisable, OnEnable |  |
| `UI/Desktop/TaskbarItem.cs` | 119 | UI.Desktop | class TaskbarItem : MonoBehaviour | Awake, OnDestroy | prefab |
| `UI/Desktop/VirtualDesktop.cs` | 277 | UI.Desktop | class VirtualDesktop : SingletonMonobehaviour<VirtualDesktop><br>class ActiveWindow | Start | prefab |
| `UI/Desktop/WindowContent.cs` | 165 | UI.Desktop | class WindowContent : MonoBehaviour |  |  |
| `UI/Desktop/WindowContentLoading.cs` | 37 | UI.Desktop | class WindowContentLoading : WindowContent |  |  |
| `UI/Desktop/WindowResizeHandle.cs` | 40 | UI.Desktop | class WindowResizeHandle : MonoBehaviour, IDragHandler, IEventSystemHandler, IBeginDragHandler | Awake |  |
| `UI/Desktop/WindowTitleBar.cs` | 135 | UI.Desktop | class WindowTitleBar : MonoBehaviour, IDragHandler, IEventSystemHandler, IBeginDragHandler | Awake | prefab |
| `UI/DoubleTextIconCountListItem.cs` | 20 | UI | class DoubleTextIconCountListItem : DoubleTextIconListItem |  |  |
| `UI/DoubleTextIconListItem.cs` | 29 | UI | class DoubleTextIconListItem : DoubleTextListItem |  | prefab |
| `UI/DoubleTextListItem.cs` | 43 | UI | class DoubleTextListItem : SelectableListItem |  |  |
| `UI/DraggableZoomableScrollRect.cs` | 119 | UI | class DraggableZoomableScrollRect : ScrollRect |  |  |
| `UI/DragHandle.cs` | 112 | UI | class DragHandle : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler | Update |  |
| `UI/DragListScrollBuffer.cs` | 41 | UI | class DragListScrollBuffer : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler |  |  |
| `UI/DualClickButton.cs` | 56 | UI | class DualClickButton : Button |  | prefab |
| `UI/ExpandingScrollViewLayout.cs` | 30 | UI | class ExpandingScrollViewLayout : MonoBehaviour |  |  |
| `UI/FileManagementFolderContents.cs` | 223 | UI | class FileManagementFolderContents : SelectableListItem, ISelectList | Awake, OnDestroy | prefab |
| `UI/FilterableDropdown.cs` | 131 | UI | class FilterableDropdown : TMP_Dropdown_WithMouseBlocker<br>class FilterableItem |  | prefab |
| `UI/FixedEdgesScrollView.cs` | 74 | UI | class FixedEdgesScrollView : MonoBehaviour | Awake |  |
| `UI/FolderDropTarget.cs` | 46 | UI | class FolderDropTarget : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IDropHandler |  | prefab |
| `UI/GridLayoutGroupFlexible.cs` | 379 | UI | class GridLayoutGroupFlexible : LayoutGroup<br>enum Axis |  |  |
| `UI/HierarchyViewList.cs` | 266 | UI | class HierarchyViewList : MonoBehaviour, ISelectList<br>class HierarchyItem : MonoBehaviour, IComparable<HierarchyItem> | Awake, OnDestroy | prefab |
| `UI/HoldButton.cs` | 77 | UI | class HoldButton : Button | OnEnable, Update |  |
| `UI/HoverTooltipTMPBlock.cs` | 83 | UI | class HoverTooltipTMPBlock : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler | Update |  |
| `UI/HUDMarkerTheme.cs` | 178 | UI | class HUDMarkerTheme : ScriptableObject, IModSource, IBundleKeyed, ISaveKeyed |  |  |
| `UI/HUDMarkerType.cs` | 33 | UI | class HUDMarkerType : ScriptableObject |  |  |
| `UI/IconOnlyListItem.cs` | 42 | UI | class IconOnlyListItem : SelectableListItem |  |  |
| `UI/IHierarchyElement.cs` | 17 | UI | interface IHierarchyElement |  |  |
| `UI/InputSpinner.cs` | 42 | UI | class InputSpinner : Spinner | Start |  |
| `UI/InvertGraphicButton.cs` | 46 | UI | class InvertGraphicButton : Button |  |  |
| `UI/InvertGraphicSelectable.cs` | 46 | UI | class InvertGraphicSelectable : Selectable |  |  |
| `UI/InvertGraphicToggle.cs` | 92 | UI | class InvertGraphicToggle : Toggle | Awake, Start |  |
| `UI/ISelectList.cs` | 15 | UI | interface ISelectList |  |  |
| `UI/ISettable.cs` | 15 | UI | interface ISettable<br>interface ISettable |  |  |
| `UI/ISkirmishLobbyMenu.cs` | 20 | UI | interface ISkirmishLobbyMenu |  |  |
| `UI/LinkedSpreadsheetRow.cs` | 65 | UI | class LinkedSpreadsheetRow : SpreadsheetRow, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler | Update |  |
| `UI/LoadingBarWithStatus.cs` | 52 | UI | class LoadingBarWithStatus : MonoBehaviour, IStatusProgress<float>, IProgress<float> |  |  |
| `UI/LoadingScreen.cs` | 84 | UI | class LoadingScreen : BaseMenu, IStatusProgress<float>, IProgress<float> |  | prefab |
| `UI/LobbyModPane.cs` | 118 | UI | class LobbyModPane : MonoBehaviour<br>enum Tab | Awake | prefab |
| `UI/LocalizedSceneString.cs` | 50 | UI | class LocalizedSceneString : MonoBehaviour | Awake, OnDestroy |  |
| `UI/LocalizedUIString.cs` | 50 | UI | class LocalizedUIString : MonoBehaviour | Awake, OnDestroy |  |
| `UI/MainMenu.cs` | 361 | UI | class MainMenu : BaseMenu | OnDestroy, Start |  |
| `UI/MainSubtitleSpeaker.cs` | 254 | UI | class MainSubtitleSpeaker : SubtitleSpeaker | Awake, OnDisable, OnEnable | prefab |
| `UI/MatchBrowser.cs` | 444 | UI | class MatchBrowser : BaseMenu | Awake, OnDestroy, Update | Command, prefab |
| `UI/MatchColorblindMode.cs` | 38 | UI | class MatchColorblindMode : MonoBehaviour | Awake |  |
| `UI/MatchColorblindModeButton.cs` | 31 | UI | class MatchColorblindModeButton : MonoBehaviour | Awake |  |
| `UI/MatchTeamColor.cs` | 24 | UI | class MatchTeamColor : MonoBehaviour | Awake |  |
| `UI/MenuController.cs` | 253 | UI | class MenuController : MonoBehaviour | Awake, OnDestroy, Update | prefab |
| `UI/ModalButtonList.cs` | 65 | UI | class ModalButtonList : BaseMenu<br>struct Option |  | prefab |
| `UI/ModalCampaignFileList.cs` | 64 | UI | class ModalCampaignFileList : ModalFileManagement<CampaignEditorListItem, NebCampaign.CampaignFileSummary> |  | prefab |
| `UI/ModalCheckboxList.cs` | 93 | UI | class ModalCheckboxList : BaseMenu |  | prefab |
| `UI/ModalConfirm.cs` | 57 | UI | class ModalConfirm : BaseMenu |  | prefab |
| `UI/ModalDropdownSelect.cs` | 58 | UI | class ModalDropdownSelect : BaseMenu |  |  |
| `UI/ModalEditPlayerProfile.cs` | 176 | UI | class ModalEditPlayerProfile : BaseMenu | Start |  |
| `UI/ModalFactionList.cs` | 26 | UI | class ModalFactionList : ModalListSelectDetailed |  | prefab |
| `UI/ModalFileManagement.cs` | 535 | UI | class ModalFileManagement : BaseMenu, ISelectList where TListItem : SelectableFileListItem where TData : class, IComparable<TData> |  | prefab |
| `UI/ModalFileManagementWorkshop.cs` | 32 | UI | class ModalFileManagementWorkshop : ModalFileManagement<TListItem, TData> where TListItem : SelectableWorkshopListItem where TData : class, IComparable<TData> |  |  |
| `UI/ModalFleetList.cs` | 135 | UI | class ModalFleetList : ModalFileManagementWorkshop<FleetListItem, Fleet.FleetSummary> |  |  |
| `UI/ModalGameInfo.cs` | 100 | UI | class ModalGameInfo : BaseMenu |  | Command, prefab |
| `UI/ModalHostGame.cs` | 77 | UI | class ModalHostGame : BaseMenu |  |  |
| `UI/ModalListSelect.cs` | 246 | UI | class ModalListSelect : BaseMenu, ISelectList<br>struct Filter | Awake | prefab |
| `UI/ModalListSelectDetailed.cs` | 74 | UI | class ModalListSelectDetailed : ModalListSelect |  | prefab |
| `UI/ModalModDownloadList.cs` | 56 | UI | class ModalModDownloadList : BaseMenu |  | prefab |
| `UI/ModalModManager.cs` | 642 | UI | class ModalModManager : BaseMenu, ISelectList | Start | prefab |
| `UI/ModalMPFilters.cs` | 130 | UI | class ModalMPFilters : BaseMenu |  |  |
| `UI/ModalPromotion.cs` | 54 | UI | class ModalPromotion : BaseMenu |  |  |
| `UI/ModalReportPlayer.cs` | 53 | UI | class ModalReportPlayer : BaseMenu | Awake |  |
| `UI/ModalSaveGameList.cs` | 258 | UI | class ModalSaveGameList : ModalListSelect |  | prefab |
| `UI/ModalSettings.cs` | 713 | UI | class ModalSettings : BaseMenu | Start | prefab |
| `UI/ModalShipTemplateList.cs` | 103 | UI | class ModalShipTemplateList : ModalFileManagementWorkshop<ShipTemplateListItem, Ship.ShipSummary> |  | prefab |
| `UI/ModalSimpleColorPicker.cs` | 98 | UI | class ModalSimpleColorPicker : BaseMenu | Start | prefab |
| `UI/ModalTextEntry.cs` | 91 | UI | class ModalTextEntry : BaseMenu |  |  |
| `UI/ModalTwoConfirm.cs` | 55 | UI | class ModalTwoConfirm : BaseMenu |  |  |
| `UI/ModalWorking.cs` | 101 | UI | class ModalWorking : BaseMenu, IProgress<float>, IStatusProgress<float> | Update | prefab |
| `UI/ModDownloadListItem.cs` | 96 | UI | class ModDownloadListItem : MonoBehaviour |  |  |
| `UI/ModListItem.cs` | 128 | UI | class ModListItem : SelectableListItem |  | prefab |
| `UI/MPMatchEntry.cs` | 274 | UI | class MPMatchEntry : MonoBehaviour<br>class SortDefault : IComparer<MPMatchEntry><br>class SortName : IComparer<MPMatchEntry><br>class SortMode : IComparer<MPMatchEntry><br>class SortPlayers : IComparer<MPMatchEntry><br>class SortMap : IComparer<MPMatchEntry><br>class SortPing : IComparer<MPMatchEntry><br>class SortLabels : IComparer<MPMatchEntry> |  | prefab |
| `UI/MultiGraphic.cs` | 82 | UI | class MultiGraphic : NonDrawingGraphic |  |  |
| `UI/NestableScrollRect.cs` | 68 | UI | class NestableScrollRect : ScrollRect |  | prefab |
| `UI/OpposedTransformFillBars.cs` | 35 | UI | class OpposedTransformFillBars : MonoBehaviour |  |  |
| `UI/PlayerColorDisplay.cs` | 76 | UI | class PlayerColorDisplay : MonoBehaviour |  | prefab |
| `UI/PlayerColorProfileSlot.cs` | 86 | UI | class PlayerColorProfileSlot : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler |  | prefab |
| `UI/PlayerDropdownMenu.cs` | 78 | UI | class PlayerDropdownMenu : MonoBehaviour |  |  |
| `UI/PlayerProfileDisplay.cs` | 81 | UI | class PlayerProfileDisplay : MonoBehaviour | Start |  |
| `UI/PlayerRankDisplay.cs` | 120 | UI | class PlayerRankDisplay : MonoBehaviour |  |  |
| `UI/PopoutListSelector.cs` | 118 | UI | class PopoutListSelector : MonoBehaviour<br>enum SelectListType |  |  |
| `UI/Pulldown.cs` | 87 | UI | class Pulldown : MonoBehaviour | Awake, LateUpdate | prefab |
| `UI/RadialLayout.cs` | 31 | UI | class RadialLayout : MonoBehaviour | Update |  |
| `UI/RandomBackdropSelector.cs` | 19 | UI | class RandomBackdropSelector : MonoBehaviour | Awake |  |
| `UI/RightClickButton.cs` | 16 | UI | class RightClickButton : Button |  |  |
| `UI/SavedCampaignGameListItem.cs` | 177 | UI | class SavedCampaignGameListItem : SavedGameListItem<br>class SnapshotItem : SavedGameListItem | OnDestroy | prefab |
| `UI/SavedGameListItem.cs` | 17 | UI | class SavedGameListItem : SelectableListItem |  |  |
| `UI/SavedSkirmishGameListItem.cs` | 43 | UI | class SavedSkirmishGameListItem : SavedGameListItem |  |  |
| `UI/SelectableFileListItem.cs` | 97 | UI | class SelectableFileListItem : SelectableListItem, IPointerDownHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler, IDragHandler |  |  |
| `UI/SelectableListItem.cs` | 130 | UI | class SelectableListItem : MonoBehaviour<br>enum SelectEffect | Awake | prefab |
| `UI/SelectableWorkshopListItem.cs` | 21 | UI | class SelectableWorkshopListItem : SelectableFileListItem |  | prefab |
| `UI/SequentialButton.cs` | 244 | UI | class SequentialButton : Button<br>struct SequenceOption<br>class SequentialButtonEvent : UnityEvent<int> | Awake | prefab |
| `UI/SequentialButtonGroup.cs` | 153 | UI | class SequentialButtonGroup : Button<br>class SequentialButtonGroupEvent : UnityEvent<int[]> | Awake, Update | prefab |
| `UI/Shadowbox.cs` | 27 | UI | class Shadowbox : MonoBehaviour | Start |  |
| `UI/ShadowboxElement.cs` | 92 | UI | class ShadowboxElement : MonoBehaviour | Start, Update |  |
| `UI/ShipFormationWeb.cs` | 141 | UI | class ShipFormationWeb : ImmediateModeShapeDrawer | Awake, OnDestroy, Start |  |
| `UI/ShipSummaryList.cs` | 38 | UI | class ShipSummaryList : MonoBehaviour |  | prefab |
| `UI/ShipSummaryListItem.cs` | 120 | UI | class ShipSummaryListItem : MonoBehaviour |  | prefab |
| `UI/ShipSummaryTooltip.cs` | 87 | UI | class ShipSummaryTooltip : MonoBehaviour | Start, Update | prefab |
| `UI/SimpleSelectList.cs` | 91 | UI | class SimpleSelectList : MonoBehaviour, ISelectList<br>class SelectListEvent : UnityEvent<SelectableListItem, bool> |  | prefab |
| `UI/SimpleSpinner.cs` | 38 | UI | class SimpleSpinner : Spinner |  |  |
| `UI/SingleImageToggle.cs` | 130 | UI | class SingleImageToggle : Toggle | Awake, OnEnable |  |
| `UI/SingleTextListItem.cs` | 39 | UI | class SingleTextListItem : SelectableListItem |  |  |
| `UI/SkirmishDebriefingPlayerSlot.cs` | 48 | UI | class SkirmishDebriefingPlayerSlot : MonoBehaviour | Awake | prefab |
| `UI/SkirmishLobbyFleetDisplay.cs` | 137 | UI | class SkirmishLobbyFleetDisplay : MonoBehaviour | OnDestroy | prefab |
| `UI/SkirmishLobbyFleetList.cs` | 53 | UI | class SkirmishLobbyFleetList : MonoBehaviour |  | prefab |
| `UI/SkirmishLobbyMenu.cs` | 313 | UI | class SkirmishLobbyMenu : BaseMenu, ISkirmishLobbyMenu | OnDestroy | prefab |
| `UI/SkirmishLobbyPlayerSlot.cs` | 361 | UI | class SkirmishLobbyPlayerSlot : BaseLobbyPlayerSlot<SkirmishLobbyPlayer, SkirmishLobbyMenu> | OnDestroy | prefab |
| `UI/SkirmishLobbySettingsPane.cs` | 96 | UI | class SkirmishLobbySettingsPane : BaseLobbySettingsPane |  | prefab |
| `UI/SkirmishLobbyTeam.cs` | 188 | UI | class SkirmishLobbyTeam : MonoBehaviour |  | prefab |
| `UI/SkirmishMissionMenu.cs` | 104 | UI | class SkirmishMissionMenu : BaseMenu, ISkirmishLobbyMenu | OnDestroy |  |
| `UI/SliderValue.cs` | 34 | UI | class SliderValue : MonoBehaviour | Start |  |
| `UI/SortHeader.cs` | 76 | UI | class SortHeader : Button<br>class SortHeaderEvent : UnityEvent<SortingDirection> | Awake | prefab |
| `UI/SortHeaderGroup.cs` | 73 | UI | class SortHeaderGroup : MonoBehaviour<br>class SortHeaderGroupEvent : UnityEvent<int, SortingDirection> | Awake |  |
| `UI/Spinner.cs` | 168 | UI | class Spinner : MonoBehaviour | Start |  |
| `UI/SpreadsheetRow.cs` | 66 | UI | class SpreadsheetRow : MonoBehaviour |  | prefab |
| `UI/SubtitleSpeaker.cs` | 189 | UI | class SubtitleSpeaker : MonoBehaviour<br>enum ButtonMode | Awake, OnDisable, OnEnable | prefab |
| `UI/TabbedControl.cs` | 125 | UI | class TabbedControl : MonoBehaviour | Start | prefab |
| `UI/TabNavigation.cs` | 51 | UI | class TabNavigation : MonoBehaviour | Update | prefab |
| `UI/TextToggle.cs` | 49 | UI | class TextToggle : Toggle | Start |  |
| `UI/TMP_Dropdown_WithMouseBlocker.cs` | 15 | UI | class TMP_Dropdown_WithMouseBlocker : TMP_Dropdown |  | prefab |
| `UI/TMP_InputField_FocusedScrollOnly.cs` | 20 | UI | class TMP_InputField_FocusedScrollOnly : TMP_InputField |  | prefab |
| `UI/TMP_LocalizedText.cs` | 75 | UI | class TMP_LocalizedText : TextMeshProUGUI | Awake, OnDestroy |  |
| `UI/Tooltip.cs` | 207 | UI | class Tooltip : MonoBehaviour | Awake, OnDestroy, OnDisable, Start, Update | prefab |
| `UI/TooltipTrigger.cs` | 116 | UI | class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler | OnDestroy, OnDisable, Update | prefab |
| `UI/TransformFillBar.cs` | 70 | UI | class TransformFillBar : MonoBehaviour |  |  |
| `UI/TriangleSlider.cs` | 79 | UI | class TriangleSlider : MonoBehaviour | Awake, Update |  |
| `UI/TruncatedTextTooltipTrigger.cs` | 38 | UI | class TruncatedTextTooltipTrigger : TooltipTrigger | Awake |  |
| `UI/UIKeyboardBlocker.cs` | 55 | UI | class UIKeyboardBlocker : MonoBehaviour | OnDestroy, OnDisable |  |
| `UI/UIMouseBlocker.cs` | 49 | UI | class UIMouseBlocker : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler | OnDestroy, OnDisable |  |
| `UI/UnscramblingText.cs` | 119 | UI | class UnscramblingText : MonoBehaviour |  |  |
| `UI/VariableCanvasScaler.cs` | 49 | UI | class VariableCanvasScaler : MonoBehaviour | OnDestroy, Start |  |
| `UI/VersionText.cs` | 15 | UI | class VersionText : MonoBehaviour | Awake |  |
| `UI/Widgets/BasePointerSelectionWidget.cs` | 188 | UI.Widgets | class BasePointerSelectionWidget : PointerWidget<TWidget> where TWidget : MonoBehaviour where TComponent : class, ISelectable<br>class Output : WidgetOutput | Update | prefab |
| `UI/Widgets/BasePositionSelectWidget.cs` | 733 | UI.Widgets | class BasePositionSelectWidget : BaseWorldspaceWidget<TWidget>, IPositionWidget, IWidget where TWidget : MonoBehaviour |  | prefab |
| `UI/Widgets/BaseScreenspaceWidget.cs` | 54 | UI.Widgets | class BaseScreenspaceWidget : BaseWidget<TWidget>, IWidget where TWidget : MonoBehaviour | Update |  |
| `UI/Widgets/BaseWidget.cs` | 75 | UI.Widgets | class BaseWidget : SingletonMonobehaviour<TWidget> where TWidget : MonoBehaviour | Awake | prefab |
| `UI/Widgets/BaseWorldspaceWidget.cs` | 181 | UI.Widgets | class BaseWorldspaceWidget : BaseWidget<TWidget>, IWidget where TWidget : MonoBehaviour | OnDisable, OnEnable, Start, Update | prefab |
| `UI/Widgets/CollisionCheckCallback.cs` | 6 | UI.Widgets |  |  |  |
| `UI/Widgets/CraftSelectWidget.cs` | 8 | UI.Widgets | class CraftSelectWidget : BasePointerSelectionWidget<CraftSelectWidget, SpacecraftGroup> |  |  |
| `UI/Widgets/DialPositionSelectWidget.cs` | 96 | UI.Widgets | class DialPositionSelectWidget : BasePositionSelectWidget<DialPositionSelectWidget>, IHorizontalPlane | Update |  |
| `UI/Widgets/EditorShipSelectWidget.cs` | 8 | UI.Widgets | class EditorShipSelectWidget : BasePointerSelectionWidget<EditorShipSelectWidget, EditorShipController> |  |  |
| `UI/Widgets/IPositionWidget.cs` | 40 | UI.Widgets | interface IPositionWidget : IWidget |  |  |
| `UI/Widgets/IWidget.cs` | 23 | UI.Widgets | interface IWidget |  |  |
| `UI/Widgets/IWidgetGameManager.cs` | 31 | UI.Widgets | interface IWidgetGameManager |  |  |
| `UI/Widgets/IWidgetPositionSource.cs` | 23 | UI.Widgets | interface IWidgetPositionSource |  |  |
| `UI/Widgets/OrientationSelectWidget.cs` | 118 | UI.Widgets | class OrientationSelectWidget : BaseWorldspaceWidget<OrientationSelectWidget><br>class OrientationOutput : WidgetOutput | Update |  |
| `UI/Widgets/PointerWidget.cs` | 63 | UI.Widgets | class PointerWidget : BaseWorldspaceWidget<TWidget> where TWidget : MonoBehaviour |  |  |
| `UI/Widgets/PositionPathOutput.cs` | 33 | UI.Widgets | class PositionPathOutput : WidgetOutput |  |  |
| `UI/Widgets/ShipSelectWidget.cs` | 8 | UI.Widgets | class ShipSelectWidget : BasePointerSelectionWidget<ShipSelectWidget, ShipController> |  |  |
| `UI/Widgets/SpherePositionSelectWidget.cs` | 328 | UI.Widgets | class SpherePositionSelectWidget : BasePositionSelectWidget<TWidget>, IProjectionSphere, IHorizontalPlane where TWidget : MonoBehaviour | Awake, Update |  |
| `UI/Widgets/SpherePositionSelectWidgetFive.cs` | 42 | UI.Widgets | class SpherePositionSelectWidgetFive : SpherePositionSelectWidget<SpherePositionSelectWidgetFive> |  |  |
| `UI/Widgets/SpherePositionSelectWidgetFour.cs` | 101 | UI.Widgets | class SpherePositionSelectWidgetFour : SpherePositionSelectWidget<SpherePositionSelectWidgetFour> |  |  |
| `UI/Widgets/SpherePositionSelectWidgetOne.cs` | 59 | UI.Widgets | class SpherePositionSelectWidgetOne : SpherePositionSelectWidget<SpherePositionSelectWidgetOne> |  |  |
| `UI/Widgets/SpherePositionSelectWidgetSeven.cs` | 49 | UI.Widgets | class SpherePositionSelectWidgetSeven : SpherePositionSelectWidget<SpherePositionSelectWidgetSeven> |  |  |
| `UI/Widgets/SpherePositionSelectWidgetSix.cs` | 41 | UI.Widgets | class SpherePositionSelectWidgetSix : SpherePositionSelectWidget<SpherePositionSelectWidgetSix> |  |  |
| `UI/Widgets/SpherePositionSelectWidgetThree.cs` | 41 | UI.Widgets | class SpherePositionSelectWidgetThree : SpherePositionSelectWidget<SpherePositionSelectWidgetThree> |  |  |
| `UI/Widgets/SpherePositionSelectWidgetTwo.cs` | 26 | UI.Widgets | class SpherePositionSelectWidgetTwo : SpherePositionSelectWidget<SpherePositionSelectWidgetTwo> |  |  |
| `UI/Widgets/StaticWidgetPositionSource.cs` | 33 | UI.Widgets | class StaticWidgetPositionSource : IWidgetPositionSource |  |  |
| `UI/Widgets/TrackPositionSource.cs` | 160 | UI.Widgets | class TrackPositionSource : IWidgetPositionSource, IBoardPiece |  |  |
| `UI/Widgets/TrackSelectWidget.cs` | 469 | UI.Widgets | class TrackSelectWidget : PointerWidget<TrackSelectWidget><br>class Output : WidgetOutput | Update |  |
| `UI/Widgets/TriadWidget.cs` | 588 | UI.Widgets | class TriadWidget : ImmediateModeShapeDrawer<br>interface ITransformable<br>enum Axis<br>enum Mode<br>enum SnapScale | OnDestroy, OnDisable, Update | prefab |
| `UI/Widgets/WidgetCallback.cs` | 4 | UI.Widgets |  |  |  |
| `UI/Widgets/WidgetOnscreenText.cs` | 102 | UI.Widgets | class WidgetOnscreenText : MonoBehaviour | Start | prefab |
| `UI/Widgets/WidgetOutput.cs` | 6 | UI.Widgets | class WidgetOutput |  |  |
| `UI/WorldPath.cs` | 178 | UI | class WorldPath : ImmediateModeShapeDrawer |  | prefab |
| `UI/WorldspaceHandle.cs` | 251 | UI | class WorldspaceHandle : MonoBehaviour<br>enum HandleStatus<br>struct HandleAppearance | OnDestroy, Update |  |
| `UI/WorldTransformHandles.cs` | 114 | UI | class WorldTransformHandles : MonoBehaviour | OnDestroy, Start, Update | prefab |
| `Utility/AddressableHelpers.cs` | 126 | Utility | class AddressableHelpers |  | Addressables |
| `Utility/AddressableResourceCollection.cs` | 323 | Utility | class AddressableResourceCollection : IReadOnlyAddressableResourceCollection<T>, IEnumerable<AssetAddress<T>>, IEnumerable<br>struct CollectionType : IEquatable<CollectionType> |  | NetworkClient, Addressables, prefab |
| `Utility/AnimationExtensions.cs` | 37 | Utility | class AnimationExtensions |  |  |
| `Utility/ArgumentsParser.cs` | 40 | Utility | class ArgumentsParser |  |  |
| `Utility/ArrayExtensions/ArrayExtensions.cs` | 20 | Utility.ArrayExtensions | class ArrayExtensions |  |  |
| `Utility/ArrayExtensions/ArrayTraverse.cs` | 38 | Utility.ArrayExtensions | class ArrayTraverse |  |  |
| `Utility/AssetAddress.cs` | 152 | Utility | struct AssetAddress : IAssetAddress, IXmlDocSerializable |  | XML, Addressables, prefab |
| `Utility/AssetAddressHelpers.cs` | 63 | Utility | class AssetAddressHelpers |  |  |
| `Utility/AssetBundleExtensions.cs` | 32 | Utility | class AssetBundleExtensions |  | XML, AssetBundle |
| `Utility/AsyncProgress.cs` | 16 | Utility | class AsyncProgress : IProgress<float> |  |  |
| `Utility/AsyncQueue.cs` | 69 | Utility | class AsyncQueue |  |  |
| `Utility/Attitude.cs` | 12 | Utility | enum Attitude |  |  |
| `Utility/AttitudeControl.cs` | 16 | Utility | enum AttitudeControl |  |  |
| `Utility/AttitudeExtensions.cs` | 57 | Utility | class AttitudeExtensions |  |  |
| `Utility/Avoider.cs` | 220 | Utility | class Avoider : MonoBehaviour |  |  |
| `Utility/AxialBillboard.cs` | 30 | Utility | class AxialBillboard : MonoBehaviour | Update |  |
| `Utility/BarycentricVector.cs` | 201 | Utility | struct BarycentricVector |  |  |
| `Utility/BasicTypeExtensions.cs` | 159 | Utility | class BasicTypeExtensions |  |  |
| `Utility/BoardingStatus.cs` | 9 | Utility | enum BoardingStatus |  |  |
| `Utility/CalloutType.cs` | 12 | Utility | enum CalloutType |  |  |
| `Utility/CalloutTypeExtensions.cs` | 14 | Utility | class CalloutTypeExtensions |  |  |
| `Utility/CastType.cs` | 10 | Utility | enum CastType |  |  |
| `Utility/ChatChannel.cs` | 8 | Utility | enum ChatChannel |  |  |
| `Utility/CircleMerger.cs` | 104 | Utility | class CircleMerger<br>struct Circle<br>class ProcessedCircle |  |  |
| `Utility/ColorblindMode.cs` | 11 | Utility | enum ColorblindMode |  |  |
| `Utility/CombatFacing.cs` | 9 | Utility | enum CombatFacing |  |  |
| `Utility/CombinedAddressableCollection.cs` | 60 | Utility | class CombinedAddressableCollection |  | Addressables |
| `Utility/CommandFunctions.cs` | 9 | Utility | enum CommandFunctions |  |  |
| `Utility/ConeDescriptor.cs` | 42 | Utility | struct ConeDescriptor |  |  |
| `Utility/ConsoleCommands.cs` | 75 | Utility | class ConsoleCommands |  | Command |
| `Utility/CoroutineRunner.cs` | 80 | Utility | class CoroutineRunner : MonoBehaviour | OnDestroy | prefab |
| `Utility/CoroutineUtils.cs` | 74 | Utility | class CoroutineUtils |  |  |
| `Utility/CrewStatus.cs` | 14 | Utility | enum CrewStatus |  |  |
| `Utility/CrewStatusExtensions.cs` | 63 | Utility | class CrewStatusExtensions |  |  |
| `Utility/CrossSceneErrorData.cs` | 7 | Utility | class CrossSceneErrorData : CrossScenePipe.CrossSceneData |  |  |
| `Utility/CrossScenePipe.cs` | 58 | Utility | class CrossScenePipe : MonoBehaviour<br>class CrossSceneData |  | prefab |
| `Utility/CustomWaitForSeconds.cs` | 35 | Utility | class CustomWaitForSeconds : CustomYieldInstruction, IXmlDocSerializable |  | XML |
| `Utility/CustomWaitForSecondsRealtime.cs` | 33 | Utility | class CustomWaitForSecondsRealtime : CustomYieldInstruction, IXmlDocSerializable |  | XML |
| `Utility/DebugStepsRecorder.cs` | 53 | Utility | class DebugStepsRecorder<br>class Step<br>class BoundsStep : Step |  |  |
| `Utility/DecoyMode.cs` | 10 | Utility | enum DecoyMode |  |  |
| `Utility/DefensiveDoctrineSettings.cs` | 170 | Utility | struct DefensiveDoctrineSettings<br>enum ThreatMode<br>enum SizeOrdering |  |  |
| `Utility/DefensiveMissileMode.cs` | 9 | Utility | enum DefensiveMissileMode |  |  |
| `Utility/DevPhase.cs` | 11 | Utility | enum DevPhase |  |  |
| `Utility/Direction.cs` | 12 | Utility | enum Direction |  |  |
| `Utility/DirectionalCulling.cs` | 40 | Utility | class DirectionalCulling : MonoBehaviour | Awake, Update |  |
| `Utility/DirectionExtensions.cs` | 105 | Utility | class DirectionExtensions |  |  |
| `Utility/DynamicVisible.cs` | 68 | Utility | class DynamicVisible : MonoBehaviour | Awake, OnEnable |  |
| `Utility/DynamicVisibleHierarchy.cs` | 10 | Utility | class DynamicVisibleHierarchy : DynamicVisible |  | prefab |
| `Utility/DynamicVisibleLODGroup.cs` | 31 | Utility | class DynamicVisibleLODGroup : DynamicVisible |  | prefab |
| `Utility/DynamicVisibleMesh.cs` | 32 | Utility | class DynamicVisibleMesh : DynamicVisible |  |  |
| `Utility/DynamicVisibleParticles.cs` | 133 | Utility | class DynamicVisibleParticles : DynamicVisible | Awake, OnDisable | prefab |
| `Utility/EmissionStatus.cs` | 13 | Utility | enum EmissionStatus |  |  |
| `Utility/FalloffType.cs` | 10 | Utility | enum FalloffType |  |  |
| `Utility/FastNameplateBaker.cs` | 376 | Utility | class FastNameplateBaker : SingletonMonobehaviour<FastNameplateBaker><br>struct BakedOutput<br>enum TargetType<br>struct BakeTarget<br>class SegmentPlates<br>class Row | Awake, OnDestroy | prefab |
| `Utility/FilePath.cs` | 99 | Utility | struct FilePath |  |  |
| `Utility/FiniteStateMachine.cs` | 56 | Utility | class FiniteStateMachine |  |  |
| `Utility/FiniteStateMachineCoroutine.cs` | 85 | Utility | class FiniteStateMachineCoroutine |  |  |
| `Utility/FlipBillboard.cs` | 19 | Utility | class FlipBillboard : MonoBehaviour | Update |  |
| `Utility/FolderContents.cs` | 43 | Utility | class FolderContents |  |  |
| `Utility/FormationStyle.cs` | 9 | Utility | enum FormationStyle |  |  |
| `Utility/FuzeType.cs` | 9 | Utility | enum FuzeType |  |  |
| `Utility/GameColors.cs` | 309 | Utility | class GameColors<br>enum ColorName<br>class Palette |  |  |
| `Utility/GameConstants.cs` | 57 | Utility | class GameConstants |  |  |
| `Utility/GameObjectExtensions.cs` | 99 | Utility | class GameObjectExtensions |  | NetworkServer, prefab |
| `Utility/GameOptionAttribute.cs` | 28 | Utility | class GameOptionAttribute : Attribute<br>enum Group |  |  |
| `Utility/GameOptionDropdownAttribute.cs` | 14 | Utility | class GameOptionDropdownAttribute : GameOptionAttribute |  |  |
| `Utility/GameOptionSliderAttribute.cs` | 20 | Utility | class GameOptionSliderAttribute : GameOptionAttribute |  |  |
| `Utility/GameOptionTickboxAttribute.cs` | 12 | Utility | class GameOptionTickboxAttribute : GameOptionAttribute |  |  |
| `Utility/GameSettings.cs` | 426 | Utility | class GameSettings<br>struct GameOptionCallbacks |  |  |
| `Utility/GameVersion.cs` | 29 | Utility | class GameVersion<br>enum VersionTime |  |  |
| `Utility/GroupProgress.cs` | 64 | Utility | class GroupProgress<br>class Member : IProgress<float> |  |  |
| `Utility/GuidExtensions.cs` | 19 | Utility | class GuidExtensions |  |  |
| `Utility/HexCoordinate.cs` | 194 | Utility | struct HexCoordinate : IXmlDocSerializable |  | XML |
| `Utility/HistoryEvent.cs` | 20 | Utility | class HistoryEvent |  |  |
| `Utility/HullBadge.cs` | 222 | Utility | class HullBadge : IBlob, IXmlSerializable |  | XML |
| `Utility/IAssetAddress.cs` | 9 | Utility | interface IAssetAddress |  |  |
| `Utility/IBlob.cs` | 9 | Utility | interface IBlob |  |  |
| `Utility/IBulkSaveComponent.cs` | 13 | Utility | interface IBulkSaveComponent |  | save-state, XML |
| `Utility/IBulkSaveObject.cs` | 15 | Utility | interface IBulkSaveObject |  | save-state, XML |
| `Utility/IClearable.cs` | 7 | Utility | interface IClearable |  |  |
| `Utility/IClusterablePoint.cs` | 9 | Utility | interface IClusterablePoint |  |  |
| `Utility/IcoSphere.cs` | 111 | Utility | class IcoSphere<br>struct TriangleIndices |  |  |
| `Utility/ICountdownEvent.cs` | 13 | Utility | interface ICountdownEvent |  |  |
| `Utility/IFactionLocked.cs` | 11 | Utility | interface IFactionLocked |  |  |
| `Utility/IFileSource.cs` | 28 | Utility | interface IFileSource |  |  |
| `Utility/IFileSummary.cs` | 19 | Utility | interface IFileSummary |  |  |
| `Utility/IFormationUnit.cs` | 22 | Utility | interface IFormationUnit |  |  |
| `Utility/IHandleTransformable.cs` | 13 | Utility | interface IHandleTransformable |  |  |
| `Utility/IHorizontalPlane.cs` | 7 | Utility | interface IHorizontalPlane |  |  |
| `Utility/ILODGroupBuilderInput.cs` | 7 | Utility | interface ILODGroupBuilderInput |  |  |
| `Utility/IModifiableStats.cs` | 12 | Utility | interface IModifiableStats |  |  |
| `Utility/IntegerSumProgress.cs` | 27 | Utility | class IntegerSumProgress |  |  |
| `Utility/IObjectPool.cs` | 7 | Utility | interface IObjectPool |  |  |
| `Utility/IPIDHost.cs` | 7 | Utility | interface IPIDHost |  |  |
| `Utility/IPointCostItem.cs` | 11 | Utility | interface IPointCostItem : ISaveKeyed |  |  |
| `Utility/IPreloadableAsset.cs` | 11 | Utility | interface IPreloadableAsset |  |  |
| `Utility/IQuantityHolder.cs` | 17 | Utility | interface IQuantityHolder |  |  |
| `Utility/IQuantityMonitor.cs` | 19 | Utility | interface IQuantityMonitor |  |  |
| `Utility/IRandomPointVolume.cs` | 11 | Utility | interface IRandomPointVolume |  |  |
| `Utility/IReadOnlyAddressableResourceCollection.cs` | 23 | Utility | interface IReadOnlyAddressableResourceCollection : IEnumerable<AssetAddress<T>>, IEnumerable |  |  |
| `Utility/ISavedGameFile.cs` | 9 | Utility | interface ISavedGameFile : IFileSummary |  |  |
| `Utility/ISettable.cs` | 6 | Utility | interface ISettable |  |  |
| `Utility/IStatusProgress.cs` | 9 | Utility | interface IStatusProgress : IProgress<T> |  |  |
| `Utility/IUndoHistory.cs` | 21 | Utility | interface IUndoHistory |  |  |
| `Utility/IVolume.cs` | 15 | Utility | interface IVolume |  |  |
| `Utility/IXmlDocSerializable.cs` | 11 | Utility | interface IXmlDocSerializable |  | XML |
| `Utility/IXmlSaveState.cs` | 19 | Utility | interface IXmlSaveState<br>interface IXmlSaveState |  | save-state, XML |
| `Utility/LabeledArrayAttribute.cs` | 8 | Utility | class LabeledArrayAttribute : PropertyAttribute |  |  |
| `Utility/LayerAttribute.cs` | 8 | Utility | class LayerAttribute : PropertyAttribute |  |  |
| `Utility/LineDrawingExtensions.cs` | 33 | Utility | class LineDrawingExtensions |  |  |
| `Utility/LineRendererShapes.cs` | 25 | Utility | class LineRendererShapes |  |  |
| `Utility/LineStyle.cs` | 39 | Utility | struct LineStyle |  |  |
| `Utility/List2D.cs` | 90 | Utility | class List2D |  |  |
| `Utility/ListExtensions.cs` | 855 | Utility | class ListExtensions<br>class CircularEnumerator : IEnumerator<T>, IEnumerator, IDisposable<br>class CircularIndexedEnumerator : IEnumerator<T>, IEnumerator, IDisposable |  | pooling, prefab |
| `Utility/ListOperationsNoAlloc.cs` | 264 | Utility | class ListOperationsNoAlloc |  |  |
| `Utility/LocalFileSource.cs` | 59 | Utility | class LocalFileSource : IFileSource |  |  |
| `Utility/Localization/Locale.cs` | 45 | Utility.Localization | class Locale |  | XML |
| `Utility/Localization/LocalizationCore.cs` | 261 | Utility.Localization | class LocalizationCore<br>struct LocaleOption |  |  |
| `Utility/Localization/LocalizationTable.cs` | 88 | Utility.Localization | class LocalizationTable |  | JSON |
| `Utility/Localization/LocalizedNameAttribute.cs` | 17 | Utility.Localization | class LocalizedNameAttribute : Attribute |  |  |
| `Utility/Localization/LocalizedStringTable.cs` | 56 | Utility.Localization | class LocalizedStringTable : ScriptableObject |  | JSON |
| `Utility/LODGroupCamoRandomizer.cs` | 23 | Utility | class LODGroupCamoRandomizer : MonoBehaviour | Start |  |
| `Utility/LODGroupSharedMaterial.cs` | 191 | Utility | class LODGroupSharedMaterial : MonoBehaviour<br>struct SharedMeshTableKey | Awake, OnDestroy | prefab |
| `Utility/Logging.cs` | 44 | Utility | class Logging<br>class CustomLogHandler : ILogHandler |  |  |
| `Utility/MatchColorblindModeShape.cs` | 37 | Utility | class MatchColorblindModeShape : MonoBehaviour | Awake |  |
| `Utility/MathHelpers.cs` | 478 | Utility | class MathHelpers |  |  |
| `Utility/MemoryFileSource.cs` | 50 | Utility | class MemoryFileSource : IFileSource |  | XML |
| `Utility/MovementSpeed.cs` | 11 | Utility | enum MovementSpeed |  |  |
| `Utility/MovementSpeedExtensions.cs` | 7 | Utility | class MovementSpeedExtensions |  |  |
| `Utility/MovementStatus.cs` | 13 | Utility | enum MovementStatus |  |  |
| `Utility/MovementStyle.cs` | 8 | Utility | enum MovementStyle |  |  |
| `Utility/MultiTaskProgress.cs` | 54 | Utility | class MultiTaskProgress : IStatusProgress<float>, IProgress<float> |  |  |
| `Utility/NameplateBaker.cs` | 218 | Utility | class NameplateBaker : MonoBehaviour<br>struct BakedTextureSet<br>enum Stage<br>class BakingTask | Awake, OnDestroy, Update | prefab |
| `Utility/NetworkObjectPooler.cs` | 147 | Utility | class NetworkObjectPooler : NetworkBehaviour<br>class NetworkPool : IObjectPool<NetworkPoolable> | Awake, OnDestroy | NetworkBehaviour, NetworkServer, pooling, prefab |
| `Utility/NetworkPoolable.cs` | 447 | Utility | class NetworkPoolable : NetworkBehaviour | OnRepooled, OnUnpooled | NetworkBehaviour, ClientRpc, NetworkServer, NetworkClient, pooling, prefab |
| `Utility/NoAllocEnumerable.cs` | 362 | Utility | struct NoAllocEnumerable<br>struct Enumerator |  |  |
| `Utility/ObjectExtensions.cs` | 141 | Utility | class ObjectExtensions |  |  |
| `Utility/ObjectPooler.cs` | 201 | Utility | class ObjectPooler : MonoBehaviour<br>class Pool : IObjectPool<Poolable> | Awake, OnDestroy | pooling, prefab |
| `Utility/Octant.cs` | 15 | Utility | enum Octant |  |  |
| `Utility/OctantExtensions.cs` | 49 | Utility | class OctantExtensions |  |  |
| `Utility/OrderedSet.cs` | 273 | Utility | class OrderedSet : ICollection<T>, IEnumerable<T>, IEnumerable, ISet<T> |  |  |
| `Utility/OverridableList.cs` | 46 | Utility | class OverridableList : IReadOnlyList<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T> |  |  |
| `Utility/OverridableObservableCollection.cs` | 45 | Utility | class OverridableObservableCollection : OverridableList<T>, INotifyCollectionChanged |  |  |
| `Utility/PDTurretMode.cs` | 9 | Utility | enum PDTurretMode |  |  |
| `Utility/PhysicsHelpers.cs` | 31 | Utility | class PhysicsHelpers |  |  |
| `Utility/PID.cs` | 70 | Utility | class PID |  |  |
| `Utility/PIDValues.cs` | 18 | Utility | struct PIDValues |  |  |
| `Utility/PlayAnimationIgnoreTimescale.cs` | 27 | Utility | class PlayAnimationIgnoreTimescale : MonoBehaviour | OnEnable |  |
| `Utility/PlayerPreferencesWrapper.cs` | 152 | Utility | class PlayerPreferencesWrapper |  |  |
| `Utility/PlayerProfile.cs` | 162 | Utility | class PlayerProfile |  |  |
| `Utility/PointClusterer.cs` | 145 | Utility | class PointClusterer |  |  |
| `Utility/PointDefenseCoop.cs` | 9 | Utility | enum PointDefenseCoop |  |  |
| `Utility/PointDefenseSettings.cs` | 21 | Utility | struct PointDefenseSettings |  |  |
| `Utility/PointDefenseZone.cs` | 8 | Utility | enum PointDefenseZone |  |  |
| `Utility/Poolable.cs` | 87 | Utility | class Poolable : MonoBehaviour | OnEnable, OnRepooled, OnUnpooled | pooling, prefab |
| `Utility/PoolableImmediateModeShapeDrawer.cs` | 40 | Utility | class PoolableImmediateModeShapeDrawer : Poolable | OnDisable, OnEnable | pooling, prefab |
| `Utility/PreloadableAddressableAsset.cs` | 137 | Utility | class PreloadableAddressableAsset : IXmlDocSerializable, IModDependencySource |  | XML, Addressables |
| `Utility/PriorityLinkedList.cs` | 31 | Utility | class PriorityLinkedList<br>class Node |  |  |
| `Utility/ProceduralLODGroupBuilder.cs` | 37 | Utility | class ProceduralLODGroupBuilder |  |  |
| `Utility/ProceduralLODGroupInput.cs` | 11 | Utility | struct ProceduralLODGroupInput |  |  |
| `Utility/PromiseUtilities.cs` | 129 | Utility | class PromiseUtilities |  |  |
| `Utility/QuantityChanged.cs` | 4 | Utility |  |  |  |
| `Utility/QuantityHolderPool.cs` | 54 | Utility | class QuantityHolderPool : IQuantityHolder |  |  |
| `Utility/QuantityMonitorPool.cs` | 82 | Utility | class QuantityMonitorPool : IQuantityMonitor |  |  |
| `Utility/QuantityStatus.cs` | 10 | Utility | enum QuantityStatus |  |  |
| `Utility/QuantityStatusExtensions.cs` | 33 | Utility | class QuantityStatusExtensions |  |  |
| `Utility/RandomDistribution.cs` | 202 | Utility | class RandomDistribution |  |  |
| `Utility/RankStructure.cs` | 85 | Utility | class RankStructure : ScriptableObject<br>struct Rank |  |  |
| `Utility/ReferenceCounted.cs` | 35 | Utility | class ReferenceCounted |  |  |
| `Utility/ReferenceEqualityComparer.cs` | 17 | Utility | class ReferenceEqualityComparer : EqualityComparer<object> |  |  |
| `Utility/ReflectionHelpers.cs` | 293 | Utility | class ReflectionHelpers |  | prefab |
| `Utility/ResizeHandle.cs` | 48 | Utility | class ResizeHandle : MonoBehaviour, IDragHandler, IEventSystemHandler, IBeginDragHandler |  |  |
| `Utility/SaveableCoroutine.cs` | 127 | Utility | class SaveableCoroutine : IXmlSaveState, IEnumerator<br>class CoroutineParamAttribute : Attribute |  | save-state, XML |
| `Utility/SavedAnimatorState.cs` | 104 | Utility | class SavedAnimatorState : XmlSaveStateHelperCustom<Animator><br>class SavedLayer |  | XML |
| `Utility/SavedCoroutine.cs` | 47 | Utility | class SavedCoroutine : XmlSaveStateHelper<SaveableCoroutine> |  | XML |
| `Utility/SaveFileCollection.cs` | 141 | Utility | class SaveFileCollection |  |  |
| `Utility/SaveFileObject.cs` | 416 | Utility | class SaveFileObject : MonoBehaviour, IXmlSaveState<uint><br>class BulkObjectSaver : XmlSaveStateHelperWithID<SaveFileObject, uint><br>class BulkComponentCollection : IXmlSaveState<br>class BulkComponentSaver : XmlSaveStateHelper<BulkComponentCollection> | OnDestroy, Start | save-state, XML, prefab |
| `Utility/ScalingValue.cs` | 29 | Utility | struct ScalingValue |  |  |
| `Utility/Sector.cs` | 18 | Utility | enum Sector |  |  |
| `Utility/SectorExtensions.cs` | 78 | Utility | class SectorExtensions |  |  |
| `Utility/SetAudioSourceVelocityUpdate.cs` | 19 | Utility | class SetAudioSourceVelocityUpdate : MonoBehaviour | Awake |  |
| `Utility/ShapesGizmos.cs` | 423 | Utility | class ShapesGizmos |  |  |
| `Utility/ShortGuid.cs` | 190 | Utility | struct ShortGuid |  |  |
| `Utility/Sides.cs` | 17 | Utility | enum Sides |  |  |
| `Utility/SidesExtensions.cs` | 20 | Utility | class SidesExtensions |  |  |
| `Utility/Sign.cs` | 8 | Utility | enum Sign |  |  |
| `Utility/SimpleMovingAverage.cs` | 56 | Utility | class SimpleMovingAverage |  |  |
| `Utility/SingletonMonobehaviour.cs` | 60 | Utility | class SingletonMonobehaviour : MonoBehaviour where TDerived : MonoBehaviour | Awake, OnDestroy | prefab |
| `Utility/Skybox.cs` | 26 | Utility | class Skybox : ScriptableObject |  |  |
| `Utility/SortingDirection.cs` | 9 | Utility | enum SortingDirection |  |  |
| `Utility/SpecialLayers.cs` | 78 | Utility | class SpecialLayers |  |  |
| `Utility/SplineInterpolators.cs` | 97 | Utility | class SplineInterpolators |  |  |
| `Utility/StandbyVisualEffect.cs` | 84 | Utility | struct StandbyVisualEffect<br>struct ColorProperty<br>struct GradientProperty<br>struct FloatProperty |  |  |
| `Utility/StartSetupHelper.cs` | 27 | Utility | class StartSetupHelper |  |  |
| `Utility/StatHelpers.cs` | 107 | Utility | class StatHelpers |  |  |
| `Utility/StringExtensions.cs` | 144 | Utility | class StringExtensions |  |  |
| `Utility/SyncBlob.cs` | 98 | Utility | class SyncBlob : SyncObject where TObject : class, IBlob, new() |  |  |
| `Utility/Synchronization.cs` | 97 | Utility | class Synchronization |  |  |
| `Utility/SyncOrderedSet.cs` | 126 | Utility | class SyncOrderedSet : SyncSet<T> |  |  |
| `Utility/Table.cs` | 98 | Utility | class Table |  |  |
| `Utility/TargetAspect.cs` | 10 | Utility | enum TargetAspect |  |  |
| `Utility/TargetingMode.cs` | 13 | Utility | enum TargetingMode |  |  |
| `Utility/TargetLeadInfo.cs` | 15 | Utility | struct TargetLeadInfo |  |  |
| `Utility/TargetMethod.cs` | 9 | Utility | enum TargetMethod |  |  |
| `Utility/TeamIdentifier.cs` | 9 | Utility | enum TeamIdentifier |  |  |
| `Utility/TeamIDExtensions.cs` | 63 | Utility | class TeamIDExtensions |  |  |
| `Utility/TextAssetFileSource.cs` | 46 | Utility | class TextAssetFileSource : IFileSource |  |  |
| `Utility/TextPlateNegative.cs` | 88 | Utility | class TextPlateNegative : MonoBehaviour |  | prefab |
| `Utility/TraversalLimits.cs` | 33 | Utility | struct TraversalLimits |  |  |
| `Utility/UndirectedGraphView.cs` | 109 | Utility | class UndirectedGraphView : IVertexAndEdgeListGraph<TVertex, SEdge<TVertex>>, IVertexListGraph<TVertex, SEdge<TVertex>>, IIncidenceGraph<TVertex, SEdge<TVertex>>, II... |  |  |
| `Utility/UndoHistory.cs` | 84 | Utility | class UndoHistory : IUndoHistory where TEvent : HistoryEvent |  |  |
| `Utility/UnityBoundsExtensions.cs` | 162 | Utility | class UnityBoundsExtensions |  |  |
| `Utility/UnityTextureExtensions.cs` | 223 | Utility | class UnityTextureExtensions |  |  |
| `Utility/UnityVectorExtensions.cs` | 504 | Utility | class UnityVectorExtensions |  |  |
| `Utility/UnmaskSide.cs` | 9 | Utility | enum UnmaskSide |  |  |
| `Utility/UnmaskVector.cs` | 9 | Utility | enum UnmaskVector |  |  |
| `Utility/UnorderedPair.cs` | 49 | Utility | struct UnorderedPair |  |  |
| `Utility/Vector3D.cs` | 216 | Utility | struct Vector3D |  |  |
| `Utility/Vector3SerializationSurrogate.cs` | 26 | Utility | class Vector3SerializationSurrogate : ISerializationSurrogate |  |  |
| `Utility/VisCellCoordinate.cs` | 92 | Utility | struct VisCellCoordinate |  |  |
| `Utility/WaitForAnd.cs` | 16 | Utility | class WaitForAnd : CustomYieldInstruction |  |  |
| `Utility/WaitForAsyncTask.cs` | 70 | Utility | class WaitForAsyncTask : CustomYieldInstruction<br>class WaitForAsyncTask : CustomYieldInstruction |  |  |
| `Utility/WaitForOr.cs` | 16 | Utility | class WaitForOr : CustomYieldInstruction |  |  |
| `Utility/WaitForPromise.cs` | 68 | Utility | class WaitForPromise : CustomYieldInstruction<br>class WaitForPromise : CustomYieldInstruction |  |  |
| `Utility/WaitForPromiseHelpers.cs` | 17 | Utility | class WaitForPromiseHelpers |  |  |
| `Utility/WaitForTransfer.cs` | 15 | Utility | class WaitForTransfer : WaitForPromise<LFTManager.TransferCompletion> |  |  |
| `Utility/WeaponRole.cs` | 13 | Utility | enum WeaponRole |  |  |
| `Utility/WeaponRoleExtensions.cs` | 66 | Utility | class WeaponRoleExtensions |  |  |
| `Utility/WeaponsControlStatus.cs` | 9 | Utility | enum WeaponsControlStatus |  |  |
| `Utility/WeaponType.cs` | 11 | Utility | enum WeaponType |  |  |
| `Utility/WeightClass.cs` | 10 | Utility | enum WeightClass |  |  |
| `Utility/WeightClassDirection.cs` | 9 | Utility | enum WeightClassDirection |  |  |
| `Utility/WeightClassExtensions.cs` | 18 | Utility | class WeightClassExtensions |  |  |
| `Utility/WorkshopFileSource.cs` | 50 | Utility | class WorkshopFileSource : IFileSource |  |  |
| `Utility/XmlDocElementAttribute.cs` | 15 | Utility | class XmlDocElementAttribute : Attribute |  |  |
| `Utility/XmlDocListAttribute.cs` | 23 | Utility | class XmlDocListAttribute : Attribute |  |  |
| `Utility/XmlDocSerializerExtensions.cs` | 809 | Utility | class XmlDocSerializerExtensions<br>class TypeSerializationInfo |  | XML |
| `Utility/XmlRecordPolymorphicTypeAttribute.cs` | 9 | Utility | class XmlRecordPolymorphicTypeAttribute : Attribute |  |  |
| `Utility/XmlSaveStateHelper.cs` | 76 | Utility | class XmlSaveStateHelper : IXmlDocSerializable where T : IXmlSaveState |  | save-state, XML |
| `Utility/XmlSaveStateHelperCustom.cs` | 71 | Utility | class XmlSaveStateHelperCustom : IXmlDocSerializable |  | XML |
| `Utility/XmlSaveStateHelperWithID.cs` | 95 | Utility | class XmlSaveStateHelperWithID : IXmlDocSerializable where T : IXmlSaveState<TIdentifier> |  | save-state, XML |

