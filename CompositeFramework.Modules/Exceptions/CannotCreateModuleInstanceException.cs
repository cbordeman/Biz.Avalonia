namespace CompositeFramework.Modules.Exceptions;

public class CannotCreateModuleInstanceException : Exception
{
    public string Name { get; }
    public string AssemblyQualifiedName { get; }
    
    public CannotCreateModuleInstanceException(string name,
        string assemblyQualifiedName, Exception innerException)
        : base($"Cannot create module instance for " +
               $"{name} from {assemblyQualifiedName}.  " +
               $"The Func<IModule> moduleFactory provided " +
               $"to ModuleManager is used to create " +
               $"IModule instances.",
            innerException)
    {
        Name = name;
        AssemblyQualifiedName = assemblyQualifiedName;
    }
}