namespace Biz.Modules.Dashboard.Core;

[Module(ModuleName = DashboardModuleConstants.ModuleName, OnDemand = true)]
//[ModuleDependency()]
public class DashboardModule : IModule
{
    public void RegisterTypes(IContainerRegistry containerRegistry)
    { 
        containerRegistry.RegisterForNavigation<DashboardView, DashboardViewModel>(DashboardModuleConstants.DashboardView);
    }
    
    public void OnInitialized(IContainerProvider containerProvider)
    {
        
    }
}