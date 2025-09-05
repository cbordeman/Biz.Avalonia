using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biz.Core.Extensions;
using Biz.Core.Models;
using Biz.Shell.ClientLoginProviders;
using Biz.Shell.Services.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using ServiceClients;
using LogLevel = Microsoft.Identity.Client.LogLevel;

namespace Biz.Shell.Desktop.Services.Auth;

public class MicrosoftLoginProvider : IClientLoginProvider
{
    readonly IConfigurationService configurationService;
    readonly IPublicClientApplication msalClient;
    readonly ILogger<MicrosoftLoginProvider> logger;

    public MicrosoftLoginProvider(ILogger<MicrosoftLoginProvider> logger,
        IConfigurationService configurationService)
    {
        this.logger = logger;
        this.configurationService = configurationService;

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
                                LogLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
                                LogLevel.Warning => Microsoft.Extensions.Logging.LogLevel.Warning,
                                LogLevel.Info => Microsoft.Extensions.Logging.LogLevel.Information,
                                LogLevel.Always => Microsoft.Extensions.Logging.LogLevel.Information,
                                LogLevel.Verbose => Microsoft.Extensions.Logging.LogLevel.Debug,
                                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
                            },
                            $"MSAL: {message}");
                    }
                },
                LogLevel.Info,
#if DEBUG
                // Personally Identifiable Information
                enablePiiLogging: true,
#endif
                enableDefaultPlatformLogging: true)
            .Build();
    }

    private async Task ClearCachedCredentials(bool clearBrowserCache)
    {
        foreach (var acct in await msalClient.GetAccountsAsync())
            await msalClient.RemoveAsync(acct);

        // If you don't want the browser to appear when logging
        // out, remove this code.  The consequence is the next
        // time the client authenticates through the browser,
        // they won't have to enter their password, which is
        // a security hole.
        if (clearBrowserCache)
        {
            try
            {
                // Open system browser to Microsoft's universal logout page
                var psi = new ProcessStartInfo
                {
                    FileName = "https://login.microsoftonline.com/common/oauth2/v2.0/logout",
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception)
            {
                // Eat, could be caused by lack of internet
                // connection, but not too important anyway.
            }
        }
    }

    public LoginProvider LoginProvider => LoginProvider.Microsoft;
    
    public async Task<(AuthenticationResult? authenticationResult,
        string? internalUserId)> LoginAsync(CancellationToken ct)
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

        var internalUserId = result == null ? null : $"M-{result.Account.HomeAccountId.Identifier}";
        return (result, internalUserId);
    }

    public void Logout(bool clearBrowserCache)
    {
        LogoutAsync(clearBrowserCache)
            .LogExceptionsAndForget(
                $"{nameof(MicrosoftLoginProvider)}.{nameof(Logout)}()",
                logger);
    }

    //  Clears existing login data, plus any provider-specific cleanup.
    public async Task LogoutAsync(bool clearBrowserCache)
    {
        await ClearCachedCredentials(clearBrowserCache);

        try
        {
            logger.LogInformation($"User logged out: {nameof(MicrosoftLoginProvider)}");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to log out: {EMessage}", e.Message);
        }
    }
}