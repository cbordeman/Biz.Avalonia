using Biz.Mobile.Services;
using Biz.Mobile.ViewModels;
using Biz.Mobile.Views;
using Biz.Shell.Platform;
using Biz.Shell.Services;
using CompositeFramework.Avalonia.Dialogs;
using CompositeFramework.Core.Dialogs;
using CompositeFramework.Core.Extensions;
using Microsoft.Identity.Client;
using Splat;

namespace Biz.Shell.iOS.Services;

// ReSharper disable once InconsistentNaming
public class iOsPlatformService : IPlatformService
{
    public void RegisterPlatformTypes()
    {
        // Register Android-specific types, except dialogs, which are 
        // registered in RegisterDialogs().
        SplatRegistrations.RegisterLazySingleton<IPlatformModuleCatalogService, MobileModuleCatalogService>();
        SplatRegistrations.RegisterLazySingleton<IDialogService, AvaloniaDialogService>();
        SplatRegistrations.RegisterLazySingleton<ISafeStorage, iOsSafeStorage>();
        
        // Dialog registration.
        //Locator.CurrentMutable.RegisterDialog<MessageDialogView, MessageDialogViewModel>();
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