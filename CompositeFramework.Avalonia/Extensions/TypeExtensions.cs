using System;
using System.Reflection;

namespace CompositeFramework.Avalonia.Extensions;

public static class TypeExtensions
{
    public static string GetAssemblyQualifiedNameWithNewTypeName
        (this Type type, string newTypeFullName)
    {
        string assemblyName = type.Assembly.GetName()
            .FullName;
        var rval = Assembly.CreateQualifiedName(
            assemblyName, newTypeFullName);
        return rval;
    }
}
