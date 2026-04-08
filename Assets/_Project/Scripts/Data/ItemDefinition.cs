using UnityEngine;

namespace _Project.Data
{
    [CreateAssetMenu(fileName = "ItemDefinition", menuName = "Inventory/Item Definition")]
    public class ItemDefinition : ScriptableObject
    {
        [SerializeField] private ItemIdType _id;
        [SerializeField] private ItemType _type;
        [SerializeField] private float _weight;
        [SerializeField] private int _maxStack;
        [SerializeField] private Sprite _inventorySlotSprite;

        [Header("Armor (Head / Torso)")]
        [SerializeField] private int _protection;

        [Header("Weapon")] 
        [SerializeField] private ItemIdType _ammoId;
        [SerializeField] private int _damage;

        public Sprite InventorySlotSprite => _inventorySlotSprite;
        public ItemIdType Id => _id;
        public ItemType Type => _type;
        public float Weight => _weight;
        public int MaxStack => _maxStack;
        
        public int Protection => _protection;
        
        public ItemIdType AmmoId => _ammoId;
        public int Damage => _damage;
    }
}