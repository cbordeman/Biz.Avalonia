using Biz.Models;
using CompositeFramework.Core;

namespace Biz.Modules.AccountManagement.Core.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task InitializeAsync();
        Task<bool> IsAuthenticated();
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
