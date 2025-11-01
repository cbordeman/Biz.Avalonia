namespace CompositeFramework.Core.Extensions;

public class TypeConstraintNotMetException : Exception
{
    public IReadonlyDependencyResolver Resolver { get; }
    public Type Type { get; }
    public string ConstraintMessage { get; }
    
    public TypeConstraintNotMetException(
        IReadonlyDependencyResolver resolver, Type type,
        string constraintMessage,
        Exception? innerException)
        : base($"Type \"{type.Name}\" did not meet constrant: " +
               $"{constraintMessage}.", innerException)
    {
        Resolver = resolver;
        Type = type;
        ConstraintMessage = constraintMessage;
    }
}