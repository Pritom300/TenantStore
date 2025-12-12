namespace TenantStore.Application.Services;

using TenantStore.Application.Common;
using TenantStore.Application.DTOs.Tenant;
using TenantStore.Application.Interfaces;
using TenantStore.Domain.Entities;
using TenantStore.Domain.Enums;
using TenantStore.Domain.Interfaces;

public class TenantService : ITenantService
{
    private readonly IUnitOfWork _unitOfWork;

    public TenantService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TenantDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tenant = await _unitOfWork.Tenants.GetByIdAsync(id, cancellationToken);
        if (tenant == null)
            return Result<TenantDto>.Failure("Tenant not found");

        return Result<TenantDto>.Success(MapToDto(tenant));
    }

    public async Task<Result<TenantDto>> GetBySubdomainAsync(string subdomain, CancellationToken cancellationToken = default)
    {
        var tenant = await _unitOfWork.Tenants.GetBySubdomainAsync(subdomain, cancellationToken);
        if (tenant == null)
            return Result<TenantDto>.Failure("Tenant not found");

        return Result<TenantDto>.Success(MapToDto(tenant));
    }

    public async Task<Result<List<TenantDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tenants = await _unitOfWork.Tenants.GetAllAsync(cancellationToken);
        var dtos = tenants.Select(MapToDto).ToList();
        return Result<List<TenantDto>>.Success(dtos);
    }

    public async Task<Result<TenantDto>> CreateAsync(CreateTenantDto dto, CancellationToken cancellationToken = default)
    {
        var subdomainExists = await _unitOfWork.Tenants.SubdomainExistsAsync(dto.Subdomain, cancellationToken);
        if (subdomainExists)
            return Result<TenantDto>.Failure("Subdomain already exists");

        var tenant = Tenant.Create(dto.Name, dto.Subdomain, dto.ThemeColor);
        await _unitOfWork.Tenants.AddAsync(tenant, cancellationToken);

        // Create admin user for the tenant
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.AdminPassword);
        var adminUser = User.Create(tenant.Id, dto.AdminName, dto.AdminEmail, passwordHash, UserRole.Admin);
        await _unitOfWork.Users.AddAsync(adminUser, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<TenantDto>.Success(MapToDto(tenant));
    }

    public async Task<Result> UpdateThemeAsync(Guid id, string themeColor, CancellationToken cancellationToken = default)
    {
        var tenant = await _unitOfWork.Tenants.GetByIdAsync(id, cancellationToken);
        if (tenant == null)
            return Result.Failure("Tenant not found");

        tenant.UpdateTheme(themeColor);
        _unitOfWork.Tenants.Update(tenant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private TenantDto MapToDto(Tenant tenant)
    {
        return new TenantDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Subdomain = tenant.Subdomain,
            ThemeColor = tenant.ThemeColor,
            IsActive = tenant.IsActive,
            SubscriptionPlan = tenant.SubscriptionPlan.ToString(),
            SubscriptionExpiresAt = tenant.SubscriptionExpiresAt,
            MaxProducts = tenant.MaxProducts,
            MaxUsers = tenant.MaxUsers,
            CreatedAt = tenant.CreatedAt
        };
    }
}