using Biz.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace ServiceClients;

public static class AddApiClientsExtensions
{
    public static IContainerRegistry AddMainApiClients(this IContainerRegistry containerRegistry,
        string servicesBaseUrl)
    {
        // Register Refit clients
        return containerRegistry
            .RegisterRefitClient<ITenantsApi, ServicesAuthHeaderHandler>(servicesBaseUrl)
            .RegisterRefitClient<IAccountApi, ServicesAuthHeaderHandler>(servicesBaseUrl);
    }

    public static Add()
    {
        IServiceCollection sc = new ServiceCollection();
        sc.AddRefitClient<ITenantsApi>();
    }
    
}