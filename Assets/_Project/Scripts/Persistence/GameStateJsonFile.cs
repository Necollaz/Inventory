using System;
using System.IO;
using UnityEngine;
using _Project.State;

namespace _Project.Persistence
{
    public sealed class GameStateJsonFile
    {
        private const string FILE_NAME = "game_state.json";
        
        private readonly string _path;
        
        public GameStateJsonFile()
        {
            _path = Path.Combine(Application.persistentDataPath, FILE_NAME);
        }
        
        public GameStateData Load()
        {
            if (!Exists())
                throw new FileNotFoundException("Save file not found", _path);
            
            var json = File.ReadAllText(_path);
            var state = JsonUtility.FromJson<GameStateData>(json);
            
            if (state == null)
                throw new InvalidDataException("Failed to deserialize GameStateData");
            
            return state;
        }
        
        public bool Exists() => File.Exists(_path);
        
        public void Save(GameStateData state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));
            
            var json = JsonUtility.ToJson(state, prettyPrint: false);
            var path = _path + ".tmp";
            
            Directory.CreateDirectory(Path.GetDirectoryName(_path) ?? Application.persistentDataPath);
            File.WriteAllText(path, json);
            
            if (File.Exists(_path))
                File.Delete(_path);
            
            File.Move(path, _path);
        }
    }
}