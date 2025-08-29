using Biz.Models;
using Biz.Models.Account;
using Data.Models;
using Microsoft.AspNetCore.Identity;
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
    public async Task<string> Login([FromBody] LoginModel model)
    {
        var user = await userManager.FindByNameAsync(model.Username);
        if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            throw new UnauthorizedAccessException();
        var roles = await userManager.GetRolesAsync(user);
        var token = jwtTokenService.GenerateJwtToken(user, roles);
        
        return token;
    }
    
    [HttpGet(IAccountApi.GetMyUserInfoPath)]
    public Task<User> GetMyUserInfo()
    {
        var c = dbContextFactory.CreateDbContext();
        var u = User.VerifyTenantUserIsActive(c);

        return Task.FromResult(u.ConvertToExternalUser());
    }

    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }

    [HttpGet("claims")]
    public IActionResult GetUserClaims()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(claims);
    }
}