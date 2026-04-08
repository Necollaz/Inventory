using _Project.Persistence;
using _Project.State;

namespace _Project.Inventory
{
    public sealed class GameState
    {
        private readonly GameStateInitializer _initializer;
        
        public GameState(GameStateInitializer initializer)
        {
            _initializer = initializer;
        }
        
        public GameStateData State => _initializer.State;
    }
}