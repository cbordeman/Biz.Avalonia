using Biz.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Data.Models;

public class AppUser : IdentityUser
{
    public AppUser()
    {
        // Default constructor for EF
        Name = string.Empty;
        id = string.Empty; 
        email = string.Empty;
        TenantUsers = new List<TenantUser>();
    }

    public AppUser(LoginProvider loginProvider, string id, string name, string? email)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("Id cannot be null or empty.", nameof(id));
        if (id.Length < 5)
            throw new ArgumentException("Id is too short.", nameof(id));
        if (id[1] != '-')
            throw new ArgumentException("Id must start with a provider prefix, e.g., G-, M-, A-, or F-.", nameof(id));

        this.id = id;
        Name = name;
        this.email = email;
        LoginProvider = loginProvider;
        TenantUsers = new List<TenantUser>();
    }

    [Required]
    [ProtectedPersonalData]
    [StringLength(100)]
    public override string Id
    {
        get => id;
        set => id = value;
    }
    string id;
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [StringLength(100)]
    public override string? Email
    {
        get => email;
        set => email = value;
    }
    string? email;

    [ProtectedPersonalData]
    
    [Required]
    public LoginProvider? LoginProvider { get; set; }

    // Navigation property for many-to-many relationship with Tenant
    public ICollection<TenantUser>? TenantUsers { get; set; }
}