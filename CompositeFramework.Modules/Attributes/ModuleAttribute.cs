using CompositeFramework.Modules.Exceptions;

namespace CompositeFramework.Modules.Attributes;

/// <summary>
/// If added via IModuleIndex.AddModuleFiles(), this is
/// required to give the module a name.
/// </summary>
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