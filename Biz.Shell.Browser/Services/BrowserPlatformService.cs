using Biz.Desktop.Services;
using Biz.Mobile.Services;
using Biz.Shell.Infrastructure;
using Biz.Shell.Platform;
using Biz.Shell.Services;
using Biz.Shell.Services.Authentication;
using Microsoft.Identity.Client;
using Prism.Ioc;

namespace Biz.Shell.Browser.Services;

public class BrowserPlatformService : IPlatformService
{
    public void RegisterPlatformTypes(IContainerRegistry containerRegistry)
    {
        // Register Android-specific types, except dialogs, which are 
        // registered in RegisterDialogs().
        containerRegistry.RegisterSingleton<IPlatformModuleCatalogService, MobileModuleCatalogService>();
        containerRegistry.RegisterSingleton<IPlatformDialogService, DesktopDialogService>();
        containerRegistry.RegisterSingleton<ISafeStorage, WasmSecureStorage>();
    }
    
    public void InitializePlatform(IContainerProvider container, 
        LoginProviderRegistry authProviderRegistry)
    {
    }
    
    public AcquireTokenInteractiveParameterBuilder PrepareMsalTokenRequest(AcquireTokenInteractiveParameterBuilder builder)
    {
        // TODO: implement
        return builder;
    }
}