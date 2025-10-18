using Biz.Shell.ClientLoginProviders;

namespace Biz.Shell.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task InitializeAsync();
        bool IsAuthenticated { get; }
        Task<(bool isLoggedIn, Tenant[]? availableTenants, string? error)> 
            LoginWithProviderAsync(
            LoginProvider providerEnum, CancellationToken ct);
        Task CompleteLogin(Tenant selectedTenant);
        Task LogoutAsync(bool invokeEvent, bool clearBrowserCache);
        Task<User?> GetCurrentUserAsync();
        AsyncEvent AuthenticationStateChanged { get; }
        IClientLoginProvider? CurrentProvider { get; }
        LoginProviderDescriptor? CurrentProviderDescriptor { get; }
    }
}