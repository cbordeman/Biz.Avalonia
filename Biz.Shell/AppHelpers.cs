using System.Diagnostics;

namespace Biz.Shell;

public static class AppHelpers
{
    public static T? GetAppStyleResource<T>(string key) where T: class
    {
        if (string.IsNullOrEmpty(key)) return null;
        
        var resource = Application.Current!.Styles.TryGetResource(key,
            Application.Current.ActualThemeVariant,
            out var res) ? res : null;
        if (resource != null)
            return (T)resource;
        else if (Debugger.IsAttached)
            Debugger.Break();
        return null;
    }
    
    public static T? GetAppResource<T>(string key) where T: class
    {
        if (string.IsNullOrEmpty(key)) return null;
        
        var resource = Application.Current!.Resources.TryGetResource(key,
            Application.Current.ActualThemeVariant,
            out var res) ? res : null;
        if (resource != null)
            return (T)resource;
        else if (Debugger.IsAttached)
            Debugger.Break();
        return null;
    }
}