namespace Biz.Shell.Converters;

public static class ObjectConverters
{
    public static readonly IValueConverter NullToTrue =
        new FuncValueConverter<string, object>(mode => mode == null);
    
    public static readonly IValueConverter NonNullToTrue =
        new FuncValueConverter<string, object>(mode => mode != null);
}