using _Project.Configs;

namespace _Project.State
{
    public sealed class InitialGameStateBuilder
    {
        private readonly InventoryConfig _config;
        
        public InitialGameStateBuilder(InventoryConfig config)
        {
            _config = config;
        }
        
        public GameStateData Create()
        {
            var slots = new InventorySlotData[_config.TotalSlots];
            
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = new InventorySlotData
                {
                    IsUnlocked = i < _config.InitialUnlockedSlots,
                    Stack = default
                };
            }
            
            return new GameStateData
            {
                Coins = 0,
                Slots = slots
            };
        }
    }
}