using Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class AppDbContext : IdentityDbContext<AppUser> // Specify AppUser as generic parameter
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<TenantUserClaim> TenantUserClaims { get; set; }
    public DbSet<TenantUser> TenantUsers { get; set; }
    public DbSet<TenantRole> TenantRoles { get; set; }
    public DbSet<AppRole> AppRoles { get; set; }

    // Optionally override OnModelCreating if customization needed
    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     base.OnModelCreating(modelBuilder);
    // }
}
