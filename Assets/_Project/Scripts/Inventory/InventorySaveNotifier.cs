using System;
using _Project.Persistence;

namespace _Project.Inventory
{
    public sealed class InventorySaveNotifier
    {
        public event Action InventoryChanged;
        
        private readonly GameStateJsonFile _storage;
        private readonly GameState _gameState;
        
        public InventorySaveNotifier(GameStateJsonFile storage, GameState gameState)
        {
            _storage = storage;
            _gameState = gameState;
        }
        
        public void SaveAndNotify()
        {
            _storage.Save(_gameState.State);
            
            InventoryChanged?.Invoke();
        }
        
        public void NotifyOnly()
        {
            InventoryChanged?.Invoke();
        }
    }
}