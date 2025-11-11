using System;
using Avalonia.Controls.ApplicationLifetimes;
using Biz.Authentication;
using Biz.Authentication.ClientLoginProvider;
using Biz.Mobile.Services;
using Biz.Mobile.ViewModels;
using Biz.Mobile.Views;
using Biz.Models;
using Biz.Shared.Infrastructure;
using Biz.Shared.Platform;
using CompositeFramework.Avalonia;
using CompositeFramework.Core.Extensions;
using Splat;

namespace Biz.Shell.Android.Services;

public class AndroidPlatformService : IPlatformService
{
    public void RegisterPlatformTypes()
    {
        SplatRegistrations.SetupIOC();
        
        // Register Android-specific types, except dialogs, which are 
        // registered in RegisterDialogs().
        SplatRegistrations.RegisterLazySingleton<IPlatformModuleCatalogService, MobileModuleCatalogService>();
        SplatRegistrations.RegisterLazySingleton<ISafeStorage, AndroidSafeStorage>();
        SplatRegistrations.RegisterLazySingleton<IClientLoginProvider, AndroidMicrosoftLoginProvider>();

        SplatRegistrations.Register<MainSmallView>();
        SplatRegistrations.Register<MainSmallViewModel>();
    }

    public void OnFrameworkInitializationCompleted(IApplicationLifetime? lifetime)
    {
        if (lifetime is
            ISingleViewApplicationLifetime singleView)
        {
            singleView.MainView = Locator.Current.Resolve<MainSmallView>();
            ViewModelLocator.SetAutoWireViewModel(singleView.MainView!, true);
        }
        else
            throw new InvalidOperationException("Wrong platform.");
    }
    
    public void InitializePlatform()
    {
        var authProviderRegistry = Locator.Current.GetService
            <LoginProviderRegistry>();
        authProviderRegistry!.RegisterLoginProvider<AndroidMicrosoftLoginProvider>(
            LoginProvider.Microsoft, "Microsoft", ResourceNames.Microsoft);
    }
}
