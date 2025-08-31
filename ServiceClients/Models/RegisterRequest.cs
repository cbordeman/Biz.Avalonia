using System.ComponentModel.DataAnnotations;

namespace ServiceClients.Models;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }
    
    [Required]
    [StringLength(50)]
    public string? Password { get; set; }
    
    [Required]
    [StringLength(100)]
    public string? Name { get; set; }
    
    [Required]
    [Phone]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }
    
    [Required]
    [StringLength(8)]
    public string? Extension { get; set; }
}

public class ChangeEmailRequest
{
    [Required]
    [StringLength(100)]
    public string? UserId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string? NewEmail { get; set; }
}

public class ChangePasswordRequest
{
    [Required]
    [StringLength(100)]
    public string? UserId { get; set; }
    
    [Required]
    [StringLength(50)]
    public string? CurrentPassword { get; set; }
    
    [Required]
    [StringLength(50)]
    public string? NewPassword { get; set; }
}

public class ChangeNameRequest
{
    [Required]
    [StringLength(100)]
    public string? UserId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string? NewName { get; set; }
}

public class ChangePhoneRequest
{
    [Required]
    [StringLength(100)]
    public string? UserId { get; set; }
    
    [Required]
    [StringLength(20)]
    [Phone]
    public string? NewPhoneNumber { get; set; }
    
    [Required]
    [StringLength(8)]
    public string? NewExtension { get; set; }
}
