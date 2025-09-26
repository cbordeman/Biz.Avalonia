using CompositeFramework.Avalonia.Commands;

namespace Biz.Shell.ViewModels;

/// <summary>
/// Loads a module and navigates within the MainContentRegion.
/// </summary>
public class NavigationCommand : AsyncCommand
{
    readonly string moduleName;
    readonly string viewName;

    public NavigationCommand(string moduleName, string viewName)
    {
        this.moduleName = moduleName;
        this.viewName = viewName;
        this.ExecuteHandler = LoadModuleAndNavigate;
    }
    
    async Task LoadModuleAndNavigate()
    {
        var nav = Locator.Current.Resolve<IMainNavigationService>();
        await nav.NavigateWithModuleAsync(moduleName, viewName);
    }
}