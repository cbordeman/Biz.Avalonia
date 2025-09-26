using System.Runtime.CompilerServices;

namespace CompositeFramework.Core;

public static class ArgumentChecker 
{
    public static void ThrowIfNullOrWhiteSpace(
        string? argument, 
        [CallerArgumentExpression(nameof(argument))] 
        string? parameterName = null)
    {
        if (string.IsNullOrWhiteSpace(argument))
            throw new ArgumentException(
                $"Argument '{argument}' is null or whitespace.", 
                parameterName);
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