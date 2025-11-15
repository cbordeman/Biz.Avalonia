using System.ComponentModel.DataAnnotations;

namespace Biz.Models.Account;

public class LoginModel
{
    [Required]
    [StringLength(50)]
    public string? Email { get; set; }
    
    [Required]
    [StringLength(50)]
    public string? Password { get; set; }
}