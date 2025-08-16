using Avalonia.Media;

namespace Biz.Core.Converters;

public static class BooleanConverters
{
    public static readonly IValueConverter NullOrString =
        new FuncValueConverter<bool, string?, string?>((value, param) => value ? null : param);

    public static readonly IValueConverter Opaque =
        new FuncValueConverter<bool, int>(value => value ? 1 : 0);

    public static readonly IValueConverter SidebarPadding =
        new FuncValueConverter<bool, Thickness>(value => value ? new Thickness(16, 8) : new Thickness(8));

    public static readonly IValueConverter SidebarTogglerHorizontalAlignment =
        new FuncValueConverter<bool, HorizontalAlignment>(value =>
            value ? HorizontalAlignment.Right : HorizontalAlignment.Center);

    public static readonly IValueConverter SidebarToggleIcon =
        new FuncValueConverter<bool, Geometry>(value =>
        {
            if (value)
                return AppHelpers.GetAppStyleResource<Geometry>("hamburger");
            else
                return AppHelpers.GetAppStyleResource<Geometry>("hamburger");
        });
}