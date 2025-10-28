using Splat;

namespace ServiceClients;

public static class ServiceClientRegistration
{
    public static void AddMainApiClients(
        string servicesBaseUrl)
    {
        // Register Refit clients
        RegisterRefitClient<ITenantsApi, ServicesAuthHeaderHandler>(servicesBaseUrl);
        RegisterRefitClient<IAccountApi, ServicesAuthHeaderHandler>(servicesBaseUrl);
    }

    static void RegisterRefitClient<TApi, TDelegatingHandler>(string baseUrl)
        where TApi : class
        where TDelegatingHandler : DelegatingHandler
    {
        // Register factory for making the API, which resolves
        // the HttpClient instance using the container.
        Locator.CurrentMutable.RegisterLazySingleton(() =>
        {
            var handler = Locator.Current.GetService<TDelegatingHandler>();
            var httpClient = new HttpClient(handler: handler!);
            httpClient.BaseAddress = new Uri(baseUrl);
            var svc = Refit.RestService.For<TApi>(httpClient);
            return svc;
        });
    }
}