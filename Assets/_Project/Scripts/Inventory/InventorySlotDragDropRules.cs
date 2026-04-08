using UnityEngine;
using _Project.Data;
using _Project.State;

namespace _Project.Inventory
{
    public sealed class InventorySlotDragDropRules
    {
        private readonly InventorySlots _slots;
        private readonly ItemDatabase _itemDatabase;
        private readonly InventorySlotMutations _mutations;
        private readonly InventorySaveNotifier _persistence;

        public InventorySlotDragDropRules(
            InventorySlots slots,
            ItemDatabase itemDatabase,
            InventorySlotMutations mutations,
            InventorySaveNotifier persistence)
        {
            _slots = slots;
            _itemDatabase = itemDatabase;
            _mutations = mutations;
            _persistence = persistence;
        }

        public bool TryApplyDragDrop(int fromSlotIndex, int toSlotIndex)
        {
            InventorySlotData[] slots = _slots.Slots;
            
            if ((uint)fromSlotIndex >= (uint)slots.Length || (uint)toSlotIndex >= (uint)slots.Length)
                return false;
            
            if (fromSlotIndex == toSlotIndex)
                return false;
            
            if (!slots[fromSlotIndex].IsUnlocked || !slots[toSlotIndex].IsUnlocked)
                return false;
            
            if (slots[fromSlotIndex].IsEmpty)
                return false;
            
            if (slots[toSlotIndex].IsEmpty)
            {
                bool moved = _mutations.TryMoveStackToEmptySlot(fromSlotIndex, toSlotIndex);
                
                if (moved)
                    _persistence.SaveAndNotify();
                
                return moved;
            }

            ItemIdType fromItemId = slots[fromSlotIndex].Stack.ItemId;
            ItemIdType toItemId = slots[toSlotIndex].Stack.ItemId;
            
            if (fromItemId == toItemId)
            {
                int maxStack = _itemDatabase.Get(fromItemId).MaxStack;
                int toAmount = slots[toSlotIndex].Stack.Amount;
                
                if (toAmount < maxStack)
                {
                    int fromAmount = slots[fromSlotIndex].Stack.Amount;
                    int space = maxStack - toAmount;
                    int move = Mathf.Min(fromAmount, space);
                    bool merged = _mutations.TryAddToExistingStack(
                        toSlotIndex,
                        fromItemId,
                        move,
                        out int actuallyAdded);
                    
                    if (!merged || actuallyAdded <= 0)
                        return false;
                    
                    InventorySlotData fromSlot = slots[fromSlotIndex];
                    ItemStackData fromStack = fromSlot.Stack;
                    
                    fromStack.Amount -= actuallyAdded;
                    
                    fromSlot.Stack = fromStack.Amount <= 0 ? default : fromStack;
                    slots[fromSlotIndex] = fromSlot;
                    
                    _persistence.SaveAndNotify();
                    
                    return true;
                }
            }

            bool swapped = _mutations.TrySwapStacks(fromSlotIndex, toSlotIndex);
            
            if (swapped)
                _persistence.SaveAndNotify();
            
            return swapped;
        }
    }
}