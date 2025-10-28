using Biz.Desktop.Services;
using Biz.Models;
using Biz.Shell.ClientLoginProviders;
using Biz.Shell.Infrastructure;
using Biz.Shell.Platform;
using Biz.Shell.Services;
using Biz.Shell.Services.Authentication;
using CompositeFramework.Avalonia.Dialogs;
using CompositeFramework.Core.Dialogs;
using Splat;
using DialogManager = ShadUI.DialogManager;

namespace Biz.Shell.Desktop.Services;

public class DesktopPlatformService : IPlatformService
{
    public void RegisterPlatformTypes()
    {
        SplatRegistrations.SetupIOC();
        
        // Register windows-specific types, except dialogs, which are 
        // registered in RegisterDialogs().
        SplatRegistrations.RegisterLazySingleton<IPlatformModuleCatalogService, DesktopModuleCatalogService>();
        SplatRegistrations.RegisterLazySingleton<IDialogService, AvaloniaDialogService>();
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