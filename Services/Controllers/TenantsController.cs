namespace Services.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class TenantsController(IDbContextFactory<AppDbContext> context) 
    : ControllerBase, ITenantsApi
{
    [HttpGet(ITenantsApi.GetMyAvailablePath)]
    [Authorize]
    public async Task<Biz.Models.Tenant[]> GetMyAvailable()
    {
        var id = User.GetInternalId(User.GetLoginProvider());
        var availableTenants = await context.CreateDbContext().AppUsers
            .Include(x => x.TenantUsers!.Where(tu => tu.IsActive && tu.Tenant.IsActive))
            .ThenInclude(tu => tu.Tenant)
            .Where(x => x.Id == id)
            .SelectMany(x => x.TenantUsers!
                .Select(tu => new Biz.Models.Tenant(tu.TenantId, tu.Tenant.Name)))
            .ToArrayAsync();
        return availableTenants;
    }
        
    //[HttpGet("{id}")]
    //public async Task<ActionResult<Data.Models.Tenant>> GetTenant(int id)
    //{
    //    var tenant = await context.Tenants.FindAsync(id);

    //    if (tenant == null)
    //    {
    //        return NotFound();
    //    }

    //    return tenant;
    //}

    //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    //[HttpPut("{id}")]
    //public async Task<IActionResult> PutTenant(int id, Data.Models.Tenant tenant)
    //{
    //    if (id != tenant.Id)
    //    {
    //        return BadRequest();
    //    }

    //    context.Entry(tenant).State = EntityState.Modified;

    //    try
    //    {
    //        await context.SaveChangesAsync();
    //    }
    //    catch (DbUpdateConcurrencyException)
    //    {
    //        if (!TenantExists(id))
    //        {
    //            return NotFound();
    //        }
    //        else
    //        {
    //            throw;
    //        }
    //    }

    //    return NoContent();
    //}

    //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    //[HttpPost]
    //public async Task<ActionResult<Data.Models.Tenant>> PostTenant(Data.Models.Tenant tenant)
    //{
    //    context.Tenants.Add(tenant);
    //    await context.SaveChangesAsync();

    //    return CreatedAtAction("GetTenant", new { id = tenant.Id }, tenant);
    //}

    //[HttpDelete("{id}")]
    //public async Task<IActionResult> DeleteTenant(int id)
    //{
    //    var tenant = await context.Tenants.FindAsync(id);
    //    if (tenant == null)
    //    {
    //        return NotFound();
    //    }

    //    context.Tenants.Remove(tenant);
    //    await context.SaveChangesAsync();

    //    return NoContent();
    //}

    //bool TenantExists(int id)
    //{
    //    return context.Tenants.Any(e => e.Id == id);
    //}
}