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
    string geometryStyleResourceName,
    string? moduleName,
    ICommand command,
    object? commandParameter)
{
    protected ICommand CommandField = command;
    
    public string ViewName { get; init; } = viewName;
    public string DisplayName { get; init; } = displayName;
    public string GeometryStyleResourceName { get; init; } = geometryStyleResourceName;
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
        string geometryStyleResourceName,
        string? moduleName) 
        : base(viewName, displayName, geometryStyleResourceName, moduleName, null!, null)
    {
        
        CommandField = NavigationCommand.Create(moduleName, viewName);
    }
}