using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Services.Config;
using Shouldly;

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
            var provider = context.Request.Headers[nameof(LoginProvider)].FirstOrDefault();

            if (Enum.TryParse<LoginProvider>(provider, true, out var parsedProvider))
            {
                return parsedProvider switch
                {
                    LoginProvider.Local => "Local",
                    LoginProvider.Google => "Google",
                    LoginProvider.Microsoft => "Microsoft",
                    LoginProvider.Facebook => "Facebook",
                    LoginProvider.Apple => "Apple",
                    _ => "Anonymous"
                };
            }

            return "Anonymous";
        };
    })
    .AddScheme<AuthenticationSchemeOptions, AnonymousAuthenticationHandler>("Anonymous", _ =>
    {
        // No options needed for anonymous
    })
    .AddJwtBearer("Local", options =>
    {
        options.Authority = externalAuthSettings.Local.Authority;
        options.Audience = externalAuthSettings.Local.Audience;
        externalAuthSettings.Local.SigningKey.ShouldNotBeNullOrWhiteSpace();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = externalAuthSettings.Local.Authority,
            ValidateAudience = true,
            ValidAudience = externalAuthSettings.Local.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(externalAuthSettings.Local.SigningKey)),
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = OnTokenValidated,
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<object>>();
                logger.LogWarning("Local JWT authentication failed: {Message}", context.Exception.Message);
                return Task.CompletedTask;
            }
        };
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
                logger.LogWarning("Microsoft authentication failed: {Message}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<object>>();
                if (context.Error != null)
                {
                    logger.LogWarning("OnChallenge error: {Error}, {Description}", context.Error, context.ErrorDescription);
                }
                return Task.CompletedTask;
            },
            OnForbidden = context =>
            {
                return Task.CompletedTask;
            }
        };
    })
    .AddJwtBearer("Facebook", options =>
    {
        options.Authority = externalAuthSettings.Facebook.Authority;
        options.Audience = externalAuthSettings.Facebook.Audience;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // Facebook tokens need custom validation
            ValidateAudience = false,
            ValidateLifetime = false // Validate manually
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
