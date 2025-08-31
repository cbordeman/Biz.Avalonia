using Azure.Communication.Email;
using Biz.Models;
using Biz.Models.Account;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using ServiceClients.Models;
using Services.Auth.Jwt;
using Services.Converters;
using Services.Services;
using Shouldly;

// ReSharper disable UnusedParameter.Global

namespace Services.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class AccountController : ControllerBase, IAccountApi
{
    readonly UserManager<AppUser> userManager;
    readonly JwtTokenIssuer jwtTokService;
    readonly IDbContextFactory<AppDbContext> dbContextFactory;
    readonly IEmailService emailService;

    public AccountController(UserManager<AppUser> userManager,
        JwtTokenIssuer jwtTokService,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IEmailService emailService)
    {
        this.dbContextFactory = dbContextFactory;
        this.emailService = emailService;
        this.userManager = userManager;
        this.jwtTokService = jwtTokService;
    }

    [HttpPost(IAccountApi.LoginPath)]
    [AllowAnonymous]
    public async Task<TokenResponse> Login([FromBody] LoginModel model)
    {
        var user = await userManager.FindByNameAsync(model.Username);
        if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            throw new UnauthorizedAccessException(
                $"User or password is incorrect.");
        var accessToken = jwtTokService.GenerateAccessToken(user);
        var dbContext = dbContextFactory.CreateDbContext();
        var refreshToken = await jwtTokService.GenerateAndSwapRefreshToken(
            user, dbContext);

        return new TokenResponse(accessToken, refreshToken);
    }

    [HttpPost(IAccountApi.RefreshTokensPath)]
    public async Task<TokenResponse> RefreshTokens(
        [FromBody] string requestRefreshToken)
    {
        var user = await userManager.GetUserAsync(HttpContext.User);
        user.ShouldNotBeNull();
        
        // Find the refresh token record
        var dbContext = dbContextFactory.CreateDbContext();
        var storedRefreshToken = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == requestRefreshToken &&
                                       rt.UserId == user.Id);

        if (storedRefreshToken == null || //storedToken.IsRevoked || 
            storedRefreshToken.ExpiryDate < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        if (storedRefreshToken.UserId != user.Id)
            throw new UnauthorizedAccessException("Refresh token belongs to another user.");

        // Generate new tokens while removing old refresh token.
        var newAccessToken = jwtTokService.GenerateAccessToken(user);
        var newRefreshToken = await jwtTokService.GenerateAndSwapRefreshToken(
            user, dbContext, storedRefreshToken);
        
        return new TokenResponse(newAccessToken, newRefreshToken);
    }

    public async Task Register(RegisterModel model)
    {
        if (!ModelState.IsValid)
            throw new BadHttpRequestException("Invalid model state.");
        // TODO: Consider removing this message fore security reasons.
        if (await userManager.FindByNameAsync(model.UserName!) != null)
            throw new BadHttpRequestException("User already exists.");
        
        var user = new AppUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = model.Email,
            UserName = model.UserName
        };

        var result = await userManager.CreateAsync(user, model.Password!);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(error.Code, error.Description);
            throw new BadHttpRequestException(
                "Failed to register user.  Errors: " +
                string.Join(", ", result.Errors));
        }

        // TODO: Send SMS or email confirmation.
        var emailToken = await userManager.GenerateChangeEmailTokenAsync(
            user, model.Email!);
        
        await emailService.SendConfirmationEmailAsync(user, emailToken);
    }
    
    [HttpGet(IAccountApi.GetMyUserInfoPath)]
    public Task<User> GetMyUserInfo()
    {
        var ctx = dbContextFactory.CreateDbContext();
        var user = User.VerifyTenantUserIsActive(ctx);

        return Task.FromResult(user.ConvertToExternalUser());
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
