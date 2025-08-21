using Avalonia;
using Avalonia.Browser;
using Biz.Shell;
using System.Threading.Tasks;
using Biz.Shell;
using Biz.Platform;

internal sealed partial class Program
{
    private static Task Main(string[] args) => BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
    {
        PlatformHelper.RegistrationService = new MobileRegistrationService();
        return AppBuilder.Configure<App>();
    }
}