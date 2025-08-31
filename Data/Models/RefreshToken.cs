using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data.Models;

[Index(nameof(Token), IsUnique = true)]
public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Token { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string? UserId { get; set; }
    public AppUser? User { get; set; }

    [Required]
    public DateTime? CreatedAt { get; set; }
    
    [Required]
    public DateTime? ExpiryDate { get; set; }
    
    // [Required]
    // public bool IsRevoked { get; set; }
}