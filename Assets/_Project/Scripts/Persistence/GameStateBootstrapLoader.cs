using _Project.State;

namespace _Project.Persistence
{
    public sealed class GameStateBootstrapLoader
    {
        private readonly GameStateJsonFile _storage;
        private readonly InitialGameStateBuilder _initialBuilder;
        
        public GameStateBootstrapLoader(GameStateJsonFile storage, InitialGameStateBuilder initialBuilder)
        {
            _storage = storage;
            _initialBuilder = initialBuilder;
        }
        
        public GameStateData LoadOrCreate()
        {
            if (_storage.Exists())
                return _storage.Load();
            
            GameStateData state = _initialBuilder.Create();
            _storage.Save(state);
            
            return state;
        }
    }
}