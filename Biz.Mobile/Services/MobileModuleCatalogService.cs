using Biz.Modules.AccountManagement;
using Biz.Modules.AccountManagement.Core;
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
            typeof(DashboardModule).AssemblyQualifiedName,
            initializationMode: InitializationMode.OnDemand);
        moduleCatalog.AddModule(
            AccountManagementConstants.ModuleName,
            typeof(AccountManagementModule).AssemblyQualifiedName,
            initializationMode: InitializationMode.OnDemand);
    }
}