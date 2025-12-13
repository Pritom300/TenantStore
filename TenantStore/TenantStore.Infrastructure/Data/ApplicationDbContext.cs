namespace TenantStore.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using TenantStore.Application.Interfaces;
using TenantStore.Domain.Common;
using TenantStore.Domain.Entities;

public class ApplicationDbContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantProvider tenantProvider)
        : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Global query filter for multi-tenancy
        modelBuilder.Entity<User>().HasQueryFilter(u => u.TenantId == _tenantProvider.TenantId);
        modelBuilder.Entity<Product>().HasQueryFilter(p => p.TenantId == _tenantProvider.TenantId);
        modelBuilder.Entity<Subscription>().HasQueryFilter(s => s.TenantId == _tenantProvider.TenantId);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Automatically set audit fields
        var entries = ChangeTracker.Entries<BaseAuditableEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.CreatedBy = "System"; // TODO: Get from current user
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedBy = "System"; // TODO: Get from current user
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
