using CompositeFramework.Core;
using CompositeFramework.Modules.Exceptions;

namespace CompositeFramework.Modules.Attributes;

/// <summary>
/// If module is loaded via IModuleIndex.AddModuleFiles(),
/// this attribute is necessary to specify the name of
/// dependency modules.  Dependency modules will be
/// recursively loaded and initialized before this module.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ModuleDependencyAttribute : Attribute
{
    /// <summary>
    /// Name of the dependent module.
    /// </summary>
    public string DependentModuleName { get; }
    
    public ModuleDependencyAttribute(string dependentModuleName)
    {
        ArgumentChecker.ThrowIfNullOrWhiteSpace(dependentModuleName);
        
        DependentModuleName = dependentModuleName;

    }
}