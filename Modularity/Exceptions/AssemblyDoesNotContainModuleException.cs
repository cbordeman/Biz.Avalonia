using System.Reflection;

namespace Modularity.Exceptions;

public class AssemblyDoesNotContainModuleException : Exception
{
    public Assembly Assembly { get; }
    
    public AssemblyDoesNotContainModuleException(Assembly assembly) : 
        base($"Assembly {assembly.GetName().Name} " +
             $"doesn't contain any classes that derive from IModule.") =>
        Assembly = assembly;
}