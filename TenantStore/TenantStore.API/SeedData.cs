namespace TenantStore.API;

using TenantStore.Application.DTOs.Tenant;
using TenantStore.Application.Interfaces;
using TenantStore.Domain.Entities;
using TenantStore.Domain.Enums;
using TenantStore.Domain.Interfaces;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Check if tenants already exist
        var existingTenants = await tenantService.GetAllAsync();
        if (existingTenants.Data?.Any() == true)
            return; // Already seeded

        // Create Alpha tenant
        var alphaResult = await tenantService.CreateAsync(new CreateTenantDto
        {
            Name = "Alpha Mart",
            Subdomain = "alpha",
            ThemeColor = "#007bff",
            AdminName = "Alpha Admin",
            AdminEmail = "admin@alpha.com",
            AdminPassword = "Admin@123"
        });

        // Create Beta tenant
        var betaResult = await tenantService.CreateAsync(new CreateTenantDto
        {
            Name = "Beta Store",
            Subdomain = "beta",
            ThemeColor = "#28a745",
            AdminName = "Beta Admin",
            AdminEmail = "admin@beta.com",
            AdminPassword = "Admin@123"
        });

        // Create SuperAdmin user (not tied to any specific tenant)
        if (alphaResult.IsSuccess)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("SuperAdmin@123");
            var superAdmin = User.Create(
                alphaResult.Data!.Id, // Use Alpha tenant for storage, but will have cross-tenant access
                "Super Administrator",
                "superadmin@tenantstore.com",
                passwordHash,
                UserRole.SuperAdmin
            );

            await unitOfWork.Users.AddAsync(superAdmin);
            await unitOfWork.SaveChangesAsync();
        }

        // Add sample User role to Alpha tenant
        if (alphaResult.IsSuccess)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("User@123");
            var regularUser = User.Create(
                alphaResult.Data!.Id,
                "Regular User",
                "user@alpha.com",
                passwordHash,
                UserRole.User
            );

            await unitOfWork.Users.AddAsync(regularUser);
            await unitOfWork.SaveChangesAsync();
        }
    }
}