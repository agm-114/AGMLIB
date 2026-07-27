using System.Reflection;
using Game;
using Game.Units;
using HarmonyLib;
using Munitions;
using Ships;
using Ships.Controls;
using UnityEngine;

namespace Lib.Generic_Gameplay;

[DisallowMultipleComponent]
[RequireComponent(typeof(DiscreteWeaponComponent))]
public sealed class DecoyAmmoFibonacciSlew : MonoBehaviour
{
    [SerializeField, Min(1f)]
    private float _targetDistance = 1000f;

    private DiscreteWeaponComponent _weapon = null!;
    private DecoyAmmoShipSpiral _shipSpiral = null!;
    private float _chaffCloudLifetime;
    private bool _active;
    private bool _positionPatternActive;
    private bool _settingSpiralTarget;

    internal bool BlocksNormalFire => _active;
    internal bool IgnoresRangeLimits => _active || _positionPatternActive;
    internal bool ParticipatesInSpiral => _active || _positionPatternActive;
    internal bool SuppressHullManeuvering => _active || _positionPatternActive;
    internal float LiveShellCapacity =>
        Mathf.Max(1f, _chaffCloudLifetime / GetShotDelay());

    private void Awake()
    {
        _weapon = GetComponent<DiscreteWeaponComponent>();
        ShipController ship = GetComponentInParent<ShipController>();
        _shipSpiral = ship.GetComponent<DecoyAmmoShipSpiral>()
            ?? ship.gameObject.AddComponent<DecoyAmmoShipSpiral>();
    }

    internal void Begin(IMunition chaff)
    {
        ConfigurePattern(chaff);
        if (_active)
        {
            _shipSpiral.Recalculate();
            return;
        }

        _positionPatternActive = false;
        _active = true;
        _shipSpiral.Recalculate();
        AimNextPoint();
    }

    internal void PositionOrderIssued()
    {
        if (_settingSpiralTarget)
        {
            return;
        }

        IMunition? selectedAmmo = _weapon.SelectedAmmoType;
        if (!DecoyAmmoSettings.IsChaffAmmo(selectedAmmo))
        {
            if (_positionPatternActive)
            {
                CancelPositionPattern();
            }

            return;
        }

        ConfigurePattern(selectedAmmo);
        _positionPatternActive = true;
        _shipSpiral.Recalculate();
        UnityEngine.Debug.Log(
            $"AGMLIB DecoyAmmoFibonacciSlew: event=position-armed weapon={_weapon.name}");
    }

    internal void NativeShotFired()
    {
        if (!_positionPatternActive)
        {
            return;
        }

        AimNextPoint();
        UnityEngine.Debug.Log(
            $"AGMLIB DecoyAmmoFibonacciSlew: event=position-advance weapon={_weapon.name}");
    }

    internal void UpdatePositionPattern()
    {
        if (_positionPatternActive &&
            ((WeaponComponent)_weapon).Internals().TargetBlocked)
        {
            AimNextPoint();
        }
    }

    internal void CancelPositionPattern()
    {
        _positionPatternActive = false;
        _shipSpiral.Recalculate();
    }

    internal bool CanTrainOnTarget(Vector3 target) =>
        _weapon.CanTrainOnTarget(target);

    internal bool TryPrepareShot()
    {
        if (!_active)
        {
            return false;
        }

        WeaponComponentInternals internals = ((WeaponComponent)_weapon).Internals();
        if (internals.TargetBlocked)
        {
            AimNextPoint();
            return false;
        }

        return internals.OnTarget;
    }

    internal void ShotFired(bool hasAnotherShot)
    {
        if (hasAnotherShot)
        {
            AimNextPoint();
            return;
        }

        _active = false;
        _shipSpiral.Recalculate();
        ((IWeapon)_weapon).CeaseFire();
    }

    private void AimNextPoint()
    {
        float distance = Mathf.Max(1f, _targetDistance);
        if (_shipSpiral.TryGetNextTarget(this, distance, out Vector3 target))
        {
            SetSpiralTarget(target);
            return;
        }

        SetSpiralTarget(_shipSpiral.GetTopTarget(distance));
    }

    private void ConfigurePattern(IMunition? chaff)
    {
        _chaffCloudLifetime = GetChaffCloudLifetime(chaff);
    }

    private static float GetChaffCloudLifetime(IMunition? chaff)
    {
        if (chaff is AirburstRocket airburst &&
            airburst.Internals().BurstEffect is GameObject burstEffect &&
            burstEffect.GetComponent<NetworkedShortDurationEffect>() is
                NetworkedShortDurationEffect lingeringEffect)
        {
            return Mathf.Max(0f, lingeringEffect.Internals().Duration);
        }

        return 0f;
    }

    private float GetShotDelay()
    {
        float? roundsPerSecond = _weapon.RoundsPerSecond;
        if (roundsPerSecond is > 0f)
        {
            return 1f / roundsPerSecond.Value;
        }

        return Mathf.Max(Time.fixedDeltaTime, _weapon.Internals().RecycleTime);
    }

    private void SetSpiralTarget(Vector3 target)
    {
        _settingSpiralTarget = true;
        try
        {
            ((IWeapon)_weapon).FirePosition(target, 0, false);
        }
        finally
        {
            _settingSpiralTarget = false;
        }
    }

}

[HarmonyPatch(typeof(WeaponComponent), "CheckOnTarget")]
internal static class WeaponComponentFibonacciSlewRangePatch
{
    private static void Prefix(WeaponComponent __instance, ref bool ignoreRange)
    {
        if (__instance.GetComponent<DecoyAmmoFibonacciSlew>()?.IgnoresRangeLimits == true)
        {
            ignoreRange = true;
        }
    }
}

[HarmonyPatch(typeof(WeaponComponent), nameof(WeaponComponent.IsTargetInRange))]
internal static class WeaponComponentFibonacciSlewTargetRangePatch
{
    private static void Postfix(WeaponComponent __instance, ref bool __result)
    {
        if (__instance.GetComponent<DecoyAmmoFibonacciSlew>()?.IgnoresRangeLimits == true)
        {
            __result = true;
        }
    }
}

[HarmonyPatch(typeof(WeaponComponent), "GetFunctionalActivityStatus")]
internal static class WeaponComponentFibonacciSlewActivityPatch
{
    private static void Postfix(WeaponComponent __instance, ref ComponentActivity __result)
    {
        if (__result == ComponentActivity.OutOfRange &&
            __instance.GetComponent<DecoyAmmoFibonacciSlew>()?.IgnoresRangeLimits == true)
        {
            __result = ComponentActivity.Training;
        }
    }
}

[HarmonyPatch(typeof(DiscreteWeaponComponent), "OnTarget")]
internal static class DiscreteWeaponComponentFibonacciSlewOnTargetPatch
{
    [HarmonyPriority(HarmonyLib.Priority.First)]
    private static bool Prefix(DiscreteWeaponComponent __instance, out int __state)
    {
        __state = __instance.Internals().MagazineFired;
        return !(__instance.GetComponent<DecoyAmmoFibonacciSlew>()?.BlocksNormalFire ?? false);
    }

    private static void Postfix(DiscreteWeaponComponent __instance, int __state)
    {
        if (__instance.Internals().MagazineFired != __state)
        {
            __instance.GetComponent<DecoyAmmoFibonacciSlew>()?.NativeShotFired();
        }
    }
}

[HarmonyPatch]
internal static class WeaponComponentFirePositionFibonacciSlewPatch
{
    private static MethodInfo TargetMethod() =>
        AccessTools.Method(
            typeof(WeaponComponent),
            $"{typeof(IWeapon).FullName}.{nameof(IWeapon.FirePosition)}",
            [typeof(Vector3), typeof(int), typeof(bool)])
        ?? throw new MissingMethodException(
            typeof(WeaponComponent).FullName,
            $"{typeof(IWeapon).FullName}.{nameof(IWeapon.FirePosition)}");

    private static void Prefix(WeaponComponent __instance) =>
        __instance.GetComponent<DecoyAmmoFibonacciSlew>()?.PositionOrderIssued();
}

[HarmonyPatch(typeof(WeaponComponent), nameof(WeaponComponent.CallRpcTargetTrack))]
internal static class WeaponComponentTargetTrackFibonacciSlewPatch
{
    private static void Prefix(WeaponComponent __instance) =>
        __instance.GetComponent<DecoyAmmoFibonacciSlew>()?.CancelPositionPattern();
}

[HarmonyPatch(typeof(WeaponComponent), nameof(WeaponComponent.CallRpcStopTracking))]
internal static class WeaponComponentStopTrackingFibonacciSlewPatch
{
    private static void Prefix(WeaponComponent __instance) =>
        __instance.GetComponent<DecoyAmmoFibonacciSlew>()?.CancelPositionPattern();
}

[HarmonyPatch]
internal static class WeaponGroupFibonacciSlewHullManeuverPatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return Find("Ships.IUnmaskingDriver.get_IsActive");
        yield return Find("Ships.IUnmaskingDriver.get_NeedsUnmasking");
        yield return Find("Ships.IFacingDriver.get_IsActive");
    }

    private static void Postfix(WeaponGroup __instance, ref bool __result)
    {
        if (__result && SuppressesHullManeuvering(__instance))
        {
            __result = false;
        }
    }

    private static bool SuppressesHullManeuvering(WeaponGroup group)
    {
        foreach (IWeapon member in group.Members)
        {
            if (member is WeaponComponent weapon &&
                weapon.GetComponent<DecoyAmmoFibonacciSlew>()?.SuppressHullManeuvering == true)
            {
                return true;
            }
        }

        return false;
    }

    private static MethodInfo Find(string methodName) =>
        AccessTools.Method(typeof(WeaponGroup), methodName)
        ?? throw new MissingMethodException(typeof(WeaponGroup).FullName, methodName);
}
