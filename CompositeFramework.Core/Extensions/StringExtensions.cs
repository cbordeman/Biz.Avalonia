using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CompositeFramework.Core.Extensions;

public static class StringExtensions
{
    public static string? ReplaceEnd(this string? str, string oldStr, string newStr)
    {
        if (str != null && str.EndsWith(oldStr))
            return str.Substring(0, str.Length - oldStr.Length) + newStr;
        return str;
    }

    public static void OpenUrlCrossPlatform(this string url)
    {
        ArgumentChecker.ThrowIfNullOrEmpty(url);
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start(new ProcessStartInfo(url!.Replace("&", "^&"))
            {
                UseShellExecute = true
            });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", url!);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", url!);
        }
    }
    
    // public static bool IsView(this string typeName)
    // {
    //     foreach (var suffix in CompositeConfiguration.ViewSuffixes)
    //         if (typeName.EndsWith(suffix))
    //             return true;
    //     return false;
    // }
}
