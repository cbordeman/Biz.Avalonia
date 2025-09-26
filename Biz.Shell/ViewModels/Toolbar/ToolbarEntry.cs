using Avalonia.Media;
using CompositeFramework.Core.ViewModels;

namespace Biz.Shell.ViewModels.Toolbar;

public class ToolbarEntry : BindingValidatingBase, IToolbarEntry
{
    readonly Func<object?, Task>? execute;
    readonly Func<object?, bool>? canExecute;

    public Geometry? Geometry
    {
        get; set => SetProperty(ref field, value);
    }
    
    public string? Text
    {
        get; init => SetProperty(ref field, value);
    }
    
    #region Command
    public AsyncRelayCommand<object?>? Command => field ??= 
        new AsyncRelayCommand<object?>(ExecuteCommand, CanExecute);
    bool CanExecute(object? p) => canExecute?.Invoke(p) ?? true;
    Task ExecuteCommand(object? p)
    {
        return execute?.Invoke(p)!;
    }
    #endregion Command
    
    public bool IsVisible
    {
        get; set => SetProperty(ref field, value);
    } = true;
    
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