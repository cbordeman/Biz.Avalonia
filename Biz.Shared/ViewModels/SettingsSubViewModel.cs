namespace Biz.Shared.ViewModels;

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

    public int MessageNumber
    {
        get;
        set => SetProperty(ref field, value);
    }

    public override Task OnNavigatedToAsync(NavigationContext ctx)
    {
        // Get and display our parameters
        MessageText = NavigationParameters.GetStringOrEmpty("key1");
        MessageNumber = NavigationParameters.GetValueOrDefault<int>("key1");
        
        return Task.CompletedTask;
    }

    public override Task<bool> CanNavigateToAsync(NavigationContext ctx)
    {
        return Task.FromResult(true);
    }
    
    public override string Area => "Settings.Sub1";
}