using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;

public class RefreshToken
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(64)]
    public string Token { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string? UserId { get; set; }
    public AppUser? User { get; set; }

    [Required]
    public DateTime? CreatedAt { get; set; }
    
    [Required]
    public DateTime? ExpiryDate { get; set; }
    
    [Required]
    public bool IsRevoked { get; set; }
}