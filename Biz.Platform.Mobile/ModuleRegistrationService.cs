using Biz.Shell;
using Biz.Shell.Services;
using Biz.Platform.ViewModels;
using Biz.Platform.Views;
using Biz.Shell;
using Prism.Ioc;

namespace Biz.Platform;

public class MobileRegistrationService : IPlatformService
{
    public void RegisterPlatformTypes(IContainerRegistry containerRegistry)
    {
        // Register mobile-specific types, except dialogs, which are 
        // registered in RegisterDialogs().
        containerRegistry.RegisterSingleton<IPlatformModuleCatalogService, MobileModuleCatalogService>();
        containerRegistry.RegisterSingleton<IPlatformDialogService, MobileDialogService>();
        
        // Prism style dialog registration.
        containerRegistry.RegisterDialog<MessageDialogView, MessageDialogViewModel>();
    }
    
    public void InitializePlatform(IContainerProvider container)
    {
    }
}