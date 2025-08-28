using Biz.Models;

namespace Biz.Shell.Services.Authentication
{
    // Like EventHandler, but no unnecessary sender or payload.
    public delegate void ChangeHandler();
    
    public interface IAuthenticationService
    {
        bool IsAuthenticated { get; }
        Task<(bool isLoggedIn, Tenant[]? availableTenants, string? error)> LoginWithGoogleAsync(CancellationToken ct);
        Task<(bool isLoggedIn, Tenant[]? availableTenants, string? error)> LoginWithMicrosoftAsync(CancellationToken ct);
        Task<(bool isLoggedIn, Tenant[]? availableTenants, string? error)> LoginWithFacebookAsync(CancellationToken ct);
        Task<(bool isLoggedIn, Tenant[]? availableTenants, string? error)> LoginWithAppleAsync(CancellationToken ct);
        Task CompleteLogin(Tenant selectedTenant);
        void Logout(bool invokeEvent);
        Task LogoutAsync(bool invokeEvent);
        Task<User?> GetCurrentUserAsync();
        event ChangeHandler AuthenticationStateChanged;
    }
}