using _Project.State;

namespace _Project.Inventory
{
    public sealed class InventorySlots
    {
        private readonly GameState _gameState;
        
        public InventorySlots(GameState gameState)
        {
            _gameState = gameState;
        }
        
        public InventorySlotData[] Slots => _gameState.State.Slots;
        public int TotalSlotCount => _gameState.State.Slots.Length;
    }
}