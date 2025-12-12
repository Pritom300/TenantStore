namespace TenantStore.Application.DTOs.Tenant;

public class CreateTenantDto
{
    public string Name { get; set; } = string.Empty;
    public string Subdomain { get; set; } = string.Empty;
    public string ThemeColor { get; set; } = "#007bff";

    // First admin user details
    public string AdminName { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public string AdminPassword { get; set; } = string.Empty;
}