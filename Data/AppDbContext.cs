using Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Data;

public class AppDbContext : IdentityDbContext<AppUser> // Specify AppUser as generic parameter
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants { get; set; } = null!;
    public DbSet<AppUser> AppUsers { get; set; } = null!;
    public DbSet<TenantUserClaim> TenantUserClaims { get; set; } = null!;
    public DbSet<TenantUser> TenantUsers { get; set; } = null!;
    public DbSet<TenantRole> TenantRoles { get; set; } = null!;
    public DbSet<AppRole> AppRoles { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
}
