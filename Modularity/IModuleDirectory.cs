namespace Modularity;

public interface IModuleDirectory
{
    IReadOnlyCollection<ModuleData> Modules { get; }
    void AddModule(string name,
        string assemblyQualifiedType,
        params string[] dependencies);
    Task AddModuleDirectory(string fileSpec);
}