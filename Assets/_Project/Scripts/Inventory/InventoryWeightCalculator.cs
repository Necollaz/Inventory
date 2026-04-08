using _Project.Data;
using _Project.State;

namespace _Project.Inventory
{
    public sealed class InventoryWeightCalculator
    {
        private readonly InventorySlots _slots;
        private readonly ItemDatabase _itemDatabase;

        public InventoryWeightCalculator(InventorySlots slots, ItemDatabase itemDatabase)
        {
            _slots = slots;
            _itemDatabase = itemDatabase;
        }

        public float CalculateTotalWeight()
        {
            InventorySlotData[] slots = _slots.Slots;
            float total = 0f;
            
            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].IsUnlocked)
                    continue;
                
                ItemStackData stack = slots[i].Stack;
                
                if (stack.ItemId == ItemIdType.None || stack.Amount <= 0)
                    continue;
                
                ItemDefinition definition = _itemDatabase.Get(stack.ItemId);
                
                total += definition.Weight * stack.Amount;
            }

            return total;
        }
    }
}