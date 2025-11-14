using Biz.Core;

namespace Biz.Models;

public record User(
    string Id,
    string Name,
    string Email,
    bool IsActive,
    LoginProvider? Provider,
    Tenant Tenant);