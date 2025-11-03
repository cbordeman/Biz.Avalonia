using System.Runtime.CompilerServices;

namespace CompositeFramework.Avalonia.Exceptions;

public class NavigationSectionNameNotSetException : Exception
{
    public NavigationSectionNameNotSetException(
        [CallerMemberName] string? memberName = null)
        : base($"Navigation service's SectionName not set " +
               $"on {nameof(ISectionNavigationService)} " +
               $"in call to {memberName}().")
    { }
}
