using Biz.Shell.Services.Authentication;

namespace Biz.Shell.Platform;

public interface IPlatformService
{
    /// <summary>
    /// Called before container has been built, in the startup of each platform project.
    /// </summary>
    void RegisterPlatformTypes();

    /// <summary>
    /// Called after container has been built, in App.OnInitialized.
    /// </summary>
    void InitializePlatform();
}