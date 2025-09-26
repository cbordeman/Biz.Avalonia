using Biz.Mobile.Services;
using Biz.Mobile.ViewModels;
using Biz.Mobile.Views;
using Biz.Models;
using Biz.Shell.ClientLoginProviders;
using Biz.Shell.Infrastructure;
using Biz.Shell.Platform;
using Biz.Shell.Services;
using Biz.Shell.Services.Authentication;
using CompositeFramework.Avalonia.Dialogs;
using Splat;
using DesktopDialogService = Biz.Shell.Services.DesktopDialogService;

namespace Biz.Shell.Android.Services;

public class AndroidPlatformService : IPlatformService
{
    public void RegisterPlatformTypes()
    {
        // Register Android-specific types, except dialogs, which are 
        // registered in RegisterDialogs().
        SplatRegistrations.RegisterLazySingleton<IPlatformModuleCatalogService, MobileModuleCatalogService>();
        SplatRegistrations.RegisterLazySingleton<DesktopDialogService, MobileDesktopDialogService>();
        SplatRegistrations.RegisterLazySingleton<ISafeStorage, AndroidSafeStorage>();
        SplatRegistrations.RegisterLazySingleton<IClientLoginProvider, AndroidMicrosoftLoginProvider>();

        // Prism style dialog registration.
        var dialogService = Locator.Current.GetService<IDialogService>();
        dialogService.RegisterDialog<MessageDialogViewModel, MessageDialogView>(); 
    }
    
    public void InitializePlatform()
    {
        var authProviderRegistry = Locator.Current.GetService
            <LoginProviderRegistry>();
        authProviderRegistry!.RegisterLoginProvider<AndroidMicrosoftLoginProvider>(
            LoginProvider.Microsoft, "Microsoft", ResourceNames.Microsoft);
    }
}
