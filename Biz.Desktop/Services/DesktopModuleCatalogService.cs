using Biz.Modules.AccountManagement.Core;
using Biz.Modules.Dashboard.Core;
using Biz.Shell.Platform;
using CompositeFramework.Modules;

namespace Biz.Desktop.Services;

public class DesktopModuleCatalogService : IPlatformModuleCatalogService
{
    public void ConfigureModuleCatalog(IModuleIndex moduleIndex)
    {
        moduleIndex.AddModuleFiles("../Modules/*.Module.dll");
    }
}
