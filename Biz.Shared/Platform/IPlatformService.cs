using Biz.Shared.Services.Authentication;

namespace Biz.Shared.Platform;

public interface IPlatformService
{
    /// <summary>
    /// Called first, before container has been built.
    /// </summary>
    void RegisterPlatformTypes();

    /// <summary>
    /// Called second, after container has been built.
    /// </summary>
    void InitializePlatform();
    
    /// <summary>
    /// Called last, must create the
    /// main view and assign it DataContext.
    /// </summary>
    void OnFrameworkInitializationCompleted(IApplicationLifetime? lifetime);
}