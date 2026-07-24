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

    internal bool CanFireDecoy() => FindCandidates().Any();

    internal bool FireDecoy()
    {
        if (!_ship.isServer)
        {
            return false;
        }

        bool fired = false;
        foreach ((WeaponGroup group, IMagazine source) in FindCandidates())
        {
            int firedByGroup = SpawnChaff(group, source);
            if (firedByGroup <= 0)
            {
                continue;
            }

            fired = true;
            UnityEngine.Debug.Log(
                $"AGMLIB DecoyAmmoSettings: event=forced-fire ship={_ship.name} group={group.GroupKey} ammo={source.AmmoType.SaveKey} count={firedByGroup}");
        }

        return fired;
    }

    private IEnumerable<(WeaponGroup Group, IMagazine Source)> FindCandidates()
    {
        foreach (IWeaponGroup candidate in _ship.WeaponGroups)
        {
            if (candidate is not WeaponGroup group ||
                group.WepType == WeaponType.Decoy ||
                group.FunctioningMemberCount == 0 ||
                group.MixedAmmo ||
                !CanSpawnChaff(group))
            {
                continue;
            }

            IMagazine? source = group.GetAvailableAmmoSources()
                .FirstOrDefault(source =>
                    source.QuantityAvailable > 0 &&
                    IsChaff(source.AmmoType));
            if (source != null)
            {
                yield return (group, source);
            }
        }
    }

    private static bool IsChaff(IMunition? ammo) =>
        ammo is IMissile { IsDecoy: true };

    private static bool CanSpawnChaff(WeaponGroup group) =>
        group.Members.Any(member =>
            member is WeaponComponent { IsFunctional: true } weapon &&
            GetCurrentMuzzle(weapon) != null);

    private static int SpawnChaff(WeaponGroup group, IMagazine source)
    {
        int spawned = 0;
        foreach (IWeapon member in group.Members)
        {
            if (source.QuantityAvailable <= 0)
            {
                break;
            }

            if (member is not WeaponComponent { IsFunctional: true } weapon ||
                GetCurrentMuzzle(weapon) is not Muzzle muzzle)
            {
                continue;
            }

            IMunition chaff = source.AmmoType;
            Vector3 direction = muzzle.transform.forward;
            source.Withdraw(1u);
            NetworkPoolable spawnedChaff = chaff.InstantiateSelf(
                muzzle.transform.position,
                Quaternion.LookRotation(direction),
                direction * chaff.FlightSpeed);
            if (spawnedChaff is ILocalImbued localChaff)
            {
                localChaff.ImbueLocal(((IMuzzleWeapon)weapon).Platform);
                localChaff.SetWeaponReportPath(weapon);
            }

            spawned++;
        }

        return spawned;
    }

    private static Muzzle? GetCurrentMuzzle(WeaponComponent weapon)
    {
        WeaponComponentInternals internals = weapon.Internals();
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
            __result = DecoyAmmoSettings.For(__instance)?.HasAnyDecoys() ?? false;
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
            __result = DecoyAmmoSettings.For(__instance)?.CanFireDecoy() ?? false;
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
        __result |= DecoyAmmoSettings.For(__instance)?.FireDecoy() ?? false;
    }
}
