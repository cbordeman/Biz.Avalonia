using CompositeFramework.Avalonia.Extensions;
using CompositeFramework.Core.Extensions;
using Splat;

namespace CompositeFramework.Avalonia;

public class ViewModelLocator : AvaloniaObject
{
    static Func<Type, string> getViewModelTypeFromView =
        GetViewModelType;

    // This is case sensitive.
    static string[] viewSuffixes =
    [
        "UserControl",
        "View",
        "Page",
        "Window",
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
    public static void ConfigureViewModelNameResolution(
        Func<Type, string> getViewModelTypeFromViewType)
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
        string viewModelTypeAssemblyQualifiedName = 
            getViewModelTypeFromView(view.GetType());

        var viewModelType = Type.GetType(viewModelTypeAssemblyQualifiedName);
        if (viewModelType == null)
            throw new Exception("Resolver returned null.");

        try
        {
            // All VMs must be registered.
            object? viewModel = Locator.Current.Resolve(viewModelType);

            if (viewModel == null)
                throw new Exception("Service resolver returned null");
            view.DataContext = viewModel;
        }
        catch (Exception e)
        {
            throw new AutoWireViewModelTypeCreationException(
                view.GetType(),
                viewModelTypeAssemblyQualifiedName,
                e);
        }
    }

    static string GetViewModelType(Type viewType)
    {
        var viewFullName = viewType.FullName;
        if (viewFullName == null)
            throw new Exception($"View type {viewType.Name} does not have a FullName.");
        
        string viewModelName = viewFullName
            .Replace(".Views.", ".ViewModels.",
                StringComparison.OrdinalIgnoreCase);
        foreach (var suffix in viewSuffixes)
        {
            if (viewModelName.EndsWith(suffix))
            {
                viewModelName = 
                    $"{viewModelName.AsSpan(0, viewModelName.Length - suffix.Length)}ViewModel";
                break;
            }
        }
        var aqn = viewType.GetAssemblyQualifiedNameWithNewTypeName(
            viewModelName);
        return aqn;
    }
}