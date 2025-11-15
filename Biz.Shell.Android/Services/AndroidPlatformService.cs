using System;
using Avalonia.Controls.ApplicationLifetimes;
using Biz.Authentication;
using Core;
using Biz.Mobile.Services;
using Biz.Mobile.ViewModels;
using Biz.Mobile.Views;
using Biz.Models;
using Biz.Shared.Infrastructure;
using Biz.Shared.Platform;
using CompositeFramework.Avalonia.Dialogs;
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
        SplatRegistrations.RegisterLazySingleton<IDialogService, AndroidDialogService>();
        SplatRegistrations.RegisterLazySingleton<PlatformAppCustomUriHandlerBase, MobilePlatformAppCustomUriHandler>();
        SplatRegistrations.RegisterLazySingleton<AndroidMicrosoftLoginProvider>();
        
        // Register views and viewmodels
        SplatRegistrations.Register<MainSmallView>();
        SplatRegistrations.Register<MainSmallViewModel>();
    }

    public void InitializePlatform()
    {
        // ShadUI dialog registration.
        var dialogService = Locator.Current.GetService<IDialogService>();
        //dialogService.Register<LoginContent, LoginViewModel>();
        //dialogService.Register<AboutContent, AboutViewModel>();

        var authProviderRegistry = Locator.Current.Resolve
            <ILoginProviderRegistry>();

        try
        {
            authProviderRegistry!.RegisterLoginProvider<AndroidMicrosoftLoginProvider>(
                LoginProvider.Microsoft, "Microsoft", ResourceNames.Microsoft);
        }
        catch (Exception exception)
        {
            throw;
        }
    }
    
    public void OnFrameworkInitializationCompleted(IApplicationLifetime? lifetime)
    {
        if (lifetime is 
            ISingleViewApplicationLifetime singleView)
        {
            singleView.MainView = Locator.Current.Resolve<MainSmallView>();
            singleView.MainView.DataContext = Locator.Current.Resolve<MainSmallViewModel>();
        }
        else
            throw new InvalidOperationException("Wrong platform.");
    }
}