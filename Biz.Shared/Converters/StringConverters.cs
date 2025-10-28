using Biz.Shared.Infrastructure;

namespace Biz.Shared.Converters;

public static class StringConverters
{
    public static readonly IValueConverter ToSelectionMode =
        new FuncValueConverter<string, SelectionMode>(mode => mode switch
        {
            "Single" => SelectionMode.Single,
            "Multiple" => SelectionMode.Multiple,
            "Toggle" => SelectionMode.Toggle,
            "Always Selected" => SelectionMode.AlwaysSelected,
            _ => throw new ArgumentOutOfRangeException(nameof(SelectionMode), mode,
                "Expected Single, Multiple, Toggle, or Always Selected.")
        });
    public static readonly IValueConverter AppResource =
        new FuncValueConverter<string, object?>(key =>
        {
            if (key == null)
                return null;
            return AppHelpers.GetAppResource<object>(key);
        });
    public static readonly IValueConverter AppStyleResource =
        new FuncValueConverter<string, object?>(key =>
        {
            if (key == null)
                return null;
            return AppHelpers.GetAppStyleResource<object>(key);
        });
    public static readonly IValueConverter NullOrEmpty =
        new FuncValueConverter<string, bool>(string.IsNullOrEmpty);
    public static readonly IValueConverter NotNullOrEmpty =
        new FuncValueConverter<string, bool>(str => !string.IsNullOrEmpty(str));
}
