namespace Biz.Shell.ViewModels;

/// <summary>
/// Loads a module and navigates within the MainContentRegion.
/// </summary>
public class NavigationCommand : DelegatingAsyncRelayCommand
{
    readonly string moduleName;
    readonly string viewName;

    NavigationCommand(
        string moduleName, string viewName,
        Func<Task> execute)
        : base(execute)
    {
        this.moduleName = moduleName;
        this.viewName = viewName;
    }
    
    /// <summary>
    /// Factory method because C# is stupid about the use of
    /// non-static contexts in the constructor.
    /// </summary>
    public static NavigationCommand Create(
        string moduleName, string viewName)
    {
        var instance = new NavigationCommand(moduleName, viewName, null!);
        
        return new NavigationCommand(
            moduleName, viewName, 
            instance.LoadModuleAndNavigate);
    }

    Task LoadModuleAndNavigate()
    {
        // Will be null if it's a primary module that
        // doesn't need to be loaded.
        var moduleManager = Locator.Current.Resolve<IModuleManager>();
        moduleManager.LoadModule(moduleName);
        
        var nav = Locator.Current.Resolve<IMainRegionNavigationService>();
        await nav.NavigateAsync(RegionNames.MainContentRegion, viewName);
        
         return Task.CompletedTask;
    }
}