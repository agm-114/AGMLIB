# Harmony patch inventory

Detected 162 active-looking `HarmonyPatch` attribute declarations. Verify overloads against the current native assembly.

| Source | Declared target arguments | Reflection references in file |
|---|---|---:|
| `AGMLIB/Advanced/Legacy/Refactored/Loose Code/Patches.cs` | `typeof(Hull), "GetAllSubPartsInternal"` | 0 |
| `AGMLIB/Common/OwnedTypeReflection.cs` | `typeof(StatHelpers), nameof(StatHelpers.InitializeStatValues` | 7 |
| `AGMLIB/Dynamic Systems/ActiveSettings.cs` | `typeof(ContinuousWeaponComponent), nameof(ContinuousWeaponComponent.ReportFired` | 0 |
| `AGMLIB/Dynamic Systems/ActiveSettings.cs` | `typeof(DiscreteWeaponComponent), "OnShellFired"` | 0 |
| `AGMLIB/Dynamic Systems/Area/Core/AreaEffect.cs` | `typeof(ActiveJammingEffect), "TargetGained"` | 0 |
| `AGMLIB/Dynamic Systems/Area/Core/AreaEffect.cs` | `typeof(ActiveJammingEffect), "TargetLost"` | 0 |
| `AGMLIB/Dynamic Systems/Area/ResupplyEffect.cs` | `typeof(HullComponent), nameof(HullComponent.LoadSaveData` | 0 |
| `AGMLIB/Dynamic Systems/BoardingModule.cs` | `typeof(DamageControlBoard), nameof(DamageControlBoard.LinkShip` | 6 |
| `AGMLIB/Dynamic Systems/DynamicActiveSignature.cs` | `typeof(Signature), nameof(Signature.CheckOccluded` | 2 |
| `AGMLIB/Dynamic Systems/DynamicActiveSignature.cs` | `typeof(Signature), nameof(Signature.GetCrossSection` | 2 |
| `AGMLIB/Dynamic Systems/DynamicActiveSignature.cs` | `typeof(Signature), nameof(Signature.GetReturnPowerDensity` | 2 |
| `AGMLIB/Dynamic Systems/DynamicReduction.cs` | `typeof(HullPartResourceConnected), nameof(HullPartResourceConnected.ConsumeResources` | 7 |
| `AGMLIB/Dynamic Systems/DynamicReduction.cs` | `typeof(HullPartResourceConnected), nameof(HullPartResourceConnected.GetResourceDemand` | 7 |
| `AGMLIB/Dynamic Systems/DynamicReduction.cs` | `typeof(ResourceItem), nameof(ResourceItem.SetResource` | 7 |
| `AGMLIB/Dynamic Systems/DynamicReduction.cs` | `typeof(ResourcePool), nameof(ResourcePool.CalculateDemandForEditor` | 7 |
| `AGMLIB/Dynamic Systems/DynamicReduction.cs` | `typeof(Ship), nameof(Ship.EditorRecalcCrewAndResources` | 7 |
| `AGMLIB/Dynamic Systems/DynamicReduction.cs` | `typeof(Ship), nameof(Ship.RunResourceTick` | 7 |
| `AGMLIB/Dynamic Systems/DynamicReduction.cs` | `typeof(Ship), nameof(Ship.SpawnAndAllocateResources` | 7 |
| `AGMLIB/Dynamic Systems/DynamicReload.cs` | `typeof(DiscreteWeaponComponent), "RunTimers"` | 6 |
| `AGMLIB/Dynamic Systems/DynamicStun.cs` | `"WeaponsControl", MethodType.Getter` | 9 |
| `AGMLIB/Dynamic Systems/DynamicStun.cs` | `typeof(ShipController` | 9 |
| `AGMLIB/Dynamic Systems/DynamicStun.cs` | `typeof(ShipController), "HandleDrivesWorkingChanged"` | 9 |
| `AGMLIB/Dynamic Systems/DynamicStun.cs` | `typeof(ShipController), "HandlePowerplantsWorkingChanged"` | 9 |
| `AGMLIB/Dynamic Systems/UI/ShipInfoButton.cs` | `typeof(HumanSkirmishPlayer), nameof(HumanSkirmishPlayer.QueueOrder` | 11 |
| `AGMLIB/Dynamic Systems/UI/ShipInfoButton.cs` | `typeof(ShipInfoBar), "MatchAllButtons"` | 11 |
| `AGMLIB/Dynamic Systems/UI/ShipSignatureDisplayReduction.cs` | `typeof(ShipController), nameof(ShipController.Initialize` | 2 |
| `AGMLIB/Dynamic Systems/UI/ShipSignatureDisplayReduction.cs` | `typeof(ShipInfoBar), "HandleSignatureSizeChanged"` | 2 |
| `AGMLIB/Dynamic Systems/UI/ShipSignatureDisplayReduction.cs` | `typeof(ShipInfoBar), "Update"` | 2 |
| `AGMLIB/Dynamic Systems/UI/ShipStatusPowerBarPatches.cs` | `typeof(ShipController), nameof(ShipController.Initialize` | 1 |
| `AGMLIB/Dynamic Systems/UI/ShipStatusPowerBarPatches.cs` | `typeof(ShipStatusDisplay), nameof(ShipStatusDisplay.LinkShip` | 1 |
| `AGMLIB/Dynamic Systems/UI/ShipStatusPowerBarPatches.cs` | `typeof(ShipStatusIconGroup), nameof(ShipStatusIconGroup.SetShip` | 1 |
| `AGMLIB/Editor/DefaultMissileTemplates.cs` | `typeof(AvailableMunitionsSet), nameof(AvailableMunitionsSet.CreateNewMissile` | 0 |
| `AGMLIB/Editor/DefaultMissileTemplates.cs` | `typeof(MissileEditorSubmodeController), nameof(MissileEditorSubmodeController.OnFleetChanged` | 0 |
| `AGMLIB/Editor/Escort.cs` | `typeof(SkirmishGameManager), "CoroutineSpawnPlayerFleet"` | 0 |
| `AGMLIB/Editor/Lore/Lore.cs` | `typeof(FleetStatsPane), nameof(FleetStatsPane.UpdateFleetStats` | 3 |
| `AGMLIB/Editor/Lore/Lore.cs` | `typeof(HullListItem), nameof(HullListItem.GetDetails` | 3 |
| `AGMLIB/Editor/Lore/Lore.cs` | `typeof(ModalListSelectDetailed), "SelectItemInternal"` | 3 |
| `AGMLIB/Editor/Lore/Lore.cs` | `typeof(PaletteItem), "GetDetailText"` | 3 |
| `AGMLIB/Editor/Lore/Lore.cs` | `typeof(PaletteItem), nameof(PaletteItem.SetComponent` | 3 |
| `AGMLIB/Editor/Lore/Lore.cs` | `typeof(PaletteItem), nameof(PaletteItem.SetCurrentCount` | 3 |
| `AGMLIB/Editor/Points/Discount.cs` | `nameof(Fleet.FleetValue), MethodType.Getter` | 2 |
| `AGMLIB/Editor/Points/Discount.cs` | `typeof(Fleet` | 2 |
| `AGMLIB/Editor/Points/Discount.cs` | `typeof(HullComponent), "GetBasePointCost"` | 2 |
| `AGMLIB/Editor/Points/Discount.cs` | `typeof(HullComponent), nameof(HullComponent.GetFormattedSubtitle` | 2 |
| `AGMLIB/Editor/Points/Discount.cs` | `typeof(HullComponent), nameof(HullComponent.GetPointCost` | 2 |
| `AGMLIB/Editor/ShipEditorSocketUI.cs` | `typeof(FleetCompositionSubmodeController), nameof(FleetCompositionSubmodeController.SetHighlightedSocket` | 3 |
| `AGMLIB/Editor/ShipEditorSocketUI.cs` | `typeof(ShipEditorPane), nameof(ShipEditorPane.OpenPalette` | 3 |
| `AGMLIB/Editor/ShipEditorSocketUI.cs` | `typeof(ShipEditorPane), nameof(ShipEditorPane.SetShip` | 3 |
| `AGMLIB/Editor/SocketClipboard.cs` | `typeof(FleetEditor.Clipboard.SocketClipboardItem), MethodType.Constructor, new Type[` | 0 |
| `AGMLIB/Editor/SocketClipboard.cs` | `typeof(FleetEditor.Clipboard.SocketClipboardItem), nameof(FleetEditor.Clipboard.SocketClipboardItem.Apply` | 0 |
| `AGMLIB/Editor/SocketPatches.cs` | `typeof(HullSocket), nameof(HullSocket.SetComponent` | 7 |
| `AGMLIB/Editor/SocketPatches.cs` | `typeof(SettingsMagazineLoadout), nameof(SettingsMagazineLoadout.AddMagazine` | 7 |
| `AGMLIB/Editor/SocketPatches.cs` | `typeof(Ship), nameof(Ship.LoadFromSave` | 7 |
| `AGMLIB/Editor/SocketPatches.cs` | `typeof(Ship), nameof(Ship.RebuildAmmoFeeder` | 7 |
| `AGMLIB/Editor/SocketPatches.cs` | `typeof(SocketItem), "SetComponent"` | 7 |
| `AGMLIB/Editor/SocketRendering.cs` | `typeof(SocketOutlineManager), nameof(SocketOutlineManager.DrawShapes` | 0 |
| `AGMLIB/Editor/Yaml/YamlLoader.cs` | `typeof(ModRecord), "LoadMod"` | 44 |
| `AGMLIB/Editor/Yaml/YamlLoader.cs` | `typeof(SocketItem), nameof(SocketItem.ButtonLeftClick` | 44 |
| `AGMLIB/Generic Gameplay/AmmoFilter.cs` | `typeof(BaseCellLauncherComponent), nameof(BaseCellLauncherComponent.IsAmmoCompatible` | 0 |
| `AGMLIB/Generic Gameplay/AmmoFilter.cs` | `typeof(BaseTubeLauncherComponent), nameof(BaseTubeLauncherComponent.IsAmmoCompatible` | 0 |
| `AGMLIB/Generic Gameplay/AmmoFilter.cs` | `typeof(BulkMagazineComponent), nameof(BulkMagazineComponent.RestrictionCheck` | 0 |
| `AGMLIB/Generic Gameplay/AmmoFilter.cs` | `typeof(WeaponComponent), nameof(WeaponComponent.IsAmmoCompatible` | 0 |
| `AGMLIB/Generic Gameplay/Armor.cs` | `typeof(ShipController), "WouldArmorHitPenetrate"` | 0 |
| `AGMLIB/Generic Gameplay/Craft/FighterLimit.cs` | `typeof(CraftCarrierController), nameof(CraftCarrierController.Tick` | 6 |
| `AGMLIB/Generic Gameplay/Craft/FighterLimit.cs` | `typeof(CraftLandingPad), nameof(CraftLandingPad.LaunchCraftFromPad` | 6 |
| `AGMLIB/Generic Gameplay/Craft/FighterLimit.cs` | `typeof(ShipController), nameof(ShipController.Initialize` | 6 |
| `AGMLIB/Generic Gameplay/Craft/FighterLimit.cs` | `typeof(Spacecraft), nameof(Spacecraft.SetInStorage` | 6 |
| `AGMLIB/Generic Gameplay/CustomAimPoint.cs` | `typeof(BaseTurretedLauncherComponent), "FixedUpdate"` | 14 |
| `AGMLIB/Generic Gameplay/CustomAimPoint.cs` | `typeof(DiscreteWeaponComponent), "CalculateLead"` | 14 |
| `AGMLIB/Generic Gameplay/DecoyAmmoSettings.cs` | `typeof(FireDecoyTask), "UpdateInternal"` | 0 |
| `AGMLIB/Generic Gameplay/DecoyAmmoSettings.cs` | `typeof(OrderTask), nameof(OrderTask.Complete` | 0 |
| `AGMLIB/Generic Gameplay/DecoyAmmoSettings.cs` | `typeof(ShipController), nameof(ShipController.HasAnyDecoys` | 0 |
| `AGMLIB/Generic Gameplay/Discrete/CustomLeadLogic.cs` | `typeof(DiscreteWeaponComponent), "CalculateLead"` | 1 |
| `AGMLIB/Generic Gameplay/Discrete/DiscreteWeaponEjectors.cs` | `typeof(FixedDiscreteWeaponComponent), "OnTarget"` | 16 |
| `AGMLIB/Generic Gameplay/Ewar/AdvancedActiveFireControlSensor.cs` | `typeof(ActiveFireControlSensor), nameof(ActiveFireControlSensor.CanSeeSignature` | 2 |
| `AGMLIB/Generic Gameplay/Ewar/AdvancedRadar.cs` | `typeof(BaseActiveSensorComponent), "CanSeeSignature"` | 12 |
| `AGMLIB/Generic Gameplay/Ewar/AdvancedRadar.cs` | `typeof(BaseActiveSensorComponent), nameof(BaseActiveSensorComponent.AcquireContacts` | 12 |
| `AGMLIB/Generic Gameplay/Ewar/AdvancedRadar.cs` | `typeof(SensorTrack), nameof(SensorTrack.AddSensor` | 12 |
| `AGMLIB/Generic Gameplay/Ewar/AdvancedRadar.cs` | `typeof(SensorTrack), nameof(SensorTrack.Update` | 12 |
| `AGMLIB/Generic Gameplay/Ewar/DopplerRadar.cs` | `typeof(SensorTrack), nameof(SensorTrack.AcquireLock` | 1 |
| `AGMLIB/Generic Gameplay/Ewar/DopplerRadar.cs` | `typeof(SensorTrack), nameof(SensorTrack.AddSensor` | 1 |
| `AGMLIB/Generic Gameplay/Ewar/DopplerRadar.cs` | `typeof(SensorTrack), nameof(SensorTrack.ReleaseLock` | 1 |
| `AGMLIB/Generic Gameplay/Ewar/DopplerRadar.cs` | `typeof(SensorTrack), nameof(SensorTrack.RemoveSensor` | 1 |
| `AGMLIB/Generic Gameplay/Ewar/DopplerRadar.cs` | `typeof(ShipDetailOverlay), nameof(ShipDetailOverlay.SetShip` | 1 |
| `AGMLIB/Generic Gameplay/Ewar/DopplerRadar.cs` | `typeof(ShipDetailOverlay), nameof(ShipDetailOverlay.UpdateVisible` | 1 |
| `AGMLIB/Generic Gameplay/Ewar/FixedEWarComponent.cs` | `` | 2 |
| `AGMLIB/Generic Gameplay/Ewar/PassiveCommsSensorComponent.cs` | `` | 10 |
| `AGMLIB/Generic Gameplay/Ewar/PassiveCommsSensorComponent.cs` | `` | 10 |
| `AGMLIB/Generic Gameplay/Ewar/PassiveCommsSensorComponent.cs` | `` | 10 |
| `AGMLIB/Generic Gameplay/Ewar/PassiveCommsSensorComponent.cs` | `typeof(SensorTrack), "Game.UI.IBoardPiece.get_ContactIntel"` | 10 |
| `AGMLIB/Generic Gameplay/Ewar/PassiveCommsSensorComponent.cs` | `typeof(SensorTrack), "Game.UI.IBoardPiece.get_ContactIntelPrefix"` | 10 |
| `AGMLIB/Generic Gameplay/Ewar/PassiveCommsSensorComponent.cs` | `typeof(SensorTrack), "Game.UI.IBoardPiece.get_OverrideColor"` | 10 |
| `AGMLIB/Generic Gameplay/Ewar/PassiveCommsSensorComponent.cs` | `typeof(Utility.Localization.LocalizationCore), nameof(Utility.Localization.LocalizationCore.LocalizeToken` | 10 |
| `AGMLIB/Generic Gameplay/Ewar/TacticalModule.cs` | `typeof(ObjectivePoint), "OnTriggerEnter"` | 0 |
| `AGMLIB/Generic Gameplay/Ewar/TacticalModule.cs` | `typeof(ShipController), nameof(ShipController.HasOffensiveAbility` | 0 |
| `AGMLIB/Generic Gameplay/GenericWeaponPatches.cs` | `typeof(DiscreteWeaponComponent), "OnTarget"` | 4 |
| `AGMLIB/Generic Gameplay/GenericWeaponPatches.cs` | `typeof(WeaponComponent), "FixedUpdate"` | 4 |
| `AGMLIB/Generic Gameplay/GenericWeaponPatches.cs` | `typeof(WeaponComponent), "GetNextMuzzle"` | 4 |
| `AGMLIB/Generic Gameplay/Missiles/KinematicMissileEjector.cs` | `typeof(Missile), "FixedUpdate"` | 6 |
| `AGMLIB/Generic Gameplay/Missiles/KinematicMissileEjector.cs` | `typeof(Missile), "Thrust"` | 6 |
| `AGMLIB/Generic Gameplay/Missiles/KinematicMissileEjector.cs` | `typeof(MissileEjector), "LaunchMissile"` | 6 |
| `AGMLIB/Generic Gameplay/Missiles/KinematicMissileEjector.cs` | `typeof(MissileEjector), "MissileInstantiated"` | 6 |
| `AGMLIB/Generic Gameplay/Missiles/KinematicMissileEjector.cs` | `typeof(MissileEjector), nameof(MissileEjector.Fire), new Type[` | 6 |
| `AGMLIB/Generic Gameplay/Missiles/KinematicMissileEjector.cs` | `typeof(MissileEjector), nameof(MissileEjector.Fire), new Type[` | 6 |
| `AGMLIB/Generic Gameplay/Missiles/KinematicMissileEjector.cs` | `typeof(MissileEjector), nameof(MissileEjector.FireEffect` | 6 |
| `AGMLIB/Generic Gameplay/Missiles/MagazineLoader.cs` | `"NeedsExternalAmmoFeed", MethodType.Getter` | 7 |
| `AGMLIB/Generic Gameplay/Missiles/MagazineLoader.cs` | `typeof(BaseCellLauncherComponent` | 7 |
| `AGMLIB/Generic Gameplay/Missiles/MagazineLoader.cs` | `typeof(BaseCellLauncherComponent), nameof(BaseCellLauncherComponent.GetDesignWarnings` | 7 |
| `AGMLIB/Generic Gameplay/Missiles/MagazineLoader.cs` | `typeof(BaseHull), nameof(BaseHull.AmmoTypeInUse` | 7 |
| `AGMLIB/Generic Gameplay/Missiles/MagazineLoader.cs` | `typeof(SettingsMagazineLoadout), nameof(SettingsMagazineLoadout.Set` | 7 |
| `AGMLIB/Generic Gameplay/Missiles/MagazineLoader.cs` | `typeof(SettingsMagazineLoadout), nameof(SettingsMagazineLoadout.UpdateQuantities` | 7 |
| `AGMLIB/Generic Gameplay/Missiles/MagazineLoader.cs` | `typeof(Ship), nameof(Ship.LoadFromSave` | 7 |
| `AGMLIB/Generic Gameplay/Modifer/CustomModiferScaling.cs` | `typeof(StatValue), "CalculateModifier"` | 2 |
| `AGMLIB/Generic Gameplay/Muzzles/GenericMuzzlePatches.cs` | `typeof(BallisticRaycastMuzzle), "CoroutineUpdateBullets"` | 3 |
| `AGMLIB/Generic Gameplay/Muzzles/GenericMuzzlePatches.cs` | `typeof(BallisticRaycastMuzzle), "DamageableImpact"` | 3 |
| `AGMLIB/Generic Gameplay/Muzzles/GenericMuzzlePatches.cs` | `typeof(BallisticRaycastMuzzle), "GenericImpact"` | 3 |
| `AGMLIB/Generic Gameplay/Muzzles/GenericMuzzlePatches.cs` | `typeof(Muzzle), nameof(Muzzle.StopFireEffect` | 3 |
| `AGMLIB/Generic Gameplay/Muzzles/GenericMuzzlePatches.cs` | `typeof(RaycastMuzzle), "DoRaycast"` | 3 |
| `AGMLIB/Generic Gameplay/Muzzles/GenericMuzzlePatches.cs` | `typeof(RaycastMuzzle), nameof(RaycastMuzzle.Fire` | 3 |
| `AGMLIB/Generic Gameplay/Muzzles/GenericMuzzlePatches.cs` | `typeof(RaycastMuzzle), nameof(RaycastMuzzle.FireEffect` | 3 |
| `AGMLIB/Generic Gameplay/Muzzles/GenericMuzzlePatches.cs` | `typeof(RaycastMuzzle), nameof(RaycastMuzzle.StopFireEffect` | 3 |
| `AGMLIB/Generic Gameplay/Muzzles/GenericMuzzlePatches.cs` | `typeof(RezzingMuzzle), nameof(RezzingMuzzle.Fire), typeof(Vector3` | 3 |
| `AGMLIB/Generic Gameplay/Muzzles/GenericMuzzlePatches.cs` | `typeof(RezzingMuzzle), nameof(RezzingMuzzle.FireEffect` | 3 |
| `AGMLIB/Generic Gameplay/Muzzles/GenericMuzzlePatches.cs` | `typeof(SinglePulseRaycastMuzzle), nameof(SinglePulseRaycastMuzzle.FireEffect` | 3 |
| `AGMLIB/Generic Gameplay/Muzzles/MultiBeam.cs` | `typeof(ContinuousWeaponComponent), "StartFiring"` | 10 |
| `AGMLIB/Generic Gameplay/Muzzles/MultiBeam.cs` | `typeof(ContinuousWeaponComponent), "StopFiring"` | 10 |
| `AGMLIB/Generic Gameplay/Muzzles/MultiBeam.cs` | `typeof(FixedContinuousWeaponComponent), "BearToTarget"` | 10 |
| `AGMLIB/Generic Gameplay/Muzzles/MultiBeam.cs` | `typeof(PointDefenseController), "TaskDirectWeapon"` | 10 |
| `AGMLIB/Generic Gameplay/Muzzles/MultiBeam.cs` | `typeof(TurretedContinuousWeaponComponent), "BearToTarget"` | 10 |
| `AGMLIB/Generic Gameplay/Muzzles/MuzzleList.cs` | `typeof(ContinuousWeaponComponent), "StartFiring"` | 6 |
| `AGMLIB/Generic Gameplay/Muzzles/MuzzleList.cs` | `typeof(WeaponComponent), "GetNextMuzzle"` | 6 |
| `AGMLIB/Generic Gameplay/TurretTesting.cs` | `typeof(TurretController), nameof(TurretController.FaceTarget` | 16 |
| `AGMLIB/Generic Gameplay/WeaponCheckOverrides.cs` | `typeof(ActiveJammingEffect), "CheckTargetValidity"` | 0 |
| `AGMLIB/Generic Gameplay/WeaponCheckOverrides.cs` | `typeof(FollowingInstanceMuzzle), nameof(FollowingInstanceMuzzle.Fire` | 0 |
| `AGMLIB/Generic Gameplay/WeaponCheckOverrides.cs` | `typeof(FollowingInstanceMuzzle), nameof(FollowingInstanceMuzzle.StopFire` | 0 |
| `AGMLIB/Generic Gameplay/WeaponCheckOverrides.cs` | `typeof(WeaponComponent), "AimCheck"` | 0 |
| `AGMLIB/Munitions/LightweightMunition/CustomArmorDamage.cs` | `typeof(ShipController), "ApplyArmorDamage"` | 4 |
| `AGMLIB/Munitions/LightweightMunition/CustomArmorDamage.cs` | `typeof(ShipController), "DoArmorDamage"` | 4 |
| `AGMLIB/Munitions/LightweightMunition/LightweightMultiShell.cs` | `typeof(LightweightKineticMunitionContainer), "DoLookAhead"` | 3 |
| `AGMLIB/Munitions/LightweightMunition/LightweightMultiShell.cs` | `typeof(LightweightKineticMunitionContainer), "OnRepooled"` | 3 |
| `AGMLIB/Munitions/ModularMissile/BeamWarheadDescriptor.cs` | `typeof(LookaheadMunitionBase), nameof(LookaheadMunitionBase.OnTriggerEnter` | 1 |
| `AGMLIB/Munitions/ModularMissile/CommandSeekers.cs` | `typeof(ActiveMissileSalvo), nameof(ActiveMissileSalvo.RegisterMissile` | 4 |
| `AGMLIB/Munitions/ModularMissile/CommandSeekers.cs` | `typeof(MissileDetailOverlay), "DrawShapes"` | 4 |
| `AGMLIB/Munitions/ModularMissile/ModularMissileOptions.cs` | `"SupportsVisualTargeting", MethodType.Getter` | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularMissileOptions.cs` | `typeof(ModularMissile` | 0 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/IModular.cs` | `typeof(DiscreteWeaponComponent), "CheckFire"` | 1 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/IModular.cs` | `typeof(WeaponComponent), nameof(WeaponComponent.ChangeAmmoType` | 1 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/Patches.cs` | `typeof(MissileComponentPalette), nameof(MissileComponentPalette.SetEditingSocket` | 6 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/Patches.cs` | `typeof(MissileSettingsPane), "OpenSettingsPanel"` | 6 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/Patches.cs` | `typeof(MissileSocket), nameof(MissileSocket.ComponentPermitted` | 6 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/Patches.cs` | `typeof(ModularMissile), "InstallSocketModuleInternal"` | 6 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/Patches.cs` | `typeof(RuntimeMissileWarhead), "ShouldFuzeOnTarget"` | 6 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/ScriptableFilter.cs` | `` | 6 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/ScriptableFilter.cs` | `MethodType.Constructor, new Type[` | 6 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/ScriptableFilter.cs` | `typeof(AvailableMunitionsSet` | 6 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/ScriptableFilter.cs` | `typeof(HullComponent), nameof(HullComponent.UseableByFaction` | 6 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/ScriptableFilter.cs` | `typeof(LookaheadMunition), nameof(LookaheadMunition.UseableByFaction` | 6 |
| `AGMLIB/Munitions/ModularMissile/ModularSystems/ScriptableFilter.cs` | `typeof(Spacecraft), nameof(Spacecraft.UseableByFaction` | 6 |
| `AGMLIB/Patches/HullPartResourceConnectedGetMissingResourcesPatch.cs` | `typeof(HullPartResourceConnected), "GetMissingResources"` | 0 |
| `AGMLIB/Server/CarrierSigPatch.cs` | `typeof(BoxSignature), "Awake"` | 2 |
| `AGMLIB/Server/DamageCollector.cs` | `typeof(ShipController), nameof(ShipController.GetAfterActionDetails` | 0 |
| `AGMLIB/Systems/DroneTester.cs` | `typeof(ShipController), nameof(ShipController.Initialize` | 0 |
