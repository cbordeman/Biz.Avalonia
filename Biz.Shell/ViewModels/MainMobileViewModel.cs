using System.Collections.Generic;
using System.Threading.Tasks;
using Biz.Shell.Views;
using Prism.Commands;
using Prism.Navigation.Regions;

namespace Biz.Shell.ViewModels;

public class MainMobileViewModel : NavigationAwareViewModelBase
{
    #region Greeting
    string greeting = "Welcome to Shell";

    public MainMobileViewModel(IContainer container) : base(container)
    { }

    public string Greeting
    {
        get => greeting;
        set => SetProperty(ref greeting, value);
    }
    #endregion Greeting

    #region NavigateSettingsCommand
    AsyncDelegateCommand? navigateSettingsCommand;
    public AsyncDelegateCommand NavigateSettingsCommand => navigateSettingsCommand ??= new AsyncDelegateCommand(ExecuteNavigateSettingsCommand, CanNavigateSettingsCommand);
    bool CanNavigateSettingsCommand() => true;
    Task ExecuteNavigateSettingsCommand()
    {
        this.RegionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(SettingsView));
        return Task.CompletedTask;
    }
    #endregion NavigateSettingsCommand
}
