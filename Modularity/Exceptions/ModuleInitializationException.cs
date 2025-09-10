namespace Modularity.Exceptions;

public class ModuleInitializationException : Exception
{
    public string Name { get; }
    
    public ModuleInitializationException(string name, 
        Exception innerException)
        : base($"Error initializaing module {name}.", innerException)
    {
        Name = name;
    }
}
