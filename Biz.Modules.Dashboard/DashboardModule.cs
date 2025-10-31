using System.Threading.Tasks;
using Biz.Modules.Dashboard.Core;
using CompositeFramework.Core.Navigation;
using CompositeFramework.Modules;
using CompositeFramework.Modules.Attributes;
using Splat;

namespace Biz.Modules.Dashboard;

// Module attributes are necessary for directory loading scenario.
[Module(DashboardConstants.ModuleName)]
//[ModuleDependency()]
public class DashboardModule : IModule
{
    public void PerformRegistrations()
    {
        var contextNavigationService = 
            Locator.Current.GetService<IContextNavigationService>();
        contextNavigationService!.RegisterForNavigation<DashboardViewModel, DashboardView>(
            DashboardConstants.DashboardView);
    }
    
    public Task InitializedAsync()
    {
        return Task.CompletedTask;
    }
}
