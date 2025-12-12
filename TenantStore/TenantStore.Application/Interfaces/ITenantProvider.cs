namespace TenantStore.Application.Interfaces;

public interface ITenantProvider
{
    Guid? TenantId { get; }
    string? TenantSubdomain { get; }
    void SetTenant(Guid tenantId, string subdomain);
}