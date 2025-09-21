using Biz.Shell.ClientLoginProviders;
using Biz.Shell.Services.Config;
using Microsoft.Identity.Client;

namespace Biz.Shell.Services.Authentication;

public abstract class MicrosoftLoginProviderBase : IClientLoginProvider
{
    protected readonly IConfigurationService ConfigurationService;
    protected IPublicClientApplication? MsalClient;
    protected readonly ILogger<MicrosoftLoginProviderBase> Logger;

    public LoginProvider LoginProvider => LoginProvider.Microsoft;
    
    protected MicrosoftLoginProviderBase(ILogger<MicrosoftLoginProviderBase> logger,
        IConfigurationService configurationService)
    {
        this.Logger = logger;
        this.ConfigurationService = configurationService;

        // ReSharper disable once VirtualMemberCallInConstructor
        CreateMsalClient();
    }

    /// <summary>
    /// Set MsalClient here.
    /// </summary>
    protected abstract void CreateMsalClient();

    private async Task ClearCachedCredentials(bool clearBrowserCache)
    {
        foreach (var acct in await MsalClient!.GetAccountsAsync())
            await MsalClient.RemoveAsync(acct);

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

    public abstract Task<(AuthenticationResult? authenticationResult,
        string? internalUserId)> LoginAsync(CancellationToken ct);

    //  Clears existing login data, plus any provider-specific cleanup.
    public async Task LogoutAsync(bool clearBrowserCache)
    {
        await ClearCachedCredentials(clearBrowserCache);

        try
        {
            Logger.LogInformation($"User logged out: {nameof(MicrosoftLoginProviderBase)}");
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to log out: {EMessage}", e.Message);
        }
    }
}