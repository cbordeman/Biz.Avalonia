using System;
using Avalonia.Controls.ApplicationLifetimes;
using Biz.Authentication;
using Biz.Mobile.Services;
using Biz.Shared.Platform;
using Biz.Shared.Services;
using CompositeFramework.Avalonia.Dialogs;
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
        SplatRegistrations.RegisterLazySingleton<ISafeStorage, WasmSecureStorage>();
        SplatRegistrations.RegisterLazySingleton<IDialogService, ShadUiDialogService>();
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
