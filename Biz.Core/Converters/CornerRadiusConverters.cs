namespace Biz.Core.Converters;

public static class CornerRadiusConverters
{
    public static readonly IValueConverter ToPixel =
        new FuncValueConverter<CornerRadius, string>(cr =>
            $"{cr.TopLeft} px");
}