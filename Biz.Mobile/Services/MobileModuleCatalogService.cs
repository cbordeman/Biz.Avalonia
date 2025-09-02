using Biz.Modules.Dashboard;
using Biz.Modules.Dashboard.Core;
using Biz.Shell.Infrastructure;
using Biz.Shell.Platform;
using Prism.Modularity;

namespace Biz.Mobile.Services;

public class MobileModuleCatalogService : IPlatformModuleCatalogService
{
    public void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        // The call to AddModule() is for the non-directory loaded
        // scenario.  Only desktop supports directory loading.
        moduleCatalog.AddModule(
            DashboardConstants.ModuleName,
            typeof(Module).AssemblyQualifiedName,
            initializationMode: InitializationMode.OnDemand);
    }
}