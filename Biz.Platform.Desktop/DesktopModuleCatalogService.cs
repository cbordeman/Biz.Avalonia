using System.Diagnostics;
using System.IO;
using System.Reflection;
using Biz.Shell;
using Biz.Shell;
using Biz.Shell.Infrastructure;
using JetBrains.Annotations;
using Prism.Modularity;

namespace Biz.Platform;

public class DesktopModuleCatalogService : IPlatformModuleCatalogService
{
    public void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        var compositeCatalog = (CompositeModuleCatalog)moduleCatalog;

        // Add modules defined in code
        var codeCatalog = new ModuleCatalog();
        // codeCatalog.AddModule(typeof(ModuleA));
        // codeCatalog.AddModule(typeof(ModuleB));
        compositeCatalog.AddCatalog(codeCatalog);

        // Add modules discovered from the Modules subdirectory.
        var ass = Assembly.GetExecutingAssembly();
        var modulesPath = Path.Combine(Path.GetDirectoryName(ass.Location)!, "Modules");
        var directoryCatalog = new DirectoryModuleCatalog() { ModulePath = modulesPath };
        compositeCatalog.AddCatalog(directoryCatalog);
    }
}