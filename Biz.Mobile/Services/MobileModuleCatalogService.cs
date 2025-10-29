using Biz.Modules.AccountManagement;
using Biz.Modules.AccountManagement.Core;
using Biz.Modules.Dashboard;
using Biz.Modules.Dashboard.Core;
using Biz.Shared.Platform;
using CompositeFramework.Modules;
using Splat;

namespace Biz.Mobile.Services;

public class MobileModuleCatalogService : IPlatformModuleCatalogService
{
    public void ConfigureModuleIndex()
    {
        var moduleIndex = Locator.Current.GetService<IModuleIndex>();
        if (moduleIndex is null)
            return;
        
        // The call to AddModule() is for the non-directory loaded
        // scenario.  Only desktop supports directory loading.
        moduleIndex.AddModule(
            DashboardConstants.ModuleName,
            typeof(DashboardModule).AssemblyQualifiedName!);
        moduleIndex.AddModule(
            AccountManagementConstants.ModuleName,
            typeof(AccountManagementModule).AssemblyQualifiedName!);
    }
}
