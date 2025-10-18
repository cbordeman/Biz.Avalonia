namespace CompositeFramework.Core.Navigation;

public class NavigationContext
{
    /// <summary>
    /// True when navigation is backward.
    /// </summary>
    public bool IsBack { get; init; }
    
    /// <summary>
    /// The location.  Initially null.
    /// </summary>
    public ILocation? Location { get; init; }

    /// <summary>
    /// Used to pass values during navigation.
    /// </summary>
    public Dictionary<string, object> Parameters { get; init; } = null!;

    // /// <summary>
    // /// The actual view instance.
    // /// </summary>
    // public object View { get; init; }
    
    /// <summary>
    /// The view name the location is registered as. 
    /// </summary>
    public string ViewName { get; init; } = null!;
}