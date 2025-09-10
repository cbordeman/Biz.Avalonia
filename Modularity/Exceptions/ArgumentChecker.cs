using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Modularity.Exceptions;

public static class ArgumentChecker 
{
    public static void ThrowIfNullOrEmpty(
        string? argument, 
        [CallerArgumentExpression(nameof(argument))] 
        string? paramName = null)
    {
        if (string.IsNullOrEmpty(argument))
            throw new ArgumentException(
                $"Argument '{argument}' is null or empty.", 
                paramName);
    }
    
    public static void ThrowIfNull(
        object? argument, 
        [CallerArgumentExpression(nameof(argument))] 
        string? paramName = null)
    {
        if (argument is null)
            throw new ArgumentNullException(paramName);
    }
}

public class CallerArgumentExpressionAttribute : Attribute
{
    public CallerArgumentExpressionAttribute(string argumentName)
    {
        throw new NotImplementedException();
    }
}