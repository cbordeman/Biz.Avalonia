using System.Text.Encodings.Web;
using Biz.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Services.Auth
{
    public class AnonymousAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        static readonly PathString[] AnonymousPaths =
        [
            new PathString("/public"),
            new PathString("/health"),
            new PathString("/openapi"),
            new PathString("/scalar"),
            new PathString("/favicon.ico")
        ];

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Allow anonymous only on special paths
            if (AnonymousPaths.Any(p => Request.Path.StartsWithSegments(p, 
                    StringComparison.OrdinalIgnoreCase)))
                return Task.FromResult(AuthenticateResult.NoResult());

            // For other paths, fail authentication (header missing so unauthorized)
            return Task.FromResult(AuthenticateResult.Fail(
                $"Missing or invalid {nameof(LoginProvider)} header"));
        }
    }
}