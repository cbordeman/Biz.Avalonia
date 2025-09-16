namespace CompositeFramework.Core.Navigation;

public delegate Task NavigationEventArgs(
    NavigationResult result,
    string error,
    NavigationContext context);
