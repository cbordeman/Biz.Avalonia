using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Biz.Shell.Infrastructure.PrismBridge;

/// <summary>
/// A service collection that bridges to Prism's service registry.
/// This class transforms a simple registration process into a Prism registration.
/// It does not support unregistration etc.
/// It is intended to be used implicitly in <see cref="BridgeServiceCollectionExtensions.RegisterBridge"/> extension methods.
/// </summary>
public sealed class BridgeServiceCollection : IServiceCollection, IDisposable
{
    /// <summary>A constructor that associates with the Prism service registry.</summary>
    /// <param name="containerRegistry">Prism service registry.</param>
    public BridgeServiceCollection(IContainerRegistry containerRegistry)
    {
        registry = containerRegistry;
        services.BuildServiceProvider();
    }

    /// <inheritdoc />
    public ServiceDescriptor this[int index]
    {
        get => services[index];
        set => services[index] = value;
    }

    /// <inheritdoc />
    public int Count => services.Count;

    /// <inheritdoc />
    public bool IsReadOnly => services.IsReadOnly;

    /// <inheritdoc />
    public void Add(ServiceDescriptor? item)
    {
        item.ShouldNotBeNull();
        ((IServiceCollection)services).Add(item);

        BridgeRegistry(item);
    }

    /// <inheritdoc />
    public void Insert(int index, ServiceDescriptor? item)
    {
        item.ShouldNotBeNull();
        services.Insert(index, item);

        BridgeRegistry(item);
    }

    /// <inheritdoc />
    public bool Remove(ServiceDescriptor item)
    {
        return services.Remove(item);
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        services.RemoveAt(index);
    }

    /// <inheritdoc />
    public void Clear()
    {
        services.Clear();
    }

    /// <inheritdoc />
    public bool Contains(ServiceDescriptor item)
    {
        return services.Contains(item);
    }

    /// <inheritdoc />
    public int IndexOf(ServiceDescriptor item)
    {
        return services.IndexOf(item);
    }

    /// <inheritdoc />
    public IEnumerator<ServiceDescriptor> GetEnumerator()
    {
        return services.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return services.GetEnumerator();
    }

    /// <inheritdoc />
    public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
    {
        services.CopyTo(array, arrayIndex);
    }

    /// <summary>Build a service provider.</summary>
    /// <returns>Service provider</returns>
    public ServiceProvider Build()
    {
        provider = services.BuildServiceProvider();
        return provider;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(disposing: true);
        // ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
        GC.SuppressFinalize(this);
    }

    void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                provider?.Dispose();
                provider = null;
            }

            disposedValue = true;
        }
    }

    readonly IContainerRegistry registry;
    readonly ServiceCollection services = [];
    ServiceProvider? provider;
    bool disposedValue;

    void BridgeRegistry(ServiceDescriptor? item)
    {
        if (item == null) return;

        if (item.ImplementationInstance != null)
        {
            registry.RegisterInstance(item.ServiceType, item.ImplementationInstance);
        }
        else if (item.ImplementationFactory != null)
        {
            switch (item.Lifetime)
            {
                case ServiceLifetime.Singleton: 
                    registry.RegisterSingleton(item.ServiceType, _ => item.ImplementationFactory(provider ?? throw new InvalidOperationException())); 
                    break;
                case ServiceLifetime.Scoped: 
                    registry.RegisterScoped(item.ServiceType, _ => item.ImplementationFactory(provider!)); 
                    break;
                case ServiceLifetime.Transient: 
                    registry.Register(item.ServiceType, _ => item.ImplementationFactory(provider!)); 
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else
        {
            item.ImplementationType.ShouldNotBeNull();
            switch (item.Lifetime)
            {
                case ServiceLifetime.Singleton: 
                    registry.RegisterSingleton(item.ServiceType, item.ImplementationType); 
                    break;
                case ServiceLifetime.Scoped: 
                    registry.RegisterScoped(item.ServiceType, item.ImplementationType); 
                    break;
                case ServiceLifetime.Transient: 
                    registry.Register(item.ServiceType, item.ImplementationType); 
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
