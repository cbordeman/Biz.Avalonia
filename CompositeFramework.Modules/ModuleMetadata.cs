using CompositeFramework.Core;
using CompositeFramework.Modules.Exceptions;

namespace CompositeFramework.Modules;

public class ModuleMetadata
{
    public string Name { get; }
    public string? FilePath { get; }
    public string AssemblyQualifiedName { get; }
    public ModuleState State { get; set; }
    public string[] Dependencies { get; }
    public IModule? Instance { get; set; }
    
    public ModuleMetadata(string name,
        string? filePath,
        string assemblyQualifiedName,
        bool isInMemory,
        string[] dependencies)
    {
        ArgumentChecker.ThrowIfNull(name);
        ArgumentChecker.ThrowIfNull(assemblyQualifiedName);
    
        if (filePath is not null && filePath.Length == 0)
            throw new ArgumentException("FullPath must not be empty.", 
                nameof(filePath));
        if (filePath is not null && isInMemory)
            throw new InvalidOperationException(
                $"FullPath cannot be set if isInMemory is true.  " +
                $"Name: {name}, FullPath: {filePath}");    
        
        Name = name;
        FilePath = filePath;
        AssemblyQualifiedName = assemblyQualifiedName;
        State = isInMemory ? ModuleState.InMemory : ModuleState.NotLoaded;
        Dependencies = dependencies;
    }
}