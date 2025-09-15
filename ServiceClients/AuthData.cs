using Biz.Models;
using JetBrains.Annotations;

namespace ServiceClients;

[UsedImplicitly]
public class AuthData
{
    public string? AccessToken { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public LoginProvider? LoginProvider { get; set; }
    public Tenant? Tenant { get; set; }
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public bool IsMfa { get; set; }
}