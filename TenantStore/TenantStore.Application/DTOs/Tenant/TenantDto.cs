namespace TenantStore.Application.DTOs.Tenant;

public class TenantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Subdomain { get; set; } = string.Empty;
    public string ThemeColor { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string SubscriptionPlan { get; set; } = string.Empty;
    public DateTime? SubscriptionExpiresAt { get; set; }
    public int MaxProducts { get; set; }
    public int MaxUsers { get; set; }
    public DateTime CreatedAt { get; set; }
}