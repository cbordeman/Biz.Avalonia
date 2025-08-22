using Biz.Core.Services;
using Biz.Mobile.Services;
using Biz.Mobile.ViewModels;
using Biz.Mobile.Views;
using Biz.Shell.Infrastructure;
using Biz.Shell.Services;
using MauiServices;
using Prism.Ioc;

namespace Biz.Shell.iOS.Services;

// ReSharper disable once InconsistentNaming
public class iOsPlatformService : IPlatformService
{
    public void RegisterPlatformTypes(IContainerRegistry containerRegistry)
    {
        // Register Android-specific types, except dialogs, which are 
        // registered in RegisterDialogs().
        containerRegistry.RegisterSingleton<IPlatformModuleCatalogService, MobileModuleCatalogService>();
        containerRegistry.RegisterSingleton<IPlatformDialogService, MobileDialogService>();
        containerRegistry.RegisterSingleton<ISafeStorage, MauiSafeStorage>();
        
        // Prism style dialog registration.
        containerRegistry.RegisterDialog<MessageDialogView, MessageDialogViewModel>();
    }

    public void InitializePlatform(IContainerProvider containerProvider)
    {
        
    }
}