using Biz.Modules.Dashboard;

namespace Biz.Shell.ViewModels;

public class SettingsSubViewModel : PageViewModelBase
{
    IRegionNavigationJournal? journal;
    string? messageText = string.Empty;
    string? messageNumber = string.Empty;

    public SettingsSubViewModel(IContainer container)
        : base(container)
    {
        Title = "Settings - SubView";
    }

    public DelegateCommand CmdNavigateBack => new(() =>
    {
        // Go back to the previous calling page, otherwise, Dashboard.
        if (journal != null && journal.CanGoBack)
            journal.GoBack();
        else
            RegionManager.RequestNavigate(RegionNames.MainContentRegion, 
                DashboardConstants.DashboardView);
    });

    public string? MessageText { get => messageText; set => SetProperty(ref messageText, value); }

    public string? MessageNumber { get => messageNumber; set => SetProperty(ref messageNumber, value); }

    /// <summary>Navigation completed successfully.</summary>
    /// <param name="navigationContext">Navigation context.</param>
    public override void OnNavigatedTo(NavigationContext navigationContext)
    {
        // Used to "Go Back" to parent
        journal = navigationContext.NavigationService.Journal;

        // Get and display our parameters
        if (navigationContext.Parameters.TryGetValue("key1", out string? value))
            MessageText = value;

        if (navigationContext.Parameters.TryGetValue("key2", out int msgNum))
            MessageNumber = msgNum.ToString();
    }

    protected override bool OnNavigatingTo(NavigationContext navigationContext)
    {
        Debug.WriteLine("OnNavigatingTo");

        // Navigation permission sample:
        return navigationContext.Parameters.ContainsKey("key1") &&
               navigationContext.Parameters.ContainsKey("key2");
    }
}