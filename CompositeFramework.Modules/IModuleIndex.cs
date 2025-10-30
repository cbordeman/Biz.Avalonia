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
    /// Add all modules that fit the given file spec in a location on disk.
    /// Can be called multiple times, and supports exclude specs.
    /// Requires the [Module] and [ModuleDependency] attributes
    /// be present on your IModule class.
    /// </summary>
    /// <example>
    /// <code>
    /// AddModuleFiles("../Modules",
    ///     includeSpec: "*.Module.dll", excludeSpec: ["*.Core.dll"]);
    /// </code>
    /// </example>
    IEnumerable<string> AddModuleFilesDirectory(string rootDir, string includeSpec, params IEnumerable<string> excludeSpecs);
}