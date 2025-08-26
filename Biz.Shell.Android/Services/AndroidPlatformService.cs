using Biz.Core.Services;
using Biz.Mobile.Services;
using Biz.Mobile.ViewModels;
using Biz.Mobile.Views;
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
        containerRegistry.RegisterSingleton<ISafeStorage, AndroidSafeStorage>();
        containerRegistry.RegisterSingleton<IPlatformMsalService, AndroidMsalService>();

        // Prism style dialog registration.
        containerRegistry.RegisterDialog<MessageDialogView, MessageDialogViewModel>();
    }

    public void InitializePlatform(IContainerProvider container)
    {
    }
}
