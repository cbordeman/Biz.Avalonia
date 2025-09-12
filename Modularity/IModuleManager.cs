using System.Reflection;
using Modularity.Exceptions;
using Splat;

namespace Modularity;

public interface IModuleManager
{
    IModuleDirectory ModuleDirectory { get; }
    IReadOnlyCollection<ModuleInstance> LoadedModules { get; }
    Task<ModuleInstance> LoadModuleAsync(string name);
}

public class ModuleManager : IModuleManager
{
    readonly List<ModuleInstance> loadedModules = new();
    readonly IMutableDependencyResolver container;
    
    public IModuleDirectory ModuleDirectory { get; }
    public IReadOnlyCollection<ModuleInstance> LoadedModules =>
        loadedModules.AsReadOnly();
    
    public ModuleManager(IModuleDirectory moduleDirectory,
        IMutableDependencyResolver container)
    {
        ArgumentChecker.ThrowIfNull(moduleDirectory);
        ArgumentChecker.ThrowIfNull(container);

        ModuleDirectory = moduleDirectory;
        this.container = container;
    }

    public async Task<ModuleInstance> LoadModuleAsync(string name)
    {
        var visitedModules = new HashSet<string>();
        var moduleInstance =
            await LoadModuleAndInitializeRecursive(
                    visitedModules,
                    name)
                .ConfigureAwait(false);
        return moduleInstance;
    }

    async Task<ModuleInstance> LoadModuleAndInitializeRecursive(
        HashSet<string> visitedModules,
        string name)
    {
        // If visited before, skip.
        if (!visitedModules.Add(name))
            return LoadedModules.First(x => x.ModuleData.Name == name);

        var moduleData = ModuleDirectory.Modules.FirstOrDefault(x => x.Name == name);
        if (moduleData == null)
            throw new ModuleNotFoundException(name);
        if (moduleData.State == ModuleState.Initialized)
            return LoadedModules.First(x => x.ModuleData.Name == name);
        if (moduleData.State is not ModuleState.NotLoaded
            and not ModuleState.InMemory)
        {
            throw new InvalidOperationException(
                $"Module {name} is in an invalid state " +
                $"{moduleData.State.ToString()!}. " +
                $"should be NotLoaded or InMemory.");
        }

        moduleData.State = ModuleState.LoadingDependencies;

        foreach (var dependency in moduleData.Dependencies)
            await LoadModuleAndInitializeRecursive(visitedModules, dependency)
                .ConfigureAwait(false);

        if (moduleData.State is not ModuleState.InMemory)
        {
            moduleData.State = ModuleState.LoadingSelf;

            ArgumentChecker.ThrowIfNullOrEmpty(moduleData.FullPath);
            if (!File.Exists(moduleData.FullPath!))
                throw new FileNotFoundException(moduleData.FullPath);
            try
            {
                moduleData.State = ModuleState.LoadingSelf;
                Assembly.LoadFrom(moduleData.FullPath!);
            }
            catch (Exception e)
            {
                throw new ModuleLoadException(name, moduleData.FullPath!, e);
            }
        }

        moduleData.State = ModuleState.CreatingInstance;

        // Create instance of IModule class.
        Type moduleType;
        try
        {
            moduleType = Type.GetType(moduleData.AssemblyQualifiedName)!;
            if (moduleType == null)
                throw new Exception($"Type.GetType returned null for " +
                                    $"{moduleData.AssemblyQualifiedName}.");
        }
        catch (Exception e)
        {
            throw new CannotFindModuleTypeException(
                moduleData.Name,
                moduleData.AssemblyQualifiedName,
                e);
        }

        IModule? moduleInstance;
        try
        {
            moduleInstance = (IModule)AppLocator.Current.GetService(moduleType)!;
            if (moduleInstance == null)
                throw new Exception($"GetService({moduleType.FullName}) returned " +
                                    $"null for {moduleType.FullName}.");
        }
        catch (Exception e)
        {
            throw new CannotCreateModuleInstanceException(
                moduleData.Name,
                moduleData.AssemblyQualifiedName, e);
        }

        loadedModules.Add(new ModuleInstance(moduleData, moduleInstance));
        
        moduleData.State = ModuleState.Initializing;

        try
        {
            moduleInstance.PerformRegistrations(AppLocator.CurrentMutable);
        }
        catch (Exception e)
        {
            throw new PerformingModuleRegistrationsException(
                moduleData.Name, e);
        }

        try
        {
            await moduleInstance.AfterInitializationAsync(
                AppLocator.Current).ConfigureAwait(false);
            moduleData.State = ModuleState.Initialized;
            return new ModuleInstance(moduleData, moduleInstance);
        }
        catch (Exception e)
        {
            throw new ModuleInitializationException(
                moduleData.Name, e);
        }
    }
}