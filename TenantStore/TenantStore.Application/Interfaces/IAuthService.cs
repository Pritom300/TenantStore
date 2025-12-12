namespace TenantStore.Application.Interfaces;

using TenantStore.Application.Common;
using TenantStore.Application.DTOs.Auth;

public interface IAuthService
{
    Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<Result<LoginResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);
}
