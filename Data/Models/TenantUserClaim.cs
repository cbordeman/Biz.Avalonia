namespace Data.Models;

[Index(nameof(ClaimName), IsUnique = true)]
public class TenantUserClaim
{
    [Key] public int Id { get; set; }

    [Required] 
    [StringLength(50)] 
    public string ClaimName { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public int TenantUserId { get; set; }

    public TenantUser TenantUser { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string? Value { get; set; }
}