using System.Diagnostics;
using Shouldly;

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
    public static readonly IValueConverter LookupAppResource =
        new FuncValueConverter<string, object?>(key =>
        {
            Application.Current.ShouldNotBeNull("Application.Current != null");
            key.ShouldNotBeNull("key != null");
            
            if (Application.Current.Resources.TryGetResource(key, 
                    Application.Current.RequestedThemeVariant,
                    out var resource))
                return resource;
            return null;
        });
}
