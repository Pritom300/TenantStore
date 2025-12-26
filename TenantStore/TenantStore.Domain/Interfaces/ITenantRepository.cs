namespace TenantStore.Domain.Interfaces;

using TenantStore.Domain.Entities;

public interface ITenantRepository : IRepository<Tenant>
{
    Task<Tenant?> GetBySubdomainAsync(string subdomain, CancellationToken cancellationToken = default);
    Task<bool> SubdomainExistsAsync(string subdomain, CancellationToken cancellationToken = default);
}