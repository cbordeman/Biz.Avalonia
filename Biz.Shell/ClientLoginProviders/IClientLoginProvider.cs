using Microsoft.Identity.Client;

namespace Biz.Shell.ClientLoginProviders;

public interface IClientLoginProvider
{
    LoginProvider LoginProvider { get; }
    Task<(AuthenticationResult? authenticationResult, 
            string? internalUserId)> 
        LoginAsync(CancellationToken ct);
    //Task ClearCachedCredentials(bool clearBrowserCache);
    void Logout(bool clearBrowserCache);
    Task LogoutAsync(bool clearBrowserCache);
}
