using Microsoft.Extensions.DependencyInjection;

namespace Biz.Shell.Infrastructure.PrismBridge;

/// <summary>
/// Extension method for bridge registration
/// </summary>
public static class BridgeServiceCollectionExtensions
{
    /// <summary>Bridging service collection registration to Prism containers.</summary>
    /// <param name="registry">Prism service registry</param>
    /// <param name="register">Registration process to the service collection.</param>
    public static void RegisterBridge(this IContainerRegistry registry, Action<IServiceCollection> register)
    {
        var services = new Biz.Shell.Infrastructure.PrismBridge.BridgeServiceCollection(registry);
        registry.RegisterInstance<IServiceCollection>(services);

        register(services);

        var provider = services.Build();
        registry.RegisterInstance<IServiceProvider>(provider);

    }
}
