using Game.Docking;
using Game.InventoryManagement;

namespace Lib.Generic_Gameplay;

/// <summary>Station-style component repairs at every replenishment berth of the owning ship.</summary>
[DisallowMultipleComponent]
public class ShipRepairFacility : MonoBehaviour
{
    [Range(0f, 1f), Tooltip("Station-style component repair allowance. Zero disables this facility.")]
    public float ComponentMaxRepair = 0.5f;
    [Range(0f, 1f), Tooltip("Additional damage-control repair speed: 0.75 means +75%.")]
    public float ComponentRepairRateBonus = 0.75f;

    internal bool IsAvailable(ShipController ship)
    {
        HullComponent? component = GetComponentInParent<HullComponent>();
        return isActiveAndEnabled && GetComponentInParent<ShipController>() == ship
            && ComponentMaxRepair > 0f
            && (component == null || component.IsFunctional && !component.IsDestroyed);
    }
}

[DisallowMultipleComponent]
internal class ShipRepairService : MonoBehaviour
{
    private class BerthRepair(ReplenishmentDockingPoint point) : IModifierSource
    {
        private ShipController? _ship;
        private IReadOnlyStat? _repair;
        private float _rate;
        public string SourceName => point.BerthName;

        internal void Refresh(IReadOnlyStat? repair, float rate)
        {
            ShipController? ship = point != null && point.isActiveAndEnabled
                && point.DockingEnabled && !point.CurrentlyUndocking ? point.DockedShip : null;
            if (ship != null && (!ReferenceEquals(ship.DockedWith, point) || ship.Ship == null || ship.DCDispatcher == null))
                ship = null;
            if (_ship == ship && ReferenceEquals(_repair, repair) && _rate == rate)
                return;
            Clear();
            if (ship == null || repair == null)
                return;
            _ship = ship;
            _repair = repair;
            _rate = rate;
            // As with native stations, each peer derives stats locally; native DC owns healing.
            ship.Ship.Hull.SetDockedStationRepairStat(repair);
            ship.Ship.AddStatModifier(this, new StatModifier("damagecontrol-repairspeed", 0f, rate));
            ship.DCDispatcher.ForceUpdate();
        }

        internal void Clear()
        {
            if (_ship != null && _ship.Ship != null)
            {
                if (ReferenceEquals(_ship.DockedWith, point) || _ship.DockedWith == null)
                    _ship.Ship.Hull.SetDockedStationRepairStat(null);
                _ship.Ship.RemoveStatModifier(this, "damagecontrol-repairspeed");
                _ship.DCDispatcher?.ForceUpdate();
            }
            _ship = null;
            _repair = null;
            _rate = 0f;
        }
    }

    private ShipController _ship = null!;
    private readonly List<ShipRepairFacility> _facilities = [];
    private readonly List<ReplenishmentDockingPoint> _points = [];
    private readonly Dictionary<ReplenishmentDockingPoint, BerthRepair> _repairs = [];
    private readonly List<ReplenishmentDockingPoint> _removedPoints = [];
    private float _nextDiscovery;
    private LiteralStatValue? _maxRepair;
    private float _rate;
    private StorageContainerPool? _emptyRestoreStorage;

    private void Awake() => _ship = GetComponent<ShipController>();
    private void OnEnable() => _nextDiscovery = 0f;

    internal IReadOnlyStat? MaxRepair { get { RefreshFacilities(); return _maxRepair; } }
    internal StatModifier? RepairRate => MaxRepair == null ? null : new("damagecontrol-repairspeed", 0f, _rate);
    internal StorageContainerPool EmptyRestoreStorage => _emptyRestoreStorage ??= new(
        "Repair Facility Restore Pool",
        [new SingleItemContainer(_ship, 65532, "Repair Facility Restores", GeneralItem.Restore, 0, 0)],
        65532, equalize: false);

    internal static ShipRepairService? For(ReplenishmentDockingPoint point) => For(point.GetComponentInParent<ShipController>());

    internal static ShipRepairService? For(ShipController? ship)
    {
        if (ship == null)
            return null;
        ShipRepairService? service = ship.GetComponent<ShipRepairService>();
        if (service != null)
            return service;
        if (!ship.GetComponentsInChildren<ShipRepairFacility>(true).Any(facility => facility.GetComponentInParent<ShipController>() == ship))
            return null;
        service = ship.gameObject.AddComponent<ShipRepairService>();
        service._ship = ship;
        return service;
    }

    private void RefreshFacilities()
    {
        if (Time.unscaledTime >= _nextDiscovery)
        {
            // Search the controller root, including installed components and inactive children.
            _ship.GetComponentsInChildren(true, _facilities);
            _ship.GetComponentsInChildren(true, _points);
            _nextDiscovery = Time.unscaledTime + 0.25f;
        }
        float repair = 0f;
        float rate = 0f;
        foreach (ShipRepairFacility facility in _facilities)
        {
            if (facility == null || !facility.IsAvailable(_ship))
                continue;
            repair = Mathf.Max(repair, Mathf.Clamp01(facility.ComponentMaxRepair));
            rate = Mathf.Max(rate, Mathf.Clamp01(facility.ComponentRepairRateBonus));
        }
        if ((_maxRepair?.Value ?? 0f) != repair)
            _maxRepair = repair > 0f ? new LiteralStatValue(StatTable.GetStatInfo("hull-component-max-repair", out _), repair, 0f) : null;
        _rate = rate;
    }

    private void Update() => RefreshDockedShips();

    internal void RefreshDockedShips()
    {
        RefreshFacilities();
        foreach (ReplenishmentDockingPoint point in _points)
        {
            if (point == null || point.GetComponentInParent<ShipController>() != _ship)
                continue;
            if (!_repairs.TryGetValue(point, out BerthRepair state))
                _repairs.Add(point, state = new(point));
            state.Refresh(_maxRepair, _rate);
        }
        _removedPoints.Clear();
        foreach (var entry in _repairs)
        {
            if (entry.Key != null && _points.Contains(entry.Key) && entry.Key.GetComponentInParent<ShipController>() == _ship)
                continue;
            entry.Value.Clear();
            _removedPoints.Add(entry.Key!);
        }
        _removedPoints.ForEach(point => _repairs.Remove(point));
    }

    private void OnDisable()
    {
        foreach (BerthRepair state in _repairs.Values)
            state.Clear();
        _repairs.Clear();
    }

    internal static void DockedChanged(IDockingPoint? oldDocked, IDockingPoint? newDocked)
    {
        if (oldDocked is ReplenishmentDockingPoint oldPoint)
            For(oldPoint)?.RefreshDockedShips();
        if (newDocked is ReplenishmentDockingPoint newPoint)
            For(newPoint)?.RefreshDockedShips();
    }
}

[HarmonyPatch(typeof(ReplenishmentDockingPoint), nameof(ReplenishmentDockingPoint.MaxComponentRepair), MethodType.Getter)]
internal static class TenderMaxComponentRepairPatch
{
    private static void Postfix(ReplenishmentDockingPoint __instance, ref IReadOnlyStat? __result) =>
        __result ??= ShipRepairService.For(__instance)?.MaxRepair;
}

[HarmonyPatch(typeof(ReplenishmentDockingPoint), nameof(ReplenishmentDockingPoint.ComponentRepairRateBonus), MethodType.Getter)]
internal static class TenderRepairRatePatch
{
    private static void Postfix(ReplenishmentDockingPoint __instance, ref StatModifier? __result) =>
        __result ??= ShipRepairService.For(__instance)?.RepairRate;
}

[HarmonyPatch(typeof(ReplenishmentDockingPoint), nameof(ReplenishmentDockingPoint.RepairServiceAvailable), MethodType.Getter)]
internal static class TenderRepairAvailabilityPatch
{
    private static void Postfix(ReplenishmentDockingPoint __instance, ref bool __result)
    {
        ShipRepairService? service = ShipRepairService.For(__instance);
        if (service == null)
            return;
        ShipController ship = __instance.GetComponentInParent<ShipController>();
        __result = service.MaxRepair != null || __result && !ReferenceEquals(ship.RestoreStoragePool, service.EmptyRestoreStorage);
    }
}

[HarmonyPatch(typeof(ShipController), nameof(ShipController.RestoreStoragePool), MethodType.Getter)]
internal static class TenderRepairRestoreStoragePatch
{
    // Native repair UI requires a storage container even for a repair-only tender with no lockers.
    private static void Postfix(ShipController __instance, ref StorageContainerPool? __result) =>
        __result ??= ShipRepairService.For(__instance)?.EmptyRestoreStorage;
}

[HarmonyPatch(typeof(ShipController), "SyncDockedWith")]
internal static class TenderDockedRepairPatch
{
    private static void Postfix(IDockingPoint? oldDocked, IDockingPoint? newDocked) =>
        ShipRepairService.DockedChanged(oldDocked, newDocked);
}
