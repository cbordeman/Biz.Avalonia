using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biz.Shell.Infrastructure;
using Biz.Shell.Services.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

namespace Biz.Shell.Desktop.Services;

public class DesktopMsalService : IPlatformMsalService
{
    readonly IConfigurationService configurationService;
    readonly ILogger<DesktopMsalService> logger;
    readonly IPublicClientApplication msalClient;
    
    public DesktopMsalService(IConfigurationService configurationService,
        ILogger<DesktopMsalService> logger)
    {
        this.configurationService = configurationService;
        this.logger = logger;
        
        // Initialize MSAL client for Microsoft authentication
        msalClient = PublicClientApplicationBuilder
            .Create(this.configurationService.Authentication.Microsoft.ClientId)
            .WithTenantId(this.configurationService.Authentication.Microsoft.TenantId)

            // Potentially different per platform
            .WithRedirectUri(this.configurationService.Authentication.Microsoft.DesktopRedirectUri)

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
                    .ExecuteAsync(ct);
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (MsalClientException ex) 
            when (ex.ErrorCode == "authentication_canceled")
        {
            throw new OperationCanceledException("Microsoft login was canceled.", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ERROR: Microsoft login failed: {Name} {ExMessage}", ex.GetType().Name, ex.Message);
        }

        return result;
    }
}
