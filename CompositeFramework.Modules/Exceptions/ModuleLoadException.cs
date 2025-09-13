namespace CompositeFramework.Modules.Exceptions;

public class ModuleLoadException : Exception
{
    public string Name { get; }
    public string FullPath { get; }
    
    public ModuleLoadException(string name, string fullPath,
        Exception innerException) : base(
            "Failed to load module {name} from {fullPath}.",
            innerException)
    {
        Name = name;
        FullPath = fullPath;
    }
}
