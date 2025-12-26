namespace TenantStore.Domain.Entities;

using TenantStore.Domain.Common;
using TenantStore.Domain.Enums;

public class Tenant : BaseAuditableEntity
{
    public string Name { get; private set; }
    public string Subdomain { get; private set; }
    public string ThemeColor { get; private set; }
    public bool IsActive { get; private set; }

    // Subscription Info
    public SubscriptionPlan SubscriptionPlan { get; private set; }
    public DateTime? SubscriptionExpiresAt { get; private set; }
    public int MaxProducts { get; private set; }
    public int MaxUsers { get; private set; }

    // Navigation Properties
    public virtual ICollection<User> Users { get; private set; }
    public virtual ICollection<Product> Products { get; private set; }

    // Private constructor for EF Core
    private Tenant()
    {
        Users = new List<User>();
        Products = new List<Product>();
    }

    // Factory method
    public static Tenant Create(string name, string subdomain, string themeColor = "#007bff")
    {
        var tenant = new Tenant
        {
            Name = name ?? throw new ArgumentNullException(nameof(name)),
            Subdomain = subdomain?.ToLower() ?? throw new ArgumentNullException(nameof(subdomain)),
            ThemeColor = themeColor ?? "#007bff",
            IsActive = true,
            SubscriptionPlan = SubscriptionPlan.Free,
            MaxProducts = 10,
            MaxUsers = 2,
            CreatedAt = DateTime.UtcNow
        };

        return tenant;
    }

    // Business Methods
    public void UpdateTheme(string themeColor)
    {
        ThemeColor = themeColor ?? throw new ArgumentNullException(nameof(themeColor));
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpgradeSubscription(SubscriptionPlan plan, int maxProducts, int maxUsers, int durationMonths)
    {
        SubscriptionPlan = plan;
        MaxProducts = maxProducts;
        MaxUsers = maxUsers;
        SubscriptionExpiresAt = DateTime.UtcNow.AddMonths(durationMonths);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool CanAddProduct()
    {
        return Products.Count(p => !p.IsDeleted) < MaxProducts;
    }

    public bool CanAddUser()
    {
        return Users.Count(u => !u.IsDeleted) < MaxUsers;
    }
}
