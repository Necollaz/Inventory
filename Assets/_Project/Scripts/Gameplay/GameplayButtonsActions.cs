using System.Collections.Generic;
using UnityEngine;
using _Project.Configs;
using _Project.Data;
using _Project.Inventory;
using _Project.Wallet;

namespace _Project.Gameplay
{
    public sealed class GameplayButtonsActions
    {
        private readonly GameplayConfig _config;
        private readonly CoinsWallet _coinsWallet;
        private readonly ItemDatabase _itemDatabase;
        private readonly InventoryFacade _inventoryFacade;
        
        private readonly ItemType[] _addItemAllowedTypes =
        {
            ItemType.Weapon,
            ItemType.Head,
            ItemType.Torso
        };
        private readonly ItemType[] _ammoAllowedTypes =
        {
            ItemType.Ammo
        };
        
        private int[] _slotIndicesBuffer;
        private int[] _amountsBuffer;

        public GameplayButtonsActions(
            GameplayConfig config,
            CoinsWallet coinsWallet,
            ItemDatabase itemDatabase,
            InventoryFacade inventoryFacade)
        {
            _config = config;
            _coinsWallet = coinsWallet;
            _itemDatabase = itemDatabase;
            _inventoryFacade = inventoryFacade;
        }

        public void AddRandomCoins()
        {
            int amount = Random.Range(_config.AddCoinsMin, _config.AddCoinsMax + 1);
            _coinsWallet.AddCoins(amount);

            Debug.Log($"Добавлено ({amount}) монет");
        }

        public void AddRandomItem()
        {
            if (!TryPickRandomItemDefinitionByAllowedTypes(_addItemAllowedTypes, out ItemDefinition chosen))
            {
                Debug.LogError("Нет доступных предметов (Weapon/Head/Torso) в ItemDatabase");
                
                return;
            }
            
            if (!_inventoryFacade.TryPlaceNewStackIntoFirstEmptyUnlockedSlot(chosen.Id, 1, out int slotIndex))
            {
                Debug.LogError("Инвентарь полон");
                
                return;
            }
            
            Debug.Log($"Добавлено {chosen.Id} в слот: {slotIndex}");
        }

        public void AddRandomAmmo()
        {
            if (!TryPickRandomItemDefinitionByAllowedTypes(_ammoAllowedTypes, out ItemDefinition chosenAmmoDefinition))
            {
                Debug.LogError("Нет доступных патронов (ItemType.Ammo) в ItemDatabase");
                
                return;
            }
            
            ItemIdType ammoId = chosenAmmoDefinition.Id;
            int amount = Random.Range(_config.AddAmmoMin, _config.AddAmmoMax + 1);
            
            EnsureBuffersForSlots(_inventoryFacade.TotalSlotCount);
            
            bool anyAdded = _inventoryFacade.TryAddStackableItem(
                ammoId,
                amount,
                _slotIndicesBuffer,
                _amountsBuffer,
                out int usedSlotCount,
                out bool inventoryFull);
            
            for (int i = 0; i < usedSlotCount; i++)
            {
                int slotIndex = _slotIndicesBuffer[i];
                int addedToThisSlot = _amountsBuffer[i];
                
                Debug.Log($"Добавлено ({addedToThisSlot}) {ammoId} в слот: {slotIndex}");
            }
            
            if (!anyAdded && inventoryFull)
            {
                Debug.LogError("Инвентарь полон");
                
                return;
            }
            
            if (inventoryFull)
            {
                Debug.LogError("Инвентарь полон");
                
                return;
            }
        }

        public void Shoot()
        {
            EnsureBuffersForSlots(_inventoryFacade.TotalSlotCount);
            
            int weaponCount = _inventoryFacade.FindAllWeapons(_slotIndicesBuffer);
            
            if (weaponCount <= 0)
            {
                Debug.LogError("Нет оружия");
                
                return;
            }
            
            int randomStartBufferIndex = Random.Range(0, weaponCount);
            bool anyWeaponWithoutAmmoWasFound = false;
            
            for (int attempt = 0; attempt < weaponCount; attempt++)
            {
                int bufferIndex = (randomStartBufferIndex + attempt) % weaponCount;
                int weaponSlotIndex = _slotIndicesBuffer[bufferIndex];
                
                if (!_inventoryFacade.TryGetWeaponDataFromSlot(
                        weaponSlotIndex,
                        out ItemIdType weaponId,
                        out ItemIdType ammoId,
                        out int damage))
                {
                    continue;
                }
                
                if (!_inventoryFacade.TryConsumeOneAmmo(ammoId, out int ammoSlotIndex))
                {
                    anyWeaponWithoutAmmoWasFound = true;
                    
                    Debug.LogError($"Нет патронов для {weaponId}");
                    
                    continue;
                }
                
                Debug.Log($"Выстрел из {weaponId}, патроны: {ammoId}, урон: {damage}");
                
                return;
            }
            
            if(anyWeaponWithoutAmmoWasFound)
                return;
            
            Debug.LogError("Нет патронов");
        }

        public void RemoveRandomItem()
        {
            EnsureBuffersForSlots(_inventoryFacade.TotalSlotCount);
            
            int nonEmptyCount = _inventoryFacade.FindAllNonEmptySlots(_slotIndicesBuffer);
            
            if (nonEmptyCount <= 0)
            {
                Debug.LogError("Инвентарь пуст");
                
                return;
            }
            
            int chosenIndexInBuffer = Random.Range(0, nonEmptyCount);
            int slotIndex = _slotIndicesBuffer[chosenIndexInBuffer];
            
            if (!_inventoryFacade.TryClearSlotAndGetRemovedData(
                    slotIndex,
                    out ItemIdType removedItemId,
                    out int removedAmount))
            {
                return;
            }
            
            Debug.Log($"Удалено ({removedAmount}) {removedItemId} из слота: {slotIndex}");
        }
        
        private bool TryPickRandomItemDefinitionByAllowedTypes(ItemType[] allowedTypes, out ItemDefinition definition)
        {
            definition = null;
            
            if (allowedTypes == null || allowedTypes.Length == 0)
                return false;
            
            int eligibleCount = 0;
            IReadOnlyList<ItemDefinition> items = _itemDatabase.Items;
            
            for (int i = 0; i < items.Count; i++)
            {
                ItemDefinition item = items[i];
                
                if (item == null)
                    continue;
                
                if (IsAllowedType(item.Type, allowedTypes))
                    eligibleCount++;
            }
            
            if (eligibleCount == 0)
                return false;
            
            int randomIndex = Random.Range(0, eligibleCount);
            
            for (int i = 0; i < items.Count; i++)
            {
                ItemDefinition item = items[i];
                
                if (item == null)
                    continue;
                
                if (!IsAllowedType(item.Type, allowedTypes))
                    continue;
                
                if (randomIndex == 0)
                {
                    definition = item;
                    
                    return true;
                }
                
                randomIndex--;
            }
            
            return false;
        }
        
        private bool IsAllowedType(ItemType type, ItemType[] allowedTypes)
        {
            for (int i = 0; i < allowedTypes.Length; i++)
            {
                if (allowedTypes[i] == type)
                    return true;
            }
            
            return false;
        }
        
        private void EnsureBuffersForSlots(int slotCount)
        {
            if (_slotIndicesBuffer == null || _slotIndicesBuffer.Length < slotCount)
                _slotIndicesBuffer = new int[slotCount];
            
            if (_amountsBuffer == null || _amountsBuffer.Length < slotCount)
                _amountsBuffer = new int[slotCount];
        }
    }
}