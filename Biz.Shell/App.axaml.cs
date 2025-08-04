namespace Biz.Shell;

public partial class App : PrismApplication
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
            case IClassicDesktopStyleApplicationLifetime desktop:
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();
                return Container.Resolve<MainWindow>();

            case ISingleViewApplicationLifetime singleViewPlatform:
                // This includes browser / WASM, so we must use form factor
                // in deciding UI elements.
                return Container.Resolve<MainSmallView>();

            default:
                throw new InvalidOperationException("Unsupported application lifetime.");
        }
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // containerRegistry.Register<MainWindow>();
        // containerRegistry.Register<MainWindowViewModel>();
        // containerRegistry.Register<MainLargeView>();
        // containerRegistry.Register<MainLargeViewModel>();
        // containerRegistry.Register<MainSmallView>();
        // containerRegistry.Register<MainSmallViewModel>();

        Debug.WriteLine("RegisterTypes()");

        // Note:
        // SidebarView isn't listed, note we're using `AutoWireViewModel` in the View's AXAML.
        // See the line, `prism:ViewModelLocator.AutoWireViewModel="True"`

        // Services
        containerRegistry.RegisterSingleton<INotificationService, NotificationService>();
        containerRegistry.RegisterSingleton<IFormFactorService, FormFactorService>();

        // Views - Region Navigation
        // containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();
        // containerRegistry.RegisterForNavigation<MainLargeView, MainLargeViewModel>();
        // containerRegistry.RegisterForNavigation<MainSmallView, MainSmallViewModel>();
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
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    protected override void OnInitialized
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  ()
    {
        Debug.WriteLine("OnInitialized()");

        // Register Views to the Region it will appear in. Don't register them in the ViewModel.
        var regionManager = Container.Resolve<IRegionManager>();

        // WARNING: Prism v11.0.0-prev4
        // - DataTemplates MUST define a DataType or else an XAML error will be thrown
        // - Error: DataTemplate inside of DataTemplates must have a DataType set
        regionManager.RegisterViewWithRegion(RegionNames.SidebarRegion, typeof(SidebarView));

        ////regionManager.RegisterViewWithRegion(RegionNames.DynamicSettingsListRegion, typeof(Setting1View));
        ////regionManager.RegisterViewWithRegion(RegionNames.DynamicSettingsListRegion, typeof(Setting2View));
    }

    /// <summary>Custom region adapter mappings.</summary>
    /// <param name="regionAdapterMappings">Region Adapters.</param>
    protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
    {
        Debug.WriteLine("ConfigureRegionAdapterMappings()");
        regionAdapterMappings.RegisterMapping<ContentControl, ContentControlRegionAdapter>();
        regionAdapterMappings.RegisterMapping<StackPanel, StackPanelRegionAdapter>();
    }
    
    protected override IModuleCatalog CreateModuleCatalog()
    {
    protected override IModuleCatalog CreateModuleCatalog()
    {
        if (OperatingSystem.IsAndroid() || 
            OperatingSystem.IsIOS() ||
            OperatingSystem.IsBrowser())
        {
            var codeCatalog = new ModuleCatalog();
            codeCatalog.AddModule(typeof(ModuleA));
            return codeCatalog;
        }
        else
        {
            var compositeCatalog = new CompositeModuleCatalog();

            // Add modules defined in code
            var codeCatalog = new ModuleCatalog();
            // codeCatalog.AddModule(typeof(ModuleA));
            // codeCatalog.AddModule(typeof(ModuleB));
            compositeCatalog.AddCatalog(codeCatalog);

            // Add modules discovered from a directory
            var directoryCatalog = new DirectoryModuleCatalog() { ModulePath = @".\Modules" };
            compositeCatalog.AddCatalog(directoryCatalog);

            return compositeCatalog;
        }
    }    }
}