using Library.Communication;
using Library.Interfaces;
using Sygnalista_A.LocalLibrary;
using Sygnalista_A.LocalLibrary.Services;
using Sygnalista_A.MVVM.ViewModels;
using Sygnalista_A.MVVM.Views;
using System.IO;
using System.Windows;

namespace Sygnalista_A;

public partial class App : PrismApplication
{
    protected override void OnExit(ExitEventArgs e)
    {
        _ = Container.Resolve<CommunicationManager>().SendMessage(["stoppedlistening"]);
        Features.ClearCache();
        File.Delete("C:\\Users\\rafal\\Desktop\\Pogromcy\\Sygnalista_A\\TurnoverMedianTable");
        base.OnExit(e);
    }

    protected override Window CreateShell()
    {
        var mainWindow = Container.Resolve<MainWindowView>();

        Container.GetContainer().RegisterInstance(mainWindow.chrome);

        return mainWindow;
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<MainWindowViewModel>();
        containerRegistry.RegisterSingleton<CompanyInfoManager>();
        containerRegistry.RegisterSingleton<UIManager>();
        containerRegistry.RegisterSingleton<Background>();
        containerRegistry.RegisterSingleton<MainLoopManager>();
        containerRegistry.RegisterSingleton<MainWindowView>();
        containerRegistry.RegisterSingleton<CommunicationManager>();
        containerRegistry.RegisterSingleton<ICommunicationService, CommunicationService>();
    }
}
