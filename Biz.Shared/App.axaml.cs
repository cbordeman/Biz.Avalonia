using Biz.Shared.Logging;
using Biz.Shared.Platform;
using Biz.Shared.Services.Authentication;
using Biz.Shared.Services.Config;
using CompositeFramework.Avalonia.Navigation;
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
        SplatRegistrations.SetupIOC();
        
        // Platform-specific registrations
        PlatformHelper.PlatformService?.RegisterPlatformTypes();

        // Register services.
        SplatRegistrations.RegisterLazySingleton<IContextNavigationService, SectionNavigationService>();
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

        // Lets you use ILogger<T>
        LoggingSetup.RegisterMicrosoftLoggerFactoryWithSplat();

        var lf = Locator.Current.GetService<ILoggerFactory>();
        var logger = lf!.CreateLogger<App>();
        logger.LogInformation("App.Initialize: Starting up");
        
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