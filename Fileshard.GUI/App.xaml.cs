using Prism.Ioc;
using System.Windows;
using Fileshard.GUI.Views;
using Prism.Unity;

namespace Fileshard.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // containerRegistry.RegisterForNavigation();
        }
    }
}
