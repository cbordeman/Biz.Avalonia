namespace CompositeFramework.Core.Extensions;

public static class DictionaryExtensions
{
    public static TValue? GetClassOrDefault<TValue>(this IDictionary<string, object> dictionary, string key)
        where TValue : class =>
        dictionary.TryGetValue(key, out var value) ? value as TValue : null;

    public static string GetStringOrEmpty(this IDictionary<string, object> dictionary, string key)
    {
        if (dictionary.TryGetValue(key, out object? value) &&
            value is string str)
            return value?.ToString() ?? string.Empty;
        return string.Empty;
    }

    public static TValue GetStructOrDefault<TValue>(this IDictionary<string, object> dictionary, string key)
        where TValue : struct =>
        dictionary.TryGetValue(key, out var value) ? (TValue)value : default;
}