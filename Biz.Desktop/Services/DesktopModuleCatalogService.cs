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
        
        var matches1 = moduleIndex.AddModuleFilesDirectory(
            rootDir: "../../../../Biz.Modules.AccountManagement/bin/Debug/net9.0",
            includeSpec: "*.Modules.*.dll",
            excludeSpecs: "*.Core.dll");
        var matches2 = moduleIndex.AddModuleFilesDirectory(
            rootDir: "../../../../Biz.Modules.Dashboard/bin/Debug/net9.0",
            includeSpec: "*.Modules.*.dll",
            excludeSpecs: "*.Core.dll");
    }
}