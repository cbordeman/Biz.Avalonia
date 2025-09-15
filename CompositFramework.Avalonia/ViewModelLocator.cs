using Avalonia;
using Avalonia.Controls;
using CompositFramework.Avalonia.Exceptions;

namespace CompositFramework.Avalonia;

public class ViewModelLocator : AvaloniaObject
{
    static Func<Type, object?>? resolve;
    static string[]? viewSuffixes =
    [
        "View", 
        "Page", 
        "UserControl",
        "Dialog"
    ];

    /// <summary>
    /// Given the viewmodel type, should return an instance.
    /// </summary>
    /// <param name="viewModelResolver"></param>
    /// <param name="recognizedViewSuffixes">Default is View, Page, or UserControl</param>
    public static void Configure(Func<Type, object?> viewModelResolver,
        params string[] recognizedViewSuffixes)
    {
        ViewModelLocator.resolve = viewModelResolver;
        ViewModelLocator.viewSuffixes = recognizedViewSuffixes;
    }
    
    // ReSharper disable once MemberCanBePrivate.Global
    public static readonly AttachedProperty<bool> AutoWireViewModelProperty =
        AvaloniaProperty.RegisterAttached<ViewModelLocator, Control, bool>(
            "AutoWireViewModel", defaultValue: false);

    static ViewModelLocator()
    {
        AutoWireViewModelProperty.Changed.AddClassHandler<Control>(OnAutoWireViewModelChanged);
    }

    public static void SetAutoWireViewModel(Control element, bool value)
    {
        element.SetValue(AutoWireViewModelProperty, value);
    }

    public static void SetAutoWireViewModel(Window window, bool value)
    {
        window.SetValue(AutoWireViewModelProperty, value);
    }
    
    public static bool GetAutoWireViewModel(Control element)
    {
        return element.GetValue(AutoWireViewModelProperty);
    }

    public static bool GetAutoWireViewModel(Window window)
    {
        return window.GetValue(AutoWireViewModelProperty);
    }

    private static void OnAutoWireViewModelChanged(Control view, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is bool and true)
            WireViewModel(view);
    }

    // public static bool TypeIsView(Type t)
    // {
    //     foreach (var suffix in viewSuffixes!)
    //         if (t.Name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
    //             return true;
    //     return false;
    // }
    
    private static void WireViewModel(Control view)
    {
        // Uses a convention based approach to VM location.
        
        if (view.DataContext != null)
            return;
        
        var viewModel = await GetViewModel(view);
        
        view.DataContext = viewModel;
    }
    static async Task<object?> GetViewModel(Control view)
    {
        var viewType = view.GetType();
        var viewName = viewType.FullName;
        if (string.IsNullOrEmpty(viewName))
            return;

        var viewModelName = viewName
            .Replace(".Views.", ".ViewModels.", StringComparison.OrdinalIgnoreCase);
        foreach (var suffix in viewSuffixes!)
            viewModelName = ReplaceEnd(viewModelName, suffix, "ViewModel");
        if (viewModelName == viewName)
            throw new AutoWireViewModelTypeDoesNotFollowConventionException(
                viewName);
        var viewModelType = Type.GetType(viewModelName!);
        if (viewModelType == null)
            return null;

        object? viewModel;
        try
        {
            viewModel = resolve?.Invoke(viewModelType);
            if (viewModel == null)
                throw new Exception("Service provider returned null");
            return viewModel;
        }
        catch (Exception e)
        {
            throw new AutoWireViewModelTypeCreationException(
                viewName, viewModelName!, e);
        }
    }

    static string? ReplaceEnd(string? str, string oldStr, string newStr)
    {
        if (str != null && str.EndsWith(oldStr))
            return str.Substring(0, str.Length - oldStr.Length) + newStr;
        return str;
    }
}