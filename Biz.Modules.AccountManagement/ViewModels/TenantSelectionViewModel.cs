using System.Diagnostics.CodeAnalysis;
using Biz.Modules.AccountManagement.Core;
using DryIoc;
using JetBrains.Annotations;
using Prism.Commands;

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

    public TenantSelectionViewModel(IContainer container) : base(container)
    {
        Title = "Login";
    }

    #region SelectCommand
    [field: AllowNull, MaybeNull]
    public AsyncRelayCommand<Tenant> SelectCommand => field ??= new AsyncRelayCommand<Tenant>(ExecuteSelectCommand, CanSelectCommand);
    static bool CanSelectCommand(Tenant t) => true;
    async Task ExecuteSelectCommand(Tenant t)
    {
        await AuthenticationService.CompleteLogin(t);
    }
    #endregion SelectCommand

    #region CancelLoginCommand
    [field: AllowNull, MaybeNull]
    public AsyncRelayCommand CancelLoginCommand => field ??= new AsyncRelayCommand(ExecuteCancelLoginCommand);
    Task ExecuteCancelLoginCommand()
    {
        NavigationService!.RequestNavigate(
            AccountManagementConstants.ModuleName,
            AccountManagementConstants.LoginView);
        return Task.CompletedTask;
    }
    #endregion CancelLoginCommand

    public override bool PersistInHistory() => false;
}
