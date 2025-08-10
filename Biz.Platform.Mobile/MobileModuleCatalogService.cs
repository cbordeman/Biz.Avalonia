using Biz.Modules.Dashboard;
using Biz.Modules.Dashboard.Core;
using Biz.Shell;
using JetBrains.Annotations;

namespace Biz.Platform;

[UsedImplicitly]
public class MobileModuleCatalogService : IPlatformModuleCatalogService
{
    public void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        // The call to AddModule() is for the non-directory loaded
        // scenario.  Only desktop supports directory loading.
        moduleCatalog.AddModule(
            DashboardConstants.ModuleName,
            typeof(DashboardModule).AssemblyQualifiedName,
            initializationMode: InitializationMode.OnDemand);
    }
}