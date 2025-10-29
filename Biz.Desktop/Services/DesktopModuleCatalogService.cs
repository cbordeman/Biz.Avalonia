using Biz.Shared.Platform;
using CompositeFramework.Modules;
using Splat;

namespace Biz.Desktop.Services;

public class DesktopModuleCatalogService : IPlatformModuleCatalogService
{
    public void ConfigureModuleIndex()
    {
        var moduleIndex = Locator.Current.GetService<IModuleIndex>();
        if (moduleIndex is null)
            return;
        
        moduleIndex.AddModuleFiles("../Modules/*.Module.dll");
    }
}