# Component inventory

Detected 123 Unity/game component-like public and internal types.

| Type | Base/interface surface | Source | Serialized fields | Lifecycle methods |
|---|---|---|---:|---|
| `ActiveFireControlSensorOptions` | `MonoBehaviour` | `AGMLIB/Generic Gameplay/Ewar/AdvancedActiveFireControlSensor.cs` | 0 |  |
| `ActiveSettings` | `ShipState, IMonoBehaviourFilter<Ship>` | `AGMLIB/Dynamic Systems/ActiveSettings.cs` | 0 |  |
| `AdvancedPaintScheme` | `MonoBehaviour` | `AGMLIB/Editor/Materials/AdvancedPaintScheme.cs` | 0 | Start, Update |
| `AdvancedRadar` | `ShipState` | `AGMLIB/Generic Gameplay/Ewar/AdvancedRadar.cs` | 0 |  |
| `AmmoCompatiblity` | `MonoBehaviour` | `AGMLIB/Generic Gameplay/AmmoFilter.cs` | 1 |  |
| `AmmoModeCycleProfileModule` | `ScriptableObject, IOnDiscreteWeaponCheckFire, IOnWeaponAmmoChanged` | `AGMLIB/Munitions/ModularMissile/ModularSystems/AmmoModeCycleProfileModule.cs` | 4 |  |
| `AmmoModeFallbackProfileModule` | `ScriptableObject, IOnDiscreteWeaponCheckFire` | `AGMLIB/Munitions/ModularMissile/ModularSystems/AmmoModeFallbackProfileModule.cs` | 3 |  |
| `AmmoModeResourceProfileModule` | `ScriptableObject, IOnWeaponAmmoChanged` | `AGMLIB/Munitions/ModularMissile/ModularSystems/AmmoModeResourceProfileModule.cs` | 4 |  |
| `AmmoModeRuntimeState` | `MonoBehaviour` | `AGMLIB/Munitions/ModularMissile/ModularSystems/AmmoModeCycleProfileModule.cs` | 4 |  |
| `AreaCommandDisruptionState` | `MonoBehaviour` | `AGMLIB/Munitions/ModularMissile/ModularSystems/AreaShipDisableProfileModule.cs` | 6 | Update |
| `AreaDamageControlSuppressionState` | `MonoBehaviour` | `AGMLIB/Munitions/ModularMissile/ModularSystems/AreaDamageControlTeamProfileModule.cs` | 4 | Update |
| `AreaDamageControlTeamProfileModule` | `ScriptableObject, IOnDamageableImpact, IOnGenericImpact` | `AGMLIB/Munitions/ModularMissile/ModularSystems/AreaDamageControlTeamProfileModule.cs` | 4 | Update |
| `AreaDebuffProfileModule` | `ScriptableObject, IOnDamageableImpact, IOnGenericImpact` | `AGMLIB/Munitions/ModularMissile/ModularSystems/AreaDebuffProfileModule.cs` | 5 |  |
| `AreaShipDisableProfileModule` | `ScriptableObject, IOnDamageableImpact, IOnGenericImpact` | `AGMLIB/Munitions/ModularMissile/ModularSystems/AreaShipDisableProfileModule.cs` | 6 | Update |
| `Armor` | `MonoBehaviour` | `AGMLIB/Generic Gameplay/Armor.cs` | 0 |  |
| `ArmorDamage` | `ScriptableObject` | `AGMLIB/Munitions/LightweightMunition/CustomArmorDamage.cs` | 0 |  |
| `AssultTeamPiece` | `MonoBehaviour` | `AGMLIB/Dynamic Systems/BoardingModule.cs` | 6 | Update |
| `BaseFilter` | `MonoBehaviour, ISimpleFilter` | `AGMLIB/Editor/SocketFilterCore.cs` | 13 | OnDestroy |
| `BaseMuzzleEffects` | `MonoBehaviour, IMuzzleEffect` | `AGMLIB/FX/MuzzleFX.cs` | 13 | Awake, FixedUpdate |
| `BaseSocketLookup` | `MonoBehaviour` | `AGMLIB/Editor/SocketFilterCore.cs` | 13 | OnDestroy |
| `BaseTrackLogic` | `MonoBehaviour` | `AGMLIB/Generic Gameplay/Ewar/TrackLogic.cs` | 0 | Awake |
| `BasicEffect` | `MonoBehaviour` | `AGMLIB/Dynamic Systems/Area/BasicEffect.cs` | 1 |  |
| `BasicSocketEditorUISettings` | `MonoBehaviour` | `AGMLIB/Editor/SocketFilterCore.cs` | 13 | OnDestroy |
| `BeamEffectModule` | `MonoBehaviour, IEffectModule` | `AGMLIB/FX/BeamEffectModule.cs` | 0 | Start, Update |
| `Binding` | `MonoBehaviour` | `AGMLIB/Dynamic Systems/UI/ShipStatusPowerBar.cs` | 0 | Awake, OnDestroy, Update |
| `BlockingWeapon` | `MonoBehaviour` | `AGMLIB/Generic Gameplay/Discrete/FixedDiscreteLauncherComponent.cs` | 2 | FixedUpdate |
| `BoardingModule` | `MonoBehaviour` | `AGMLIB/Dynamic Systems/BoardingModule.cs` | 6 | Update |
| `BrokenDesignator` | `MonoBehaviour` | `AGMLIB/Common/BrokenDesignator.cs` | 0 | Awake |
| `ChildSocket` | `MonoBehaviour` | `AGMLIB/Common/ChildSocket.cs` | 0 | FixedUpdate, OnDestroy, Start |
| `ComponentDependencies` | `MonoBehaviour` | `AGMLIB/Editor/SocketFilterCore.cs` | 13 | OnDestroy |
| `ContinuousWeaponComponentStartFiring` | `MonoBehaviour` | `AGMLIB/Generic Gameplay/Muzzles/MuzzleList.cs` | 0 | Awake |
| `CraftLaunchLimit` | `MonoBehaviour` | `AGMLIB/Generic Gameplay/Craft/FighterLimit.cs` | 0 |  |
| `CraftWeaponSettings` | `MonoBehaviour` | `AGMLIB/Generic Gameplay/Craft/CraftWeaponSettings.cs` | 1 |  |
| `CraftWorkSlotComponentBase` | `MonoBehaviour, ICraftWorkSlotProvider` | `AGMLIB/Generic Gameplay/Craft/LightweightCraftWorkSlotComponent.cs` | 5 | Awake |
| `CustomAimPoint` | `ShipState` | `AGMLIB/Generic Gameplay/CustomAimPoint.cs` | 0 |  |
| `CustomBehaviorThrusterPartConfig` | `MonoBehaviour` | `AGMLIB/Generic Gameplay/Engines/CustomBehaviorThrusterPartConfig.cs` | 0 |  |
| `CustomCollider` | `MonoBehaviour` | `AGMLIB/Advanced/Legacy/Physics/CustomCollider.cs` | 0 | FixedUpdate |
| `CustomModiferScaling` | `MonoBehaviour` | `AGMLIB/Generic Gameplay/Modifer/CustomModiferScaling.cs` | 0 |  |
| `CustomTraversalLimits` | `MonoBehaviour` | `AGMLIB/Generic Gameplay/TurretTesting.cs` | 0 |  |
| `DecoyAmmoSettings` | `MonoBehaviour` | `AGMLIB/Generic Gameplay/DecoyAmmoSettings.cs` | 0 | Awake, OnDestroy, Update |
| `DefaultMissileTemplate` | `MonoBehaviour` | `AGMLIB/Editor/DefaultMissileTemplates.cs` | 0 |  |
| `DiscreteMagazine` | `MonoBehaviour` | `AGMLIB/Generic Gameplay/Discrete/DiscreteMag.cs` | 0 |  |
| `DiscreteWeaponComponentLeadLogic` | `MonoBehaviour` | `AGMLIB/Generic Gameplay/Discrete/CustomLeadLogic.cs` | 0 |  |
| `DiscreteWeaponEjectors` | `MonoBehaviour` | `AGMLIB/Generic Gameplay/Discrete/DiscreteWeaponEjectors.cs` | 1 | Start |
| `DopplerNotchSettings` | `MonoBehaviour` | `AGMLIB/Generic Gameplay/Ewar/DopplerRadar.cs` | 2 |  |
| `DroneTester` | `ShipState` | `AGMLIB/Systems/DroneTester.cs` | 11 | LateUpdate |
| `DynamicGlow` | `MonoBehaviour` | `AGMLIB/Dynamic Systems/DynamicGlow.cs` | 0 | FixedUpdate, Start |
| `DynamicReductionCache` | `MonoBehaviour` | `AGMLIB/Dynamic Systems/DynamicReduction.cs` | 0 |  |
| `DynamicReload` | `MonoBehaviour` | `AGMLIB/Dynamic Systems/DynamicReload.cs` | 0 |  |
| `DynamicWorkingCache` | `MonoBehaviour` | `AGMLIB/Dynamic Systems/DynamicStun.cs` | 0 |  |
| `EffectSpawner` | `MonoBehaviour` | `AGMLIB/FX/EffectSpawner.cs` | 0 | Start |
| `EjectorArray` | `MonoBehaviour` | `AGMLIB/Advanced/Legacy/Rework/EjectorArray.cs` | 0 | Start, Update |
| `Escort` | `MonoBehaviour` | `AGMLIB/Editor/Escort.cs` | 0 |  |
| `Expander` | `MonoBehaviour` | `AGMLIB/Advanced/Legacy/Physics/Expander.cs` | 0 | Start, Update |
| `FleetListPaneHandleShipAdded` | `MonoBehaviour` | `AGMLIB/Editor/Escort.cs` | 0 |  |
| `FormationManager` | `ShipState` | `AGMLIB/Advanced/StrikeCraft/FormationManager.cs` | 1 | FixedUpdate |
| `Fragment` | `MonoBehaviour` | `AGMLIB/Munitions/LightweightMunition/LightweightMultiShell.cs` | 2 | FixedUpdate, Update |
| `Fragments` | `MonoBehaviour` | `AGMLIB/Munitions/LightweightMunition/LightweightMultiShell.cs` | 2 | FixedUpdate, Update |
| `HoldingTargets` | `ShipState` | `AGMLIB/Advanced/Legacy/Refactored/HoldingTargets.cs` | 0 |  |
| `IFFComponent` | `MonoBehaviour` | `AGMLIB/Generic Gameplay/Ewar/IFFComponent.cs` | 0 | FixedUpdate, Start |
| `IndexedSocketFilter` | `MonoBehaviour, IFilterIndexed` | `AGMLIB/Munitions/ModularMissile/ModularSystems/IndexedSocketFilter.cs` | 8 |  |
| `InternalDiscreteMagazine` | `MonoBehaviour, IMagazineProvider` | `AGMLIB/Generic Gameplay/Discrete/DiscreteMag.cs` | 0 |  |
| `InternalShipState` | `NetworkBehaviour` | `AGMLIB/Common/ShipState.cs` | 0 | Awake |
| `InventoryRules` | `ScriptableObject` | `AGMLIB/Systems/Class1.cs` | 0 |  |
| `JammerSupportDescriptorSpawnJammingEffect` | `MonoBehaviour` | `AGMLIB/Munitions/ModularMissile/Illum.cs` | 0 |  |
| `KeyDebugger` | `ShipState` | `AGMLIB/Editor/Debugger.cs` | 0 | Start, Update |
| `KinematicMissile` | `MonoBehaviour, IMissileFixedUpdate` | `AGMLIB/Generic Gameplay/Missiles/KinematicMissileEjector.cs` | 0 | FixedUpdate, Start |
| `LaunchKinematics` | `MonoBehaviour, IMissileFixedUpdate` | `AGMLIB/Generic Gameplay/Missiles/KinematicMissileEjector.cs` | 0 | FixedUpdate, Start |
| `LightWeightShellBurst` | `ScriptableObject` | `AGMLIB/Munitions/LightweightMunition/LightweightKineticBurstContainer.cs` | 0 |  |
| `Lore` | `MonoBehaviour` | `AGMLIB/Editor/Lore/Lore.cs` | 8 |  |
| `MagazineLoader` | `MonoBehaviour` | `AGMLIB/Generic Gameplay/Missiles/MagazineLoader.cs` | 0 |  |
| `MaterialRandomizer` | `MonoBehaviour` | `AGMLIB/Editor/Materials/MaterialRandomizer.cs` | 0 | Awake |
| `MeleeWeapon` | `MonoBehaviour` | `AGMLIB/Advanced/Legacy/Physics/MeleeWeapon.cs` | 0 | FixedUpdate, Start |
| `MissileSpawner` | `MonoBehaviour` | `AGMLIB/Advanced/Legacy/Rework/MissileSpawner.cs` | 0 | Start |
| `ModularMissileOptions` | `MonoBehaviour` | `AGMLIB/Munitions/ModularMissile/ModularMissileOptions.cs` | 0 |  |
| `MonoComponent` | `MonoBehaviour` | `AGMLIB/Common/Common.cs` | 0 |  |
| `MovementTarget` | `MonoBehaviour` | `AGMLIB/Advanced/StrikeCraft/MovementTarget.cs` | 9 | Awake, FixedUpdate, OnDestroy, Update |
| `MuzzleEffects` | `MonoBehaviour` | `AGMLIB/FX/MuzzleFX.cs` | 13 | Awake, FixedUpdate |
| `MuzzleList` | `MonoBehaviour` | `AGMLIB/Generic Gameplay/Muzzles/MuzzleList.cs` | 0 | Awake |
| `MuzzleState` | `MonoBehaviour` | `AGMLIB/Generic Gameplay/Muzzles/MuzzleList.cs` | 0 | Awake |
| `MVPMagazine` | `MonoBehaviour, IMagazineProvider` | `AGMLIB/Generic Gameplay/Discrete/MVPMagazine.cs` | 0 |  |
| `PaintScheme` | `MonoBehaviour` | `AGMLIB/Advanced/Hull/PaintScheme.cs` | 0 | Awake, FixedUpdate |
| `PaintSchemeMaterialOverride` | `MonoBehaviour` | `AGMLIB/Advanced/Hull/PaintSchemeMaterialOverride.cs` | 0 | Start |
| `PaintSchemeMaterialTarget` | `MonoBehaviour` | `AGMLIB/Advanced/Hull/PaintSchemeMaterialTarget.cs` | 0 | Awake, LateUpdate |
| `PassiveCommsSensorComponent` | `HullComponent, ISensorComponent, IDeltaSensor, ISensor, IJammable, IEWarTarget, IOwned, IHullComponent` | `AGMLIB/Generic Gameplay/Ewar/PassiveCommsSensorComponent.cs` | 10 | OnDisable, OnEnable |
| `PrefabVisualEffectFactory` | `MonoBehaviour` | `AGMLIB/Generic Gameplay/SimpleVisualEffectFactory.cs` | 0 | Awake |
| `PulsedLocalEffect` | `MonoBehaviour` | `AGMLIB/FX/PulsedLocalEffect.cs` | 3 |  |
| `RequiredResources` | `MonoBehaviour` | `AGMLIB/Dynamic Systems/DynamicReduction.cs` | 0 |  |
| `ResourceBar` | `MonoBehaviour` | `AGMLIB/Dynamic Systems/UI/ResourceBar.cs` | 0 | FixedUpdate, Start |
| `RestoreStatus` | `MonoBehaviour` | `AGMLIB/Dynamic Systems/Area/RepairEffect.cs` | 0 | FixedUpdate |
| `Rope` | `MonoBehaviour` | `AGMLIB/Advanced/Legacy/Physics/Rope.cs` | 0 | FixedUpdate, Start, Update |
| `RotationMonitor` | `MonoBehaviour` | `AGMLIB/Advanced/Legacy/Rework/RotationMonitor.cs` | 0 | Start, Update |
| `RuntimeTimeFuse` | `MonoBehaviour` | `AGMLIB/Munitions/ModularMissile/BeamWarheadDescriptor.cs` | 0 |  |
| `SalvageRules` | `ScriptableObject` | `AGMLIB/Systems/Class1.cs` | 0 |  |
| `ScalingAOEExplosionEffectModule` | `MonoBehaviour, IDamageDealer, IDamageCharacteristic, IEffectModule, ILocalImbued, IOwned, IWeightedEffect` | `AGMLIB/Generic Gameplay/ScalingAOEExplosionEffectModule.cs` | 20 |  |
| `ScriptableFilter` | `ScriptableObject, IFilterIndexed` | `AGMLIB/Munitions/ModularMissile/ModularSystems/ScriptableFilter.cs` | 12 |  |
| `SetCopiedAssetId` | `MonoBehaviour` | `AGMLIB/SetCopiedAssetId.cs` | 0 |  |
| `ShipInfoButton` | `ShipState` | `AGMLIB/Dynamic Systems/UI/ShipInfoButton.cs` | 0 | Update |
| `ShipSignatureDisplayReduction` | `MonoBehaviour` | `AGMLIB/Dynamic Systems/UI/ShipSignatureDisplayReduction.cs` | 0 | Awake, FixedUpdate |
| `ShipState` | `NetworkBehaviour` | `AGMLIB/Common/ShipState.cs` | 0 | Awake |
| `ShipStatusPowerBar` | `MonoBehaviour` | `AGMLIB/Dynamic Systems/UI/ShipStatusPowerBar.cs` | 0 | Awake, OnDestroy, Update |
| `ShipStatusPowerBarUpdater` | `MonoBehaviour` | `AGMLIB/Dynamic Systems/UI/ShipStatusPowerBar.cs` | 0 | Awake, OnDestroy, Update |
| `SimpleCraft` | `MonoBehaviour, ICraft` | `AGMLIB/Advanced/StrikeCraft/SimpleCraft.cs` | 5 | Awake, FixedUpdate |
| `SimpleDiscount` | `MonoBehaviour` | `AGMLIB/Editor/Points/Discount.cs` | 0 |  |
| `SimpleMagazine` | `MonoBehaviour, IMagazine` | `AGMLIB/Advanced/Legacy/LegacyMissile/SimpleMagazine.cs` | 3 |  |
| `SimpleSlider` | `MonoBehaviour` | `AGMLIB/Advanced/Legacy/Physics/SimpleSlider.cs` | 0 | Start, Update |
| `SingleRayDamagerSettings` | `ScriptableObject` | `AGMLIB/Munitions/Damagers/SingleRayDamagerSettings.cs` | 0 |  |
| `SocketEditorChildSettings` | `MonoBehaviour` | `AGMLIB/Editor/SocketFilterCore.cs` | 13 | OnDestroy |
| `SocketEditorParentSettings` | `MonoBehaviour` | `AGMLIB/Editor/SocketFilterCore.cs` | 13 | OnDestroy |
| `SocketGroupComponentChangeBinding` | `MonoBehaviour` | `AGMLIB/Editor/SocketFilterCore.cs` | 13 | OnDestroy |
| `SocketGroupDropdown` | `MonoBehaviour, IPointerEnterHandler, IPointerExitHandler` | `AGMLIB/Editor/SocketFilterCore.cs` | 13 | OnDestroy |
| `SpatialSoundEffectModule` | `MonoBehaviour, IEffectModule` | `AGMLIB/FX/SpatialSoundEffectModule.cs` | 1 |  |
| `StartState` | `MonoBehaviour` | `AGMLIB/Dynamic Systems/Area/ResupplyEffect.cs` | 0 |  |
| `StaticFX` | `MonoBehaviour` | `AGMLIB/FX/DynamicFX.cs` | 0 | Start |
| `StrikeCraft` | `MonoBehaviour` | `AGMLIB/Advanced/Legacy/Refactored/StrikeCraft.cs` | 0 | FixedUpdate, Start, Update |
| `StringFormatter` | `MonoBehaviour` | `AGMLIB/Editor/Lore/StringFormatter.cs` | 7 |  |
| `StructureLinker` | `MonoBehaviour` | `AGMLIB/Advanced/Legacy/Physics/StructureLinker.cs` | 0 | Start, Update |
| `TacticalModule` | `ShipState` | `AGMLIB/Generic Gameplay/Ewar/TacticalModule.cs` | 3 | Update |
| `TestingComponentMarker` | `MonoBehaviour` | `AGMLIB/Testing/TestingComponents.cs` | 2 |  |
| `TimedAreaDebuffRemoval` | `MonoBehaviour` | `AGMLIB/Munitions/ModularMissile/ModularSystems/AreaDebuffProfileModule.cs` | 5 |  |
| `TimedDestroyer` | `MonoBehaviour` | `AGMLIB/Munitions/ModularMissile/CommandSeekers.cs` | 3 | FixedUpdate, Start |
| `WeaponCheckOverrides` | `MonoBehaviour` | `AGMLIB/Generic Gameplay/WeaponCheckOverrides.cs` | 7 |  |
| `YamlComp` | `MonoBehaviour` | `AGMLIB/Editor/Yaml/YamlLoader.cs` | 0 | Awake |
