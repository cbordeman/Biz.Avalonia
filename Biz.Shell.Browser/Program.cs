using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Biz.Shell;
using Biz.Shell.Browser.Services;
using Biz.Shell.Infrastructure;

internal sealed partial class Program
{
    private static Task Main(string[] args) => BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
    {
        // TODO: setup browser platform services
        PlatformHelper.PlatformService = new BrowserPlatformService();
        return AppBuilder.Configure<App>();
    }
}