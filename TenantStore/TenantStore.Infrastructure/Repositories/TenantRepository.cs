//namespace TenantStore.Infrastructure.Repositories;

//using Microsoft.EntityFrameworkCore;
//using TenantStore.Domain.Entities;
//using TenantStore.Domain.Interfaces;
//using TenantStore.Infrastructure.Data;

//public class TenantRepository : Repository<Tenant>, ITenantRepository
//{
//    public TenantRepository(ApplicationDbContext context) : base(context)
//    {
//    }

//    public async Task<Tenant?> GetBySubdomainAsync(string subdomain, CancellationToken cancellationToken = default)
//    {
//        return await _dbSet
//            .FirstOrDefaultAsync(t => t.Subdomain.ToLower() == subdomain.ToLower(), cancellationToken);
//    }

//    public async Task<bool> SubdomainExistsAsync(string subdomain, CancellationToken cancellationToken = default)
//    {
//        return await _dbSet
//            .AnyAsync(t => t.Subdomain.ToLower() == subdomain.ToLower(), cancellationToken);
//    }

//    // Override to ignore tenant filter for Tenant entity itself
//    public override async Task<IEnumerable<Tenant>> GetAllAsync(CancellationToken cancellationToken = default)
//    {
//        return await _dbSet.IgnoreQueryFilters().ToListAsync(cancellationToken);
//    }

//    public override async Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
//    {
//        return await _dbSet.IgnoreQueryFilters().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
//    }
//}


using Microsoft.EntityFrameworkCore;
using TenantStore.Domain.Entities;
using TenantStore.Domain.Interfaces;
using TenantStore.Infrastructure.Data;
using TenantStore.Infrastructure.Repositories;

public class TenantRepository : Repository<Tenant>, ITenantRepository
{
    public TenantRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Tenant?> GetBySubdomainAsync(string subdomain, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .Include(t => t.Products.Where(p => !p.IsDeleted))
            .Include(t => t.Users.Where(u => !u.IsDeleted))
            .FirstOrDefaultAsync(t => t.Subdomain.ToLower() == subdomain.ToLower(), cancellationToken);
    }

    public async Task<bool> SubdomainExistsAsync(string subdomain, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .AnyAsync(t => t.Subdomain.ToLower() == subdomain.ToLower(), cancellationToken);
    }

    public override async Task<IEnumerable<Tenant>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .Include(t => t.Products.Where(p => !p.IsDeleted))
            .Include(t => t.Users.Where(u => !u.IsDeleted))
            .ToListAsync(cancellationToken);
    }

    public override async Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .Include(t => t.Products.Where(p => !p.IsDeleted))
            .Include(t => t.Users.Where(u => !u.IsDeleted))
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }
}