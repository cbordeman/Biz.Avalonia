using Microsoft.Identity.Client;

namespace Biz.Shell.Platform;

public interface IPlatformMsalService
{
    Task<AuthenticationResult?> LoginUsingMsal(CancellationToken ct);
    Task ClearCache();
}