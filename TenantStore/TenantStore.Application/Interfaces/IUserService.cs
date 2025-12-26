namespace TenantStore.Application.Interfaces;

using TenantStore.Application.Common;
using TenantStore.Application.DTOs.User;

public interface IUserService
{
    Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<List<UserDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<UserDto>> UpdateAsync(Guid id, UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task<Result> ChangeRoleAsync(Guid id, string role, CancellationToken cancellationToken = default);
}