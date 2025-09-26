using CompositeFramework.Core;
using CompositeFramework.Modules.Attributes;
using CompositeFramework.Modules.Exceptions;
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

    public async virtual Task AddModuleFiles(string fileSpec)
    {
        ArgumentChecker.ThrowIfNullOrWhiteSpace(fileSpec);

        // Does not load assemblies into memory or
        // initialize them.
        await Task.Run(() =>
        {
            var directory = Path.GetDirectoryName(fileSpec);
            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException(directory);
            var pattern = Path.GetFileName(fileSpec);
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentException(
                    "No file pattern specified.  " +
                    "Should match only module DLLs, " +
                    "such as *.Modules.*.dll");

            foreach (string file in Directory.GetFiles(directory))
            {
                var filePath = Path.Combine(directory, file);

                var moduleMetadata = GetAssemblyMetadata(filePath);

                if (modules.Any(m => m.Name == moduleMetadata.Name))
                    throw new ModuleAlreadyAddedException(
                        moduleMetadata.Name);
                modules.Add(moduleMetadata);
            }
        });
    }

    protected virtual ModuleMetadata GetAssemblyMetadata(string filePath)
    {
        // Load assembly in reflection-only context inside this AppDomain
        var assembly = AssemblyDefinition.ReadAssembly(filePath);

        // Find the first IModule.
        var moduleClass = assembly.MainModule.Types.FirstOrDefault(type =>
            type.IsClass &&
            type.Interfaces.Any(i =>
                i.InterfaceType.Name == typeof(IModule).FullName));
        if (moduleClass == null)
            throw new AssemblyDoesNotContainModuleException(filePath);
        var moduleAttribute =
            moduleClass.CustomAttributes.FirstOrDefault(
                a => a.AttributeType.GetType() == 
                     typeof(ModuleAttribute));
        if (moduleAttribute == null)
            throw new MissingModuleAttributeException(moduleClass.Name);
        var moduleDependencies = moduleClass
            .CustomAttributes.Where(a =>
                a.AttributeType.GetType() ==
                typeof(ModuleDependencyAttribute))
            .Select(ca => (string)ReadCustomAttributeProperty(ca,
                nameof(ModuleDependencyAttribute.DependentModuleName))!)
            .ToArray();
        var moduleName = (string)ReadCustomAttributeProperty(moduleAttribute,
            nameof(ModuleAttribute.Name))!;
        if (string.IsNullOrWhiteSpace(moduleName))
            throw new MissingModuleAttributeException(moduleClass.Name);
        var aqn = GetAssemblyQualifiedName(moduleClass);
        var moduleMetadata = new ModuleMetadata(
            moduleName, filePath, aqn, false, moduleDependencies);

        return moduleMetadata;
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