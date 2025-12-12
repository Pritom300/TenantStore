namespace TenantStore.Application.Interfaces;

using TenantStore.Application.Common;
using TenantStore.Application.DTOs.Tenant;

public interface ITenantService
{
    Task<Result<TenantDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<TenantDto>> GetBySubdomainAsync(string subdomain, CancellationToken cancellationToken = default);
    Task<Result<List<TenantDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<TenantDto>> CreateAsync(CreateTenantDto dto, CancellationToken cancellationToken = default);
    Task<Result> UpdateThemeAsync(Guid id, string themeColor, CancellationToken cancellationToken = default);
}