using UnityEngine;
using _Project.Data;
using _Project.State;

namespace _Project.Inventory
{
    public sealed class InventorySlotMutations
    {
        private readonly InventorySlots _slots;
        private readonly ItemDatabase _itemDatabase;

        public InventorySlotMutations(InventorySlots slots, ItemDatabase itemDatabase)
        {
            _slots = slots;
            _itemDatabase = itemDatabase;
        }

        public bool TryPlaceNewStack(int slotIndex, ItemIdType itemId, int amount)
        {
            InventorySlotData[] slots = _slots.Slots;
            
            if ((uint)slotIndex >= (uint)slots.Length)
                return false;
            
            if (!slots[slotIndex].IsUnlocked)
                return false;
            
            if (!slots[slotIndex].IsEmpty)
                return false;
            
            slots[slotIndex] = new InventorySlotData
            {
                IsUnlocked = true,
                Stack = new ItemStackData
                {
                    ItemId = itemId,
                    Amount = amount
                }
            };
            
            return true;
        }

        public bool TryClearSlot(int slotIndex, out ItemIdType removedItemId, out int removedAmount)
        {
            removedItemId = ItemIdType.None;
            removedAmount = 0;
            InventorySlotData[] slots = _slots.Slots;
            
            if ((uint)slotIndex >= (uint)slots.Length)
                return false;
            
            InventorySlotData slot = slots[slotIndex];
            
            if (!slot.IsUnlocked || slot.IsEmpty)
                return false;
            
            removedItemId = slot.Stack.ItemId;
            removedAmount = slot.Stack.Amount;
            slot.Stack = default;
            slots[slotIndex] = slot;
            
            return true;
        }

        public bool TryConsumeOneFromAmmoSlot(int ammoSlotIndex)
        {
            InventorySlotData[] slots = _slots.Slots;
            
            if ((uint)ammoSlotIndex >= (uint)slots.Length)
                return false;
            
            InventorySlotData slot = slots[ammoSlotIndex];
            ItemStackData stack = slot.Stack;
            
            if (stack.Amount <= 0)
                return false;
            
            stack.Amount -= 1;
            
            if (stack.Amount <= 0)
                slot.Stack = default;
            else
                slot.Stack = stack;
            
            slots[ammoSlotIndex] = slot;
            
            return true;
        }

        public bool TryAddToExistingStack(int slotIndex, ItemIdType itemId, int addAmount, out int actuallyAdded)
        {
            actuallyAdded = 0;
            
            if (addAmount <= 0)
                return false;
            
            InventorySlotData[] slots = _slots.Slots;
            
            if ((uint)slotIndex >= (uint)slots.Length)
                return false;
            
            InventorySlotData slot = slots[slotIndex];
            
            if (!slot.IsUnlocked || slot.IsEmpty)
                return false;
            
            ItemStackData stack = slot.Stack;
            
            if (stack.ItemId != itemId)
                return false;
            
            ItemDefinition definition = _itemDatabase.Get(itemId);
            int maxStack = definition.MaxStack;
            int space = maxStack - stack.Amount;
            
            if (space <= 0)
                return false;
            
            actuallyAdded = Mathf.Min(addAmount, space);
            
            stack.Amount += actuallyAdded;
            
            slot.Stack = stack;
            slots[slotIndex] = slot;
            
            return true;
        }

        public bool TrySwapStacks(int fromSlotIndex, int toSlotIndex)
        {
            InventorySlotData[] slots = _slots.Slots;
            
            if ((uint)fromSlotIndex >= (uint)slots.Length || (uint)toSlotIndex >= (uint)slots.Length)
                return false;
            
            InventorySlotData fromSlot = slots[fromSlotIndex];
            InventorySlotData toSlot = slots[toSlotIndex];
            
            if (!fromSlot.IsUnlocked || !toSlot.IsUnlocked)
                return false;
            
            ItemStackData temp = toSlot.Stack;
            toSlot.Stack = fromSlot.Stack;
            fromSlot.Stack = temp;
            slots[toSlotIndex] = toSlot;
            slots[fromSlotIndex] = fromSlot;
            
            return true;
        }

        public bool TryMoveStackToEmptySlot(int fromSlotIndex, int toSlotIndex)
        {
            InventorySlotData[] slots = _slots.Slots;
            
            if ((uint)fromSlotIndex >= (uint)slots.Length || (uint)toSlotIndex >= (uint)slots.Length)
                return false;
            
            InventorySlotData fromSlot = slots[fromSlotIndex];
            InventorySlotData toSlot = slots[toSlotIndex];
            
            if (!fromSlot.IsUnlocked || !toSlot.IsUnlocked)
                return false;
            
            if (fromSlot.IsEmpty || !toSlot.IsEmpty)
                return false;
            
            toSlot.Stack = fromSlot.Stack;
            fromSlot.Stack = default;
            slots[toSlotIndex] = toSlot;
            slots[fromSlotIndex] = fromSlot;
            
            return true;
        }

        public bool TrySetUnlocked(int slotIndex)
        {
            InventorySlotData[] slots = _slots.Slots;
            
            if ((uint)slotIndex >= (uint)slots.Length)
                return false;
            
            InventorySlotData slot = slots[slotIndex];
            slot.IsUnlocked = true;
            slots[slotIndex] = slot;
            
            return true;
        }
    }
}