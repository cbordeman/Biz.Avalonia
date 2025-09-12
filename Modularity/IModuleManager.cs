namespace Modularity;

public interface IModuleManager
{
    IModuleDirectory ModuleDirectory { get; }
    IReadOnlyCollection<ModuleDataAndInstance> LoadedModules { get; }
    Task<ModuleDataAndInstance> LoadModuleAsync(string name);
}