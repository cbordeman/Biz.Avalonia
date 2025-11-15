namespace Biz.Models;

public class Tenant(int tenantId, string name)
{
    public int TenantId { get; init; } = tenantId;
    public string Name { get; init; } = name;
}