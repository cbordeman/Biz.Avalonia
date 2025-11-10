using Avalonia.Input;
using CompositeFramework.Avalonia.Controls;
using NavigationDirection = CompositeFramework.Core.Navigation.NavigationDirection;

namespace CompositeFramework.Avalonia.Navigation;

/// <summary>
/// Use in AXAML to assign the section name for a ContentControl.
/// </summary>
public class SectionManager : AvaloniaObject
{
    /// <summary>
    /// Provides the content control associated with each section name.
    /// </summary>
    public static IReadOnlyDictionary<string, ContentControl>
         SectionNameRegistrations => SectionNameRegistrationsInternal;
    
    static readonly Dictionary<string, ContentControl> 
        SectionNameRegistrationsInternal = new();
    
    // ReSharper disable once MemberCanBePrivate.Global
    public static readonly AttachedProperty<string?> SectionNameProperty =
        AvaloniaProperty.RegisterAttached<ViewModelLocator, ContentControl, string?>(
            "SectionName",
            defaultValue: null);

    static SectionManager()
    {
        SectionNameProperty.Changed.AddClassHandler<ContentControl>(OnSectionNameChanged);
    }

    public static void SetSectionName(ContentControl element, string? sectionName)
    {
        element.SetValue(SectionNameProperty, sectionName);
    }

    public static string? GetSectionName(ContentControl element)
    {
        return element.GetValue(SectionNameProperty);
    }

    private static void OnSectionNameChanged(ContentControl element, AvaloniaPropertyChangedEventArgs e)
    {
        var oldSectionName = e.OldValue as string;
        var newSectionName = e.NewValue as string;
        
        if (oldSectionName == newSectionName)
            return;
        
        element.SetValue(SectionNameProperty, newSectionName);
        
        // Remove old section.
        if (oldSectionName != null)
            SectionNameRegistrationsInternal.Remove(oldSectionName);

        if (newSectionName == null)
            return;
        
        // Add new section name.
        if (!SectionNameRegistrationsInternal.TryAdd(newSectionName, element))
            throw new DuplicateSectionNameException(element, newSectionName);
    }

    public static void ChangeSlideAnimation(string sectionName, 
        NavigationDirection direction)
    {
        if (!SectionNameRegistrationsInternal.TryGetValue(sectionName, out var element))
            throw new KeyNotFoundException($"Section {sectionName} not found.");
        else if (element is ReversibleTransitioningContentControl slidingContentControl)
            slidingContentControl.Direction = direction;
    }
}
