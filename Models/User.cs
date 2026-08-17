using Core;

namespace Biz.Models;

public record User(
    string Id,
    string Name,
    string? Initials,
    string Email,
    bool IsActive,
    LoginProvider? Provider,
    Tenant Tenant)
{
    public string EmailLink => $"mailto:{Email}";
}