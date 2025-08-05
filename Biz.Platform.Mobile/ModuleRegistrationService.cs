using Biz.Core;
using Biz.Shell;

namespace Biz.Platform;

public class MobileRegistrationService : IPlatformRegistrationService
{
    public void RegisterPlatformTypes(IContainerRegistry containerRegistry)
    {
        // Register mobile-specific types
        containerRegistry.RegisterSingleton<IPlatformModuleCatalogService, MobileModuleCatalogService>();
    }
}