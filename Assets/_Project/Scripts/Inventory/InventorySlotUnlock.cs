using _Project.Configs;
using _Project.Wallet;

namespace _Project.Inventory
{
    public sealed class InventorySlotUnlock
    {
        private readonly InventoryConfig _config;
        private readonly CoinsWallet _coinsWallet;
        private readonly InventorySlotQuery _query;
        private readonly InventorySlotMutations _mutations;
        private readonly InventorySaveNotifier _persistence;

        public InventorySlotUnlock(
            InventoryConfig config,
            CoinsWallet coinsWallet,
            InventorySlotQuery query,
            InventorySlotMutations mutations,
            InventorySaveNotifier persistence)
        {
            _config = config;
            _coinsWallet = coinsWallet;
            _query = query;
            _mutations = mutations;
            _persistence = persistence;
        }

        public bool TryUnlockSlot(int slotIndex)
        {
            if (!_query.TryFindNextLockedSlot(out int nextLockedSlotIndex) || nextLockedSlotIndex != slotIndex)
                return false;
            
            int cost = _config.GetUnlockCostForSlotIndex(slotIndex);
            
            if (cost <= 0 || cost == int.MaxValue)
                return false;
            
            if (!_coinsWallet.TrySpendCoins(cost, save: false))
                return false;
            
            if (!_mutations.TrySetUnlocked(slotIndex))
                return false;
            
            _persistence.SaveAndNotify();
            
            return true;
        }
    }
}