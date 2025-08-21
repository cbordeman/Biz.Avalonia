using Biz.Core.Models;

namespace Data.Models;

public class AppUser
{
    public AppUser()
    {
        // Default constructor for EF
        Id = string.Empty;
        Name = string.Empty;
        Email = string.Empty;
        TenantUsers = new List<TenantUser>();
    }

    public AppUser(LoginProvider loginProvider, string id, string name, string email)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("Id cannot be null or empty.", nameof(id));
        if (id.Length < 5)
            throw new ArgumentException("Id is too short.", nameof(id));
        if (id[1] != '-')
            throw new ArgumentException("Id must start with a provider prefix, e.g., G-, M-, A-, or F-.", nameof(id));

        Id = id;
        Name = name;
        Email = email;
        LoginProvider = loginProvider;
        TenantUsers = new List<TenantUser>();
    }

    [Key]
    [StringLength(100)]
    // G-{provider's id} or M- or A- or F-
    public string Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [StringLength(100)]
    public string Email { get; set; }

    [Required]
    public LoginProvider? LoginProvider { get; set; }

    // Navigation property for many-to-many relationship with Tenant
    public ICollection<TenantUser>? TenantUsers { get; set; }
}