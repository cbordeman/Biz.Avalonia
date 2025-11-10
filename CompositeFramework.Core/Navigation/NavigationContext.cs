namespace CompositeFramework.Core.Navigation;

public class NavigationContext
{
    /// <summary>
    /// True when navigation is backward.
    /// </summary>
    public NavigationDirection Direction { get; init; }

    /// <summary>
    /// The location.  May be null if couldn't create.
    /// </summary>
    public ILocation? Location { get; set; }
}