namespace TenantStore.Domain.Entities;

using TenantStore.Domain.Common;
using TenantStore.Domain.Enums;

public class Subscription : BaseAuditableEntity
{
    public Guid TenantId { get; private set; }
    public SubscriptionPlan Plan { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsActive { get; private set; }
    public decimal Price { get; private set; }

    // Navigation Property
    public virtual Tenant Tenant { get; private set; }

    private Subscription() { }

    public static Subscription Create(Guid tenantId, SubscriptionPlan plan, int durationMonths, decimal price)
    {
        var subscription = new Subscription
        {
            TenantId = tenantId,
            Plan = plan,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(durationMonths),
            IsActive = true,
            Price = price,
            CreatedAt = DateTime.UtcNow
        };

        return subscription;
    }

    public void Renew(int durationMonths)
    {
        EndDate = EndDate.AddMonths(durationMonths);
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsExpired() => DateTime.UtcNow > EndDate;
}