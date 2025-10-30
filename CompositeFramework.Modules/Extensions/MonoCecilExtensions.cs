using Mono.Cecil;

namespace CompositeFramework.Modules.Extensions;

public static class MonoCecilExtensions
{
    /// <summary>
    /// Reads a Mono.Cecil CustomAttribute's constructor argument. 
    /// </summary>
    public static T? ReadCustomAttributeConstructorArgument<T>(
        this CustomAttribute ca, int index)
    {
        if (index >= ca.ConstructorArguments.Count)
            throw new Exception($"Constructor argument index {index} does not exist.");
        var val = ca.ConstructorArguments[index].Value;
        return val switch
        {
            null => default,
            T t => t,
            _ => throw new Exception($"Constructor argument {index} is not of type {typeof(T).FullName}.")
        };
    }
    
    /// <summary>
    /// Reads a Mono.Cecil CustomAttribute's named property, which
    /// is a property with a getter and a setter. 
    /// </summary>
    public static T? ReadCustomAttributeNamedProperty<T>(
        this CustomAttribute ca, string propertyName)
    {
        var property = ca.Properties.FirstOrDefault(p =>
            p.Name == propertyName);
    
        if (property.Name != null)
        {
            // Value is stored in property.Argument.Value
            var val = property.Argument.Value;
            return val switch
            {
                null => default,
                T t => t,
                _ => throw new Exception($"Named Property {propertyName} is not of type {typeof(T).FullName}.")
            };
        }
        else
        {
            throw new Exception($"Property {propertyName} " +
                                $"not found on attribute " +
                                $"{ca.AttributeType.FullName}.");
        }
    }

    public static string GetAssemblyQualifiedName(
        this TypeDefinition typeDef)
    {
        // includes version, culture, PKT
        var assemblyName = typeDef.Module.Assembly.Name.FullName;
        return $"{typeDef.FullName}, {assemblyName}";
    }
}