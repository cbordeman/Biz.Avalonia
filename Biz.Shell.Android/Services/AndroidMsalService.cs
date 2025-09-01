using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.Util;
using Biz.Shell.Infrastructure;
using Biz.Shell.Platform;
using Biz.Shell.Services.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

namespace Biz.Shell.Android.Services;

public class AndroidMsalService : IPlatformMsalService
{
    readonly IConfigurationService configurationService;
    readonly ILogger<AndroidMsalService> logger;
    private IPublicClientApplication msalClient;
    
    public AndroidMsalService(IConfigurationService configurationService,
        ILogger<AndroidMsalService> logger)
    {
        this.configurationService = configurationService;
        this.logger = logger;
        
        // Initialize MSAL client for Microsoft authentication
        msalClient = PublicClientApplicationBuilder
            .Create(this.configurationService.Authentication.Microsoft.ClientId)
            .WithTenantId(this.configurationService.Authentication.Microsoft.TenantId)

            // Android requires this.
            .WithParentActivityOrWindow(() => MainActivity.GetActivity!())

            // Android only supports redirect URIs with custom schemes
            .WithRedirectUri(this.configurationService.Authentication.Microsoft.MobileRedirectUri)

            .WithLogging(
                (level, message, _) =>
                {
                    if (message != null)
                    {
                        logger.Log(
                            level switch
                            {
                                Microsoft.Identity.Client.LogLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
                                Microsoft.Identity.Client.LogLevel.Warning => Microsoft.Extensions.Logging.LogLevel.Warning,
                                Microsoft.Identity.Client.LogLevel.Info => Microsoft.Extensions.Logging.LogLevel.Information,
                                Microsoft.Identity.Client.LogLevel.Always => Microsoft.Extensions.Logging.LogLevel.Information,
                                Microsoft.Identity.Client.LogLevel.Verbose => Microsoft.Extensions.Logging.LogLevel.Debug,
                                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
                            },
                            message);
                        Log.Debug("MSAL-VERBOSE", $"[{level}] {message}");
                    }
                },
                Microsoft.Identity.Client.LogLevel.Info,
#if DEBUG
                enablePiiLogging: true, // Personally Identifiable Information 
#endif
                enableDefaultPlatformLogging: true)
            .Build();
    }
    
    public async Task ClearCache()
    {
        foreach (var acct in await msalClient.GetAccountsAsync())
            await msalClient.RemoveAsync(acct);
    }
    
    public async Task<AuthenticationResult?> LoginUsingMsal(CancellationToken ct)
    {
        AuthenticationResult? result = null;
        try
        {
            var accounts = await msalClient.GetAccountsAsync();
            string[] scopes = configurationService.Authentication.Microsoft.Scopes;
            var builder = msalClient.AcquireTokenInteractive(scopes);
            try
            {
                result = await msalClient
                    .AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                    .ExecuteAsync(ct);
            }
            catch (MsalUiRequiredException)
            {
                result = await builder
                    .WithUseEmbeddedWebView(true)
                    .ExecuteAsync(ct);
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ERROR: Microsoft login failed: {Name} {ExMessage}", ex.GetType().Name, ex.Message);
        }

        return result;
    }
}
