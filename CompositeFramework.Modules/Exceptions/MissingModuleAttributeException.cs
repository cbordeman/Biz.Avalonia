namespace CompositeFramework.Modules.Exceptions;

public class MissingModuleAttributeException : Exception
{
    public string ModuleType { get; }

    public MissingModuleAttributeException(string moduleType)
        : base($"Module class {moduleType} " +
               $"must have the [Module] attribute with a name specified.")
    {
        ModuleType = moduleType;
    }
}
