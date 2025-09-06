using Biz.Modules.AccountManagement.Core;
using Biz.Modules.AccountManagement.ViewModels;
using Biz.Modules.AccountManagement.Views;
using Prism.Modularity;

namespace Biz.Modules.AccountManagement;

// Module attributes are necessary for directory loading scenario.
[Module(ModuleName = AccountManagementConstants.ModuleName, OnDemand = true)]
//[ModuleDependency()]
public class AccountManagementModule : IModule
{
    public void RegisterTypes(IContainerRegistry containerRegistry)
    { 
        containerRegistry.RegisterForNavigation<LoginView, LoginViewModel>(
            AccountManagementConstants.LoginView);
        containerRegistry.RegisterForNavigation<TenantSelectionView, TenantSelectionViewModel>(
            AccountManagementConstants.TenantSelectionView);
    }
    
    public void OnInitialized(IContainerProvider containerProvider)
    {
        
    }
}