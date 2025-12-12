namespace TenantStore.Application.Interfaces;

using TenantStore.Domain.Entities;

public interface ITokenService
{
    string GenerateToken(User user, Guid tenantId);
    Guid? ValidateToken(string token);
}