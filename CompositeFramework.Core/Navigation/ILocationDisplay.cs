namespace CompositeFramework.Core.Navigation;

/// <summary>
/// Used to give location a DisplayName and Description.
/// </summary>
public interface ILocationDisplay
{
    /// <summary>
    /// Defaults to GetType().Name.  Can be used in breadcrumbs,
    /// and when navigating back to target a certain location.
    /// Is not unique. 
    /// </summary>
    string DisplayName => GetType().Name;

    /// <summary>
    /// A longer description of the location.
    /// </summary>
    string Description => string.Empty;
}
