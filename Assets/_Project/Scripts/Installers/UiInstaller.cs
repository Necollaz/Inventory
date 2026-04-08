using Zenject;
using _Project.UI;

namespace _Project.Installers
{
    public sealed class UiInstaller : Installer<UiInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameplayHudView>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<InventoryGridView>().FromComponentInHierarchy().AsSingle();
            Container.Bind<InventoryDragGhost>().FromComponentInHierarchy().AsSingle();
        }
    }
}