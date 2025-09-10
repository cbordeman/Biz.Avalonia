using Modularity.Exceptions;

namespace Modularity;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ModuleAttribute : Attribute
{
    public string Name { get; }

    public ModuleAttribute(string name)
    {
        ArgumentChecker.ThrowIfNullOrEmpty(name);
        
        Name = name;
    }
}
