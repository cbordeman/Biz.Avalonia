using System.Windows.Input;
using Avalonia.Media;
using Prism.Mvvm;

namespace Biz.Core.ViewModels.Toolbar;

public abstract class ToolbarEntry : BindableBase
{
    #region GeometryStyleResourceName
    public Geometry? GeometryStyleResourceName
    {
        get => geometryStyleResourceName;
        set => SetProperty(ref geometryStyleResourceName, value);
    }
    Geometry? geometryStyleResourceName;        
    #endregion GeometryStyleResourceName

    #region Text
    public string? Text
    {
        get => text;
        set => SetProperty(ref text, value);
    }
    string? text;        
    #endregion Text
    
    #region Command
    public ICommand? Command
    {
        get => command;
        set => SetProperty(ref command, value);
    }
    ICommand? command;        
    #endregion Command

    #region CommandParameter
    public object? CommandParameter
    {
        get => commandParameter;
        set => SetProperty(ref commandParameter, value);
    }
    object? commandParameter;        
    #endregion CommandParameter
    
    #region IsVisible
    public bool IsVisible
    {
        get => isVisible;
        set => SetProperty(ref isVisible, value);
    }
    bool isVisible;        
    #endregion IsVisible
}