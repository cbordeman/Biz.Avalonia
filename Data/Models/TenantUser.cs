namespace Data.Models;

[PrimaryKey(nameof(TenantId), nameof(AppUserId))]
public class TenantUser
{
    public int TenantId { get; set; }

    [Required]
    [StringLength(100)]
    public string AppUserId { get; set; } = string.Empty;

    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public AppUser AppUser { get; set; } = null!;

    // Indicates if the user is active in the tenant
    public bool IsActive { get; set; }

    public ICollection<TenantUserClaim> Claims { get; set; } = new List<TenantUserClaim>();
    public ICollection<TenantRole> Roles { get; set; } = new List<TenantRole>();

}