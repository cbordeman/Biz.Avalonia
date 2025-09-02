using Biz.Modules.Dashboard.Core;
using Prism.Ioc;
using Prism.Modularity;

namespace Biz.Modules.Dashboard;

// Module attributes are necessary for directory loading scenario.
[Module(ModuleName = DashboardConstants.ModuleName, OnDemand = true)]
//[ModuleDependency()]
public class Module : IModule
{
    public void RegisterTypes(IContainerRegistry containerRegistry)
    { 
        containerRegistry.RegisterForNavigation<DashboardView, DashboardViewModel>(
            DashboardConstants.DashboardView);
    }
    
    public void OnInitialized(IContainerProvider containerProvider)
    {
        
    }
}