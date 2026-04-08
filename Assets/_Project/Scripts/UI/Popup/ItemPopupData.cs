using _Project.Data;

namespace _Project.UI
{
    public struct ItemPopupData
    {
        public ItemType type;
        public ItemIdType ammoId;
        
        public string title;
        
        public bool hasProtection;
        public bool hasWeaponStats;
        
        public float weight;
        
        public int amount;
        public int protection;
        public int damage;
    }
}