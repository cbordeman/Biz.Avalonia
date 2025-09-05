using Microsoft.Identity.Client;

namespace Biz.Shell.ClientLoginProviders;

public interface IClientLoginProvider
{
    Task<AuthenticationResult?> LoginAsync(CancellationToken ct);
    Task ClearCache(bool clearBrowserCache);
    void Logout(bool clearBrowserCache);
    Task LogoutAsync(bool clearBrowserCache);
}
