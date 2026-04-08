using System;

namespace _Project.State
{
    [Serializable]
    public struct InventorySlotData
    {
        public ItemStackData Stack;
        public bool IsUnlocked;
        
        public bool IsEmpty => Stack.IsEmpty;
    }
}