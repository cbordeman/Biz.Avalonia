using System;
using Avalonia.Controls.ApplicationLifetimes;
using Biz.Mobile.Services;
using Biz.Shell.Platform;
using Biz.Shell.Services;
using CompositeFramework.Avalonia.Dialogs;
using CompositeFramework.Core.Dialogs;
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
        SplatRegistrations.RegisterLazySingleton<IDialogService, AvaloniaDialogService>();
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