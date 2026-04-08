using UnityEngine;
using Zenject;
using _Project.Configs;
using _Project.Data;
using _Project.Inventory;
using _Project.Persistence;
using _Project.State;
using _Project.Wallet;

namespace _Project.UI
{
    public sealed class InventoryGridView : MonoBehaviour
    {
        private const int TOTAL_SLOT_COUNT = 50;
        
        [SerializeField] private InventorySlotView _slotPrefab;
        [SerializeField] private Transform _slotsRoot;
        [SerializeField] private InventoryDragGhost _inventoryDragGhost;
        [SerializeField] private ItemInfoPopupView _itemInfoPopupView;
        
        private DiContainer _container;
        private GameStateInitializer _initializer;
        private InventoryFacade _inventoryFacade;
        private ItemDatabase _itemDatabase;
        private InventoryConfig _inventoryConfig;
        private CoinsWallet _coinsWallet;
        private InventorySlotView[] _slotViews;

        [Inject]
        public void Construct(
            DiContainer container,
            GameStateInitializer initializer,
            InventoryFacade inventoryFacade,
            ItemDatabase itemDatabase,
            InventoryConfig inventoryConfig,
            CoinsWallet coinsWallet)
        {
            _container = container;
            _initializer = initializer;
            _inventoryFacade = inventoryFacade;
            _itemDatabase = itemDatabase;
            _inventoryConfig = inventoryConfig;
            _coinsWallet = coinsWallet;
        }

        private void Awake()
        {
            BuildSlots();
        }

        private void OnEnable()
        {
            _inventoryFacade.InventoryChanged += OnInventoryChanged;
            _coinsWallet.BalanceChanged += OnBalanceChanged;
        }

        private void OnDisable()
        {
            _inventoryFacade.InventoryChanged -= OnInventoryChanged;
            _coinsWallet.BalanceChanged -= OnBalanceChanged;
        }

        private void Start()
        {
            RefreshAll();
        }

        private void BuildSlots()
        {
            _slotViews = new InventorySlotView[TOTAL_SLOT_COUNT];
            
            for (int i = 0; i < TOTAL_SLOT_COUNT; i++)
            {
                InventorySlotView view = Instantiate(_slotPrefab, _slotsRoot);
                _container.InjectGameObject(view.gameObject);
                view.SetSlotIndex(i);
                view.BindClick(OnSlotClicked);
                
                _slotViews[i] = view;

                InventorySlotDragDrop dragDrop = view.GetComponent<InventorySlotDragDrop>();
                
                dragDrop?.SetDragGhost(_inventoryDragGhost);
            }
        }

        private void OnInventoryChanged()
        {
            RefreshAll();
        }

        private void OnBalanceChanged(int balance)
        {
            RefreshAll();
        }

        private void OnSlotClicked(int slotIndex)
        {
            InventorySlotData[] slots = _initializer.State.Slots;
            
            if ((uint)slotIndex >= (uint)slots.Length)
                return;
            
            InventorySlotData slot = slots[slotIndex];
            
            if (!slot.IsUnlocked)
            {
                _inventoryFacade.TryUnlockSlot(slotIndex);
                
                return;
            }
            
            if (slot.IsEmpty)
                return;
            
            ItemIdType itemId = slot.Stack.ItemId;
            int amount = slot.Stack.Amount;
            ItemDefinition definition = _itemDatabase.Get(itemId);
            
            ItemPopupData popupData = new ItemPopupData
            {
                title = itemId.ToString(),
                type = definition.Type,
                weight = definition.Weight,
                amount = amount
            };
            
            if (definition.Type == ItemType.Head || definition.Type == ItemType.Torso)
            {
                popupData.hasProtection = true;
                popupData.protection = definition.Protection;
            }
            
            if (definition.Type == ItemType.Weapon)
            {
                popupData.hasWeaponStats = true;
                popupData.damage = definition.Damage;
                popupData.ammoId = definition.AmmoId;
            }
            
            RectTransform slotRectTransform = _slotViews[slotIndex].GetComponent<RectTransform>();
            _itemInfoPopupView?.Show(popupData, slotRectTransform);
        }

        private void RefreshAll()
        {
            InventorySlotData[] slots = _initializer.State.Slots;
            int nextLockedSlotIndex = -1;
            
            if (_inventoryFacade.TryFindNextLockedSlot(out int found))
                nextLockedSlotIndex = found;
            
            for (int i = 0; i < _slotViews.Length; i++)
            {
                InventorySlotViewState state = BuildStateForSlot(i, slots, nextLockedSlotIndex);
                _slotViews[i].Refresh(state);
            }
        }

        private InventorySlotViewState BuildStateForSlot(
            int slotIndex,
            InventorySlotData[] slots,
            int nextLockedSlotIndex)
        {
            InventorySlotData slot = slots[slotIndex];
            
            if (!slot.IsUnlocked)
            {
                int cost = _inventoryConfig.GetUnlockCostForSlotIndex(slotIndex);
                bool isNextPurchasable = slotIndex == nextLockedSlotIndex;
                
                return new InventorySlotViewState
                {
                    IsLocked = true,
                    IsNextPurchasableSlot = isNextPurchasable,
                    ShowUnlockCost = true,
                    UnlockCost = cost,
                    IsEmpty = true,
                    ItemSprite = null,
                    StackAmount = 0
                };
            }
            
            if (slot.IsEmpty)
            {
                return new InventorySlotViewState
                {
                    IsLocked = false,
                    IsNextPurchasableSlot = false,
                    ShowUnlockCost = false,
                    UnlockCost = 0,
                    IsEmpty = true,
                    ItemSprite = null,
                    StackAmount = 0
                };
            }
            
            ItemIdType itemId = slot.Stack.ItemId;
            int amount = slot.Stack.Amount;
            ItemDefinition definition = _itemDatabase.Get(itemId);
            
            return new InventorySlotViewState
            {
                IsLocked = false,
                IsNextPurchasableSlot = false,
                ShowUnlockCost = false,
                UnlockCost = 0,
                IsEmpty = false,
                ItemSprite = definition.InventorySlotSprite,
                StackAmount = amount
            };
        }
    }
}