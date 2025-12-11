namespace TenantStore.Domain.Interfaces;

using TenantStore.Domain.Entities;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetActiveProductsAsync(Guid tenantId, CancellationToken cancellationToken = default);
}