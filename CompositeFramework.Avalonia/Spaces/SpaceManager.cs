using Avalonia;
using Avalonia.Controls;
using CompositeFramework.Avalonia.Exceptions;

namespace CompositeFramework.Avalonia.Spaces;

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

    private static void OnSpaceNameChanged(Control element, AvaloniaPropertyChangedEventArgs e)
    {
        var oldSpaceName = e.OldValue as string;
        var newSpaceName = e.NewValue as string;
        
        if (oldSpaceName == newSpaceName)
            return;
        
        element.SetValue(SpaceNameProperty, newSpaceName);
        
        // Remove old space.
        if (oldSpaceName != null)
            SpaceNameRegistrations.Remove(oldSpaceName);

        if (newSpaceName == null)
            return;
        
        // Add new space name.
        if (!SpaceNameRegistrations.TryAdd(newSpaceName, element))
            throw new DuplicateSpaceNameException(element, newSpaceName);
    }
}