using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Biz.Shared;
using Biz.Shared.Platform;
using Biz.Shell.Browser.Services;
using Serilog;

internal sealed partial class Program
{
    private static Task Main(string[] args)
    {
        ClientLogging.Initialize();

        try
        {
            BuildAvaloniaApp()
                .WithInterFont()
                .StartBrowserAppAsync("out");
            
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            Log.Logger.Error(e, "Error during startup: {Message}.", e.Message);
            throw;
        }
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        // TODO: setup browser platform services
        PlatformHelper.PlatformService = new BrowserPlatformService();
        return AppBuilder.Configure<App>();
    }
}
