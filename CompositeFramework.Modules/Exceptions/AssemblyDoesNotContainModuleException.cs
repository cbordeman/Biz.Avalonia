using System.Reflection;

namespace CompositeFramework.Modules.Exceptions;

public class AssemblyDoesNotContainModuleException : Exception
{
    public string FilePath { get; }
    
    public AssemblyDoesNotContainModuleException(string filePath) : 
        base($"Assembly {Path.GetFileNameWithoutExtension(filePath)} " +
             $"doesn't contain any classes that inherit from IModule.") =>
        FilePath = filePath;
}