using Avalonia.Media;

namespace Biz.Shell.Converters;

public static class BooleanConverters
{
    public static readonly IValueConverter NullOrString =
        new FuncValueConverter<bool, string?, string?>((value, param) => value ? null : param);

    public static readonly IValueConverter Reverse =
        new FuncValueConverter<bool, bool>(value => !value);
    
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
                // TODO: Add new icon to collapse sidebar
                return AppHelpers.GetAppStyleResource<Geometry>("hamburger") ?? throw new InvalidOperationException();
            else
                return AppHelpers.GetAppStyleResource<Geometry>("hamburger") ?? throw new InvalidOperationException();
        });
}