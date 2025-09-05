using System;
using System.Text.Json;
using Biz.Core.Models;

namespace Biz.Core.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Deserializes from Json.
    /// </summary>
    /// <param name="str"></param>
    public static T? Deserialize<T>(this string str)
    {
        return JsonSerializer.Deserialize<T>(str);
    }
}