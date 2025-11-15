using System.Security.Claims;
using Core;
using Services.Controllers;

namespace Services.Infrastructure;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the login provider from claims.
    /// </summary>
    /// <param name="cp"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static LoginProvider GetLoginProvider(this ClaimsPrincipal cp)
    {
        var loginProviderString = cp.FindFirst(Claims.LoginProvider)?.Value; // Gets the login provider
        if (loginProviderString == null)
            throw new UnauthorizedAccessException();
        if (Enum.TryParse<LoginProvider>(loginProviderString, out var parsedProvider))
            return parsedProvider;
        throw new UnauthorizedAccessException();
    }

    public static string GetInternalId(this ClaimsPrincipal cp, LoginProvider loginProvider)
    {
        switch (loginProvider)
        {
            case LoginProvider.Microsoft:
                var objectId = cp.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
                // Social tenantId, not our internal tenantId.  We combine it with
                // objectId to get a value equivalent to the
                // MSAL result.Account.HomeAccountId.Identifier (on the client side).
                var tenantId =
                    cp.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value; // the MS social value
                if (objectId == null || tenantId == null)
                    throw new UnauthorizedAccessException("objectId or tenantId are not in the Microsoft token.");
                // Add a prefix so we have a single, unique field for our AppUser key.
                var id = $"M-{objectId}.{tenantId}";
                return id;

            default:
                throw new ArgumentOutOfRangeException(nameof(loginProvider), loginProvider, null);
        }
    }

    /// <summary>
    /// If user or tenant is inactive, throws UnauthorizedAccessException.
    /// </summary>
    /// <param name="cp"></param>
    /// <param name="db"></param>
    /// <returns>The TenantUser, with User and Tenant hydrated.
    /// Only the one selected Tenant from claims is hydrated.</returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public static Data.Models.TenantUser VerifyTenantUserIsActive(this ClaimsPrincipal cp,
        AppDbContext db)
    {
        var userId = cp.GetInternalId(cp.GetLoginProvider());
        var internalTenantId = cp.FindFirst(Claims.InternalTenantId)?.Value;
        if (internalTenantId == null)
            throw new UnauthorizedAccessException($"{Claims.InternalTenantId} HTTP header was not found.");
        if (!int.TryParse(internalTenantId, out int parsedInternalTenantId) ||
            parsedInternalTenantId < 1)
            throw new UnauthorizedAccessException(
                $"TenantId HTTP header was invalid.  Value was: {internalTenantId}.");

        // Validate that the tenant belongs to the user.  Can't
        // trust the value in the http header, it could be spoofed.
        var tu = db.TenantUsers
            .Include(tu => tu.Tenant)
            .Include(tu => tu.AppUser)
            .FirstOrDefault(tu => tu.AppUserId == userId &&
                                  tu.TenantId == parsedInternalTenantId &&
                                  tu.IsActive && tu.Tenant.IsActive);
        if (tu == null || tu.AppUser == null || tu.Tenant == null)
            throw new UnauthorizedAccessException($"User {userId} does not belong to " +
                                                  $"tenant {parsedInternalTenantId} or " +
                                                  "user or tenant is inactive.");
        // If we got here, user and tenant are active and both
        // are hydrated in the object.  Only the one selected Tenant
        // from claims is returned.
        return tu;
    }
}