using System;
using Avalonia.Controls.ApplicationLifetimes;
using Biz.Mobile.Services;
using Biz.Mobile.ViewModels;
using Biz.Mobile.Views;
using Biz.Models;
using Biz.Shared.ClientLoginProviders;
using Biz.Shared.Infrastructure;
using Biz.Shared.Platform;
using Biz.Shared.Services;
using Biz.Shared.Services.Authentication;
using CompositeFramework.Avalonia;
using CompositeFramework.Avalonia.Dialogs;
using CompositeFramework.Core.Dialogs;
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
        SplatRegistrations.RegisterLazySingleton<IDialogService, AvaloniaDialogService>();
        SplatRegistrations.RegisterLazySingleton<ISafeStorage, AndroidSafeStorage>();
        SplatRegistrations.RegisterLazySingleton<IClientLoginProvider, AndroidMicrosoftLoginProvider>();

        SplatRegistrations.Register<MainSmallView>();
        SplatRegistrations.Register<MainSmallViewModel>();

        
        // Prism style dialog registration.
        //var dialogService = Locator.Current.GetService<IDialogService>();
        //dialogService.RegisterDialog<MessageDialogViewModel, MessageDialogView>(); 
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
