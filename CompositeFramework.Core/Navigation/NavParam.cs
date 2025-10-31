namespace CompositeFramework.Core.Navigation;

/// <summary>
/// A navigation parameter.
/// </summary>
/// <param name="Name"></param>
/// <param name="Value"></param>
public record NavParam(string Name, object? Value);