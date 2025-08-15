namespace Biz.Shell.ViewModels;

/// <summary>
/// This vm has an associated DataTemplate.
/// </summary>
public class SidebarHeaderViewModel
{
    public string? Header { get; init; }
    public ObservableCollection<SidebarItemViewModel>? Children { get; set; }
}

public class SidebarItemViewModel(
    string viewName,
    string displayName,
    string icon,
    string? moduleName,
    ICommand command,
    object? commandParameter)
{
    protected ICommand CommandField = command;
    
    public string ViewName { get; init; } = viewName;
    public string DisplayName { get; init; } = displayName;
    public string Icon { get; init; } = icon;
    public string? ModuleName { get; } = moduleName;
    public object? CommandParameter { get; set; } = commandParameter;

    public virtual ICommand Command
    {
        get => CommandField;
        protected set => CommandField = value;
    }
}

public class SideBarNavigationItemViewModel : SidebarItemViewModel
{
    public SideBarNavigationItemViewModel(string viewName,
        string displayName,
        string icon,
        string? moduleName) 
        : base(viewName, displayName, icon, moduleName, null!, null)
    {
        ViewName = viewName;
        DisplayName = displayName;
        Icon = icon;
        CommandField = NavigationCommand.Create(moduleName, viewName);
    }
}