using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CompositeFramework.Core.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Replaces text if found at the end of the string.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="oldStr"></param>
    /// <param name="newStr"></param>
    /// <returns></returns>
    public static string? ReplaceEnd(this string? str, string oldStr, string newStr)
    {
        if (str != null && str.EndsWith(oldStr))
            return str.Substring(0, str.Length - oldStr.Length) + newStr;
        return str;
    }

    /// <summary>
    /// Opens a URL using the platform specific API.
    /// </summary>
    /// <param name="url"></param>
    public static void OpenUrlCrossPlatform(this string url)
    {
        ArgumentChecker.ThrowIfNullOrWhiteSpace(url);
        
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
}
