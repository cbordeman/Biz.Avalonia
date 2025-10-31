using Biz.Shared.Platform;
using Biz.Shared.Services.Authentication;
using Biz.Shared.Services.Config;
using CompositeFramework.Avalonia;
using CompositeFramework.Avalonia.Navigation;
using CompositeFramework.Avalonia.Sections;
using Microsoft.Maui.Accessibility;
using ServiceClients;
using ShadUI;

namespace Biz.Shared;

public partial class App : Application
{
    public override void Initialize()
    {
        try
        {
            AvaloniaXamlLoader.Load(this);

            PerformRegistrations();
            
            // initialize module index.
            var moduleCatalogService = Locator.Current.GetService<IPlatformModuleCatalogService>();
            moduleCatalogService?.ConfigureModuleIndex();
            
            PlatformHelper.PlatformService?.InitializePlatform();
            
            var authService = Locator.Current.GetService<IAuthenticationService>();
            Debug.Assert(authService != null);
            authService
                .InitializeAsync()
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        throw t.Exception;
                });
        }
        catch (TypeResolutionFailedException tx)
        {
            Console.WriteLine($"Failed to resolve type:\n{tx.GetBaseException()}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to initialize application:\n{e}");
            Environment.Exit(1);
        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        PlatformHelper.PlatformService?.OnFrameworkInitializationCompleted(ApplicationLifetime);
        
        base.OnFrameworkInitializationCompleted();
    }

    static void PerformRegistrations()
    {
        // Register CompositeFramework.Avalonia services.
        CompositeAvaloniaInitializer.RegisterDefaultServices();
        
        SplatRegistrations.SetupIOC();
        
        // Platform-specific registrations
        PlatformHelper.PlatformService?.RegisterPlatformTypes();

        // Register our services.
        SplatRegistrations.RegisterLazySingleton<IConfigurationService, ConfigurationService>();
        SplatRegistrations.RegisterLazySingleton<IAuthDataStore, SecureStorageAuthDataStore>();
        SplatRegistrations.RegisterLazySingleton<IAuthenticationService, AuthenticationService>();
        SplatRegistrations.RegisterLazySingleton<ServicesAuthHeaderHandler>();
        SplatRegistrations.RegisterLazySingleton<IFormFactorService, FormFactorService>();
        SplatRegistrations.RegisterLazySingleton<IMainNavigationService, MainNavigationService>();
        SplatRegistrations.RegisterLazySingleton<LoginProviderRegistry>();
        SplatRegistrations.RegisterLazySingleton<INotificationService, NotificationService>();
        
        ModularityInitializer.RegisterRequiredTypes();
        
        // Get configuration service to access maps API key
        var configService = Locator.Current.Resolve<IConfigurationService>();
        Debug.Assert(configService != null, "configService was null");

        //var mapsApiKey = configService.Maps.BingMapsApiKey;

        string servicesUrl;
        if (OperatingSystem.IsAndroid())
        {
            configService.Server.AndroidServicesUrl.ShouldNotBeNull();
            servicesUrl = configService.Server.AndroidServicesUrl;
        }
        else if (OperatingSystem.IsWindows())
        {
            configService.Server.WindowsServicesUrl.ShouldNotBeNull();
            servicesUrl = configService.Server.WindowsServicesUrl;
        }
        else
            throw new InvalidOperationException("Unsupported platform.");

        
        
        ServiceClientRegistration.AddMainApiClients(servicesUrl);

        // ShadUI services
        SplatRegistrations.RegisterLazySingleton<ToastManager>();

        // Views and ViewModels
        SplatRegistrations.RegisterLazySingleton<SettingsView>(); 
        SplatRegistrations.RegisterLazySingleton<SettingsViewModel>(); 
        SplatRegistrations.RegisterLazySingleton<SettingsSubView>();
        SplatRegistrations.RegisterLazySingleton<SettingsSubViewModel>();
        
        // Accessibility
        Locator.CurrentMutable.RegisterConstant(SemanticScreenReader.Default);

        // Dialogs, etc. 
    }
}