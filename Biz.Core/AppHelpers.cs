using System;
using System.Diagnostics;

namespace Biz.Core;

public static class AppHelpers
{
    public static T GetAppStyleResource<T>(string key) where T: class
    {
        Debug.Assert(Application.Current != null, "Application.Current != null");
        Debug.Assert(!string.IsNullOrEmpty(key), $"{nameof(key)} is null or empty.");
        
        var resource = Application.Current.Styles.TryGetResource(key,
            Application.Current.ActualThemeVariant,
            out var res) ? res : null;
        if (resource != null)
            return (T)resource;
        else
            throw new Exception($"App Style Resource {key} not found");
    }
    
    public static T GetAppResource<T>(string key) where T: class
    {
        Debug.Assert(Application.Current != null, "Application.Current != null");
        Debug.Assert(key != null, nameof(key) + " != null");
        
        var resource = Application.Current.Resources.TryGetResource(key,
            Application.Current.ActualThemeVariant,
            out var res) ? res : null;
        if (resource != null)
            return (T)resource;
        else
            throw new Exception($"App Regular Resource {key} not found");
    }
}