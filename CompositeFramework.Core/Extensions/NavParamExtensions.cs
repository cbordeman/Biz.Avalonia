namespace CompositeFramework.Core.Extensions;

/// <summary>
/// Extension methods to help get values from navigation parameters.
/// </summary>
public static class NavParamExtensions
{
    // public static TValue? GetObject<TValue>(this NavParam[] parameters,
    //     string key)
    //     where TValue : class
    // {
    //     return (TValue?)parameters.FirstOrDefault(x => x.Name == key)?.Value;
    // }

    public static TValue GetValueOrDefault<TValue>(this NavParam[] parameters,
        string key)
    {
        var v = parameters.FirstOrDefault(x => x.Name == key);
        if (v?.Value == null)
            return default!;
        return (TValue)v.Value;
    }
    
    public static string GetStringOrEmpty(this NavParam[] parameters,
        string key)
    {
        return parameters.FirstOrDefault(x => x.Name == key) is { Value: string str } 
            ? str : string.Empty;
    }

    // public static TValue GetStructOrDefault<TValue>(this NavParam[] parameters,
    //     string key)
    //     where TValue : struct
    // {
    //     return parameters.FirstOrDefault(x => x.Name == key) is { Value: TValue val }
    //         ? val : default;
    // }
}