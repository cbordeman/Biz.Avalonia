using Splat;

namespace CompositeFramework.Modules;

public interface IModule
{
    /// <summary>
    /// Inject your container in your module constructor and
    /// perform registrations.
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
    /// Called after the module is fully loaded by:
    /// IModuleManager.Load("ModuleA").  If you need
    /// access to your container, inject it or use a
    /// service locator.
    /// </summary>
    /// <returns></returns>
    Task InitializeAsync();
}