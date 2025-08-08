namespace Biz.Modules.Dashboard.Core;

// Module attributes are necessary for directory loading scenario.
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