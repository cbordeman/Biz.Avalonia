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

    public int MessageNumber
    {
        get;
        set => SetProperty(ref field, value);
    }

    public override Task OnNavigatedToAsync(NavigationContext ctx)
    {
        // Get and display our parameters
        MessageText = ctx.Parameters.GetStringOrEmpty("key1");
        MessageNumber = ctx.Parameters.GetStructOrDefault<int>("key1");
        
        return Task.CompletedTask;
    }

    public override Task<bool> CanNavigateToAsync(NavigationContext ctx)
    {
        // Navigation permission sample:
        return Task.FromResult(ctx.Parameters.ContainsKey("key1") &&
                               ctx.Parameters.ContainsKey("key2"));
    }
    
    public override string Area => "Settings.Sub1";
}