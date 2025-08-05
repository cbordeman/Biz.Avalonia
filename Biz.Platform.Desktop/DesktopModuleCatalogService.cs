using System.Diagnostics;
using System.Reflection;
using Biz.Core;
using Biz.Shell;
using JetBrains.Annotations;

namespace Biz.Platform;

[UsedImplicitly]
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

        // Add modules discovered from a directory
        var ass = Assembly.GetExecutingAssembly();
        var location = ass.Location;
        var sourceDirectory = Path.GetDirectoryName(location);
        string mods;
        if (Debugger.IsAttached)
        {
            mods = sourceDirectory!.Replace(".Shell.Desktop", ".Shell");
            mods = Path.Combine(mods, "Modules");
        }
        else
            mods = Path.Combine(sourceDirectory!, "Modules");
        var directoryCatalog = new DirectoryModuleCatalog() { ModulePath = mods };
        compositeCatalog.AddCatalog(directoryCatalog);
    }
}