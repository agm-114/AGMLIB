using Ships;
using Ships.Controls;
using SmallCraft;
using UnityEngine;

public abstract class CraftWorkSlotComponentBase : MonoBehaviour, ICraftWorkSlotProvider
{
    public abstract ICraftWorkSlot[] GetWorkSlots();

    ICraftWorkSlot[] ICraftWorkSlotProvider.WorkSlots => GetWorkSlots();
}

public class LightweightCraftWorkSlotComponent : CraftWorkSlotComponentBase
{
    private sealed class WorkSlot : ICraftWorkSlot
    {
        private readonly LightweightCraftWorkSlotComponent _parent;

        public bool IsFunctional => _parent.IsFunctional;

        public bool CanPreflight => _parent._canPreflight;

        public bool CanPostflight => _parent._canPostflight;

        public bool CanRepair => _parent._canRepair;

        public Spacecraft CurrentlyAssigned { get; set; } = null!;

        public WorkSlot(LightweightCraftWorkSlotComponent parent)
        {
            _parent = parent;
        }

        public bool CanFitCraft(Spacecraft craft)
        {
            return true;
        }
    }

    [Header("Craft Work Slot")]
    [Min(0)]
    [SerializeField]
    private int _fixedWorkSlots = 1;

    [Min(0)]
    [Tooltip("Additional work slots for each tile of the parent hull component.")]
    [SerializeField]
    private int _workSlotsPerScaleUnit = 0;

    [SerializeField]
    private bool _canPreflight = true;

    [SerializeField]
    private bool _canPostflight = true;

    [SerializeField]
    private bool _canRepair = false;

    private HullPart? _functionalParent;

    private ICraftWorkSlot[]? _workSlots;

    public override ICraftWorkSlot[] GetWorkSlots()
    {
        if (_workSlots == null)
        {
            int workSlotCount = Mathf.Max(0, _fixedWorkSlots)
                + (Mathf.Max(0, _workSlotsPerScaleUnit) * GetParentScale());
            _workSlots = new ICraftWorkSlot[workSlotCount];
            for (int i = 0; i < workSlotCount; i++)
            {
                _workSlots[i] = new WorkSlot(this);
            }
        }

        return _workSlots;
    }

    private bool IsFunctional => _functionalParent != null && _functionalParent.IsDoingWork;

    private int GetParentScale() => _functionalParent?.TileSizeMultiplier ?? 1;

    private void Awake()
    {
        _functionalParent = GetComponentInParent<HullPart>();
    }
}
