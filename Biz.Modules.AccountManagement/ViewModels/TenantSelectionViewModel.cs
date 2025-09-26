using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Biz.Modules.AccountManagement.Core;
using Biz.Shell.Services;
using CompositeFramework.Core.Extensions;

namespace Biz.Modules.AccountManagement.ViewModels;

[UsedImplicitly]
public class TenantSelectionViewModel : PageViewModelBase
{
    #region AvailableTenants
    public Tenant[] AvailableTenants
    {
        get;
        set => SetProperty(ref field, value);
    } = null!;
    #endregion AvailableTenants

    public TenantSelectionViewModel()
    {
        Title = "Login";
    }

    #region SelectCommand
    [field: AllowNull, MaybeNull]
    public AsyncRelayCommand<Tenant> SelectCommand => field ??= new AsyncRelayCommand<Tenant>(ExecuteSelectCommand, CanSelectCommand);
    static bool CanSelectCommand(Tenant? t) => true;
    async Task ExecuteSelectCommand(Tenant? t)
    {
        Debug.Assert(t != null, nameof(t) + " != null");
        await AuthenticationService.CompleteLogin(t);
    }
    #endregion SelectCommand

    #region CancelLoginCommand
    [field: AllowNull, MaybeNull]
    public AsyncRelayCommand CancelLoginCommand => field ??= new AsyncRelayCommand(ExecuteCancelLoginCommand);
    async Task ExecuteCancelLoginCommand()
    {
        var mainNavService = Locator.Current.Resolve<IMainNavigationService>();
        await mainNavService.NavigateWithModuleAsync(
            AccountManagementConstants.ModuleName,
            AccountManagementConstants.LoginView);
    }
    #endregion CancelLoginCommand

    public override bool PersistInHistory() => false;

    public override string Area => "Account";
}