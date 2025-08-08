using Biz.Core;
using Biz.Core.Services;
using Biz.Shell;

namespace Biz.Platform;

public class DesktopRegistrationService : IPlatformRegistrationService
{
    public void RegisterPlatformTypes(IContainerRegistry containerRegistry)
    {
        // Register desktop-specific types
        containerRegistry.RegisterSingleton<IPlatformModuleCatalogService,DesktopModuleCatalogService>();
        containerRegistry.RegisterSingleton<IPlatformDialogService,DesktopDialogService>();
    }
}