namespace Modularity;

public enum ModuleState
{
    /// <summary>
    /// Not loaded into memory.
    /// </summary>
    NotLoaded,
    
    LoadingDependencies,
    LoadingSelf,
    CreatingInstance,
    InMemory,
    Initializing,
    Initialized
}
