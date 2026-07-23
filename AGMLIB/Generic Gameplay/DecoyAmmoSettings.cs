using System.Reflection;
using Game.Intel;
using Game.Orders;
using Game.Sensors;
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
    private const int AimPointAttempts = 24;
    private const float AimPointDistance = 10f;

    private ShipController _ship = null!;

    /// <summary>
    /// Temporary rollout switch. Once false, only ships with this component remain enabled.
    /// </summary>
    public static bool EnableGlobally { get; set; } = true;

    internal static DecoyAmmoSettings? For(ShipController ship)
    {
        DecoyAmmoSettings? settings = ship.GetComponent<DecoyAmmoSettings>();
        return settings != null || EnableGlobally
            ? settings ?? ship.gameObject.AddComponent<DecoyAmmoSettings>()
            : null;
    }

    private void Awake() => _ship = GetComponent<ShipController>();

    internal bool HasAnyDecoys() => FindCandidates().Any();

    internal bool CanFireDecoy()
    {
        foreach ((WeaponGroup Group, IMunition Ammo) candidate in FindCandidates())
        {
            if (candidate.Group.SelectedAmmoType != candidate.Ammo ||
                candidate.Group.CheckCanFire(playerOrder: false))
            {
                return true;
            }
        }

        return false;
    }

    internal bool FireDecoy(ITrack? threat)
    {
        if (!_ship.isServer)
        {
            return false;
        }

        bool accepted = false;
        foreach ((WeaponGroup Group, IMunition Ammo) candidate in FindCandidates())
        {
            WeaponGroup group = candidate.Group;
            if (!CanCounter(candidate.Ammo, threat) ||
                !SelectAmmo(group, candidate.Ammo) ||
                !group.CheckCanFire(playerOrder: false) ||
                !Fire(group))
            {
                continue;
            }

            accepted = true;
            UnityEngine.Debug.Log(
                $"AGMLIB DecoyAmmoSettings: event=fire ship={_ship.name} group={group.GroupKey} ammo={candidate.Ammo.SaveKey} target={(threat == null ? "none" : "track")}");
        }

        return accepted;
    }

    private IEnumerable<(WeaponGroup Group, IMunition Ammo)> FindCandidates()
    {
        foreach (IWeaponGroup candidate in _ship.WeaponGroups)
        {
            if (candidate is not WeaponGroup group ||
                group.WepType == WeaponType.Decoy ||
                group.FunctioningMemberCount == 0 ||
                group.MixedAmmo)
            {
                continue;
            }

            IMunition? ammo = FindDecoyAmmo(group);
            if (ammo != null)
            {
                yield return (group, ammo);
            }
        }
    }

    private static IMunition? FindDecoyAmmo(IWeaponGroup group)
    {
        IMunition? selectedAmmo = group.SelectedAmmoType;
        if (IsDecoy(selectedAmmo) && group.AmmoQuantityRemaining(selectedAmmo!) > 0)
        {
            return selectedAmmo;
        }

        if (!group.SupportsAmmoSelection || !group.SupportsImmediateAmmoSwitch)
        {
            return null;
        }

        foreach (IMagazine source in group.GetAvailableAmmoSources())
        {
            IMunition ammo = source.AmmoType;
            if (source.QuantityAvailable > 0 && IsDecoy(ammo))
            {
                return ammo;
            }
        }

        return null;
    }

    private static bool IsDecoy(IMunition? ammo) =>
        ammo is IMissile { IsDecoy: true, DecoyInfo: DecoyCapabilities };

    private static bool CanCounter(IMunition ammo, ITrack? threat)
    {
        if (ammo is not IMissile { DecoyInfo: DecoyCapabilities capabilities })
        {
            return false;
        }

        if (threat?.IntelReport is MissileIntelReport missileReport)
        {
            return capabilities.CanDecoyMissile(missileReport.DecoySeekerMask) > 0;
        }

        return !capabilities.RequireTarget || threat != null;
    }

    private static bool SelectAmmo(IWeaponGroup group, IMunition ammo) =>
        group.SelectedAmmoType == ammo || group.ChangeAmmoType(ammo);

    private static bool Fire(WeaponGroup group)
    {
        if (!group.SupportsPositionTargeting)
        {
            return false;
        }

        if (group.PositionTargetingIsDirectional)
        {
            return group.FirePosition(Vector3.up, -1, playerOrder: false);
        }

        return FireForwardThenSlew(group);
    }

    private static bool FireForwardThenSlew(WeaponGroup group)
    {
        bool accepted = false;
        foreach (IWeapon weapon in group.Members)
        {
            if (!weapon.CanFire || !weapon.SupportsPositionTargeting)
            {
                continue;
            }

            Muzzle? muzzle = GetCurrentMuzzle(weapon);
            if (muzzle == null)
            {
                continue;
            }

            Action? unsubscribe = null;
            if (!TrySubscribeToFired(muzzle, () =>
            {
                unsubscribe?.Invoke();
                weapon.CeaseFire();
                if (TryFindAimPoint(weapon, muzzle, out Vector3 randomAimPoint))
                {
                    weapon.FirePosition(randomAimPoint, -1, playerOrder: false);
                }
            }, out unsubscribe))
            {
                continue;
            }

            Vector3 forwardAimPoint = muzzle.transform.position +
                muzzle.transform.forward * AimPointDistance;
            weapon.FirePosition(forwardAimPoint, -1, playerOrder: false);
            accepted = true;
        }

        return accepted;
    }

    private static bool TrySubscribeToFired(
        Muzzle muzzle,
        Action onFired,
        out Action? unsubscribe)
    {
        switch (muzzle)
        {
            case RezzingMuzzle rezzing:
                Action<NetworkPoolable> rezzingHandler = _ => onFired();
                rezzing.OnFired += rezzingHandler;
                unsubscribe = () => rezzing.OnFired -= rezzingHandler;
                return true;
            case ImmediateLaunchMissileMuzzle missile:
                Action<IMissile> missileHandler = _ => onFired();
                missile.OnFired += missileHandler;
                unsubscribe = () => missile.OnFired -= missileHandler;
                return true;
            default:
                unsubscribe = null;
                return false;
        }
    }

    private static bool TryFindAimPoint(
        IWeapon weapon,
        Muzzle muzzle,
        out Vector3 aimPoint)
    {
        for (int attempt = 0; attempt < AimPointAttempts; attempt++)
        {
            aimPoint = muzzle.transform.position +
                UnityEngine.Random.onUnitSphere * AimPointDistance;
            if (weapon.CanTrainOnTarget(aimPoint) &&
                weapon.IsTargetInRange(aimPoint, Vector3.zero, 0f, strict: true) &&
                !MunitionsHelpers.CheckObstaclesInWay(weapon.AimFromPosition, aimPoint))
            {
                return true;
            }
        }

        aimPoint = default;
        return false;
    }

    private static Muzzle? GetCurrentMuzzle(IWeapon weapon)
    {
        if (weapon is not WeaponComponent weaponComponent)
        {
            return null;
        }

        WeaponComponentInternals internals = weaponComponent.Internals();
        Muzzle[] muzzles = internals.Muzzles;
        int currentMuzzle = internals.CurrentMuzzle;
        return muzzles != null && (uint)currentMuzzle < (uint)muzzles.Length
            ? muzzles[currentMuzzle]
            : null;
    }

}

internal static class ShipControllerDecoyPatchTargets
{
    internal static MethodBase Find(string methodName)
    {
        InterfaceMapping map = typeof(ShipController).GetInterfaceMap(typeof(IWarshipOrderReceiver));
        int index = Array.FindIndex(map.InterfaceMethods, method => method.Name == methodName);
        return index >= 0
            ? map.TargetMethods[index]
            : throw new MissingMethodException(typeof(ShipController).FullName, methodName);
    }
}

[HarmonyPatch(typeof(ShipController), nameof(ShipController.HasAnyDecoys))]
internal static class ShipControllerHasAnyDecoysSidecarPatch
{
    private static void Postfix(ShipController __instance, ref bool __result)
    {
        if (!__result)
        {
            __result = DecoyAmmoSettings.For(__instance)?.HasAnyDecoys() ?? false;
        }
    }
}

[HarmonyPatch]
internal static class ShipControllerCanFireDecoySidecarPatch
{
    private static MethodBase TargetMethod()
    {
        return ShipControllerDecoyPatchTargets.Find(nameof(IWarshipOrderReceiver.CanFireDecoy));
    }

    private static void Postfix(ShipController __instance, ref bool __result)
    {
        if (!__result)
        {
            __result = DecoyAmmoSettings.For(__instance)?.CanFireDecoy() ?? false;
        }
    }
}

[HarmonyPatch]
internal static class ShipControllerFireDecoySidecarPatch
{
    private static MethodBase TargetMethod()
    {
        return ShipControllerDecoyPatchTargets.Find(nameof(IWarshipOrderReceiver.FireDecoy));
    }

    private static void Postfix(ShipController __instance, ITrack __0, ref bool __result)
    {
        bool sidecarAccepted = DecoyAmmoSettings.For(__instance)?.FireDecoy(__0) ?? false;
        __result |= sidecarAccepted;
    }
}
