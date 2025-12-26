namespace TenantStore.Infrastructure.Services;

using TenantStore.Application.Interfaces;

public class TenantProvider : ITenantProvider
{
    private Guid? _tenantId;
    private string? _tenantSubdomain;

    public Guid? TenantId => _tenantId;
    public string? TenantSubdomain => _tenantSubdomain;

    public void SetTenant(Guid tenantId, string subdomain)
    {
        _tenantId = tenantId;
        _tenantSubdomain = subdomain;
    }
}
