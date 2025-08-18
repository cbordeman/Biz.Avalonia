using System.Diagnostics;

namespace Biz.Core.Converters;

public static class StringConverters
{
    public static readonly IValueConverter ToSelectionMode =
        new FuncValueConverter<string, SelectionMode>(mode => mode switch
        {
            "Single" => SelectionMode.Single,
            "Multiple" => SelectionMode.Multiple,
            "Toggle" => SelectionMode.Toggle,
            "Always Selected" => SelectionMode.AlwaysSelected,
            _ => SelectionMode.Single
        });
    public static readonly IValueConverter AppResource =
        new FuncValueConverter<string, object?>(key =>
        {
            Debug.Assert(!string.IsNullOrEmpty(key), $"{nameof(key)} is null or empty.");
            return AppHelpers.GetAppResource<object>(key);
        });
    public static readonly IValueConverter AppStyleResource =
        new FuncValueConverter<string, object?>(key =>
        {
            Debug.Assert(!string.IsNullOrEmpty(key), $"{nameof(key)} is null or empty.");
            return AppHelpers.GetAppStyleResource<object>(key);
        });
}
