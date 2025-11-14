using System.Security.Claims;
using Biz.Core;
using Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Services.Controllers;
using Shouldly;
using Tenant = Biz.Models.Tenant;

namespace Services.Auth;

public static partial class Auth
{
    static async Task OnTokenValidated(TokenValidatedContext context)
    {
        // Get requested provider from HTTP header.  Guaranteed to be correct
        // since we already authenticated using it (we selected the auth scheme).
        var lph = context.Request.Headers[nameof(LoginProvider)].FirstOrDefault()!;
        var loginProvider = (LoginProvider)Enum.Parse(
            typeof(LoginProvider), lph);

        // Add a new identity with new claim for the request's login provider.
        context.Principal!.AddIdentity(
            new ClaimsIdentity([new(Claims.LoginProvider, lph)]));
        
        var internalUserId = context.Principal!.GetInternalId(loginProvider);
        AppUser? user = null;
        var db = context.HttpContext.RequestServices
            // ReSharper disable once MethodHasAsyncOverload
            .GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext();
        if (!string.IsNullOrEmpty(internalUserId))
        {
            user = await db.AppUsers
                // Only get active TenantUsers in active Tenants
                // User might not be active in any tenant, but we still want to know if
                // the user exists.
                .Include(x => x.TenantUsers!.Where(tu => tu.IsActive && tu.Tenant.IsActive))
                .ThenInclude(tu => tu.Tenant)
                .FirstOrDefaultAsync(x => x.Id == internalUserId);
        }
        
        if (user == null)
        {
            // For local accounts, the user has to have existed or the token couldn't
            // be validated.  Let's assert this to be certain.
            loginProvider.ShouldNotBe(LoginProvider.Local);
            
            // User does not exist at all, create a new one in default tenant.

            var name = context.Principal?.FindFirst("name")?.Value ?? string.Empty;
            string? email;
            switch (loginProvider)
            {
                case LoginProvider.Microsoft:
                    email = context.Principal?.FindFirst("preferred_username")?.Value ?? string.Empty;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            user = new AppUser(loginProvider, internalUserId, name, email);
            db.AppUsers.Add(user);

            // Assign new user to default tenant
            var defaultTenant = await db.Tenants.FirstOrDefaultAsync(t => t.IsDefault);
            if (defaultTenant == null)
            {
                context.Fail("Database misconfigured, no default tenant.");
                return;
            }

            db.TenantUsers.Add(new TenantUser
            {
                TenantId = defaultTenant.Id,
                AppUserId = user.Id,
                IsActive = true
            });

            try
            {
                //var changes = db.ChangeTracker.ToDebugString();
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<object>>();
                Log.Logger.Error(e, "Failed saving new user to database.");
                throw;
            }

            context.Fail("Must provide a TenantId header.  Check body for active and available tenants.");
            return;
        }

        // User exists.

        if (loginProvider == LoginProvider.Local)
        {
            // For local, enforce email confirmation.
            user.ShouldNotBeNull();
            if (!user.EmailConfirmed)
            {
                context.Fail("Email not confirmed.");
                return;
            }
        }
       
        // Get requested internal tenant ID from HTTP header and put it into Claims in
        //  TenantUser.  ClaimsPrincipal.GetInternalTenantId() can be used in
        // Controllers to retrieve the value, and it is guaranteed to belong to the user.
        // If the header is NOT present, we will authenticate successfully so that
        // Controllers that don't require a TenantId can still work.
        var tenantIdHeaderValue = context.Request.Headers["TenantId"].FirstOrDefault();
        if (!string.IsNullOrEmpty(tenantIdHeaderValue))
        {
            if (!Enum.TryParse(tenantIdHeaderValue, out int requestedInternalTenantId) ||
                requestedInternalTenantId < 1)
            {
                context.Fail(
                    $"TenantId header value \"{tenantIdHeaderValue}\" " +
                    $"is not a valid integer.");
                return;
            }

            // If requested internal tenant exists for user and user is active in it, succeed.
            user.TenantUsers.ShouldNotBeNull();
            foreach (var tu in user.TenantUsers)
                if (tu.TenantId == requestedInternalTenantId && tu.IsActive)
                {
                    // Success
                    await context.Response.WriteAsJsonAsync(
                        new User(user.Id, user.Name, user.Email!,
                            true, user.LoginProvider,
                            new Tenant(
                                tu.Tenant.Id, tu.Tenant.Name)));

                    // Add a new identity with new claim for our
                    // internal TenantId.
                    var claims = new List<Claim>
                    {
                        new(Claims.InternalTenantId, 
                            requestedInternalTenantId.ToString())
                    };
                    var appIdentity = new ClaimsIdentity(claims);
                    context.Principal!.AddIdentity(appIdentity);

                    return;
                }

            // User is not active in the requested tenant.
            context.Fail(
                $"User is not active in requested internal tenantId " +
                $"\"{requestedInternalTenantId},\" or the tenant is inactive.  " +
                "Check body for active and available tenants.");
        }
    }
}