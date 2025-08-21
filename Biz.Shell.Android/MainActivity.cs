using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Avalonia;
using Avalonia.Android;
using Biz.Shell;
using Biz.Platform;
using Biz.Shell.Infrastructure;
using Microsoft.Identity.Client;

namespace Biz.Shell.Android;

[Activity(
    Exported = true,
    Label = "Biz.Shell.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        PlatformHelper.RegistrationService = new MobileRegistrationService();
        
        base.OnCreate(savedInstanceState);
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
    
    protected override void OnActivityResult(
        int requestCode, 
        Result resultCode, 
        Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        // This is required for the MSAL library to receive the authentication result
        AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
    }
}
