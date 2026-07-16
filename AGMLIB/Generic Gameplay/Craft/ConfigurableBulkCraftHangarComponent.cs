using System.Collections.Generic;
using System.Linq;
using Ships;
using Ships.Controls;
using UnityEngine;

public class ConfigurableBulkCraftHangarComponent : BulkCraftHangarComponent, ICraftWorkSlotProvider
{
    [Header("Craft Work Slots")]
    [SerializeField]
    private List<CraftWorkSlotComponentBase> _workSlotProviders = new();

    ICraftWorkSlot[] ICraftWorkSlotProvider.WorkSlots => [.. _workSlotProviders.SelectMany(provider => provider.GetWorkSlots())];
}