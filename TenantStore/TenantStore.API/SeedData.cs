namespace TenantStore.API;

using TenantStore.Application.DTOs.Tenant;
using TenantStore.Application.Interfaces;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();

        // Check if tenants already exist
        var existingTenants = await tenantService.GetAllAsync();
        if (existingTenants.Data?.Any() == true)
            return; // Already seeded

        // Create Alpha tenant
        await tenantService.CreateAsync(new CreateTenantDto
        {
            Name = "Alpha Mart",
            Subdomain = "alpha",
            ThemeColor = "#007bff",
            AdminName = "Alpha Admin",
            AdminEmail = "admin@alpha.com",
            AdminPassword = "Admin@123"
        });

        // Create Beta tenant
        await tenantService.CreateAsync(new CreateTenantDto
        {
            Name = "Beta Store",
            Subdomain = "beta",
            ThemeColor = "#28a745",
            AdminName = "Beta Admin",
            AdminEmail = "admin@beta.com",
            AdminPassword = "Admin@123"
        });
    }
}