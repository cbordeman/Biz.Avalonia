using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) 
    : DbContext(options)
{
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<TenantUserClaim> TenantUserClaims { get; set; }
    public DbSet<TenantUser> TenantUsers { get; set; }
    public DbSet<TenantRole> TenantRoles { get; set; }
    public DbSet<AppRole> AppRoles { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseSnakeCaseNamingConvention();

}