using System.Reflection;
using Biz.Core;
using Biz.Shell;
using JetBrains.Annotations;

namespace Biz.Platform;

[UsedImplicitly]
public class DesktopModuleCatalogService : IPlatformModuleCatalogService
{
    public IModuleCatalog GetPrismModuleCatalog()
    {
        var compositeCatalog = new CompositeModuleCatalog();

        // Add modules defined in code
        var codeCatalog = new ModuleCatalog();
        // codeCatalog.AddModule(typeof(ModuleA));
        // codeCatalog.AddModule(typeof(ModuleB));
        compositeCatalog.AddCatalog(codeCatalog);

        // Add modules discovered from a directory
        var location = Assembly.GetExecutingAssembly().Location;
        var directoryCatalog = new DirectoryModuleCatalog()
        {
            ModulePath = Path.Combine(location, "Modules")
        };
        compositeCatalog.AddCatalog(directoryCatalog);

        return compositeCatalog; 
    }
}