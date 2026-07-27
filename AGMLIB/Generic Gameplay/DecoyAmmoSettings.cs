using System.Reflection;
using Game.Orders;
using Game.Units;
using HarmonyLib;
using Munitions;
using Ships;
using Ships.Controls;
using UnityEngine;
using Utility;

namespace Lib.Generic_Gameplay;

[DisallowMultipleComponent]
public sealed class DecoyAmmoSettings : MonoBehaviour
{
    [SerializeField, Min(1)]
    private int _maxQueuedRequests = 4;

    private DiscreteWeaponComponent _weapon = null!;
    private ShipController _ship = null!;
    private DecoyAmmoFibonacciSlew _fibonacciSlew = null!;
    private int _pendingRequests;

    /// <summary>
    /// Temporary rollout switch. Once false, only weapons with this component remain enabled.
    /// </summary>
    public static bool EnableGlobally { get; set; } = false;

    internal static IEnumerable<DecoyAmmoSettings> ForShip(ShipController ship)
    {
        if (EnableGlobally)
        {
            foreach (DiscreteWeaponComponent weapon in
                     ship.GetComponentsInChildren<DiscreteWeaponComponent>(true))
            {
                _ = weapon.GetComponent<DecoyAmmoSettings>()
                    ?? weapon.gameObject.AddComponent<DecoyAmmoSettings>();
            }
        }

        foreach (DecoyAmmoSettings settings in
                 ship.GetComponentsInChildren<DecoyAmmoSettings>(true))
        {
            if (settings.TryBind(ship))
            {
                yield return settings;
            }
        }
    }

    private void Awake()
    {
        _weapon = GetComponent<DiscreteWeaponComponent>();
        _ship = GetComponentInParent<ShipController>();
        _fibonacciSlew = GetComponent<DecoyAmmoFibonacciSlew>()
            ?? gameObject.AddComponent<DecoyAmmoFibonacciSlew>();
    }

    internal void TryDrainQueue()
    {
        if (_pendingRequests == 0 ||
            _ship == null ||
            !_ship.isServer ||
            !_weapon.IsFunctional)
        {
            return;
        }

        DiscreteWeaponComponentInternals internals = _weapon.Internals();
        if (internals.Reloading || internals.WaitingForMuzzle)
        {
            return;
        }

        if (!_fibonacciSlew.TryPrepareShot())
        {
            return;
        }

        if (!TryGetChaffSource(out IMagazine? source) ||
            !TryGetNextMuzzle(out Muzzle? muzzle))
        {
            return;
        }

        Muzzle selectedMuzzle = muzzle!;
        IMagazine previousSource = selectedMuzzle.Internals().AmmoSource;
        try
        {
            selectedMuzzle.SetAmmoSource(source);
            selectedMuzzle.Fire();
        }
        finally
        {
            selectedMuzzle.SetAmmoSource(previousSource);
        }

        _pendingRequests--;
        _fibonacciSlew.ShotFired(_pendingRequests > 0);
        internals.MagazineFired++;
        if (internals.MagazineFired >= internals.MagazineSize)
        {
            internals.StartReload();
        }
        else
        {
            internals.WaitingForMuzzle = true;
            internals.MuzzleAccum = internals.RandomlyDeviateMuzzleTime
                ? UnityEngine.Random.Range(-0.5f, 0.5f)
                : 0f;
        }

        UnityEngine.Debug.Log(
            $"AGMLIB DecoyAmmoSettings: event=buffered-fire weapon={_weapon.name} ammo={source!.AmmoType.SaveKey} pending={_pendingRequests}");
    }

    internal bool HasAnyDecoys() =>
        _weapon.IsFunctional &&
        TryGetChaffSource(out _) &&
        HasMuzzles();

    internal bool CanQueueRequest()
    {
        if (!_weapon.IsFunctional ||
            !HasMuzzles() ||
            !TryGetChaffSource(out IMagazine? source))
        {
            return false;
        }

        return _pendingRequests < GetQueueCapacity(source!);
    }

    internal bool QueueRequest()
    {
        if (!_ship.isServer ||
            !_weapon.IsFunctional ||
            !HasMuzzles() ||
            !TryGetChaffSource(out IMagazine? source))
        {
            return false;
        }

        if (_pendingRequests < GetQueueCapacity(source!))
        {
            _pendingRequests++;
            _fibonacciSlew.Begin(source!.AmmoType);
        }

        return true;
    }

    private bool TryBind(ShipController ship)
    {
        _weapon ??= GetComponent<DiscreteWeaponComponent>();
        _ship ??= ship;
        return _weapon != null && _ship == ship;
    }

    private int GetQueueCapacity(IMagazine source) =>
        Mathf.Min(Mathf.Max(1, _maxQueuedRequests), source.QuantityAvailable);

    internal static bool IsChaffAmmo(IMunition? ammo) =>
        ammo is IMissile { IsDecoy: true };

    private bool TryGetChaffSource(out IMagazine? source)
    {
        WeaponGroup? group = _weapon.Group;
        source = group?
            .GetAvailableAmmoSources()
            .FirstOrDefault(candidate =>
                candidate.QuantityAvailable > 0 &&
                IsChaffAmmo(candidate.AmmoType));
        return group is
        {
            WepType: not WeaponType.Decoy,
            MixedAmmo: false,
        } &&
            source != null;
    }

    private bool HasMuzzles() =>
        ((WeaponComponent)_weapon).Internals().Muzzles is { Length: > 0 };

    private bool TryGetNextMuzzle(out Muzzle? muzzle)
    {
        WeaponComponentInternals internals = ((WeaponComponent)_weapon).Internals();
        Muzzle[] muzzles = internals.Muzzles;
        if (muzzles == null || muzzles.Length == 0)
        {
            muzzle = null;
            return false;
        }

        int index = internals.CurrentMuzzle;
        muzzle = muzzles[index];
        internals.CurrentMuzzle = (index + 1) % muzzles.Length;
        return muzzle != null;
    }
}

internal static class ShipControllerDecoyPatchTargets
{
    internal static MethodBase Find(string methodName)
    {
        InterfaceMapping map =
            typeof(ShipController).GetInterfaceMap(typeof(IWarshipOrderReceiver));
        int index = Array.FindIndex(
            map.InterfaceMethods,
            method => method.Name == methodName);
        return index >= 0
            ? map.TargetMethods[index]
            : throw new MissingMethodException(
                typeof(ShipController).FullName,
                $"{typeof(IWarshipOrderReceiver).FullName}.{methodName}");
    }
}

[HarmonyPatch(typeof(ShipController), nameof(ShipController.HasAnyDecoys))]
internal static class ShipControllerHasAnyDecoysSidecarPatch
{
    private static void Postfix(ShipController __instance, ref bool __result)
    {
        if (!__result)
        {
            __result = DecoyAmmoSettings.ForShip(__instance)
                .Any(settings => settings.HasAnyDecoys());
        }
    }
}

[HarmonyPatch]
internal static class ShipControllerCanFireDecoySidecarPatch
{
    private static MethodBase TargetMethod() =>
        ShipControllerDecoyPatchTargets.Find(nameof(IWarshipOrderReceiver.CanFireDecoy));

    private static void Postfix(ShipController __instance, ref bool __result)
    {
        if (!__result)
        {
            __result = DecoyAmmoSettings.ForShip(__instance)
                .Any(settings => settings.CanQueueRequest());
        }
    }
}

[HarmonyPatch]
internal static class ShipControllerFireDecoySidecarPatch
{
    private static MethodBase TargetMethod() =>
        ShipControllerDecoyPatchTargets.Find(nameof(IWarshipOrderReceiver.FireDecoy));

    private static void Postfix(
        ShipController __instance,
        ref bool __result)
    {
        bool queued = false;
        foreach (DecoyAmmoSettings settings in DecoyAmmoSettings.ForShip(__instance))
        {
            queued |= settings.QueueRequest();
        }

        __result |= queued;
    }
}

[HarmonyPatch(typeof(DiscreteWeaponComponent), "RunTimers")]
internal static class DiscreteWeaponComponentRunTimersDecoySidecarPatch
{
    private static void Postfix(DiscreteWeaponComponent __instance)
    {
        __instance.GetComponent<DecoyAmmoFibonacciSlew>()?.UpdatePositionPattern();
        __instance.GetComponent<DecoyAmmoSettings>()?.TryDrainQueue();
    }
}
