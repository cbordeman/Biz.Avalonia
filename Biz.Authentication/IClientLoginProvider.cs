using Biz.Models;
using Microsoft.Identity.Client;

namespace Biz.Authentication;

/// <summary>
/// Provides client side code for a single login
/// provider like Google, Apple, Facebook, or Local accounts.
/// </summary>
public interface IClientLoginProvider
{
    LoginProvider LoginProvider { get; }
    Task<(AuthenticationResult? authenticationResult, string? internalUserId)> 
        LoginAsync(CancellationToken ct);
    Task LogoutAsync(bool clearBrowserCache);
}
