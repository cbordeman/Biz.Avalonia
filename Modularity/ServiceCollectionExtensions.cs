using Microsoft.Extensions.DependencyInjection;

namespace Modularity;

public static class ModularityInitialization
{
    /// <summary>
    /// Returns a mutable IServiceCollection and IServiceProvider
    /// implementation. 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection InitializeModularity(
        this IServiceCollection services)
    {
        var instance = new SplatServiceCollectionAdapter();
        services
            .AddSingleton<IServiceCollection>(instance)
            .AddSingleton<IServiceProvider>(instance);
        return services;
    }
}