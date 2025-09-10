namespace Modularity;

public class MissingModuleAttributeException : Exception
{
    public Type ModuleType { get; }

    public MissingModuleAttributeException(Type moduleType)
        : base($"Module class {moduleType.FullName} " +
               $"must have the [Module] attribute.")
    {
        ModuleType = moduleType;
    }
}
