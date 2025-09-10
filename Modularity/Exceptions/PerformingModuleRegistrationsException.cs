namespace Modularity.Exceptions;

public class PerformingModuleRegistrationsException : Exception
{
    public string Name { get; }
    
    public PerformingModuleRegistrationsException(string name,
        Exception innerException)
        : base($"Error performing module registrations in module " +
               $"{name}.", innerException)
    {
        Name = name;
    }
}