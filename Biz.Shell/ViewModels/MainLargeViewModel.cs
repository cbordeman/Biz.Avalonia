using System.Threading.Tasks;
using Biz.Core.ViewModels;
using Prism.Commands;

namespace Biz.Shell.ViewModels;

public class MainLargeViewModel : NavigationAwareViewModelBase
{
    #region Greeting
    string greeting = "Welcome to Shell (Large)";
    public string Greeting
    {
        get => greeting;
        set => SetProperty(ref greeting, value);
    }
    #endregion Greeting

    public MainLargeViewModel(IContainer container) : base(container)
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