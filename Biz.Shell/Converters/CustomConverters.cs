using Avalonia.Media;

namespace Biz.Shell.Converters;

public static class CustomConverters
{
    /// <summary>
    /// Pass currently selected (string) and item's selection (string)
    /// as CommandParameter.
    /// </summary>
    public static readonly IValueConverter ToSelectionBackgroundBrush =
        new FuncValueConverter<string?, string?, IBrush?>(Convert);

    static IBrush? Convert(string? value, string? parameter)
    {
        if (value is null || parameter is null)
            // ReSharper disable once NotResolvedInText
            throw new ArgumentNullException(
                "Binding value or CommandParameter is null.");
        
        if (!string.Equals(parameter, value, StringComparison.Ordinal))
            return Brushes.Transparent;

        return AppHelpers.GetAppStyleResource<IBrush>("SelectionColor");
    }
}