using Microsoft.Extensions.Logging;
using Biz.Core.Logging;
using Prism.Container.DryIoc;


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
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
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
        PlatformHelper.RegistrationService?.RegisterPlatformTypes(containerRegistry);
        
        // Note:
        // SidebarView isn't listed, note we're using `AutoWireViewModel` in the View's AXAML.
        // See the line, `prism:ViewModelLocator.AutoWireViewModel="True"`

        // Logging
        containerRegistry.GetContainer().RegisterLoggerFactory(
            LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            }));
        
        // Services
        containerRegistry.RegisterSingleton<INotificationService, NotificationService>();
        containerRegistry.RegisterSingleton<IFormFactorService, FormFactorService>();
        containerRegistry.RegisterSingleton<IMainRegionNavigationService, MainContentRegionNavigationService>();

        // Views - Region Navigation
        containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();
        containerRegistry.RegisterForNavigation<SettingsSubView, SettingsSubViewModel>();

        // Dialogs, etc. 
    }

    private void DisableAvaloniaDataAnnotationValidation()
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
        var regionManager = Container.Resolve<IRegionManager>();

        // WARNING: Prism v11.0.0-prev4
        // - DataTemplates MUST define a DataType, or else an XAML error will be thrown
        // - Error: DataTemplate inside DataTemplates must have a DataType set
        regionManager.RegisterViewWithRegion(RegionNames.SidebarRegion, typeof(SidebarView));
        ////regionManager.RegisterViewWithRegion(RegionNames.DynamicSettingsListRegion, typeof(Setting1View));
        ////regionManager.RegisterViewWithRegion(RegionNames.DynamicSettingsListRegion, typeof(Setting2View));
        
        PlatformHelper.RegistrationService?.InitializePlatform(Container);
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