using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Data
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeField] private List<ItemDefinition> _items;

        private Dictionary<ItemIdType, ItemDefinition> _map;
        
        public IReadOnlyList<ItemDefinition> Items => _items;

        private void OnEnable()
        {
            BuildMap();
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            BuildMap();
        }
#endif
        private void BuildMap()
        {
            _map = new Dictionary<ItemIdType, ItemDefinition>();
            
            if (_items == null)
                return;
            
            foreach (ItemDefinition definition in _items)
            {
                if (definition == null)
                    continue;
                
                if (_map.ContainsKey(definition.Id))
                    Debug.LogWarning($"Duplicate ItemId: {definition.Id} in {name}");
                
                _map[definition.Id] = definition;
            }
        }
        
        public bool TryGet(ItemIdType id, out ItemDefinition definition)
        {
            if (_map == null)
                BuildMap();
            
            return _map.TryGetValue(id, out definition);
        }
        
        public ItemDefinition Get(ItemIdType id)
        {
            if (!TryGet(id, out ItemDefinition definition))
                throw new InvalidOperationException($"No ItemDefinition for {id}");
            
            return definition;
        }
    }
}