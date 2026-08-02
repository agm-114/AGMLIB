# Legacy source file atlas

The legacy assembly decompiles to 170 source files. This atlas names every file so the migration
does not silently lose a helper or patch. Compiler-generated bodies are represented by their
declaring source file.

## Entry point, data, UI, and utilities

Disposition: delete `ModEntryPoint` and serializer/UI bootstrapping; migrate reusable data to native
assets or the serialized-type mapping. The embedded Cobyla solver existed to fit the old shield
visual and is not required by the modern component.

- `ActiveMissileSalvoExtensions.cs`
- `ComplexCrewJobNames.cs`
- `Cureos/Numerics/CalcfcDelegate.cs`
- `Cureos/Numerics/Cobyla.cs`
- `Cureos/Numerics/CobylaExitStatus.cs`
- `CustomBehaviorThrusterPartConfig.cs`
- `ExpandedMountDisplayUtil.cs`
- `Factions/FactionDescriptionWithoutDefaults.cs`
- `FleetEditor/ClusterMagazineAmmoItem.cs`
- `FleetEditor/MissileEditor/ClusterWarheadSettings.cs`
- `FleetEditor/PaletteItemExtensions.cs`
- `FleetEditor/SettingsClusterLoadout.cs`
- `Game/Intel/ShieldedShipIntelReport.cs`
- `Game/Orders/Tasks/ToggleShieldsTask.cs`
- `HaloShipUpgrade.cs`
- `HullSegmentPatchIndicator.cs`
- `HullSocketBuiltIn.cs`
- `MissileSummaryExtensions.cs`
- `ModEntryPoint.cs`
- `ModularMissileExtensions.cs`
- `Properties/AssemblyInfo.cs`
- `RotationFollower.cs`
- `ShieldMonitor.cs`
- `SocketComponentScaler.cs`
- `StorageMonitor.cs`
- `SyncVisualEffect.cs`
- `TurretHideBase.cs`
- `Utility/UtilityGameObjectGetOrAddComponent.cs`

Modern outcome:

- the monitor interface failure disappears because the old monitors and expanded-mount UI are
  removed;
- `CrewJobLabels`, `RotationFollower`, `SocketComponentScaler`, and
  `VisualEffectStateFollower` cover retained authored behavior;
- shield orders/reports must use a current, typed order/network integration rather than custom
  serializer registration.

## Munition and missile runtime

Disposition: re-author to current native missile, seeker, lightweight munition, warhead, pooling,
and bulk-save APIs. Keep only the narrow command-seeker profile and native-submunition profile in
AGMLIB.

- `Munitions/CustomCommandGuidedSeeker.cs`
- `Munitions/GuidedShellMunition.cs`
- `Munitions/GuidedSplashingShellMunition.cs`
- `Munitions/InstancedDamagers/CustomStructureOnlyDamager.cs`
- `Munitions/InstancedDamagers/DestroyShieldsDamager.cs`
- `Munitions/InstancedDamagers/OverloadDebuffDamager.cs`
- `Munitions/LightweightDebuffMACShell.cs`
- `Munitions/LightweightMACShell.cs`
- `Munitions/MissileBlankWarhead.cs`
- `Munitions/ModularMissiles/Descriptors/Warheads/ClusterWarheadDescriptor.cs`
- `Munitions/ModularMissiles/ModularMissileDetailStringReplacement.cs`
- `Munitions/MultipleMissileSeeker.cs`
- `Munitions/PassiveSeeker.cs`
- `Munitions/PlasmaTorpedo.cs`
- `Munitions/PlasmaTorpedoDirect.cs`

`GuidedShellMunition` and its pool patch cannot be ported by renaming a base type; the current game
removed that inheritance chain. Plasma shells/torpedoes need current munition authoring.

Non-serialized helper outcomes:

- `CustomStructureOnlyDamager` is replaced by the current lightweight shell's authored dedicated
  structure-damage path and native `StructureOnlyDamager`;
- `DestroyShieldsDamager` becomes the explicit `IShieldDisruptingDamageDealer` wrapper created by
  `ModernDebuffKineticShell`; shield code does not inspect native private chain fields;
- `OverloadDebuffDamager` becomes `ImpactComponentDebuffDamager` with typed current
  `HullComponent.Internals()` access;
- `MissileBlankWarhead` is unnecessary when the current missile is authored without a damaging
  warhead/detonation role;
- `PassiveSeeker` maps to the current passive seeker descriptor/runtime, while
  `MultipleMissileSeeker` maps to current modular seeker slots; neither old type has a serialized
  bundle use;
- `PlasmaTorpedo` maps to `ModernEvasiveCruiseMissile`; the unused direct variant has no serialized
  bundle use and stays deleted.

## Ship, weapon, resource, and shield runtime

Disposition is per the serialized script inventory. Native current components own ordinary weapon,
resource, magazine, save, and report behavior; AGMLIB owns only the remaining custom delta.

- `Ships/BerthingComponentFractional.cs`
- `Ships/ChargingRezzingMuzzle.cs`
- `Ships/ComponentHullPaintLODSharedIndexed.cs`
- `Ships/CovenantBarrelGlow.cs`
- `Ships/CustomBehaviorThrusterPart.cs`
- `Ships/CustomLineBeamMuzzleEffects.cs`
- `Ships/FixedEWarComponent.cs`
- `Ships/HullComponentCostExtensions.cs`
- `Ships/HullComponentExtensions.cs`
- `Ships/HullComponentStorage.cs`
- `Ships/HullComponentTileable.cs`
- `Ships/HullPartDrive.cs`
- `Ships/HullResources.cs`
- `Ships/HullSocketFixedWeaponGuidance.cs`
- `Ships/InfiniteRezzingMuzzle.cs`
- `Ships/IStorageResourceSubscriber.cs`
- `Ships/MACFixedDiscreteWeaponComponent.cs`
- `Ships/MultipleEjectorTubeLauncherComponent.cs`
- `Ships/PassiveSensorComponentCustomWake.cs`
- `Ships/PlasmaCannon.cs`
- `Ships/PlasmaLance.cs`
- `Ships/PowerUsageEmissive.cs`
- `Ships/ResourcePoolStored.cs`
- `Ships/ResourcePoolStoredExtensions.cs`
- `Ships/RestrictedBulkMagazineComponent.cs`
- `Ships/Shield/ShieldComponent.cs`
- `Ships/Shield/ShieldComponentEffects.cs`
- `Ships/Shield/ShieldComponentHolder.cs`
- `Ships/Shield/ShieldManager.cs`
- `Ships/Shield/ShieldNetworkBehavior.cs`
- `Ships/Shield/ShieldWeaponGroup.cs`
- `Ships/ShieldComponentHullPaint.cs`
- `Ships/ShipExtensionsShield.cs`
- `Ships/ShipResourceExtensions.cs`
- `Ships/SocketCapRemover.cs`
- `Ships/SocketRestrictor/ComponentRequiresWhitelist.cs`
- `Ships/SocketRestrictor/PatchComponentPaletteSocketRestrictor.cs`
- `Ships/SocketRestrictor/PatchHullSocketSocketRestrictor.cs`
- `Ships/SocketRestrictor/SocketComponentBlacklist.cs`
- `Ships/SocketRestrictor/SocketComponentWhitelist.cs`
- `Ships/TripleChargingRezzingMuzzle.cs`
- `Ships/TripleMACFixedDiscreteWeaponComponent.cs`
- `Ships/TurretedContinuousWeaponComponentExtra.cs`
- `Ships/TurretedDiscreteWeaponComponentEnergy.cs`
- `Ships/TurretedDiscreteWeaponComponentExtra.cs`
- `Ships/VisualEffectBeamMuzzleEffects.cs`

Notable replacement boundaries:

- `ResourcePoolStored*`, `HullComponentStorage`, storage subscribers, and storage monitors are
  deleted. Shields have explicit capacity and ordinary native power demand.
- `RestrictedBulkMagazineComponent` becomes native `BulkMagazineComponent` plus a filter.
- `LightweightDebuffMACShell` becomes `ModernDebuffKineticShell`; ordinary MAC shells are current
  native lightweight kinetic assets.
- both MAC component classes converge on `MultiModeFixedWeapon`;
- both charging muzzle classes converge on `MultiModeChargingRezzingMuzzle`;
- shield manager/component gameplay converges on the opt-in `ModernShieldComponent`; a small
  current network presentation relay remains an integration task;
- all six legacy shield-holder components converge on exact-collider `ShieldHitSurface` bindings;
- both socket restriction patches are replaced by two small opt-in current patches keyed by rule
  components and stable save keys.

## Editor, faction, fleet, and point-cost patches

Disposition: delete. Current native APIs should be configured through assets. Reintroduce a patch
only after a failing current-version test identifies a missing capability.

- `PatchActionMenuOrderTurretWeaponSelection.cs`
- `PatchAvailableMunitionsSet.cs`
- `PatchBaseCellLauncherComponentGetFormattedStats.cs`
- `PatchBaseHull.cs`
- `PatchBaseHullEditorFormatHullStats.cs`
- `PatchBaseHullGetTotalPoints.cs`
- `PatchBerthingComponent.cs`
- `PatchComponentPaletteGetItemsForSocket.cs`
- `PatchEditingMissileList.cs`
- `PatchFleet.cs`
- `PatchFleetCompositionSubmodeController.cs`
- `PatchFleetDetailSummary.cs`
- `PatchFleetEditorController.cs`
- `PatchFleetEditorControllerSetFleet.cs`
- `PatchHullComponentCanTile.cs`
- `PatchHullComponentCostCovenant.cs`
- `PatchHullComponentGetFormattedResources.cs`
- `PatchHullComponentInitStats.cs`
- `PatchHullComponentResources.cs`
- `PatchHullComponentUseableByFaction.cs`
- `PatchHullSegmentBasicMakeBakeMaterial.cs`
- `PatchHullSocket.cs`
- `PatchLightweightMunitionBaseUseableByFaction.cs`
- `PatchLookaheadMunitionUseableByFaction.cs`
- `PatchMissileComponentDescriptorUseableByFaction.cs`
- `PatchMissileEditorSubmodeController.cs`
- `PatchMissileEngineDescriptor.cs`
- `PatchMissileSettingsPane.cs`
- `PatchModRecordLoadMod.cs`
- `PatchModularMissileGetDetailTextInternal.cs`
- `PatchModularMissileUseableByFaction.cs`
- `PatchMountStatusDisplaySetMountsEMD.cs`
- `PatchPaletteItemGetDetails.cs`
- `PatchResourceItem.cs`
- `PatchShipEditorFormatCrew.cs`
- `PatchShipInfoBar.cs`
- `PatchShipNameGeneratorHuman.cs`
- `PatchShipStatsPaneShield.cs`
- `PatchShipValidatePointTotal.cs`
- `PatchStatHelper.cs`
- `PatchWeaponSelectList.cs`
- `PatchWeaponSelectListEMD.cs`

The modern socket-rule palette postfix in AGMLIB is the intentional exception; it activates only
when the selected socket carries an AGMLIB whitelist or blacklist.

## AI, orders, targeting, and weapon-facing patches

Disposition: delete and test native behavior first. The old patches target removed AI types and
private weapon-group methods.

- `PatchAICaptain.cs`
- `PatchAICaptainAssignWeaponTargets.cs`
- `PatchFirePositionTask.cs`
- `PatchFireTrackTask.cs`
- `PatchHumanSkirmishPlayer.cs`
- `PatchPointDefenseController.cs`
- `PatchShipControllerCoroutineReturnFireShield.cs`
- `PatchShipControllerEW.cs`
- `PatchWeaponGroupGetFacingForTargetInternal.cs`
- `PatchWeaponGroupGetFacingTarget.cs`
- `PatchWeaponGroupGetPreFacingForPosition.cs`

Acceptance tests must cover AI selection of fixed MACs, plasma weapons, missile launchers, point
defense, return fire, and shielded targets before any replacement patch is considered.

## Missile runtime, pooling, and guidance patches

Disposition: delete in favor of current native missile runtime. `ModernSubmunitionWarhead` delegates
spawn, programming, pooling, salvo, and multiplayer behavior to the native base.

- `PatchLookaheadMunitionBase.cs`
- `PatchMissile.cs`
- `PatchMissileEjector.cs`
- `PatchModularMissile.cs`
- `PatchNetworkObjectPoolerGuidedShell.cs`
- `PatchRuntimeMissileGuidance.cs`
- `PatchRuntimeMissileWarhead.cs`

`MultiEjectorTubeLauncher` uses the current `MissileEjector.Fire` overloads and the current launcher
RPC provider; it does not restore the old missile-ejector patches.

## Ship initialization, resource, serialization, UI, and shield patches

Disposition: delete and replace with component-owned lifecycle. The two current shield damage
patches are narrow adapters that do nothing unless a `ModernShieldComponent` is registered.

- `PatchFriendlyShipItemShield.cs`
- `PatchFriendlyShipItemStorage.cs`
- `PatchGenericSerializersWriteWeaponGroupShield.cs`
- `PatchPassiveSensorComponentCustomWake.cs`
- `PatchShip.cs`
- `PatchShipBuildUngroupedWeaponGroupsShield.cs`
- `PatchShipControllerDoArmorDamageOnly.cs`
- `PatchShipControllerDoDamage.cs`
- `PatchShipControllerGetCurrentThrusterPower.cs`
- `PatchShipControllerGetReport.cs`
- `PatchShipControllerHandleThrusterFunctioningChanged.cs`
- `PatchShipControllerInitialize.cs`
- `PatchShipControllerInitializePowerUsageEmissive.cs`
- `PatchShipControllerInitializeShield.cs`
- `PatchResourcePool.cs`
- `PatchShipResources.cs`
- `PatchShipRunResourceTickStorage.cs`
- `PatchSkirmishGameManager.cs`
- `PatchSkirmishGameManagerBeam.cs`
- `PatchSkirmishGameManagerShield.cs`
- `PatchSpectatorShipItemStorage.cs`

The rebuilt shield order and remote-client presentation must use a current network component or
component-routing extension; it must not regenerate the old generic serializers.

## Count check

- entry/data/UI/utilities: 28 files;
- munition/missile runtime: 15 files;
- ship/weapon/resource/shield runtime: 46 files;
- editor/faction/fleet/cost patches: 42 files;
- AI/order/targeting patches: 11 files;
- missile runtime patches: 7 files;
- ship/resource/serialization/UI patches: 21 files.

Total: 170 files.
