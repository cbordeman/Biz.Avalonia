namespace CompositeFramework.Modules;

public enum ModuleState
{
    /// <summary>
    /// Not loaded into memory.
    /// </summary>
    NotLoaded,
    
    LoadingDependencies,
    LoadingSelf,
    CreatingInstance,
    
    /// <summary>
    /// Referenced modules start as InMemory and
    /// are initialized in IModuleManager.Load().
    /// </summary>
    InMemory,
    
    Initializing,
    
    /// <summary>
    /// Fully loaded.
    /// </summary>
    Initialized
}
