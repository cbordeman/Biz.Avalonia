using System.Text.Json;
using Biz.Configuration;
using Biz.Modules.AccountManagement.Core.Services.Authentication;
using Microsoft.Maui.Authentication;
using Serilog;
using ServiceClients;
using Shouldly;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Biz.Modules.AccountManagement.Services.Authentication;

public class AuthenticationService(
    IConfigurationService configurationService,
    IAuthDataStore authDataStore,
    ITenantsApi tenantsApi,
    LoginProviderRegistry loginProviderRegistry)
    : IAuthenticationService
{
    public AsyncEvent AuthenticationStateChanged { get; } = new();

    public IClientLoginProvider? CurrentProvider { get; private set; }
    public LoginProviderDescriptor? CurrentProviderDescriptor { get; private set; }

    public async Task InitializeAsync()
    {
        if (authDataStore.Data == null)
            await authDataStore.RestoreAuthDataAsync();
        if (authDataStore.Data == null ||
            authDataStore.Data.LoginProvider == null)
            return;
        if (loginProviderRegistry.Descriptors
            .TryGetValue(authDataStore.Data.LoginProvider.Value,
                out var descriptor))
        {
            CurrentProviderDescriptor = descriptor;
            CurrentProvider = (IClientLoginProvider)
                Locator.Current.Resolve(descriptor.ProviderType);
        }
    }

    public async Task<bool> IsAuthenticated()
    {
        if (authDataStore.Data == null)
            await authDataStore.RestoreAuthDataAsync();
                
        if (authDataStore.Data == null || authDataStore.Data.LoginProvider == null)
            return false;

        if (authDataStore.Data.Tenant == null || authDataStore.Data.Tenant.TenantId < 1)
            return false;

        // If it's a Facebook token, validate it
        // if (authDataStore.Data.LoginProvider == LoginProvider.Facebook)
        //     return await ValidateFacebookTokenAsync(authDataStore.Data.AccessToken!);

        if (authDataStore.Data.ExpiresAt <= DateTimeOffset.UtcNow.AddMinutes(-5))
            return false;
        return true;
    }

    public async Task<(bool isLoggedIn, Tenant[]? availableTenants, string? error)>
        LoginWithGoogleAsync(CancellationToken ct)
    {
        try
        {
            var authData = new AuthData
            {
                AccessToken = "google_token",
                ExpiresAt = DateTimeOffset.UtcNow.AddHours(1), // Simulated expiry
                LoginProvider = LoginProvider.Google,
                //TenantId = 0, // TODO: set tenant ID if applicable
                //TenantName = "Some Tenant", // TODO: set tenant name if applicable
                Id = "Google ID",
                Name = "Google User",
                Email = "user@gmail.com",
                IsMfa = false // TODO: set MFA status if applicable
            };
            await authDataStore.SaveAuthDataAsync();
            await AuthenticationStateChanged.PublishSequentiallyAsync();
            return (true, null, null);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Log.Logger.Warning(ex, $"Google login failed: {ex.Message}");
            return (false, null, ex.Message);
        }
    }

    public async Task<(bool isLoggedIn, Tenant[]? availableTenants,
            string? error)> LoginWithProviderAsync(
        LoginProvider providerEnum,
        CancellationToken ct)
    {
        if (await IsAuthenticated())
            await LogoutAsync(false, false);

        try
        {
            CurrentProviderDescriptor =
                loginProviderRegistry.Descriptors[providerEnum];
            CurrentProvider = (IClientLoginProvider)
                Locator.Current.Resolve(
                    CurrentProviderDescriptor.ProviderType);
        }
        catch (Exception e)
        {
            Log.Logger.Error(e,
                "No registered login provider for {LoginProvider}: " +
                "{EMessage}",
                providerEnum,
                e.Message);
            throw;
        }

        try
        {
            authDataStore.RemoveAuthData();
            await CurrentProvider.LogoutAsync(false);
        }
        catch (Exception e)
        {
            Log.Logger.Error(e, $"ERROR: Failed to get available tenants: {e.Message}");
            return (false, null, e.Message);
        }

        var (result, internalUserId) =
            await CurrentProvider.LoginAsync(ct);

        try
        {
            if (result != null)
            {
                internalUserId.ShouldNotBeNullOrEmpty();

                //var allclaims = string.Join(", ", result.ClaimsPrincipal.Claims.Select(c => c.Type));
                var isMfa = result.ClaimsPrincipal.Claims.Any(c =>
                    c.Type == "mfr" && c.Value.Split().Contains("mfa"));
                return await SaveAuthenticationResultAndGetTenants(
                    internalUserId,
                    result.AccessToken,
                    result.ExpiresOn,
                    result.Account.Username,
                    result.Account.Username,
                    isMfa);
            }

            Log.Logger.Error("Result was NULL.");
            return (false, null, "Login failed");
        }
        catch (Exception e)
        {
            Log.Logger.Error(e, $"ERROR: Failed to get available tenants: {e.Message}");
            return (false, null, e.Message);
        }

    }

    // Called by LoginWithXXXAsync methods.
    async Task<(bool isLoggedIn, Tenant[]? availableTenants, string? error)>
        SaveAuthenticationResultAndGetTenants(
        string internalUserId, string accessToken,
        DateTimeOffset expiresOn,
        string name, string email, bool isMfa)
    {
        CurrentProvider.ShouldNotBeNull();

        authDataStore.RemoveAuthData();

        authDataStore.Data.ShouldNotBeNull("authDataStore.Data was null");
        authDataStore.Data.Id = internalUserId;
        authDataStore.Data.AccessToken = accessToken;
        authDataStore.Data.ExpiresAt = expiresOn;
        authDataStore.Data.LoginProvider = CurrentProvider.LoginProvider;
        authDataStore.Data.Name = name;
        authDataStore.Data.Email = email;
        authDataStore.Data.IsMfa = isMfa;
        // Note that Tenant is not set here; it will be set later.
        authDataStore.Data.Tenant = null;

        await authDataStore.SaveAuthDataAsync();

        // Fetch available tenants for the user.
        var availableTenants = await tenantsApi.GetMyAvailable();
        switch (availableTenants.Length)
        {
            case 0:
                Log.Logger.Warning("No available tenants found for user {UserId}", authDataStore.Data.Id);
                // No tenants available, no login.
                return (false, null, "No tenants available");
            case 1:
                // Just one tenant available, complete login immediately.
                await CompleteLogin(availableTenants[0]);
                return (true, [availableTenants[0]], null);
            default:
                // Caller must show a tenant selection UI then call
                return (false, availableTenants, null);
        }
    }

    public async Task CompleteLogin(Tenant selectedTenant)
    {
        authDataStore.Data.ShouldNotBeNull("authDataStore.Data was null");
        authDataStore.Data.Tenant = selectedTenant;
        await authDataStore.SaveAuthDataAsync();

        await AuthenticationStateChanged.PublishSequentiallyAsync();
    }

    public async Task<(bool isLoggedIn, Tenant[]? availableTenants, string? error)>
        LoginWithFacebookAsync(CancellationToken ct)
    {
        try
        {
            var facebookConfig = configurationService.Authentication.Facebook;
            var authUrl = GenerateFacebookAuthUrl(facebookConfig);
            var authCode = await LaunchFacebookAuthAsync(authUrl);
            if (string.IsNullOrEmpty(authCode))
            {
                Log.Logger.Information("Facebook authentication was cancelled by user");
                return (false, null, null);
            }
            var accessToken = await ExchangeCodeForTokenAsync(authCode, facebookConfig);
            if (string.IsNullOrEmpty(accessToken))
            {
                Log.Logger.Warning("Failed to obtain Facebook access token");
                return (false, null, "Failed to obtain token.");
            }
            var facebookUserProfile = await GetFacebookUserProfileAsync(accessToken);
            if (facebookUserProfile == null)
            {
                Log.Logger.Error("Failed to retrieve Facebook user profile");
                return (false, null, "Failed to retrieve user profile.");
            }

            var name = facebookUserProfile.Name;
            if (string.IsNullOrWhiteSpace(facebookUserProfile.Name))
                name = facebookUserProfile.FirstName.Trim() + ' ' + facebookUserProfile.LastName.Trim();
            var authData = new AuthData
            {
                AccessToken = accessToken,
                // No ExpiresAt in Facebook login, use token validation instead
                LoginProvider = LoginProvider.Facebook,
                //TenantId = 0, // TODO: set tenant ID if applicable
                //TenantName = "Some Tenant", // TODO: set tenant name if applicable
                Id = facebookUserProfile.Id,
                Name = name,
                Email = facebookUserProfile.Email,
                ExpiresAt = null,
                IsMfa = false // TODO: set MFA status if applicable
            };

            await authDataStore.SaveAuthDataAsync();
            await AuthenticationStateChanged.PublishSequentiallyAsync();
            return (true, null, null);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Log.Logger.Warning(ex, $"Facebook login failed: {ex.Message}");
            return (false, null, ex.Message);
        }
    }

    public async Task<(bool isLoggedIn, Tenant[]? availableTenants, string? error)>
        LoginWithAppleAsync(CancellationToken ct)
    {
        try
        {
            // mock implementation for Apple login
            var authData = new AuthData
            {
                AccessToken = "apple_token",
                ExpiresAt = null, // not sure if Apple provides expiry
                LoginProvider = LoginProvider.Apple,
                //TenantId = 0, // TODO: set tenant ID if applicable
                //TenantName = "Some Tenant", // TODO: set tenant name if applicable
                Id = "Apple ID",
                Name = "Apple User",
                Email = "user@apple.com",
                IsMfa = false // TODO: set MFA status if applicable
            };

            await authDataStore.SaveAuthDataAsync();
            await AuthenticationStateChanged.PublishSequentiallyAsync();
            return (true, null, null);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Log.Logger.Warning(ex, $"Apple login failed: {ex.Message}");
            return (false, null, ex.Message);
        }
    }

    public async Task LogoutAsync(bool invokeEvent, bool clearBrowserCache)
    {
        if (CurrentProvider != null)
            await CurrentProvider.LogoutAsync(clearBrowserCache);

        authDataStore.Data = null;
        await authDataStore.SaveAuthDataAsync();

        CurrentProviderDescriptor = null;
        CurrentProvider = null;

        if (invokeEvent)
            await AuthenticationStateChanged.PublishSequentiallyAsync();
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        if (!await IsAuthenticated())
            return null;

        if (authDataStore.Data == null)
            await authDataStore.RestoreAuthDataAsync();
        if (authDataStore.Data != null)
        {
            authDataStore.Data.Id.ShouldNotBeNullOrEmpty();
            authDataStore.Data.Name.ShouldNotBeNullOrEmpty();
            authDataStore.Data.Email.ShouldNotBeNullOrEmpty();
            authDataStore.Data.LoginProvider.ShouldNotBeNull();
            authDataStore.Data.Tenant.ShouldNotBeNull();

            var user = new User(
                authDataStore.Data.Id,
                authDataStore.Data.Name,
                authDataStore.Data.Email,
                true,
                authDataStore.Data.LoginProvider,
                authDataStore.Data.Tenant);

            // TODO: Make call the accounts service to get the user information

            return user;
        }

        return null;
    }

    string GenerateFacebookAuthUrl(FacebookAuth facebookConfig)
    {
        var scopes = string.Join(",", facebookConfig.Scopes);
        return $"https://www.facebook.com/v18.0/dialog/oauth?" +
               $"client_id={facebookConfig.ClientId}&" +
               $"redirect_uri={Uri.EscapeDataString(facebookConfig.RedirectUri)}&" +
               $"scope={Uri.EscapeDataString(scopes)}&" +
               $"response_type=code&" +
               $"state={Guid.NewGuid()}";
    }

    async Task<string?> LaunchFacebookAuthAsync(string authUrl)
    {
        try
        {
            var facebookConfig = configurationService.Authentication.Facebook;

            // Use MAUI's WebAuthenticator for OAuth flow
            var callbackUrl = new Uri(facebookConfig.RedirectUri);

            var authResult = await WebAuthenticator.Default.AuthenticateAsync(
                new Uri(authUrl),
                callbackUrl);

            // Extract the authorization code from the callback URL
            if (authResult.Properties.TryGetValue("code", out string? authCode))
                return authCode;

            Log.Logger.Error("No authorization code found in callback");
            return null;
        }
        catch (TaskCanceledException)
        {
            Log.Logger.Information("Facebook authentication was cancelled by user");
            return null;
        }
        catch (Exception ex)
        {
            Log.Logger.Warning(ex, $"Failed to launch Facebook auth: {ex.Message}");
            return null;
        }
    }

    async Task<string?> ExchangeCodeForTokenAsync(string authCode, FacebookAuth facebookConfig)
    {
        try
        {
            var tokenUrl = "https://graph.facebook.com/v18.0/oauth/access_token";

            using var client = new HttpClient();
            var content = new FormUrlEncodedContent([
                new KeyValuePair<string, string>("client_id", facebookConfig.ClientId),
                new KeyValuePair<string, string>("redirect_uri", facebookConfig.RedirectUri),
                new KeyValuePair<string, string>("code", authCode),
                new KeyValuePair<string, string>("grant_type", "authorization_code")
            ]);

            var response = await client.PostAsync(tokenUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<FacebookTokenResponse>(responseContent);
                return tokenResponse?.AccessToken;
            }
            else
            {
                Log.Logger.Error($"Facebook token exchange failed: Status code {response.StatusCode}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, $"Failed to exchange code for token: {ex.Message}");
            return null;
        }
    }

    async Task<FacebookUserProfile?> GetFacebookUserProfileAsync(string accessToken)
    {
        try
        {
            using var client = new HttpClient();

            // Get user profile from Facebook Graph API
            var profileUrl = $"https://graph.facebook.com/v18.0/me?fields=id,name,email,first_name,last_name&access_token={accessToken}";

            var response = await client.GetAsync(profileUrl);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var userProfile = JsonSerializer.Deserialize<FacebookUserProfile>(responseContent);
                return userProfile;
            }
            else
            {
                Log.Logger.Error($"Failed to get Facebook user profile: {response.StatusCode}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, $"Failed to get Facebook user profile: {ex.Message}");
            return null;
        }
    }

    async Task<bool> ValidateFacebookTokenAsync(string accessToken)
    {
        try
        {
            using var client = new HttpClient();

            // Check if the token is still valid by making a request to Facebook's debug endpoint
            var debugUrl = $"https://graph.facebook.com/v18.0/debug_token?input_token={accessToken}&access_token={accessToken}";

            var response = await client.GetAsync(debugUrl);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var debugResponse = JsonSerializer.Deserialize<FacebookDebugResponse>(responseContent);

                // Check if the token is valid and not expired
                return debugResponse?.Data.IsValid == true &&
                       debugResponse.Data.ExpiresAt > DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }

            return false;
        }
        catch (Exception ex)
        {
            Log.Logger.Warning(ex, $"Failed to validate Facebook token: {ex.Message}");
            return false;
        }
    }

    class FacebookUserProfile
    {
        public string Id { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
    }

    class FacebookTokenResponse
    {
        public string AccessToken { get; init; } = string.Empty;
        public string TokenType { get; init; } = string.Empty;
        public int ExpiresIn { get; init; }
    }

    class FacebookDebugResponse
    {
        public FacebookDebugData Data { get; init; } = new();
    }

    class FacebookDebugData
    {
        public bool IsValid { get; set; }
        public long ExpiresAt { get; set; }
    }

    public class SerializableClaim
    {
        public SerializableClaim() { }
        public SerializableClaim(string type, string value)
        {
            Type = type;
            Value = value;
        }
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
