using Biz.Models;
using Biz.Shell.ClientLoginProviders;
using Biz.Shell.Infrastructure;
using Biz.Shell.Platform;
using Biz.Shell.Services;
using Biz.Shell.Services.Authentication;
using ShadUI;
using Splat;
using DesktopDialogService = Biz.Shell.Services.DesktopDialogService;

namespace Biz.Shell.Desktop.Services;

public class DesktopPlatformService : IPlatformService
{
    public void RegisterPlatformTypes()
    {
        // Register windows-specific types, except dialogs, which are 
        // registered in RegisterDialogs().
        SplatRegistrations.RegisterLazySingleton<IPlatformModuleCatalogService, DesktopModuleCatalogService>();
        SplatRegistrations.RegisterLazySingleton<DesktopDialogService, Biz.Desktop.Services.DesktopDialogService>();
        SplatRegistrations.RegisterLazySingleton<ISafeStorage, WindowsSafeStorage>();
        SplatRegistrations.RegisterLazySingleton<PlatformAppCustomUriHandlerBase, DesktopPlatformAppCustomUriHandler>();
        SplatRegistrations.RegisterLazySingleton<IClientLoginProvider, DesktopMicrosoftLoginProvider>();
    }
    
    public void InitializePlatform()
    {
        // ShadUI dialog registration.
        var dialogService = Locator.Current.GetService<DialogManager>();
        //dialogService.Register<LoginContent, LoginViewModel>();
        //dialogService.Register<AboutContent, AboutViewModel>();
        
        var authProviderRegistry = Locator.Current.GetService
            <LoginProviderRegistry>();
        authProviderRegistry!.RegisterLoginProvider<DesktopMicrosoftLoginProvider>(
            LoginProvider.Microsoft, "Microsoft", ResourceNames.Microsoft);
    }
}