using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Biz.Models;
using Biz.Models.Account;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using ServiceClients.Models;
using Services.Auth.Jwt;
using Services.Converters;

// ReSharper disable UnusedParameter.Global

namespace Services.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class AccountController : ControllerBase, IAccountApi
{
    readonly UserManager<AppUser> userManager;
    readonly JwtTokenIssuer jwtTokenService;
    readonly IDbContextFactory<AppDbContext> dbContextFactory;

    public AccountController(UserManager<AppUser> userManager,
        JwtTokenIssuer jwtTokenService,
        IDbContextFactory<AppDbContext> dbContextFactory)
    {
        this.dbContextFactory = dbContextFactory;
        this.userManager = userManager;
        this.jwtTokenService = jwtTokenService;
    }

    [HttpPost(IAccountApi.LoginPath)]
    [AllowAnonymous]
    public async Task<LoginResponse> Login([FromBody] LoginModel model)
    {
        var user = await userManager.FindByNameAsync(model.Username);
        if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            throw new UnauthorizedAccessException(
                $"User {model.Username} is not registered or " +
                $"password is incorrect.");
        var accessToken = jwtTokenService.GenerateAccessToken(user);
        var refreshToken = await jwtTokenService.GenerateAndStoreRefreshToken(user);

        return new LoginResponse(accessToken, refreshToken);
    }

    [HttpPost(IAccountApi.RefreshAccessTokenPath)]
    public async Task<string> RefreshAccessToken([FromBody] string requestRefreshToken)
    {
        var user = await userManager.GetUserAsync(HttpContext.User);
        
        // Find the refresh token record
        var dbContext = dbContextFactory.CreateDbContext();
        var storedToken = await dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == requestRefreshToken);

        if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiryDate < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        if (storedToken.User == null || storedToken.User.Id != user.Id)
            throw new UnauthorizedAccessException("Refresh token belongs to another user.");

        // Revoke old refresh token
        storedToken.IsRevoked = true;
        await dbContext.SaveChangesAsync();

        // Generate new tokens
        var newAccessToken = jwtTokenService.GenerateAccessToken(user, roles);
        var newRefreshToken = await jwtTokenService.GenerateAndStoreRefreshToken(user);
        
        // Store new refresh token
        var refreshTokenEntity = new RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            ExpiryDate = newRefreshTokenExpiry,
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false,
        };
        dbContext.RefreshTokens.Add(refreshTokenEntity);
        await dbContext.SaveChangesAsync();

        return Ok(new TokenResponse
        {
            AccessToken = newAccessToken,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            RefreshToken = newRefreshToken,
            RefreshTokenExpiresAt = newRefreshTokenExpiry,
        });
    }

    [HttpGet(IAccountApi.GetMyUserInfoPath)]
    public Task<User> GetMyUserInfo()
    {
        var c = dbContextFactory.CreateDbContext();
        var u = User.VerifyTenantUserIsActive(c);

        return Task.FromResult(u.ConvertToExternalUser());
    }

    // [HttpGet("{id}")]
    // public string Get(int id)
    // {
    //     return "value";
    // }
    //
    // [HttpPost]
    // public void Post([FromBody] string value)
    // {
    // }
    //
    // [HttpPut("{id}")]
    // public void Put(int id, [FromBody] string value)
    // {
    // }
    //
    // [HttpDelete("{id}")]
    // public void Delete(int id)
    // {
    // }
    //
    // [HttpGet("claims")]
    // public IActionResult GetUserClaims()
    // {
    //     var claims = User.Claims.Select(c => new { c.Type, c.Value });
    //     return Ok(claims);
    // }
}
