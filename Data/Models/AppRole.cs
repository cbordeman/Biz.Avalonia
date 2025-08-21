namespace Data.Models;

public class AppRole
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string AppRoleName { get; init; } = null!;

    public ICollection<AppUser> AppUsers { get; set; } = new List<AppUser>();
}