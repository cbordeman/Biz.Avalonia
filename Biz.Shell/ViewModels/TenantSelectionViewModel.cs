using Biz.Models;

namespace Biz.Shell.ViewModels;

public class TenantSelectionViewModel : PageViewModelBase
{
    #region AvailableTenants
    Tenant[] availableTenants = null!;
    public Tenant[] AvailableTenants
    {
        get => availableTenants;
        set => SetProperty(ref availableTenants, value);
    }
    #endregion AvailableTenants

    public TenantSelectionViewModel(IContainer container) : base(container)
    {
        Title = "Select Tenant";
    }
    
    #region SelectCommand
    AsyncDelegateCommand<Tenant>? selectCommand;
    public AsyncDelegateCommand<Tenant> SelectCommand => selectCommand ??= new AsyncDelegateCommand<Tenant>(ExecuteSelectCommand, CanSelectCommand);
    static bool CanSelectCommand(Tenant t) => true;
    async Task ExecuteSelectCommand(Tenant t)
    {
        await AuthenticationService.CompleteLogin(t);
    }
    #endregion SelectCommand

    #region CancelLoginCommand
    AsyncDelegateCommand? cancelLoginCommand;
    public AsyncDelegateCommand CancelLoginCommand => cancelLoginCommand ??= new AsyncDelegateCommand(ExecuteCancelLoginCommand);
    Task ExecuteCancelLoginCommand()
    {
        NavigationService.RequestNavigate(GlobalConstants.LoginView);
        return Task.CompletedTask;
    }
    #endregion CancelLoginCommand

    protected void OnNavigatedToAsync(INavigationParameters parameters)
    {
        if (parameters.ContainsKey("availableTenants"))
            AvailableTenants = parameters.GetValue<Tenant[]>("availableTenants");
    }
    
    public override bool PersistInHistory() => false;
}