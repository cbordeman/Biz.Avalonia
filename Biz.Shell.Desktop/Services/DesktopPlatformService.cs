using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Biz.Authentication;
using Biz.Authentication.ClientLoginProvider;
using Core;
using Biz.Desktop.Services;
using Biz.Desktop.ViewModels;
using Biz.Desktop.Views;
using Biz.Models;
using Biz.Shared.Infrastructure;
using Biz.Shared.Platform;
using Biz.Shared.Services;
using CompositeFramework.Avalonia.Dialogs;
using CompositeFramework.Core.Extensions;

namespace Biz.Shell.Desktop.Services;

public class DesktopPlatformService : IPlatformService
{
    public void RegisterPlatformTypes()
    {
        SplatRegistrations.SetupIOC();
        
        // Register windows-specific types, except dialogs, which are 
        // registered in RegisterDialogs().
        SplatRegistrations.RegisterLazySingleton<IPlatformModuleCatalogService, DesktopModuleCatalogService>();
        SplatRegistrations.RegisterLazySingleton<ISafeStorage, WindowsSafeStorage>();
        SplatRegistrations.RegisterLazySingleton<IDialogService, ShadUiDialogService>();
        SplatRegistrations.RegisterLazySingleton<PlatformAppCustomUriHandlerBase, DesktopPlatformAppCustomUriHandler>();
        SplatRegistrations.RegisterLazySingleton<DesktopMicrosoftLoginProvider>();
        
        // Register views and viewmodels
        SplatRegistrations.RegisterLazySingleton<MainWindow>();
        SplatRegistrations.RegisterLazySingleton<MainWindowViewModel>();
        SplatRegistrations.RegisterLazySingleton<MainLargeView>();
        SplatRegistrations.RegisterLazySingleton<MainLargeViewModel>();
    }
    
    public void InitializePlatform()
    {
        // ShadUI dialog registration.
        var dialogService = Locator.Current.GetService<IDialogService>();
        //dialogService.Register<LoginContent, LoginViewModel>();
        //dialogService.Register<AboutContent, AboutViewModel>();

        var authProviderRegistry = Locator.Current.Resolve
            <ILoginProviderRegistry>();
        authProviderRegistry!.RegisterLoginProvider<DesktopMicrosoftLoginProvider>(
            LoginProvider.Microsoft, "Microsoft", ResourceNames.Microsoft);
    }

    /// <summary>
    /// Called last, must create the
    /// main view and assign it DataContext.
    /// </summary>
    public void OnFrameworkInitializationCompleted(IApplicationLifetime? lifetime)
    {
        if (lifetime is 
            IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = Locator.Current.Resolve<MainWindow>();
            desktop.MainWindow.DataContext = Locator.Current.Resolve<MainWindowViewModel>();
        }
        else
            throw new InvalidOperationException("Wrong platform.");
    }
}