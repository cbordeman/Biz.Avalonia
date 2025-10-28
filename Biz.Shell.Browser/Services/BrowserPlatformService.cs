using System;
using Avalonia.Controls.ApplicationLifetimes;
using Biz.Mobile.Services;
using Biz.Shared.Platform;
using Biz.Shared.Services;
using CompositeFramework.Avalonia.Dialogs;
using CompositeFramework.Core.Dialogs;
using Microsoft.Identity.Client;
using Splat;

namespace Biz.Shell.Browser.Services;

public class BrowserPlatformService : IPlatformService
{
    public void RegisterPlatformTypes()
    {
        SplatRegistrations.SetupIOC();
        
        // Register Android-specific types, except dialogs, which are 
        // registered in RegisterDialogs().
        SplatRegistrations.RegisterLazySingleton<IPlatformModuleCatalogService, MobileModuleCatalogService>();
        SplatRegistrations.RegisterLazySingleton<IDialogService, AvaloniaDialogService>();
        SplatRegistrations.RegisterLazySingleton<ISafeStorage, WasmSecureStorage>();
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