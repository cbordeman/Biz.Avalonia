namespace CompositeFramework.Modules.Exceptions;

public class ModuleAlreadyAddedException : Exception
{
    public string Name { get; }
    
    public ModuleAlreadyAddedException(string name) 
        : base($"Module {name} already added.") =>
        Name = name;
}
