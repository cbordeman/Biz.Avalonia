using Biz.Shell.ClientLoginProviders;

namespace Biz.Shell.Services.Authentication
{
    // Like EventHandler, but no unnecessary sender or payload.
    public delegate void ChangeHandler();
    
    public interface IAuthenticationService
    {
        Task InitializeAsync();
        bool IsAuthenticated { get; }
        Task<(bool isLoggedIn, Tenant[]? availableTenants, string? error)> 
            LoginWithProviderAsync(
            LoginProvider providerEnum, CancellationToken ct);
        Task CompleteLogin(Tenant selectedTenant);
        void Logout(bool invokeEvent, bool clearBrowserCache);
        Task LogoutAsync(bool invokeEvent, bool clearBrowserCache);
        Task<User?> GetCurrentUserAsync();
        AsyncEvent AuthenticationStateChanged { get; }
        IClientLoginProvider? CurrentProvider { get; }
        LoginProviderDescriptor? CurrentProviderDescriptor { get; }
    }
}