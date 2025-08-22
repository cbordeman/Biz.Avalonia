using Biz.Mobile.Services;
using Biz.Platform.ViewModels;
using Biz.Platform.Views;
using Biz.Shell.Infrastructure;
using Biz.Shell.Services;
using Prism.Ioc;

namespace Biz.Shell.Android.Services;

public class AndroidPlatformService : IPlatformService
{
    public void RegisterPlatformTypes(IContainerRegistry containerRegistry)
    {
        // Register Android-specific types, except dialogs, which are 
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