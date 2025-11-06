using System;
using Avalonia.Controls.ApplicationLifetimes;
using Biz.Mobile.Services;
using Biz.Shared.Platform;
using Biz.Shared.Services;
using Splat;

namespace Biz.Shell.iOS.Services;

// ReSharper disable once InconsistentNaming
public class IOsPlatformService : IPlatformService
{
    public void RegisterPlatformTypes()
    {
        SplatRegistrations.SetupIOC();
        
        // Register Android-specific types, except dialogs, which are 
        // registered in RegisterDialogs().
        SplatRegistrations.RegisterLazySingleton<IPlatformModuleCatalogService, MobileModuleCatalogService>();
        SplatRegistrations.RegisterLazySingleton<ISafeStorage, iOsSafeStorage>();
        
        // Dialog registration.
        //Locator.CurrentMutable.RegisterDialog<MessageDialogView, MessageDialogViewModel>();
    }
    public void OnFrameworkInitializationCompleted(IApplicationLifetime? lifetime)
    {
        throw new NotImplementedException();
    }

    public void InitializePlatform()
    {
        throw new NotImplementedException();
    }
}