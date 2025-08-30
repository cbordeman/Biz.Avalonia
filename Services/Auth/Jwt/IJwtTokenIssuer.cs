using Data.Models;
using ServiceClients.Models;

namespace Services.Auth.Jwt;

public interface IJwtTokenIssuer
{
    TokenWithExpiry GenerateAccessToken(AppUser user);
}
