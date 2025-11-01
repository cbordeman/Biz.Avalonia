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
        SplatRegistrations.SetupIOC();
        
        SplatRegistrations.RegisterLazySingleton<TenantSelectionViewModel>();
        SplatRegistrations.RegisterLazySingleton<TenantSelectionView>();
        SplatRegistrations.RegisterLazySingleton<LoginViewModel>();
        SplatRegistrations.RegisterLazySingleton<LoginView>();
        
        var contextNavigationService = Locator.Current.Resolve<IContextNavigationService>();
        contextNavigationService.RegisterForNavigation<LoginViewModel, LoginView>(
            AccountManagementConstants.LoginView);
        contextNavigationService.RegisterForNavigation<TenantSelectionViewModel, TenantSelectionView>(
            AccountManagementConstants.TenantSelectionView);
    }
    
    public Task InitializedAsync() => Task.CompletedTask;
}