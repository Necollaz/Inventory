using System;
using _Project.Data;

namespace _Project.Inventory
{
    public sealed class InventoryFacade
    {
        public event Action InventoryChanged
        {
            add => _persistence.InventoryChanged += value;
            remove => _persistence.InventoryChanged -= value;
        }
        
        private readonly InventorySlots _slots;
        private readonly InventorySaveNotifier _persistence;
        private readonly InventorySlotQuery _query;
        private readonly InventorySlotUnlock _slotUnlock;
        private readonly InventoryStackAdd _stackAdd;
        private readonly InventoryWeaponShot _weaponShot;
        private readonly InventorySlotDragDropRules _slotDragDropRules;
        private readonly InventoryWeightCalculator _weightCalculator;
        
        public InventoryFacade(
            InventorySlots slots,
            InventorySaveNotifier persistence,
            InventorySlotQuery query,
            InventorySlotUnlock slotUnlock,
            InventoryStackAdd stackAdd,
            InventoryWeaponShot weaponShot,
            InventorySlotDragDropRules slotDragDropRules,
            InventoryWeightCalculator weightCalculator)
        {
            _slots = slots;
            _persistence = persistence;
            _query = query;
            _slotUnlock = slotUnlock;
            _stackAdd = stackAdd;
            _weaponShot = weaponShot;
            _slotDragDropRules = slotDragDropRules;
            _weightCalculator = weightCalculator;
        }
        
        public int TotalSlotCount => _slots.TotalSlotCount;
        
        public bool TryFindNextLockedSlot(out int slotIndex) => _query.TryFindNextLockedSlot(out slotIndex);
        
        public bool TryUnlockSlot(int slotIndex) => _slotUnlock.TryUnlockSlot(slotIndex);
        
        public bool TryPlaceNewStackIntoFirstEmptyUnlockedSlot(ItemIdType itemId, int amount, out int slotIndex) =>
            _stackAdd.TryPlaceNewStackIntoFirstEmptyUnlockedSlot(itemId, amount, out slotIndex);
        
        public bool TryConsumeOneAmmo(ItemIdType ammoId, out int ammoSlotIndex) =>
            _weaponShot.TryConsumeOneAmmo(ammoId, out ammoSlotIndex);
        
        public bool TryClearSlotAndGetRemovedData(int slotIndex, out ItemIdType itemId, out int amount) =>
            _stackAdd.TryClearSlotAndGetRemovedData(slotIndex, out itemId, out amount);
        
        public bool TryApplyDragDrop(int fromSlotIndex, int toSlotIndex) =>
            _slotDragDropRules.TryApplyDragDrop(fromSlotIndex, toSlotIndex);
        
        public bool TryAddStackableItem(
            ItemIdType itemId,
            int amount,
            int[] usedSlotIndicesBuffer,
            int[] addedAmountsBuffer,
            out int usedSlotCount,
            out bool inventoryFull) => 
            _stackAdd.TryAddStackableItem(
                itemId,
                amount,
                usedSlotIndicesBuffer,
                addedAmountsBuffer,
                out usedSlotCount,
                out inventoryFull);
        
        public bool TryGetWeaponDataFromSlot(
            int slotIndex,
            out ItemIdType weaponId,
            out ItemIdType ammoId,
            out int damage) =>
            _weaponShot.TryGetWeaponDataFromSlot(slotIndex, out weaponId, out ammoId, out damage);
        
        public int FindAllWeapons(int[] resultIndicesBuffer) => _query.FindAllWeapons(resultIndicesBuffer);
        
        public int FindAllNonEmptySlots(int[] resultIndicesBuffer) => _query.FindAllNonEmptySlots(resultIndicesBuffer);
        
        public float CalculateTotalWeight() => _weightCalculator.CalculateTotalWeight();
    }
}