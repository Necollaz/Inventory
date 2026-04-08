using System;
using _Project.Data;

namespace _Project.State
{
    [Serializable]
    public struct ItemStackData
    {
        public ItemIdType ItemId;
        public int Amount;
        
        public bool IsEmpty => ItemId == ItemIdType.None || Amount <= 0;
    }
}