namespace TenantStore.Domain.Entities;

using TenantStore.Domain.Common;
using TenantStore.Domain.Enums;

public class User : BaseAuditableEntity
{
    public Guid TenantId { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation Property
    public virtual Tenant Tenant { get; private set; }

    // Private constructor for EF Core
    private User() { }

    // Factory method
    public static User Create(Guid tenantId, string name, string email, string passwordHash, UserRole role = UserRole.User)
    {
        var user = new User
        {
            TenantId = tenantId,
            Name = name ?? throw new ArgumentNullException(nameof(name)),
            Email = email?.ToLower() ?? throw new ArgumentNullException(nameof(email)),
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash)),
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        return user;
    }

    // Business Methods
    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string name, string email)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email?.ToLower() ?? throw new ArgumentNullException(nameof(email));
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeRole(UserRole role)
    {
        Role = role;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsAdmin() => Role == UserRole.Admin;

    public bool IsSuperAdmin() => Role == UserRole.SuperAdmin;
}
