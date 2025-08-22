using Biz.Core.Extensions;

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
    
}