using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Biz.Models;
using Biz.Models.Account;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using ServiceClients.Models;
using Services.Auth.Jwt;
using Services.Converters;
using Shouldly;

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
    public async Task<TokenResponse> Login([FromBody] LoginModel model)
    {
        var user = await userManager.FindByNameAsync(model.Username);
        if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            throw new UnauthorizedAccessException(
                $"User or password is incorrect.");
        var accessToken = jwtTokenService.GenerateAccessToken(user);
        var refreshToken = await jwtTokenService.GenerateAndStoreRefreshToken(user);

        return new TokenResponse(accessToken, refreshToken);
    }

    [HttpPost(IAccountApi.RefreshTokensPath)]
    public async Task<TokenResponse> RefreshTokens([FromBody] string requestRefreshToken)
    {
        var user = await userManager.GetUserAsync(HttpContext.User);
        user.ShouldNotBeNull();
        
        // Find the refresh token record
        var dbContext = dbContextFactory.CreateDbContext();
        var storedToken = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == requestRefreshToken &&
                                       rt.UserId == user.Id);

        if (storedToken == null || //storedToken.IsRevoked || 
            storedToken.ExpiryDate < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        if (storedToken.UserId != user.Id)
            throw new UnauthorizedAccessException("Refresh token belongs to another user.");

        // Remove old refresh token
        dbContext.RefreshTokens.Remove(storedToken);
        //storedToken.IsRevoked = true;
        await dbContext.SaveChangesAsync();

        // Generate new tokens
        var newAccessToken = jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = await jwtTokenService.GenerateAndStoreRefreshToken(user);
        
        return new TokenResponse(newAccessToken, newRefreshToken);
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
