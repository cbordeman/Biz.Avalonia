using Splat;

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
    public static void RegisterRequiredTypes()
    {
        SplatRegistrations.SetupIOC();
        
        SplatRegistrations.RegisterLazySingleton<IModuleManager, ModuleManager>();
        SplatRegistrations.RegisterLazySingleton<IModuleIndex, StandardModuleIndex>();
    }
}
