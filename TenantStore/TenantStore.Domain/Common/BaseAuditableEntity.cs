namespace TenantStore.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }

    protected BaseAuditableEntity() : base()
    {
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }
}