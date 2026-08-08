using Core;

namespace Biz.Models;

public record User(
    string Id,
    string Name,
    string? SourceAvaRes,
    string Email,
    bool IsActive,
    LoginProvider? Provider,
    Tenant Tenant);