using Biz.Authentication.ClientLoginProvider;
using Core;
using Biz.Models;
using CompositeFramework.Core;
using System.Threading.Tasks;
using System.Threading;

namespace Biz.Authentication
{
    public interface IAuthenticationService
    {
        Task InitializeAsync();
        
        Task<(bool isLoggedIn, Tenant[]? availableTenants, string? error)>
            LoginWithProviderAsync(
            LoginProvider providerEnum, CancellationToken ct);
        Task CompleteLogin(Tenant selectedTenant);
        Task LogoutAsync(bool invokeEvent, bool clearBrowserCache);
        User? CurrentUser { get; }
        bool IsLoggedIn { get; }
        
        AsyncEvent AuthenticationStateChanged { get; }
        IClientLoginProvider? CurrentProvider { get; }
        LoginProviderDescriptor? CurrentProviderDescriptor { get; }
    }
}
