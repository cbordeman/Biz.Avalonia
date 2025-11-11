using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Biz.Desktop.Services;
using Biz.Desktop.ViewModels;
using Biz.Desktop.Views;
using Biz.Models;
using Biz.Modules.AccountManagement.Core.Services.Authentication;
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
        SplatRegistrations.RegisterLazySingleton<IDialogService, ShadUiDialogService>();
        SplatRegistrations.RegisterLazySingleton<ISafeStorage, WindowsSafeStorage>();
        SplatRegistrations.RegisterLazySingleton<PlatformAppCustomUriHandlerBase, DesktopPlatformAppCustomUriHandler>();
        SplatRegistrations.RegisterLazySingleton<IClientLoginProvider, DesktopMicrosoftLoginProvider>();
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
            // Avoid duplicate validations from both Avalonia and the
            // CommunityToolkit.  More info:
            // https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

            desktop.MainWindow = Locator.Current.Resolve<MainWindow>();
            desktop.MainWindow.DataContext = Locator.Current.Resolve<MainWindowViewModel>();
        }
        else
            throw new InvalidOperationException("Wrong platform.");
    }
    
    void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
            BindingPlugins.DataValidators.Remove(plugin);
    }
}