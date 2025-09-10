using System.ComponentModel.Design;
using System.Net.Mime;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace Modularity.Avalonia;

public abstract class PrismApplicationBase : Application
{
    IModuleDirectory? moduleDirectory;

    public AvaloniaObject MainWindow { get; protected set; }

    public IServiceProvider Container { get; protected set; }
    
    public override void Initialize()
    {
        base.Initialize();

        moduleDirectory = CreateModuleCatalog();
        RegisterRequiredTypes();
        RegisterTypes(_containerExtension);

        ConfigureModuleCatalog(moduleDirectory);

        var regionAdapterMappings = _containerExtension.Resolve<RegionAdapterMappings>();
        ConfigureRegionAdapterMappings(regionAdapterMappings);

        var defaultRegionBehaviors = _containerExtension.Resolve<IRegionBehaviorFactory>();
        ConfigureDefaultRegionBehaviors(defaultRegionBehaviors);

        RegisterFrameworkExceptionTypes();

        var shell = CreateShell();
        if (shell != null)
        {
            MvvmHelpers.AutowireViewModel(shell);
            InitializeShell(shell);
        }

    }

    /// <summary>Framework initialization has completed.</summary>
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            desktopLifetime.MainWindow = MainWindow as Window;
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewLifetime)
            singleViewLifetime.MainView = MainWindow as Control;

        base.OnFrameworkInitializationCompleted();
    }

    protected virtual void OnInitialized()
    {
        if (MainWindow is Window w)
            w.Show();
    }

    protected virtual void ConfigureModuleDirectory(
        IModuleDirectory moduleDirectory)
    {
    }
}
