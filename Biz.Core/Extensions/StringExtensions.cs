using System;
using System.Text.Json;
using Biz.Core.Models;

namespace Biz.Core.Extensions;

public static class StringExtensions
{
    public static string GetInternalId(this string id, LoginProvider provider)
    {
        switch (provider)
        {
            case LoginProvider.Local:
                return id;
            case LoginProvider.Google:
                return $"G-{id}";
            case LoginProvider.Microsoft:
                ;
            case LoginProvider.Facebook:
                return $"M-{id}";
            case LoginProvider.Apple:
                return $"A-{id}";
            default:
                throw new ArgumentOutOfRangeException(nameof(provider), provider, null);
        }
    }
    
    /// <summary>
    /// Deserializes from Json.
    /// </summary>
    /// <param name="str"></param>
    public static T? Deserialize<T>(this string str)
    {
        return JsonSerializer.Deserialize<T>(str);
    }
}