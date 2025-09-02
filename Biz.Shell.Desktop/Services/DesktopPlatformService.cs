using Biz.Desktop.Services;
using Biz.Shell.Platform;
using Biz.Shell.Services;
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
        containerRegistry.RegisterSingleton<IPlatformMsalService, DesktopMsalService>();
        containerRegistry.RegisterSingleton<PlatformAppCustomUriHandlerBase, DesktopPlatformAppCustomUriHandler>();
    }
    
    public void InitializePlatform(IContainerProvider container)
    {
        // ShadUI dialog registration.
        var dialogService = container.Resolve<DialogManager>();
        //dialogService.Register<LoginContent, LoginViewModel>();
        //dialogService.Register<AboutContent, AboutViewModel>();
    }
}