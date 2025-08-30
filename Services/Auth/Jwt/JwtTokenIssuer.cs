using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Data.Config;
using Data.Models;
using Microsoft.IdentityModel.Tokens;
using ServiceClients.Models;

namespace Services.Auth.Jwt;

public class JwtTokenIssuer(JwtIssuerSettings settings,
    IDbContextFactory<AppDbContext> dbContextFactory) 
    : IJwtTokenIssuer, IDisposable
{

    readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();

    public TokenWithExpiry GenerateAccessToken(AppUser user)
    {
        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.UserName!)
        ];

        // // Add roles as claims
        // foreach (var role in roles)
        //     claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(settings.SecurityKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                settings.AccessTokenExpirationMinutes),
            signingCredentials: creds);

        var accessTokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        
        return new TokenWithExpiry(accessTokenValue, token.ValidTo);
    }

    public async Task<TokenWithExpiry> 
        GenerateAndStoreRefreshToken(AppUser user)
    {
        var randomNumber = new byte[64];
        
        rng.GetBytes(randomNumber);
        var tokenValue = Convert.ToBase64String(randomNumber);
        
        var dbContext = dbContextFactory.CreateDbContext();
        var refreshToken = new RefreshToken
        {
            Token = tokenValue,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(
                settings.RefreshTokenExpirationDays)
        };

        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync();

        return new TokenWithExpiry(
            tokenValue, (DateTime)refreshToken.ExpiryDate);
    }
    
    public void Dispose()
    {
        rng.Dispose();
        GC.SuppressFinalize(this);
    }
}