using Microsoft.Identity.Client;

namespace Biz.Shell.Infrastructure;

public interface IPlatformMsalService
{
    Task<AuthenticationResult?> LoginUsingMsal(CancellationToken ct);
    Task ClearCache();
}
