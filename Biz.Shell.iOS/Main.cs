using Biz.Shell.iOS.Services;
using Biz.Shell.Platform;
using UIKit;

namespace Biz.Shell.iOS;

public class Application
{
    // This is the main entry point of the application.
    static void Main(string[] args)
    {
        // TODO: setup iOS platform services
        PlatformHelper.PlatformService = new IOsPlatformService();        
        
        // if you want to use a different Application Delegate class from "AppDelegate"
        // you can specify it here.
        UIApplication.Main(args, null, typeof(AppDelegate));
    }
}