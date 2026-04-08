using System.Globalization;
using UnityEngine;
using TMPro;
using Zenject;
using _Project.Inventory;
using _Project.Wallet;

namespace _Project.UI
{
    public sealed class GameplayHudView : MonoBehaviour, IInitializable
    {
        private const string COINS_TEXT_PREFIX = "Монеты: ";
        private const string WEIGHT_TEXT_PREFIX = "Вес: ";

        [SerializeField] private TextMeshProUGUI _coinsText;
        [SerializeField] private TextMeshProUGUI _weightText;

        private CoinsWallet _coinsWallet;
        private InventoryFacade _inventory;

        [Inject]
        public void Construct(CoinsWallet coinsWallet, InventoryFacade inventory)
        {
            _coinsWallet = coinsWallet;
            _inventory = inventory;
        }

        void IInitializable.Initialize()
        {
            _coinsWallet.BalanceChanged += OnBalanceChanged;
            _inventory.InventoryChanged += OnInventoryChanged;

            RefreshCoins();
            RefreshWeight();
        }

        private void OnDestroy()
        {
            if (_coinsWallet != null)
                _coinsWallet.BalanceChanged -= OnBalanceChanged;

            if (_inventory != null)
                _inventory.InventoryChanged -= OnInventoryChanged;
        }

        private void OnBalanceChanged(int balance)
        {
            RefreshCoins();
        }

        private void OnInventoryChanged()
        {
            RefreshWeight();
        }

        private void RefreshCoins()
        {
            _coinsText.text = COINS_TEXT_PREFIX + _coinsWallet.Balance.ToString(CultureInfo.InvariantCulture);
        }

        private void RefreshWeight()
        {
            float weight = _inventory.CalculateTotalWeight();
            _weightText.text = WEIGHT_TEXT_PREFIX + weight.ToString("F3", CultureInfo.InvariantCulture);
        }
    }
}