using Biz.Shell.Logging;
using Biz.Shell.Platform;
using Biz.Shell.Services.Authentication;
using Biz.Shell.Services.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Accessibility;
using Prism.Container.DryIoc;
using Prism.DryIoc;
using ServiceClients;
using Shouldly;
using IFormFactorService = Biz.Shell.Services.IFormFactorService;

namespace Biz.Shell;

public class App : PrismApplication
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        base.Initialize();
    }

    protected override AvaloniaObject CreateShell()
    {
        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime:
                // Avoid duplicate validations from both Avalonia and the
                // CommunityToolkit.  More info:
                // https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();
                return Container.Resolve<MainWindow>();

            case ISingleViewApplicationLifetime:
                // This includes browser / WASM, so we must use the form factor
                // in deciding UI elements.
                return Container.Resolve<MainSmallView>();

            default:
                throw new InvalidOperationException("Unsupported application lifetime.");
        }
    }

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        var pmcs = ContainerLocator.Current.Resolve<IPlatformModuleCatalogService>();
        pmcs.ConfigureModuleCatalog(moduleCatalog);
    }

    protected override IModuleCatalog CreateModuleCatalog() => new CompositeModuleCatalog();

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Platform-specific registrations
        PlatformHelper.PlatformService?.RegisterPlatformTypes(containerRegistry);

        // Register services.
        containerRegistry
            .RegisterSingleton<IConfigurationService, ConfigurationService>()
            .RegisterSingleton<IAuthenticationService, AuthenticationService>()
            .RegisterSingleton<IAuthDataStore, SecureStorageAuthDataStore>()
            .RegisterSingleton<ServicesAuthHeaderHandler>();

        // Get configuration service to access maps API key
        // TODO: Fix this to use the IConfigurationService via dependency
        // injection.
        var configService = new ConfigurationService();
        //var mapsApiKey = configService.Maps.BingMapsApiKey;

        // Logging
        containerRegistry.GetContainer().RegisterLoggerFactory(
            LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            }));

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
        
        containerRegistry.AddMainApiClients(servicesUrl);

        // Services
        containerRegistry.RegisterSingleton<INotificationService, NotificationService>();
        containerRegistry.RegisterSingleton<IFormFactorService, ViewControlService>();
        containerRegistry.RegisterSingleton<IMainRegionNavigationService, MainContentRegionNavigationService>();

        // Views - Region Navigation
        containerRegistry
            .RegisterForNavigation<SettingsView, SettingsViewModel>()
            .RegisterForNavigation<SettingsSubView, SettingsSubViewModel>();
        
        // Accessibility
        containerRegistry
            .RegisterInstance(SemanticScreenReader.Default);

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

    protected override void OnInitialized()
    {
        // Register Views to the Region it will appear in. Don't register them in the ViewModel.
        //var regionManager = Container.Resolve<IRegionManager>();

        // WARNING: Prism v11.0.0-prev4
        // - DataTemplates MUST define a DataType, or else an XAML error will be thrown
        // - Error: DataTemplate inside DataTemplates must have a DataType set
        ////regionManager.RegisterViewWithRegion(RegionNames.DynamicSettingsListRegion, typeof(Setting1View));
        ////regionManager.RegisterViewWithRegion(RegionNames.DynamicSettingsListRegion, typeof(Setting2View));

        PlatformHelper.PlatformService?.InitializePlatform(Container);
    }

    /// <summary>Custom region adapter mappings.</summary>
    /// <param name="regionAdapterMappings">Region Adapters.</param>
    protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
    {
        regionAdapterMappings.RegisterMapping<ContentControl, ContentControlRegionAdapter>();
        regionAdapterMappings.RegisterMapping<StackPanel, StackPanelRegionAdapter>();
        regionAdapterMappings.RegisterMapping<ItemsControl, ItemsControlRegionAdapter>();
        regionAdapterMappings.RegisterMapping<TransitioningContentControl, TransitioningContentControlRegionAdapter>();
    }
}