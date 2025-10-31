using CompositeFramework.Core;
using CompositeFramework.Modules.Attributes;
using CompositeFramework.Modules.Exceptions;
using CompositeFramework.Modules.Extensions;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Mono.Cecil;

namespace CompositeFramework.Modules;

/// <summary>
/// Implementation supports adding referenced assemblies and
/// adding files in a folder using a file specification. 
/// </summary>
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class StandardModuleIndex : IModuleIndex
{
    readonly string moduleAttributeFullName = typeof(ModuleAttribute).FullName!;
    readonly string moduleDependenciesAttributeFullName = typeof(ModuleDependencyAttribute).FullName!;
    
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
        
        try
        {
            var fullDirectory = Path.GetFullPath(rootDir);
            var dirInfo = new DirectoryInfo(fullDirectory);
            if (!dirInfo.Exists)
                throw new DirectoryNotFoundException(fullDirectory);
        
            var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
            matcher.AddInclude(includeFileSpec);
            foreach (var excludeSpec in excludeFileSpecs)
                matcher.AddExclude(excludeSpec);
            var result = matcher.Execute(new DirectoryInfoWrapper(dirInfo));
            var matchedFiles = result.Files.Select(file => Path.Combine(fullDirectory, file.Path));
        
            foreach (string file in matchedFiles)
            {
                var moduleMetadata = GetAssemblyMetadata(file);

                if (modules.Any(m => m.Name == moduleMetadata.Name))
                    throw new ModuleAlreadyAddedException(
                        moduleMetadata.Name);
                modules.Add(moduleMetadata);
            }
            return matchedFiles;            
        }
        catch (Exception)
        {
            // Catch for debug
            throw;
        }
    }

    protected virtual ModuleMetadata GetAssemblyMetadata(string filePath)
    {
        try
        {
            // Load assembly metadata in reflection-only context
            // using Mono.Cecil. 
            var assembly = AssemblyDefinition.ReadAssembly(filePath);

            // Find the first IModule.
            var moduleClass = assembly.MainModule.Types .FirstOrDefault(type => 
                type.IsClass &&
                type.Interfaces.Any(i => 
                    i.InterfaceType.FullName == 
                    typeof(IModule).FullName));
            if (moduleClass == null)
                throw new AssemblyDoesNotContainModuleException(filePath);
            CustomAttribute? moduleAttribute = null;
            foreach (var a in moduleClass.CustomAttributes)
            {
                var attributeTypeFullName = a.AttributeType.FullName;
                if (attributeTypeFullName == moduleAttributeFullName)
                {
                    moduleAttribute = a;
                    break;
                }
            }
            if (moduleAttribute == null)
                throw new MissingModuleAttributeException(moduleClass.Name);
            string[] moduleDependencies = moduleClass
                .CustomAttributes.Where(a =>
                    a.AttributeType.GetType().FullName ==
                    moduleDependenciesAttributeFullName)
                .Select(ca => ca.ReadCustomAttributeConstructorArgument<string>(0))
                .Where(d => !string.IsNullOrWhiteSpace(d))
                .ToArray()!;
            string moduleName = moduleAttribute.ReadCustomAttributeConstructorArgument<string>(0)!;
            string aqn = moduleClass.GetAssemblyQualifiedName();
            var moduleMetadata = new ModuleMetadata(
                moduleName,
                filePath,
                aqn,
                false,
                moduleDependencies);

            return moduleMetadata;
        }
        catch (Exception)
        {
            // Catch for debug.
            throw;
        }
    }
}
