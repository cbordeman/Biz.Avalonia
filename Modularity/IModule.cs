using Splat;

namespace Modularity;

public interface IModule
{
    void PerformRegistrations(IMutableDependencyResolver services);
    Task AfterInitializationAsync(IReadonlyDependencyResolver provider);
}