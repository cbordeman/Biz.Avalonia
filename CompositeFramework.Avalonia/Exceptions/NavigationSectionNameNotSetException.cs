using System.Runtime.CompilerServices;

namespace CompositeFramework.Avalonia.Exceptions;

public class NavigationSectionNameNotSetException : Exception
{
    public ContentControl? ContentControl { get; }
    public string? SectionName { get; }

    public NavigationSectionNameNotSetException(
        ContentControl? contentControl = null, 
        string? sectionName = null,
        [CallerMemberName] string? memberName = null)
        : base($"Navigation service's SectionName not set " +
               $"on {nameof(ISectionNavigationService)} " +
               $"in call to {memberName}().")
    {
        ContentControl = contentControl;
        SectionName = sectionName;
    }
}
