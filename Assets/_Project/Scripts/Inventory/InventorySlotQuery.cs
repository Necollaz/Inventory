using System;
using _Project.Data;
using _Project.State;

namespace _Project.Inventory
{
    public sealed class InventorySlotQuery
    {
        private readonly InventorySlots _slots;
        private readonly ItemDatabase _itemDatabase;

        public InventorySlotQuery(InventorySlots slots, ItemDatabase itemDatabase)
        {
            _slots = slots;
            _itemDatabase = itemDatabase;
        }

        public bool TryFindFirstEmptyUnlockedSlot(out int slotIndex)
        {
            InventorySlotData[] slots = _slots.Slots;
            
            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].IsUnlocked)
                    continue;
                
                if (!slots[i].IsEmpty)
                    continue;
                
                slotIndex = i;
                
                return true;
            }

            slotIndex = -1;
            
            return false;
        }

        public bool TryFindNextLockedSlot(out int slotIndex)
        {
            InventorySlotData[] slots = _slots.Slots;
            
            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].IsUnlocked)
                {
                    slotIndex = i;
                    
                    return true;
                }
            }

            slotIndex = -1;
            
            return false;
        }

        public int FindAllWeapons(int[] resultIndicesBuffer)
        {
            if (resultIndicesBuffer == null)
                throw new ArgumentNullException(nameof(resultIndicesBuffer));
            
            InventorySlotData[] slots = _slots.Slots;
            int count = 0;
            
            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].IsUnlocked)
                    continue;
                
                if (slots[i].IsEmpty)
                    continue;
                
                ItemIdType id = slots[i].Stack.ItemId;
                ItemDefinition definition = _itemDatabase.Get(id);
                
                if (definition.Type != ItemType.Weapon)
                    continue;
                
                if (count < resultIndicesBuffer.Length)
                    resultIndicesBuffer[count] = i;
                
                count++;
            }

            return count;
        }

        public int FindAllNonEmptySlots(int[] resultIndicesBuffer)
        {
            if (resultIndicesBuffer == null)
                throw new ArgumentNullException(nameof(resultIndicesBuffer));
            
            InventorySlotData[] slots = _slots.Slots;
            int count = 0;
            
            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].IsUnlocked)
                    continue;
                
                if (slots[i].IsEmpty)
                    continue;
                
                if (count < resultIndicesBuffer.Length)
                    resultIndicesBuffer[count] = i;
                
                count++;
            }

            return count;
        }

        public int FindNonFullStacks(ItemIdType itemId, int[] resultIndicesBuffer)
        {
            if (resultIndicesBuffer == null)
                throw new ArgumentNullException(nameof(resultIndicesBuffer));
            
            if (itemId == ItemIdType.None)
                return 0;
            
            ItemDefinition definition = _itemDatabase.Get(itemId);
            int maxStack = definition.MaxStack;
            InventorySlotData[] slots = _slots.Slots;
            int count = 0;
            
            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].IsUnlocked)
                    continue;
                
                ItemStackData stack = slots[i].Stack;
                
                if (stack.ItemId != itemId)
                    continue;
                
                if (stack.Amount <= 0)
                    continue;
                
                if (stack.Amount < maxStack)
                {
                    if (count < resultIndicesBuffer.Length)
                        resultIndicesBuffer[count] = i;
                    
                    count++;
                }
            }

            return count;
        }

        public bool TryFindFirstAmmoSlot(ItemIdType ammoId, out int slotIndex)
        {
            InventorySlotData[] slots = _slots.Slots;
            
            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].IsUnlocked)
                    continue;
                
                ItemStackData stack = slots[i].Stack;
                
                if (stack.ItemId != ammoId)
                    continue;
                
                if (stack.Amount <= 0)
                    continue;
                
                slotIndex = i;
                
                return true;
            }

            slotIndex = -1;
            
            return false;
        }
    }
}