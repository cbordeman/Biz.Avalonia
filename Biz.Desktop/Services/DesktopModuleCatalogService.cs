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
        
        var matches = moduleIndex.AddModuleFilesDirectory(
            rootDir: "Modules",
            includeSpec: "*.Modules.*.dll",
            excludeSpecs: "*.Core.dll");
    }
}