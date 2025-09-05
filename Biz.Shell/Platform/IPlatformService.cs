using Biz.Shell.Services.Authentication;

namespace Biz.Shell.Platform;

public interface IPlatformService
{
    /// <summary>
    /// Called before container has been built, in the startup of each platform project.
    /// </summary>
    /// <param name="containerRegistry"></param>
    void RegisterPlatformTypes(IContainerRegistry containerRegistry);
    
    /// <summary>
    /// Called after container has been built, in App.OnInitialized.
    /// </summary>
    /// <param name="containerProvider"></param>
    void InitializePlatform(IContainerProvider containerProvider,
        AuthenticationProviderRegistry authProviderRegistry
        );
}