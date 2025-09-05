using Biz.Core.Models;
using Biz.Desktop.Services;
using Biz.Shell.ClientLoginProviders;
using Biz.Shell.Desktop.Services.Auth;
using Biz.Shell.Infrastructure;
using Biz.Shell.Platform;
using Biz.Shell.Services;
using Biz.Shell.Services.Authentication;
using Prism.Ioc;
using ShadUI;

namespace Biz.Shell.Desktop.Services;

public class DesktopPlatformService : IPlatformService
{
    public void RegisterPlatformTypes(IContainerRegistry containerRegistry)
    {
        // Register windows-specific types, except dialogs, which are 
        // registered in RegisterDialogs().
        containerRegistry.RegisterSingleton<IPlatformModuleCatalogService, DesktopModuleCatalogService>();
        containerRegistry.RegisterSingleton<IPlatformDialogService, DesktopDialogService>();
        containerRegistry.RegisterSingleton<ISafeStorage, WindowsSafeStorage>();
        containerRegistry.RegisterSingleton<PlatformAppCustomUriHandlerBase, DesktopPlatformAppCustomUriHandler>();
        containerRegistry.RegisterSingleton<IClientLoginProvider, MicrosoftLoginProvider>();
    }
    
    public void InitializePlatform(IContainerProvider container,
        AuthenticationProviderRegistry authProviderRegistry)
    {
        // ShadUI dialog registration.
        var dialogService = container.Resolve<DialogManager>();
        //dialogService.Register<LoginContent, LoginViewModel>();
        //dialogService.Register<AboutContent, AboutViewModel>();
        
        authProviderRegistry.RegisterLoginProvider<MicrosoftLoginProvider>(
            LoginProvider.Microsoft, "Microsoft", ResourceNames.Microsoft);
    }
}