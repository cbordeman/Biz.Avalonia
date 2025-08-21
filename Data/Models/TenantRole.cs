namespace Data.Models;

public class TenantRole
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string TenantRoleName { get; init; } = null!;
    
    public ICollection<TenantUser> TenantUsers { get; set; } = new List<TenantUser>();
}