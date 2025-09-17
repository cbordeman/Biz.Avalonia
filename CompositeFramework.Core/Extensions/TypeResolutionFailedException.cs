namespace CompositeFramework.Core.Extensions;

public class TypeResolutionFailedException : Exception
{
    public Type TypeLookedUp { get; }
    
    public TypeResolutionFailedException(Type typeLookedUp,
        Exception? innerException)
        : base($"Resolution of type \"{typeLookedUp.Name}\" failed.", innerException)
    {
        TypeLookedUp = typeLookedUp;
    }
}