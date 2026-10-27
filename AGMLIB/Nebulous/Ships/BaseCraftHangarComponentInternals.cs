using Game.InventoryManagement;

public static partial class NativeInternalsExtensions
{
    public static BaseCraftHangarComponentInternals Internals(this BaseCraftHangarComponent hangar) => new(hangar);
}

public readonly struct BaseCraftHangarComponentInternals(BaseCraftHangarComponent hangar)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<BaseCraftHangarComponent, QuantityChanged?> QuantityChanged =
            AccessTools.FieldRefAccess<BaseCraftHangarComponent, QuantityChanged?>("_onQuantityChanged");
        internal static readonly AccessTools.FieldRef<BaseCraftHangarComponent, StorageChanged?> NewItemTypeAdded =
            AccessTools.FieldRefAccess<BaseCraftHangarComponent, StorageChanged?>("_onNewItemTypeAdded");
        internal static readonly AccessTools.FieldRef<BaseCraftHangarComponent, CraftHangarChanged?> StorageChanged =
            AccessTools.FieldRefAccess<BaseCraftHangarComponent, CraftHangarChanged?>("OnStorageChanged");
    }

    public QuantityChanged? QuantityChanged => Refs.QuantityChanged(hangar);
    public StorageChanged? NewItemTypeAdded => Refs.NewItemTypeAdded(hangar);
    public CraftHangarChanged? StorageChanged => Refs.StorageChanged(hangar);
}
