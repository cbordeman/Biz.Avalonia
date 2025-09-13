using System.Reflection;

namespace CompositeFramework.Modules.Exceptions;

public class AssemblyDoesNotContainModuleException : Exception
{
    public string FilePath { get; }
    
    public AssemblyDoesNotContainModuleException(string filePath) : 
        base($"Assembly {filePath} " +
             $"doesn't contain any classes that derive from IModule.") =>
        FilePath = filePath;
}