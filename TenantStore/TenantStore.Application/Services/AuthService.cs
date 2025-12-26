namespace TenantStore.Application.Services;

using TenantStore.Application.Common;
using TenantStore.Application.DTOs.Auth;
using TenantStore.Application.Interfaces;
using TenantStore.Domain.Entities;
using TenantStore.Domain.Enums;
using TenantStore.Domain.Interfaces;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly ITenantProvider _tenantProvider;

    public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, ITenantProvider tenantProvider)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _tenantProvider = tenantProvider;
    }

    public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        if (_tenantProvider.TenantId == null)
            return Result<LoginResponseDto>.Failure("Tenant not found");

        var user = await _unitOfWork.Users.GetByEmailAsync(_tenantProvider.TenantId.Value, request.Email, cancellationToken);

        if (user == null)
            return Result<LoginResponseDto>.Failure("Invalid email or password");

        // Verify password (simplified - in real app use BCrypt or similar)
        if (!VerifyPassword(request.Password, user.PasswordHash))
            return Result<LoginResponseDto>.Failure("Invalid email or password");

        if (!user.IsActive)
            return Result<LoginResponseDto>.Failure("User account is deactivated");

        var tenant = await _unitOfWork.Tenants.GetByIdAsync(user.TenantId, cancellationToken);
        if (tenant == null || !tenant.IsActive)
            return Result<LoginResponseDto>.Failure("Tenant is inactive");

        var token = _tokenService.GenerateToken(user, tenant.Id);

        var response = new LoginResponseDto
        {
            Token = token,
            User = new UserInfoDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString()
            },
            Tenant = new TenantInfoDto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                Subdomain = tenant.Subdomain,
                ThemeColor = tenant.ThemeColor
            }
        };

        return Result<LoginResponseDto>.Success(response);
    }

    public async Task<Result<LoginResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        //Subscritpion check

        if (_tenantProvider.TenantId == null)
            return Result<LoginResponseDto>.Failure("Tenant not found");

        var tenant = await _unitOfWork.Tenants.GetByIdAsync(_tenantProvider.TenantId.Value, cancellationToken);
        if (tenant == null)
            return Result<LoginResponseDto>.Failure("Tenant not found");

        //  CHECK USER LIMIT
        var currentUserCount = await _unitOfWork.Users
            .CountAsync(u => u.TenantId == _tenantProvider.TenantId.Value && !u.IsDeleted, cancellationToken);

        if (currentUserCount >= tenant.MaxUsers)
        {
            return Result<LoginResponseDto>.Failure(
                $"User limit reached! Your {tenant.SubscriptionPlan} plan allows {tenant.MaxUsers} users. " +
                $"You currently have {currentUserCount} users. Please upgrade your subscription."
            );
        }
        //Subscription check end
       

        if (!tenant.CanAddUser())
            return Result<LoginResponseDto>.Failure("User limit reached for this subscription");

        var emailExists = await _unitOfWork.Users.EmailExistsAsync(_tenantProvider.TenantId.Value, request.Email, cancellationToken);
        if (emailExists)
            return Result<LoginResponseDto>.Failure("Email already registered");

        var passwordHash = HashPassword(request.Password);
        var user = User.Create(_tenantProvider.TenantId.Value, request.Name, request.Email, passwordHash, UserRole.User);

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _tokenService.GenerateToken(user, tenant.Id);

        var response = new LoginResponseDto
        {
            Token = token,
            User = new UserInfoDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString()
            },
            Tenant = new TenantInfoDto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                Subdomain = tenant.Subdomain,
                ThemeColor = tenant.ThemeColor
            }
        };

        return Result<LoginResponseDto>.Success(response);
    }

    private string HashPassword(string password)
    {
        // IMPORTANT: In production, use BCrypt.Net-Next or similar
        // This is simplified for demonstration
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        // IMPORTANT: In production, use BCrypt.Net-Next or similar
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}