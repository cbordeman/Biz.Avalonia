using System.Threading.Tasks;

namespace Biz.Shell.ViewModels;

public class MainSmallViewModel : NavigationAwareViewModelBase
{
    #region Greeting

    string greeting = "Welcome to Shell (Small)";

    public string Greeting
    {
        get => greeting;
        set => SetProperty(ref greeting, value);
    }

    #endregion Greeting

    public MainSmallViewModel(IContainer container) : base(container)
    {
    }

    #region NavigateSettingsCommand

    AsyncDelegateCommand? navigateSettingsCommand;

    public AsyncDelegateCommand NavigateSettingsCommand => navigateSettingsCommand ??=
        new AsyncDelegateCommand(ExecuteNavigateSettingsCommand, CanNavigateSettingsCommand);

    bool CanNavigateSettingsCommand() => true;

    Task ExecuteNavigateSettingsCommand()
    {
        this.RegionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(SettingsView));
        return Task.CompletedTask;
    }

    #endregion NavigateSettingsCommand
}