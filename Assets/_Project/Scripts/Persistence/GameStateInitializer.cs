using Zenject;
using _Project.State;

namespace _Project.Persistence
{
    public sealed class GameStateInitializer : IInitializable
    {
        private readonly GameStateBootstrapLoader _bootstrapLoader;
        
        public GameStateInitializer(GameStateBootstrapLoader bootstrapLoader)
        {
            _bootstrapLoader = bootstrapLoader;
        }
        
        public GameStateData State { get; private set; }
        
        void IInitializable.Initialize()
        {
            State = _bootstrapLoader.LoadOrCreate();
        }
    }
}