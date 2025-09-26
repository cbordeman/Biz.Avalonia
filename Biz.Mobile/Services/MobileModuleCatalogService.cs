using Biz.Modules.AccountManagement;
using Biz.Modules.AccountManagement.Core;
using Biz.Modules.Dashboard;
using Biz.Modules.Dashboard.Core;
using Biz.Shell.Platform;
using CompositeFramework.Modules;

namespace Biz.Mobile.Services;

public class MobileModuleCatalogService : IPlatformModuleCatalogService
{
    public void ConfigureModuleCatalog(IModuleIndex moduleCatalog)
    {
        // The call to AddModule() is for the non-directory loaded
        // scenario.  Only desktop supports directory loading.
        moduleCatalog.AddModule(
            DashboardConstants.ModuleName,
            typeof(DashboardModule).AssemblyQualifiedName!);
        moduleCatalog.AddModule(
            AccountManagementConstants.ModuleName,
            typeof(AccountManagementModule).AssemblyQualifiedName!);
    }
}
