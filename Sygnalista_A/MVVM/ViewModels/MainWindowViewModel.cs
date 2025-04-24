using Sygnalista_A.LocalLibrary;
using Sygnalista_A.LocalLibrary.Services;
using System.Windows.Input;

namespace Sygnalista_A.MVVM.ViewModels;

public class MainWindowViewModel : BindableBase
{
    public MainWindowViewModel(MainLoopManager mainLoopManager, CompanyInfoManager companyInfoManager,
        UIManager uIManager)
    {
        NameTranslation.InitializeTranslations();

        this.mainLoopManager = mainLoopManager;
        CompanyInfoManager = companyInfoManager;
        UIManager = uIManager;
    }

    private readonly MainLoopManager mainLoopManager;

    public CompanyInfoManager CompanyInfoManager { get; }
    public UIManager UIManager { get; }
    public int ForceButtonBackgroundColor { get; set; } = 1;
    public ICommand ExecuteSygnalista_A =>
        new DelegateCommand(async () =>
        {
            if (MainLoopManager.IsSearching)
            {
                _ = mainLoopManager.StopSygnalista_A();
            }

            else
            {
                _ = mainLoopManager.ResetStartSygnalista_A();
            }
        });

    public ICommand ForceAutoScriptCommand =>
        new DelegateCommand(async () =>
        {
            if (mainLoopManager.ForceAutoScript)
            {
                mainLoopManager.ForceAutoScript = false;
                ForceButtonBackgroundColor = 2;
            }

            else
            {
                mainLoopManager.ForceAutoScript = true;
                ForceButtonBackgroundColor = 1;
            }
            RaisePropertyChanged(nameof(ForceButtonBackgroundColor));
        });
}