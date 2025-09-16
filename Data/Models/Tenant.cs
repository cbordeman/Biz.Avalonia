namespace Data.Models;

public class Tenant
{
    [Key] public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required] public bool IsActive { get; set; }
    [Required] public bool IsDefault { get; set; }

    // Navigation property for many-to-many relationship with AppUser
    public ICollection<TenantUser>? TenantUsers { get; set; }
}