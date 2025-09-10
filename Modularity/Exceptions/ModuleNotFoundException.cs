namespace Modularity.Exceptions;

public class ModuleNotFoundException : Exception
{
    public string Name { get; }
    
    public ModuleNotFoundException(string name) 
        : base($"Module {name} not found in module directory.") =>
        Name = name;
}