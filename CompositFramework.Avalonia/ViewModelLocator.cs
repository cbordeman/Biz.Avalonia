using Avalonia;
using Avalonia.Controls;
using CompositeFramework.Core.Extensions;
using CompositFramework.Avalonia.Exceptions;
using Splat;

namespace CompositFramework.Avalonia;

public class ViewModelLocator : AvaloniaObject
{
    static Func<string, string>? getViewModelTypeFromView;

    static string[] viewSuffixes =
    [
        "View",
        "Page",
        "UserControl",
        "Dialog"
    ];

    /// <summary>
    /// Given the viewmodel type, should return an instance.
    /// </summary>
    /// <param name="recognizedViewSuffixes">Default is
    /// View, Page, Dialog, or UserControl.</param>
    public static void ConfigureViewSuffixes(
        params string[] recognizedViewSuffixes)
    {
        if (recognizedViewSuffixes.Length == 0)
            throw new Exception("View suffixes must not be empty.");
        viewSuffixes = recognizedViewSuffixes;
    }

    /// <summary>
    /// Change how the viewmodel name is derived from the view name.
    /// </summary>
    /// <param name="getViewModelTypeFromViewType"></param>
    public static void ConfigureViewModelNameResolution(
        Func<string, string>? getViewModelTypeFromViewType = null)
    {
        getViewModelTypeFromView = getViewModelTypeFromViewType;
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
            object? viewModel = Locator.Current.Resolve(viewModelType);

            if (viewModel == null)
                throw new Exception("Service resolver returned null");
            view.DataContext = viewModel;
        }
        catch (Exception e)
        {
            throw new AutoWireViewModelTypeCreationException(
                viewAssemblyQualifiedName,
                viewModelTypeAssemblyQualifiedName,
                e);
        }

    }

    static string GetViewModelType(string viewName)
    {
        var viewModelName = viewName
            .Replace(".Views.", ".ViewModels.", StringComparison.OrdinalIgnoreCase);
        foreach (var suffix in viewSuffixes)
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
