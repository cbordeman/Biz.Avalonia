using System.Text.Json;

namespace Biz.Core.Extensions;

public static class ObjectExtensions
{
    static readonly JsonSerializerOptions JsonSerializationOptions =
        new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    
    /// <summary>
    /// Serializes to Json.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string SerializeToJson(this object obj) =>
        JsonSerializer.Serialize(obj, JsonSerializationOptions);
}
