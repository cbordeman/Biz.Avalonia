using System.Reflection;
using CompositeFramework.Modules;
using CompositeFramework.Modules.Exceptions;

namespace CompositeFramework.Modules;

public static class ModularityInitializer
{
    /// <summary>
    /// Registers the required types for Modularity.
    /// Alternatively, you can register your own
    /// IModuleManager and IModuleIndex implementations.
    /// <br/>
    /// Call with a type resolver, which typically invokes
    /// something RegisterSingleton() on the container
    /// of your choice.  The first argument will be an
    /// interface, and the second is the implementing type.
    /// </summary>
    /// <example>
    /// <code>
    /// ModularityInitializer
    ///     .RegisterRequiredTypes((i, ct) => services.RegisterSingleton(t, ct));
    /// </code>
    /// </example>
    public static void RegisterRequiredTypes(
        Action<Type, Type> registerSingletonType)
    {
        registerSingletonType(typeof(IModuleManager), 
            typeof(ModuleManager));
        registerSingletonType(typeof(IModuleIndex), 
            typeof(StandardModuleIndex));
    }
}

public class ModuleManager : IModuleManager
{
    readonly List<ModuleDataAndInstance> loadedModules = new();
    readonly Func<Type, IModule> moduleFactory;

    public IModuleIndex ModuleIndex { get; }
    public IReadOnlyCollection<ModuleDataAndInstance>
        LoadedModules => loadedModules.AsReadOnly();

    public ModuleManager(IModuleIndex moduleIndex,
        Func<Type, IModule> moduleFactory)
    {
        ArgumentChecker.ThrowIfNull(moduleFactory);

        ModuleIndex = moduleIndex;
        this.moduleFactory = moduleFactory;
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

            ArgumentChecker.ThrowIfNullOrEmpty(moduleMetadata.FilePath);
            if (!File.Exists(moduleMetadata.FilePath!))
                throw new FileNotFoundException(moduleMetadata.FilePath);
            try
            {
                moduleMetadata.State = ModuleState.LoadingSelf;
                Assembly.LoadFrom(moduleMetadata.FilePath!);
            }
            catch (Exception e)
            {
                throw new ModuleLoadException(name, moduleMetadata.FilePath!, e);
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
            moduleInstance = moduleFactory(moduleType)!;
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
            await moduleInstance.InitializeAsync().ConfigureAwait(false);
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
