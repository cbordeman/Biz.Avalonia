namespace Modularity;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ModuleDependencyAttribute : Attribute
{
    public string DependentModuleName { get; }
    
    public ModuleDependencyAttribute(string dependentModuleName)
    {
        DependentModuleName = dependentModuleName;

    }
}