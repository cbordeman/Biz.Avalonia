using System.Threading;
using Biz.Models;

namespace Biz.Shell.Services
{
    public interface IAuthenticationService
    {
        Task<bool> IsAuthenticatedAsync();
        Task<(bool isLoggedIn, Tenant[]? availableTenants)> 
            LoginWithGoogleAsync(CancellationToken ct);
        Task<(bool isLoggedIn, Tenant[]? availableTenants)> 
            LoginWithMicrosoftAsync(CancellationToken ct);
        Task<(bool isLoggedIn, Tenant[]? availableTenants)> 
            LoginWithFacebookAsync(CancellationToken ct);
        Task<(bool isLoggedIn, Tenant[]? availableTenants)> 
            LoginWithAppleAsync(CancellationToken ct);
        Task CompleteLogin(Tenant selectedTenant);
        Task Logout();
        Task<User?> GetCurrentUserAsync();
        event EventHandler<bool> AuthenticationStateChanged;
    }
} 