namespace CompositeFramework.Avalonia.Navigation;

/// <summary>
/// Used to hold an instance of the view for locations where
/// location.KeepViewAlive is true.
/// </summary>
internal record LocationWithViewInstance(
    ILocation Location,
    Type ViewType,
    object? ViewInstance)
{
    public ILocation Location { get; set; } = Location;
    public Type ViewType { get; init; } = ViewType;
}
