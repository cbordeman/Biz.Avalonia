namespace Modularity.Exceptions;

public class CannotFindModuleTypeException : Exception
{
    public string Name { get; }
    public string ModuleTypeAssemblyQualifiedName { get; }
    public CannotFindModuleTypeException(
        string name,
        string moduleTypeAssemblyQualifiedName,
        Exception innerException) 
        : base($"Cannot find module type {moduleTypeAssemblyQualifiedName} " +
               $"for module {name}.", innerException)
    {
        Name = name;
        ModuleTypeAssemblyQualifiedName = moduleTypeAssemblyQualifiedName;
    }
}
