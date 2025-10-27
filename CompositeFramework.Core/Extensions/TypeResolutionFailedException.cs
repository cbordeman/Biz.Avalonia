namespace CompositeFramework.Core.Extensions;

public class TypeResolutionFailedException : Exception
{
    public IReadonlyDependencyResolver Resolver { get; }
    public Type TypeLookedUp { get; }
    
    public TypeResolutionFailedException(
        IReadonlyDependencyResolver resolver, Type typeLookedUp,
        Exception? innerException)
        : base($"Resolution of type \"{typeLookedUp.Name}\" failed.", innerException)
    {
        Resolver = resolver;
        TypeLookedUp = typeLookedUp;
    }
}