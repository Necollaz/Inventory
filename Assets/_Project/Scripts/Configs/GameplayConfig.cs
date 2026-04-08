using UnityEngine;

namespace _Project.Configs
{
    [CreateAssetMenu(fileName = "GameplayConfig", menuName = "Inventory/Gameplay Config")]
    public class GameplayConfig : ScriptableObject
    {
        [Header("Add coins button")]
        [SerializeField] private int _addCoinsMin = 9;
        [SerializeField] private int _addCoinsMax = 99;
        
        [Header("Add ammo button")]
        [SerializeField] private int _addAmmoMin = 10;
        [SerializeField] private int _addAmmoMax = 30;
        
        public int AddCoinsMin => _addCoinsMin;
        public int AddCoinsMax => _addCoinsMax;
        public int AddAmmoMin => _addAmmoMin;
        public int AddAmmoMax => _addAmmoMax;
    }
}