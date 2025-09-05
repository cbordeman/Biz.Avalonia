using System.ComponentModel.DataAnnotations;
using Biz.Core;

namespace ServiceClients.Models;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }
    
    [Required]
    [StringLength(50)]
    [RegularExpression(AppConstants.PasswordRegex)]
    public string? Password { get; set; }
    
    [Required]
    [StringLength(100)]
    // Actual name, e.g. John Doe
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
    [RegularExpression(AppConstants.PasswordRegex)]
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

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }
}

public class ResetPasswordRequest
{
    [Required]
    [StringLength(100)]
    public string? Email { get; set; }

    [Required]
    [StringLength(100)]
    public string? Token { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 8)]
    [RegularExpression(AppConstants.PasswordRegex)]
    public string? Password { get; set; }
}
