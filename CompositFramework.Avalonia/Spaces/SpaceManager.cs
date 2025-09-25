using Avalonia;
using Avalonia.Controls;
using CompositFramework.Avalonia.Exceptions;

namespace CompositFramework.Avalonia.Spaces;

public class SpaceManager : AvaloniaObject
{
    static readonly Dictionary<string, Control> 
        SpaceNameRegistrations = new();
    
    // ReSharper disable once MemberCanBePrivate.Global
    public static readonly AttachedProperty<string?> SpaceNameProperty =
        AvaloniaProperty.RegisterAttached<ViewModelLocator, Control, string?>(
            "SpaceName",
            defaultValue: null);

    static SpaceManager()
    {
        SpaceNameProperty.Changed.AddClassHandler<Control>(OnSpaceNameChanged);
    }

    public static void SetSpaceName(Control element, string? spaceName)
    {
        element.SetValue(SpaceNameProperty, spaceName);
    }

    public static string? GetSpaceName(Control element)
    {
        return element.GetValue(SpaceNameProperty);
    }

    private static void OnSpaceNameChanged(Control view, AvaloniaPropertyChangedEventArgs e)
    {
        RegisterSpaceName(view, e.NewValue as string);
    }

    private static void RegisterSpaceName(Control element, string? spaceName)
    {
        var oldSpaceName = element.GetValue(SpaceNameProperty);
        if (oldSpaceName == spaceName)
            return;
        
        element.SetValue(SpaceNameProperty, spaceName);
        
        // Remove old space.
        if (oldSpaceName != null)
            SpaceNameRegistrations.Remove(oldSpaceName);

        if (spaceName == null)
            return;
        
        // Add new space name.
        if (!SpaceNameRegistrations.TryAdd(spaceName, element))
            throw new DuplicateSpaceNameException(element, spaceName);
    }
}