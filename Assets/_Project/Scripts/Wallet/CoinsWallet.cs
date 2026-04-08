using System;
using _Project.Persistence;

namespace _Project.Wallet
{
    public sealed class CoinsWallet
    {
        public event Action<int> BalanceChanged;
        
        private readonly GameStateInitializer _initializer;
        private readonly GameStateJsonFile _storage;
        
        public CoinsWallet(GameStateInitializer initializer, GameStateJsonFile storage)
        {
            _initializer = initializer;
            _storage = storage;
        }
        
        public int Balance => _initializer.State.Coins;
        
        public bool TrySpendCoins(int amount, bool save)
        {
            if (amount <= 0)
                return false;
            
            int current = _initializer.State.Coins;
            
            if (current < amount)
                return false;
            
            _initializer.State.Coins = current - amount;
            
            NotifyBalanceChangedAndMaybeSave(save);
            
            return true;
        }
        
        public void AddCoins(int amount)
        {
            if (amount <= 0)
                return;
            
            _initializer.State.Coins += amount;

            NotifyBalanceChangedAndMaybeSave(true);
        }
        
        private void NotifyBalanceChangedAndMaybeSave(bool save)
        {
            BalanceChanged?.Invoke(_initializer.State.Coins);
            
            if (save)
                _storage.Save(_initializer.State);
        }
    }
}