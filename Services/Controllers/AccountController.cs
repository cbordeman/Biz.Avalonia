using Services.Converters;

namespace Services.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
//[Authorize(AuthenticationSchemes = "Microsoft")]
public class AccountController(
    IDbContextFactory<AppDbContext> dbContextFactory)
    : ControllerBase, IAccountApi
{
    [HttpGet(IAccountApi.GetMyUserInfoPath)]
    public Task<Biz.Models.User> GetMyUserInfo()
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