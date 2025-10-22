using Biz.Shell.Logging;
using Biz.Shell.Services.Authentication;
using Biz.Shell.Services.Config;
using CompositeFramework.Avalonia;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Accessibility;
using ServiceClients;
using IFormFactorService = Biz.Shell.Services.IFormFactorService;

namespace Biz.Shell;

public partial class App : Application
{
    public override async void Initialize()
    {
        try
        {
            AvaloniaXamlLoader.Load(this);

            PerformRegistrations();
            PlatformHelper.PlatformService?.InitializePlatform();

            var authService = Locator.Current.GetService<IAuthenticationService>();
            Debug.Assert(authService != null);
            await authService.InitializeAsync();
        }
        catch (Exception e)
        {
            //No precompiled XAML found for Biz.Shell.App,
            //make sure to specify x:Class and include your
            //XAML file as AvaloniaResource

            Console.WriteLine("Failed to initialize application:\n" +
                              e.ToString());
            Environment.Exit(1);
        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is
            IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the
            // CommunityToolkit.  More info:
            // https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

            desktop.MainWindow = Locator.Current.GetService<MainWindow>();
            ViewModelLocator.SetAutoWireViewModel(desktop.MainWindow!, true);
        }
        else if (ApplicationLifetime is
                 ISingleViewApplicationLifetime singleView)
        {
            singleView.MainView = Locator.Current.GetService<MainSmallView>();
            ViewModelLocator.SetAutoWireViewModel(singleView.MainView!, true);
        }

        base.OnFrameworkInitializationCompleted();
    }

    static void PerformRegistrations()
    {
        // Views and ViewModels

        // Platform-specific registrations
        PlatformHelper.PlatformService?.RegisterPlatformTypes();

        // Register services.
        // SplatRegistrations.RegisterLazySingleton<ConfigurationService>();
        // SplatRegistrations.RegisterLazySingleton<IAuthDataStore, SecureStorageAuthDataStore>();
        // SplatRegistrations.RegisterLazySingleton<IAuthenticationService, AuthenticationService>();
        // SplatRegistrations.RegisterLazySingleton<ServicesAuthHeaderHandler>();

        // Get configuration service to access maps API key
        // TODO: Fix this to use the IConfigurationService via dependency
        // injection.
        ConfigurationService? configService = Locator.Current.GetService<ConfigurationService>();
        Debug.Assert(configService != null, "configService was null");

        //var mapsApiKey = configService.Maps.BingMapsApiKey;

        // Lets you use ILogger<T>
        LoggingSetup.RegisterMicrosoftLoggerFactoryWithSplat();

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

        // Services
        // SplatRegistrations.RegisterLazySingleton<INotificationService, NotificationService>();
        // SplatRegistrations.RegisterLazySingleton<IFormFactorService, ViewControlService>();
        // SplatRegistrations.RegisterLazySingleton<IMainNavigationService, MainNavigationService>();
        // SplatRegistrations.RegisterLazySingleton<LoginProviderRegistry>();

        // Views - Region Navigation
        // SplatRegistrations.Register<SettingsView>(); 
        // SplatRegistrations.Register<SettingsViewModel>(); 
        // SplatRegistrations.Register<SettingsSubView>();
        // SplatRegistrations.Register<SettingsSubViewModel>();

        // Accessibility
        //Locator.CurrentMutable.RegisterConstant(SemanticScreenReader.Default);

        // Dialogs, etc. 
    }

    void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
            BindingPlugins.DataValidators.Remove(plugin);
    }

}
