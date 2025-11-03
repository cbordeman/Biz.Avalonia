using System.Runtime.CompilerServices;

namespace CompositeFramework.Avalonia.Exceptions;

public class NavigationContextNotSetException : Exception
{
    public NavigationContextNotSetException(
        [CallerMemberName] string? memberName = null)
        : base($"Navigation context not set on {nameof(ISectionNavigationService)} " +
               $"in call to {memberName}().")
    { }
}
