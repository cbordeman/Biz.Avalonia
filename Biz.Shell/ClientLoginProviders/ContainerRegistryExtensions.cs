using System.Net.Http;
using Refit;

namespace Biz.Shell.ClientLoginProviders;

public static class ContainerRegistryExtensions
{
    public static IContainerRegistry RegisterRefitClient
        <TApi, TDelegatingHandler>(
        this IContainerRegistry containerRegistry, string baseUrl)
        where TApi : class
        where TDelegatingHandler : DelegatingHandler
    {
        // Register factory for making the API, which resolves
        // the HttpClient instance using the container.
        containerRegistry.RegisterSingleton<TApi>(provider =>
        {
            var handler = provider.Resolve<TDelegatingHandler>();
            var httpClient = new HttpClient(handler: handler);
            httpClient.BaseAddress = new Uri(baseUrl);
            return RestService.For<TApi>(httpClient);
        });
        
        return containerRegistry;
    }
}