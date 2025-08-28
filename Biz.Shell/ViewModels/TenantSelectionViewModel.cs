using System.Diagnostics.CodeAnalysis;
using Biz.Models;

namespace Biz.Shell.ViewModels;

public class TenantSelectionViewModel : PageViewModelBase
{
    #region AvailableTenants
    public Tenant[] AvailableTenants
    {
        get;
        set => SetProperty(ref field, value);
    } = null!;
    #endregion AvailableTenants

    public TenantSelectionViewModel(IContainer container) : base(container)
    {
        Title = "Login";
    }

    #region SelectCommand
    [field: AllowNull, MaybeNull]
    public AsyncDelegateCommand<Tenant> SelectCommand => field ??= new AsyncDelegateCommand<Tenant>(ExecuteSelectCommand, CanSelectCommand);
    static bool CanSelectCommand(Tenant t) => true;
    async Task ExecuteSelectCommand(Tenant t)
    {
        await AuthenticationService.CompleteLogin(t);
    }
    #endregion SelectCommand

    #region CancelLoginCommand
    [field: AllowNull, MaybeNull]
    public AsyncDelegateCommand CancelLoginCommand => field ??= new AsyncDelegateCommand(ExecuteCancelLoginCommand);
    Task ExecuteCancelLoginCommand()
    {
        NavigationService.RequestNavigate(nameof(LoginView));
        return Task.CompletedTask;
    }
    #endregion CancelLoginCommand

    public override bool PersistInHistory() => false;
}
