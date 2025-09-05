using Biz.Models;
using Biz.Shell.ClientLoginProviders;

namespace Biz.Shell.Services.Authentication
{
    // Like EventHandler, but no unnecessary sender or payload.
    public delegate void ChangeHandler();
    
    public interface IAuthenticationService
    {
        bool IsAuthenticated { get; }
        Task<(bool isLoggedIn, Tenant[]? availableTenants, string? error)> 
            LoginWithProviderAsync(
            LoginProvider providerEnum, CancellationToken ct);
        Task CompleteLogin(Tenant selectedTenant);
        void Logout(bool invokeEvent, bool clearBrowserCache);
        Task LogoutAsync(bool invokeEvent, bool clearBrowserCache);
        Task<User?> GetCurrentUserAsync();
        event ChangeHandler AuthenticationStateChanged;
        IClientLoginProvider? CurrentProvider { get; }
        LoginProviderDescriptor? CurrentProviderDescriptor { get; }
    }
}