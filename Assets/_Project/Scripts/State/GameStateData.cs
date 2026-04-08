using System;

namespace _Project.State
{
    [Serializable]
    public class GameStateData
    {
        public InventorySlotData[] Slots;
        public int Coins;
    }
}