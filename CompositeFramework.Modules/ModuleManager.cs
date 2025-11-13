using System.Reflection;
using CompositeFramework.Core;
using CompositeFramework.Modules;
using CompositeFramework.Modules.Exceptions;

namespace CompositeFramework.Modules;

public class ModuleManager : IModuleManager
{
    readonly List<ModuleDataAndInstance> loadedModules = new();

    public IModuleIndex ModuleIndex { get; }
    public IReadOnlyCollection<ModuleDataAndInstance>
        LoadedModules => loadedModules.AsReadOnly();

    public ModuleManager(
        IModuleIndex moduleIndex)
    {
        ModuleIndex = moduleIndex;
    }

    public async Task<ModuleDataAndInstance> LoadModuleAsync(string name)
    {
        var visitedModules = new HashSet<string>();
        var moduleInstance =
            await LoadModuleAndInitializeRecursive(
                    visitedModules,
                    name)
                .ConfigureAwait(false);
        return moduleInstance;
    }

    async Task<ModuleDataAndInstance> LoadModuleAndInitializeRecursive(
        HashSet<string> visitedModules,
        string name)
    {
        // If visited before, stop.
        if (!visitedModules.Add(name))
            return LoadedModules.First(x => x.ModuleData.Name == name);

        var moduleMetadata = ModuleIndex.Modules.FirstOrDefault(x => x.Name == name);
        if (moduleMetadata == null)
            throw new ModuleNotFoundException(name);
        if (moduleMetadata.State == ModuleState.Initialized)
            return LoadedModules.First(x => x.ModuleData.Name == name);
        if (moduleMetadata.State is not ModuleState.NotLoaded
            and not ModuleState.InMemory)
        {
            throw new InvalidOperationException(
                $"Module {name} is in an invalid state " +
                $"{moduleMetadata.State.ToString()!}. " +
                $"should be NotLoaded or InMemory.");
        }

        moduleMetadata.State = ModuleState.LoadingDependencies;

        foreach (var dependency in moduleMetadata.Dependencies)
            await LoadModuleAndInitializeRecursive(visitedModules, dependency)
                .ConfigureAwait(false);

        if (moduleMetadata.State is not ModuleState.InMemory)
        {
            moduleMetadata.State = ModuleState.LoadingSelf;

            if (moduleMetadata.FilePath != null)
            {
                if (!File.Exists(moduleMetadata.FilePath!))
                    throw new FileNotFoundException(moduleMetadata.FilePath);

                try
                {
                    Assembly.LoadFrom(moduleMetadata.FilePath!);
                }
                catch (Exception e)
                {
                    throw new ModuleLoadException(name, moduleMetadata.FilePath!, e);
                }
            }
        }

        moduleMetadata.State = ModuleState.CreatingInstance;

        // Create instance of IModule class.
        Type moduleType;
        try
        {
            moduleType = Type.GetType(moduleMetadata.AssemblyQualifiedName)!;
            if (moduleType == null)
                throw new Exception($"Type.GetType returned null for " +
                                    $"{moduleMetadata.AssemblyQualifiedName}.");
        }
        catch (Exception e)
        {
            throw new CannotFindModuleTypeException(
                moduleMetadata.Name,
                moduleMetadata.AssemblyQualifiedName,
                e);
        }

        IModule? moduleInstance;
        try
        {
            moduleInstance = (IModule)Activator.CreateInstance(moduleType)!;
            if (moduleInstance == null)
                throw new Exception($"GetService({moduleType.FullName}) returned " +
                                    $"null for {moduleType.FullName}.");
        }
        catch (Exception e)
        {
            throw new CannotCreateModuleInstanceException(
                moduleMetadata.Name,
                moduleMetadata.AssemblyQualifiedName,
                e);
        }

        moduleMetadata.State = ModuleState.Initializing;

        try
        {
            moduleInstance.PerformRegistrations();
        }
        catch (Exception e)
        {
            throw new PerformingModuleRegistrationsException(
                moduleMetadata.Name,
                e);
        }

        try
        {
            await moduleInstance.InitializedAsync().ConfigureAwait(false);
            moduleMetadata.State = ModuleState.Initialized;

            var loadedModuleAndInstance = new ModuleDataAndInstance(
                moduleMetadata,
                moduleInstance);
            loadedModules.Add(loadedModuleAndInstance);
            return loadedModuleAndInstance;
        }
        catch (Exception e)
        {
            throw new ModuleInitializationException(
                moduleMetadata.Name,
                e);
        }
    }
}
