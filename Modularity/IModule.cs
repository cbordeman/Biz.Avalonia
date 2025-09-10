using System.ComponentModel.Design;

namespace Modularity;

public interface IModule
{
    void PerformRegistrations(IServiceContainer services);
    Task InitializedAsync(IServiceProvider provider);
}