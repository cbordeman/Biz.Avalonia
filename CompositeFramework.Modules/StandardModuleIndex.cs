using CompositeFramework.Core;
using CompositeFramework.Modules.Attributes;
using CompositeFramework.Modules.Exceptions;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Mono.Cecil;

namespace CompositeFramework.Modules;

/// <summary>
/// Implementation supports adding referenced assemblies and
/// adding files in a folder using a file specification. 
/// </summary>
public class StandardModuleIndex : IModuleIndex
{
    readonly List<ModuleMetadata> modules = new();

    public IReadOnlyCollection<ModuleMetadata> Modules =>
        modules.AsReadOnly();

    public virtual void AddModule(string name,
        string assemblyQualifiedType,
        params string[] dependencies)
    {
        ArgumentChecker.ThrowIfNullOrWhiteSpace(name);
        ArgumentChecker.ThrowIfNullOrWhiteSpace(assemblyQualifiedType);

        if (modules.Any(m => m.Name == name))
            throw new ModuleAlreadyAddedException(name);

        var moduleData = new ModuleMetadata(
            name,
            null,
            assemblyQualifiedType,
            true,
            dependencies);
        modules.Add(moduleData);
    }

    public virtual IEnumerable<string> AddModuleFilesDirectory(
        string rootDir, 
        string includeFileSpec, 
        params IEnumerable<string> excludeFileSpecs)
    {
        ArgumentChecker.ThrowIfNullOrWhiteSpace(rootDir);
        ArgumentChecker.ThrowIfNullOrWhiteSpace(includeFileSpec);

        // Does not load assemblies into memory or
        // initialize them.
        var fullDirectory = Path.GetFullPath(rootDir);
        var dirInfo = new DirectoryInfo(fullDirectory);
        if (!dirInfo.Exists)
            throw new DirectoryNotFoundException(fullDirectory);
        
        var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
        matcher.AddInclude(includeFileSpec);
        foreach (var excludeSpec in excludeFileSpecs)
            matcher.AddExclude(excludeSpec);
        var result = matcher.Execute(new DirectoryInfoWrapper(dirInfo));
        var matchedFiles = result.Files.Select(file => Path.Combine(rootDir, file.Path));
        
        foreach (string file in matchedFiles)
        {
            var filePath = Path.Combine(fullDirectory, file);

            var moduleMetadata = GetAssemblyMetadata(filePath);

            if (modules.Any(m => m.Name == moduleMetadata.Name))
                throw new ModuleAlreadyAddedException(
                    moduleMetadata.Name);
            modules.Add(moduleMetadata);
        }
        return matchedFiles;
    }

    protected virtual ModuleMetadata GetAssemblyMetadata(string filePath)
    {
        AssemblyDefinition? assembly;
        TypeDefinition? moduleClass;
        CustomAttribute? moduleAttribute;
        string[] moduleDependencies;
        string moduleName;
        string aqn;
        ModuleMetadata moduleMetadata;
        try
        {
            // Load assembly metadata in reflection-only context
            // using Mono.Cecil. 
            assembly = AssemblyDefinition.ReadAssembly(filePath);
            
            // Find the first IModule.
            moduleClass = null;
            foreach (var type in assembly.MainModule.Types)
            {
                if (type.IsClass && type.Interfaces.Any(i => i.InterfaceType.Name == typeof(IModule).FullName))
                {
                    moduleClass = type;
                    break;
                }
            }
            if (moduleClass == null)
                throw new AssemblyDoesNotContainModuleException(filePath);
            moduleAttribute = moduleClass.CustomAttributes.FirstOrDefault(
                a => a.AttributeType.GetType() ==
                     typeof(ModuleAttribute));
            if (moduleAttribute == null)
                throw new MissingModuleAttributeException(moduleClass.Name);
            moduleDependencies = moduleClass
                .CustomAttributes.Where(a =>
                    a.AttributeType.GetType() ==
                    typeof(ModuleDependencyAttribute))
                .Select(ca => (string)ReadCustomAttributeProperty(ca,
                    nameof(ModuleDependencyAttribute.DependentModuleName))!)
                .ToArray();
            moduleName = (string)ReadCustomAttributeProperty(moduleAttribute,
                nameof(ModuleAttribute.Name))!;
            if (string.IsNullOrWhiteSpace(moduleName))
                throw new MissingModuleAttributeException(moduleClass.Name);
            aqn = GetAssemblyQualifiedName(moduleClass);
            moduleMetadata = new ModuleMetadata(
                moduleName,
                filePath,
                aqn,
                false,
                moduleDependencies);

            return moduleMetadata;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    static object? ReadCustomAttributeProperty(CustomAttribute ca, string propertyName)
    {
        var property = ca.Properties.FirstOrDefault(p =>
            p.Name == propertyName);

        if (property.Name != null)
        {
            // Value is stored in property.Argument.Value
            return property.Argument.Value;
        }
        else
        {
            throw new Exception($"Property {propertyName} " +
                                $"not found on attribute " +
                                $"{ca.AttributeType.FullName}.");
        }
    }

    static string GetAssemblyQualifiedName(TypeDefinition typeDef)
    {
        // includes version, culture, PKT
        var assemblyName = typeDef.Module.Assembly.Name.FullName;
        return $"{typeDef.FullName}, {assemblyName}";
    }
}
