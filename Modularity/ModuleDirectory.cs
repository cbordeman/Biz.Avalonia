using System.Reflection;
using Modularity.Exceptions;

namespace Modularity;

public class ModuleDirectory : IModuleDirectory
{
    readonly List<ModuleData> modules = new();

    public IReadOnlyCollection<ModuleData> Modules =>
        modules.AsReadOnly();

    public void AddModule(string name,
        string assemblyQualifiedType,
        params string[] dependencies)
    {
        ArgumentChecker.ThrowIfNullOrEmpty(name);
        ArgumentChecker.ThrowIfNullOrEmpty(assemblyQualifiedType);

        if (modules.Any(m => m.Name == name))
            throw new ModuleAlreadyAddedException(name);
        
        var moduleData = new ModuleData(
            name,
            null,
            assemblyQualifiedType,
            true,
            dependencies);
        modules.Add(moduleData);
    }

    public async Task AddModuleDirectory(string fileSpec)
    {
        ArgumentChecker.ThrowIfNullOrEmpty(fileSpec);
        
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
                var fullPath = Path.GetFullPath(Path.Combine(directory, file));
                
                var assembly = Assembly.LoadFrom(fullPath);
                var exported = assembly.ExportedTypes;
                var moduleClass = exported.FirstOrDefault(
                    t => typeof(IModule).IsAssignableFrom(t));
                if (moduleClass == null)
                    throw new AssemblyDoesNotContainModuleException(assembly);
                var moduleAttribute =
                    moduleClass.GetCustomAttribute<ModuleAttribute>();
                if (moduleAttribute == null)
                    throw new MissingModuleAttributeException(moduleClass);
                if (modules.Any(m => m.Name == moduleAttribute.Name))
                    throw new ModuleAlreadyAddedException(
                        moduleAttribute.Name);
                var moduleDependencies = moduleClass
                    .GetCustomAttributes<ModuleDependencyAttribute>()
                    .Select(a => a.DependentModuleName)
                    .Distinct().ToArray();

                var moduleData = new ModuleData(
                    moduleAttribute.Name,
                    fullPath,
                    moduleClass.AssemblyQualifiedName!,
                    false,
                    moduleDependencies);
                modules.Add(moduleData);
            }
        });
    }
}