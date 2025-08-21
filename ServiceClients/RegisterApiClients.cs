using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace ServiceClients;

public static class RegisterApiClients
{
    public static IServiceCollection AddApiClients(this IServiceCollection services,
        string servicesBaseUrl)
    {
        var baseUri = new Uri(servicesBaseUrl);

        // Register Refit clients
        services.AddRefitClient<ITenantsApi>()
            .ConfigureHttpClient(c => c.BaseAddress = baseUri)
            .AddHttpMessageHandler<ServicesAuthHeaderHandler>();
        services.AddRefitClient<IAccountApi>()
            .ConfigureHttpClient(c => c.BaseAddress = baseUri)
            .AddHttpMessageHandler<ServicesAuthHeaderHandler>();

        // Add other API clients here as needed

        return services;
    }
}