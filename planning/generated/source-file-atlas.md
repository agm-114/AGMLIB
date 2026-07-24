# Source file atlas

Every C# source file is listed with the structural signals used to prioritize review. Counts are navigation aids, not proof of runtime behavior.

## Advanced

| Source | Lines | Types | Namespace | Serialized | Lifecycle | Patches | Reflection | Logs | TODO markers |
|---|---:|---|---|---:|---|---:|---:|---:|---:|
| `AGMLIB/Advanced/Hull/AutoColliderSampler.cs` | 68 | AutoColliderSampler : BaseColliderSampler |  | 0 | Awake, OnDestroy | 0 | 0 | 0 | 0 |
| `AGMLIB/Advanced/Hull/ComplexHull.cs` | 16 | ComplexHull : Hull |  | 0 |  | 0 | 1 | 0 | 0 |
| `AGMLIB/Advanced/Hull/PaintScheme.cs` | 326 | ShaderProperties<br>PaintScheme : MonoBehaviour<br>PaintableHullSegment : HullSegment |  | 0 | Awake, FixedUpdate | 0 | 14 | 15 | 0 |
| `AGMLIB/Advanced/Hull/PaintSchemeMaterialOverride.cs` | 55 | PaintSchemeMaterialOverride : MonoBehaviour |  | 0 | Start | 0 | 0 | 3 | 0 |
| `AGMLIB/Advanced/Hull/PaintSchemeMaterialTarget.cs` | 130 | PaintSchemeMaterialTarget : MonoBehaviour |  | 0 | Awake, LateUpdate | 0 | 0 | 1 | 0 |
| `AGMLIB/Advanced/Legacy/LegacyMissile/AdvancedLoiteringMissile.cs` | 227 | AdvancedLoiteringMissile : LoiteringMissile |  | 3 |  | 0 | 2 | 0 | 0 |
| `AGMLIB/Advanced/Legacy/LegacyMissile/AdvancedShellWarhead.cs` | 83 | AdvancedShellWarhead : MissileWarhead |  | 9 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Advanced/Legacy/LegacyMissile/CompoundWarhead.cs` | 57 | CompoundWarhead : MissileWarhead |  | 1 |  | 0 | 0 | 1 | 0 |
| `AGMLIB/Advanced/Legacy/LegacyMissile/ModularShell.cs` | 208 | ModularShell : ShellMunition |  | 11 |  | 0 | 0 | 9 | 0 |
| `AGMLIB/Advanced/Legacy/LegacyMissile/SimpleMagazine.cs` | 65 | SimpleMagazine : MonoBehaviour, IMagazine |  | 3 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Advanced/Legacy/LegacyMissile/SimpleShellWarhead.cs` | 35 | SimpleShellWarhead : MissileWarhead |  | 1 |  | 0 | 0 | 1 | 0 |
| `AGMLIB/Advanced/Legacy/Physics/CustomCollider.cs` | 35 | CustomCollider : MonoBehaviour |  | 0 | FixedUpdate | 0 | 0 | 3 | 0 |
| `AGMLIB/Advanced/Legacy/Physics/Expander.cs` | 16 | Expander : MonoBehaviour |  | 0 | Start, Update | 0 | 0 | 0 | 0 |
| `AGMLIB/Advanced/Legacy/Physics/MeleeWeapon.cs` | 79 | MeleeWeapon : MonoBehaviour |  | 0 | FixedUpdate, Start | 0 | 0 | 5 | 0 |
| `AGMLIB/Advanced/Legacy/Physics/Rope.cs` | 153 | Rope : MonoBehaviour<br>RopeSegment |  | 0 | FixedUpdate, Start, Update | 0 | 0 | 0 | 0 |
| `AGMLIB/Advanced/Legacy/Physics/SimpleSlider.cs` | 22 | SimpleSlider : MonoBehaviour |  | 0 | Start, Update | 0 | 0 | 0 | 0 |
| `AGMLIB/Advanced/Legacy/Physics/StructureLinker.cs` | 36 | StructureLinker : MonoBehaviour |  | 0 | Start, Update | 0 | 0 | 0 | 0 |
| `AGMLIB/Advanced/Legacy/Refactored/FixedDiscreteMissileMagWeaponComponent.cs` | 16 | FixedDiscreteMissileMagWeaponComponent : FixedDiscreteMagWeaponComponent //, IMissileLauncher |  | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Advanced/Legacy/Refactored/GunLauncher.cs` | 58 | GunLauncher : AmmoCompatiblity<br>HullComponentAwake<br>HullComponentConsumeResourcesDead |  | 0 |  | 0 | 7 | 1 | 0 |
| `AGMLIB/Advanced/Legacy/Refactored/HoldingTargets.cs` | 11 | HoldingTargets : ShipState |  | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Advanced/Legacy/Refactored/Loose Code/Clipboard.cs` | 35 |  |  | 0 | Start | 0 | 0 | 0 | 0 |
| `AGMLIB/Advanced/Legacy/Refactored/Loose Code/Patches.cs` | 59 | DynamicVisibleParticlesUpdatePlaying<br>IDynamicCollectablePart<br>ShipControllerInitialize<br>HullGetAllSubPartsInternal |  | 0 |  | 1 | 0 | 5 | 0 |
| `AGMLIB/Advanced/Legacy/Refactored/MissileMag.cs` | 265 | MagazineCellMissileMagazine : BaseCellMissileMagazine<br>HybridCellMissileMagazine : StandardCellMissileMagazine |  | 0 |  | 0 | 0 | 4 | 0 |
| `AGMLIB/Advanced/Legacy/Refactored/PaintableHullSegment.cs` | 42 | PaintableHullSegment : HullSegment |  | 0 | Awake | 0 | 1 | 1 | 0 |
| `AGMLIB/Advanced/Legacy/Refactored/StrikeCraft.cs` | 45 | StrikeCraft : MonoBehaviour |  | 0 | FixedUpdate, Start, Update | 0 | 0 | 0 | 0 |
| `AGMLIB/Advanced/Legacy/Rework/AdvancedShipStatusDisplay.cs` | 6 | AdvancedShipStatusDisplay : ShipStatusDisplay |  | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Advanced/Legacy/Rework/EjectorArray.cs` | 119 | EjectorArray : MonoBehaviour |  | 0 | Start, Update | 0 | 1 | 5 | 0 |
| `AGMLIB/Advanced/Legacy/Rework/MissileSpawner.cs` | 52 | MissileSpawner : MonoBehaviour |  | 0 | Start | 0 | 0 | 9 | 0 |
| `AGMLIB/Advanced/Legacy/Rework/RotationMonitor.cs` | 15 | RotationMonitor : MonoBehaviour |  | 0 | Start, Update | 0 | 0 | 1 | 0 |
| `AGMLIB/Advanced/Legacy/Rework/ShipPDTarget.cs` | 56 | ShipPDTarget : DumbfireRocket, IDamageable |  | 0 |  | 0 | 0 | 3 | 0 |
| `AGMLIB/Advanced/StrikeCraft/Craft.cs` | 141 | CraftKinematics2<br>Craft : DumbfireRocket, ICraft |  | 4 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Advanced/StrikeCraft/FormationManager.cs` | 70 | FormationManager : ShipState |  | 1 | FixedUpdate | 0 | 4 | 1 | 0 |
| `AGMLIB/Advanced/StrikeCraft/MovementTarget.cs` | 84 | MovementTarget : MonoBehaviour |  | 9 | Awake, FixedUpdate, OnDestroy, Update | 0 | 1 | 0 | 0 |
| `AGMLIB/Advanced/StrikeCraft/SimpleCraft.cs` | 64 | CraftKinematics<br>ICraft<br>SimpleCraft : MonoBehaviour, ICraft |  | 5 | Awake, FixedUpdate | 0 | 0 | 0 | 0 |

## Common

| Source | Lines | Types | Namespace | Serialized | Lifecycle | Patches | Reflection | Logs | TODO markers |
|---|---:|---|---|---:|---|---:|---:|---:|---:|
| `AGMLIB/Common/BrokenDesignator.cs` | 41 | BrokenDesignator : MonoBehaviour |  | 0 | Awake | 0 | 2 | 1 | 0 |
| `AGMLIB/Common/ChildSocket.cs` | 55 | ChildSocket : MonoBehaviour |  | 0 | FixedUpdate, OnDestroy, Start | 0 | 2 | 0 | 0 |
| `AGMLIB/Common/Common.cs` | 189 | Common<br>CommonNonStatic<br>MonoComponent : MonoBehaviour |  | 0 |  | 0 | 5 | 5 | 0 |
| `AGMLIB/Common/Filters.cs` | 53 | Filters | Lib | 0 |  | 0 | 2 | 2 | 0 |
| `AGMLIB/Common/OwnedTypeReflection.cs` | 147 | OwnedTypeReflection<br>InitializeOwnedTypeStatValuesPatch |  | 0 |  | 1 | 7 | 3 | 0 |
| `AGMLIB/Common/RectTransformDebug.cs` | 111 | RectTransformDebug |  | 0 |  | 0 | 0 | 2 | 0 |
| `AGMLIB/Common/ShipState.cs` | 173 | ConditionalState<br>InternalShipState : NetworkBehaviour<br>ShipState : NetworkBehaviour |  | 0 | Awake | 0 | 1 | 0 | 0 |

## Craft

| Source | Lines | Types | Namespace | Serialized | Lifecycle | Patches | Reflection | Logs | TODO markers |
|---|---:|---|---|---:|---|---:|---:|---:|---:|
| `AGMLIB/Craft/AdvancedCraftMissileComponent.cs` | 107 | AdvancedCraftMissileComponent : CraftMissileComponent | Lib.Craft | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Craft/AutoCraftGunTurretWeapon.cs` | 146 | AutoCraftGunTurretWeapon : CraftGunTurretWeapon<br>AutoRuntimeCraftContinuousWeapon : RuntimeCraftContinuousWeapon<br>AutoRuntimeCraftDiscreteWeapon : RuntimeCraftDiscreteWeapon | Lib.Craft | 0 |  | 0 | 6 | 4 | 0 |

## Dynamic Systems

| Source | Lines | Types | Namespace | Serialized | Lifecycle | Patches | Reflection | Logs | TODO markers |
|---|---:|---|---|---:|---|---:|---:|---:|---:|
| `AGMLIB/Dynamic Systems/ActiveSettings.cs` | 185 | IMonoBehaviourFilter<br>ActiveSettings : ShipState, IMonoBehaviourFilter<Ship><br>ContinuousWeaponComponentReportFired<br>DiscreteWeaponComponentReportFired |  | 0 |  | 2 | 0 | 2 | 0 |
| `AGMLIB/Dynamic Systems/AOEModifer.cs` | 374 | OldAOEModifer : ActiveSettings, IJammingSource<br>ColliderComparer : IEqualityComparer<Collider><br>ActiveEWarEffectTargetGained<br>ActiveEWarEffectTargetLost |  | 1 | Start | 0 | 3 | 17 | 0 |
| `AGMLIB/Dynamic Systems/Area/AmmoConsumingFalloffEffect.cs` | 63 | AmmoConsumingFalloffEffect : FalloffEffect<Ship> | Lib.Dynamic_Systems.Area | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Dynamic Systems/Area/BasicEffect.cs` | 108 | BasicEffect : MonoBehaviour<br>GenericBasicEffect | AGMLIB.Dynamic_Systems.Area | 1 |  | 0 | 0 | 4 | 0 |
| `AGMLIB/Dynamic Systems/Area/Core/AreaEffect.cs` | 261 | AreaEffect : ActiveSettings<br>ColliderComparer : IEqualityComparer<Collider><br>ActiveEWarEffectTargetGained2<br>ActiveEWarEffectTargetLost2 |  | 0 | Start | 2 | 0 | 10 | 0 |
| `AGMLIB/Dynamic Systems/Area/Core/PulsedAreaEffect.cs` | 22 | PulsedAreaEffect : AreaEffect | AGMLIB.Dynamic_Systems.Area | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Dynamic Systems/Area/FalloffEffect.cs` | 16 | FalloffEffect | AGMLIB.Dynamic_Systems.Area | 0 | Start | 0 | 0 | 0 | 0 |
| `AGMLIB/Dynamic Systems/Area/JammingEffect.cs` | 116 | JammingEffect<br>ShipJammingEffect : JammingEffect<Ship><br>GenericSeekerEffect : JammingEffect<ModularMissile><br>MissileSoftkillEffect : GenericSeekerEffect<br>MissileJammingEffect : GenericSeekerEffect | AGMLIB.Dynamic_Systems.Area | 0 |  | 0 | 2 | 1 | 0 |
| `AGMLIB/Dynamic Systems/Area/RepairEffect.cs` | 166 | RestoreStatus : MonoBehaviour<br>RepairEffect : AmmoConsumingFalloffEffect | AGMLIB.Dynamic_Systems.Area | 0 | FixedUpdate | 0 | 3 | 4 | 0 |
| `AGMLIB/Dynamic Systems/Area/ResupplyEffect.cs` | 226 | ResupplyEffect : AmmoConsumingFalloffEffect<br>StartState : MonoBehaviour<br>HullComponentLoadSaveData | AGMLIB.Dynamic_Systems.Area | 0 |  | 1 | 0 | 6 | 0 |
| `AGMLIB/Dynamic Systems/Area/ShipModiferEffect.cs` | 56 | ShipModiferEffect : FalloffEffect<Ship> | AGMLIB.Dynamic_Systems.Area | 0 |  | 0 | 0 | 6 | 0 |
| `AGMLIB/Dynamic Systems/BoardingModule.cs` | 86 | BoardingModule : MonoBehaviour<br>AssualtTeam : DamageControlTeam<br>AssultTeamPiece : MonoBehaviour<br>DamageControlBoardLinkShip |  | 6 | Update | 1 | 6 | 3 | 0 |
| `AGMLIB/Dynamic Systems/DynamicActiveSignature.cs` | 116 | DynamicActiveSignature : ActiveSettings<br>SignatureCheckOccluded<br>SignatureGetReturnPowerDensity<br>SignatureGetCrossSection |  | 0 |  | 3 | 2 | 9 | 0 |
| `AGMLIB/Dynamic Systems/DynamicAnimator.cs` | 63 | DynamicAnimator : ActiveSettings |  | 6 |  | 0 | 0 | 1 | 0 |
| `AGMLIB/Dynamic Systems/DynamicComponent.cs` | 29 | DynamicComponent : ActiveSettings |  | 0 |  | 0 | 2 | 0 | 0 |
| `AGMLIB/Dynamic Systems/DynamicDamage.cs` | 63 | DynamicDamage : ActiveSettings<br>DamageTarget |  | 0 |  | 0 | 0 | 3 | 0 |
| `AGMLIB/Dynamic Systems/DynamicGlow.cs` | 96 | DynamicGlow : MonoBehaviour<br>Mode |  | 0 | FixedUpdate, Start | 0 | 0 | 8 | 0 |
| `AGMLIB/Dynamic Systems/DynamicModifer.cs` | 117 | DynamicModifer : ActiveSettings<br>ModifierMode |  | 0 |  | 0 | 0 | 8 | 0 |
| `AGMLIB/Dynamic Systems/DynamicReduction.cs` | 302 | DynamicReductionCache : MonoBehaviour<br>DynamicReduction : ActiveSettings<br>RequiredResources : MonoBehaviour<br>ShipSpawnAndAllocateResources<br>ShipRunResourceTick<br>ShipEditorRecalcCrewAndResources<br>HullPartResourceConnectedConsumeResources<br>HullPartResourceConnectedGetResourceDemand<br>ResourcePoolCalculateDemandForEditor<br>ResourceItemSetResource |  | 0 |  | 7 | 7 | 16 | 0 |
| `AGMLIB/Dynamic Systems/DynamicReload.cs` | 34 | DynamicReload : MonoBehaviour<br>DiscreteWeaponComponentRunTimers | Lib.Dynamic_Systems | 0 |  | 1 | 6 | 3 | 0 |
| `AGMLIB/Dynamic Systems/DynamicStun.cs` | 238 | DynamicStun : ActiveSettings<br>KnockbackMode<br>DynamicWorkingCache : MonoBehaviour<br>TimedDynamicStun : DynamicStun<br>DynamicWorkingHelpers<br>ShipControllerWeaponsControl<br>ShipControllerHandleDrivesWorkingChanged<br>ShipControllerHandlePowerplantsWorkingChanged |  | 0 |  | 4 | 9 | 23 | 0 |
| `AGMLIB/Dynamic Systems/DynamicThrottleTimeModifer.cs` | 134 | DynamicThrottleTimeModifer : ActiveSettings, IModifierSource<br>TransitionType<br>AsymmetricDynamicThrottleTimeModifer : DynamicThrottleTimeModifer |  | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Dynamic Systems/DynamicTransform.cs` | 52 | DynamicTransform : ActiveSettings |  | 0 |  | 0 | 0 | 6 | 0 |
| `AGMLIB/Dynamic Systems/ResourceComponent.cs` | 67 | ResourceComponent : ActiveSettings<br>FillBehavior |  | 0 | Start | 0 | 0 | 4 | 0 |
| `AGMLIB/Dynamic Systems/ResourceModule.cs` | 73 | ResourceModule : ActiveSettings<br>GenerationScaling |  | 0 | Start | 0 | 0 | 7 | 0 |
| `AGMLIB/Dynamic Systems/SimpleDynamicModifer.cs` | 45 | SimpleDynamicModifer : ActiveSettings, IModifierSource |  | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Dynamic Systems/UI/DynamicButtonState.cs` | 86 | DynamicButtonState : ActiveSettings<br>ButtonLockState<br>ForcedButtonState |  | 0 | Start | 0 | 0 | 1 | 0 |
| `AGMLIB/Dynamic Systems/UI/ResourceBar.cs` | 124 | ResourceBar : MonoBehaviour |  | 0 | FixedUpdate, Start | 0 | 0 | 14 | 0 |
| `AGMLIB/Dynamic Systems/UI/ShipInfoButton.cs` | 394 | ButtonTask : WarshipOrderTask<br>ShipInfoButton : ShipState<br>ButtonData<br>ShipInfoBarMatchAllButtons<br>HumanSkirmishPlayerQueueOrder |  | 0 | Update | 2 | 11 | 8 | 0 |
| `AGMLIB/Dynamic Systems/UI/ShipSignatureDisplayReduction.cs` | 177 | ShipSignatureDisplayReduction : MonoBehaviour<br>ShipControllerInitializeSignatureDisplayReduction<br>ShipInfoBarHandleSignatureSizeChangedReduction<br>ShipInfoBarUpdateSignatureDisplayReduction |  | 0 | Awake, FixedUpdate | 3 | 2 | 0 | 0 |
| `AGMLIB/Dynamic Systems/UI/ShipStatusPowerBar.cs` | 711 | ShipStatusPowerBar : MonoBehaviour<br>DisplaySurface<br>Binding : MonoBehaviour<br>DebugShipStatusPowerIconBar : ShipStatusPowerBar<br>ShipStatusPowerBarUpdater : MonoBehaviour |  | 0 | Awake, OnDestroy, Update | 0 | 1 | 0 | 0 |
| `AGMLIB/Dynamic Systems/UI/ShipStatusPowerBarBinding.cs` | 174 | ShipStatusPowerBarBinding : ShipStatusPowerBar.Binding |  | 0 | OnDestroy, OnDisable, OnEnable | 0 | 2 | 0 | 0 |
| `AGMLIB/Dynamic Systems/UI/ShipStatusPowerBarPatches.cs` | 37 | ShipStatusIconGroupSetShipPowerStatusBar<br>ShipStatusDisplayLinkShipPowerStatusBoard<br>ShipControllerInitializePowerStatusBar |  | 0 |  | 3 | 1 | 0 | 0 |
| `AGMLIB/Dynamic Systems/UI/ShipStatusPowerStatusBoardBinding.cs` | 163 | ShipStatusPowerStatusBoardBinding : ShipStatusPowerBar.Binding |  | 0 | OnDestroy, OnDisable, OnEnable | 0 | 0 | 0 | 0 |

## Editor

| Source | Lines | Types | Namespace | Serialized | Lifecycle | Patches | Reflection | Logs | TODO markers |
|---|---:|---|---|---:|---|---:|---:|---:|---:|
| `AGMLIB/Editor/Debugger.cs` | 58 | KeyDebugger : ShipState<br>Described |  | 0 | Start, Update | 0 | 1 | 4 | 0 |
| `AGMLIB/Editor/DefaultMissileTemplates.cs` | 392 | DefaultMissileSocketFillMode<br>DefaultMissileTemplate : MonoBehaviour<br>DefaultMissileTemplateLoader<br>ApplyDefaultMissileTemplateOnCreatePatch<br>AddDefaultMissileTemplatesOnFleetChangedPatch |  | 0 |  | 2 | 0 | 9 | 0 |
| `AGMLIB/Editor/Escort.cs` | 171 | Escort : MonoBehaviour<br>SerializedFleetEnforceShipLimit<br>FleetEditorFleetCompositionSubmodeController<br>FleetListPaneHandleShipAdded : MonoBehaviour<br>SkirmishGameManagerCoroutineSpawnPlayerFleet |  | 0 |  | 1 | 0 | 5 | 0 |
| `AGMLIB/Editor/ExperimentalStringFormatter.cs` | 74 | AdvancedTag<br>ExperimentalStringFormatter : StringFormatter |  | 2 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Editor/Lore/Lore.cs` | 251 | Lore : MonoBehaviour<br>ComponentPaletteCreateItemPatch<br>FleetStatsPaneUpdateFleetStats<br>PaletteItemSetCurrentCount<br>ModalListSelectDetailedSelectItemInternal<br>ComponentPaletteGetDetailTextPatch<br>HullListItemGetDetails |  | 8 |  | 6 | 3 | 7 | 0 |
| `AGMLIB/Editor/Lore/StringFormatter.cs` | 150 | BasicTag<br>CustomColor<br>StringFormatter : MonoBehaviour |  | 7 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Editor/Materials/AdvancedPaintScheme.cs` | 305 | AdvancedPaintScheme : MonoBehaviour<br>SegmentOverride<br>MaterialOverrides<br>IngredientDrawerUIE : PropertyDrawer |  | 0 | Start, Update | 0 | 1 | 10 | 0 |
| `AGMLIB/Editor/Materials/MaterialRandomizer.cs` | 17 | MaterialRandomizer : MonoBehaviour | AGMLIB.Editor | 0 | Awake | 0 | 0 | 1 | 0 |
| `AGMLIB/Editor/Points/Discount.cs` | 132 | SimpleDiscount : MonoBehaviour<br>Discount : SimpleDiscount<br>HullComponentGetPointCost<br>HullComponentGetBasePointCost<br>HullComponentGetFormattedSubtitle<br>FleetFleetValue |  | 0 |  | 5 | 2 | 14 | 0 |
| `AGMLIB/Editor/ShipEditorSocketUI.cs` | 230 | ShipEditorPaneSetShip<br>FleetCompositionSubmodeControllerSetHighlightedSocketPatch<br>ShipEditorPanePatch |  | 0 |  | 3 | 3 | 14 | 0 |
| `AGMLIB/Editor/SocketClipboard.cs` | 98 | SocketClipboardItemConstructorPatch<br>SocketClipboardItemApplyPatch<br>CopiedSocketChild<br>SocketClipboardChildren |  | 0 |  | 2 | 0 | 0 | 0 |
| `AGMLIB/Editor/SocketFilterCore.cs` | 537 | ICoreFilter<br>ISimpleFilter<br>BaseFilter : MonoBehaviour, ISimpleFilter<br>AmmoTagFilter : BaseFilter<br>IFilter : ICoreFilter<br>IFilterIndexed : IFilter<br>RenderShape<br>SimpleAndFilter : BaseFilter, ISimpleFilter<br>SimpleOwnerFilter : BaseFilter, ISimpleFilter<br>SimpleFilter : BaseFilter, ISimpleFilter<br>SocketHelpers<br>BasicSocketEditorUISettings : MonoBehaviour<br>SocketEditorUISettings : BasicSocketEditorUISettings<br>BaseSocketLookup : MonoBehaviour<br>KeyBasedFilterLookup : BaseSocketLookup<br>SocketEditorParentSettings : MonoBehaviour<br>SocketEditorChildSettings : MonoBehaviour<br>ComponentDependencies : MonoBehaviour<br>SocketGroupComponentChangeBinding : MonoBehaviour<br>SocketGroupDropdown : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler<br>SocketFilters : BasicSocketEditorUISettings, IFilter |  | 13 | OnDestroy | 0 | 2 | 14 | 0 |
| `AGMLIB/Editor/SocketPatches.cs` | 326 | SettingsMagazineLoadoutAddMagazine<br>LoadPatch<br>ShipRebuildAmmoFeeder<br>SocketItemSetComponent<br>HullSocketPatch |  | 0 |  | 5 | 7 | 26 | 0 |
| `AGMLIB/Editor/SocketRendering.cs` | 228 | SocketOutlineManagerSetSockets<br>SocketOutlineManagerDrawShapes |  | 0 |  | 1 | 0 | 6 | 0 |
| `AGMLIB/Editor/Tips/GenericComponentTip.cs` | 33 | GenericComponentTip : ShipDesignTip | Lib.Editor.Tips | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Editor/Yaml/Components/YamlHullComponent.cs` | 72 | YamlVector3Int<br>YamlStatModifier<br>YamlResourceModifier<br>YamlHullComponent : YamlHullPart | Lib.Editor.Yaml.Components | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Editor/Yaml/Components/YamlHullPart.cs` | 18 | YamlHullPart | Lib.Editor.Yaml.Components | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Editor/Yaml/YamlLoader.cs` | 168 | clicktest<br>YamlComp : MonoBehaviour<br>ModRecordLoadMod | Lib.Editor.Yaml | 0 | Awake | 2 | 44 | 12 | 0 |

## AGMLIB

| Source | Lines | Types | Namespace | Serialized | Lifecycle | Patches | Reflection | Logs | TODO markers |
|---|---:|---|---|---:|---|---:|---:|---:|---:|
| `AGMLIB/EntryPoint.cs` | 577 | EntryPoint : IModEntryPoint<br>HullComponentGetMissingResources : MonoBehaviour<br>BundleCache<br>QuickLoad<br>DependencyPatch |  | 0 |  | 1 | 4 | 43 | 0 |
| `AGMLIB/GlobalSuppressions.cs` | 25 |  |  | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/SetCopiedAssetId.cs` | 37 | SetCopiedAssetId : MonoBehaviour | Utility | 0 |  | 0 | 1 | 0 | 0 |

## FX

| Source | Lines | Types | Namespace | Serialized | Lifecycle | Patches | Reflection | Logs | TODO markers |
|---|---:|---|---|---:|---|---:|---:|---:|---:|
| `AGMLIB/FX/BeamEffectModule.cs` | 58 | BeamEffectModule : MonoBehaviour, IEffectModule<br>ShortDurationBeamEffect : BeamEffectModule |  | 0 | Start, Update | 0 | 2 | 3 | 0 |
| `AGMLIB/FX/DynamicFX.cs` | 79 | DynamicFX : ActiveSettings<br>StaticFX : MonoBehaviour |  | 0 | Start | 0 | 0 | 1 | 0 |
| `AGMLIB/FX/EffectSpawner.cs` | 89 | EffectSpawner : MonoBehaviour |  | 0 | Start | 0 | 0 | 5 | 0 |
| `AGMLIB/FX/LineBeamMuzzleEffects.cs` | 87 | ResizingLineBeamMuzzleEffects : LineBeamMuzzleEffects<br>CurvedResizingLineBeamMuzzleEffects : ResizingLineBeamMuzzleEffects |  | 0 |  | 0 | 0 | 1 | 0 |
| `AGMLIB/FX/Materials/ShaderMaterial.cs` | 86 | ShaderMaterial : MonoComponent<br>FixedShaderMaterial : ShaderMaterial<br>AuroraMaterial : FixedShaderMaterial<br>GrazerMaterial : FixedShaderMaterial<br>BeamMaterial : FixedShaderMaterial<br>SaveKeyShaderMaterial : ShaderMaterial |  | 0 |  | 0 | 0 | 2 | 0 |
| `AGMLIB/FX/MuzzleFX.cs` | 292 | IMuzzleEffect<br>MuzzleEffects : MonoBehaviour<br>BaseMuzzleEffects : MonoBehaviour, IMuzzleEffect<br>MuzzleSpawnedEffects : BaseMuzzleEffects<br>MuzzleFireHitEffects : BaseMuzzleEffects<br>MuzzleSoundEffects : BaseMuzzleEffects<br>MuzzleGlowerEffects : BaseMuzzleEffects<br>MuzzleSoundSource : MuzzleSoundEffects<br>LongPulseRaycastMuzzle : SinglePulseRaycastMuzzle | Lib.Generic_Gameplay.Discrete | 13 | Awake, FixedUpdate | 0 | 0 | 10 | 0 |
| `AGMLIB/FX/PulsedLineBeamMuzzleEffects.cs` | 9 | PulsedLineBeamMuzzleEffects : ResizingLineBeamMuzzleEffects |  | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/FX/PulsedLocalEffect.cs` | 27 | PulsedLocalEffect : MonoBehaviour | Lib.FX | 3 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/FX/PulsedThrusterPart.cs` | 25 | PulsedThrusterPart : ThrusterPart |  | 0 |  | 0 | 1 | 0 | 0 |
| `AGMLIB/FX/ShortDurationFollowingModularEffect.cs` | 21 | ShortDurationFollowingModularEffect : FollowingModularEffect | Lib.FX | 1 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/FX/SpatialSoundEffectModule.cs` | 11 | SpatialSoundEffectModule : MonoBehaviour, IEffectModule | Lib.FX | 1 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/FX/VFXThrusterEvent.cs` | 11 | VFXThrusterEvent : VFXOutputEventAbstractHandler |  | 0 |  | 0 | 0 | 0 | 0 |

## Generic Gameplay

| Source | Lines | Types | Namespace | Serialized | Lifecycle | Patches | Reflection | Logs | TODO markers |
|---|---:|---|---|---:|---|---:|---:|---:|---:|
| `AGMLIB/Generic Gameplay/AmmoFilter.cs` | 83 | AmmoCompatiblity : MonoBehaviour<br>AmmoFilter : AmmoCompatiblity<br>BaseCellLauncherComponentIsAmmoCompatible<br>BaseTubeLauncherComponentIsAmmoCompatible<br>WeaponComponentIsAmmoCompatible<br>BulkMagazineComponentRestrictionCheck |  | 1 |  | 4 | 0 | 4 | 0 |
| `AGMLIB/Generic Gameplay/AreaEffectRuntime.cs` | 23 | AreaEffectRuntime |  | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Generic Gameplay/Armor.cs` | 15 | Armor : MonoBehaviour<br>ShipControllerWouldArmorHitPenetrate |  | 0 |  | 1 | 0 | 1 | 0 |
| `AGMLIB/Generic Gameplay/Continuous/CasemateContinuousWeaponComponent.cs` | 161 | CasemateContinuousWeaponComponent : FixedContinuousWeaponComponent, IFixedWeapon<br>CasemateContinuousWeaponState : SavedHullComponentStates.StateElement | AGMLIB.Generic_Gameplay.Continuous | 4 |  | 0 | 1 | 0 | 0 |
| `AGMLIB/Generic Gameplay/Craft/ConfigurableBulkCraftHangarComponent.cs` | 14 | ConfigurableBulkCraftHangarComponent : BulkCraftHangarComponent, ICraftWorkSlotProvider |  | 1 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Generic Gameplay/Craft/CraftWeaponSettings.cs` | 6 | CraftWeaponSettings : MonoBehaviour |  | 1 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Generic Gameplay/Craft/FighterLimit.cs` | 264 | CraftLaunchLimit : MonoBehaviour<br>CraftLaunchLimitExtensions<br>ShipControllerInitializeCraftLaunchLimitPatch<br>CraftCarrierControllerTickCraftLaunchLimitPatch<br>SuspendedLaunchOrder<br>CraftLandingPadLaunchCraftLimitPatch<br>SpacecraftSetInStorageCraftLaunchLimitPatch |  | 0 |  | 4 | 6 | 6 | 0 |
| `AGMLIB/Generic Gameplay/Craft/LightweightCraftWorkSlotComponent.cs` | 88 | CraftWorkSlotComponentBase : MonoBehaviour, ICraftWorkSlotProvider<br>LightweightCraftWorkSlotComponent : CraftWorkSlotComponentBase<br>WorkSlot : ICraftWorkSlot |  | 5 | Awake | 0 | 0 | 0 | 0 |
| `AGMLIB/Generic Gameplay/CustomAimPoint.cs` | 101 | CustomAimPoint : ShipState<br>BaseTurretedLauncherComponentFixedUpdate<br>WeaponComponentGetAimPoint |  | 0 |  | 2 | 14 | 3 | 0 |
| `AGMLIB/Generic Gameplay/DecoyAmmoSettings.cs` | 589 | DecoyOrderResult<br>DecoyAmmoSettings : MonoBehaviour<br>DecoyFireRequest<br>ShotSubscription<br>WeaponGroupFirePositionSidecarPatch<br>ShipControllerHasAnyDecoysSidecarPatch<br>FireDecoyTaskSidecarPatch<br>FireDecoyTaskCompletionSidecarPatch | Lib.Generic_Gameplay | 0 | Awake, OnDestroy, Update | 4 | 0 | 1 | 0 |
| `AGMLIB/Generic Gameplay/Discrete/CustomLeadLogic.cs` | 40 | DiscreteWeaponComponentLeadLogic : MonoBehaviour<br>DiscreteWeaponComponentCalculateLead |  | 0 |  | 1 | 1 | 0 | 0 |
| `AGMLIB/Generic Gameplay/Discrete/DiscreteMag.cs` | 58 | InternalDiscreteMagazine : MonoBehaviour, IMagazineProvider<br>DiscreteMagazine : MonoBehaviour |  | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Generic Gameplay/Discrete/DiscreteWeaponEjectors.cs` | 143 | DiscreteWeaponEjectors : MonoBehaviour<br>FixedDiscreteWeaponComponentOnTarget |  | 1 | Start | 1 | 16 | 3 | 0 |
| `AGMLIB/Generic Gameplay/Discrete/FixedDiscreteLauncherComponent.cs` | 53 | BlockingWeapon : MonoBehaviour<br>FixedDiscreteLauncherComponent : FixedDiscreteWeaponComponent |  | 2 | FixedUpdate | 0 | 2 | 1 | 0 |
| `AGMLIB/Generic Gameplay/Discrete/FixedDiscreteMagWeaponComponent.cs` | 490 | FixedDiscreteMagWeaponComponent : FixedDiscreteWeaponComponent, IMagazineProvider, ISimpleStorageContainer, IStorageContainer, IConfigurableMagazineLoadout |  | 5 |  | 0 | 2 | 0 | 0 |
| `AGMLIB/Generic Gameplay/Discrete/MVPMagazine.cs` | 72 | MVPMagazine : MonoBehaviour, IMagazineProvider | Lib.Generic_Gameplay.Discrete | 0 |  | 0 | 0 | 1 | 0 |
| `AGMLIB/Generic Gameplay/Discrete/TurretedDiscreteMagWeaponComponent.cs` | 494 | TurretedDiscreteMagWeaponComponent : TurretedDiscreteWeaponComponent, IMagazineProvider, ISimpleStorageContainer, IStorageContainer, IConfigurableMagazineLoadout |  | 5 |  | 0 | 2 | 0 | 0 |
| `AGMLIB/Generic Gameplay/Engines/AngledThruster.cs` | 244 | AngledThruster : HullPart, IDynamicCollectablePart | Lib.Generic_Gameplay.Engines | 9 | FixedUpdate, Update | 0 | 0 | 5 | 0 |
| `AGMLIB/Generic Gameplay/Engines/CustomBehaviorThrusterPart.cs` | 270 | CustomBehaviorThrusterPart : HullPart, IThruster<br>Throttle | Ships | 6 | FixedUpdate, Update | 0 | 0 | 0 | 0 |
| `AGMLIB/Generic Gameplay/Engines/CustomBehaviorThrusterPartConfig.cs` | 7 | CustomBehaviorThrusterPartConfig : MonoBehaviour |  | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Generic Gameplay/Engines/RuntimeThruster.cs` | 245 | RuntimeThruster : HullPart, IThruster<br>Throttle | Lib.Generic_Gameplay.Engines | 0 |  | 0 | 0 | 5 | 0 |
| `AGMLIB/Generic Gameplay/Ewar/AdvancedActiveFireControlSensor.cs` | 53 | ActiveFireControlSensorOptions : MonoBehaviour<br>ActiveFireControlSensorCanSeeSignature | Lib.Generic_Gameplay.Ewar | 0 |  | 1 | 2 | 0 | 0 |
| `AGMLIB/Generic Gameplay/Ewar/AdvancedRadar.cs` | 174 | AdvancedRadar : ShipState<br>AdvancedRadarHelpers<br>SensorTrackAddSensor<br>SensorTrackUpdate<br>BaseActiveSensorComponentAcquireContacts<br>SensorTrackUpdateMode<br>BaseActiveSensorComponentCanSeeSignature<br>BaseActiveSensorComponentAttemptToGain<br>BaseActiveSensorComponentAcquireWithDelta |  | 0 |  | 4 | 12 | 7 | 0 |
| `AGMLIB/Generic Gameplay/Ewar/DopplerRadar.cs` | 586 | DopplerConeAngleMode<br>DopplerNotchSettings : MonoBehaviour<br>IDopplerNotchSensor<br>DopplerNotchMath<br>DopplerInternalActiveSensorComponent : InternalActiveSensorComponent, IDopplerNotchSensor<br>DopplerActiveFireControlSensor : ActiveFireControlSensor, IDopplerNotchSensor<br>DopplerNotchContact<br>DopplerNotchContactRegistry<br>DopplerNotchTrackAddedPatch<br>DopplerNotchTrackRemovedPatch<br>DopplerNotchLockAddedPatch<br>DopplerNotchLockRemovedPatch<br>DopplerNotchOverlay : ImmediateModeShapeDrawer<br>DopplerNotchOverlaySetupPatch<br>DopplerNotchOverlayVisibilityPatch<br>DopplerTestingComponentFactory : ITestingComponentFactory |  | 2 |  | 6 | 1 | 0 | 0 |
| `AGMLIB/Generic Gameplay/Ewar/FixedEWarComponent.cs` | 112 | FixedEWarComponent : FixedContinuousWeaponComponent, IEWarWeapon, IWeapon, IHullComponent, ITuneableEWar<br>WeaponGroupFixedEWarNeedsFacing |  | 8 |  | 1 | 2 | 0 | 0 |
| `AGMLIB/Generic Gameplay/Ewar/IFFComponent.cs` | 89 | IFFComponent : MonoBehaviour |  | 0 | FixedUpdate, Start | 0 | 7 | 11 | 0 |
| `AGMLIB/Generic Gameplay/Ewar/MultiActiveFireControlSensor.cs` | 20 | MultiSensor : ActiveFireControlSensor<br>MultiActiveFireControlSensor : ActiveFireControlSensor | Lib.Generic_Gameplay.Ewar | 0 | Update | 0 | 0 | 0 | 0 |
| `AGMLIB/Generic Gameplay/Ewar/PassiveCommsSensorComponent.cs` | 890 | PassiveCommsDetectionMode<br>PassiveCommsSensorComponent : HullComponent, ISensorComponent, IDeltaSensor, ISensor, IJammable, IEWarTarget, IOwned, IHullComponent<br>PassiveCommsSensorTrackHelpers<br>PassiveCommsSensorContactIntelPrefix<br>PassiveCommsSensorNetworkedContactIntelPrefix<br>PassiveCommsSensorSigintToken<br>PassiveCommsSensorLobContactIntel<br>PassiveCommsSensorNetworkedContactSymbol<br>PassiveCommsSensorOverrideColor<br>PassiveCommsSensorNetworkedOverrideColor |  | 10 | OnDisable, OnEnable | 7 | 10 | 1 | 0 |
| `AGMLIB/Generic Gameplay/Ewar/TacticalModule.cs` | 43 | HasOffensiveAbilityTweak<br>ContestTweak<br>TacticalModule : ShipState |  | 3 | Update | 2 | 0 | 1 | 0 |
| `AGMLIB/Generic Gameplay/Ewar/TrackLogic.cs` | 91 | BaseTrackLogic : MonoBehaviour<br>ListTrackLogic : BaseTrackLogic<br>RatioTrackLogic : BaseTrackLogic<br>FirecontrolTrackLogic : BaseTrackLogic | Lib.Generic_Gameplay.Ewar | 0 | Awake | 0 | 0 | 0 | 0 |
| `AGMLIB/Generic Gameplay/Ewar/TurretedInternalActiveSensorComponent - Copy.cs` | 24 | TurretedInternalActiveSensorComponent : BaseActiveSensorComponent |  | 1 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Generic Gameplay/Ewar/TurretedInternalActiveSensorComponent.cs` | 43 | InternalSectorActiveSensorComponent : BaseActiveSensorComponent |  | 1 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Generic Gameplay/GenericWeaponPatches.cs` | 40 | DiscreteWeaponComponentOnTarget<br>WeaponComponentGetNextMuzzle<br>WeaponComponentFixedUpdate |  | 0 |  | 3 | 4 | 5 | 0 |
| `AGMLIB/Generic Gameplay/Missiles/FixedCellLauncherComponent.cs` | 66 | FixedCellLauncherComponent : CellLauncherComponent, IFixedWeapon |  | 0 |  | 0 | 1 | 1 | 0 |
| `AGMLIB/Generic Gameplay/Missiles/FixedTubeLauncherComponent.cs` | 53 | FixedTubeLauncherComponent : TubeLauncherComponent, IFixedWeapon |  | 0 |  | 0 | 3 | 2 | 0 |
| `AGMLIB/Generic Gameplay/Missiles/KinematicMissileEjector.cs` | 362 | KinematicLauncher : ActiveSettings<br>IMissileFixedUpdate<br>MissileLaunchKinematics : LaunchKinematics<br>LaunchKinematics : MonoBehaviour, IMissileFixedUpdate<br>ClampMode<br>KinematicMissile : MonoBehaviour, IMissileFixedUpdate<br>MissileEjectorFireEffect<br>MissileEjectorMissileInstantiated<br>MissileFixedUpate<br>MissileThrust<br>MissileEjectorLaunchMissile<br>MissileEjectorFire1<br>MissileEjectorFire2 |  | 0 | FixedUpdate, Start | 7 | 6 | 24 | 0 |
| `AGMLIB/Generic Gameplay/Missiles/MagazineLoader.cs` | 202 | MagazineLoader : MonoBehaviour<br>CellLauncherComponentNeedsExternalAmmoFeed<br>SettingsMagazineLoadoutSet<br>SettingsMagazineLoadoutUpdateQuantities<br>BaseCellLauncherComponentGetDesignWarnings<br>BaseHullAmmoTypeInUse<br>ShipLoadFromSave |  | 0 |  | 7 | 7 | 21 | 0 |
| `AGMLIB/Generic Gameplay/Modifer/CustomModiferScaling.cs` | 111 | CustomModiferScaling : MonoBehaviour<br>ScalingMode<br>StatValueCalculateModifierCustomScaling |  | 0 |  | 1 | 2 | 0 | 0 |
| `AGMLIB/Generic Gameplay/Muzzles/CustomPulseRaycastMuzzle.cs` | 63 | CustomPulseRaycastMuzzle : SinglePulseRaycastMuzzle, IDirectDamageMuzzle | Lib.Generic_Gameplay.Muzzles | 2 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Generic Gameplay/Muzzles/DelayedContinuousRaycastMuzzle.cs` | 206 | DelayedContinuousRaycastMuzzle : RaycastMuzzle, IDirectDamageMuzzle | Lib.Generic_Gameplay.Muzzles | 13 | FixedUpdate | 0 | 3 | 0 | 0 |
| `AGMLIB/Generic Gameplay/Muzzles/DelayedRezzingMuzzle.cs` | 267 | DelayedRezzingMuzzle : RezzingMuzzle |  | 15 | Start, Update | 0 | 0 | 1 | 0 |
| `AGMLIB/Generic Gameplay/Muzzles/GenericMuzzlePatches.cs` | 177 | MuzzleStopFireEffect<br>RaycastMuzzleStopFireEffect<br>MuzzleFireEffect<br>RaycastMuzzleFire<br>RaycastMuzzleFireEffect<br>RezzingMuzzleFireEffect<br>DelayedRezzingMuzzleFire<br>SinglePulseRaycastMuzzleFireEffect<br>RaycastMuzzleDoRaycast<br>BallisticRaycastMuzzleDamageableImpact<br>BallisticRaycastMuzzleGenericImpact<br>BallisticRaycastMuzzleFixedUpdate<br>RaycastBulletObject |  | 0 |  | 11 | 3 | 16 | 0 |
| `AGMLIB/Generic Gameplay/Muzzles/MultiBeam.cs` | 313 | MultiBeam : MultiTarget<br>MultiTarget : MonoComponent<br>PointDefenseControllerTaskDirectWeapon<br>TurretedContinuousWeaponComponentBearToTarget<br>FixedContinuousWeaponComponentBearToTarget<br>ContinuousWeaponComponentStopFiring<br>ContinuousWeaponComponentStartFiring |  | 0 |  | 5 | 10 | 8 | 0 |
| `AGMLIB/Generic Gameplay/Muzzles/MuzzleList.cs` | 198 | MuzzleList : MonoBehaviour<br>WeaponComponentPatch<br>DummyMuzzle : Muzzle<br>MuzzleState : MonoBehaviour<br>ContinuousWeaponComponentStartFiring : MonoBehaviour | AGMLIB.Generic_Gameplay.Discrete | 0 | Awake | 2 | 6 | 3 | 0 |
| `AGMLIB/Generic Gameplay/Muzzles/PulsedContinuousRaycastMuzzle.cs` | 77 | DelayedPulseRaycastMuzzle : RaycastMuzzle, IDirectDamageMuzzle | Lib.Generic_Gameplay.Muzzles | 3 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Generic Gameplay/ScalingAOEExplosionEffectModule.cs` | 307 | IWeightedEffect<br>ScalingAOEExplosionEffectModule : MonoBehaviour, IDamageDealer, IDamageCharacteristic, IEffectModule, ILocalImbued, IOwned, IWeightedEffect | Lib.Generic_Gameplay | 20 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Generic Gameplay/SimpleVisualEffectFactory.cs` | 212 | PrefabVisualEffectFactory : MonoBehaviour<br>BundleLocation<br>FieldName<br>ComponentName | AGMLIB.Generic_Gameplay | 0 | Awake | 0 | 10 | 3 | 0 |
| `AGMLIB/Generic Gameplay/TurretTesting.cs` | 203 | CustomTartTraversalLimits : CustomTraversalLimits<br>CustomTraversalLimits : MonoBehaviour<br>TurretControllerFaceTarget<br>TurretControllerTargetWithinLimits |  | 0 |  | 1 | 16 | 13 | 0 |
| `AGMLIB/Generic Gameplay/WeaponCheckOverrides.cs` | 294 | WeaponCheckOverrides : MonoBehaviour<br>FratricidalWeapon : WeaponCheckOverrides<br>WeaponCheckOverrideResolver<br>WeaponComponentAimCheckOverridesPatch<br>AimCheckState<br>ActiveJammingEffectCheckTargetValidityOverridesPatch<br>FollowingInstanceMuzzleFireWeaponCheckOverridesPatch<br>FollowingInstanceMuzzleStopFireWeaponCheckOverridesPatch |  | 7 |  | 4 | 0 | 0 | 0 |

## Munitions

| Source | Lines | Types | Namespace | Serialized | Lifecycle | Patches | Reflection | Logs | TODO markers |
|---|---:|---|---|---:|---|---:|---:|---:|---:|
| `AGMLIB/Munitions/Damagers/MultiRayConeDamagerSettings.cs` | 25 | MultiRayConeDamagerSettings : SingleRayDamagerSettings | Lib.Munitions.Damagers | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/Damagers/SingleRayDamagerSettings.cs` | 73 | RangeBasedDamageCharacteristic : IDamageCharacteristic<br>SingleRayDamagerSettings : ScriptableObject | Lib.Munitions.Damagers | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/Damagers/SingleSpherecastDamagerSettings.cs` | 25 | SingleSpherecastDamagerSettings : SingleRayDamagerSettings | Lib.Munitions.Damagers | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/Damagers/SpallingRayDamagerSettings.cs` | 22 | SpallingRayDamagerSettings : MultiRayConeDamagerSettings | Lib.Munitions.Damagers | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/DescribedWarhead.cs` | 44 | DescribedWarhead : MissileWarhead |  | 1 |  | 0 | 0 | 1 | 0 |
| `AGMLIB/Munitions/LightweightKineticPenetrator.cs` | 0 |  |  | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/LightweightMunition/Basic/ModularLightweightAirburstShell.cs` | 26 | ModularLightweightAirburstShell : LightweightAirburstShell, IModular | AGMLIB.Munitions.LightweightMunition.Basic | 1 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/LightweightMunition/Basic/ModularLightweightClusterShell.cs` | 26 | ModularLightweightClusterShell : LightweightClusterShell, IModular | AGMLIB.Munitions.LightweightMunition.Basic | 1 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/LightweightMunition/Basic/ModularLightweightKineticShell.cs` | 26 | ModularLightweightKineticShell : LightweightKineticShell, IModular | AGMLIB.Munitions.LightweightMunition.Basic | 1 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/LightweightMunition/Basic/ModularLightweightProximityShell.cs` | 26 | ModularLightweightProximityShell : LightweightProximityShell, IModular | AGMLIB.Munitions.LightweightMunition.Basic | 1 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/LightweightMunition/Basic/ModularLightweightSplashingShell.cs` | 27 | ModularLightweightSplashingShell : LightweightSplashingShell, IModular | AGMLIB.Munitions.LightweightMunition.Basic | 1 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/LightweightMunition/CustomArmorDamage.cs` | 124 | ArmorDamage : ScriptableObject<br>CustomArmorDamage : MultiplierArmorDamage<br>MultiplierArmorDamage : ArmorDamage<br>FixedArmorDamage : ArmorDamage<br>ShipControllerApplyArmorDamage<br>ShipControllerDoArmorDamage | AGMLIB.Munitions.LightweightMunition | 0 |  | 2 | 4 | 12 | 0 |
| `AGMLIB/Munitions/LightweightMunition/LightweightKineticBurstContainer.cs` | 51 | LightWeightShellBurst : ScriptableObject<br>LightweightKineticBurstContainer : LightweightKineticMunitionContainer | AGMLIB.Munitions.LightweightMunition | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/LightweightMunition/LightweightMultiShell.cs` | 303 | Fragment : MonoBehaviour<br>Fragments : MonoBehaviour<br>LightweightMultiShell : LightweightKineticShell<br>LightweightKineticMunitionContainerOnRepooled<br>LightweightKineticMunitionContainerDoLookAhead | Lib.Munitions.LightweightMunition | 2 | FixedUpdate, Update | 2 | 3 | 3 | 0 |
| `AGMLIB/Munitions/LightweightMunition/LightweightSelectiveClusterShell.cs` | 52 | LightweightSelectiveClusterShell : LightweightClusterShell<br>FuseOptions<br>ModularLightweightSelectiveClusterShell : LightweightSelectiveClusterShell, IModular | AGMLIB.Munitions.LightweightMunition | 2 |  | 0 | 1 | 0 | 0 |
| `AGMLIB/Munitions/LightweightMunition/LightweightSelectiveProximityShell.cs` | 60 | LightweightSelectiveProximityShell : LightweightProximityShell<br>FuseOptions<br>ModularLightweightSelectiveProximityShell : LightweightSelectiveProximityShell, IModular | AGMLIB.Munitions.LightweightMunition | 2 |  | 0 | 1 | 0 | 0 |
| `AGMLIB/Munitions/LightweightMunition/ModularLookaheadGenericShell.cs` | 80 | LightweightGenericShell : LightweightKineticShell<br>ModularLightweightGenericShell : LightweightGenericShell, IModular | AGMLIB.Munitions.LightweightMunition | 2 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/BeamWarheadDescriptor.cs` | 242 | RuntimeTimeFuse : MonoBehaviour<br>RuntimeTimeFuseState : RuntimeMissileBehaviourState<br>BeamWarheadDescriptor : AngleWarheadDescriptor, IFuse<br>ColliderComparer : IEqualityComparer<Collider><br>LookaheadMunitionOnTriggerEnter |  | 0 |  | 1 | 1 | 9 | 0 |
| `AGMLIB/Munitions/ModularMissile/CommandSeekers.cs` | 886 | AdvancedModularMissile : ModularMissile<br>DebugLine<br>IDebugableMissileSeeker<br>PositionSeekerDescriptor : CommandSeekerDescriptor<br>SeekerSettings<br>SeekerSourceSettings<br>SensorUpdateMode<br>SeekerMode<br>TimedDestroyer : MonoBehaviour<br>RuntimePostionSeeker : RuntimeCommandSeeker, IDebugableMissileSeeker<br>SeekerPositionData<br>ActiveMissileSalvoRegisterPatch<br>MissileDetailOverlayDrawShapesPatch<br>RangedCommandSeekerDescriptor : CommandSeekerDescriptor<br>CommandRangeLostBehavior<br>RuntimeRangedCommandSeeker : RuntimeCommandSeeker |  | 3 | FixedUpdate, Start | 2 | 4 | 2 | 0 |
| `AGMLIB/Munitions/ModularMissile/FusedFragmentationWarheadDescriptor.cs` | 19 | FusedFragmentationWarheadDescriptor : FragmentationWarheadDescriptor, IFuse |  | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/FuseSupportDescriptor.cs` | 121 | IFuse<br>FuseSupportDescriptor : BaseSupportDescriptor, IFuse<br>FuseWarheadDescriptor : BaseWarheadDescriptor |  | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/Illum.cs` | 52 | JammerSupportDescriptorSpawnJammingEffect : MonoBehaviour |  | 0 |  | 0 | 11 | 2 | 0 |
| `AGMLIB/Munitions/ModularMissile/LoiteringModularMissile.cs` | 719 | LoiteringModularMissile : ModularMissile<br>LoiteringFlightPhase |  | 14 |  | 0 | 0 | 1 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularDescriptors/Basic/ModularActiveSeekerDescriptor.cs` | 9 | ModularActiveSeekerDescriptor : ActiveSeekerDescriptor, IModular |  | 1 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularDescriptors/Basic/ModularBaseSupportDescriptor.cs` | 9 | ModularBaseSupportDescriptorr : BaseSupportDescriptor, IModular |  | 1 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularDescriptors/Basic/ModularBeamWarheadDescriptor.cs` | 8 | ModularBeamWarheadDescriptor : BeamWarheadDescriptor, IModular |  | 1 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularDescriptors/Basic/ModularCommandSeekerDescriptor.cs` | 9 | ModularCommandSeekerDescriptor : CommandSeekerDescriptor, IModular |  | 1 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularDescriptors/Basic/ModularCommandSeekers.cs` | 17 | ModularPositionSeekerDescriptor : PositionSeekerDescriptor, IModular<br>ModularRangedCommandSeekerDescriptor : RangedCommandSeekerDescriptor, IModular |  | 2 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularDescriptors/Basic/ModularDecoyLaunchingSupportDescriptor.cs` | 9 | ModularDecoyLaunchingSupportDescriptor : DecoyLaunchingSupportDescriptor, IModular |  | 1 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularDescriptors/Basic/ModularFragmentationWarheadDescriptor.cs` | 9 | ModularFragmentationWarheadDescriptor : FragmentationWarheadDescriptor, IModular |  | 1 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularDescriptors/Basic/ModularImpactConeWarheadDescriptor.cs` | 9 | ModularImpactConeWarheadDescriptor : ImpactConeWarheadDescriptor, IModular |  | 1 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularDescriptors/Basic/ModularJammerSupportDescriptor.cs` | 134 | ModularJammerSupportDescriptor : JammerSupportDescriptor, IModular<br>BeamRuntimeJammerSupport : RuntimeMissileBehaviour |  | 2 | FixedUpdate | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularDescriptors/Basic/ModularKineticPenetratorWarheadDescriptor.cs` | 9 | ModularKineticPenetratorWarheadDescriptor : KineticPenetratorWarheadDescriptor, IModular |  | 1 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularDescriptors/Basic/ModularMissileEngineDescriptor.cs` | 10 | ModularMissileEngineDescriptor : MissileEngineDescriptor, IModular |  | 1 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularDescriptors/Basic/ModularPassiveARHSeekerDescriptor.cs` | 9 | ModularPassiveARHSeekerDescriptor : PassiveARHSeekerDescriptor, IModular |  | 1 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularDescriptors/Basic/ModularPassiveSeekerDescriptor.cs` | 9 | ModularPassiveSeekerDescriptor : PassiveSeekerDescriptor, IModular |  | 1 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularDescriptors/ModularCruiseGuidanceDescriptor.cs` | 85 | ModularCruiseGuidanceDescriptor : CruiseGuidanceDescriptor, IModular, ILimited |  | 5 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularDescriptors/ModularDirectGuidanceDescriptor.cs` | 92 | ModularDirectGuidanceDescriptor : DirectGuidanceDescriptor, IModular, ILimited |  | 6 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularMissileOptions.cs` | 20 | ModularMissileOptions : MonoBehaviour<br>ModularMissileSupportsVisualTargeting |  | 0 |  | 2 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/AmmoModeCycleProfileModule.cs` | 107 | AmmoModeRuntimeState : MonoBehaviour<br>AmmoModeCycleProfileModule : ScriptableObject, IOnDiscreteWeaponCheckFire, IOnWeaponAmmoChanged<br>AmmoModeCycleProfileRuntime |  | 4 |  | 0 | 10 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/AmmoModeFallbackProfileModule.cs` | 68 | AmmoModeFallbackProfileModule : ScriptableObject, IOnDiscreteWeaponCheckFire<br>AmmoModeFallbackProfileRuntime |  | 3 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/AmmoModeResourceProfileModule.cs` | 99 | AmmoModeResourceFailurePolicy<br>AmmoModeResourceProfileModule : ScriptableObject, IOnWeaponAmmoChanged<br>AmmoModeResourceProfileRuntime |  | 4 |  | 0 | 3 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/AreaDamageControlTeamProfileModule.cs` | 123 | AreaTeamEffect<br>AreaDamageControlTeamProfileModule : ScriptableObject, IOnDamageableImpact, IOnGenericImpact<br>AreaDamageControlSuppressionState : MonoBehaviour |  | 4 | Update | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/AreaDebuffProfileModule.cs` | 156 | AreaDebuffProfileModule : ScriptableObject, IOnDamageableImpact, IOnGenericImpact<br>TimedAreaDebuffRemoval : MonoBehaviour |  | 5 |  | 0 | 6 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/AreaShipDisableProfileModule.cs` | 136 | AreaShipCommandDisruption<br>AreaShipDisableProfileModule : ScriptableObject, IOnDamageableImpact, IOnGenericImpact<br>AreaCommandDisruptionState : MonoBehaviour |  | 6 | Update | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/IModular.cs` | 146 | IModular<br>ILimited<br>IOnWeaponAmmoChanged<br>IOnDiscreteWeaponCheckFire<br>IOnDamageableImpact<br>IOnGenericImpact<br>Modular<br>WeaponComponentChangeAmmoTypeModuleCallbacks<br>DiscreteWeaponComponentCheckFireModuleCallbacks |  | 0 |  | 2 | 1 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/IndexedSocketFilter.cs` | 31 | IndexedSocketFilter : MonoBehaviour, IFilterIndexed |  | 8 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/MissileConfigurationEnums.cs` | 36 | WeaponRole<br>LaunchType<br>TargetLost<br>Terminal<br>Guidance |  | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/Patches.cs` | 369 | MissileComponentDescriptorInstallSocketModuleInternal<br>MissileComponentPaletteSetEditingSocket<br>MissileSocketComponentPermittedPatch<br>MissileSettingsPaneOpenSettingsPanel<br>RuntimeMissileWarhead_ShouldFuzeOnTargetPatch |  | 0 |  | 5 | 6 | 28 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/ScriptableFilter.cs` | 333 | ScriptableFilter : ScriptableObject, IFilterIndexed<br>ModularFactionDescription : FactionDescription, IModular<br>HullComponentUseableByFaction<br>FactionDescriptionCheckSharedEquipment<br>LookaheadMunitionUseableByFaction<br>SpacecraftUseableByFaction<br>AvailableMunitionsSetConstructor<br>LightweightMunitionBaseUseableByFaction<br>IFactionLockedUseableByFaction |  | 12 |  | 6 | 6 | 30 | 0 |
| `AGMLIB/Munitions/ModularMissile/Warheads/AngleWarheadDescriptor.cs` | 207 | ThresholdType |  | 0 |  | 0 | 2 | 4 | 0 |
| `AGMLIB/Munitions/ModularMissile/Warheads/ShellWarheadDescriptor.cs` | 165 | BaseShellWarheadDescriptor : AngleWarheadDescriptor<br>ShellWarheadDescriptor : BaseShellWarheadDescriptor<br>LightWeightShellWarheadDescriptor : BaseShellWarheadDescriptor<br>MultiShellWarheadDescriptor : ShellWarheadDescriptor |  | 0 |  | 0 | 1 | 0 | 0 |

## Nebulous

| Source | Lines | Types | Namespace | Serialized | Lifecycle | Patches | Reflection | Logs | TODO markers |
|---|---:|---|---|---:|---|---:|---:|---:|---:|
| `AGMLIB/Nebulous/Bundles/BundleManagerInternals.cs` | 32 | NativeInternalsExtensions<br>BundleManagerInternals<br>Refs |  | 0 |  | 0 | 4 | 0 | 0 |
| `AGMLIB/Nebulous/Game/Orders/Tasks/FireDecoyTaskInternals.cs` | 32 | NativeInternalsExtensions<br>FireDecoyTaskInternals<br>Refs |  | 0 |  | 0 | 4 | 0 | 0 |
| `AGMLIB/Nebulous/Munitions/ModularMissiles/ModularMissileInternals.cs` | 65 | NativeInternalsExtensions<br>ModularMissileInternals<br>Refs |  | 0 |  | 0 | 4 | 0 | 0 |
| `AGMLIB/Nebulous/Ships/WeaponComponentInternals.cs` | 31 | NativeInternalsExtensions<br>WeaponComponentInternals<br>Refs |  | 0 |  | 0 | 4 | 0 | 0 |

## Properties

| Source | Lines | Types | Namespace | Serialized | Lifecycle | Patches | Reflection | Logs | TODO markers |
|---|---:|---|---|---:|---|---:|---:|---:|---:|
| `AGMLIB/Properties/Settings.Designer.cs` | 27 | Settings : global::System.Configuration.ApplicationSettingsBase | Lib.Properties | 0 |  | 0 | 0 | 0 | 0 |

## Server

| Source | Lines | Types | Namespace | Serialized | Lifecycle | Patches | Reflection | Logs | TODO markers |
|---|---:|---|---|---:|---|---:|---:|---:|---:|
| `AGMLIB/Server/CarrierSigPatch.cs` | 21 | BoxSignatureAwake |  | 0 |  | 1 | 2 | 0 | 0 |
| `AGMLIB/Server/DamageCollector.cs` | 223 | FleetTools<br>InventoryItem<br>SerializedInventory<br>ShipControllerGetAfterActionSummary |  | 0 |  | 1 | 0 | 2 | 0 |
| `AGMLIB/Server/Logging.cs` | 66 | Logging<br>TimeWrapper<br>DebugLogError<br>DebugLogErrorComplex<br>DebugLog<br>DebugLogComplex<br>DebugLogWarning<br>DebugLogWarningComplex | AGMLIB.Server | 0 |  | 0 | 0 | 1 | 0 |

## Systems

| Source | Lines | Types | Namespace | Serialized | Lifecycle | Patches | Reflection | Logs | TODO markers |
|---|---:|---|---|---:|---|---:|---:|---:|---:|
| `AGMLIB/Systems/Class1.cs` | 13 | SalvageRules : ScriptableObject<br>InventoryRules : ScriptableObject |  | 0 |  | 0 | 0 | 0 | 0 |
| `AGMLIB/Systems/DroneTester.cs` | 317 | ShipControllerInitializeDroneTester<br>DroneTester : ShipState<br>DroneTesterMode<br>DroneFlight |  | 11 | LateUpdate | 1 | 0 | 0 | 0 |

## Testing

| Source | Lines | Types | Namespace | Serialized | Lifecycle | Patches | Reflection | Logs | TODO markers |
|---|---:|---|---|---:|---|---:|---:|---:|---:|
| `AGMLIB/Testing/TestingComponents.cs` | 566 | TestingComponentAttribute : Attribute<br>TestingComponentFactoryAttribute : Attribute<br>ITestingComponentFactory<br>TestingComponentReport<br>TestingComponentMarker : MonoBehaviour<br>TestingComponentBuilder<br>TestingComponentContext<br>TestingComponentBootstrap<br>DiscoveryJob | Lib.Testing | 2 |  | 0 | 18 | 7 | 0 |
| `AGMLIB/Testing/TestingPrefabInspectors.cs` | 218 | TestingPrefabInspectorAttribute : Attribute<br>ITestingPrefabInspector<br>TestingPrefabInspectionContext<br>TestingPrefabColliderReport<br>TestingPrefabInspectorBootstrap | Lib.Testing | 0 |  | 0 | 0 | 3 | 0 |
| `AGMLIB/Testing/TestingPrefabYamlDumper.cs` | 1128 | TestingPrefabYamlDumper<br>PrefabDumpEntry<br>EnabledModDumpEntry<br>ReferenceComparer : IEqualityComparer<object> | Lib.Testing | 0 |  | 0 | 6 | 5 | 0 |

## AGMMULTITOOL.cs

| Source | Lines | Types | Namespace | Serialized | Lifecycle | Patches | Reflection | Logs | TODO markers |
|---|---:|---|---|---:|---|---:|---:|---:|---:|
| `AGMMULTITOOL.cs` | 586 | Information : EditorWindow<br>AGMMULTITOOL : EditorWindow |  | 0 | OnEnable, Start | 0 | 1 | 11 | 0 |
