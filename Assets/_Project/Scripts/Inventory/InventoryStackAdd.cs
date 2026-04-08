using UnityEngine;
using _Project.Data;

namespace _Project.Inventory
{
    public sealed class InventoryStackAdd
    {
        private readonly InventorySlots _slots;
        private readonly ItemDatabase _itemDatabase;
        private readonly InventorySlotQuery _query;
        private readonly InventorySlotMutations _mutations;
        private readonly InventorySaveNotifier _persistence;
        
        private int[] _nonFullStackIndicesBuffer;

        public InventoryStackAdd(
            InventorySlots slots,
            ItemDatabase itemDatabase,
            InventorySlotQuery query,
            InventorySlotMutations mutations,
            InventorySaveNotifier persistence)
        {
            _slots = slots;
            _itemDatabase = itemDatabase;
            _query = query;
            _mutations = mutations;
            _persistence = persistence;
        }
        
        public bool TryClearSlotAndGetRemovedData(int slotIndex, out ItemIdType removedItemId, out int removedAmount)
        {
            if (!_mutations.TryClearSlot(slotIndex, out removedItemId, out removedAmount))
                return false;
            
            _persistence.SaveAndNotify();
            
            return true;
        }

        public bool TryPlaceNewStackIntoFirstEmptyUnlockedSlot(ItemIdType itemId, int amount, out int slotIndex)
        {
            slotIndex = -1;
            
            if (itemId == ItemIdType.None || amount <= 0)
                return false;
            
            if (!_query.TryFindFirstEmptyUnlockedSlot(out slotIndex))
                return false;
            
            if (!_mutations.TryPlaceNewStack(slotIndex, itemId, amount))
                return false;
            
            _persistence.SaveAndNotify();
            
            return true;
        }

        public bool TryAddStackableItem(
            ItemIdType itemId,
            int amount,
            int[] usedSlotIndicesBuffer,
            int[] addedAmountsBuffer,
            out int usedSlotCount,
            out bool inventoryFull)
        {
            usedSlotCount = 0;
            inventoryFull = false;
            
            if (itemId == ItemIdType.None || amount <= 0)
                return false;
            
            int remaining = amount;
            bool anyAdded = false;
            
            EnsureNonFullBuffer(_slots.TotalSlotCount);
            
            int nonFullCount = _query.FindNonFullStacks(itemId, _nonFullStackIndicesBuffer);
            int maxStack = _itemDatabase.Get(itemId).MaxStack;
            
            for (int i = 0; i < nonFullCount && remaining > 0; i++)
            {
                int slotIndex = _nonFullStackIndicesBuffer[i];
                
                if (_mutations.TryAddToExistingStack(slotIndex, itemId, remaining, out int actuallyAdded))
                {
                    WriteUsedSlot(
                        usedSlotIndicesBuffer,
                        addedAmountsBuffer,
                        ref usedSlotCount,
                        slotIndex,
                        actuallyAdded);
                    
                    remaining -= actuallyAdded;
                    
                    anyAdded = true;
                }
            }

            while (remaining > 0)
            {
                if (!_query.TryFindFirstEmptyUnlockedSlot(out int emptySlotIndex))
                {
                    inventoryFull = true;
                    
                    break;
                }

                int add = Mathf.Min(remaining, maxStack);
                
                if (_mutations.TryPlaceNewStack(emptySlotIndex, itemId, add))
                {
                    WriteUsedSlot(
                        usedSlotIndicesBuffer,
                        addedAmountsBuffer,
                        ref usedSlotCount,
                        emptySlotIndex,
                        add);
                    
                    remaining -= add;
                    
                    anyAdded = true;
                }
                else
                {
                    inventoryFull = true;
                    
                    break;
                }
            }

            if (anyAdded)
                _persistence.SaveAndNotify();
            
            return anyAdded;
        }

        private void EnsureNonFullBuffer(int requiredLength)
        {
            if (_nonFullStackIndicesBuffer == null || _nonFullStackIndicesBuffer.Length < requiredLength)
                _nonFullStackIndicesBuffer = new int[requiredLength];
        }

        private void WriteUsedSlot(
            int[] usedSlotIndicesBuffer,
            int[] addedAmountsBuffer,
            ref int usedSlotCount,
            int slotIndex,
            int addedAmount)
        {
            if (usedSlotIndicesBuffer != null && usedSlotCount < usedSlotIndicesBuffer.Length)
                usedSlotIndicesBuffer[usedSlotCount] = slotIndex;
            
            if (addedAmountsBuffer != null && usedSlotCount < addedAmountsBuffer.Length)
                addedAmountsBuffer[usedSlotCount] = addedAmount;
            
            usedSlotCount++;
        }
    }
}