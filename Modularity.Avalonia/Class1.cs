using System.ComponentModel.Design;
using System.Net.Mime;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace Modularity.Avalonia;

public static class ServiceCollectionExtensions
{
    public static IServiceCollectionProvider InitializeModularity(
        this IServiceCollection services)
    {
        services.RegisterSingleton
        return new SplatServiceCollectionAdapter();
    }
}
