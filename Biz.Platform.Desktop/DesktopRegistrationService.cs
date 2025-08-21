using Biz.Shell;
using Biz.Shell.Services;
using Biz.Shell;
using Biz.Shell.Infrastructure;
using Prism.Ioc;
using ShadUI;

namespace Biz.Platform;

public class DesktopService : IPlatformService
{
    public void RegisterPlatformTypes(IContainerRegistry containerRegistry)
    {
        // Register desktop-specific types, except dialogs, which are 
        // registered in RegisterDialogs().
        containerRegistry.RegisterSingleton<IPlatformModuleCatalogService,DesktopModuleCatalogService>();
        containerRegistry.RegisterSingleton<IPlatformDialogService,DesktopDialogService>();
    }
    
    public void InitializePlatform(IContainerProvider container)
    {
        // ShadUI dialog registration.
        var dialogService = container.Resolve<DialogManager>();
        //dialogService.Register<LoginContent, LoginViewModel>();
        //dialogService.Register<AboutContent, AboutViewModel>();
    }
}