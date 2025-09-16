namespace CompositeFramework.Core.Navigation;

public class NavigationContext
{
    /// <summary>
    /// True when navigation is backward.
    /// </summary>
    public bool IsBack { get; init; }
    
    /// <summary>
    /// The location.
    /// </summary>
    public INavigationAware? Location { get; init; }
    
    /// <summary>
    /// Used to pass values during navigation.
    /// </summary>
    public Dictionary<string, object>? Parameters { get; init; }
}
