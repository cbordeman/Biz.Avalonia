using Biz.Models;

namespace Biz.Shell.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<bool> IsAuthenticatedAsync();
        Task<(bool isLoggedIn, Tenant[]? availableTenants, string? error)> LoginWithGoogleAsync(CancellationToken ct);
        Task<(bool isLoggedIn, Tenant[]? availableTenants, string? error)> LoginWithMicrosoftAsync(CancellationToken ct);
        Task<(bool isLoggedIn, Tenant[]? availableTenants, string? error)> LoginWithFacebookAsync(CancellationToken ct);
        Task<(bool isLoggedIn, Tenant[]? availableTenants, string? error)> LoginWithAppleAsync(CancellationToken ct);
        Task CompleteLogin(Tenant selectedTenant);
        void Logout(bool invokeEvent);
        Task LogoutAsync(bool invokeEvent);
        Task<User?> GetCurrentUserAsync();
        event EventHandler<bool> AuthenticationStateChanged;
    }
} 