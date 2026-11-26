using System.Reflection;
using System.Runtime.CompilerServices;
using SmallCraft;

/// <summary>A native craft position belonging to a deck with overlapping, differently sized positions.</summary>
public class MultiCraftLandingPad : ElevatorCraftLandingPad
{
    [Header("Shared Deck")]
    [SerializeField, Tooltip("Optional child positions owned by this deck. Leave empty to use direct child pads in hierarchy order.")]
    private List<MultiCraftLandingPad> _positions = [];
    [SerializeField, Min(0), Tooltip("Leave at zero for independent child positions and a whole-deck parent. For custom overlaps, list the positions and give every pad a positive bitmask.")]
    private int _occupiedSpaces = 0;

    private MultiCraftLandingPad? _deck;
    private MultiCraftLandingPad[]? _layout;
    private bool _sharedElevator;
    internal bool HoldingElevatorOpen { get; set; }

    internal MultiCraftLandingPad Deck => _deck ?? ResolveDeck();
    internal IReadOnlyList<MultiCraftLandingPad> Positions => Deck._layout ??= Deck.BuildLayout();

    private MultiCraftLandingPad ResolveDeck()
    {
        var parent = transform.parent != null ? transform.parent.GetComponent<MultiCraftLandingPad>() : null;
        return _positions.Count == 0 && parent != null && parent._deck == null
            && (parent._positions.Count == 0 || parent._positions.Contains(this)) ? parent : this;
    }

    protected override void Awake()
    {
        BindListedPositions();
        base.Awake();
    }

    private void BindListedPositions()
    {
        foreach (var child in _positions)
        {
            if (child == null || child == this || child._positions.Count != 0 || (child._deck != null && child._deck != this))
                throw new InvalidOperationException($"Check {name}'s child position list for missing pads, nested decks, or multiple parents.");
            child._deck = this;
        }
    }

    private MultiCraftLandingPad[] BuildLayout()
    {
        if (_positions.Count > 0)
        {
            BindListedPositions();
            return [this, .. _positions];
        }
        List<MultiCraftLandingPad> positions = [this];
        foreach (Transform child in transform)
        {
            if (!child.TryGetComponent(out MultiCraftLandingPad pad) || pad.Deck != this)
                continue;
            pad._deck = this;
            positions.Add(pad);
        }
        return [.. positions];
    }
    private bool InUse => ((ICraftEntryExitPoint)this).IsReserved || OperationInProgress;
    private bool UsesSpaceMasks => _positions.Count != 0 && _occupiedSpaces > 0;
    internal bool HasConflict
    {
        get
        {
            var deck = Deck;
            if (deck.UsesSpaceMasks)
                return Positions.Any(p => p != this && (_occupiedSpaces & p._occupiedSpaces) != 0 && p.InUse);
            return deck == this ? Positions.Any(p => p != this && p.InUse) : deck.InUse;
        }
    }

    private void ValidateDeck()
    {
        MultiCraftLandingPad deck = Deck;
        if (deck._deck != null)
            throw new InvalidOperationException($"Multi-craft deck {deck.name} needs one root.");
        HashSet<MultiCraftLandingPad> seen = [];
        HashSet<Animator> animations = [];
        var elevator = deck.Internals().Animations;
        foreach (MultiCraftLandingPad position in deck.Positions)
        {
            if (position == null || !seen.Add(position) || position.Deck != deck || (position != deck && position._positions.Count != 0))
                throw new InvalidOperationException($"Multi-craft deck {deck.name} has a missing, duplicate, or incorrectly owned position.");
            var native = position.Internals();
            bool usesElevator = native.Animations == null || native.Animations == elevator;
            if ((deck.UsesSpaceMasks && position._occupiedSpaces <= 0) || native.Part == null || native.Part != deck.Internals().Part
                || elevator == null || (!usesElevator && !animations.Add(native.Animations!)) || position.PadAnchor == null
                || position.PadDimensions.x <= 0 || position.PadDimensions.y <= 0 || position.PadDimensions.z <= 0)
                throw new InvalidOperationException($"Check {position.name}'s dimensions, space mask, shared hull part, deck Animator, and launch anchor.");
            if (usesElevator && position != deck)
            {
                position._sharedElevator = deck._sharedElevator = true;
                native.BindElevator(deck);
            }
        }
        if (deck._sharedElevator)
            deck.Internals().BindElevator(deck);
        if (!seen.Contains(this))
            throw new InvalidOperationException($"Multi-craft position {name} is missing from its deck's position list.");
    }

    internal static ICraftEntryExitPoint[] Expand(IEnumerable<ICraftEntryExitPoint> pads)
    {
        List<ICraftEntryExitPoint> expanded = pads.Distinct().ToList();
        HashSet<ICraftEntryExitPoint> seen = [.. expanded];
        HashSet<MultiCraftLandingPad> decks = [];
        foreach (MultiCraftLandingPad position in expanded.OfType<MultiCraftLandingPad>().ToArray())
        {
            if (!decks.Add(position.Deck))
                continue;
            position.ValidateDeck();
            foreach (MultiCraftLandingPad sibling in position.Positions)
                if (seen.Add(sibling))
                    expanded.Add(sibling);
        }
        return [.. expanded];
    }

    internal static void AssignIndices(IEnumerable<CraftLandingPad> positions)
    {
        // Native indices are local to a hull part. All positions must exist on both peers before registration.
        int index = 0;
        foreach (CraftLandingPad pad in positions)
            pad.SetIndex(checked((ushort)index++));
    }

    internal static int CorrectCapacity(IEnumerable<ICraftEntryExitPoint> pads, Spacecraft craft, bool exactFit, int nativeCount)
    {
        foreach (IGrouping<MultiCraftLandingPad, MultiCraftLandingPad> deck in pads.OfType<MultiCraftLandingPad>().GroupBy(pad => pad.Deck))
        {
            var fitting = deck.Where(pad => exactFit ? pad.PadSize == craft.RequiredPadSize : pad.CanFitCraft(craft)).ToArray();
            nativeCount += deck.Key.CompatibleCapacity(fitting) - fitting.Length;
        }
        return nativeCount;
    }

    internal int CompatibleCapacity(IReadOnlyList<MultiCraftLandingPad> fitting) => !UsesSpaceMasks
        ? Math.Max(fitting.Count(p => p != this), fitting.Contains(this) ? 1 : 0)
        : MaximumCompatiblePositions(fitting.Select(p => p._occupiedSpaces).Distinct().ToArray());

    private static int MaximumCompatiblePositions(IReadOnlyList<int> masks, int index = 0, int occupied = 0)
    {
        int best = 0;
        for (; masks.Count - index > best; index++)
            if ((occupied & masks[index]) == 0)
                best = Math.Max(best, 1 + MaximumCompatiblePositions(masks, index + 1, occupied | masks[index]));
        return best;
    }

    internal static bool BlockAdditionalTraffic(bool started, ICraftEntryExitPoint? previous, ICraftEntryExitPoint? candidate) =>
        started && !(previous is MultiCraftLandingPad first && candidate is MultiCraftLandingPad next
            && first != next && first.Deck == next.Deck && !next.HasConflict);

    public override void Vaporize()
    {
        foreach (MultiCraftLandingPad position in Positions)
            position.VaporizePosition();
    }

    private void VaporizePosition()
    {
        base.Vaporize();
        if (HoldingElevatorOpen)
            SetOpenState(false);
    }

    internal bool ApplyElevatorRequest(ref bool open)
    {
        if (!_sharedElevator)
            return true;
        HoldingElevatorOpen = open;
        open = open || Positions.Any(p => p != null && p.HoldingElevatorOpen);
        var native = this.Internals();
        return native.Animations != null && native.Animations.GetBool(native.OpenControl) != open;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (HoldingElevatorOpen)
            SetOpenState(false);
    }
}

[HarmonyPatch(typeof(CraftLandingPad))]
internal static class MultiCraftElevatorPatch
{
    // This is the native RPC receiver on the host and clients. Each position releases only its own request.
    [HarmonyPrefix, HarmonyPatch(nameof(CraftLandingPad.SetOpenState))]
    private static bool SetOpen(CraftLandingPad __instance, ref bool open) =>
        __instance is not MultiCraftLandingPad pad || pad.ApplyElevatorRequest(ref open);

    [HarmonyPostfix, HarmonyPatch("Utility.IBulkSaveComponent.WriteComponentState")]
    private static void Save(CraftLandingPad __instance, System.Xml.XmlElement self)
    {
        if (__instance is MultiCraftLandingPad { HoldingElevatorOpen: true })
            self.SetAttribute("AgmlibElevatorOpen", "1");
    }

    [HarmonyPrefix, HarmonyPatch("Utility.IBulkSaveComponent.LoadComponentState")]
    private static void Load(CraftLandingPad __instance, System.Xml.XmlElement self)
    {
        if (__instance is MultiCraftLandingPad pad)
            pad.HoldingElevatorOpen = self.GetAttribute("AgmlibElevatorOpen") == "1";
    }
}

[HarmonyPatch]
internal static class MultiCraftLandingPadAvailabilityPatch
{
    private static IEnumerable<MethodBase> TargetMethods() =>
        new[] { nameof(CraftLandingPad.AvailableForImmediateUse), "get_IsUseable" }
            .Select(name => AccessTools.Method(typeof(CraftLandingPad), name));

    private static void Postfix(CraftLandingPad __instance, ref bool __result)
    {
        if (__result && __instance is MultiCraftLandingPad pad)
            __result = pad.isActiveAndEnabled && pad.Deck.isActiveAndEnabled && !pad.HasConflict;
    }
}

[HarmonyPatch]
internal static class MultiCraftLandingPadHangarPatch
{
    private static IEnumerable<MethodBase> TargetMethods() =>
        new[] { typeof(SingleCraftHangarComponent), typeof(MultiCraftHangarComponent), typeof(MultiPadBulkCraftHangarComponent) }
            .Select(type => AccessTools.PropertyGetter(type, nameof(ICraftHangar.LandingPads)));

    private static readonly ConditionalWeakTable<ICraftEntryExitPoint[], ICraftEntryExitPoint[]> Expanded = new();

    private static void Postfix(ref ICraftEntryExitPoint[] __result)
    {
        if (__result != null)
            __result = Expanded.GetValue(__result, pads => pads.Any(pad => pad is MultiCraftLandingPad) ? MultiCraftLandingPad.Expand(pads) : pads);
    }
}

[HarmonyPatch(typeof(CraftCarrierController), nameof(CraftCarrierController.Initialize))]
internal static class MultiCraftLandingPadRegistrationPatch
{
    private static void Prefix(List<ICraftHangar> hangars, ref List<ICraftEntryExitPoint> standalonePads)
    {
        if (standalonePads.Any(pad => pad is MultiCraftLandingPad))
            standalonePads = [.. MultiCraftLandingPad.Expand(standalonePads)];
        foreach (var part in standalonePads.Concat(hangars.SelectMany(hangar => hangar.LandingPads ?? []))
            .OfType<CraftLandingPad>().GroupBy(pad => pad.Internals().Part))
            if (part.Any(pad => pad is MultiCraftLandingPad))
                MultiCraftLandingPad.AssignIndices(part);
    }
}

// Only widen the pad-size test inside hangar admission. Traffic still tests the individual position.
[HarmonyPatch]
internal static class MultiCraftHangarAdmissionPatch
{
    private static IEnumerable<MethodBase> TargetMethods() =>
        new[] { typeof(SingleCraftHangarComponent), typeof(MultiCraftHangarComponent), typeof(MultiPadBulkCraftHangarComponent) }
            .Select(type => AccessTools.Method(type, nameof(ICraftHangar.CanFitCraft)));

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        bool bulk = original.DeclaringType == typeof(MultiPadBulkCraftHangarComponent);
        MethodInfo singleFit = AccessTools.Method(typeof(CraftLandingPad), nameof(CraftLandingPad.CanFitCraft));
        MethodInfo replacement = AccessTools.Method(typeof(MultiCraftHangarAdmissionPatch), bulk ? nameof(AnyDeckFits) : nameof(DeckFits));
        int matches = 0;
        foreach (CodeInstruction instruction in instructions)
        {
            if (bulk ? instruction.operand is MethodInfo { DeclaringType: var type, Name: nameof(Enumerable.Any) } && type == typeof(Enumerable)
                     : instruction.Calls(singleFit))
            {
                instruction.opcode = System.Reflection.Emit.OpCodes.Call;
                instruction.operand = replacement;
                matches++;
            }
            yield return instruction;
        }
        if (matches != 1)
            throw new InvalidOperationException($"Expected one pad fit test in {original}.");
    }

    private static bool DeckFits(CraftLandingPad pad, Spacecraft craft) =>
        pad.CanFitCraft(craft) || pad is MultiCraftLandingPad shared && shared.Positions.Any(position => position.CanFitCraft(craft));

    private static bool AnyDeckFits(IEnumerable<CraftLandingPad> pads, Func<CraftLandingPad, bool> fits) =>
        pads.Any(pad => fits(pad) || pad is MultiCraftLandingPad shared && shared.Positions.Any(fits));
}

[HarmonyPatch(typeof(CraftCarrierController))]
internal static class MultiCraftLandingPadCapacityPatch
{
    [HarmonyPostfix, HarmonyPatch(nameof(CraftCarrierController.MaxSingleCycleLaunch))]
    private static void Maximum(CraftCarrierController __instance, Spacecraft craft, ref int __result) =>
        __result = MultiCraftLandingPad.CorrectCapacity(__instance.Internals().SharedPads, craft, false, __result);

    [HarmonyPostfix, HarmonyPatch(nameof(CraftCarrierController.BaseSingleCycleLaunch))]
    private static void Base(CraftCarrierController __instance, Spacecraft craft, bool exactFit, ref int __result) =>
        __result = MultiCraftLandingPad.CorrectCapacity(__instance.Internals().SharedPads, craft, exactFit, __result);
}
