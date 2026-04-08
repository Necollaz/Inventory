using UnityEngine;
using Zenject;
using _Project.Configs;
using _Project.Data;
using _Project.Gameplay;
using _Project.Inventory;
using _Project.Persistence;
using _Project.State;
using _Project.UI;
using _Project.Wallet;

namespace _Project.Installers
{
    public sealed class GameInstaller : MonoInstaller
    {
        [Header("Configs (ScriptableObjects)")]
        [SerializeField] private InventoryConfig _inventoryConfig;
        [SerializeField] private GameplayConfig _gameplayConfig;
        [SerializeField] private ItemDatabase _itemDatabase;

        public override void InstallBindings()
        {
            Container.Bind<InventoryConfig>().FromInstance(_inventoryConfig).AsSingle();
            
            Container.Bind<GameplayConfig>().FromInstance(_gameplayConfig).AsSingle();
            Container.Bind<ItemDatabase>().FromInstance(_itemDatabase).AsSingle();
            
            Container.Bind<InitialGameStateBuilder>().AsSingle();
            Container.Bind<GameStateJsonFile>().AsSingle();
            Container.Bind<GameStateBootstrapLoader>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameStateInitializer>().AsSingle();
            Container.Bind<GameplayButtonsActions>().AsSingle();
            
            Container.Bind<CoinsWallet>().AsSingle();
            
            Container.Bind<GameState>().AsSingle();
            Container.Bind<InventorySlots>().AsSingle();
            Container.Bind<InventorySaveNotifier>().AsSingle();
            Container.Bind<InventorySlotQuery>().AsSingle();
            Container.Bind<InventorySlotMutations>().AsSingle();
            Container.Bind<InventorySlotUnlock>().AsSingle();
            Container.Bind<InventoryStackAdd>().AsSingle();
            Container.Bind<InventoryWeaponShot>().AsSingle();
            Container.Bind<InventorySlotDragDropRules>().AsSingle();
            Container.Bind<InventoryWeightCalculator>().AsSingle();
            
            Container.Bind<InventoryFacade>().AsSingle();
            Container.Bind<InventoryDragSession>().AsSingle();
            
            UiInstaller.Install(Container);
        }
    }
}