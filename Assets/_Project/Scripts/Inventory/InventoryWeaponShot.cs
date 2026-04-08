using _Project.Data;
using _Project.State;

namespace _Project.Inventory
{
    public sealed class InventoryWeaponShot
    {
        private readonly InventorySlotQuery _query;
        private readonly InventorySlotMutations _mutations;
        private readonly ItemDatabase _itemDatabase;
        private readonly InventorySlots _slots;
        private readonly InventorySaveNotifier _persistence;

        public InventoryWeaponShot(
            InventorySlotQuery query,
            InventorySlotMutations mutations,
            ItemDatabase itemDatabase,
            InventorySlots slots,
            InventorySaveNotifier persistence)
        {
            _query = query;
            _mutations = mutations;
            _itemDatabase = itemDatabase;
            _slots = slots;
            _persistence = persistence;
        }

        public bool TryGetWeaponDataFromSlot(
            int slotIndex,
            out ItemIdType weaponId,
            out ItemIdType ammoId,
            out int damage)
        {
            weaponId = ItemIdType.None;
            ammoId = ItemIdType.None;
            damage = 0;
            
            InventorySlotData[] slots = _slots.Slots;
            
            if ((uint)slotIndex >= (uint)slots.Length)
                return false;
            
            InventorySlotData slot = slots[slotIndex];
            
            if (!slot.IsUnlocked || slot.IsEmpty)
                return false;
            
            weaponId = slot.Stack.ItemId;
            ItemDefinition definition = _itemDatabase.Get(weaponId);
            
            if (definition.Type != ItemType.Weapon)
                return false;
            
            ammoId = definition.AmmoId;
            damage = definition.Damage;
            
            return true;
        }

        public bool TryConsumeOneAmmo(ItemIdType ammoId, out int ammoSlotIndex)
        {
            ammoSlotIndex = -1;
            
            if (!_query.TryFindFirstAmmoSlot(ammoId, out ammoSlotIndex))
                return false;
            
            if (!_mutations.TryConsumeOneFromAmmoSlot(ammoSlotIndex))
                return false;
            
            _persistence.SaveAndNotify();
            
            return true;
        }
    }
}