using UnityEngine;
using UnityEngine.UI;
using Zenject;
using _Project.Gameplay;

namespace _Project.UI
{
    public sealed class GameplayButtonsView : MonoBehaviour
    {
        [SerializeField] private Button _addCoinsButton;
        [SerializeField] private Button _addItemButton;
        [SerializeField] private Button _addAmmoButton;
        [SerializeField] private Button _shootButton;
        [SerializeField] private Button _removeItemButton;
        
        private GameplayButtonsActions _buttonsActions;
        
        [Inject]
        public void Construct(GameplayButtonsActions buttonsActions)
        {
            _buttonsActions = buttonsActions;
        }
        
        private void OnEnable()
        {
            _addCoinsButton.onClick.AddListener(OnAddCoinsClicked);
            _addItemButton.onClick.AddListener(OnAddItemClicked);
            _addAmmoButton.onClick.AddListener(OnAddAmmoClicked);
            _shootButton.onClick.AddListener(OnShootClicked);
            _removeItemButton.onClick.AddListener(OnRemoveItemClicked);
        }
        
        private void OnDisable()
        {
            _addCoinsButton.onClick.RemoveListener(OnAddCoinsClicked);
            _addItemButton.onClick.RemoveListener(OnAddItemClicked);
            _addAmmoButton.onClick.RemoveListener(OnAddAmmoClicked);
            _shootButton.onClick.RemoveListener(OnShootClicked);
            _removeItemButton.onClick.RemoveListener(OnRemoveItemClicked);
        }
        
        private void OnAddCoinsClicked()
        {
            _buttonsActions.AddRandomCoins();
        }
        
        private void OnAddItemClicked()
        {
            _buttonsActions.AddRandomItem();
        }
        
        private void OnAddAmmoClicked()
        {
            _buttonsActions.AddRandomAmmo();
        }
        
        private void OnShootClicked()
        {
            _buttonsActions.Shoot();
        }
        
        private void OnRemoveItemClicked()
        {
            _buttonsActions.RemoveRandomItem();
        }
    }
}