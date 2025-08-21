using Avalonia.Media;
using Prism.Mvvm;

namespace Biz.Shell.ViewModels.Toolbar;

public class ToolbarEntry : BindableBase, IToolbarEntry
{
    readonly Func<object?, Task>? execute;
    readonly Func<object?, bool>? canExecute;

    #region Geometry
    public Geometry? Geometry
    {
        get => geometry;
        set => SetProperty(ref geometry, value);
    }
    Geometry? geometry;        
    #endregion Geometry
    
    #region Text
    public string? Text
    {
        get => text;
        init => SetProperty(ref text, value);
    }
    string? text;        
    #endregion Text

    #region Command
    AsyncDelegateCommandWithParam<object?>? command;
    public AsyncDelegateCommandWithParam<object?> Command => command ??= 
        new AsyncDelegateCommandWithParam<object?>(ExecuteCommand, CanExecute);
    bool CanExecute(object? p) => canExecute?.Invoke(p) ?? true;
    Task ExecuteCommand(object? p)
    {
        return execute?.Invoke(p)!;
    }
    #endregion Command
    
    #region IsVisible
    public bool IsVisible
    {
        get => isVisible;
        set => SetProperty(ref isVisible, value);
    }
    bool isVisible = true;
    #endregion IsVisible
    
    public ToolbarEntry(string? text = null,
        string? geometryStyleResourceName = null,
        Func<object?, Task>? execute = null,
        Func<object?, bool>? canExecute = null)
    {
        if (geometryStyleResourceName is not null)
            this.geometry = AppHelpers.GetAppStyleResource<Geometry>(geometryStyleResourceName);
        this.execute = execute;
        this.canExecute = canExecute;
        this.text = text;
    }
}