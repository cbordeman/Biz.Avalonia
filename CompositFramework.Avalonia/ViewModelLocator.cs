using Avalonia;
using Avalonia.Controls;
using CompositFramework.Avalonia.Exceptions;

namespace CompositFramework.Avalonia;

public class ViewModelLocator : AvaloniaObject
{
    static Func<Type, object?>? resolve;
    static Func<string, string>? getViewModelTypeFromView;

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
    /// <param name="getViewModelTypeFromViewType">If provided, used to get the
    /// ViewModel type from the View type.</param>
    /// <param name="recognizedViewSuffixes">Default is View, Page, or UserControl</param>
    public static void Configure(
        Func<Type, object?> viewModelResolver,
        Func<string, string>? getViewModelTypeFromViewType = null,
        string[]? recognizedViewSuffixes = null)
    {
        getViewModelTypeFromView = getViewModelTypeFromViewType;
        resolve = viewModelResolver;
        if (recognizedViewSuffixes != null)
            ViewModelLocator.viewSuffixes = recognizedViewSuffixes;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static readonly AttachedProperty<bool> AutoWireViewModelProperty =
        AvaloniaProperty.RegisterAttached<ViewModelLocator, Control, bool>(
            "AutoWireViewModel",
            defaultValue: false);

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
        if (e.NewValue is bool and true && view.DataContext == null)
            WireViewModel(view);
    }

    private static void WireViewModel(Control view)
    {
        string? viewModelTypeAssemblyQualifiedName;
        var viewType = view.GetType();
        var viewAssemblyQualifiedName = viewType.AssemblyQualifiedName;
        if (viewAssemblyQualifiedName == null)
            throw new Exception("View type does not have an AssemblyQualifiedName.");
        
        if (getViewModelTypeFromView == null)
            // Uses a convention based approach to VM location.
            viewModelTypeAssemblyQualifiedName = 
                GetViewModelType(viewAssemblyQualifiedName);
        else
            // User user provided menas to get vm type from view.
            viewModelTypeAssemblyQualifiedName = 
                getViewModelTypeFromView(viewAssemblyQualifiedName);

        var viewModelType = Type.GetType(viewModelTypeAssemblyQualifiedName);
        if (viewModelType == null)
            throw new Exception("Resolver returned null.");

        try
        {
            object? viewModel = resolve?.Invoke(viewModelType);

            if (viewModel == null)
                throw new Exception("Service resolver returned null");
            view.DataContext = viewModel;
        }
        catch (Exception e)
        {
            throw new AutoWireViewModelTypeCreationException(
                viewAssemblyQualifiedName, 
                viewModelTypeAssemblyQualifiedName, e);
        }

    }

    static string GetViewModelType(string viewName)
    {
        var viewModelName = viewName
            .Replace(".Views.", ".ViewModels.", StringComparison.OrdinalIgnoreCase);
        foreach (var suffix in viewSuffixes!)
            viewModelName = ReplaceEnd(viewModelName, suffix, "ViewModel");
        if (viewModelName == viewName)
            throw new AutoWireViewModelTypeDoesNotFollowConventionException(
                viewName);
        return viewModelName!;
    }

    static string? ReplaceEnd(string? str, string oldStr, string newStr)
    {
        if (str != null && str.EndsWith(oldStr))
            return str.Substring(0, str.Length - oldStr.Length) + newStr;
        return str;
    }
}
