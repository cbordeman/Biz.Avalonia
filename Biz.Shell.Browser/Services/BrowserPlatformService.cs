using Biz.Desktop.Services;
using Biz.Mobile.Services;
using Biz.Shell.Infrastructure;
using Biz.Shell.Platform;
using Biz.Shell.Services;
using Biz.Shell.Services.Authentication;
using Microsoft.Identity.Client;
using Prism.Ioc;
using DesktopDialogService = Biz.Shell.Services.DesktopDialogService;

namespace Biz.Shell.Browser.Services;

public class BrowserPlatformService : IPlatformService
{
    public void RegisterPlatformTypes()
    {
        // Register Android-specific types, except dialogs, which are 
        // registered in RegisterDialogs().
        SplatRegistrations.RegisterLazySingleton<IPlatformModuleCatalogService, MobileModuleCatalogService>();
        SplatRegistrations.RegisterLazySingleton<DesktopDialogService, Desktop.Services.DesktopDialogService>();
        SplatRegistrations.RegisterLazySingleton<ISafeStorage, WasmSecureStorage>();
    }
    
    public void InitializePlatform()
    {
    }
    
    public AcquireTokenInteractiveParameterBuilder PrepareMsalTokenRequest(AcquireTokenInteractiveParameterBuilder builder)
    {
        // TODO: implement
        return builder;
    }
}