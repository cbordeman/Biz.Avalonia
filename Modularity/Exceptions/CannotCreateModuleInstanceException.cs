namespace Modularity.Exceptions;

public class CannotCreateModuleInstanceException : Exception
{
    public string Name { get; }
    public string AssemblyQualifiedName { get; }
    
    public CannotCreateModuleInstanceException(string name,
        string assemblyQualifiedName, Exception innerException)
        : base($"Cannot create module instance for " +
               $"{name} from {assemblyQualifiedName}.  Ensure " +
               $"the type has a parameterless constructor.",
            innerException)
    {
        Name = name;
        AssemblyQualifiedName = assemblyQualifiedName;
    }
}