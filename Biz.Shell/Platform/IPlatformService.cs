using Biz.Shell.Services.Authentication;

namespace Biz.Shell.Platform;

public interface IPlatformService
{
    /// <summary>
    /// Called before container has been built, in the startup of each platform project.
    /// </summary>
    void RegisterPlatformTypes();

    /// <summary>
    /// Called after container is built, must create the
    /// main view and assign it DataContext.
    /// </summary>
    void OnFrameworkInitializationCompleted(IApplicationLifetime? lifetime);
    
    /// <summary>
    /// Called after container has been built, in App.OnInitialized.
    /// </summary>
    void InitializePlatform();
}