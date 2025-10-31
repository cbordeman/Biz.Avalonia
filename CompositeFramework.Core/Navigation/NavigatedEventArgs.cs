namespace CompositeFramework.Core.Navigation;

public record NavigatedEventArgs(
    NavigationResult Result,
    string LocationName,
    Exception? Exception, 
    NavigationContext Context);
