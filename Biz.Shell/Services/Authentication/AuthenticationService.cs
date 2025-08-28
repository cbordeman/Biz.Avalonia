using System.Net.Http;
using System.Text.Json;
using Biz.Models;
using Biz.Shell.Services.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Authentication;
using ServiceClients;
using Shouldly;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Biz.Shell.Services.Authentication;

[UsedImplicitly]
public class AuthenticationService : IAuthenticationService
{
    readonly IConfigurationService configurationService;
    readonly IAuthDataStore authDataStore;
    readonly IPlatformMsalService platformMsalService;
    readonly ITenantsApi tenantsApi;
    readonly ILogger<AuthenticationService> logger;
    readonly IRegionManager regionManager;

    public event ChangeHandler? AuthenticationStateChanged;

    public AuthenticationService(IConfigurationService configurationService,
        IAuthDataStore authDataStore, IPlatformMsalService platformMsalService,
        ITenantsApi tenantsApi, ILogger<AuthenticationService> logger,
        IRegionManager regionManager)
    {
        this.configurationService = configurationService;
        this.authDataStore = authDataStore;
        this.platformMsalService = platformMsalService;
        this.tenantsApi = tenantsApi;
        this.logger = logger;
        this.regionManager = regionManager;

        //bool useSystemBrowser = App.Current.IsSystemWebViewAvailable();
        
//         // Initialize MSAL client for Microsoft authentication
//         msalClient = PublicClientApplicationBuilder
//             .Create(this.configurationService.Authentication.Microsoft.ClientId)
//             .WithTenantId(this.configurationService.Authentication.Microsoft.TenantId)
//             
// #if ANDROID 
//             // Android only supports redirect URIs with custom schemes
//             .WithRedirectUri(this.configurationService.Authentication.Microsoft.MobileRedirectUri)
// #elif IOS
//             // ReSharper disable once StringLiteralTypo
//             .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
//             // iOS only supports redirect URIs with custom schemes
//             .WithRedirectUri(this.configurationService.Authentication.Microsoft.MobileRedirectUri)
// #else // Windows, Mac
//             // Desktop only supports loopback URIs
//             .WithRedirectUri(this.configurationService.Authentication.Microsoft.DesktopRedirectUri)
// #endif
//
//             .WithLogging(
//                 (level, message, _) =>
//                 {
//                     logger.Log(
//                         level switch
//                         {
//                             LogLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
//                             LogLevel.Warning => Microsoft.Extensions.Logging.LogLevel.Warning,
//                             LogLevel.Info => Microsoft.Extensions.Logging.LogLevel.Information,
//                             LogLevel.Always => Microsoft.Extensions.Logging.LogLevel.Information,
//                             LogLevel.Verbose => Microsoft.Extensions.Logging.LogLevel.Debug,
//                             _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
//                         },
//                         message);
// // #if ANDROID
// //                     Log.Debug("MSAL-VERBOSE", $"[{level}] {message}");
// // #endif
//                 },
//                 LogLevel.Info,
// #if DEBUG
//                 enablePiiLogging: true,  // Personally Identifiable Information 
// #endif
//                 enableDefaultPlatformLogging: true)
//             .Build();
    }

    public bool IsAuthenticated
    {
        get
        {
            if (authDataStore.Data == null)
                authDataStore.RestoreAuthDataAsync().LogExceptionsAndForget(
                    "Restoring auth data",
                    logger);
            if (authDataStore.Data == null)
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
            AuthenticationStateChanged?.Invoke();
            return (true, null, null);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, $"Google login failed: {ex.Message}");
            return (false, null, ex.Message);
        }
    }

    public async Task<(bool isLoggedIn, Tenant[]? availableTenants, string? error)> 
        LoginWithMicrosoftAsync(CancellationToken ct)
    {
        try
        {
            await LogoutAsync(false);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"ERROR: Failed to get available tenants: {e.Message}");
            return (false, null, e.Message);
        }
        
        var result = await platformMsalService.LoginUsingMsal(ct);
        
        try
        {
            if (result != null)
            {
                var userId = result.Account.HomeAccountId.Identifier
                    .GetInternalId(LoginProvider.Microsoft);
                //var allclaims = string.Join(", ", result.ClaimsPrincipal.Claims.Select(c => c.Type));
                var isMfa = result.ClaimsPrincipal.Claims.Any(c =>
                    c.Type == "mfr" && c.Value.Split().Contains("mfa"));
                return await SaveAuthenticationResultAndGetTenants(LoginProvider.Microsoft, userId, 
                    result.AccessToken, result.ExpiresOn,
                    result.Account.Username, result.Account.Username,
                    isMfa);
            }

            logger.LogDebug("ERROR: result was NULL.");
            return (false, null, "Login failed");
        }
        catch (Exception e)
        {
            logger.LogError(e, $"ERROR: Failed to get available tenants: {e.Message}");
            return (false, null, e.Message);
        }

    }

    // Called by LoginWithXXXAsync methods.
    async Task<(bool isLoggedIn, Tenant[]? availableTenants, string? error)>
        SaveAuthenticationResultAndGetTenants(
        LoginProvider loginProvider, 
        string userId, string accessToken, 
        DateTimeOffset expiresOn,
        string name, string email, bool isMfa)
    {
        authDataStore.RemoveAuthData();
        
        authDataStore.Data.ShouldNotBeNull("authDataStore.Data was null");
        authDataStore.Data.Id = userId;
        authDataStore.Data.AccessToken = accessToken;
        authDataStore.Data.ExpiresAt = expiresOn;
        authDataStore.Data.LoginProvider = loginProvider;
        authDataStore.Data.Name = name;
        authDataStore.Data.Email = email;
        authDataStore.Data.IsMfa = isMfa;
        // Note that Tenant is not set here; it will be set later.

        await authDataStore.SaveAuthDataAsync();

        // Fetch available tenants for the user.
        var availableTenants = await tenantsApi.GetMyAvailable();
        switch (availableTenants.Length)
        {
            case 0:
                logger.LogWarning("No available tenants found for user {UserId}", authDataStore.Data.Id);
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

        AuthenticationStateChanged?.Invoke();
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
                logger.LogInformation("Facebook authentication was cancelled by user");
                return (false, null, null);
            }
            var accessToken = await ExchangeCodeForTokenAsync(authCode, facebookConfig);
            if (string.IsNullOrEmpty(accessToken))
            {
                logger.LogWarning("Failed to obtain Facebook access token");
                return (false, null, "Failed to obtain token.");
            }
            var facebookUserProfile = await GetFacebookUserProfileAsync(accessToken);
            if (facebookUserProfile == null)
            {
                logger.LogError("Failed to retrieve Facebook user profile");
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
            AuthenticationStateChanged?.Invoke();
            return (true, null, null);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, $"Facebook login failed: {ex.Message}");
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
            AuthenticationStateChanged?.Invoke();
            return (true, null, null);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, $"Apple login failed: {ex.Message}");
            return (false, null, ex.Message);
        }
    }

    public void Logout(bool invokeEvent)
    {
        LogoutAsync(invokeEvent).LogExceptionsAndForget(
            $"{nameof(AuthenticationService)}.{nameof(Logout)}()",
            logger);
    }

    //  Clears existing login data, plus any provider-specific cleanup.
    public async Task LogoutAsync(bool invokeEvent)
    {
        // Clear history
        var mainRegion = regionManager.Regions[RegionNames.MainContentRegion];
        mainRegion.NavigationService.Journal.Clear();
        
        // Clear MSAL
        await platformMsalService.ClearCache();

        if (authDataStore.Data != null)
        {
            try
            {
                switch (authDataStore.Data.LoginProvider)
                {
                    case LoginProvider.Google:
                        break;
                    case LoginProvider.Microsoft:
                        break;
                    case LoginProvider.Facebook:
                        break;
                    case LoginProvider.Apple:
                        break;
                    case null:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                authDataStore.RemoveAuthData();
                logger.LogInformation("User logged out.");
            }
            catch (Exception e)
            {
                logger.LogError(e, $"ERROR: Failed to log out: {e.Message}");
            }
        }

        if (invokeEvent)
            AuthenticationStateChanged?.Invoke();
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        if (!IsAuthenticated)
            return null;
        
        if (authDataStore.Data == null)
            await authDataStore.RestoreAuthDataAsync();
        if (authDataStore.Data != null)
        {
            Debug.Assert(authDataStore.Data.Id != null);
            Debug.Assert(authDataStore.Data.Name != null);
            Debug.Assert(authDataStore.Data.Email != null);
            Debug.Assert(authDataStore.Data.LoginProvider != null);
            Debug.Assert(authDataStore.Data.Tenant != null);

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

            logger.LogError("No authorization code found in callback");
            return null;
        }
        catch (TaskCanceledException)
        {
            logger.LogInformation("Facebook authentication was cancelled by user");
            return null;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, $"Failed to launch Facebook auth: {ex.Message}");
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
                logger.LogError($"Facebook token exchange failed: Status code {response.StatusCode}");
                return null;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to exchange code for token: {ex.Message}");
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
                logger.LogError($"Failed to get Facebook user profile: {response.StatusCode}");
                return null;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to get Facebook user profile: {ex.Message}");
            return null;
        }
    }

    [UsedImplicitly]
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
            logger.LogWarning(ex, $"Failed to validate Facebook token: {ex.Message}");
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