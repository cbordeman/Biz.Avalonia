using Biz.Modules.Dashboard.Core;
using Biz.Shell;
using JetBrains.Annotations;

namespace Biz.Platform;

[UsedImplicitly]
public class MobileModuleCatalogService : IPlatformModuleCatalogService
{
    public void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        moduleCatalog.AddModule(
            DashboardModuleConstants.ModuleName,
            typeof(DashboardModule).AssemblyQualifiedName,
            InitializationMode.OnDemand);
    }
}