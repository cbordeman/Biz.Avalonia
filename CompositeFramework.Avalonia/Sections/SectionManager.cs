using Avalonia;
using Avalonia.Controls;
using CompositeFramework.Avalonia.Exceptions;

namespace CompositeFramework.Avalonia.Sections;

public class SectionManager : AvaloniaObject
{
    static readonly Dictionary<string, Control> 
        SectionNameRegistrations = new();
    
    // ReSharper disable once MemberCanBePrivate.Global
    public static readonly AttachedProperty<string?> SectionNameProperty =
        AvaloniaProperty.RegisterAttached<ViewModelLocator, Control, string?>(
            "SectionName",
            defaultValue: null);

    static SectionManager()
    {
        SectionNameProperty.Changed.AddClassHandler<Control>(OnSectionNameChanged);
    }

    public static void SetSectionName(Control element, string? sectionName)
    {
        element.SetValue(SectionNameProperty, sectionName);
    }

    public static string? GetSectionName(Control element)
    {
        return element.GetValue(SectionNameProperty);
    }

    private static void OnSectionNameChanged(Control element, AvaloniaPropertyChangedEventArgs e)
    {
        var oldSectionName = e.OldValue as string;
        var newSectionName = e.NewValue as string;
        
        if (oldSectionName == newSectionName)
            return;
        
        element.SetValue(SectionNameProperty, newSectionName);
        
        // Remove old section.
        if (oldSectionName != null)
            SectionNameRegistrations.Remove(oldSectionName);

        if (newSectionName == null)
            return;
        
        // Add new section name.
        if (!SectionNameRegistrations.TryAdd(newSectionName, element))
            throw new DuplicateSectionNameException(element, newSectionName);
    }
}