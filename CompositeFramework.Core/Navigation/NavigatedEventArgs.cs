namespace CompositeFramework.Core.Navigation;

public record NavigatedEventArgs(
    NavigationResult Result, string Error, NavigationContext Context);
