namespace TenantStore.Domain.Entities;

using TenantStore.Domain.Common;

public class Product : BaseAuditableEntity
{
    public Guid TenantId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation Property
    public virtual Tenant Tenant { get; private set; }

    // Private constructor for EF Core
    private Product() { }

    // Factory method
    public static Product Create(Guid tenantId, string name, decimal price, int stock, string? description = null, string? imageUrl = null)
    {
        var product = new Product
        {
            TenantId = tenantId,
            Name = name ?? throw new ArgumentNullException(nameof(name)),
            Description = description,
            ImageUrl = imageUrl,
            Price = price >= 0 ? price : throw new ArgumentException("Price cannot be negative", nameof(price)),
            Stock = stock >= 0 ? stock : throw new ArgumentException("Stock cannot be negative", nameof(stock)),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        return product;
    }

    // Business Methods
    public void UpdateDetails(string name, string? description, decimal price)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        Price = price >= 0 ? price : throw new ArgumentException("Price cannot be negative", nameof(price));
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStock(int quantity)
    {
        Stock = quantity >= 0 ? quantity : throw new ArgumentException("Stock cannot be negative", nameof(quantity));
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        Stock += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReduceStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (Stock < quantity)
            throw new InvalidOperationException("Insufficient stock");

        Stock -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetImage(string imageUrl)
    {
        ImageUrl = imageUrl;
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

    public bool IsInStock() => Stock > 0;
}