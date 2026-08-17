namespace Biz.Shared.ViewModels;

/// <summary>
/// This vm has an associated DataTemplate.
/// </summary>
public class MenuHeaderViewModel
{
    public string? Header { get; init; }
    public ObservableCollection<MenuItemViewModel>? MenuItems { get; set; }
}

public class BaseMenuItemViewModel;

public class MenuItemViewModel(
    string viewName,
    object header,
    string? iconText,
    string? moduleName,
    ICommand command,
    object? commandParameter) : BaseMenuItemViewModel
{
    protected ICommand CommandField = command;

    public string ViewName { get; init; } = viewName;
    public object Header { get; init; } = header;
    public string? IconText { get; init; } = iconText;
    public string? ModuleName { get; } = moduleName;
    public object? CommandParameter { get; set; } = commandParameter;

    public virtual ICommand Command
    {
        get => CommandField;
        protected set => CommandField = value;
    }
}

public class ProfileMenuItemViewModel : BaseMenuItemViewModel
{
    public string Name { get; set; }
    // Example: avares://Biz.Shared/Assets/user.png
    public string? SourceAvaRes { get; set; }
    public string Email { get; set; }
    public ICommand Command { get; }

    public ProfileMenuItemViewModel(string name, string sourceAvaRes, string email,
        ICommand command)
    {
        Name = name;
        SourceAvaRes = sourceAvaRes;
        Email = email;
        Command = command;
    }
}

public class SeparatorMenuItemViewModel : BaseMenuItemViewModel { }

public class NavigationMenuItemViewModel : MenuItemViewModel
{
    public NavigationMenuItemViewModel(
        string viewName, string header,
        string geometryStyleResourceName, string moduleName)
        : base(viewName, header, geometryStyleResourceName,
            moduleName, null!, viewName)
    {
        CommandField = new NavigationCommand(moduleName, viewName);
    }
}
