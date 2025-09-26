using Biz.Modules.AccountManagement.ViewModels;
using Biz.Modules.AccountManagement.Views;
using CompositeFramework.Modules;
using CompositeFramework.Modules.Attributes;

namespace Biz.Modules.AccountManagement;

// Module attributes are necessary for directory loading scenario.
[Module(AccountManagementConstants.ModuleName)]
//[ModuleDependency()]
public class AccountManagementModule : IModule
{
    public void PerformRegistrations()
    { 
        var contextNavigationService = Locator.Current.Resolve<IContextNavigationService>();
        contextNavigationService.RegisterForNavigation<LoginView, LoginViewModel>(
            AccountManagementConstants.LoginView);
        contextNavigationService.RegisterForNavigation<TenantSelectionView, TenantSelectionViewModel>(
            AccountManagementConstants.TenantSelectionView);
    }
    
    public Task InitializedAsync() => Task.CompletedTask;
}