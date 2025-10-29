using System;
using Biz.Shared;
using Biz.Shared.Platform;
using Biz.Shell.iOS.Services;
using Serilog;
using UIKit;

namespace Biz.Shell.iOS;

public class Application
{
    // This is the main entry point of the application.
    static void Main(string[] args)
    {
        ClientLogging.Initialize();

        try
        {
            // TODO: setup iOS platform services
            PlatformHelper.PlatformService = new IOsPlatformService();        
        
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, typeof(AppDelegate));
        }
        catch (Exception e)
        {
            Log.Logger.Error(e, "Error during startup: {Message}.", e.Message);
            throw;
        }
    }
}