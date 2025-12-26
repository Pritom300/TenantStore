namespace TenantStore.Application.Services;

using TenantStore.Application.Common;
using TenantStore.Application.DTOs.User;
using TenantStore.Application.Interfaces;
using TenantStore.Domain.Entities;
using TenantStore.Domain.Enums;
using TenantStore.Domain.Interfaces;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantProvider _tenantProvider;

    public UserService(IUnitOfWork unitOfWork, ITenantProvider tenantProvider)
    {
        _unitOfWork = unitOfWork;
        _tenantProvider = tenantProvider;
    }

    public async Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
        if (user == null)
            return Result<UserDto>.Failure("User not found");

        return Result<UserDto>.Success(MapToDto(user));
    }

    public async Task<Result<List<UserDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (_tenantProvider.TenantId == null)
            return Result<List<UserDto>>.Failure("Tenant not found");

        var users = await _unitOfWork.Users.GetUsersByTenantAsync(_tenantProvider.TenantId.Value, cancellationToken);
        var dtos = users.Select(MapToDto).ToList();
        return Result<List<UserDto>>.Success(dtos);
    }

    public async Task<Result<UserDto>> UpdateAsync(Guid id, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
        if (user == null)
            return Result<UserDto>.Failure("User not found");

        user.UpdateProfile(dto.Name, dto.Email);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<UserDto>.Success(MapToDto(user));
    }

    public async Task<Result> ChangeRoleAsync(Guid id, string role, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
        if (user == null)
            return Result.Failure("User not found");

        if (!Enum.TryParse<UserRole>(role, out var userRole))
            return Result.Failure("Invalid role");

        user.ChangeRole(userRole);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }

   
}