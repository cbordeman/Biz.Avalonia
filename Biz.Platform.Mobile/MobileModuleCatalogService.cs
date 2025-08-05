using System.Reflection;
using Biz.Core;
using Biz.Modules.Dashboard.Core;
using Biz.Shell;
using JetBrains.Annotations;

namespace Biz.Platform;

[UsedImplicitly]
public class MobileModuleCatalogService : IPlatformModuleCatalogService
{
    public IModuleCatalog GetPrismModuleCatalog()
    {
        var codeCatalog = new ModuleCatalog();
        codeCatalog.AddModule(typeof(DashboardModule));
        return codeCatalog; 
    }
}