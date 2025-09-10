using Modularity.Exceptions;

namespace Modularity;

public class ModuleData
{
    public string Name { get; internal set; }
    public string? FullPath { get; }
    public string AssemblyQualifiedName { get; }
    public ModuleState State { get; internal set; }
    public string[] Dependencies { get; }
    
    internal ModuleData(string name,
        string? fullPath,
        string assemblyQualifiedName,
        bool isInMemory,
        string[] dependencies)
    {
        ArgumentChecker.ThrowIfNull(name);
        ArgumentChecker.ThrowIfNull(assemblyQualifiedName);
    
        if (fullPath is not null && fullPath.Length == 0)
            throw new ArgumentException("FullPath must not be empty.", 
                nameof(fullPath));
        if (fullPath is not null && isInMemory)
            throw new InvalidOperationException(
                $"FullPath cannot be set if isInMemory is true.  " +
                $"Name: {name}, FullPath: {fullPath}");    
        
        Name = name;
        FullPath = fullPath;
        AssemblyQualifiedName = assemblyQualifiedName;
        State = isInMemory ? ModuleState.InMemory : ModuleState.NotLoaded;
        Dependencies = dependencies;
    }
}