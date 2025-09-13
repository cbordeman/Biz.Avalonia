namespace CompositeFramework.Modules;

public interface IModuleIndex
{
    /// <summary>
    /// A read only list of the Modules that have been loaded
    /// and initialized.
    /// </summary>
    IReadOnlyCollection<ModuleMetadata> Modules { get; }
    
    /// <summary>
    /// Adds but does not yet initializes a given module
    /// that is referenced by your application.
    /// Its initial state will be InMemory.
    /// </summary>
    /// <example>
    /// <code>
    /// AddModule("ModuleA",
    ///     typeof(ModuleA).AssemblyQualifyingType,
    ///     "ParentModule");
    /// </code>
    /// </example>
    void AddModule(string name,
        string assemblyQualifiedType,
        params string[] dependencies);
    
    /// <summary>
    /// Add all modules that fit the given file spec and
    /// directory on disk.  Can be called multiple times.
    /// Uses the [Module] and [ModuleDependency] attributes
    /// on your IModule class.
    /// </summary>
    /// <example>
    /// <code>
    /// AddModuleFiles("../Modules/*.Module.dll");
    /// </code>
    /// </example>
    Task AddModuleFiles(string fileSpec);
}