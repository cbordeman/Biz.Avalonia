using System.Collections;
using System.ComponentModel.Design;
using Microsoft.Extensions.DependencyInjection;

namespace Modularity;

public interface IModule
{
    void PerformRegistrations(IServiceContainer services);
    Task InitializedAsync(IServiceProvider provider);
}