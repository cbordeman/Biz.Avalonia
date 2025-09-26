using Splat;

namespace CompositeFramework.Modules;

public interface IModule
{
    /// <summary>
    /// Called when the module is being lazy loaded.
    /// If you need access to the container, inject it or use
    /// Splat's Locator.Current.
    /// <br/> 
    /// WARNING: Do not use Microsoft's IServiceCollection and
    /// IServiceProvider, which are immutable after the
    /// app.Build() is called.  Splat and DryIoc, among others,
    /// should work well.  CompositeFramework is agnostic as
    /// long as the container allows registrations at the time
    /// modules are loaded.
    /// </summary>
    void PerformRegistrations();
    
    /// <summary>
    /// Called after the module and its dependencies are
    /// lazy loaded via: IModuleManager.Load("ModuleA").
    /// If you need access to the container, inject it or use
    /// Splat's Locator.CurrentMutable and Locator.Current.
    /// </summary>
    /// <returns></returns>
    Task InitializedAsync();
}