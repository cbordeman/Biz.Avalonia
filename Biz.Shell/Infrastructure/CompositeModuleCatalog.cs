namespace Biz.Shell.Infrastructure;

public class CompositeModuleCatalog : IModuleCatalog
{
    private readonly List<IModuleCatalog> catalogs = [];
    private readonly List<IModuleInfo> localModules = [];
    private bool isInitialized;

    public IEnumerable<IModuleInfo> Modules
    {
        get
        {
            return catalogs.SelectMany(c => c.Modules).Concat(localModules).ToList();
        }
    }

    /// <summary>
    /// Adds a child IModuleCatalog to this composite catalog.
    /// </summary>
    public void AddCatalog(IModuleCatalog catalog)
    {
        if (isInitialized)
            throw new InvalidOperationException("Cannot add catalogs after initialization.");

        catalogs.Add(catalog ?? throw new ArgumentNullException(nameof(catalog)));
    }

    public IEnumerable<IModuleInfo> GetDependentModules(IModuleInfo moduleInfo)
    {
        if (moduleInfo == null) throw new ArgumentNullException(nameof(moduleInfo));

        var allModules = Modules.ToList();

        // Return modules whose DependsOn contains the module's ModuleName
        return allModules.Where(m => m.DependsOn?.Contains(moduleInfo.ModuleName) ?? false);
    }

    public IEnumerable<IModuleInfo> CompleteListWithDependencies(IEnumerable<IModuleInfo> modules)
    {
        ArgumentNullException.ThrowIfNull(modules);

        var allModules = Modules.ToList();
        var completed = new List<IModuleInfo>();

        foreach (var module in modules)
        {
            AddModuleAndDependencies(module, allModules, completed);
        }

        return completed;
    }

    private void AddModuleAndDependencies(IModuleInfo module, List<IModuleInfo> allModules, List<IModuleInfo> collected)
    {
        if (collected.Contains(module))
            return;

        // Recursively add dependencies first
        var dependencies = allModules.Where(m => module.DependsOn?.Contains(m.ModuleName) ?? false).ToList();
        foreach (var dep in dependencies)
        {
            AddModuleAndDependencies(dep, allModules, collected);
        }

        collected.Add(module);
    }

    public void Initialize()
    {
        if (isInitialized) return;

        foreach (var catalog in catalogs)
        {
            catalog.Initialize();
        }

        isInitialized = true;
    }

    public IModuleCatalog AddModule(IModuleInfo moduleInfo)
    {
        if (isInitialized)
            throw new InvalidOperationException("Cannot add modules after initialization.");

        if (moduleInfo == null)
            throw new ArgumentNullException(nameof(moduleInfo));

        localModules.Add(moduleInfo);
        return this;
    }
}

