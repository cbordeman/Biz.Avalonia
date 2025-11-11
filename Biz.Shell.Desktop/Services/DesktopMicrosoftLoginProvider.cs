using Biz.Authentication;
using Biz.Authentication.ClientLoginProvider;
using Biz.Configuration;
using Microsoft.Identity.Client;
using Serilog.Events;
using LogLevel = Microsoft.Identity.Client.LogLevel;

namespace Biz.Shell.Desktop.Services;

public class DesktopMicrosoftLoginProvider : MicrosoftLoginProviderBase
{
    public DesktopMicrosoftLoginProvider(
        IConfigurationService configurationService)
        : base(configurationService) { }

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
                        Log.Logger.Write(
                            level switch
                            {
                                LogLevel.Error => LogEventLevel.Error,
                                LogLevel.Warning => LogEventLevel.Warning,
                                LogLevel.Info => LogEventLevel.Information,
                                LogLevel.Always => LogEventLevel.Information,
                                LogLevel.Verbose => LogEventLevel.Verbose,
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
            Log.Logger.Error(ex, "ERROR: Microsoft login failed: {Name} {ExMessage}", ex.GetType().Name, ex.Message);
        }

        var internalUserId = result == null ? null : $"M-{result.Account.HomeAccountId.Identifier}";
        return (result, internalUserId);
    }
}