using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Services.Config;

namespace Services.Auth;

public static partial class Auth
{
    /// <summary>
    /// Add JWT Bearer authentication for each login provider.
    /// Logger is resolved at runtime in event callbacks from HttpContext.RequestServices.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="externalAuthSettings"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureAuthentication(
        this IServiceCollection services,
        ExternalAuthSettings externalAuthSettings)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = "MultiAuth";
            options.DefaultChallengeScheme = "MultiAuth";
            options.DefaultForbidScheme = "MultiAuth";
        })
            .AddPolicyScheme("MultiAuth", "Multiple Auth Providers", options =>
            {
                options.ForwardDefaultSelector = context =>
                {
#if DEBUG
                    // ReSharper disable once UseDiscardAssignment
                    var unused = context.Request.Headers["Authorization"].FirstOrDefault();
#endif
                    var provider = context.Request.Headers[nameof(LoginProvider)].FirstOrDefault();
                    if (!Enum.TryParse<LoginProvider>(provider, out _))
                        return "Anonymous";

                    return provider;
                };
            })
            .AddScheme<AuthenticationSchemeOptions, AnonymousAuthenticationHandler>(
                "Anonymous", _ =>
                {
                    // use options if needed
                })
            .AddJwtBearer("Google", options =>
            {
                options.Authority = externalAuthSettings.Google.Authority;
                options.Audience = externalAuthSettings.Google.Audience;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = externalAuthSettings.Google.Authority,
                    ValidateAudience = true,
                    ValidAudience = externalAuthSettings.Google.Audience,
                    ValidateLifetime = true
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = OnTokenValidated
                };
            })
            .AddJwtBearer("Microsoft", options =>
            {
                options.Authority = externalAuthSettings.Microsoft.Authority;
                options.Audience = externalAuthSettings.Microsoft.Audience;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = externalAuthSettings.Microsoft.Authority,
                    ValidateAudience = true,
                    ValidAudience = externalAuthSettings.Microsoft.Audience,
                    ValidateLifetime = true,
                    LogValidationExceptions = true
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = OnTokenValidated,
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<object>>();
                        logger.LogWarning("Authentication failed: {Message}", context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<object>>();
                        if (context.Error != null)
                            logger.LogWarning("OnChallenge error: {Error}, {Description}", context.Error, context.ErrorDescription);
                        return Task.CompletedTask;
                    },
                    OnForbidden = _ => Task.CompletedTask,
                    // OnMessageReceived can be added here if needed
                };
            })
            .AddJwtBearer("Facebook", options =>
            {
                options.Authority = externalAuthSettings.Facebook.Authority;
                options.Audience = externalAuthSettings.Facebook.Audience;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // Facebook tokens are not always JWTs, may require custom validation
                    ValidateAudience = false,
                    ValidateLifetime = false // We'll validate lifetime manually
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = async context =>
                    {
                        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                        if (string.IsNullOrEmpty(token))
                        {
                            context.NoResult();
                            return;
                        }

                        using var client = new HttpClient();
                        var debugUrl = $"https://graph.facebook.com/v18.0/debug_token?input_token={token}&access_token={token}";
                        var response = await client.GetAsync(debugUrl);
                        if (!response.IsSuccessStatusCode)
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<object>>();
                            logger.LogError("Facebook token validation failed (HTTP error). StatusCode: {StatusCode}", response.StatusCode);
                            context.Fail("Facebook token validation failed (HTTP error).");
                            return;
                        }

                        var responseContent = await response.Content.ReadAsStringAsync();
                        using var doc = JsonDocument.Parse(responseContent);
                        var data = doc.RootElement.GetProperty("data");
                        var isValid = data.GetProperty("is_valid").GetBoolean();
                        var expiresAt = data.GetProperty("expires_at").GetInt64();
                        if (!isValid || expiresAt <= DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<object>>();
                            logger.LogWarning("Facebook token is invalid or expired.");
                            context.Fail("Facebook token is invalid or expired.");
                            return;
                        }

                        // If valid, set the token for further processing
                        context.Token = token;
                    },
                    OnTokenValidated = OnTokenValidated
                };
            })
            .AddJwtBearer("Apple", options =>
            {
                options.Authority = externalAuthSettings.Apple.Authority;
                options.Audience = externalAuthSettings.Apple.Audience;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = externalAuthSettings.Apple.Authority,
                    ValidateAudience = true,
                    ValidAudience = externalAuthSettings.Apple.Audience,
                    ValidateLifetime = true
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = OnTokenValidated
                };
            });

        return services;
    }
}
