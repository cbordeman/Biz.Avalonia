using Biz.Mobile.Services;
using Biz.Mobile.ViewModels;
using Biz.Mobile.Views;
using Biz.Shell.Infrastructure;
using Biz.Shell.Platform;
using Biz.Shell.Services;
using Biz.Shell.Services.Authentication;
using CompositFramework.Avalonia.Dialogs;
using Microsoft.Identity.Client;
using Prism.Ioc;
using Splat;
using DesktopDialogService = Biz.Shell.Services.DesktopDialogService;

namespace Biz.Shell.iOS.Services;

// ReSharper disable once InconsistentNaming
public class iOsPlatformService : IPlatformService
{
    public void RegisterPlatformTypes()
    {
        // Register Android-specific types, except dialogs, which are 
        // registered in RegisterDialogs().
        SplatRegistrations.RegisterLazySingleton<IPlatformModuleCatalogService, MobileModuleCatalogService>();
        SplatRegistrations.RegisterLazySingleton<DesktopDialogService, MobileDesktopDialogService>();
        SplatRegistrations.RegisterLazySingleton<ISafeStorage, iOsSafeStorage>();
        
        // Prism style dialog registration.
        containerRegistry.RegisterDialog<MessageDialogView, MessageDialogViewModel>();
    }

    public void InitializePlatform()
    {
        
    }
 
    public AcquireTokenInteractiveParameterBuilder PrepareMsalTokenRequest(AcquireTokenInteractiveParameterBuilder builder)
    {
        // TODO: Implement.
        return builder;
    }
}