using SteamPlayersConnector.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace SteamPlayersConnector
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IOpenFileService, OpenFileService>();
        }
    }
}
