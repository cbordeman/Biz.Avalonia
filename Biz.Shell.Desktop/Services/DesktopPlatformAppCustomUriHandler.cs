using System;
using System.Threading.Tasks;
using Biz.Shell.Platform;
using Microsoft.Extensions.Logging;
using Prism.Ioc;

namespace Biz.Shell.Desktop.Services;

public class DesktopPlatformAppCustomUriHandler
    : IPlatformAppCustomUriHandler
{
    readonly ILogger<DesktopPlatformAppCustomUriHandler> logger;
    public DesktopPlatformAppCustomUriHandler(
        ILogger<DesktopPlatformAppCustomUriHandler> logger)
    {
        this.logger = logger;
    }

    public async Task HandleUri(string uriString)
    {
        try
        {
            var uri = new Uri(uriString);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

            string? token = query["token"];
            string? email = query["email"];

            // TODO: Open views
            switch (uri.Host)
            {
                case IPlatformAppCustomUriHandler.ShowUiForForgotPassword:
                    
                    break;
                case IPlatformAppCustomUriHandler.ShowUiForRegisterUser:
                    break;
                
                default:
                    throw new Exception($"Unknown URI: {uri.Host}");
                
            }

        }
        catch (Exception e)
        {
            logger.LogError(e, $"Failed to handle URI {uriString}: {e.GetType().Name}: {e.Message}");
        }
    }
}