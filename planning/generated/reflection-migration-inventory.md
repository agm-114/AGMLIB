# Native reflection migration inventory

Files are ordered by the number of known reflection-boundary tokens. Each known native member should move incrementally to a typed `Internals()` accessor when its owning function is changed.

| Source | Reflection-boundary tokens | Harmony attributes |
|---|---:|---:|
| `AGMLIB/Editor/Yaml/YamlLoader.cs` | 44 | 2 |
| `AGMLIB/Testing/TestingComponents.cs` | 18 | 0 |
| `AGMLIB/Generic Gameplay/TurretTesting.cs` | 16 | 1 |
| `AGMLIB/Generic Gameplay/Discrete/DiscreteWeaponEjectors.cs` | 16 | 1 |
| `AGMLIB/Advanced/Hull/PaintScheme.cs` | 14 | 0 |
| `AGMLIB/Generic Gameplay/CustomAimPoint.cs` | 14 | 2 |
| `AGMLIB/Generic Gameplay/Ewar/AdvancedRadar.cs` | 12 | 4 |
| `AGMLIB/Dynamic Systems/UI/ShipInfoButton.cs` | 11 | 2 |
| `AGMLIB/Munitions/ModularMissile/Illum.cs` | 11 | 0 |
| `AGMLIB/Generic Gameplay/Muzzles/MultiBeam.cs` | 10 | 5 |
| `AGMLIB/Generic Gameplay/Ewar/PassiveCommsSensorComponent.cs` | 10 | 7 |
| `AGMLIB/Generic Gameplay/SimpleVisualEffectFactory.cs` | 10 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/AmmoModeCycleProfileModule.cs` | 10 | 0 |
| `AGMLIB/Dynamic Systems/DynamicStun.cs` | 9 | 4 |
| `AGMLIB/Generic Gameplay/Missiles/MagazineLoader.cs` | 7 | 7 |
| `AGMLIB/Common/OwnedTypeReflection.cs` | 7 | 1 |
| `AGMLIB/Dynamic Systems/DynamicReduction.cs` | 7 | 7 |
| `AGMLIB/Editor/SocketPatches.cs` | 7 | 5 |
| `AGMLIB/Advanced/Legacy/Refactored/GunLauncher.cs` | 7 | 0 |
| `AGMLIB/Generic Gameplay/Ewar/IFFComponent.cs` | 7 | 0 |
| `AGMLIB/Testing/TestingPrefabYamlDumper.cs` | 6 | 0 |
| `AGMLIB/Generic Gameplay/Muzzles/MuzzleList.cs` | 6 | 2 |
| `AGMLIB/Generic Gameplay/Missiles/KinematicMissileEjector.cs` | 6 | 7 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/ScriptableFilter.cs` | 6 | 6 |
| `AGMLIB/Generic Gameplay/Craft/FighterLimit.cs` | 6 | 4 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/Patches.cs` | 6 | 5 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/AreaDebuffProfileModule.cs` | 6 | 0 |
| `AGMLIB/Craft/AutoCraftGunTurretWeapon.cs` | 6 | 0 |
| `AGMLIB/Dynamic Systems/DynamicReload.cs` | 6 | 1 |
| `AGMLIB/Dynamic Systems/BoardingModule.cs` | 6 | 1 |
| `AGMLIB/Common/Common.cs` | 5 | 0 |
| `AGMLIB/Nebulous/Bundles/BundleManagerInternals.cs` | 4 | 0 |
| `AGMLIB/Munitions/LightweightMunition/CustomArmorDamage.cs` | 4 | 2 |
| `AGMLIB/EntryPoint.cs` | 4 | 1 |
| `AGMLIB/Advanced/StrikeCraft/FormationManager.cs` | 4 | 0 |
| `AGMLIB/Generic Gameplay/GenericWeaponPatches.cs` | 4 | 3 |
| `AGMLIB/Nebulous/Game/Orders/Tasks/FireDecoyTaskInternals.cs` | 4 | 0 |
| `AGMLIB/Nebulous/Munitions/ModularMissiles/ModularMissileInternals.cs` | 4 | 0 |
| `AGMLIB/Nebulous/Ships/WeaponComponentInternals.cs` | 4 | 0 |
| `AGMLIB/Munitions/ModularMissile/CommandSeekers.cs` | 4 | 2 |
| `AGMLIB/Dynamic Systems/AOEModifer.cs` | 3 | 0 |
| `AGMLIB/Generic Gameplay/Muzzles/GenericMuzzlePatches.cs` | 3 | 11 |
| `AGMLIB/Editor/Lore/Lore.cs` | 3 | 6 |
| `AGMLIB/Generic Gameplay/Missiles/FixedTubeLauncherComponent.cs` | 3 | 0 |
| `AGMLIB/Generic Gameplay/Muzzles/DelayedContinuousRaycastMuzzle.cs` | 3 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/AmmoModeResourceProfileModule.cs` | 3 | 0 |
| `AGMLIB/Editor/ShipEditorSocketUI.cs` | 3 | 3 |
| `AGMLIB/Munitions/LightweightMunition/LightweightMultiShell.cs` | 3 | 2 |
| `AGMLIB/Dynamic Systems/Area/RepairEffect.cs` | 3 | 0 |
| `AGMLIB/Generic Gameplay/Modifer/CustomModiferScaling.cs` | 2 | 1 |
| `AGMLIB/Generic Gameplay/Ewar/FixedEWarComponent.cs` | 2 | 1 |
| `AGMLIB/Dynamic Systems/UI/ShipSignatureDisplayReduction.cs` | 2 | 3 |
| `AGMLIB/Dynamic Systems/DynamicComponent.cs` | 2 | 0 |
| `AGMLIB/Server/CarrierSigPatch.cs` | 2 | 1 |
| `AGMLIB/Editor/Points/Discount.cs` | 2 | 5 |
| `AGMLIB/Dynamic Systems/UI/ShipStatusPowerBarBinding.cs` | 2 | 0 |
| `AGMLIB/Common/ChildSocket.cs` | 2 | 0 |
| `AGMLIB/Common/BrokenDesignator.cs` | 2 | 0 |
| `AGMLIB/Common/Filters.cs` | 2 | 0 |
| `AGMLIB/Dynamic Systems/DynamicActiveSignature.cs` | 2 | 3 |
| `AGMLIB/Dynamic Systems/Area/JammingEffect.cs` | 2 | 0 |
| `AGMLIB/Generic Gameplay/Discrete/FixedDiscreteLauncherComponent.cs` | 2 | 0 |
| `AGMLIB/Generic Gameplay/Ewar/AdvancedActiveFireControlSensor.cs` | 2 | 1 |
| `AGMLIB/Generic Gameplay/Discrete/FixedDiscreteMagWeaponComponent.cs` | 2 | 0 |
| `AGMLIB/Generic Gameplay/Discrete/TurretedDiscreteMagWeaponComponent.cs` | 2 | 0 |
| `AGMLIB/Munitions/ModularMissile/Warheads/AngleWarheadDescriptor.cs` | 2 | 0 |
| `AGMLIB/Advanced/Legacy/LegacyMissile/AdvancedLoiteringMissile.cs` | 2 | 0 |
| `AGMLIB/Editor/SocketFilterCore.cs` | 2 | 0 |
| `AGMLIB/FX/BeamEffectModule.cs` | 2 | 0 |
| `AGMMULTITOOL.cs` | 1 | 0 |
| `AGMLIB/Munitions/ModularMissile/Warheads/ShellWarheadDescriptor.cs` | 1 | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/IModular.cs` | 1 | 2 |
| `AGMLIB/SetCopiedAssetId.cs` | 1 | 0 |
| `AGMLIB/Dynamic Systems/UI/ShipStatusPowerBarPatches.cs` | 1 | 3 |
| `AGMLIB/Dynamic Systems/UI/ShipStatusPowerBar.cs` | 1 | 0 |
| `AGMLIB/Editor/Materials/AdvancedPaintScheme.cs` | 1 | 0 |
| `AGMLIB/Editor/Debugger.cs` | 1 | 0 |
| `AGMLIB/Advanced/Legacy/Rework/EjectorArray.cs` | 1 | 0 |
| `AGMLIB/Advanced/Legacy/Refactored/PaintableHullSegment.cs` | 1 | 0 |
| `AGMLIB/Common/ShipState.cs` | 1 | 0 |
| `AGMLIB/Advanced/StrikeCraft/MovementTarget.cs` | 1 | 0 |
| `AGMLIB/FX/PulsedThrusterPart.cs` | 1 | 0 |
| `AGMLIB/Munitions/LightweightMunition/LightweightSelectiveClusterShell.cs` | 1 | 0 |
| `AGMLIB/Generic Gameplay/Missiles/FixedCellLauncherComponent.cs` | 1 | 0 |
| `AGMLIB/Munitions/ModularMissile/BeamWarheadDescriptor.cs` | 1 | 1 |
| `AGMLIB/Munitions/LightweightMunition/LightweightSelectiveProximityShell.cs` | 1 | 0 |
| `AGMLIB/Generic Gameplay/Discrete/CustomLeadLogic.cs` | 1 | 1 |
| `AGMLIB/Generic Gameplay/Continuous/CasemateContinuousWeaponComponent.cs` | 1 | 0 |
| `AGMLIB/Generic Gameplay/Ewar/DopplerRadar.cs` | 1 | 6 |
| `AGMLIB/Advanced/Hull/ComplexHull.cs` | 1 | 0 |
