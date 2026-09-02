using FleetEditor.CraftEditor;
using Munitions;
using Ships;
using SmallCraft;
using SmallCraft.Components;

namespace Lib.Generic_Gameplay;

/// <summary>
/// Opts a spacecraft prefab into automatic default loadout selection when one of
/// its fixed sockets changes. Attach this to the spacecraft prefab hierarchy.
/// </summary>
[DisallowMultipleComponent]
public sealed class FixedSocketLoadoutSidecar : MonoBehaviour
{
    internal int RepairMissingSelections(
        Spacecraft craft,
        CraftLoadoutSet loadouts,
        IMunitionCollection munitions)
    {
        int repaired = 0;
        foreach (SerializedCraftLoadout loadout in loadouts.AllLoadouts)
        {
            foreach (StaticSpacecraftSocket socket in craft.StaticSockets)
            {
                if (TrySelectDefaultForSocket(craft, socket, loadout, munitions))
                {
                    repaired++;
                }
            }
        }

        return repaired;
    }

    internal bool RepairMissingSelection(
        Spacecraft craft,
        StaticSpacecraftSocket socket,
        SerializedCraftLoadout loadout,
        IMunitionCollection munitions) =>
        TrySelectDefaultForSocket(craft, socket, loadout, munitions);

    private static bool TrySelectDefaultForSocket(
        Spacecraft craft,
        StaticSpacecraftSocket socket,
        SerializedCraftLoadout loadout,
        IMunitionCollection munitions)
    {
        CraftLoadoutMatrix matrix = new(
            craft,
            munitions,
            loadout,
            newLoadout: false);

        LoadoutMatrixSlot[] socketSlots = matrix.ByColumn
            .SelectMany(column => column)
            .Where(slot => slot.SourceSocket == socket)
            .Distinct()
            .ToArray();

        if (socketSlots.Any(slot => slot.InUse))
        {
            return false;
        }

        LoadoutMatrixSlot? selection = socketSlots
            .FirstOrDefault(slot => !slot.Unavailable);
        if (selection == null)
        {
            return false;
        }

        selection.SetInUseEditor(inUse: true);
        UpdateSerializedLoadoutWithSocketKeys(craft, matrix, loadout);
        return true;
    }

    private static void UpdateSerializedLoadoutWithSocketKeys(
        Spacecraft craft,
        CraftLoadoutMatrix matrix,
        SerializedCraftLoadout loadout)
    {
        LoadoutMatrixSlot[] allSlots = matrix.ByColumn
            .SelectMany(column => column)
            .Distinct()
            .ToArray();

        List<SerializedCraftLoadout.GeneralLoadoutElement> elements = new();
        foreach (SpacecraftSocket socket in craft.AllSockets)
        {
            SerializedCraftLoadout.GeneralLoadoutElement? element =
                socket.GenerateLoadout(
                    allSlots.Where(slot => slot.SourceSocket == socket));
            if (element == null)
            {
                continue;
            }

            // Vanilla's default CraftComponent implementation creates a
            // SimpleOccupiedElement without assigning the owning socket key.
            // Correct the generated element only for sidecar-enabled craft.
            element.SocketKey = socket.SocketKey;
            elements.Add(element);
        }

        loadout.UpdateElements(elements.ToArray());
    }
}

[DisallowMultipleComponent]
internal sealed class FixedSocketLoadoutEditorBridge : MonoBehaviour
{
    private LoadoutMatrixEditor? _editor;
    private FixedSocketLoadoutSidecar? _activeSidecar;
    private Spacecraft? _craft;
    private CraftLoadoutSet? _loadouts;
    private Fleet? _fleet;
    private IMunitionCollection? _munitions;
    private bool _refreshPending;

    internal void Bind(
        LoadoutMatrixEditor editor,
        Spacecraft? craft,
        CraftLoadoutSet? loadouts,
        Fleet? fleet,
        IMunitionCollection? munitions)
    {
        Unbind();

        FixedSocketLoadoutSidecar? sidecar = ResolveSidecar(craft);
        if (sidecar == null ||
            !sidecar.enabled ||
            craft == null ||
            loadouts == null ||
            fleet == null ||
            munitions == null)
        {
            return;
        }

        _editor = editor;
        _activeSidecar = sidecar;
        _craft = craft;
        _loadouts = loadouts;
        _fleet = fleet;
        _munitions = munitions;

        // Vanilla subscribes during SetCraft. Binding in its postfix keeps this
        // supplemental observer after native stale-element cleanup.
        _craft.OnStaticSocketChanged += HandleStaticSocketChanged;
        Repair("bind");
    }

    internal void Repair(string trigger)
    {
        if (_activeSidecar == null ||
            _craft == null ||
            _loadouts == null ||
            _munitions == null)
        {
            return;
        }

        int repaired = _activeSidecar.RepairMissingSelections(
            _craft,
            _loadouts,
            _munitions);
        RequestRefreshIfRepaired(repaired, trigger, socketKey: null);
    }

    internal void Unbind()
    {
        if (_craft != null)
        {
            _craft.OnStaticSocketChanged -= HandleStaticSocketChanged;
        }

        _editor = null;
        _activeSidecar = null;
        _craft = null;
        _loadouts = null;
        _fleet = null;
        _munitions = null;
        _refreshPending = false;
    }

    private void LateUpdate()
    {
        if (!_refreshPending)
        {
            return;
        }

        _refreshPending = false;
        if (_editor != null)
        {
            _editor.SetCraft(_craft, _loadouts, _fleet, _munitions);
        }
    }

    private void HandleStaticSocketChanged(Spacecraft craft, string socketKey)
    {
        if (craft != _craft ||
            _activeSidecar == null ||
            _loadouts == null ||
            _munitions == null)
        {
            return;
        }

        StaticSpacecraftSocket? socket = craft.StaticSockets
            .FirstOrDefault(candidate => candidate.SocketKey == socketKey);
        if (socket == null)
        {
            return;
        }

        int repaired = 0;
        foreach (SerializedCraftLoadout loadout in _loadouts.AllLoadouts)
        {
            if (_activeSidecar.RepairMissingSelection(
                    craft,
                    socket,
                    loadout,
                    _munitions))
            {
                repaired++;
            }
        }

        RequestRefreshIfRepaired(repaired, "socket-change", socketKey);
    }

    private void RequestRefreshIfRepaired(
        int repaired,
        string trigger,
        string? socketKey)
    {
        if (repaired == 0 || _craft == null)
        {
            return;
        }

        _refreshPending = true;
        Debug.Log(
            $"AGMLIB FixedSocketLoadoutSidecar: event=repair trigger={trigger} craft={_craft.name} socket={socketKey ?? "all"} repaired={repaired}");
    }

    private static FixedSocketLoadoutSidecar? ResolveSidecar(Spacecraft? craft) =>
        craft == null
            ? null
            : craft.GetComponent<FixedSocketLoadoutSidecar>()
              ?? craft.GetComponentInParent<FixedSocketLoadoutSidecar>()
              ?? craft.GetComponentInChildren<FixedSocketLoadoutSidecar>(includeInactive: true);

    private void OnDestroy() => Unbind();
}

[HarmonyPatch(typeof(LoadoutMatrixEditor), nameof(LoadoutMatrixEditor.SetCraft))]
internal static class LoadoutMatrixEditorFixedSocketLoadoutSidecarPatch
{
    private static void Prefix(LoadoutMatrixEditor __instance) =>
        __instance.GetComponent<FixedSocketLoadoutEditorBridge>()?.Unbind();

    private static void Postfix(
        LoadoutMatrixEditor __instance,
        Spacecraft? craft,
        CraftLoadoutSet? loadouts,
        Fleet? fleet,
        IMunitionCollection? munitions)
    {
        FixedSocketLoadoutEditorBridge bridge =
            __instance.GetComponent<FixedSocketLoadoutEditorBridge>()
            ?? __instance.gameObject.AddComponent<FixedSocketLoadoutEditorBridge>();
        bridge.Bind(__instance, craft, loadouts, fleet, munitions);
    }
}

[HarmonyPatch(typeof(LoadoutMatrixEditor), "RebuildMatrix")]
internal static class LoadoutMatrixEditorRebuildFixedSocketLoadoutSidecarPatch
{
    private static void Postfix(LoadoutMatrixEditor __instance) =>
        __instance.GetComponent<FixedSocketLoadoutEditorBridge>()?
            .Repair("matrix-rebuild");
}
