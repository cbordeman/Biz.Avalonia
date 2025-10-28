using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biz.Shared.Services.Authentication;
using Biz.Shared.Services.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using LogLevel = Microsoft.Identity.Client.LogLevel;

namespace Biz.Shell.Desktop.Services;

public class DesktopMicrosoftLoginProvider : MicrosoftLoginProviderBase
{
    public DesktopMicrosoftLoginProvider(ILogger<DesktopMicrosoftLoginProvider> logger,
        IConfigurationService configurationService)
        : base(logger, configurationService) { }

    protected override void CreateMsalClient()
    {
        // Initialize MSAL client for Microsoft authentication
        MsalClient = PublicClientApplicationBuilder
            .Create(this.ConfigurationService.Authentication.Microsoft.ClientId)
            .WithTenantId(this.ConfigurationService.Authentication.Microsoft.TenantId)

            // Potentially different per platform
            .WithRedirectUri(this.ConfigurationService.Authentication.Microsoft.DesktopRedirectUri)
            .WithLogging(
                (level, message, _) =>
                {
                    if (message != null)
                    {
                        Logger.Log(
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

    public override async Task<(AuthenticationResult? authenticationResult,
        string? internalUserId)> LoginAsync(CancellationToken ct)
    {
        AuthenticationResult? result = null;
        try
        {
            var accounts = await MsalClient!.GetAccountsAsync();
            string[] scopes = ConfigurationService.Authentication.Microsoft.Scopes;
            var builder = MsalClient.AcquireTokenInteractive(scopes);
            try
            {
                result = await MsalClient
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
            Logger.LogError(ex, "ERROR: Microsoft login failed: {Name} {ExMessage}", ex.GetType().Name, ex.Message);
        }

        var internalUserId = result == null ? null : $"M-{result.Account.HomeAccountId.Identifier}";
        return (result, internalUserId);
    }
}