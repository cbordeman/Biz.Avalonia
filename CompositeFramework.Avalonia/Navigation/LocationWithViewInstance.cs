namespace CompositeFramework.Avalonia.Navigation;

/// <summary>
/// Used to hold an instance of the view for locations where
/// location.KeepViewAlive is true.
/// </summary>
/// <param name="Location"></param>
/// <param name="ViewInstance"></param>
internal record LocationWithViewInstance(ILocation Location,
    // ReSharper disable once NotAccessedPositionalProperty.Global
    object? ViewInstance);
