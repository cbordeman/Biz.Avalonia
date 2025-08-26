using Android.App;
using Android.Content;
using Microsoft.Identity.Client;

namespace Biz.Shell.Android;

// This replicates what you would put in the AndroidManifest.xml
[Activity(Exported = true)]
[IntentFilter(
    [
        Intent.ActionView
    ],
    Categories =
    [
        Intent.CategoryDefault, Intent.CategoryBrowsable
    ],
    DataScheme = "msale872b112-dec9-4763-be06-a99ec0144724",
    DataHost = "auth")]
public class MsalRedirectActivity : BrowserTabActivity
{
    // No code needed, this just enables the redirect URI intent filtering

    public MsalRedirectActivity()
    {
    }
}
