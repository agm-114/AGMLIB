using Game.Sensors;
using Munitions;
using SmallCraft;
using SmallCraft.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Lib.Craft
{
    public class AdvancedCraftMissileComponent : CraftMissileComponent
    {


        public override void FireFromSlot(BaseCraftMissileSlot slot, ITrack target, int salvoId, Action<IMissile> onFired)
        {
            slot.Fire(target, salvoId, onFired);
        }



        public override LoadoutMatrixRow[] GetLoadoutMatrixRows(CraftLoadoutMatrix matrix, SpacecraftSocket socket, AvailableMunitionsSet fleetMunitions, SerializedCraftLoadout.GeneralLoadoutElement existing, bool newLoadout)
        {
            SerializedCraftLoadout.MissileSelection existingMissile = existing as SerializedCraftLoadout.MissileSelection;

            // Use a dictionary to cache the filtered munition types for each tag.
            // This avoids redundant lookups if multiple slots use the same filter.
            Dictionary<MunitionTags, IEnumerable<IMunition>> taggedMunitionsCache = new Dictionary<MunitionTags, IEnumerable<IMunition>>();

            LoadoutMatrixRow[] array = _missileSlots.SelectMany((BaseCraftMissileSlot baseCraftMissileSlot, int i) =>
            {
                // Determine the ammo tag for the current slot.
                // If the index 'i' is out of bounds for _ammoTags, use the last one.
                MunitionTags currentAmmoTag = (i < _ammoTags.Length) ? _ammoTags[i] : _ammoTags.LastOrDefault();

                // Get the list of munitions that match the current tag.
                // Use the cache to avoid re-filtering if the tag has been seen before.
                if (!taggedMunitionsCache.TryGetValue(currentAmmoTag, out IEnumerable<IMunition> types))
                {
                    types = fleetMunitions.GetAllMunitionsWithTags(new[] { currentAmmoTag })
                                          .Where(type => type is IMissile missile && !missile.IsDecoy);
                    taggedMunitionsCache[currentAmmoTag] = types;
                }

                // Create a LoadoutMatrixMissile row for each valid munition type.
                return types.Select(type => new LoadoutMatrixMissile
                {
                    MissileType = (type as IMissile),
                    Slots = new[] {
                new LoadoutMatrixSlot(
                    matrix,
                    socket,
                    this,
                    baseCraftMissileSlot.LoadoutSlots,
                    existingMissile != null && i < existingMissile.MissileKeys.Length && existingMissile.MissileKeys[i] == type.SaveKey
                )
            }
                });
            }).ToArray();

            // The rest of the logic for pairing slots remains the same.
            foreach (LoadoutMatrixRow row in array)
            {
                LoadoutMatrixSlot[] slots = row.Slots;
                foreach (LoadoutMatrixSlot slot in slots)
                {
                    // Note: This logic assumes each row will now only have one slot.
                    // You may need to adjust this depending on the intended behavior.
                    slot.PairedSlots = row.Slots.Where((LoadoutMatrixSlot x) => x != slot).ToArray();
                    slot.PairedSlotsMirrorInUse = false;
                }
            }

            return array;

            /*

            IEnumerable<IMunition> types = fleetMunitions.GetAllMunitionsWithTags(_ammoTags);
            types = types.Where(type => type is IMissile missile && !missile.IsDecoy);
            SerializedCraftLoadout.MissileSelection existingMissile = existing as SerializedCraftLoadout.MissileSelection;

            LoadoutMatrixRow[] array = types.Select(types => new LoadoutMatrixMissile
            {
                MissileType = (types as IMissile),
                Slots = _missileSlots.Select((BaseCraftMissileSlot baseCraftMissileSlot, int i) => new LoadoutMatrixSlot(matrix, socket, this, baseCraftMissileSlot.LoadoutSlots, existingMissile != null && i < existingMissile.MissileKeys.Length && existingMissile.MissileKeys[i] == types.SaveKey)).ToArray()
            }).ToArray();

;
            foreach (LoadoutMatrixRow row in array)
            {
                LoadoutMatrixSlot[] slots = row.Slots;
                foreach (LoadoutMatrixSlot slot in slots)
                {
                    slot.PairedSlots = row.Slots.Where((LoadoutMatrixSlot x) => x != slot).ToArray();
                    slot.PairedSlotsMirrorInUse = false;
                }
            }
            return array;
            */
        }
    }
}
