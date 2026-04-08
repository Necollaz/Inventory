using UnityEngine;

namespace _Project.UI
{
    public struct InventorySlotViewState
    {
        public Sprite ItemSprite;
        public bool IsLocked;
        public bool ShowUnlockCost;
        public bool IsNextPurchasableSlot;
        public bool IsEmpty;
        public int UnlockCost;
        public int StackAmount;
    }
}