namespace CompositeFramework.Modules;

public interface IModuleManager
{
    /// <summary>
    /// The index implementation being used.
    /// </summary>
    IModuleIndex ModuleIndex { get; }
    
    /// <summary>
    /// A readonly list of all the fully loaded and initialized
    /// modules.
    /// </summary>
    IReadOnlyCollection<ModuleDataAndInstance> LoadedModules { get; }
    
    /// <summary>
    /// Loads module into memory and initializes it.
    /// </summary>
    /// <param name="name">The name passed to in IModuleIndex.AddModule()
    /// or, if loaded from a file, the [Module] attribute.</param>
    /// <returns></returns>
    Task<ModuleDataAndInstance> LoadModuleAsync(string name);
}