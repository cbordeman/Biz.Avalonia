using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace Modularity;

public interface IServiceCollectionProvider
    : IServiceCollection, IServiceProvider;

/// <summary>
/// Splat wrapper that Implements both IServiceProvider
/// and IServiceCollection, but remains mutable.
/// </summary>
public class SplatServiceCollectionAdapter
    : IServiceCollectionProvider
{
    readonly List<ServiceDescriptor> descriptors = new();

    public SplatServiceCollectionAdapter()
    {
        ((IServiceCollection)this).UseMicrosoftDependencyResolver();
        ((IServiceProvider)this).UseMicrosoftDependencyResolver();
    }
    
    #region IServiceProvider implementation
    public object? GetService(Type serviceType)
        => Locator.Current.GetService(serviceType);
    #endregion IServiceProvider implementation
    
    #region IServiceCollection implementation
    public ServiceDescriptor this[int index]
    {
        get => descriptors[index];
        set => descriptors[index] = value;
    }

    public int Count => descriptors.Count;
    public bool IsReadOnly => false;

    public void Add(ServiceDescriptor item)
    {
        descriptors.Add(item);
        RegisterWithSplat(item);
    }

    public void Clear()
    {
        foreach (var descriptor in descriptors)
            UnregisterWithSplat(descriptor.ServiceType);
        descriptors.Clear();
    }

    public bool Contains(ServiceDescriptor item) => descriptors.Contains(item);
    public void CopyTo(ServiceDescriptor[] array, int arrayIndex) => descriptors.CopyTo(array, arrayIndex);
    public IEnumerator<ServiceDescriptor> GetEnumerator() => descriptors.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int IndexOf(ServiceDescriptor item) => descriptors.IndexOf(item);

    public void Insert(int index, ServiceDescriptor item)
    {
        descriptors.Insert(index, item);
        RegisterWithSplat(item);
    }

    public bool Remove(ServiceDescriptor item)
    {
        bool removed = descriptors.Remove(item);
        if (removed)
            UnregisterWithSplat(item.ServiceType);
        return removed;
    }

    public void RemoveAt(int index)
    {
        var item = descriptors[index];
        descriptors.RemoveAt(index);
        UnregisterWithSplat(item.ServiceType);
    }
    
    void RegisterWithSplat(ServiceDescriptor descriptor)
    {
        // Register according to Microsoft DI lifetimes
        var mutable = Locator.CurrentMutable;
        if (descriptor.ImplementationInstance != null)
        {
            mutable.RegisterConstant(descriptor.ImplementationInstance, descriptor.ServiceType);
        }
        else if (descriptor.ImplementationFactory != null)
        {
            mutable.Register(() => descriptor.ImplementationFactory(this), descriptor.ServiceType);
        }
        else if (descriptor.ImplementationType != null)
        {
            // By default, register as lazy singleton
            mutable.RegisterLazySingleton(
                () =>
                {
                    try
                    {
                        return Activator.CreateInstance(descriptor.ImplementationType!);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Failed to create instance of {descriptor.ImplementationType}", e);
                    }
                }, descriptor.ServiceType);
        }
        else
        {
            throw new InvalidOperationException("Implementation type is null");
        }
    }

    void UnregisterWithSplat(Type serviceType) =>
        Locator.CurrentMutable.UnregisterCurrent(serviceType);
    
    #endregion IServiceCollection implementation
}