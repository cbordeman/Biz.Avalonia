namespace CompositeFramework.Avalonia.Navigation;

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
}