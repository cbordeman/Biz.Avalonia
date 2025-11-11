using Biz.Authentication;
using Biz.Modules.AccountManagement.ViewModels;
using Biz.Modules.AccountManagement.Views;
using CompositeFramework.Modules;
using CompositeFramework.Modules.Attributes;
using ServiceClients;

namespace Biz.Modules.AccountManagement;

// Module attributes are necessary for directory loading scenario.
[Module(AccountManagementConstants.ModuleName)]
//[ModuleDependency()]
public class AccountManagementModule : IModule
{
    public void PerformRegistrations()
    {
        SplatRegistrations.SetupIOC();
        
        SplatRegistrations.Register<TenantSelectionViewModel>();
        SplatRegistrations.Register<TenantSelectionView>();
        SplatRegistrations.Register<LoginViewModel>();
        SplatRegistrations.Register<LoginView>();

        var contextNavigationService = Locator.Current.Resolve<ISectionNavigationService>();
        contextNavigationService.RegisterForNavigation<LoginViewModel, LoginView>(
            AccountManagementConstants.LoginView);
        contextNavigationService.RegisterForNavigation<TenantSelectionViewModel, TenantSelectionView>(
            AccountManagementConstants.TenantSelectionView);

        // Dialog registration.
        //var dialogService = Locator.Current.Resolve<IDialogService>();
        // dialogService.RegisterDialog<LoginViewModel, LoginView>(
        //     AccountManagementConstants.LoginView);
    }
    
    public Task InitializedAsync() => Task.CompletedTask;
}