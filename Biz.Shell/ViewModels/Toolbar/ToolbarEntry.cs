using System.Diagnostics.CodeAnalysis;
using Avalonia.Media;

namespace Biz.Shell.ViewModels.Toolbar;

public class ToolbarEntry : BindableBase, IToolbarEntry
{
    readonly Func<object?, Task>? execute;
    readonly Func<object?, bool>? canExecute;

    #region Geometry
    public Geometry? Geometry
    {
        get;
        set => SetProperty(ref field, value);
    }
    #endregion Geometry
    
    #region Text
    public string? Text
    {
        get;
        init => SetProperty(ref field, value);
    }
    #endregion Text

    #region Command
    [field: AllowNull, MaybeNull]
    public AsyncDelegateCommandWithParam<object?> Command => field ??= 
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
        get;
        set => SetProperty(ref field, value);
    } = true;
    #endregion IsVisible
    
    public ToolbarEntry(string? text = null,
        string? geometryStyleResourceName = null,
        Func<object?, Task>? execute = null,
        Func<object?, bool>? canExecute = null)
    {
        if (geometryStyleResourceName is not null)
            this.Geometry = AppHelpers.GetAppStyleResource<Geometry>(geometryStyleResourceName);
        this.execute = execute;
        this.canExecute = canExecute;
        this.Text = text;
    }
}