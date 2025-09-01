using System;
using System.Reflection;
using Avalonia;
using Biz.Core;
using Biz.Shell.Desktop.Services;
using Biz.Shell.Infrastructure;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace Biz.Shell.Desktop;

[UsedImplicitly]
sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        PlatformHelper.PlatformService = new DesktopPlatformService();
        
        // Register biz:... links
        RegisterCustomUriSchemeIfMissing();
        
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    // ReSharper disable once MemberCanBePrivate.Global
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    static void RegisterCustomUriSchemeIfMissing()
    {
        // Customize this by modifying AppConstants.
        
#pragma warning disable CA1416 // unimportant platform warning
        var schemeKey = 
            @"SOFTWARE\Classes\" +
            AppConstants.CustomUriSchemeName;
        
        using var key = Registry.CurrentUser.OpenSubKey(schemeKey);
        
        if (key != null)
            return; // Already registered, do nothing

        using var newKey = Registry.CurrentUser.CreateSubKey(schemeKey);
        
        newKey.SetValue("", $"URL:{
            AppConstants.CustomUriProtocolFriendlyName}");
        newKey.SetValue("URL Protocol", "");

        // Set icon (optional)
        var exePath = Assembly.GetEntryAssembly()!.Location;
        using var iconKey = newKey.CreateSubKey("DefaultIcon");
        iconKey.SetValue("", exePath + ",1");

        // Set shell\open\command handler
        using var commandKey = newKey.CreateSubKey(@"shell\open\command");
        commandKey.SetValue("", $"\"{exePath}\" \"%1\"");
#pragma warning restore CA1416
    }
}