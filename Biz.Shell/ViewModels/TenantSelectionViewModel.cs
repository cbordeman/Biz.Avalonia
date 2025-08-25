using System.Reflection;
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
        Title = "Login";
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
        NavigationService.RequestNavigate(nameof(LoginView));
        return Task.CompletedTask;
    }
    #endregion CancelLoginCommand

    public override bool PersistInHistory() => false;
}
