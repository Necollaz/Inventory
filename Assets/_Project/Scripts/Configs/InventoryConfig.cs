using UnityEngine;

namespace _Project.Configs
{
    [CreateAssetMenu(fileName = "InventoryConfig", menuName = "Inventory/Inventory Config")]
    public class InventoryConfig : ScriptableObject
    {
        [SerializeField] private int _totalSlots = 50;
        [SerializeField] private int _initialUnlockedSlots = 15;
        [SerializeField] private int[] _slotUnlockCosts;
        
        public int TotalSlots => _totalSlots;
        public int InitialUnlockedSlots => _initialUnlockedSlots;
        
        public int GetUnlockCostForSlotIndex(int zeroBasedSlotIndex)
        {
            if (zeroBasedSlotIndex < _initialUnlockedSlots || zeroBasedSlotIndex >= _totalSlots)
                return 0;
            
            int costArrayIndex = zeroBasedSlotIndex - _initialUnlockedSlots;
            
            if (_slotUnlockCosts == null || costArrayIndex < 0 || costArrayIndex >= _slotUnlockCosts.Length)
            {
                Debug.LogWarning(
                    $"InventoryConfig: нет цены для слота {zeroBasedSlotIndex}. " +
                    $"Нужен массив _slotUnlockCosts длиной {_totalSlots - _initialUnlockedSlots}, " +
                    $"индекс в массиве = {costArrayIndex}.");
                
                return 0;
            }
            
            return _slotUnlockCosts[costArrayIndex];
        }
    }
}