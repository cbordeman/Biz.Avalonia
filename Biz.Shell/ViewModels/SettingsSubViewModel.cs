using Biz.Modules.Dashboard;
using Biz.Modules.Dashboard.Core;

namespace Biz.Shell.ViewModels;

public class SettingsSubViewModel : PageViewModelBase
{
    public SettingsSubViewModel()
    {
        Title = "Settings - SubView";
    }

    // public DelegateCommand CmdNavigateBack => new(() =>
    // {
    //     // Go back to the previous calling page, otherwise, Dashboard.
    //     if (journal != null && journal.CanGoBack)
    //         journal.GoBack();
    //     else
    //         NavigationService.RequestNavigate(RegionNames.MainContentRegion, 
    //             DashboardConstants.DashboardView);
    // });

    public string? MessageText
    {
        get;
        set => SetProperty(ref field, value);
    } = string.Empty;

    public string? MessageNumber
    {
        get;
        set => SetProperty(ref field, value);
    } = string.Empty;

    /// <summary>Navigation completed successfully.</summary>
    /// <param name="navigationContext">Navigation context.</param>
    public override void OnNavigatedTo(NavigationContext navigationContext)
    {
        
        // // Get and display our parameters
        // if (navigationContext.Parameters.TryGetValue("key1", out string? value))
        //     MessageText = value;
        //
        // if (navigationContext.Parameters.TryGetValue("key2", out int msgNum))
        //     MessageNumber = msgNum.ToString();
    }

    protected override bool OnNavigatingTo(NavigationContext navigationContext)
    {
        Debug.WriteLine("OnNavigatingTo");

        // Navigation permission sample:
        return navigationContext.Parameters != null &&
               navigationContext.Parameters.ContainsKey("key1") &&
               navigationContext.Parameters.ContainsKey("key2");
    }
    public override string Area => "Settings.Sub1";
}